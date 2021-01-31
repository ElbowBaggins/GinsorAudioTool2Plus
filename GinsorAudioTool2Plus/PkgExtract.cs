
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GinsorAudioTool2Plus
{
    internal class PkgExtract
    {
        public PkgExtract(PkgStream pkgStreamInput)
        {
            this._pkgStream = pkgStreamInput;
        }

        public PkgExtract()
        {
        }

        public byte[] ToBuffer(uint fnumber)
        {
            PkgEntry pkgEntry = default(PkgEntry);
            pkgEntry = this.GetPkgEntryInfo(fnumber);
            byte[] array = new byte[pkgEntry.BlockCount * 0x40000U];
            using (FileStream fileStream = File.OpenRead(Form1.RecD2PkgDir() + pkgEntry.StartBlockPkg))
            {
                for (uint num = pkgEntry.StartBlock; num < pkgEntry.StartBlock + pkgEntry.BlockCount; num += 1U)
                {
                    BlockEntry blockEntry = this._pkgStream.BlockEntryList[(int)num];
                    byte[] array2 = new byte[blockEntry.Size];
                    fileStream.Seek((long)((ulong)blockEntry.Offset), SeekOrigin.Begin);
                    fileStream.Read(array2, 0, (int)blockEntry.Size);
                    if (blockEntry.Encrypted)
                    {
                        if (!blockEntry.AltKey)
                        {
                            array2 = this.DecryptBuffer(array2, this._pkgStream.Nonce, blockEntry.GcmTag, 1);
                        }
                        else
                        {
                            array2 = this.DecryptBuffer(array2, this._pkgStream.Nonce, blockEntry.GcmTag, 2);
                        }
                    }
                    uint num2 = blockEntry.Size;
                    if (blockEntry.Compressed)
                    {
                        array2 = this.DecompressBuffer(array2);
                        num2 = 0x40000U;
                    }
                    Array.Copy(array2, 0L, array, (long)((ulong)((num - pkgEntry.StartBlock) * 0x40000U)), (long)((ulong)num2));
                }
                fileStream.Close();
            }
            byte[] array3 = new byte[pkgEntry.EntrySize];
            Array.Copy(array, (long)((ulong)pkgEntry.InBlockOffset), array3, 0L, (long)((ulong)pkgEntry.EntrySize));
            return array3;
        }

        public void Extract(uint fnumber, string outputDir)
        {
            byte[] bytes = this.ToBuffer(fnumber);
            var pkgEntry = this.GetPkgEntryInfo(fnumber);
            string str = string.Concat(new string[]
            {
        this._pkgStream.Header.PackageId.ToString("X4"),
        "_",
        fnumber.ToString("X4"),
        "_",
        this._pkgStream.Header.LangId.ToString("X2"),
        "_",
        pkgEntry.EntryA.ToString("X4"),
        "_",
        pkgEntry.TypeA.ToString("X4"),
        ".bin"
            });
            string text = outputDir + str;
            Helpers.FileExistsDelete(text);
            File.WriteAllBytes(text, bytes);
        }

        public void ExtractAll(string outputDir)
        {
            PkgExtract.ExtractAllHelper extractAllHelper = new PkgExtract.ExtractAllHelper
            {
                Pkgex = this,
                OutputDir = outputDir
            };
            foreach (var pkgEntry in this._pkgStream.PkgEntryList)
            {
                extractAllHelper.ExtractAll0(pkgEntry);
            }
        }

        public void ExtractAllLimited(string outputDir)
        {
            PkgExtract.ExtractAllLimitedHelper extractAllLimitedHelper = new PkgExtract.ExtractAllLimitedHelper
            {
                PkgEx = this,
                OutputDir = outputDir
            };
            Parallel.ForEach<PkgEntry>(this._pkgStream.PkgEntryList, new Action<PkgEntry>(extractAllLimitedHelper.ExtractAllLimited0));
        }

        public byte[] ToBuffer(PkgFile pkgFile)
        {
            PkgEntry pkgEntry = pkgFile.PkgEntry;
            byte[] array = new byte[pkgEntry.BlockCount * 0x40000U];
            using (FileStream fileStream = File.OpenRead(Form1.RecD2PkgDir() + pkgEntry.StartBlockPkg))
            {
                foreach (BlockEntry blockEntry in pkgFile.BlockEntries)
                {
                    byte[] array2 = new byte[blockEntry.Size];
                    fileStream.Seek((long)((ulong)blockEntry.Offset), SeekOrigin.Begin);
                    fileStream.Read(array2, 0, (int)blockEntry.Size);
                    if (blockEntry.Encrypted)
                    {
                        if (!blockEntry.AltKey)
                        {
                            array2 = this.DecryptBuffer(array2, pkgFile.Nonce, blockEntry.GcmTag, 1);
                        }
                        else
                        {
                            array2 = this.DecryptBuffer(array2, pkgFile.Nonce, blockEntry.GcmTag, 2);
                        }
                    }
                    uint num = blockEntry.Size;
                    if (blockEntry.Compressed)
                    {
                        array2 = this.DecompressBuffer(array2);
                        num = 0x40000U;
                    }
                    Array.Copy(array2, 0L, array, (long)((ulong)((blockEntry.BlockNumber - pkgEntry.StartBlock) * 0x40000U)), (long)((ulong)num));
                }
                fileStream.Close();
            }
            byte[] array3 = new byte[pkgEntry.EntrySize];
            Array.Copy(array, (long)((ulong)pkgEntry.InBlockOffset), array3, 0L, (long)((ulong)pkgEntry.EntrySize));
            return array3;
        }

        public void Extract(PkgFile pkgFile, string outputDir)
        {
            byte[] bytes = this.ToBuffer(pkgFile);
            string filename = pkgFile.Filename;
            string text = outputDir + filename;
            Helpers.FileExistsDelete(text);
            File.WriteAllBytes(text, bytes);
        }

        private PkgEntry GetPkgEntryInfo(uint fnumber)
        {
            return this._pkgStream.PkgEntryList[(int)fnumber];
        }

        private byte[] DecryptBuffer(byte[] encryptedInput, byte[] nonce, byte[] gcmtag, int keyinfo)
        {
            byte[] result = new byte[encryptedInput.Length];

            using (var authenticatedAesCng = new AesGcm(keyinfo == 1 ? _key : _altKey)) {
              authenticatedAesCng.Decrypt(nonce, encryptedInput, gcmtag, result);
            }
            return result;
        }

        private byte[] DecompressBuffer(byte[] inputBuffer)
        {
            int bufferSize = inputBuffer.Count<byte>();
            int num = 0x40000;
            byte[] array = new byte[num];
            OodleDllWrapper.OodleLZ_Decompress(inputBuffer, bufferSize, array, num, 0U, 0U, 0UL, 0U, 0U, 0U, 0U, 0U, 0U, 3U);
            return array;
        }

       private readonly byte[] _key = new byte[]
       {
      0xD6,
      0x2A,
      0xB2,
      0xC1,
      0xC,
      0xC0,
      0x1B,
      0xC5,
      0x35,
      0xDB,
      0x7B,
      0x86,
      0x55,
      0xC7,
      0xDC,
      0x3B
       };

        private readonly byte[] _altKey = new byte[]
        {
        0x3A,
        0x4A,
        0x5D,
        0x36,
        0x73,
        0xA6,
        0x60,
        0x58,
        0x7E,
        0x63,
        0xE6,
        0x76,
        0xE4,
        0x08,
        0x92,
        0xB5
        };

        private readonly PkgStream _pkgStream;

        [CompilerGenerated]
        private sealed class ExtractAllHelper
        {
            public ExtractAllHelper()
            {
            }

            internal void ExtractAll0(PkgEntry pkgListEntry)
            {
                this.Pkgex.Extract(pkgListEntry.EntryNumber, this.OutputDir);
            }

            public PkgExtract Pkgex;

            public string OutputDir;
        }

        [CompilerGenerated]
        private sealed class ExtractAllLimitedHelper
        {
            public ExtractAllLimitedHelper()
            {
            }

            internal void ExtractAllLimited0(PkgEntry pkgListEntry)
            {
                if (pkgListEntry.FileType != "Image" && pkgListEntry.FileType != "Audio" && pkgListEntry.FileType != "Text")
                {

                    this.PkgEx.Extract(pkgListEntry.EntryNumber, this.OutputDir);

                }
            }

            public PkgExtract PkgEx;

            public string OutputDir;
        }
    }
}
