using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

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
  }
}
