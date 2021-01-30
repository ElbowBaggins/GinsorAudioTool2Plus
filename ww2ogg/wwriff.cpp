#pragma warning disable 6262
#include "wwriff.h"
#include "Bits.h"
#include "codebook.h"
#include "errors.h"
#include "stdint.h"

#include <cstring>
#include <iostream>

namespace WWise
{
    using Bits::BitStream;
    using Bits::OggBitStream;
    using Bits::ArrayStreamBuffer;
    using Bits::ArbitraryPrecisionUnsignedInt;
    using Bits::ArbitraryPrecisionUnsignedIntV;
    using Bits::Read16BitLittleEndian;
    using Bits::Read16BitBigEndian;
    using Bits::Write16BitLittleEndian;
    using Bits::Write16BitBigEndian;
    using Bits::Read32BitLittleEndian;
    using Bits::Read32BitBigEndian;
    using Bits::Write32BitLittleEndian;
    using Bits::Write32BitBigEndian;
    class Packet
    {
        long Offset;
        uint16_t Size;
        uint32_t AbsoluteGranule;
        bool NoGranule;
    public:
        Packet(istream& i, long o, bool little_endian, bool no_granule = false) : Offset(o), Size(-1), AbsoluteGranule(0), NoGranule(no_granule) {
            i.seekg(Offset);

            if (little_endian)
            {
                Size = Read16BitLittleEndian(i);
                if (!NoGranule)
                {
                    AbsoluteGranule = Read32BitLittleEndian(i);
                }
            }
            else
            {
                Size = Read16BitBigEndian(i);
                if (!NoGranule)
                {
                    AbsoluteGranule = Read32BitBigEndian(i);
                }
            }
        }

        long HeaderSize() const
        {
            return NoGranule ? 2 : 6;
        }
        long GetOffset() const
        {
            return Offset + HeaderSize();
        }
        uint16_t GetSize() const
        {
            return Size;
        }
        uint32_t Granule() const
        {
            return AbsoluteGranule;
        }
        long NextOffset() const
        {
            return Offset + HeaderSize() + Size;
        }
    };

    /* Old 8 byte header */
    class Packet8
    {
        long Offset;
        uint32_t Size;
        uint32_t AbsoluteGranule;
    public:
        Packet8(istream& i, const long o, const bool littleEndian) : Offset(o), Size(-1), AbsoluteGranule(0) {
            i.seekg(Offset);

            if (littleEndian)
            {
                Size = Read32BitLittleEndian(i);
                AbsoluteGranule = Read32BitLittleEndian(i);
            }
            else
            {
                Size = Read32BitBigEndian(i);
                AbsoluteGranule = Read32BitBigEndian(i);
            }
        }

        static long HeaderSize() { return 8; }
        long GetOffset() const
        {
            return Offset + HeaderSize();
        }
        uint32_t GetSize() const
        {
            return Size;
        }
        uint32_t Granule() const
        {
            return AbsoluteGranule;
        }
        long NextOffset() const
        {
            return Offset + HeaderSize() + Size;
        }
    };

    class VorbisPacketHeader
    {
        uint8_t Type;

        static const char VORBIS_STR[6];

    public:
        explicit VorbisPacketHeader(const uint8_t t) : Type(t) {}

        friend OggBitStream& operator << (OggBitStream& bStream, const VorbisPacketHeader& vph) {
            const ArbitraryPrecisionUnsignedInt<8> t(vph.Type);
            bStream << t;

            for (unsigned int i = 0; i < 6; i++)
            {
                ArbitraryPrecisionUnsignedInt<8> c(VORBIS_STR[i]);
                bStream << c;
            }

            return bStream;
        }
    };

