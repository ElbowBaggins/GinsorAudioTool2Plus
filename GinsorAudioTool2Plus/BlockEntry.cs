namespace GinsorAudioTool2Plus
{
  public struct BlockEntry
  {
    public uint BlockNumber;

    public uint Offset;

    public uint Size;

    public ushort PatchId;

    public ushort Flag;

    public bool Encrypted;

    public bool Compressed;

    public bool AltKey;

    public byte[] Md5Hash;

    public byte[] GcmTag;
  }
}
