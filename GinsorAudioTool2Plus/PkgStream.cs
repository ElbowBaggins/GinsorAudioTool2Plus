using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace GinsorAudioTool2Plus
{
  public class PkgStream
  {
    public PkgStream(Stream s)
    {
      this._pkgbuffer = s;
      this._pkgListEntries = Form1.RecvPkgListEntries();
      this.StartProcess();
    }

    public void StartProcess()
    {
      int num = 0;
      while (num != 0xA && num != -1)
      {
        try
        {
          switch (num)
          {
            case 0:
              this.ProcessHeader(this._pkgbuffer);
              num = 1;
              break;
            case 1:
              this.ProcessEntryTable(this._pkgbuffer);
              num = 2;
              break;
            case 2:
              this.ProcessBlockTable(this._pkgbuffer);
              num = 3;
              break;
            case 3:
              this.MakeNonce(this.Header.PackageId);
              num = 4;
              break;
            case 4:
              this.ReadBlock(this._pkgbuffer);
              num = 5;
              break;
            case 5:
              this.ReadEntries(this._pkgbuffer);
              num = 7;
              break;
            case 7:
              this._pkgbuffer.Close();
              num = 0xA;
              break;
          }
        }
        catch (Exception)
        {
          num = -1;
        }
      }
    }

    public void ProcessHeader(Stream s)
    {
      this.Header = new PkgStream.PkgHeader
      {
        Version = Helpers.ReadUShort(s),
        Platform = Helpers.ReadUShort(s),
        PackageId = Helpers.ReadUShort(s),
        Unk0X06 = Helpers.ReadUShort(s),
        Unk0X08 = Helpers.ReadUInt(s),
        Unk0X0C = Helpers.ReadUInt(s),
        BuildDate = Helpers.ReadUInt(s),
        Unk0X14 = Helpers.ReadUInt(s),
        Unk0X18 = Helpers.ReadUShort(s),
        PkgType = Helpers.ReadUShort(s),
        Unk0X1C = Helpers.ReadUInt(s),
        PatchId = Helpers.ReadUShort(s),
        LangId = Helpers.ReadUShort(s)
      };
    }

    public void ProcessEntryTable(Stream s)
    {
      this.Entries = default(PkgStream.EntryTable);
      if (this.Header.PkgType != 1)
      {
        s.Seek(0xB4L, SeekOrigin.Begin);
        this.Entries.Size = Helpers.ReadUInt(s);
        this.Entries.Offset = Helpers.ReadUInt(s);
        return;
      }
      s.Seek(0x110L, SeekOrigin.Begin);
      uint num = Helpers.ReadUInt(s);
      s.Seek((long)((ulong)(num + 0x10U)), SeekOrigin.Begin);
      this.Entries.Size = Helpers.ReadUInt(s);
      s.Seek(4L, SeekOrigin.Current);
      uint num2 = Helpers.ReadUInt(s);
      this.Entries.Offset = num + 0x28U + num2;
    }

    public void ProcessBlockTable(Stream s)
    {
      this.Blocks = default(PkgStream.BlockTable);
      if (this.Header.PkgType != 1)
      {
        s.Seek(0xD0L, SeekOrigin.Begin);
        this.Blocks.Size = Helpers.ReadUInt(s);
        this.Blocks.Offset = Helpers.ReadUInt(s);
        return;
      }
      s.Seek(0x110L, SeekOrigin.Begin);
      uint num = Helpers.ReadUInt(s);
      s.Seek((long)((ulong)(num + 0x20U)), SeekOrigin.Begin);
      this.Blocks.Size = Helpers.ReadUInt(s);
      s.Seek(4L, SeekOrigin.Current);
      uint num2 = Helpers.ReadUInt(s);
      this.Blocks.Offset = num + 0x38U + num2;
    }

    public void MakeNonce(ushort packageId)
    {
      this.Nonce = this._baseNonce;
      byte[] nonce = this.Nonce;
      int num = 0;
      nonce[num] ^= (byte)(packageId >> 8 & 0xFF);
      byte[] nonce2 = this.Nonce;
      int num2 = 1;
      nonce2[num2] ^= 0x26;
      byte[] nonce3 = this.Nonce;
      int num3 = 0xB;
      nonce3[num3] ^= (byte)(packageId & 0xFF);
    }

    public void ReadBlock(Stream s)
    {
      s.Seek((long)((ulong)this.Blocks.Offset), SeekOrigin.Begin);
      for (uint num = 0U; num < this.Blocks.Size; num += 1U)
      {
        BlockEntry blockEntry = new BlockEntry
        {
          BlockNumber = num,
          Offset = Helpers.ReadUInt(s),
          Size = Helpers.ReadUInt(s),
          PatchId = Helpers.ReadUShort(s),
          Flag = Helpers.ReadUShort(s)
        };
        if ((blockEntry.Flag & 1) != 0)
        {
          blockEntry.Encrypted = true;
        }
        if ((blockEntry.Flag & 2) != 0)
        {
          blockEntry.Compressed = true;
        }
        if ((blockEntry.Flag & 4) != 0)
        {
          blockEntry.AltKey = true;
        }
        blockEntry.Md5Hash = Helpers.ReadByteArray(s, 0x14U);
        blockEntry.GcmTag = Helpers.ReadByteArray(s, 0x10U);
        this.BlockEntryList.Add(blockEntry);
      }
    }

    public void ReadEntries(Stream s)
    {
      s.Seek((long)((ulong)this.Entries.Offset), SeekOrigin.Begin);
      for (uint num = 0U; num < this.Entries.Size; num += 1U)
      {
        PkgEntry pkgEntry = default(PkgEntry);
        pkgEntry.EntryNumber = num;
        pkgEntry.EntryA = Helpers.ReadUInt(s);
        pkgEntry.TypeA = Helpers.ReadUInt(s);
        pkgEntry.EntryB = Helpers.ReadULong(s);
        pkgEntry.StartBlock = (uint)(pkgEntry.EntryB & 0x3FFFUL);
        pkgEntry.StartBlockOffset = this.Blocks.Offset + pkgEntry.StartBlock * 0x30U;
        int index = this._pkgListEntries.FindIndex(new Predicate<PkgListEntry>(this.ReadEntries19));
        PkgListEntry pkgListEntry = this._pkgListEntries[index];
        BlockEntry blockEntry = this.BlockEntryList[(int)pkgEntry.StartBlock];
        pkgEntry.StartBlockPkg = string.Concat(new object[]
        {
          pkgListEntry.Basename,
          "_",
          blockEntry.PatchId,
          ".pkg"
        });
        pkgEntry.InBlockOffset = (uint)((pkgEntry.EntryB >> 0xE & 0x3FFFUL) * 0x10UL);
        pkgEntry.EntrySize = (uint)(pkgEntry.EntryB >> 0x1C & 0x3FFFFFFFUL);
        pkgEntry.BlockCount = this.CalcBlockCount(pkgEntry.InBlockOffset, pkgEntry.EntrySize);
        pkgEntry.UnkEntryInfo = (uint)(pkgEntry.EntryB >> 0x3A & 0x3FUL);
        pkgEntry.Filename = string.Concat(new string[]
        {
          this.Header.PackageId.ToString("X4"),
          "_",
          pkgEntry.EntryNumber.ToString("X4"),
          "_",
          this.Header.LangId.ToString("X2")
        });
        pkgEntry.FileType = FileClassification.FileClassify(pkgEntry.EntryA, pkgEntry.TypeA);
        this.PkgEntryList.Add(pkgEntry);
      }
    }

    public uint CalcBlockCount(uint entryInBlockOffset, uint entrySize)
    {
      uint num = 0x40000U;
      return (entryInBlockOffset + entrySize + num - 1U) / num;
    }

    public static PkgStream PkgStreamFromFile(string pkgFile)
    {
      PkgStream result;
      using (FileStream fileStream = File.OpenRead(pkgFile))
      {
        result = new PkgStream(fileStream);
        fileStream.Close();
      }
      return result;
    }

    [CompilerGenerated]
    private bool ReadEntries19(PkgListEntry c)
    {
      return c.PackageId == this.Header.PackageId && c.LangId == this.Header.LangId;
    }

    private readonly Stream _pkgbuffer;

    private readonly byte[] _baseNonce = new byte[]
    {
      0x84,
      0xDF,
      0x11,
      0xC0,
      0xAC,
      0xAB,
      0xFA,
      0x20,
      0x33,
      0x11,
      0x26,
      0x99
    };

    private readonly List<PkgListEntry> _pkgListEntries;

    public PkgStream.PkgHeader Header;

    public PkgStream.EntryTable Entries;

    public PkgStream.BlockTable Blocks;

    public byte[] Nonce;

    public List<PkgEntry> PkgEntryList = new List<PkgEntry>();

    public List<BlockEntry> BlockEntryList = new List<BlockEntry>();

    public struct PkgHeader
    {
      public ushort Version;

      public ushort Platform;

      public ushort PackageId;

      public ushort Unk0X06;

      public uint Unk0X08;

      public uint Unk0X0C;

      public uint BuildDate;

      public uint Unk0X14;

      public ushort Unk0X18;

      public ushort PkgType;

      public uint Unk0X1C;

      public ushort PatchId;

      public ushort LangId;
    }

    public struct EntryTable
    {
      public uint Size;

      public uint Offset;
    }

    public struct BlockTable
    {
      public uint Size;

      public uint Offset;
    }
  }
}
