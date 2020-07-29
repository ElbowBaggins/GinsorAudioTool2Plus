#include "pch.h"

#include <iostream>
#include <sstream>
#include "ww2ogg.h"
#include "wwriff.h"

using Runtime::InteropServices::Marshal;

const int EIGHT_MB = 8388608;



IO::MemoryStream^ ww2ogg::Ww2Ogg::ww2ogg(IO::MemoryStream^ wem, IO::MemoryStream^ codeBook) {

  string name = "ww2ogg";
  string codeBookName = "Destiny 2";

  std::stringstream wemStream(std::stringstream::in|std::stringstream::out|std::stringstream::binary);
  cli::array<unsigned char>^ managedArray = wem->ToArray();
  char* copy = new char[managedArray->Length];
  Marshal::Copy(managedArray, 0, IntPtr(copy), managedArray->Length);
  wemStream.write(copy, managedArray->Length);

  std::stringstream codebookStream(std::stringstream::in|std::stringstream::out|std::stringstream::binary);
  cli::array<unsigned char>^ codebookArray = codeBook->ToArray();
  char* codebookCopy = new char[codebookArray->Length];
  Marshal::Copy(codebookArray, 0, IntPtr(codebookCopy), codebookArray->Length);
  codebookStream.write(codebookCopy, codebookArray->Length);

  auto converter = new Wwise_RIFF_Vorbis(
      name,
      wemStream,
      codeBookName,
      codebookStream,
      false,
      false,
      ForcePacketFormat::kNoForcePacketFormat
  );
  std::stringstream outStream(std::stringstream::in|std::stringstream::out|std::stringstream::binary);
  converter->generate_ogg(outStream);
  outStream.seekg(0, ios_base::end);
  auto length = outStream.tellg();
  outStream.seekg(0);
  char* unmanagedResult = new char[length];
  outStream.read(unmanagedResult, length);
  const auto resultArray = gcnew cli::array<unsigned char>(length);
  Marshal::Copy(IntPtr(unmanagedResult), resultArray, 0, length);
  const auto result = gcnew IO::MemoryStream(resultArray);
  return result;
}

