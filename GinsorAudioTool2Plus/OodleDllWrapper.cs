using System.Runtime.InteropServices;

namespace GinsorAudioTool2Plus
{
  internal class OodleDllWrapper
  {
    [DllImport("oo2core_3_win64.dll")]
    public static extern int OodleLZ_Decompress(byte[] buffer, int bufferSize, byte[] outputBuffer, int outputBufferSize, uint a, uint b, ulong c, uint d, uint e, uint f, uint g, uint h, uint i, uint threadModule);

    public OodleDllWrapper()
    {
    }
  }
}
