#pragma warning disable 649
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace GinsorAudioTool2Plus
{
  internal class PkgList
  {
    public PkgList(string d2PkgDirInput, string pkglistFileInput)
    {
      this.StartProcess();
    }

    public void StartProcess()
    {
      int num = 0;
      while (num != 0xA && num != -1)
      {
        try
        {
          if (num != 0)
          {
            if (num != 1)
            {
              if (num == 6)
              {
                num = 0xA;
              }
            }
            else
            {
              this.WritePkgListFile();
              num = 6;
            }
          }
          else
          {
            this.GenerateList();
            num = 1;
          }
        }
        catch (Exception)
        {
          num = -1;
        }
      }
    }

    public void GenerateList()
    {
      foreach (string path in Directory.GetFiles(Form1.RecD2PkgDir(), "*.pkg"))
      {
        using (FileStream fileStream = File.OpenRead(path))
        {
          PkgList.GenerateListHelper2 generateListHelper = new PkgList.GenerateListHelper2();
          PkgStream pkgStream = new PkgStream(fileStream);
          generateListHelper.PkgListEntry = default(PkgListEntry);
          generateListHelper.PkgListEntry.PackageId = pkgStream.Header.PackageId;
          generateListHelper.PkgListEntry.PatchId = pkgStream.Header.PatchId;
          generateListHelper.PkgListEntry.LangId = pkgStream.Header.LangId;
          string[] array = Path.GetFileNameWithoutExtension(path).Split(new char[]
          {
            '_'
          });
          string[] array2 = new string[array.Length - 1];
          Array.Copy(array, array2, array2.Length);
          generateListHelper.PkgListEntry.Basename = string.Join("_", array2);
          PkgListEntry pkgListEntry = this.PkgListEntryList.Find(new Predicate<PkgListEntry>(generateListHelper.GenerateList0));
          bool flag = pkgListEntry.Equals(default(PkgListEntry));
          if (flag)
          {
            this.PkgListEntryList.Add(generateListHelper.PkgListEntry);
          }
          else
          {
            bool flag2 = pkgListEntry.PatchId < generateListHelper.PkgListEntry.PatchId;
            if (flag2)
            {
              pkgListEntry.PackageId = generateListHelper.PkgListEntry.PackageId;
              pkgListEntry.PatchId = generateListHelper.PkgListEntry.PatchId;
              pkgListEntry.LangId = generateListHelper.PkgListEntry.LangId;
              pkgListEntry.Basename = generateListHelper.PkgListEntry.Basename;
              int index = this.PkgListEntryList.FindIndex(new Predicate<PkgListEntry>(generateListHelper.GenerateList1));
              this.PkgListEntryList[index] = pkgListEntry;
            }
          }
        }
      }
    }

    public void WritePkgListFile()
    {
      string text = Form1.RecPkglistfile();
      Helpers.FileExistsDelete(text);
      Helpers.DirNotExistCreate(Path.GetDirectoryName(text));
      File.WriteAllText(text, JsonConvert.SerializeObject(this.PkgListEntryList));
    }

    public List<PkgListEntry> PkgListEntryList = new List<PkgListEntry>();

    [CompilerGenerated]
    private sealed class GenerateListHelper
    {
      public GenerateListHelper()
      {
      }

      internal bool GenerateList0(PkgListEntry c)
      {
        return c.PackageId == this.PkgListEntry.PackageId && c.LangId == this.PkgListEntry.LangId;
      }

      internal bool GenerateList1(PkgListEntry c)
      {
        return c.PackageId == this.PkgListEntry.PackageId && c.LangId == this.PkgListEntry.LangId;
      }

      public PkgListEntry PkgListEntry;
    }

    [CompilerGenerated]
    private sealed class GenerateListHelper2
    {
      public GenerateListHelper2()
      {
      }

      internal bool GenerateList0(PkgListEntry c)
      {
        return c.PackageId == this.PkgListEntry.PackageId && c.LangId == this.PkgListEntry.LangId;
      }

      internal bool GenerateList1(PkgListEntry c)
      {
        return c.PackageId == this.PkgListEntry.PackageId && c.LangId == this.PkgListEntry.LangId;
      }

      public PkgListEntry PkgListEntry;
    }
  }
}
