using System.Collections.Generic;

namespace GinsorAudioTool2Plus
{
  public class FileClassification
  {
    public static PkgFileType SetD2Filetype(PkgEntry pkgEntry)
    {
      PkgFileType result = PkgFileType.None;
      if (pkgEntry.FileType == FileClassification.ImageType)
      {
        result = PkgFileType.Image;
      }
      else if (pkgEntry.FileType == FileClassification.TextType)
      {
        result = PkgFileType.Text;
      }
      else if (pkgEntry.FileType == FileClassification.AudioType)
      {
        result = PkgFileType.Audio;
      }
      else if (pkgEntry.FileType == FileClassification.ModelType)
      {
        result = PkgFileType.Model;
      }
      return result;
    }

    public static string RecImageTypeName()
    {
      return FileClassification.ImageType;
    }

    public static string FileClassify(uint entryA, uint typeA)
    {
      uint num = 0U;
      if (FileClassification.FileTypeClassifier.ContainsKey(entryA))
      {
        num = FileClassification.FileTypeClassifier[entryA];
      }
      else if (FileClassification.FileTypeClassifier.ContainsKey(typeA))
      {
        num = FileClassification.FileTypeClassifier[typeA];
      }
      if (num == 0U && typeA >= 0x4000U && typeA <= 0x40FFU)
      {
        num = 0x11U;
      }
      if (num == 0U && typeA >= 0x25000U && typeA <= 0x250FFU)
      {
        num = 0x10U;
      }
      return FileClassification.FileTypes[num];
    }

    public FileClassification()
    {
    }

    // Note: this type is marked as 'beforefieldinit'.
    static FileClassification()
    {
    }

    private static readonly string ImageType = "Image Ref";

    private static readonly string TextType = "Text Ref";

    private static readonly string AudioType = "Audio Ref";

    private static readonly string ModelType = "Model Mega";

    public static Dictionary<uint, string> FileTypes = new Dictionary<uint, string>
    {
      {
        0U,
        "Unclassified"
      },
      {
        0x10U,
        "Image"
      },
      {
        0x11U,
        FileClassification.ImageType
      },
      {
        0x12U,
        "Image Combiner"
      },
      {
        0x13U,
        "Image Combiner Ref"
      },
      {
        0x20U,
        "Audio"
      },
      {
        0x21U,
        FileClassification.AudioType
      },
      {
        0x22U,
        "Audio BKHD"
      },
      {
        0x30U,
        "Text"
      },
      {
        0x31U,
        FileClassification.TextType
      },
      {
        0x40U,
        FileClassification.ModelType
      },
      {
        0x41U,
        "Model Faces"
      },
      {
        0x42U,
        "Model Faces Ref"
      },
      {
        0x43U,
        "Model Vertics"
      },
      {
        0x44U,
        "Model Vertics Ref"
      },
      {
        0x49U,
        "Model Misc"
      },
      {
        0x4AU,
        "Model Misc Ref"
      },
      {
        0x50U,
        "DXBC Shader"
      },
      {
        0x51U,
        "DXBC Shader Ref"
      },
      {
        0x52U,
        "DXBC Shader Chunk"
      },
      {
        0x53U,
        "DXBC Shader Chunk Ref"
      },
      {
        0x60U,
        "Havok A"
      },
      {
        0x61U,
        "Havok A Ref"
      },
      {
        0x62U,
        "Havok B"
      },
      {
        0x70U,
        "Filename Files"
      },
      {
        0x71U,
        "Filename Files Ref"
      },
      {
        0x80U,
        "Map (Test)"
      },
      {
        0x90U,
        "Voice Lines Files"
      },
      {
        0xF000U,
        "Unknown"
      }
    };

