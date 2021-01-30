#pragma once

#include "crc.h"
#include "errors.h"

#include <iostream>
#include <limits>
#include <stdint.h>

// host-endian-neutral integer reading
namespace WWise::Bits
{
    inline uint32_t Read32BitLittleEndian(unsigned char bytes[4])
    {
        uint32_t value = 0;
        for (auto i = 3; i >= 0; i--)
        {
            value <<= 8;
            value |= bytes[i];
        }

        return value;
    }

    inline uint32_t Read32BitLittleEndian(std::istream& inputStream)
    {
        char bytes[4];
        inputStream.read(bytes, 4);

        return Read32BitLittleEndian(reinterpret_cast<unsigned char*>(bytes));
    }

    inline void Write32BitLittleEndian(unsigned char bytes[4], uint32_t value)
    {
        for (auto i = 0; i < 4; i++)
        {
            bytes[i] = value & 0xFF;
            value >>= 8;
        }
    }

    inline void Write32BitLittleEndian(std::ostream& outputStream, const uint32_t values)
    {
        char bytes[4];

        Write32BitLittleEndian(reinterpret_cast<unsigned char*>(bytes), values);

        outputStream.write(bytes, 4);
    }

    inline uint16_t Read16BitLittleEndian(unsigned char bytes[2])
    {
        uint16_t value = 0;
        for (auto i = 1; i >= 0; i--)
        {
            value <<= 8;
            value |= bytes[i];
        }

        return value;
    }

    inline uint16_t Read16BitLittleEndian(std::istream& inputStream)
    {
        char bytes[2];
        inputStream.read(bytes, 2);

        return Read16BitLittleEndian(reinterpret_cast<unsigned char*>(bytes));
    }

    inline void Write16BitLittleEndian(unsigned char bytes[2], uint16_t value)
    {
        for (auto i = 0; i < 2; i++)
        {
            bytes[i] = value & 0xFF;
            value >>= 8;
        }
    }

    inline void Write16BitLittleEndian(std::ostream& outputStream, const uint16_t value)
    {
        char bytes[2];

        Write16BitLittleEndian(reinterpret_cast<unsigned char*>(bytes), value);

        outputStream.write(bytes, 2);
    }

    inline uint32_t Read32BitBigEndian(unsigned char bytes[4])
    {
        uint32_t value = 0;
        for (auto i = 0; i < 4; i++)
        {
            value <<= 8;
            value |= bytes[i];
        }

        return value;
    }

    inline uint32_t Read32BitBigEndian(std::istream& inputStream)
    {
        char bytes[4];
        inputStream.read(bytes, 4);

        return Read32BitBigEndian(reinterpret_cast<unsigned char*>(bytes));
    }

    inline void Write32BitBigEndian(unsigned char bytes[4], uint32_t value)
    {
        for (auto i = 3; i >= 0; i--)
        {
            bytes[i] = value & 0xFF;
            value >>= 8;
        }
    }

    inline void Write32BitBigEndian(std::ostream& outputStream, const uint32_t value)
    {
        char bytes[4];

        Write32BitBigEndian(reinterpret_cast<unsigned char*>(bytes), value);

        outputStream.write(bytes, 4);
    }

    inline uint16_t Read16BitBigEndian(unsigned char bytes[2])
    {
        uint16_t value = 0;
        for (auto i = 0; i < 2; i++)
        {
            value <<= 8;
            value |= bytes[i];
        }

        return value;
    }

    inline uint16_t Read16BitBigEndian(std::istream& inputStream)
    {
        char bytes[2];
        inputStream.read(bytes, 2);

        return Read16BitBigEndian(reinterpret_cast<unsigned char*>(bytes));
    }

    inline void Write16BitBigEndian(unsigned char bytes[2], uint16_t value)
    {
        for (auto i = 1; i >= 0; i--)
        {
            bytes[i] = value & 0xFF;
            value >>= 8;
        }
    }

    inline void Write16BitBigEndian(std::ostream& outputStream, const uint16_t value)
    {
        char bytes[2];

        Write16BitBigEndian(reinterpret_cast<unsigned char*>(bytes), value);

        outputStream.write(bytes, 2);
    }

    class BitStream
    {
        std::istream& InputStream;

        unsigned char BitBuffer;
        unsigned int BitsLeft;
        unsigned long TotalBitsRead;

        public:
            class WeirdCharSizeException
            {
            };

            class OutOfBitsException
            {
            };

            explicit BitStream(std::istream& inputStream)
                : InputStream(inputStream),
                  BitBuffer(0),
                  BitsLeft(0),
                  TotalBitsRead(0)
            {
                if constexpr (std::numeric_limits<unsigned char>::digits != 8)
                    throw WeirdCharSizeException();
            }

