// ProcessorDirect.h

#pragma once
#include <intrin.h>
using namespace System;

namespace ProcessorDirect {

#pragma managed(push, off)
	static unsigned int dopopcnt(unsigned int i)
	{
		return __popcnt(i);
	}
#pragma managed(pop)

	public ref class ProcessorFuncs
	{
	public: 
		static unsigned int BitCount(unsigned int i)
		{
			return dopopcnt(i);
		}
	};
}