    const char VorbisPacketHeader::VORBIS_STR[6] = { 'v','o','r','b','i','s' };
    OggReStreamer::OggReStreamer(
        const string& name,
        istream& wemStream,
        const string& codebooksName,
        istream& codebookStream,
        bool inlineCodebooks,
        bool fullSetup,
        ForcePacketFormat forcePacketFormat
    )
        :
        FileName(name),
        CodebooksName(codebooksName),
        Infile(wemStream),
        CodebookFile(codebookStream),
        FileSize(-1),
        LittleEndian(true),
        RiffSize(-1),
        FmtOffset(-1),
        CueOffset(-1),
        ListOffset(-1),
        SmplOffset(-1),
        VorbOffset(-1),
        DataOffset(-1),
        FmtSize(-1),
        CueSize(-1),
        ListSize(-1),
        SmplSize(-1),
        VorbSize(-1),
        DataSize(-1),
        Channels(0),
        SampleRate(0),
        AvgBytesPerSecond(0),
        ExtUnk(0),
        Subtype(0),
        CueCount(0),
        LoopCount(0),
        LoopStart(0),
        LoopEnd(0),
        SampleCount(0),
        SetupPacketOffset(0),
        FirstAudioPacketOffset(0),
        Uid(0),
        Blocksize0Pow(0),
        Blocksize1Pow(0),
        InlineCodebooks(inlineCodebooks),
        FullSetup(fullSetup),
        HeaderTriadPresent(false),
        OldPacketHeaders(false),
        NoGranule(false),
        ModPackets(false),
        Read16(nullptr),
        Read32(nullptr)
    {
        if (!Infile) throw File_open_error(name);

        Infile.seekg(0, ios::end);
        FileSize = Infile.tellg();


        // check RIFF header
        {
            unsigned char riffHead[4]{ '\0' }, head[4]{ '\0' };
            Infile.seekg(0, ios::beg);
            Infile.read(reinterpret_cast<char*>(riffHead), 4);

            if (memcmp(&riffHead[0], "RIFX", 4))
            {
                if (memcmp(&riffHead[0], "RIFF", 4))
                {
                    throw Parse_error_str("missing RIFF");
                }
                else
                {
                    LittleEndian = true;
                }
            }
            else
            {
                LittleEndian = false;
            }

            if (LittleEndian)
            {
                Read16 = Read16BitLittleEndian;
                Read32 = Read32BitLittleEndian;
            }
            else
            {
                Read16 = Read16BitBigEndian;
                Read32 = Read32BitBigEndian;
            }

            RiffSize = Read32(Infile) + 8;

            if (RiffSize > FileSize) throw Parse_error_str("RIFF truncated");

            Infile.read(reinterpret_cast<char*>(head), 4);
            if (memcmp(&head[0], "WAVE", 4)) throw Parse_error_str("missing WAVE");
        }

        // read chunks
        long chunkOffset = 12;
        while (chunkOffset < RiffSize)
        {
            Infile.seekg(chunkOffset, ios::beg);

            if (chunkOffset + 8 > RiffSize) throw Parse_error_str("chunk header truncated");

            char chunkType[4];
            Infile.read(chunkType, 4);
            uint32_t chunkSize;

            chunkSize = Read32(Infile);

            if (!memcmp(chunkType, "fmt ", 4))
            {
                FmtOffset = chunkOffset + 8;
                FmtSize = chunkSize;
            }
            else if (!memcmp(chunkType, "cue ", 4))
            {
                CueOffset = chunkOffset + 8;
                CueSize = chunkSize;
            }
            else if (!memcmp(chunkType, "LIST", 4))
            {
                ListOffset = chunkOffset + 8;
                ListSize = chunkSize;
            }
            else if (!memcmp(chunkType, "smpl", 4))
            {
                SmplOffset = chunkOffset + 8;
                SmplSize = chunkSize;
            }
            else if (!memcmp(chunkType, "vorb", 4))
            {
                VorbOffset = chunkOffset + 8;
                VorbSize = chunkSize;
            }
            else if (!memcmp(chunkType, "data", 4))
            {
                DataOffset = chunkOffset + 8;
                DataSize = chunkSize;
            }

            chunkOffset = chunkOffset + 8 + chunkSize;
        }

        if (chunkOffset > RiffSize) throw Parse_error_str("chunk truncated");

        // check that we have the chunks we're expecting
        if (-1 == FmtOffset && -1 == DataOffset) throw Parse_error_str("expected fmt, data chunks");

        // read fmt
        if (-1 == VorbOffset && 0x42 != FmtSize) throw Parse_error_str("expected 0x42 fmt if vorb missing");

        if (-1 != VorbOffset && 0x28 != FmtSize && 0x18 != FmtSize && 0x12 != FmtSize) throw Parse_error_str("bad fmt size");

        if (-1 == VorbOffset && 0x42 == FmtSize)
        {
            // fake it out
            VorbOffset = FmtOffset + 0x18;
        }

        Infile.seekg(FmtOffset, ios::beg);
        if (UINT16_C(0xFFFF) != Read16(Infile)) throw Parse_error_str("bad codec id");
        Channels = Read16(Infile);
        SampleRate = Read32(Infile);
        AvgBytesPerSecond = Read32(Infile);
        if (0U != Read16(Infile)) throw Parse_error_str("bad block align");
        if (0U != Read16(Infile)) throw Parse_error_str("expected 0 bps");
        if (FmtSize - 0x12 != Read16(Infile)) throw Parse_error_str("bad extra fmt length");

        if (FmtSize - 0x12 >= 2) {
            // read extra fmt
            ExtUnk = Read16(Infile);
            if (FmtSize - 0x12 >= 6) {
                Subtype = Read32(Infile);
            }
        }

        if (FmtSize == 0x28)
        {
            char whoKnowsBuf[16];
            const unsigned char whoKnowsBufCheck[16] = { 1,0,0,0, 0,0,0x10,0, 0x80,0,0,0xAA, 0,0x38,0x9b,0x71 };
            Infile.read(whoKnowsBuf, 16);
            if (memcmp(whoKnowsBuf, whoKnowsBufCheck, 16)) throw Parse_error_str("expected signature in extra fmt?");
        }

        // read cue
        if (-1 != CueOffset)
        {
            Infile.seekg(CueOffset);

            CueCount = Read32(Infile);
        }

        // read LIST
        if (-1 != ListOffset)
        {
#if 0
            if (4 != _LIST_size) throw Parse_error_str("bad LIST size");
            char adtlbuf[4];
            const char adtlbuf_check[4] = { 'a','d','t','l' };
            _infile.seekg(_LIST_offset);
            _infile.read(adtlbuf, 4);
            if (memcmp(adtlbuf, adtlbuf_check, 4)) throw Parse_error_str("expected only adtl in LIST");
#endif
        }

        // read smpl
        if (-1 != SmplOffset)
        {
            Infile.seekg(SmplOffset + 0x1C);
            LoopCount = Read32(Infile);

            if (1 != LoopCount) throw Parse_error_str("expected one loop");

            Infile.seekg(SmplOffset + 0x2c);
            LoopStart = Read32(Infile);
            LoopEnd = Read32(Infile);
        }

        // read vorb
        switch (VorbSize)
        {
        case -1:
        case 0x28:
        case 0x2A:
        case 0x2C:
        case 0x32:
        case 0x34:
            Infile.seekg(VorbOffset + 0x00, ios::beg);
            break;

        default:
            throw Parse_error_str("bad vorb size");
        }

        SampleCount = Read32(Infile);

        switch (VorbSize)
        {
        case -1:
        case 0x2A:
        {
            NoGranule = true;

            Infile.seekg(VorbOffset + 0x4, ios::beg);
            uint32_t modSignal = Read32(Infile);

            // set
            // D9     11011001
            // CB     11001011
            // BC     10111100
            // B2     10110010
            // unset
            // 4A     01001010
            // 4B     01001011
            // 69     01101001
            // 70     01110000
            // A7     10100111 !!!

            // seems to be 0xD9 when _mod_packets should be set
            // also seen 0xCB, 0xBC, 0xB2
            if (0x4A != modSignal && 0x4B != modSignal && 0x69 != modSignal && 0x70 != modSignal)
            {
                ModPackets = true;
            }
            Infile.seekg(VorbOffset + 0x10, ios::beg);
            break;
        }

        default:
            Infile.seekg(VorbOffset + 0x18, ios::beg);
            break;
        }

        if (forcePacketFormat == ForceNoModPackets)
        {
            ModPackets = false;
        }
        else if (forcePacketFormat == ForceModPackets)
        {
            ModPackets = true;
        }

        SetupPacketOffset = Read32(Infile);
        FirstAudioPacketOffset = Read32(Infile);

        switch (VorbSize)
        {
        default:
        case -1:
        case 0x2A:
            Infile.seekg(VorbOffset + 0x24, ios::beg);
            break;

        case 0x32:
        case 0x34:
            Infile.seekg(VorbOffset + 0x2C, ios::beg);
            break;
        }

        switch (VorbSize)
        {
        case 0x28:
        case 0x2C:
            // ok to leave _uid, _blocksize_0_pow and _blocksize_1_pow unset
            HeaderTriadPresent = true;
            OldPacketHeaders = true;
            break;
        default:
        case -1:
        case 0x2A:
        case 0x32:
        case 0x34:
            Uid = Read32(Infile);
            Blocksize0Pow = Infile.get();
            Blocksize1Pow = Infile.get();
            break;
        }

        // check/set loops now that we know total sample count
        if (0 != LoopCount)
        {
            if (LoopEnd == 0)
            {
                LoopEnd = SampleCount;
            }
            else
            {
                LoopEnd = LoopEnd + 1;
            }

            if (LoopStart >= SampleCount || LoopEnd > SampleCount || LoopStart > LoopEnd)
                throw Parse_error_str("loops out of range");
        }

        // check subtype now that we know the vorb info
        // this is clearly just the channel layout
        switch (Subtype)
        {
        case 4:     /* 1 channel, no seek table */
        case 3:     /* 2 channels */
        case 0x33:  /* 4 channels */
        case 0x37:  /* 5 channels, seek or not */
        case 0x3b:  /* 5 channels, no seek table */
        case 0x3f:  /* 6 channels, no seek table */
            break;
        default:
            //throw Parse_error_str("unknown subtype");
            break;
        }
    }