            bool GetBit()
            {
                if (BitsLeft == 0)
                {
                    const auto c = InputStream.get();
                    if (c == EOF)
                        throw OutOfBitsException();
                    BitBuffer = c;
                    BitsLeft = 8;
                }
                TotalBitsRead++;
                BitsLeft--;
                return ((BitBuffer & (0x80 >> BitsLeft)) != 0);
            }

            unsigned long GetTotalBitsRead() const
            {
                return TotalBitsRead;
            }
    };

    class OggBitStream
    {
        std::ostream& OutputStream;

        unsigned char BitBuffer;
        unsigned int BitsStored;

        enum { HeaderBytes = 27, MaxSegments = 255, SegmentSize = 255 };

        unsigned int PayloadBytes;
        bool First, Continued;
        unsigned char PageBuffer[HeaderBytes + MaxSegments + SegmentSize * MaxSegments];
        uint32_t Granule;
        uint32_t SequenceNumber;

        public:
            class WeirdCharSize
            {
            };

            explicit OggBitStream(std::ostream& outputStream)
                : OutputStream(outputStream),
                  BitBuffer(0),
                  BitsStored(0),
                  PayloadBytes(0),
                  First(true),
                  Continued(false),
                  Granule(0),
                  SequenceNumber(0)
            {
                std::fill_n(PageBuffer, HeaderBytes + MaxSegments + SegmentSize * MaxSegments, '\0');
                if constexpr (std::numeric_limits<unsigned char>::digits != 8)
                    throw WeirdCharSize();
            }

            void PutBit(const bool bit)
            {
                if (bit)
                    BitBuffer |= 1 << BitsStored;

                BitsStored++;
                if (BitsStored == 8)
                {
                    FlushBits();
                }
            }

            void SetGranule(const uint32_t granule)
            {
                Granule = granule;
            }

            void FlushBits(void)
            {
                if (BitsStored != 0)
                {
                    if (PayloadBytes == SegmentSize * MaxSegments)
                    {
                        FlushPage(true);
                        throw Parse_error_str("ran out of space in an Ogg packet");
                    }

                    PageBuffer[HeaderBytes + MaxSegments + PayloadBytes] = BitBuffer;
                    PayloadBytes++;

                    BitsStored = 0;
                    BitBuffer = 0;
                }
            }

            void FlushPage(const bool nextContinued = false, const bool last = false)
            {
                if (PayloadBytes != SegmentSize * MaxSegments)
                {
                    FlushBits();
                }

                if (PayloadBytes != 0)
                {
                    unsigned int segments = (PayloadBytes + SegmentSize) / SegmentSize; // intentionally round up
                    if (segments == MaxSegments + 1)
                        segments = MaxSegments; // at max eschews the final 0

                    // move payload back
                    for (unsigned int i = 0; i < PayloadBytes; i++)
                    {
                        PageBuffer[HeaderBytes + segments + i] = PageBuffer[HeaderBytes + MaxSegments + i];
                    }

                    PageBuffer[0] = 'O';
                    PageBuffer[1] = 'g';
                    PageBuffer[2] = 'g';
                    PageBuffer[3] = 'S';
                    PageBuffer[4] = 0; // stream_structure_version
                    PageBuffer[5] = (Continued ? 1 : 0) | (First ? 2 : 0) | (last ? 4 : 0); // header_type_flag
                    Write32BitLittleEndian(&PageBuffer[6], Granule); // granule low bits
                    Write32BitLittleEndian(&PageBuffer[10], 0); // granule high bits
                    if (Granule == UINT32_C(0xFFFFFFFF))
                        Write32BitLittleEndian(&PageBuffer[10], UINT32_C(0xFFFFFFFF));
                    Write32BitLittleEndian(&PageBuffer[14], 1); // stream serial number
                    Write32BitLittleEndian(&PageBuffer[18], SequenceNumber); // page sequence number
                    Write32BitLittleEndian(&PageBuffer[22], 0); // checksum (0 for now)
                    PageBuffer[26] = segments; // segment count

                    // lacing values
                    for (unsigned int i = 0, bytesLeft = PayloadBytes; i < segments; i++)
                    {
                        if (bytesLeft >= SegmentSize)
                        {
                            bytesLeft -= SegmentSize;
                            PageBuffer[27 + i] = SegmentSize;
                        }
                        else
                        {
                            PageBuffer[27 + i] = bytesLeft;
                        }
                    }

                    // checksum
                    Write32BitLittleEndian(&PageBuffer[22], checksum(PageBuffer, HeaderBytes + segments + PayloadBytes));

                    // output to ostream
                    for (unsigned int i = 0; i < HeaderBytes + segments + PayloadBytes; i++)
                    {
                        OutputStream.put(PageBuffer[i]);
                    }

                    SequenceNumber++;
                    First = false;
                    Continued = nextContinued;
                    PayloadBytes = 0;
                }
            }

