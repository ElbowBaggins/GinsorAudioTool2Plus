using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GinsorAudioTool2Plus
{
  internal class PkgText
  {
    public PkgText(PkgStream pkgStreamInput)
    {
      this._d2Files = this.GetD2Files(pkgStreamInput);
    }

    public PkgText(PkgFile pkgFile)
    {
      this._pkgFile = pkgFile;
    }

    public PkgText(List<PkgStream> pkgStreamList)
    {
      this._d2Files = this.GetD2Files(pkgStreamList);
    }

    public List<TextResult> GetTextResults()
    {
      if (this._pkgFile.PkgFileType == PkgFileType.Text)
      {
        this.StartProcess();
      }
      return this._textResults;
    }

    public List<TextResult> GetTextResults2()
    {
      foreach (PkgFile d2File in this._d2Files)
      {
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          this.StartProcess();
        }
      }
      return this._textResults;
    }

    public List<string> GetStringList()
    {
      if (this._pkgFile.PkgFileType == PkgFileType.Text)
      {
        this.StartProcess();
      }
      return this._stringListWithIds;
    }

    public void SaveTexts(string outputFile)
    {
      if (this._pkgFile.PkgFileType == PkgFileType.Text)
      {
        this.StartProcess();
        Helpers.FileExistsDelete(outputFile);
        Helpers.DirNotExistCreate(Path.GetDirectoryName(outputFile));
        File.AppendAllLines(outputFile, this._textResults.Select(new Func<TextResult, string>(PkgText.SaveTextHelper0.SaveTextHelperInstance.SaveTexts15)));
      }
    }

    public void SaveTextsInclIds(string outputFile)
    {
      if (this._pkgFile.PkgFileType == PkgFileType.Text)
      {
        this.StartProcess();
        Helpers.FileExistsDelete(outputFile);
        Helpers.DirNotExistCreate(Path.GetDirectoryName(outputFile));
        File.AppendAllLines(outputFile, this._stringListWithIds);
      }
    }

    public void SaveJson(string outputFile)
    {
      if (this._pkgFile.PkgFileType == PkgFileType.Text)
      {
        this.StartProcess();
        Helpers.FileExistsDelete(outputFile);
        Helpers.DirNotExistCreate(Path.GetDirectoryName(outputFile));
        string contents = JsonSerializer.Serialize<List<TextResult>>(_textResults, new JsonSerializerOptions
        {
          IncludeFields = true,
          WriteIndented = true
        });
        File.AppendAllText(outputFile, contents);
      }
    }

    public void SaveAll(string outputDir)
    {
      Helpers.DirNotExistCreate(outputDir);
      foreach (PkgFile d2File in this._d2Files)
      {
        this._textResults = new List<TextResult>();
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          Helpers.FileExistsDelete(outputDir + d2File.Filename.Replace(".bin", ".txt"));
          this.SaveTexts(outputDir + d2File.Filename.Replace(".bin", ".txt"));
        }
      }
    }

    public void SaveAllInOne(string outputFile)
    {
      foreach (PkgFile d2File in this._d2Files)
      {
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          this.StartProcess();
        }
      }
      Helpers.FileExistsDelete(outputFile);
      Helpers.DirNotExistCreate(Path.GetDirectoryName(outputFile));
      File.AppendAllLines(outputFile, this._textResults.Select(new Func<TextResult, string>(PkgText.SaveTextHelper0.SaveTextHelperInstance.SaveAllInOne0)));
    }

    public void SaveAllInclIds(string outputDir)
    {
      Helpers.DirNotExistCreate(outputDir);
      foreach (PkgFile d2File in this._d2Files)
      {
        this._stringListWithIds = new List<string>();
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          Helpers.FileExistsDelete(outputDir + d2File.Filename.Replace(".bin", ".txt"));
          this.SaveTextsInclIds(outputDir + d2File.Filename.Replace(".bin", ".txt"));
        }
      }
    }

    public void SaveAllInclIdsInOne(string outputFile)
    {
      foreach (PkgFile d2File in this._d2Files)
      {
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          this._stringListWithIds.Add(string.Concat(new string[]
          {
            "---------- ",
            this._pkgFile.Filename,
            " - ",
            this._pkgFile.Filehash.ToString("X8"),
            " ----------"
          }));
          this.StartProcess();
        }
      }
      Helpers.FileExistsDelete(outputFile);
      Helpers.DirNotExistCreate(Path.GetDirectoryName(outputFile));
      File.AppendAllLines(outputFile, this._stringListWithIds);
    }

    public void SaveAllJson(string outputDir)
    {
      Helpers.DirNotExistCreate(outputDir);
      foreach (PkgFile d2File in this._d2Files)
      {
        this._textResults = new List<TextResult>();
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          Helpers.FileExistsDelete(outputDir + d2File.Filename.Replace(".bin", ".json"));
          this.SaveJson(outputDir + d2File.Filename.Replace(".bin", ".json"));
        }
      }
    }

    public void SaveAllJsonInOne(string outputFile)
    {
      foreach (PkgFile d2File in this._d2Files)
      {
        if (d2File.PkgFileType == PkgFileType.Text)
        {
          this._pkgFile = d2File;
          this.StartProcess();
        }
      }
      Helpers.FileExistsDelete(outputFile);
      Helpers.DirNotExistCreate(Path.GetDirectoryName(outputFile));
      string contents = JsonSerializer.Serialize<List<TextResult>>(this._textResults, new JsonSerializerOptions() {
        IncludeFields = true,
        WriteIndented = true
      });
      File.AppendAllText(outputFile, contents);
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
              if (this._pkgFile.PkgFileType != PkgFileType.Text)
              {
                num = -1;
              }
              else
              {
                this.ReadParam();
                num = 1;
              }
              break;
            case 1:
              this.ReadRawTextfile();
              num = 2;
              break;
            case 2:
              this.RawTextHeader();
              num = 3;
              break;
            case 3:
              this.StringMeta();
              num = 4;
              break;
            case 4:
              this.EntryMeta();
              num = 5;
              break;
            case 5:
              this.GetStringLists();
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

    private void ReadParam()
    {
      this._d2TextParam = default(D2TextParam);
      MemoryStream memoryStream = new MemoryStream(new PkgExtract().ToBuffer(this._pkgFile));
      memoryStream.Seek(0x18L, SeekOrigin.Begin);
      this._d2TextParam.EngFileHash = Helpers.ReadUInt(memoryStream);
      memoryStream.Seek(0x20L, SeekOrigin.Begin);
      this._d2TextParam.GerFileHash = Helpers.ReadUInt(memoryStream);
      memoryStream.Seek(0x2CL, SeekOrigin.Begin);
      this._d2TextParam.EspFileHash = Helpers.ReadUInt(memoryStream);
      memoryStream.Seek(0x60L, SeekOrigin.Begin);
      this._d2TextParam.NumOfstringHashes = Helpers.ReadUInt(memoryStream);
      memoryStream.Seek(0x70L, SeekOrigin.Begin);
      this._d2TextParam.StringHashList = new Dictionary<uint, uint>();
      for (uint num = 0U; num < this._d2TextParam.NumOfstringHashes; num += 1U)
      {
        uint value = Helpers.ReadUInt(memoryStream);
        this._d2TextParam.StringHashList.Add(num, value);
      }
    }

    private void ReadRawTextfile()
    {
      PkgFile pkgFile = Calculation.Hash2File(this._d2TextParam.EngFileHash);
      byte[] buffer = new PkgExtract().ToBuffer(pkgFile);
      this._d2TextMs = new MemoryStream(buffer);
    }

    private void RawTextHeader()
    {
      this._textHeader = default(TextHeader);
      this._textHeader.FileLength = Helpers.ReadUInt(this._d2TextMs);
      this._d2TextMs.Seek(8L, SeekOrigin.Begin);
      this._textHeader.NumOfEntries = Helpers.ReadUInt(this._d2TextMs);
      this._d2TextMs.Seek(0x40L, SeekOrigin.Begin);
      this._textHeader.OffsetTs = Helpers.ReadUInt(this._d2TextMs) + 0x50U;
      this._d2TextMs.Seek(0x48L, SeekOrigin.Begin);
      this._textHeader.NumOfString = Helpers.ReadUInt(this._d2TextMs);
      this._d2TextMs.Seek(0x50L, SeekOrigin.Begin);
      this._textHeader.OffsetSms = Helpers.ReadUInt(this._d2TextMs) + 0x60U;
    }

    private void StringMeta()
    {
      this._d2TextMs.Seek((long)((ulong)this._textHeader.OffsetSms), SeekOrigin.Begin);
      this._stringMetas = new List<StringMeta>();
      for (uint num = 0U; num < this._textHeader.NumOfEntries; num += 1U)
      {
        StringMeta item = default(StringMeta);
        item.EntryMetaOffset = _textHeader.OffsetSms + Helpers.ReadULong(this._d2TextMs) + num * 0x10U;
        item.NumOfEntries = Helpers.ReadUInt(this._d2TextMs);
        this._d2TextMs.Seek(4L, SeekOrigin.Current);
        this._stringMetas.Add(item);
      }
    }

    private void EntryMeta()
    {
      this._allEntryMetas = new List<List<EntryMeta>>();
      foreach (StringMeta stringMeta in this._stringMetas)
      {
        List<EntryMeta> list = new List<EntryMeta>();
        for (uint num = 0U; num < stringMeta.NumOfEntries; num += 1U)
        {
          EntryMeta item = default(EntryMeta);
          this._d2TextMs.Seek((long)((ulong)((uint)stringMeta.EntryMetaOffset + num * 0x20U + 8U)), SeekOrigin.Begin);
          item.OffsetTs = (uint)this._d2TextMs.Position + Helpers.ReadUInt(this._d2TextMs);
          this._d2TextMs.Seek((long)((ulong)((uint)stringMeta.EntryMetaOffset + num * 0x20U + 0x14U)), SeekOrigin.Begin);
          item.ReadLength = Helpers.ReadUShort(this._d2TextMs);
          item.StringLength = Helpers.ReadUShort(this._d2TextMs);
          item.Obfuscator = Helpers.ReadUShort(this._d2TextMs);
          list.Add(item);
        }
        this._allEntryMetas.Add(list);
      }
    }

    private void GetStringLists()
    {
      uint num = 0U;
      int num2 = 0;
            foreach (List<EntryMeta> list in this._allEntryMetas)
            {
                num2++;
                if (num2 == 0x69)
                {
                    Helpers.FileExistsDelete("extracted/atextcount.log");
                }
                string text = "";
                foreach (EntryMeta entryMeta in list)
                {
                    this._d2TextMs.Seek((long)((ulong)entryMeta.OffsetTs), SeekOrigin.Begin);
                    byte[] array = new byte[entryMeta.ReadLength];
                    this._d2TextMs.Read(array, 0, entryMeta.ReadLength);
                    if (entryMeta.Obfuscator == 0xE142)
                    {
                        text += "[Arc Kill]";
                    }
                    else if (entryMeta.Obfuscator == 0xE13F)
                    {
                        text += "[Solar Kill]";
                    }
                    else if (entryMeta.Obfuscator == 0xE143)
                    {
                        text += "[Void Kill]";
                    }
                    else
                    {
                        try
                        {
                            text += PkgText.D2TextEncoding(array, entryMeta.Obfuscator);
                        }
                        catch (Exception)
                        {
                            text += "GinsorTool.Error";
                        }
                    }
                }
                TextResult item;
                if (!this._d2TextParam.StringHashList.ContainsKey(num))
                {
                    continue;
                }
                item = new TextResult
                {
                    TextRefHash = Helpers.InvertUint32(this._pkgFile.Filehash),

                    StringHash = this._d2TextParam.StringHashList[num],

                    StringText = text
                };

                this._textResults.Add(item);
                this._stringListWithIds.Add("[" + Helpers.InvertUint32(this._d2TextParam.StringHashList[num]).ToString("X8") + "]: " + text);

                num += 1U;
            }
    }

    public static string D2TextEncoding(byte[] unobf, uint obf)
    {
      byte[] array = new byte[unobf.Length];
      int i = 0;
      while (i < unobf.Length)
      {
        if (unobf[i] <= 0x7F)
        {
          if (unobf[i] + obf < 0x7FU)
          {
            array[i] = (byte)(unobf[i] + obf);
          }
          else
          {
            array[i] = 0;
          }
          i++;
        }
        else if (i <= unobf.Length - 3)
        {
          byte[] array2 = new byte[3];
          byte[] array3 = new byte[3];
          array2[0] = unobf[i];
          array2[1] = unobf[i + 1];
          array2[2] = (byte)(unobf[i + 2] + obf);
          array3[0] = unobf[i];
          array3[1] = (byte)(unobf[i + 1] + obf);
          array3[2] = (byte)(unobf[i + 2] + obf);
          byte[] array4 = BitConverter.GetBytes(BitConverter.ToUInt32(new byte[]
          {
            array2[0],
            array2[1],
            array2[2],
            0
          }.Reverse<byte>().ToArray<byte>(), 0) + 0xC0C000U).Reverse<byte>().Take(3).ToArray<byte>();
          int charCount = Encoding.UTF8.GetCharCount(array2);
          int charCount2 = Encoding.UTF8.GetCharCount(array3);
          int charCount3 = Encoding.UTF8.GetCharCount(array4);
          if (charCount == 1)
          {
            array[i] = array2[0];
            array[i + 1] = array2[1];
            array[i + 2] = array2[2];
            i += 3;
          }
          else if (charCount3 == 1)
          {
            array[i] = array4[0];
            array[i + 1] = array4[1];
            array[i + 2] = array4[2];
            i += 3;
          }
          else if (charCount2 == 3)
          {
            byte[] bytes = BitConverter.GetBytes(BitConverter.ToUInt16(array3.Take(2).Reverse<byte>().ToArray<byte>(), 0) + 0xC0);
            array[i] = bytes[1];
            array[i + 1] = bytes[0];
            i += 2;
          }
          else if (charCount2 == 2)
          {
            array[i] = array3[0];
            array[i + 1] = array3[1];
            i += 2;
          }
        }
        else if (i <= unobf.Length - 2)
        {
          byte[] array5 = new byte[]
          {
            unobf[i],
            (byte)(unobf[i + 1] + obf)
          };
          int charCount4 = Encoding.UTF8.GetCharCount(array5);
          if (charCount4 == 2)
          {
            byte[] bytes2 = BitConverter.GetBytes(BitConverter.ToUInt16(array5.Take(2).Reverse<byte>().ToArray<byte>(), 0) + 0xC0);
            array[i] = bytes2[1];
            array[i + 1] = bytes2[0];
            i += 2;
          }
          else if (charCount4 == 1)
          {
            array[i] = array5[0];
            array[i + 1] = array5[1];
            i += 2;
          }
          if (i <= unobf.Length - 2)
          {
          }
        }
        else
        {
          array[i] = 0;
          i++;
        }
      }
      string text = "";
      int j = 0;
      while (j < array.Length)
      {
        if (array[j] == 0)
        {
          text = (text ?? "");
          j++;
        }
        else if (array[j] <= 0x7F)
        {
          string str = text;
          char c = (char)array[j];
          text = str + c.ToString();
          j++;
        }
        else if (array[j] > 0x7F)
        {
          if (j <= array.Length - 3)
          {
            byte[] array6 = new byte[]
            {
              array[j],
              array[j + 1],
              array[j + 2]
            };
            int charCount5 = Encoding.UTF8.GetCharCount(array6);
            if (charCount5 == 1)
            {
              text += Encoding.UTF8.GetString(array6);
              j += 3;
            }
            if (charCount5 == 2)
            {
              text += Encoding.UTF8.GetString(array6.Take(2).ToArray<byte>());
              j += 2;
            }
            else if (charCount5 == 3)
            {
              string str2 = text;
              char c = (char)array6[1];
              text = str2 + c.ToString();
              j += 2;
            }
          }
          else
          {
            byte[] array7 = new byte[]
            {
              array[j],
              array[j + 1]
            };
            int charCount6 = Encoding.UTF8.GetCharCount(array7);
            if (charCount6 == 1)
            {
              text += Encoding.UTF8.GetString(array7);
            }
            else if (charCount6 == 2)
            {
              string str3 = text;
              char c = (char)array7[1];
              text = str3 + c.ToString();
            }
            j += 2;
          }
        }
      }
      return text;
    }

    private List<PkgFile> GetD2Files(PkgStream pkgStream)
    {
      PkgText.GetFilesHelper0 getFilesHelper = new PkgText.GetFilesHelper0
      {
        PkgStream = pkgStream,
        TextInstance = this
      };

      Parallel.ForEach<PkgEntry>(getFilesHelper.PkgStream.PkgEntryList, new Action<PkgEntry>(getFilesHelper.GetFiles0));
      return this._d2Files;
    }

    private List<PkgFile> GetD2Files(List<PkgStream> pkgStreams)
    {
      using (List<PkgStream>.Enumerator enumerator = pkgStreams.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          PkgText.GetFilesHelper getFilesHelper = new PkgText.GetFilesHelper
          {
            TextInstance = this,
            PkgStream = enumerator.Current
          };
   
                    Parallel.ForEach<PkgEntry>(getFilesHelper.PkgStream.PkgEntryList, new Action<PkgEntry>(getFilesHelper.GetFiles0));
                }
      }
      return this._d2Files;
    }

    private PkgFile _pkgFile;

    private readonly List<PkgFile> _d2Files = new List<PkgFile>();

    private D2TextParam _d2TextParam;

    private MemoryStream _d2TextMs;

    private TextHeader _textHeader;

    private List<StringMeta> _stringMetas;

    private List<List<EntryMeta>> _allEntryMetas;

    private List<TextResult> _textResults = new List<TextResult>();

    private List<string> _stringListWithIds = new List<string>();

    [CompilerGenerated]
    [Serializable]
    private sealed class SaveTextHelper0
    {
      // Note: this type is marked as 'beforefieldinit'.
      static SaveTextHelper0()
      {
      }

      public SaveTextHelper0()
      {
      }

      internal string SaveTexts15(TextResult tr)
      {
        return tr.StringText;
      }

      internal string SaveAllInOne0(TextResult tr)
      {
        return tr.StringText;
      }

      public static readonly PkgText.SaveTextHelper0 SaveTextHelperInstance = new PkgText.SaveTextHelper0();
    }

    [CompilerGenerated]
    private sealed class GetFilesHelper0
    {
      public GetFilesHelper0()
      {
      }

      internal void GetFiles0(PkgEntry pkgListEntry)
      {
        PkgFile pkgFile = new PkgFile(this.PkgStream, pkgListEntry.EntryNumber);
        if (pkgFile.PkgFileType == PkgFileType.Text)
        {
          List<PkgFile> d2Files = this.TextInstance._d2Files;
          lock (d2Files)
          {
            this.TextInstance._d2Files.Add(pkgFile);
          }
        }
      }

      public PkgStream PkgStream;

      public PkgText TextInstance;
    }

    [CompilerGenerated]
    private sealed class GetFilesHelper
    {
      public GetFilesHelper()
      {
      }

      internal void GetFiles0(PkgEntry pkgListEntry)
      {
        PkgFile pkgFile = new PkgFile(this.PkgStream, pkgListEntry.EntryNumber);
        if (pkgFile.PkgFileType == PkgFileType.Text)
        {
          List<PkgFile> d2Files = this.TextInstance._d2Files;
          lock (d2Files)
          {
            this.TextInstance._d2Files.Add(pkgFile);
          }
        }
      }

      public PkgStream PkgStream;

      public PkgText TextInstance;
    }
  }
}