    void OggReStreamer::PrintInfo(void) const
    {
        if (LittleEndian)
        {
            cout << "RIFF WAVE";
        }
        else
        {
            cout << "RIFX WAVE";
        }
        cout << " " << Channels << " channel";
        if (Channels != 1) cout << "s";
        cout << " " << SampleRate << " Hz " << AvgBytesPerSecond * 8 << " bps" << endl;
        cout << SampleCount << " samples" << endl;

        if (0 != LoopCount)
        {
            cout << "loop from " << LoopStart << " to " << LoopEnd << endl;
        }

        if (OldPacketHeaders)
        {
            cout << "- 8 byte (old) packet headers" << endl;
        }
        else if (NoGranule)
        {
            cout << "- 2 byte packet headers, no granule" << endl;
        }
        else
        {
            cout << "- 6 byte packet headers" << endl;
        }

        if (HeaderTriadPresent)
        {
            cout << "- Vorbis header triad present" << endl;
        }

        if (FullSetup || HeaderTriadPresent)
        {
            cout << "- full setup header" << endl;
        }
        else
        {
            cout << "- stripped setup header" << endl;
        }

        if (InlineCodebooks || HeaderTriadPresent)
        {
            cout << "- inline codebooks" << endl;
        }
        else
        {
            cout << "- external codebooks (" << CodebooksName << ")" << endl;
        }

        if (ModPackets)
        {
            cout << "- modified Vorbis packets" << endl;
        }
        else
        {
            cout << "- standard Vorbis packets" << endl;
        }
    }

