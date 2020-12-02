using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GinsorAudioTool2Plus
{
  public class PkgFile
  {
    public PkgFile(string pkgIn, uint fnumberIn)
    {
      this.Fnumber = fnumberIn;
      this.Pkg = pkgIn;
      this.GetPkgStream();
      this.GetFilename(this.Fnumber);
      this.Filehash = Helpers.InvertUint32(Calculation.GetHash(this.Filename));
      this.StartProcess();
    }

    public PkgFile(PkgStream pkgStream, uint fnumber)
    {
      this.Fnumber = fnumber;
      this._pkgStream = pkgStream;
      this.GetPkgFile();
      this.GetFilename(fnumber);
      this.Filehash = Helpers.InvertUint32(Calculation.GetHash(this.Filename));
      this.PkgEntry = this._pkgStream.PkgEntryList[(int)fnumber];
      this.Nonce = pkgStream.Nonce;
      this.PackageId = pkgStream.Header.PackageId;
      this.LangId = pkgStream.Header.LangId;
      this.PkgFileType = FileClassification.SetD2Filetype(this.PkgEntry);
      this.GetBlockEntries();
    }

    public PkgFile(PkgStream pkgStream)
    {
      this._pkgStream = pkgStream;
      this.GetPkgFile();
      this.Nonce = pkgStream.Nonce;
      this.PackageId = pkgStream.Header.PackageId;
      this.LangId = pkgStream.Header.LangId;
    }

    public PkgFile(uint filehashIn)
    {
      this.Fnumber = Calculation.CalcFilenumber(filehashIn);
      this.Filehash = filehashIn;
      this.GetPkgFile(this.Filehash);
      this.GetPkgStream();
      this.GetFilename(this.Fnumber);
      this.StartProcess();
    }

    public PkgFile(string filenameIn)
    {
      this.Filehash = Calculation.GetHash(filenameIn);
      this.Fnumber = Calculation.CalcFilenumber(this.Filehash);
      this.GetPkgFile(this.Filehash);
      this.GetPkgStream();
      this.GetFilename(this.Fnumber);
      this.StartProcess();
    }

    public void StartProcess()
    {
      while (this._state != 0xA && this._state != -1)
      {
        try
        {
          switch (this._state)
          {
            case 0:
              this.GetPkgEntry();
              this._state = 1;
              break;
            case 1:
              this.PkgFileType = FileClassification.SetD2Filetype(this.PkgEntry);
              this._state = 2;
              break;
            case 2:
              this.GetBlockEntries();
              this._state = 3;
              break;
            case 3:
              this.MakeNonce(this._pkgStream.Header.PackageId);
              this._state = 4;
              break;
            case 4:
              this.GetPackageId();
              this._state = 5;
              break;
            case 5:
              this.GetLangId();
              this._state = 7;
              break;
            case 7:
              this._state = 0xA;
              break;
          }
        }
        catch (Exception)
        {
          this._state = -1;
        }
      }
    }

    private void GetPkgEntry()
    {
      this.PkgEntry = this._pkgStream.PkgEntryList[(int)this.Fnumber];
    }

    private void GetPkgStream()
    {
      this._pkgStream = PkgStream.PkgStreamFromFile(this.Pkg);
    }

    private void GetPkgFile()
    {
      List<PkgListEntry> list = Form1.RecvPkgListEntries();
      int index = list.FindIndex(new Predicate<PkgListEntry>(this.GetPkgFile21));
      PkgListEntry pkgListEntry = list[index];
      string text = Form1.RecD2PkgDir();
      this.Pkg = string.Concat(new string[]
      {
        text,
        pkgListEntry.Basename,
        "_",
        pkgListEntry.PatchId.ToString(""),
        ".pkg"
      });
    }

    private void GetPkgFile(uint filehashIn)
    {
      PkgFile.GetPkgFileHelper0 getPkgFileHelper = new PkgFile.GetPkgFileHelper0
      {
        PackageId = Calculation.CalcPackageId(filehashIn)
      };
      List<PkgListEntry> list = Form1.RecvPkgListEntries();
      int index = list.FindIndex(new Predicate<PkgListEntry>(getPkgFileHelper.GetPkgFile0));
      PkgListEntry pkgListEntry = list[index];
      string text = Form1.RecD2PkgDir();
      this.Pkg = string.Concat(new string[]
      {
        text,
        pkgListEntry.Basename,
        "_",
        pkgListEntry.PatchId.ToString(""),
        ".pkg"
      });
    }

    private void GetFilename(uint fnumberIn)
    {
      this.Filename = string.Concat(new string[]
      {
        this._pkgStream.Header.PackageId.ToString("X4"),
        "_",
        fnumberIn.ToString("X4"),
        "_",
        this._pkgStream.Header.LangId.ToString("X2"),
        ".bin"
      });
    }

    private void GetBlockEntries()
    {
      for (uint num = this.PkgEntry.StartBlock; num < this.PkgEntry.StartBlock + this.PkgEntry.BlockCount; num += 1U)
      {
        BlockEntry item = this._pkgStream.BlockEntryList[(int)num];
        this.BlockEntries.Add(item);
      }
    }

    public void MakeNonce(ushort packageId)
    {

     this.Nonce = this._baseNonce;
      byte[] nonce = this.Nonce;
      int num = 0;
      nonce[num] ^= (byte)(packageId >> 8 & 0xFF);
      byte[] nonce2 = this.Nonce;
      int num2 = 1;
      nonce2[num2] ^= 0x35;
      byte[] nonce3 = this.Nonce;
      int num3 = 0xB;
      nonce3[num3] ^= (byte)(packageId & 0xFF);

    }

    private void GetPackageId()
    {
      this.PackageId = this._pkgStream.Header.PackageId;
    }

    private void GetLangId()
    {
      this.LangId = this._pkgStream.Header.LangId;
    }

    public void Test(uint fnumber)
    {
      this.Fnumber = fnumber;
      this.GetFilename(fnumber);
      this.Filehash = Helpers.InvertUint32(Calculation.GetHash(this.Filename));
      this.PkgEntry = this._pkgStream.PkgEntryList[(int)fnumber];
      this.PkgFileType = FileClassification.SetD2Filetype(this.PkgEntry);
      this.GetBlockEntries();
    }

    [CompilerGenerated]
    private bool GetPkgFile21(PkgListEntry c)
    {
      return c.PackageId == this._pkgStream.Header.PackageId && c.LangId == this._pkgStream.Header.LangId;
    }

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

    private int _state;

    private PkgStream _pkgStream;

    public string Pkg;

    public uint Filehash;

    public string Filename;

    public PkgEntry PkgEntry;

    public PkgFileType PkgFileType;

    public List<BlockEntry> BlockEntries = new List<BlockEntry>();

    public byte[] Nonce;

    public ushort PackageId;

    public uint Fnumber;

    public ushort LangId;

    [CompilerGenerated]
    private sealed class GetPkgFileHelper0
    {
      public GetPkgFileHelper0()
      {
      }

      internal bool GetPkgFile0(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      public uint PackageId;
    }
  }
}
