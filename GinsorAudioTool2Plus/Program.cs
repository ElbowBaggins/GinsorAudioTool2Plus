using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GinsorAudioTool2Plus
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
  }
}
