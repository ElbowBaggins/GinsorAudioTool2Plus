#pragma once

using namespace System;

namespace WWise {
	public ref class Converter sealed
    {
	  public:
      static IO::MemoryStream^ ToOgg(IO::MemoryStream^ wem, IO::MemoryStream^ codeBook);
	};
}