    void OggReStreamer::GenerateOggHeader(OggBitStream& os, bool*& modeBlockFlag, int& modeBits) const
    {
        // generate identification packet
        {
            VorbisPacketHeader vorbisPacketHeader(1);

            os << vorbisPacketHeader;

            ArbitraryPrecisionUnsignedInt<32> version(0);
            os << version;

            ArbitraryPrecisionUnsignedInt<8> ch(Channels);
            os << ch;

            ArbitraryPrecisionUnsignedInt<32> sampleRate(SampleRate);
            os << sampleRate;

            ArbitraryPrecisionUnsignedInt<32> bitRateMax(0);
            os << bitRateMax;

            ArbitraryPrecisionUnsignedInt<32> bitRateNominal(AvgBytesPerSecond * 8);
            os << bitRateNominal;

            ArbitraryPrecisionUnsignedInt<32> bitRateMinimum(0);
            os << bitRateMinimum;

            ArbitraryPrecisionUnsignedInt<4> blockSize0(Blocksize0Pow);
            os << blockSize0;

            ArbitraryPrecisionUnsignedInt<4> blockSize1(Blocksize1Pow);
            os << blockSize1;

            ArbitraryPrecisionUnsignedInt<1> framing(1);
            os << framing;

            // identification packet on its own page
            os.FlushPage();
        }

        // generate comment packet
        {
            VorbisPacketHeader vorbisPacketHeader(3);

            os << vorbisPacketHeader;

            static const char VENDOR[] = "Converted from AudioKinetic WWise by ww2ogg 0.25.0.x (GinsorAudioTool2Plus)";
            ArbitraryPrecisionUnsignedInt<32> vendorSize(75);

            os << vendorSize;
            for (size_t i = 0; i < vendorSize; i++) {
                ArbitraryPrecisionUnsignedInt<8> c(VENDOR[i]);
                os << c;
            }

            if (0 == LoopCount)
            {
                // no user comments
                ArbitraryPrecisionUnsignedInt<32> userCommentCount(0);
                os << userCommentCount;
            }
            else
            {
                // two comments, loop start and end
                ArbitraryPrecisionUnsignedInt<32> userCommentCount(2);
                os << userCommentCount;

                stringstream loopStartStr;
                stringstream loopEndStr;

                loopStartStr << "LoopStart=" << LoopStart;
                loopEndStr << "LoopEnd=" << LoopEnd;

                ArbitraryPrecisionUnsignedInt<32> loopStartCommentLength;
                loopStartCommentLength = static_cast<unsigned int>(loopStartStr.str().length());
                os << loopStartCommentLength;
                for (unsigned int i = 0; i < loopStartCommentLength; i++)
                {
                    ArbitraryPrecisionUnsignedInt<8> c(loopStartStr.str().c_str()[i]);
                    os << c;
                }

                ArbitraryPrecisionUnsignedInt<32> loopEndCommentLength;
                loopEndCommentLength = static_cast<unsigned int>(loopEndStr.str().length());
                os << loopEndCommentLength;
                for (unsigned int i = 0; i < loopEndCommentLength; i++)
                {
                    ArbitraryPrecisionUnsignedInt<8> c(loopEndStr.str().c_str()[i]);
                    os << c;
                }
            }

            ArbitraryPrecisionUnsignedInt<1> framing(1);
            os << framing;

            //os.flush_bits();
            os.FlushPage();
        }

        // generate setup packet
        {
            VorbisPacketHeader vorbisPacketHeader(5);

            os << vorbisPacketHeader;

            Packet setupPacket(Infile, DataOffset + SetupPacketOffset, LittleEndian, NoGranule);

            Infile.seekg(setupPacket.GetOffset());
            if (setupPacket.Granule() != 0) throw Parse_error_str("setup packet granule != 0");
            BitStream ss(Infile);

            // codebook count
            ArbitraryPrecisionUnsignedInt<8> codebookCountLess1;
            ss >> codebookCountLess1;
            auto codebookCount = codebookCountLess1 + 1;
            os << codebookCountLess1;

            // rebuild codebooks
            if (InlineCodebooks)
            {
                codebook_library cbl;

                for (unsigned int i = 0; i < codebookCount; i++)
                {
                    if (FullSetup)
                    {
                        cbl.copy(ss, os);
                    }
                    else
                    {
                        cbl.rebuild(ss, 0, os);
                    }
                }
            }
            else
            {
                /* external codebooks */

                codebook_library cbl(CodebooksName, CodebookFile);

                for (unsigned int i = 0; i < codebookCount; i++)
                {
                    ArbitraryPrecisionUnsignedInt<10> codebookId;
                    ss >> codebookId;
                    //cout << "Codebook " << i << " = " << codebook_id << endl;
                    try
                    {
                        cbl.rebuild(codebookId, os);
                    }
                    catch (Invalid_id e)
                    {
                        //         B         C         V
                        //    4    2    4    3    5    6
                        // 0100 0010 0100 0011 0101 0110
                        // \_______|____ ___|/
                        //              X
                        //            11 0100 0010

                        if (codebookId == 0x342)
                        {
                            ArbitraryPrecisionUnsignedInt<14> codebookIdentifier;
                            ss >> codebookIdentifier;

                            //         B         C         V
                            //    4    2    4    3    5    6
                            // 0100 0010 0100 0011 0101 0110
                            //           \_____|_ _|_______/
                            //                   X
                            //         01 0101 10 01 0000
                            if (codebookIdentifier == 0x1590)
                            {
                                // starts with BCV, probably --full-setup
                                throw Parse_error_str(
                                    "invalid codebook id 0x342, try --full-setup");
                            }
                        }

                        // just an invalid codebook
                        throw;
                    }
                }
            }

            // Time Domain transforms (placeholder)
            ArbitraryPrecisionUnsignedInt<6> timeCountLess1(0);
            os << timeCountLess1;
            ArbitraryPrecisionUnsignedInt<16> dummyTimeValue(0);
            os << dummyTimeValue;

            if (FullSetup)
            {
                while (ss.GetTotalBitsRead() < setupPacket.GetSize() * 8u)
                {
                    ArbitraryPrecisionUnsignedInt<1> bit;
                    ss >> bit;
                    os << bit;
                }
            }
            else    // _full_setup
            {
                // floor count
                ArbitraryPrecisionUnsignedInt<6> floorCountLess1;
                ss >> floorCountLess1;
                unsigned int floorCount = floorCountLess1 + 1;
                os << floorCountLess1;

                // rebuild floors
                for (unsigned int i = 0; i < floorCount; i++)
                {
                    // Always floor type 1
                    ArbitraryPrecisionUnsignedInt<16> floorType(1);
                    os << floorType;

                    ArbitraryPrecisionUnsignedInt<5> floor1Partitions;
                    ss >> floor1Partitions;
                    os << floor1Partitions;

                    auto floor1PartitionClassList = new unsigned int[floor1Partitions];

                    unsigned int maximumClass = 0;
                    for (unsigned int j = 0; j < floor1Partitions; j++)
                    {
                        ArbitraryPrecisionUnsignedInt<4> floor1PartitionClass;
                        ss >> floor1PartitionClass;
                        os << floor1PartitionClass;

                        floor1PartitionClassList[j] = floor1PartitionClass;

                        if (floor1PartitionClass > maximumClass)
                            maximumClass = floor1PartitionClass;
                    }

                    unsigned int* floor1ClassDimensionsList = new unsigned int[maximumClass + 1];

                    for (unsigned int j = 0; j <= maximumClass; j++)
                    {
                        ArbitraryPrecisionUnsignedInt<3> classDimensionsLess1;
                        ss >> classDimensionsLess1;
                        os << classDimensionsLess1;

                        floor1ClassDimensionsList[j] = classDimensionsLess1 + 1;

                        ArbitraryPrecisionUnsignedInt<2> classSubclasses;
                        ss >> classSubclasses;
                        os << classSubclasses;

                        if (0 != classSubclasses)
                        {
                            ArbitraryPrecisionUnsignedInt<8> masterBook;
                            ss >> masterBook;
                            os << masterBook;

                            if (masterBook >= codebookCount)
                                throw Parse_error_str("invalid floor1 master book");
                        }

                        for (unsigned int k = 0; k < (1U << classSubclasses); k++)
                        {
                            ArbitraryPrecisionUnsignedInt<8> subclassBookPlus1;
                            ss >> subclassBookPlus1;
                            os << subclassBookPlus1;

                            auto subclassBook = static_cast<int>(subclassBookPlus1) - 1;
                            if (subclassBook >= 0 && static_cast<unsigned int>(subclassBook) >= codebookCount)
                                throw Parse_error_str("invalid floor1 subclass book");
                        }
                    }

                    ArbitraryPrecisionUnsignedInt<2> floor1MultiplierLess1;
                    ss >> floor1MultiplierLess1;
                    os << floor1MultiplierLess1;

                    ArbitraryPrecisionUnsignedInt<4> rangeBits;
                    ss >> rangeBits;
                    os << rangeBits;

                    for (unsigned int j = 0; j < floor1Partitions; j++)
                    {
                        auto currentClassNumber = floor1PartitionClassList[j];
                        for (unsigned int k = 0; k < floor1ClassDimensionsList[currentClassNumber]; k++)
                        {
                            ArbitraryPrecisionUnsignedIntV bitUintV(rangeBits);
                            ss >> bitUintV;
                            os << bitUintV;
                        }
                    }

                    delete[] floor1ClassDimensionsList;
                    delete[] floor1PartitionClassList;
                }

                // residue count
                ArbitraryPrecisionUnsignedInt<6> residueCountLess1;
                ss >> residueCountLess1;
                auto residueCount = residueCountLess1 + 1;
                os << residueCountLess1;

                // rebuild residues
                for (unsigned int i = 0; i < residueCount; i++)
                {
                    ArbitraryPrecisionUnsignedInt<2> residueType;
                    ss >> residueType;
                    os << ArbitraryPrecisionUnsignedInt<16>(residueType);

                    if (residueType > 2) throw Parse_error_str("invalid residue type");

                    ArbitraryPrecisionUnsignedInt<24> residueBegin, residueEnd, residuePartitionSizeLess1;
                    ArbitraryPrecisionUnsignedInt<6> residueClassificationsLess1;
                    ArbitraryPrecisionUnsignedInt<8> residueClassbook;

                    ss >> residueBegin >> residueEnd >> residuePartitionSizeLess1 >> residueClassificationsLess1 >> residueClassbook;
                    unsigned int residueClassifications = residueClassificationsLess1 + 1;
                    os << residueBegin << residueEnd << residuePartitionSizeLess1 << residueClassificationsLess1 << residueClassbook;

                    if (residueClassbook >= codebookCount) throw Parse_error_str("invalid residue classbook");

                    unsigned int* residueCascade = new unsigned int[residueClassifications];

                    for (unsigned int j = 0; j < residueClassifications; j++)
                    {
                        ArbitraryPrecisionUnsignedInt<5> highBits(0);
                        ArbitraryPrecisionUnsignedInt<3> lowBits;

                        ss >> lowBits;
                        os << lowBits;

                        ArbitraryPrecisionUnsignedInt<1> bit;
                        ss >> bit;
                        os << bit;
                        if (bit)
                        {
                            ss >> highBits;
                            os << highBits;
                        }

                        residueCascade[j] = highBits * 8 + lowBits;
                    }

                    for (unsigned int j = 0; j < residueClassifications; j++)
                    {
                        for (unsigned int k = 0; k < 8; k++)
                        {
                            if (residueCascade[j] & (1 << k))
                            {
                                ArbitraryPrecisionUnsignedInt<8> residueBook;
                                ss >> residueBook;
                                os << residueBook;

                                if (residueBook >= codebookCount) throw Parse_error_str("invalid residue book");
                            }
                        }
                    }

                    delete[] residueCascade;
                }

                // mapping count
                ArbitraryPrecisionUnsignedInt<6> mappingCountLess1;
                ss >> mappingCountLess1;
                auto mappingCount = mappingCountLess1 + 1;
                os << mappingCountLess1;

                for (unsigned int i = 0; i < mappingCount; i++)
                {
                    // always mapping type 0, the only one
                    ArbitraryPrecisionUnsignedInt<16> mappingType(0);

                    os << mappingType;

                    ArbitraryPrecisionUnsignedInt<1> subMapsFlag;
                    ss >> subMapsFlag;
                    os << subMapsFlag;

                    unsigned int subMaps = 1;
                    if (subMapsFlag)
                    {
                        ArbitraryPrecisionUnsignedInt<4> subMapsLess1;

                        ss >> subMapsLess1;
                        subMaps = subMapsLess1 + 1;
                        os << subMapsLess1;
                    }

                    ArbitraryPrecisionUnsignedInt<1> squarePolarFlag;
                    ss >> squarePolarFlag;
                    os << squarePolarFlag;

                    if (squarePolarFlag)
                    {
                        ArbitraryPrecisionUnsignedInt<8> couplingStepsLess1;
                        ss >> couplingStepsLess1;
                        auto couplingSteps = couplingStepsLess1 + 1;
                        os << couplingStepsLess1;

                        for (unsigned int j = 0; j < couplingSteps; j++)
                        {
                            ArbitraryPrecisionUnsignedIntV magnitude(ilog(Channels - 1)), angle(ilog(Channels - 1));

                            ss >> magnitude >> angle;
                            os << magnitude << angle;

                            if (angle == magnitude || magnitude >= Channels || angle >= Channels) throw Parse_error_str("invalid coupling");
                        }
                    }

                    // a rare reserved field not removed by Ak!
                    ArbitraryPrecisionUnsignedInt<2> mappingReserved;
                    ss >> mappingReserved;
                    os << mappingReserved;
                    if (0 != mappingReserved) throw Parse_error_str("mapping reserved field nonzero");

                    if (subMaps > 1)
                    {
                        for (unsigned int j = 0; j < Channels; j++)
                        {
                            ArbitraryPrecisionUnsignedInt<4> mappingMux;
                            ss >> mappingMux;
                            os << mappingMux;

                            if (mappingMux >= subMaps) throw Parse_error_str("mapping_mux >= submaps");
                        }
                    }

                    for (unsigned int j = 0; j < subMaps; j++)
                    {
                        // Another! Unused time domain transform configuration placeholder!
                        ArbitraryPrecisionUnsignedInt<8> timeConfig;
                        ss >> timeConfig;
                        os << timeConfig;

                        ArbitraryPrecisionUnsignedInt<8> floorNumber;
                        ss >> floorNumber;
                        os << floorNumber;
                        if (floorNumber >= floorCount) throw Parse_error_str("invalid floor mapping");

                        ArbitraryPrecisionUnsignedInt<8> residueNumber;
                        ss >> residueNumber;
                        os << residueNumber;
                        if (residueNumber >= residueCount) throw Parse_error_str("invalid residue mapping");
                    }
                }

                // mode count
                ArbitraryPrecisionUnsignedInt<6> modeCountLess1;
                ss >> modeCountLess1;
                unsigned int modeCount = modeCountLess1 + 1;
                os << modeCountLess1;

                modeBlockFlag = new bool[modeCount];
                modeBits = ilog(modeCount - 1);

                //cout << mode_count << " modes" << endl;

                for (unsigned int i = 0; i < modeCount; i++)
                {
                    ArbitraryPrecisionUnsignedInt<1> blockFlag;
                    ss >> blockFlag;
                    os << blockFlag;

                    modeBlockFlag[i] = (blockFlag != 0);

                    // only 0 valid for windowtype and transformtype
                    ArbitraryPrecisionUnsignedInt<16> windowType(0), transformType(0);
                    os << windowType << transformType;

                    ArbitraryPrecisionUnsignedInt<8> mapping;
                    ss >> mapping;
                    os << mapping;
                    if (mapping >= mappingCount) throw Parse_error_str("invalid mode mapping");
                }

                ArbitraryPrecisionUnsignedInt<1> framing(1);
                os << framing;

            } // _full_setup

            os.FlushPage();

            if ((ss.GetTotalBitsRead() + 7) / 8 != setupPacket.GetSize()) throw Parse_error_str("didn't read exactly setup packet");

            if (setupPacket.NextOffset() != DataOffset + static_cast<long>(FirstAudioPacketOffset)) throw Parse_error_str("first audio packet doesn't follow setup packet");

        }
    }

