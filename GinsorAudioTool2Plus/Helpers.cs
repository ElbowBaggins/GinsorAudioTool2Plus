using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GinsorAudioTool2Plus
{
  public class Helpers
  {
    public static string SelectPkgFile()
    {
      string result = null;
      string str = "*.pkg";
      string str2 = "Destiny 2 Package File";
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Filter = str2 + "|" + str
      };
      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        try
        {
          Stream stream;
          if ((stream = openFileDialog.OpenFile()) != null)
          {
            using (stream)
            {
              result = openFileDialog.FileName.Replace("\\", "/");
            }
          }
        }
        catch
        {
        }
      }
      return result;
    }

    public static string SelectD2Exe()
    {
      string result = null;
      string str = "destiny2.exe";
      string str2 = "Destiny 2 Executable";
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Filter = str2 + "|" + str
      };
      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        try
        {
          Stream stream;
          if ((stream = openFileDialog.OpenFile()) != null)
          {
            using (stream)
            {
              result = openFileDialog.FileName.Replace("\\", "/");
            }
          }
        }
        catch
        {
        }
      }
      return result;
    }

    public static void DirNotExistCreate(string folder)
    {
      try
      {
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
      }
      catch (IOException ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    public static void FileExistsDelete(string filepath)
    {
      try
      {
        if (File.Exists(filepath))
        {
          File.Delete(filepath);
        }
      }
      catch (IOException ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    public static ushort ReadUShort(Stream s)
    {
      byte[] array = new byte[2];
      s.Read(array, 0, 2);
      return BitConverter.ToUInt16(array, 0);
    }

    public static short ReadShort(Stream s)
    {
      byte[] array = new byte[2];
      s.Read(array, 0, 2);
      return BitConverter.ToInt16(array, 0);
    }

    public static uint ReadUInt(Stream s)
    {
      byte[] array = new byte[4];
      s.Read(array, 0, 4);
      return BitConverter.ToUInt32(array, 0);
    }

    public static ulong ReadULong(Stream s)
    {
      byte[] array = new byte[8];
      s.Read(array, 0, 8);
      return BitConverter.ToUInt64(array, 0);
    }

    public static float ReadFloat(Stream s)
    {
      byte[] array = new byte[4];
      s.Read(array, 0, 4);
      return BitConverter.ToSingle(array, 0);
    }

    public static double ReadDouble(Stream s)
    {
      byte[] array = new byte[8];
      s.Read(array, 0, 8);
      return BitConverter.ToDouble(array, 0);
    }

    public static byte[] ReadByteArray(Stream s, uint size)
    {
      byte[] array = new byte[size];
      int num = 0;
      while ((num += s.Read(array, num, (int)(size - (ulong)num))) < (long)((ulong)size))
      {
      }
      return array;
    }

    public static byte[] HexStringToByteArray(string hex)
    {
      Helpers.HexStringToByteArrayHelper hexStringToByteArrayHelper = new Helpers.HexStringToByteArrayHelper
      {
        Hex = hex
      };
      return Enumerable.Range(0, hexStringToByteArrayHelper.Hex.Length).Where(new Func<int, bool>(Helpers.HashStringToByteArrayHelper2.Helper2.HexStringToByteArray)).Select(new Func<int, byte>(hexStringToByteArrayHelper.HexStringToByteArray1)).ToArray<byte>();
    }

    public static string ReadNullString(Stream s)
    {
      string text = "";
      byte b;
      while ((b = (byte)s.ReadByte()) > 0 && s.Position < s.Length)
      {
        string str = text;
        char c = (char)b;
        text = str + c.ToString();
      }
      return text;
    }

    public static uint InvertUint32(uint hash)
    {
      hash = ((hash & 0xFFU) << 0x18 | (hash & 0xFF00U) << 8 | (hash & 0xFF0000U) >> 8 | (hash & 0xFF000000U) >> 0x18);
      return hash;
    }

    public static string ByteArrayAsString(byte[] data)
    {
      if (data == null)
      {
        data = new byte[0];
      }
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte value in data)
      {
        stringBuilder.Append((char)value);
      }
      return stringBuilder.ToString();
    }

    public Helpers()
    {
    }

    [CompilerGenerated]
    private sealed class HexStringToByteArrayHelper
    {
      public HexStringToByteArrayHelper()
      {
      }

      internal byte HexStringToByteArray1(int x)
      {
        return Convert.ToByte(this.Hex.Substring(x, 2), 0x10);
      }

      public string Hex;
    }

    [CompilerGenerated]
    [Serializable]
    private sealed class HashStringToByteArrayHelper2
    {
      // Note: this type is marked as 'beforefieldinit'.
      static HashStringToByteArrayHelper2()
      {
      }

      public HashStringToByteArrayHelper2()
      {
      }

      internal bool HexStringToByteArray(int x)
      {
        return x % 2 == 0;
      }

      public static readonly Helpers.HashStringToByteArrayHelper2 Helper2 = new Helpers.HashStringToByteArrayHelper2();
    }
        /// <summary>
        /// A method used to load the hash64 table from a file called "h64"
        /// </summary>
        /// <returns>A dictionary which has hash64 values and their 32 bit hashes</returns>
        public Dictionary<ulong, uint> LoadH64File()
        {
            Dictionary<ulong, uint> hash64_table = new Dictionary<UInt64, UInt32>();
            using (FileStream File = new FileStream("h64", FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader BinReader = new BinaryReader(File))
                {
                    ulong h64Val;
                    uint hVal;
                    byte[] buf = new byte[4];
                    h64Val = BinReader.ReadUInt64();
                    hVal = BinReader.ReadUInt32();
                    hash64_table[h64Val] = hVal;
                    while (BinReader.BaseStream.Position != BinReader.BaseStream.Length)
                    {
                        h64Val = BinReader.ReadUInt64();
                        hVal = BinReader.ReadUInt32();
                        hash64_table[h64Val] = hVal;
                    }
                }
            }
            return hash64_table;
        }

        /// <summary>
        /// A method used to save the hash64 table to a file called "h64"
        /// </summary>
        /// <param name="hash64_table">The dictionary of hash64s</param>
        public bool SaveH64File(Dictionary<ulong, uint> hash64_table)
        {
            using (FileStream File = new FileStream("h64", FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter BinWriter = new BinaryWriter(File))
                {
                    foreach (var element in hash64_table)
                    {
                        BinWriter.Write(element.Key);
                        BinWriter.Write(element.Value);
                    }
                }
                File.Close();
            }
            return true;
        }

        /// <summary>
        /// A method used to get the table of hash64 values
        /// </summary>
        /// <param name="packagesPath">String that points to folder of packages</param>
        /// <returns>A dictionary which has hash64 values and their 32 bit hashes</returns>
        public Dictionary<ulong, uint> GenerateH64Table(string packagesPath)
        {
            Dictionary<ulong, uint> Hash64Table = new Dictionary<ulong, uint>();

            foreach (string package in Directory.GetFiles(packagesPath))
            {
                uint hash64TableOffset;
                uint hash64TableCount;
                using (FileStream File = new FileStream(package, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader BinReader = new BinaryReader(File))
                    {
                        File.Seek(0xB8, 0);
                        hash64TableCount = BinReader.ReadUInt32();
                        if (hash64TableCount == 0)
                            continue;
                        hash64TableOffset = BinReader.ReadUInt32();
                        hash64TableOffset += 64 + 0x10;

                        for (uint i = hash64TableOffset; i < hash64TableOffset + hash64TableCount * 0x10; i += 0x10)
                        {
                            UInt64 h64Val;
                            File.Seek(i, 0);
                            h64Val = BinReader.ReadUInt64();
                            UInt32 hVal;
                            hVal = BinReader.ReadUInt32();
                            Hash64Table[h64Val] = hVal;
                        }
                    }
                    File.Close();
                }
            }
            return Hash64Table;
        }
        /// <summary>
        /// A method used to get the hash64 table from either the "h64" file, or the packages if it doesnt exist.
        /// </summary>
        /// <returns>A dictionary which has hash64 values and their 32 bit hashes</returns>
        public Dictionary<ulong, uint> GetH64Table(string pkgsPath)
        {
            Dictionary<ulong, uint> hash64_table;
            if (File.Exists("h64"))
            {
                hash64_table = LoadH64File();
                if (hash64_table.Count < 10000)
                {
                    hash64_table = GenerateH64Table(pkgsPath);
                    SaveH64File(hash64_table);
                }
            }
            else
            {
                hash64_table = GenerateH64Table(pkgsPath);
                SaveH64File(hash64_table);
            }
            return hash64_table;
        }
  }
}
