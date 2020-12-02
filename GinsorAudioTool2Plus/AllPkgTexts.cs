using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GinsorAudioTool2Plus
{
  internal class AllPkgTexts
  {
    public AllPkgTexts()
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
            if (num == 6)
            {
              num = 0xA;
            }
          }
          else
          {
            this.GenerateList();
            num = 6;
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
      AllPkgTexts.GenerateListHelper0 generateListHelper = new AllPkgTexts.GenerateListHelper0();
      IEnumerable<PkgListEntry> source = Form1.RecvPkgListEntries();
      generateListHelper.D2PkgDir = Form1.RecD2PkgDir();
      string text = Form1.RecAllTextDbFile();
      generateListHelper.PkgStreamList = new List<PkgStream>();
      Parallel.ForEach<PkgListEntry>(source, new Action<PkgListEntry>(generateListHelper.GenerateList0));
      List<TextResult> textResults = new PkgText(generateListHelper.PkgStreamList).GetTextResults2();
      Helpers.FileExistsDelete(text);
      File.WriteAllText(text, JsonConvert.SerializeObject(textResults));
    }

    [CompilerGenerated]
    private sealed class GenerateListHelper1
    {
      public GenerateListHelper1()
      {
      }

      internal void GenerateList0(PkgListEntry pkgListEntry)
      {
        PkgStream item = PkgStream.PkgStreamFromFile(string.Concat(new string[]
        {
          this.D2PkgDir,
          pkgListEntry.Basename,
          "_",
          pkgListEntry.PatchId.ToString(""),
          ".pkg"
        }));
        List<PkgStream> obj = this.PkgStreamList;
        lock (obj)
        {
          this.PkgStreamList.Add(item);
        }
      }

      public string D2PkgDir;

      public List<PkgStream> PkgStreamList;
    }

    [CompilerGenerated]
    private sealed class GenerateListHelper0
    {
      public GenerateListHelper0()
      {
      }

            internal void GenerateList0(PkgListEntry pkgListEntry)
            {

                PkgStream item = PkgStream.PkgStreamFromFile(string.Concat(new string[]
                {
          this.D2PkgDir,
          pkgListEntry.Basename,
          "_",
          pkgListEntry.PatchId.ToString(""),
          ".pkg"
                }));
                List<PkgStream> list = this.PkgStreamList;
                List<PkgStream> obj = list;
                lock (obj)
                {
                    list.Add(item);
                }

            }

      public string D2PkgDir;

      public List<PkgStream> PkgStreamList;
    }
  }
}