    void OggReStreamer::GenerateOgg(ostream& of) const
    {
        OggBitStream os(of);

        bool* modeBlockFlag = nullptr;
        auto modeBits = 0;
        auto prevBlockFlag = false;

        if (HeaderTriadPresent)
        {
            GenerateOggHeaderWithTriad(os);
        }
        else
        {
            GenerateOggHeader(os, modeBlockFlag, modeBits);
        }

        // Audio pages
        {
            long offset = DataOffset + FirstAudioPacketOffset;

            while (offset < DataOffset + DataSize)
            {
                uint32_t size, granule;
                long packetHeaderSize, packetPayloadOffset, nextOffset;

                if (OldPacketHeaders)
                {
                    Packet8 audioPacket(Infile, offset, LittleEndian);
                    packetHeaderSize = audioPacket.HeaderSize();
                    size = audioPacket.GetSize();
                    packetPayloadOffset = audioPacket.GetOffset();
                    granule = audioPacket.Granule();
                    nextOffset = audioPacket.NextOffset();
                }
                else
                {
                    Packet audioPacket(Infile, offset, LittleEndian, NoGranule);
                    packetHeaderSize = audioPacket.HeaderSize();
                    size = audioPacket.GetSize();
                    packetPayloadOffset = audioPacket.GetOffset();
                    granule = audioPacket.Granule();
                    nextOffset = audioPacket.NextOffset();
                }

                if (offset + packetHeaderSize > DataOffset + DataSize) {
                    throw Parse_error_str("page header truncated");
                }

                offset = packetPayloadOffset;

                Infile.seekg(offset);
                // HACK: don't know what to do here
                if (granule == UINT32_C(0xFFFFFFFF))
                {
                    os.SetGranule(1);
                }
                else
                {
                    os.SetGranule(granule);
                }

                // first byte
                if (ModPackets)
                {
                    // need to rebuild packet type and window info

                    if (!modeBlockFlag)
                    {
                        throw Parse_error_str("didn't load mode_blockflag");
                    }

                    // OUT: 1 bit packet type (0 == audio)
                    ArbitraryPrecisionUnsignedInt<1> packetType(0);
                    os << packetType;

                    ArbitraryPrecisionUnsignedIntV* modeNumberP;
                    ArbitraryPrecisionUnsignedIntV* remainderP;

                    {
                        // collect mode number from first byte

                        BitStream ss(Infile);

                        // IN/OUT: N bit mode number (max 6 bits)
                        modeNumberP = new ArbitraryPrecisionUnsignedIntV(modeBits);
                        ss >> *modeNumberP;
                        os << *modeNumberP;

                        // IN: remaining bits of first (input) byte
                        remainderP = new ArbitraryPrecisionUnsignedIntV(8 - modeBits);
                        ss >> *remainderP;
                    }

                    if (modeBlockFlag[*modeNumberP])
                    {
                        // long window, peek at next frame

                        Infile.seekg(nextOffset);
                        auto nextBlockFlag = false;
                        if (nextOffset + packetHeaderSize <= DataOffset + DataSize)
                        {

                            // mod_packets always goes with 6-byte headers
                            Packet audioPacket(Infile, nextOffset, LittleEndian, NoGranule);
                            const uint32_t nextPacketSize = audioPacket.GetSize();
                            if (nextPacketSize > 0)
                            {
                                Infile.seekg(audioPacket.GetOffset());

                                BitStream ss(Infile);
                                ArbitraryPrecisionUnsignedIntV nextModeNumber(modeBits);

                                ss >> nextModeNumber;

                                nextBlockFlag = modeBlockFlag[nextModeNumber];
                            }
                        }

                        // OUT: previous window type bit
                        ArbitraryPrecisionUnsignedInt<1> prevWindowType(prevBlockFlag);
                        os << prevWindowType;

                        // OUT: next window type bit
                        ArbitraryPrecisionUnsignedInt<1> nextWindowType(nextBlockFlag);
                        os << nextWindowType;

                        // fix seek for rest of stream
                        Infile.seekg(offset + 1);
                    }

                    prevBlockFlag = modeBlockFlag[*modeNumberP];
                    delete modeNumberP;

                    // OUT: remaining bits of first (input) byte
                    os << *remainderP;
                    delete remainderP;
                }
                else
                {
                    // nothing unusual for first byte
                    int v = Infile.get();
                    if (v < 0)
                    {
                        throw Parse_error_str("file truncated");
                    }
                    ArbitraryPrecisionUnsignedInt<8> c(v);
                    os << c;
                }

                // remainder of packet
                for (unsigned int i = 1; i < size; i++)
                {
                    int v = Infile.get();
                    if (v < 0)
                    {
                        throw Parse_error_str("file truncated");
                    }
                    ArbitraryPrecisionUnsignedInt<8> c(v);
                    os << c;
                }

                offset = nextOffset;
                os.FlushPage(false, (offset == DataOffset + DataSize));
            }
            if (offset > DataOffset + DataSize) throw Parse_error_str("page truncated");
        }

        delete[] modeBlockFlag;
    }

