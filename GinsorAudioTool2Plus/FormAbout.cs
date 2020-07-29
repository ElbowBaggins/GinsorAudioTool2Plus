using System.ComponentModel;
using System.Windows.Forms;

namespace GinsorAudioTool2Plus
{
  public partial class FormAbout : Form
  {
    public FormAbout()
    {
      this.InitializeComponent();
    }

    public FormAbout(IContainer components) {
      this.components = components;
      this.InitializeComponent();
    }
  }
}
