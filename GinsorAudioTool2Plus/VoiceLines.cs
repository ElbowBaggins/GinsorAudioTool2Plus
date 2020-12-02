using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace GinsorAudioTool2Plus
{
  internal class VoiceLines
  {
    public VoiceLines(uint voiceCollectionHash)
    {
      this._pkgFile = new PkgFile(voiceCollectionHash);
      byte[] buffer = new PkgExtract().ToBuffer(this._pkgFile);
      this._vcms = new MemoryStream(buffer);
      this.StartProcess();
      this._vcms.Close();
    }

    public VoiceLines(string filename)
    {
      this._pkgFile = new PkgFile(filename);
      byte[] buffer = new PkgExtract().ToBuffer(this._pkgFile);
      this._vcms = new MemoryStream(buffer);
      this.StartProcess();
      this._vcms.Close();
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
              this.ReadHeader();
              num = 1;
              break;
            case 1:
              this.GetHeader1Info();
              num = 2;
              break;
            case 2:
              this.GetHeader2Info();
              num = 3;
              break;
            case 3:
              this.ReadDataBlocks();
              num = 7;
              break;
            case 7:
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

    private void ReadHeader()
    {
      this._vcms.Seek(8L, SeekOrigin.Begin);
      this._arrayEntries = Helpers.ReadUInt(this._vcms);
      this._vcms.Seek(4L, SeekOrigin.Current);
      this._headerPointer1 = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms) + 0x10U;
      this._vcms.Seek(0xCL, SeekOrigin.Current);
      this._headerPointer2 = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms) + 0x10U;
    }

    private void GetHeader1Info()
    {
      this._vcms.Seek((long)((ulong)this._headerPointer1), SeekOrigin.Begin);
      this._voiceCollectionHeader1List = new List<VoiceCollectionHeader1>();
      for (uint num = 0U; num < this._arrayEntries; num += 1U)
      {
        VoiceCollectionHeader1 item = default(VoiceCollectionHeader1);
        item.Hash1 = Helpers.ReadUInt(this._vcms);
        item.Length = Helpers.ReadFloat(this._vcms);
        this._voiceCollectionHeader1List.Add(item);
      }
    }

    private void GetHeader2Info()
    {
      this._vcms.Seek((long)((ulong)this._headerPointer2), SeekOrigin.Begin);
      this._voiceCollectionHeader2List = new List<VoiceCollectionHeader2>();
      for (uint num = 0U; num < this._arrayEntries; num += 1U)
      {
        VoiceCollectionHeader2 item = default(VoiceCollectionHeader2);
        item.Hash1 = Helpers.ReadUInt(this._vcms);
        this._vcms.Seek(4L, SeekOrigin.Current);
        item.Pointer = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms);
        this._vcms.Seek(4L, SeekOrigin.Current);
        this._voiceCollectionHeader2List.Add(item);
      }
    }

    private void ReadDataBlocks()
    {
      using (List<VoiceCollectionHeader1>.Enumerator enumerator = this._voiceCollectionHeader1List.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          VoiceLines.ReadDataBlocksHelper readDataBlocksHelper = new VoiceLines.ReadDataBlocksHelper
          {
            VoiceCollectionHeader1 = enumerator.Current
          };
          int num = this._voiceCollectionHeader2List.FindIndex(new Predicate<VoiceCollectionHeader2>(readDataBlocksHelper.ReadDataBlocks0));
          VoiceCollectionHeader2 voiceCollectionHeader = default(VoiceCollectionHeader2);
          if (num != -1)
          {
            voiceCollectionHeader = this._voiceCollectionHeader2List[num];
          }
          this._voiceEntry = default(VoiceEntry);
          this._voiceEntry.Header2Hash1 = voiceCollectionHeader.Hash1;
          this.Loop(voiceCollectionHeader.Pointer);
        }
      }
    }

        private void Loop(uint initialPointer)
    {
      this._vcms.Seek((long)((ulong)(initialPointer - 4U)), SeekOrigin.Begin);
      uint num = Helpers.ReadUInt(this._vcms);

    if (num == this._specialClassHash)
    {
        this.SpecialClassCase(initialPointer);
        return;
    }
      if (num == this._arrayClassHash)
      {
        this._vcms.Seek((long)((ulong)(initialPointer + 0x20U)), SeekOrigin.Begin);
        uint num2 = Helpers.ReadUInt(this._vcms);
        this._vcms.Seek(4L, SeekOrigin.Current);
        uint num3 = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms) + 0xCU;
        this._vcms.Seek((long)((ulong)num3), SeekOrigin.Begin);
        for (uint num4 = 0U; num4 < num2; num4 += 1U)
        {
          this._vcms.Seek((long)((ulong)(num3 + 0x48U * num4 + 0x44U)), SeekOrigin.Begin);
          uint num5 = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms);
          this._vcms.Seek((long)((ulong)(num5 - 4U)), SeekOrigin.Begin);
          uint num6 = Helpers.ReadUInt(this._vcms);
          if (num6 == this._entryClassHash)
          {
            this._vcms.Seek((long)((ulong)num5), SeekOrigin.Begin);
            uint hash = Helpers.ReadUInt(this._vcms);
            this.TestList.Add(Helpers.InvertUint32(hash).ToString("X8"));
            this.ReadEntry(num5);
          }
          else if (num6 == this._arrayClassHash)
          {
            this.Loop(num5);
          }
          else if (num6 == this._specialClassHash)
          {
            this.SpecialClassCase(num5);
          }
        }
        return;
      }
      if (num == this._entryClassHash)
      {
        this._vcms.Seek((long)((ulong)initialPointer), SeekOrigin.Begin);
        uint hash2 = Helpers.ReadUInt(this._vcms);
        this.TestList.Add(Helpers.InvertUint32(hash2).ToString("X8"));
        this.ReadEntry(initialPointer);
      }
    }

    private void ReadEntry(uint pointer)
    {
      this._vcms.Seek((long)((ulong)pointer), SeekOrigin.Begin);
      this._voiceEntry.FileHash = this._pkgFile.Filehash;
      this._voiceEntry.EntryHash = Helpers.ReadUInt(this._vcms);
      this._vcms.Seek(0x14L, SeekOrigin.Current);
      this._voiceEntry.AudioCollectionFileHash = Helpers.ReadUInt(this._vcms);
            this._vcms.Seek(0xc, SeekOrigin.Current);
      this._voiceEntry.TextRefFileHash = Helpers.ReadUInt(this._vcms);
      this._voiceEntry.TextHash = Helpers.ReadUInt(this._vcms);
      this._voiceEntry.Length = Helpers.ReadFloat(this._vcms);
            this._vcms.Seek(4, SeekOrigin.Current);
      this._voiceEntry.AudioCollectionFileHash2 = Helpers.ReadUInt(this._vcms);
            this._vcms.Seek(0xc, SeekOrigin.Current);
            this._voiceEntry.TextRefFileHash2 = Helpers.ReadUInt(this._vcms);
      this._voiceEntry.TextHash2 = Helpers.ReadUInt(this._vcms);
      this._voiceEntry.Length2 = Helpers.ReadFloat(this._vcms);
            this._vcms.Seek(4, SeekOrigin.Current);
            Helpers.InvertUint32(Helpers.ReadUInt(this._vcms));
      this._voiceEntry.NarratorHash = Helpers.ReadUInt(this._vcms);
      if (this._voiceEntry.TextRefFileHash != 0xFFFFFFFFU && this._voiceEntry.TextHash != 0xC59D1C81U)
      {
        this._voiceEntry.Text = this.GetText(this._voiceEntry.TextRefFileHash, this._voiceEntry.TextHash);
        this._voiceEntry.Text2 = this.GetText(this._voiceEntry.TextRefFileHash2, this._voiceEntry.TextHash2);
        this._voiceEntry.Narrator = this.GetNarrator(this._voiceEntry.NarratorHash);
        this.VoiceEntries.Add(this._voiceEntry);
        VoiceEntryLtd item = default(VoiceEntryLtd);
        item.EntryHash = this._voiceEntry.EntryHash;
        item.AudioCollectionFileHash = this._voiceEntry.AudioCollectionFileHash;
        item.Length = this._voiceEntry.Length;
        item.Text = this._voiceEntry.Text;
        item.Narrator = this._voiceEntry.Narrator;
        this.VoiceEntriesLtd.Add(item);
      }
    }

    private string GetText(uint textRefHash, uint textHash)
    {
      VoiceLines.GetTextHelper getTextHelper = new VoiceLines.GetTextHelper
      {
        TextRefHash = textRefHash,
        TextHash = textHash
      };
      int num = this._allTextsDb.FindIndex(new Predicate<TextResult>(getTextHelper.GetText0));
      string result = "Unknown";
      if (num != -1)
      {
        result = this._allTextsDb[num].StringText;
      }
      return result;
    }

    private string GetNarrator(uint textHash)
    {
      VoiceLines.GetNarratorHelper getNarratorHelper = new VoiceLines.GetNarratorHelper
      {
        TextHash = textHash
      };
      int num = this._allTextsDb.FindIndex(new Predicate<TextResult>(getNarratorHelper.GetNarrator0));
      string result = "Narrator not defined";
      if (num != -1)
      {
        result = this._allTextsDb[num].StringText;
      }
      return result;
    }

    public uint GetAudioFile(uint audioCollectionHash)
    {
      PkgFile pkgFile = new PkgFile(audioCollectionHash);
      MemoryStream memoryStream = new MemoryStream(new PkgExtract().ToBuffer(pkgFile));
      memoryStream.Seek(0x20L, SeekOrigin.Begin);
      uint num = (uint)memoryStream.Position + Helpers.ReadUInt(memoryStream);
      memoryStream.Seek((long)((ulong)(num + 0x10U)), SeekOrigin.Begin);
      uint result = Helpers.ReadUInt(memoryStream);
      memoryStream.Close();
      return result;
    }

    public uint GetAudioEntryA(uint audioFileHash)
    {
      return new PkgFile(audioFileHash).PkgEntry.EntryA;
    }

    private void SpecialClassCase(uint pointer)
    {
      this._vcms.Seek((long)((ulong)(pointer + 0x18U)), SeekOrigin.Begin);
      uint num = Helpers.ReadUInt(this._vcms);
      this._vcms.Seek((long)((ulong)(pointer + 0x20U)), SeekOrigin.Begin);
      uint num2 = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms) + 0x10U;
      for (uint num3 = 0U; num3 < num; num3 += 1U)
      {
        this._vcms.Seek((long)((ulong)(num2 + num3 * 8U)), SeekOrigin.Begin);
        uint num4 = (uint)this._vcms.Position + Helpers.ReadUInt(this._vcms);
        this._vcms.Seek((long)((ulong)(num4 - 4U)), SeekOrigin.Begin);
        uint num5 = Helpers.ReadUInt(this._vcms);
        if (num5 == this._entryClassHash)
        {
          this._vcms.Seek((long)((ulong)num4), SeekOrigin.Begin);
          this.TestList.Add(Helpers.InvertUint32(Helpers.ReadUInt(this._vcms)).ToString("X8"));
          this.ReadEntry(num4);
        }
        else if (num5 == this._arrayClassHash)
        {
          this.Loop(num4);
        }
        else if (num5 == this._specialClassHash)
        {
          this.SpecialClassCase(num4);
        }
      }
    }

    public void Brute()
    {
      while (this._vcms.Position < this._vcms.Length)
      {
        if (Helpers.ReadUInt(this._vcms) == _entryClassHash)
        {
          this.ReadEntry((uint)this._vcms.Position);
        }
      }
    }

    private readonly uint _arrayClassHash = 0x8080972AU;

    private readonly uint _entryClassHash = 0x80809733U;

    private readonly uint _specialClassHash = 0x8080972DU;

    private readonly PkgFile _pkgFile;

    private readonly List<TextResult> _allTextsDb = Form1.RecAllTextDb();

    private readonly MemoryStream _vcms;

    private uint _arrayEntries;

    private uint _headerPointer1;

    private uint _headerPointer2;

    private List<VoiceCollectionHeader1> _voiceCollectionHeader1List;

    private List<VoiceCollectionHeader2> _voiceCollectionHeader2List;

    public List<string> TestList = new List<string>();

    public List<VoiceEntry> VoiceEntries = new List<VoiceEntry>();

    public List<VoiceEntryLtd> VoiceEntriesLtd = new List<VoiceEntryLtd>();

    private VoiceEntry _voiceEntry;

    [CompilerGenerated]
    private sealed class ReadDataBlocksHelper
    {
      public ReadDataBlocksHelper()
      {
      }

      internal bool ReadDataBlocks0(VoiceCollectionHeader2 c)
      {
        return c.Hash1 == this.VoiceCollectionHeader1.Hash1;
      }

      public VoiceCollectionHeader1 VoiceCollectionHeader1;
    }

    [CompilerGenerated]
    private sealed class GetTextHelper
    {
      public GetTextHelper()
      {
      }

      internal bool GetText0(TextResult c)
      {
        return c.TextRefHash == this.TextRefHash && c.StringHash == this.TextHash;
      }

      public uint TextRefHash;

      public uint TextHash;
    }

    [CompilerGenerated]
    private sealed class GetNarratorHelper
    {
      public GetNarratorHelper()
      {
      }

      internal bool GetNarrator0(TextResult c)
      {
        return c.StringHash == this.TextHash;
      }

      public uint TextHash;
    }
  }
}
