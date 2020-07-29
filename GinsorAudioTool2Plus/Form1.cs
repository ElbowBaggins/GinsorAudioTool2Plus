using NAudio.Vorbis;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GinsorAudioTool2Plus.Properties;
using ww2ogg;

namespace GinsorAudioTool2Plus
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      this.InitializeComponent();
      this.DeactivateButtons();
      this.IniFileLoad();
      this.UpdtEventLogger("Loading Databases...");
      this.LoadCaches();
    }

    public void IniFileLoad()
    {
      string path = "settings.ini";
      if (!File.Exists(path))
      {
        MessageBox.Show("Select your Destiny2.exe", "Path Selection", MessageBoxButtons.OK);
        this.CreateIni();
        return;
      }
      string[] array = File.ReadAllLines(path);
      if (array.Count<string>() == 0)
      {
        MessageBox.Show("Select your Destiny2.exe", "Path Selection", MessageBoxButtons.OK);
        this.UpdtEventLogger("Empty Settings File");
        this.CreateIni();
        return;
      }
      this.D2Dir = array[0] + "/";
      this.D2Dir = this.D2Dir.Replace("\\", "/").Replace("//", "/");
      if (File.Exists(this.D2Dir + "destiny2.exe"))
      {
        Form1.D2PkgDir = this.D2Dir + "packages/";
        this.UpdtEventLogger("Destiny 2 Directory:     " + this.D2Dir);
        return;
      }
      MessageBox.Show("Select your Destiny2.exe", "Path Selection", MessageBoxButtons.OK);
      this.UpdtEventLogger("Wrong Settings File");
      this.CreateIni();
    }

    public void CreateIni()
    {
      string path = Helpers.SelectD2Exe();
      this.D2Dir = Path.GetDirectoryName(path) + "/";
      this.D2Dir = this.D2Dir.Replace("\\", "/").Replace("//", "/");
      Form1.D2PkgDir = this.D2Dir + "packages/";
      Helpers.FileExistsDelete("settings.ini");
      File.AppendAllText("settings.ini", this.D2Dir);
      this.IniFileLoad();
    }

    public async void LoadCaches()
    {
      await this.LoadPkgList();
      await this.LoadAllTexts();
      await this.OpenTranscriptFile();
      this.ActivateButtons();
      this.UpdtEventLogger("Ready.");
    }

    public async Task LoadPkgList()
    {
      bool flag = File.Exists(Form1.PkglistFile);
      if (flag)
      {
        Form1.PkgListEntries.AddRange(JsonConvert.DeserializeObject<List<PkgListEntry>>(File.ReadAllText(Form1.PkglistFile)));
        this.UpdtEventLogger("PKG Database Loaded.");
      }
      else
      {
        this.UpdtEventLogger("Generating PKG Database. Please wait...");
        await Task.Run(new Action(Form1.LoadPkgListHelper000.Inst.loadPkgListb__0_0));
        this.UpdtEventLogger("Generating PKG Database completed.");
        await this.LoadPkgList();
      }
    }

    public async Task LoadAllTexts()
    {
      bool flag = File.Exists(Form1.AllTextsFile);
      if (flag)
      {
        Form1.AllTextsDb.AddRange(JsonConvert.DeserializeObject<List<TextResult>>(File.ReadAllText(Form1.AllTextsFile)));
        this.UpdtEventLogger("String Database Loaded.");
      }
      else
      {
        this.UpdtEventLogger("Generating String Database. Please wait...");
        await Task.Run(new Action(Form1.LoadAllTextsHelper000.Helpinst.loadAllTextsb__0_0));
        this.UpdtEventLogger("Generating String Database completed.");
        await this.LoadAllTexts();
      }
    }

    public static List<PkgListEntry> RecvPkgListEntries()
    {
      return Form1.PkgListEntries;
    }

    public static string RecPkglistfile()
    {
      return Form1.PkglistFile;
    }

    public static string RecD2PkgDir()
    {
      return Form1.D2PkgDir;
    }

    public static List<TextResult> RecAllTextDb()
    {
      return Form1.AllTextsDb;
    }

    public static string RecAllTextDbFile()
    {
      return Form1.AllTextsFile;
    }

    public void UpdtEventLogger(string logText)
    {
      this.tb_EventLogger.AppendText(string.Concat(new string[]
      {
        " [",
        DateTime.Now.ToString("hh:mm:ss tt"),
        "]: ",
        logText,
        Environment.NewLine
      }));
    }

    private void DisposeWave()
    {
      if (this._wavePlayer != null)
      {
        if (this._wavePlayer.PlaybackState == PlaybackState.Playing)
        {
          this._wavePlayer.Stop();
        }
        this._wavePlayer.Dispose();
        this._wavePlayer = null;
      }
      if (this._waveStream != null)
      {
        this._waveStream.Dispose();
        this._waveStream = null;
      }
    }

    public static void Revorb(MemoryStream audioMs, string outfile)
    {
      Helpers.FileExistsDelete(outfile);
      Helpers.DirNotExistCreate(Path.GetDirectoryName(outfile));
      File.WriteAllBytes(outfile, audioMs.ToArray());
      ProcessStartInfo processStartInfo = new ProcessStartInfo
      {
        CreateNoWindow = true,
        UseShellExecute = false,
        FileName = Form1.RevorberExe,
        WindowStyle = ProcessWindowStyle.Hidden,
        Arguments = outfile
      };
      try
      {
        using (Process process = Process.Start(processStartInfo))
        {
          process.WaitForExit();
        }
      }
      catch
      {
      }
    }

    private void DeactivateButtons()
    {
      this.topMenu_loadDefault.Enabled = false;
      this.topMenu_reloadDatabase.Enabled = false;
      this.tb_searchBox.Enabled = false;
      this.cb_narratorFilter.Enabled = false;
      this.btn_stopMusic.Enabled = false;
      this.btn_expandTreeview.Enabled = false;
      this.btn_collapseTreeview.Enabled = false;
    }

    private void ActivateButtons()
    {
      this.topMenu_loadDefault.Enabled = true;
      this.topMenu_reloadDatabase.Enabled = true;
      this.tb_searchBox.Enabled = true;
      this.cb_narratorFilter.Enabled = true;
      this.btn_stopMusic.Enabled = true;
      this.btn_expandTreeview.Enabled = true;
      this.btn_collapseTreeview.Enabled = true;
    }

    private async void TopMenu_reloadDatabase_Click(object sender, EventArgs e)
    {
      this.UpdtEventLogger("Clearing all Databases...");
      this.treeView1.Nodes.Clear();
      Form1.PkgListEntries = new List<PkgListEntry>();
      Form1.AllTextsDb = new List<TextResult>();
      Helpers.FileExistsDelete(Form1.DefaultJson);
      Helpers.FileExistsDelete(Form1.PkglistFile);
      Helpers.FileExistsDelete(Form1.AllTextsFile);
      Helpers.FileExistsDelete("settings.ini");
      this.UpdtEventLogger("Re-Generating all Databases...");
      this.DeactivateButtons();
      this.CreateIni();
      await this.LoadPkgList();
      await this.LoadAllTexts();
      await this.OpenTranscriptFile();
      this.UpdtEventLogger("Ready.");
    }

    private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (e.Node.Tag != null)
      {
        VoiceEntryLtd voiceEntryLtd = (VoiceEntryLtd)e.Node.Tag;
        string logText = voiceEntryLtd.Narrator + ": " + voiceEntryLtd.Text;
        float length = voiceEntryLtd.Length;
        uint entryHash = voiceEntryLtd.EntryHash;
        string text = null;
        this.UpdtEventLogger("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        this.UpdtEventLogger(logText);
        this.UpdtEventLogger("Entryhash: " + entryHash.ToString("D10"));
        if (voiceEntryLtd.AudioCollectionFileHash != 0xFFFFFFFFU)
        {
          text = string.Concat(new string[]
          {
            Form1.StdOutputDir,
            "/",
            entryHash.ToString(""),
            "_",
            Calculation.Hash2File(voiceEntryLtd.AudioCollectionFileHash).Filename.Replace(".bin", ".ogg")
          });
          this.UpdtEventLogger(string.Concat(new string[]
          {
            "Audio: ",
            Path.GetFileName(text),
            "  (",
            length.ToString("N2"),
            " s)"
          }));
        }
        this.DisposeWave();
        if (voiceEntryLtd.AudioCollectionFileHash != 0xFFFFFFFFU)
        {
          PkgFile pkgFile = new PkgFile(voiceEntryLtd.AudioCollectionFileHash);
          MemoryStream memoryStream = new MemoryStream(new PkgExtract().ToBuffer(pkgFile));
          memoryStream.Seek(0x20L, SeekOrigin.Begin);
          uint num = (uint)memoryStream.Position + Helpers.ReadUInt(memoryStream) + 0x10U;
          memoryStream.Seek((long)((ulong)num), SeekOrigin.Begin);
          uint filehashIn = Helpers.ReadUInt(memoryStream);
          memoryStream.Close();
          PkgFile pkgFile2 = new PkgFile(filehashIn);
          MemoryStream memoryStream2 = new MemoryStream(new PkgExtract().ToBuffer(pkgFile2));
          if (Helpers.ReadUInt(memoryStream2) != 0x46464952U)
          {
            this.UpdtEventLogger("No Audiofile for selected Transcript.");
            return;
          }
          memoryStream2.Seek(0L, SeekOrigin.Begin);
          MemoryStream memoryStream3 = Ww2Ogg.ww2ogg(memoryStream2, new MemoryStream(Resources.CodeBook));
          memoryStream2.Close();
          MemoryStream memoryStream4 = new MemoryStream(memoryStream3.ToArray());
          MemoryStream revorbedStream = new MemoryStream();
          GinsorAudioTool2Plus.Revorb.revorb(memoryStream4, revorbedStream);
          revorbedStream.Seek(0, SeekOrigin.Begin);
          this._waveStream = new VorbisWaveReader(revorbedStream);
          this._wavePlayer = new DirectSoundOut();
          this._wavePlayer.Init(this._waveStream);
          this._wavePlayer.Play();
          if (this.cb_saveAudio.Checked) {
            Helpers.FileExistsDelete(text);
            Helpers.DirNotExistCreate(Path.GetDirectoryName(text));
            File.WriteAllBytes(text, revorbedStream.ToArray());
          }
        }
        else
        {
          this.UpdtEventLogger("No Audiofile for selected Transcript.");
        }
      }
    }

    private void Btn_stopMusic_Click(object sender, EventArgs e)
    {
      this.DisposeWave();
    }

    private void Btn_expandTreeview_Click(object sender, EventArgs e)
    {
      this.treeView1.BeginUpdate();
      this.treeView1.ExpandAll();
      this.treeView1.EndUpdate();
      this.treeView1.Nodes[0].EnsureVisible();
    }

    private void Btn_collapseTreeview_Click(object sender, EventArgs e)
    {
      this.treeView1.BeginUpdate();
      this.treeView1.CollapseAll();
      this.treeView1.EndUpdate();
      this.treeView1.Nodes[0].EnsureVisible();
    }

    private void Tb_searchBox_MouseClick(object sender, MouseEventArgs e)
    {
      if (this.tb_searchBox.Text == "Search...")
      {
        this.tb_searchBox.Text = "";
      }
    }

    private async void Tb_searchBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Return && this._allVoiceListLtd.Count > 0)
      {
        Form1.SearchTermHelper searchTermHelper = new Form1.SearchTermHelper();
        this.treeView1.Nodes.Clear();
        searchTermHelper.SearchTerm = this.tb_searchBox.Text;
        this.treeView1.BeginUpdate();
        int num = 0;
        int num2 = 0;
        searchTermHelper.Counter = 0;
        using (List<List<VoiceEntryLtd>>.Enumerator enumerator1 = this._allVoiceListLtd.GetEnumerator())
        {
          while (enumerator1.MoveNext())
          {
            Form1.VoiceEntryHelper1 voiceEntryHelper = new Form1.VoiceEntryHelper1
            {
              Locals = searchTermHelper,
              VoiceEntry = enumerator1.Current
            };
            Form1.SearchBoxKeyDownHandler searchBoxKeyDownHandler = new Form1.SearchBoxKeyDownHandler
            {
              Locals = voiceEntryHelper,
              Checker = 0,
              FileListTreeNodes = new List<TreeNode>()
            };
            await Task.Run(new Action(searchBoxKeyDownHandler.OnKeyDown));
            if (searchBoxKeyDownHandler.Checker == 1)
            {
              this.treeView1.Nodes.Add(string.Concat(new object[]
              {
                "Collection ",
                num2.ToString("D3"),
                "   (",
                searchBoxKeyDownHandler.FileListTreeNodes.Count,
                " Entries)"
              }));
              if (searchBoxKeyDownHandler.FileListTreeNodes.Count > 0)
              {
                this.treeView1.Nodes[num].Nodes.AddRange(searchBoxKeyDownHandler.FileListTreeNodes.ToArray());
                this.treeView1.Nodes[num].Expand();
              }
              num++;
            }
            num2++;
            searchBoxKeyDownHandler = null;
          }
        }
        this.treeView1.ShowNodeToolTips = true;
        this.treeView1.EndUpdate();
        this.UpdtEventLogger(searchTermHelper.Counter + " Voice Lines for " + searchTermHelper.SearchTerm);
        searchTermHelper = null;
      }
    }

    private async void Cb_narratorFilter_DropDownClosed(object sender, EventArgs e)
    {
      if (this._allVoiceListLtd.Count > 0)
      {
        Form1.SearchTermHelper0 searchTermHelper = new Form1.SearchTermHelper0();
        this.treeView1.Nodes.Clear();
        searchTermHelper.SearchTerm = this.cb_narratorFilter.Text;
        this.treeView1.BeginUpdate();
        int num = 0;
        int num2 = 0;
        searchTermHelper.Counter = 0;
        using (List<List<VoiceEntryLtd>>.Enumerator enumerator1 = this._allVoiceListLtd.GetEnumerator())
        {
          while (enumerator1.MoveNext())
          {
            Form1.VoiceEntryHelper0 voiceEntryHelper = new Form1.VoiceEntryHelper0
            {
              Locals = searchTermHelper,
              VoiceEntry = enumerator1.Current
            };
            Form1.NarratorFilterDdClosedHelper narratorFilterDdClosedHelper = new Form1.NarratorFilterDdClosedHelper
            {
              Locals = voiceEntryHelper,
              Checker = 0,
              FileListTreeNodes = new List<TreeNode>()
            };
            await Task.Run(new Action(narratorFilterDdClosedHelper.OnDdClosed));
            if (narratorFilterDdClosedHelper.Checker == 1)
            {
              this.treeView1.Nodes.Add(string.Concat(new object[]
              {
                "Collection ",
                num2.ToString("D3"),
                "   (",
                narratorFilterDdClosedHelper.FileListTreeNodes.Count,
                " Entries)"
              }));
              if (narratorFilterDdClosedHelper.FileListTreeNodes.Count > 0)
              {
                this.treeView1.Nodes[num].Nodes.AddRange(narratorFilterDdClosedHelper.FileListTreeNodes.ToArray());
                this.treeView1.Nodes[num].Expand();
              }
              num++;
            }
            num2++;
            narratorFilterDdClosedHelper = null;
          }
        }
        this.treeView1.ShowNodeToolTips = true;
        this.treeView1.EndUpdate();
        this.UpdtEventLogger(searchTermHelper.Counter + " Voice Lines for Narrator " + searchTermHelper.SearchTerm);
        searchTermHelper = null;
      }
    }

    private void TopMenu_about_Click(object sender, EventArgs e)
    {
      new FormAbout().ShowDialog();
    }

    private async void TopMenu_loadDefault_Click(object sender, EventArgs e)
    {
      await this.OpenTranscriptFile();
    }

    private async Task OpenTranscriptFile()
    {
      if (File.Exists(Form1.DefaultJson))
      {
        Form1.NarratorListHelper narratorListHelper = new Form1.NarratorListHelper();
        this.treeView1.Visible = false;
        this.treeView1.Nodes.Clear();
        this.treeView1.BeginUpdate();
        Stream stream = new MemoryStream(Encryption.Decrypt(File.ReadAllBytes(Form1.DefaultJson)));
        this.UpdtEventLogger("Loading Transcript Database... (" + Form1.DefaultJson + ")");
        using (StreamReader streamReader = new StreamReader(stream))
        {
          JsonSerializer jsonSerializer = new JsonSerializer();
          this._allVoiceListLtd = (List<List<VoiceEntryLtd>>)jsonSerializer.Deserialize(streamReader, typeof(List<List<VoiceEntryLtd>>));
        }
        int num = 0;
        int num2 = 0;
        narratorListHelper.NarratorList = new List<string>();
        using (List<List<VoiceEntryLtd>>.Enumerator enumerator1 = this._allVoiceListLtd.GetEnumerator())
        {
          while (enumerator1.MoveNext())
          {
            Form1.VoiceEntryHelper voiceEntryHelper = new Form1.VoiceEntryHelper
            {
              Locals = narratorListHelper,
              VoiceEntry = enumerator1.Current
            };
            Form1.OpenTranscriptFileHelper openTranscriptFileHelper = new Form1.OpenTranscriptFileHelper
            {
              Locals = voiceEntryHelper,
              FileListTreeNodes = new List<TreeNode>()
            };
            this.treeView1.Nodes.Add(string.Concat(new object[]
            {
              "Collection ",
              num.ToString("D3"),
              "   (",
              openTranscriptFileHelper.Locals.VoiceEntry.Count,
              " Entries)"
            }));
            await Task.Run(new Action(openTranscriptFileHelper.openTranscriptFileb__0));
            if (openTranscriptFileHelper.FileListTreeNodes.Count > 0)
            {
              this.treeView1.Nodes[num].Nodes.AddRange(openTranscriptFileHelper.FileListTreeNodes.ToArray());
              this.treeView1.Nodes[num].Expand();
            }
            num++;
            num2 += openTranscriptFileHelper.Locals.VoiceEntry.Count<VoiceEntryLtd>();
            openTranscriptFileHelper = null;
          }
        }
        this.treeView1.ShowNodeToolTips = true;
        this.treeView1.EndUpdate();
        this.treeView1.Visible = true;
        narratorListHelper.NarratorList = narratorListHelper.NarratorList.Distinct<string>().ToList<string>();
        narratorListHelper.NarratorList.Add("");
        narratorListHelper.NarratorList.Sort();
        this.cb_narratorFilter.BeginUpdate();
        this.cb_narratorFilter.Items.Clear();
        this.cb_narratorFilter.Items.AddRange(narratorListHelper.NarratorList.ToArray());
        this.cb_narratorFilter.EndUpdate();
        this.UpdtEventLogger(num2.ToString("N0") + " Transcripts Loaded.");
        narratorListHelper = null;
      }
      else
      {
        await this.GenerateTranscriptDb();
      }
    }

    private async Task GenerateTranscriptDb()
    {
      Form1.GenerateTranscriptDbHelper0 generateTranscriptDbHelper = new Form1.GenerateTranscriptDbHelper0();
      this.UpdtEventLogger("Generating Transcript Database. Please wait...");
      Helpers.FileExistsDelete(Form1.DefaultJson);
      generateTranscriptDbHelper.PkgStreamList = new List<PkgStream>();
      await Task.Run(new Action(generateTranscriptDbHelper.generateTranscriptDbb__0));
      generateTranscriptDbHelper.AllVoiceListLtdTmp = new List<List<VoiceEntryLtd>>();
      await Task.Run(new Action(generateTranscriptDbHelper.generateTranscriptDbb__1));
      Helpers.FileExistsDelete(Form1.DefaultJson);
      Helpers.DirNotExistCreate(Path.GetDirectoryName(Form1.DefaultJson));
      string s = JsonConvert.SerializeObject(generateTranscriptDbHelper.AllVoiceListLtdTmp, Formatting.Indented);
      byte[] bytes = Encoding.UTF8.GetBytes(s);
      File.WriteAllBytes(Form1.DefaultJson, Encryption.Encrypt(bytes));
      this.UpdtEventLogger("Generating Transcript Database completed.");
      await this.OpenTranscriptFile();
    }

    public string D2Dir;

    public static string D2PkgDir;

    public static string StdOutputDir = "ogg/";

    public static string DefaultJson = AppDomain.CurrentDomain.BaseDirectory + "transcripts.json";

    public static string RevorberExe = "res/revorb.exe";

    public static List<PkgListEntry> PkgListEntries = new List<PkgListEntry>();

    private static readonly string PkglistFile = AppDomain.CurrentDomain.BaseDirectory + "pkg.json";

    public static List<TextResult> AllTextsDb = new List<TextResult>();

    public static string AllTextsFile = AppDomain.CurrentDomain.BaseDirectory + "strings.json";

    private DirectSoundOut _wavePlayer;

    private VorbisWaveReader _waveStream;

    private List<List<VoiceEntryLtd>> _allVoiceListLtd = new List<List<VoiceEntryLtd>>();

    [CompilerGenerated]
    [Serializable]
    private sealed class TextLoader0
    {
      internal void LoadPkgList160()
      {
        new PkgList(Form1.D2PkgDir, Form1.PkglistFile);
      }

      internal void LoadPkgList16()
      {
        new AllPkgTexts();
      }

      public static readonly Form1.TextLoader0 Loader0 = new Form1.TextLoader0();

      public static Action Action1;

      public static Action Action2;
    }

    [CompilerGenerated]
    private sealed class SearchTermHelper
    {
      public string SearchTerm;

      public int Counter;
    }

    [CompilerGenerated]
    private sealed class VoiceEntryHelper1
    {
      public List<VoiceEntryLtd> VoiceEntry;

      public Form1.SearchTermHelper Locals;
    }

    [CompilerGenerated]
    private sealed class SearchBoxKeyDownHandler
    {
      internal void OnKeyDown()
      {
        foreach (VoiceEntryLtd voiceEntryLtd in this.Locals.VoiceEntry)
        {
          uint entryHash;
          if (!voiceEntryLtd.Narrator.ToLower().Contains(this.Locals.Locals.SearchTerm.ToLower()) && !voiceEntryLtd.Text.ToLower().Contains(this.Locals.Locals.SearchTerm.ToLower()))
          {
            entryHash = voiceEntryLtd.EntryHash;
            if (!entryHash.ToString("").Contains(this.Locals.Locals.SearchTerm.ToLower()))
            {
              continue;
            }
          }
          this.Checker = 1;
          string text = voiceEntryLtd.Narrator + ": " + voiceEntryLtd.Text;
          string str = "[";
          entryHash = voiceEntryLtd.EntryHash;
          TreeNode treeNode = new TreeNode(str + entryHash.ToString("D10") + "]  " + text)
          {
            ToolTipText = text,
            Tag = voiceEntryLtd
          };
          this.FileListTreeNodes.Add(treeNode);
          int counter = this.Locals.Locals.Counter;
          this.Locals.Locals.Counter = counter + 1;
        }
      }

      public int Checker;

      public List<TreeNode> FileListTreeNodes;

      public Form1.VoiceEntryHelper1 Locals;
    }

    [CompilerGenerated]
    private sealed class SearchTermHelper0
    {
      public string SearchTerm;

      public int Counter;
    }

    [CompilerGenerated]
    private sealed class VoiceEntryHelper0
    {
      public List<VoiceEntryLtd> VoiceEntry;

      public Form1.SearchTermHelper0 Locals;
    }

    [CompilerGenerated]
    private sealed class NarratorFilterDdClosedHelper
    {
      internal void OnDdClosed()
      {
        foreach (VoiceEntryLtd voiceEntryLtd in this.Locals.VoiceEntry)
        {
          if (voiceEntryLtd.Narrator.ToLower().Contains(this.Locals.Locals.SearchTerm.ToLower()))
          {
            this.Checker = 1;
            string text = voiceEntryLtd.Narrator + ": " + voiceEntryLtd.Text;
            string str = "[";
            uint entryHash = voiceEntryLtd.EntryHash;
            TreeNode treeNode = new TreeNode(str + entryHash.ToString("D10") + "]  " + text)
            {
              ToolTipText = text,
              Tag = voiceEntryLtd
            };
            this.FileListTreeNodes.Add(treeNode);
            int counter = this.Locals.Locals.Counter;
            this.Locals.Locals.Counter = counter + 1;
          }
        }
      }

      public int Checker;

      public List<TreeNode> FileListTreeNodes;

      public Form1.VoiceEntryHelper0 Locals;
    }

    [CompilerGenerated]
    private sealed class NarratorListHelper
    {
      public List<string> NarratorList;
    }

    [CompilerGenerated]
    private sealed class VoiceEntryHelper
    {
      public List<VoiceEntryLtd> VoiceEntry;

      public Form1.NarratorListHelper Locals;
    }

    [CompilerGenerated]
    private sealed class OpenTranscriptFileHelper
    {
      internal void openTranscriptFileb__0()
      {
        foreach (VoiceEntryLtd voiceEntryLtd in this.Locals.VoiceEntry)
        {
          string text = voiceEntryLtd.Narrator + ": " + voiceEntryLtd.Text;
          string str = "[";
          uint entryHash = voiceEntryLtd.EntryHash;
          TreeNode treeNode = new TreeNode(str + entryHash.ToString("D10") + "]  " + text)
          {
            ToolTipText = text,
            Tag = voiceEntryLtd
          };
          this.FileListTreeNodes.Add(treeNode);
          if (voiceEntryLtd.Narrator != "" && voiceEntryLtd.Narrator != " ")
          {
            this.Locals.Locals.NarratorList.Add(voiceEntryLtd.Narrator);
          }
        }
      }

      public List<TreeNode> FileListTreeNodes;

      public Form1.VoiceEntryHelper Locals;
    }

    [CompilerGenerated]
    private sealed class GenerateTranscriptDbHelper0
    {
      internal void generateTranscriptDbb__0()
      {
        IEnumerable<PkgListEntry> pkgListEntries = Form1.PkgListEntries;
        Action<PkgListEntry> body;
        if ((body = this.Action1) == null)
        {
          body = (this.Action1 = new Action<PkgListEntry>(this.generateTranscriptDbb__2));
        }
        Parallel.ForEach<PkgListEntry>(pkgListEntries, body);
      }

      internal void generateTranscriptDbb__2(PkgListEntry pkgListEntry)
      {
        PkgStream item = PkgStream.PkgStreamFromFile(string.Concat(new string[]
        {
          Form1.D2PkgDir,
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

      internal void generateTranscriptDbb__1()
      {
        IEnumerable<PkgStream> source = this.PkgStreamList;
        Action<PkgStream> body;
        if ((body = this.Action2) == null)
        {
          body = (this.Action2 = new Action<PkgStream>(this.generateTranscriptDbb__3));
        }
        Parallel.ForEach<PkgStream>(source, body);
      }

      internal void generateTranscriptDbb__3(PkgStream pkgStream)
      {
        foreach (PkgEntry pkgEntry in pkgStream.PkgEntryList)
        {
          if (pkgEntry.FileType == "Voice Lines Files")
          {
            List<VoiceEntryLtd> voiceEntriesLtd = new VoiceLines(pkgEntry.Filename).VoiceEntriesLtd;
            if (voiceEntriesLtd.Count > 0)
            {
              List<List<VoiceEntryLtd>> allVoiceListLtdTmp = this.AllVoiceListLtdTmp;
              lock (allVoiceListLtdTmp)
              {
                this.AllVoiceListLtdTmp.Add(voiceEntriesLtd);
              }
            }
          }
        }
      }

      public List<PkgStream> PkgStreamList;

      public List<List<VoiceEntryLtd>> AllVoiceListLtdTmp;

      public Action<PkgListEntry> Action1;

      public Action<PkgStream> Action2;
    }

    [CompilerGenerated]
    [Serializable]
    private sealed class LoadPkgListHelper000
    {
      internal void loadPkgListb__0_0()
      {
        new PkgList(Form1.D2PkgDir, Form1.PkglistFile);
      }

      public static readonly Form1.LoadPkgListHelper000 Inst = new Form1.LoadPkgListHelper000();

      public static Action Act;
    }

    [CompilerGenerated]
    [Serializable]
    private sealed class LoadAllTextsHelper000
    {
      internal void loadAllTextsb__0_0()
      {
        new AllPkgTexts();
      }

      public static readonly Form1.LoadAllTextsHelper000 Helpinst = new Form1.LoadAllTextsHelper000();

      public static Action Action;
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
  }
}
