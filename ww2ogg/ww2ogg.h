#pragma once
#include "wwriff.h"
#include "Bit_stream.h"

using namespace System;

namespace ww2ogg {
	public ref class Ww2Ogg
	{
	  public:
      static IO::MemoryStream^ ww2ogg(IO::MemoryStream^ wem, IO::MemoryStream^ codeBook);
	};
}
