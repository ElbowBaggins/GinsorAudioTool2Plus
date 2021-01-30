#pragma once

#include "Bits.h"
#include "stdint.h"

#include <string>

using namespace std;
namespace WWise
{
    enum ForcePacketFormat {
        NoForcePacketFormat,
        ForceModPackets,
        ForceNoModPackets
    };

    class OggReStreamer
    {
        string FileName;
        string CodebooksName;
        istream& Infile;
        istream& CodebookFile;
        std::streamoff FileSize;

        bool LittleEndian;

        long RiffSize;
        long FmtOffset, CueOffset, ListOffset, SmplOffset, VorbOffset, DataOffset;
        long FmtSize, CueSize, ListSize, SmplSize, VorbSize, DataSize;

        // RIFF fmt
        uint16_t Channels;
        uint32_t SampleRate;
        uint32_t AvgBytesPerSecond;

        // RIFF extended fmt
        uint16_t ExtUnk;
        uint32_t Subtype;

        // cue info
        uint32_t CueCount;

        // smpl info
        uint32_t LoopCount, LoopStart, LoopEnd;

        // vorbis info
        uint32_t SampleCount;
        uint32_t SetupPacketOffset;
        uint32_t FirstAudioPacketOffset;
        uint32_t Uid;
        uint8_t Blocksize0Pow;
        uint8_t Blocksize1Pow;

        const bool InlineCodebooks, FullSetup;
        bool HeaderTriadPresent, OldPacketHeaders;
        bool NoGranule, ModPackets;

        uint16_t(*Read16)(std::istream& is);
        uint32_t(*Read32)(std::istream& is);
    public:
        OggReStreamer(
            const string& name,
            istream& wemStream,
            const string& codebooksName,
            istream& codebookStream,
            bool inlineCodebooks,
            bool fullSetup,
            ForcePacketFormat forcePacketFormat
        );

        void PrintInfo() const;

        void GenerateOgg(ostream& of) const;
        void GenerateOggHeader(Bits::OggBitStream& os, bool*& modeBlockFlag, int& modeBits) const;
        void GenerateOggHeaderWithTriad(Bits::OggBitStream& os) const;
    };
}