    void OggReStreamer::GenerateOggHeaderWithTriad(OggBitStream& os) const
    {
        // Header page triad
        {
            long offset = DataOffset + SetupPacketOffset;

            // copy information packet
            {
                Packet8 information_packet(Infile, offset, LittleEndian);
                uint32_t size = information_packet.GetSize();

                if (information_packet.Granule() != 0)
                {
                    throw Parse_error_str("information packet granule != 0");
                }

                Infile.seekg(information_packet.GetOffset());

                ArbitraryPrecisionUnsignedInt<8> c(Infile.get());
                if (1 != c)
                {
                    throw Parse_error_str("wrong type for information packet");
                }

                os << c;

                for (unsigned int i = 1; i < size; i++)
                {
                    c = Infile.get();
                    os << c;
                }

                // identification packet on its own page
                os.FlushPage();

                offset = information_packet.NextOffset();
            }

            // copy comment packet 
            {
                Packet8 comment_packet(Infile, offset, LittleEndian);
                uint16_t size = comment_packet.GetSize();

                if (comment_packet.Granule() != 0)
                {
                    throw Parse_error_str("comment packet granule != 0");
                }

                Infile.seekg(comment_packet.GetOffset());

                ArbitraryPrecisionUnsignedInt<8> c(Infile.get());
                if (3 != c)
                {
                    throw Parse_error_str("wrong type for comment packet");
                }

                os << c;

                for (unsigned int i = 1; i < size; i++)
                {
                    c = Infile.get();
                    os << c;
                }

                // identification packet on its own page
                os.FlushPage();

                offset = comment_packet.NextOffset();
            }

            // copy setup packet
            {
                Packet8 setup_packet(Infile, offset, LittleEndian);

                Infile.seekg(setup_packet.GetOffset());
                if (setup_packet.Granule() != 0) throw Parse_error_str("setup packet granule != 0");
                Bits::BitStream ss(Infile);

                ArbitraryPrecisionUnsignedInt<8> c;
                ss >> c;

                // type
                if (5 != c)
                {
                    throw Parse_error_str("wrong type for setup packet");
                }
                os << c;

                // 'vorbis'
                for (unsigned int i = 0; i < 6; i++)
                {
                    ss >> c;
                    os << c;
                }

                // codebook count
                ArbitraryPrecisionUnsignedInt<8> codebookCountLess1;
                ss >> codebookCountLess1;
                const auto codebookCount = codebookCountLess1 + 1;
                os << codebookCountLess1;

                codebook_library cbl;

                // rebuild codebooks
                for (unsigned int i = 0; i < codebookCount; i++)
                {
                    cbl.copy(ss, os);
                }

                while (ss.GetTotalBitsRead() < setup_packet.GetSize() * 8u)
                {
                    ArbitraryPrecisionUnsignedInt<1> bit;
                    ss >> bit;
                    os << bit;
                }

                os.FlushPage();

                offset = setup_packet.NextOffset();
            }

            if (offset != DataOffset + static_cast<long>(FirstAudioPacketOffset)) throw Parse_error_str("first audio packet doesn't follow setup packet");

        }

    }

}

/* Modern 2 or 6 byte header */

