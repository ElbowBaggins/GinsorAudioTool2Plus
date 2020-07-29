using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace GinsorAudioTool2Plus
{
  internal class Calculation
  {
    public static PkgFile Hash2File(string hashString)
    {
      Calculation.Hash2FileHelper0 hash2FileHelper = new Calculation.Hash2FileHelper0();
      hashString = hashString.Replace(" ", "");
      uint num = Helpers.InvertUint32(Convert.ToUInt32(hashString, 0x10));
      hash2FileHelper.PackageId = Calculation.CalcPackageId(num);
      Calculation.CalcFilenumber(num);
      List<PkgListEntry> list = Form1.RecvPkgListEntries();
      int num2 = list.FindIndex(new Predicate<PkgListEntry>(hash2FileHelper.Hash2File0));
      if (num2 == -1)
      {
        num = Helpers.InvertUint32(num);
        hash2FileHelper.PackageId = Calculation.CalcPackageId(num);
        num2 = list.FindIndex(new Predicate<PkgListEntry>(hash2FileHelper.Hash2File1));
      }
      PkgListEntry pkgListEntry = list[num2];
      string text = Form1.RecD2PkgDir();
      string.Concat(new string[]
      {
        text,
        pkgListEntry.Basename,
        "_",
        pkgListEntry.PatchId.ToString(""),
        ".pkg"
      });
      return new PkgFile(num);
    }

    public static PkgFile Hash2File(uint hash)
    {
      Calculation.HashToFileHelper1 hashToFileHelper = new Calculation.HashToFileHelper1
      {
        PackageId = Calculation.CalcPackageId(hash)
      };
      bool flag = hashToFileHelper.PackageId == 0x513U;
      if (flag)
      {
        hashToFileHelper.PackageId = 0x913U;
      }
      Calculation.CalcFilenumber(hash);
      List<PkgListEntry> list = Form1.RecvPkgListEntries();
      int num = list.FindIndex(new Predicate<PkgListEntry>(hashToFileHelper.HashToFile0));
      bool flag2 = num == -1;
      if (flag2)
      {
        hash = Helpers.InvertUint32(hash);
        hashToFileHelper.PackageId = Calculation.CalcPackageId(hash);
        num = list.FindIndex(new Predicate<PkgListEntry>(hashToFileHelper.Hash2File1));
      }
      PkgListEntry pkgListEntry = list[num];
      string text = Form1.RecD2PkgDir();
      string.Concat(new string[]
      {
        text,
        pkgListEntry.Basename,
        "_",
        pkgListEntry.PatchId.ToString(""),
        ".pkg"
      });
      return new PkgFile(hash);
    }

    public static PkgFile File2Hash(string filename)
    {
      return new PkgFile(Calculation.GetHash(filename));
    }

    public static uint GetHash(string filename)
    {
      filename = Path.GetFileName(filename);
      uint num = Convert.ToUInt32(filename.Substring(0, 2), 0x10);
      uint num2 = Convert.ToUInt32(filename.Substring(2, 2), 0x10);
      uint num3 = Convert.ToUInt32(filename.Substring(5, 4), 0x10);
      return ((0x404U + num) * 0x100U + num2 << 0xD) + num3;
    }

    public static uint getHash_OLD(string filename)
    {
      uint num = Convert.ToUInt32(filename.Substring(0, 4), 0x10);
      uint num2 = Convert.ToUInt32(filename.Substring(5, 4), 0x10);
      uint num3 = 0x80000000U;
      uint num4 = 0x81FFF000U;
      uint result = 0U;
      while (num3 <= num4)
      {
        uint num5 = Calculation.CalcPackageId(num3 + num2);
        uint num6 = Calculation.CalcFilenumber(num3 + num2);
        if (num5 == num & num6 == num2)
        {
          result = num3 + num2;
          if (num >= 0x500U)
          {
            break;
          }
        }
        num3 += 0x1000U;
      }
      return result;
    }

    public static uint CalcPackageId(uint hash)
    {
      bool flag = hash <= 0x80FFFFFFU;
      bool flag2 = flag;
      uint num;
      if (flag2)
      {
        num = (hash >> 0xD & 0x3FFU);
      }
      else
      {
        num = ((hash >> 0xD & 0x3FFU) | 0x400U);
      }
      uint num2 = num;
      uint result;
      if (num2 != 0x519U)
      {
        if (num2 != 0x539U)
        {
          result = num;
        }
        else
        {
          result = 0x939U;
        }
      }
      else
      {
        result = 0x919U;
      }
      return result;
    }

    public static uint CalcFilenumber(uint hash)
    {
      return hash & 0x1FFFU;
    }

    public Calculation()
    {
    }

    [CompilerGenerated]
    private sealed class Hash2FileHelper0
    {
      public Hash2FileHelper0()
      {
      }

      internal bool Hash2File0(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      internal bool Hash2File1(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      public uint PackageId;
    }

    [CompilerGenerated]
    private sealed class Hash2FileHelper2
    {
      public Hash2FileHelper2()
      {
      }

      internal bool Hash2File0(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      internal bool Hash2File1(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      public uint PackageId;
    }

    [CompilerGenerated]
    private sealed class HashToFileHelper1
    {
      public HashToFileHelper1()
      {
      }

      internal bool HashToFile0(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      internal bool Hash2File1(PkgListEntry c)
      {
        return c.PackageId == this.PackageId && (c.LangId == 0 || c.LangId == 1);
      }

      public uint PackageId;
    }
  }
}