            ~OggBitStream()
            {
                FlushPage();
            }
    };

    class ArbitraryPrecisionUnsignedIntV
    {
        unsigned int Precision;
        unsigned int Value;
        public:
            class TooManyBitsException
            {
            };

            class ValueTooLargeException
            {
            };

            explicit ArbitraryPrecisionUnsignedIntV(unsigned int s)
                : Precision(s),
                  Value(0)
            {
                if (s > static_cast<unsigned int>(std::numeric_limits<unsigned int>::digits))
                    throw TooManyBitsException();
            }

            ArbitraryPrecisionUnsignedIntV(unsigned int s, unsigned int v)
                : Precision(s),
                  Value(v)
            {
                if (Precision > static_cast<unsigned int>(std::numeric_limits<unsigned int>::digits))
                    throw TooManyBitsException();
                if ((v >> (Precision - 1U)) > 1U)
                    throw ValueTooLargeException();
            }

            ArbitraryPrecisionUnsignedIntV& operator=(unsigned int v)
            {
                if ((v >> (Precision - 1U)) > 1U)
                    throw ValueTooLargeException();
                Value = v;
                return *this;
            }

            operator unsigned int() const
            {
                return Value;
            }

            friend BitStream& operator >>(BitStream& bStream, ArbitraryPrecisionUnsignedIntV& bui)
            {
                bui.Value = 0;
                for (unsigned int i = 0; i < bui.Precision; i++)
                {
                    if (bStream.GetBit())
                        bui.Value |= (1U << i);
                }
                return bStream;
            }

            friend OggBitStream& operator <<(OggBitStream& bStream, const ArbitraryPrecisionUnsignedIntV& bui)
            {
                for (unsigned int i = 0; i < bui.Precision; i++)
                {
                    bStream.PutBit((bui.Value & (1U << i)) != 0);
                }
                return bStream;
            }
    };

    template <unsigned int BitSize> class ArbitraryPrecisionUnsignedInt
    {
        unsigned int Total;
        public:
            class TooManyBitsException
            {
            };

            class ValueTooLargeException
            {
            };

            ArbitraryPrecisionUnsignedInt()
                : Total(0)
            {
                if (BitSize > static_cast<unsigned int>(std::numeric_limits<unsigned int>::digits))
                    throw TooManyBitsException();
            }

            explicit ArbitraryPrecisionUnsignedInt(unsigned int v)
                : Total(v)
            {
                if (BitSize > static_cast<unsigned int>(std::numeric_limits<unsigned int>::digits))
                    throw TooManyBitsException();
                if ((v >> (BitSize - 1U)) > 1U)
                    throw ValueTooLargeException();
            }

            ArbitraryPrecisionUnsignedInt& operator =(unsigned int v)
            {
                if ((v >> (BitSize - 1U)) > 1U)
                    throw ValueTooLargeException();
                Total = v;
                return *this;
            }

            operator unsigned int() const
            {
                return Total;
            }

            friend BitStream& operator >>(BitStream& bstream, ArbitraryPrecisionUnsignedInt& bui)
            {
                bui.Total = 0;
                for (unsigned int i = 0; i < BitSize; i++)
                {
                    if (bstream.GetBit())
                        bui.Total |= (1U << i);
                }
                return bstream;
            }

            friend OggBitStream& operator <<(OggBitStream& bstream, const ArbitraryPrecisionUnsignedInt& bui)
            {
                for (unsigned int i = 0; i < BitSize; i++)
                {
                    bstream.PutBit((bui.Total & (1U << i)) != 0);
                }
                return bstream;
            }
    };

    class ArrayStreamBuffer final : public std::streambuf
    {   
        char* Buffer;
        int Size;

        public:
            ArrayStreamBuffer(const char* source, const int length)
                : Buffer(nullptr), Size(length)
            {
                Buffer = new char[Size];
                for (auto i = 0; i < Size; i++)
                    Buffer[i] = source[i];
                setg(Buffer, Buffer, Buffer + Size);
            }
            ArrayStreamBuffer(const ArrayStreamBuffer& rhs) : ArrayStreamBuffer(rhs.Buffer, rhs.Size) {}
            ~ArrayStreamBuffer()
            {
                delete[] Buffer;
            }
            ArrayStreamBuffer& operator=(ArrayStreamBuffer& rhs) const
            {
                return rhs;
            }
    };
}