    private static readonly Dictionary<uint, uint> FileTypeClassifier = new Dictionary<uint, uint>
    {
      {
        0x6607DU,
        0x10U
      },
      {
        0x6507DU,
        0x10U
      },
      {
        0x66099U,
        0x10U
      },
      {
        0x6604EU,
        0x10U
      },
      {
        0x66059U,
        0x10U
      },
      {
        0x80804A69U,
        0x12U
      },
      {
        0x80804A53U,
        0x13U
      },
      {
        0x1359CU,
        0x20U
      },
      {
        0x355CU,
        0x22U
      },
      {
        0x80809A8AU,
        0x30U
      },
      {
        0x80809A88U,
        0x31U
      },
      {
        0x808073A5U,
        0x40U
      },
      {
        0x25199U,
        0x41U
      },
      {
        0x2518AU,
        0x41U
      },
      {
        0x25195U,
        0x41U
      },
      {
        0x4199U,
        0x42U
      },
      {
        0x418AU,
        0x42U
      },
      {
        0x4195U,
        0x42U
      },
      {
        0x25119U,
        0x43U
      },
      {
        0x25114U,
        0x43U
      },
      {
        0x2510AU,
        0x43U
      },
      {
        0x25115U,
        0x43U
      },
      {
        0x4119U,
        0x44U
      },
      {
        0x4114U,
        0x44U
      },
      {
        0x410AU,
        0x44U
      },
      {
        0x4115U,
        0x44U
      },
      {
        0x251CCU,
        0x49U
      },
      {
        0x251D5U,
        0x49U
      },
      {
        0x25194U,
        0x49U
      },
      {
        0x251DAU,
        0x49U
      },
      {
        0x251CAU,
        0x49U
      },
      {
        0x41CCU,
        0x4AU
      },
      {
        0x41D5U,
        0x4AU
      },
      {
        0x4194U,
        0x4AU
      },
      {
        0x41DAU,
        0x4AU
      },
      {
        0x41CAU,
        0x4AU
      },
      {
        0x25253U,
        0x50U
      },
      {
        0x25213U,
        0x50U
      },
      {
        0x2520CU,
        0x50U
      },
      {
        0x2520FU,
        0x50U
      },
      {
        0x2524CU,
        0x50U
      },
      {
        0x2520AU,
        0x50U
      },
      {
        0x2521AU,
        0x50U
      },
      {
        0x2520BU,
        0x50U
      },
      {
        0x2525AU,
        0x50U
      },
      {
        0x2522CU,
        0x50U
      },
      {
        0x2529AU,
        0x50U
      },
      {
        0x25214U,
        0x50U
      },
      {
        0x25215U,
        0x50U
      },
      {
        0x2520DU,
        0x50U
      },
      {
        0x2524AU,
        0x50U
      },
      {
        0x25255U,
        0x50U
      },
      {
        0x4253U,
        0x51U
      },
      {
        0x4213U,
        0x51U
      },
      {
        0x420CU,
        0x51U
      },
      {
        0x420FU,
        0x51U
      },
      {
        0x424CU,
        0x51U
      },
      {
        0x420AU,
        0x51U
      },
      {
        0x421AU,
        0x51U
      },
      {
        0x420BU,
        0x51U
      },
      {
        0x425AU,
        0x51U
      },
      {
        0x422CU,
        0x51U
      },
      {
        0x429AU,
        0x51U
      },
      {
        0x4214U,
        0x51U
      },
      {
        0x4215U,
        0x51U
      },
      {
        0x420DU,
        0x51U
      },
      {
        0x424AU,
        0x51U
      },
      {
        0x4255U,
        0x51U
      },
      {
        0x2539AU,
        0x52U
      },
      {
        0x439AU,
        0x53U
      },
      {
        0x35D4U,
        0x60U
      },
      {
        0x8080727AU,
        0x61U
      },
      {
        0x35D9U,
        0x62U
      },
      {
        0x8080902DU,
        0x70U
      },
      {
        0x8080816CU,
        0x70U
      },
      {
        0x8080941EU,
        0x70U
      },
      {
        0x8080744AU,
        0x71U
      },
      {
        0x3012U,
        0x80U
      },
      {
        0x80808D54U,
        0x90U
      },
      {
        0x808071E8U,
        0xF000U
      },
      {
        0x80809C36U,
        0xF000U
      },
      {
        0x80809B14U,
        0xF000U
      },
      {
        0x80809468U,
        0xF000U
      },
      {
        0x808071D9U,
        0xF000U
      },
      {
        0x80809BBBU,
        0xF000U
      },
      {
        0x80806E28U,
        0xF000U
      },
      {
        0x80807140U,
        0xF000U
      },
      {
        0x80806F68U,
        0xF000U
      },
      {
        0x8080707EU,
        0xF000U
      },
      {
        0x80806E2CU,
        0xF000U
      },
      {
        0x808071DCU,
        0xF000U
      },
      {
        0x80809BB6U,
        0xF000U
      },
      {
        0x80808162U,
        0xF000U
      },
      {
        0x80809C0FU,
        0xF000U
      },
      {
        0x80809C54U,
        0xF000U
      },
      {
        0x8080462FU,
        0xF000U
      }
    };
  }
}
