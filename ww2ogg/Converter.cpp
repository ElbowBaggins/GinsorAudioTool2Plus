#include "Converter.h"
#include "wwriff.h"

#include <sstream>

using Runtime::InteropServices::Marshal;

const int EIGHT_MB = 8388608;

IO::MemoryStream^ WWise::Converter::ToOgg(IO::MemoryStream^ wem, IO::MemoryStream^ codeBook) {

  string name = "ww2ogg";
  string codeBookName = "Destiny 2";

  std::stringstream wemStream(std::stringstream::in|std::stringstream::out|std::stringstream::binary);
  auto managedArray = wem->ToArray();
  auto copy = new char[managedArray->Length]{ '\0' };
  Marshal::Copy(managedArray, 0, IntPtr(copy), managedArray->Length);
  wemStream.write(copy, managedArray->Length);

  std::stringstream codebookStream(std::stringstream::in|std::stringstream::out|std::stringstream::binary);
  auto codebookArray = codeBook->ToArray();
  auto codebookCopy = new char[codebookArray->Length]{ '\0' };
  Marshal::Copy(codebookArray, 0, IntPtr(codebookCopy), codebookArray->Length);
  codebookStream.write(codebookCopy, codebookArray->Length);

  auto converter = new OggReStreamer(
      name,
      wemStream,
      codeBookName,
      codebookStream,
      false,
      false,
      NoForcePacketFormat
  );
  std::stringstream outStream(std::stringstream::in|std::stringstream::out|std::stringstream::binary);
  converter->GenerateOgg(outStream);
  outStream.seekg(0, ios_base::end);
  auto length = outStream.tellg();
  outStream.seekg(0);
  auto unmanagedResult = new char[length]{ '\0' };
  outStream.read(unmanagedResult, length);
  auto iLength = static_cast<int>(length);
  const auto resultArray = gcnew cli::array<unsigned char>(iLength);
  Marshal::Copy(IntPtr(unmanagedResult), resultArray, 0, iLength);
  const auto result = gcnew IO::MemoryStream(resultArray);
  delete[] copy;
  delete[] codebookCopy;
  delete[] unmanagedResult;
  return result;
}

