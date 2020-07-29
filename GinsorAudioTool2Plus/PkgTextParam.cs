using System.Collections.Generic;

namespace GinsorAudioTool2Plus
{
  public struct D2TextParam
  {
    public uint EngFileHash;

    public uint GerFileHash;

    public uint EspFileHash;

    public uint NumOfstringHashes;

    public Dictionary<uint, uint> StringHashList;
  }
}
