/////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.ExaSerializers - A lightning fast & lightweight simple binary serializer.
// Copyright (c) 2015 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////

#if USE_UNSAFE
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CenterCLR.ExaSerializers.Generators
{
	internal static class UnsafeUtilities
	{
		#region UnsafeCopyBits
#if NETFX_CORE
		[MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)16)]
#elif USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.ForwardRef)]
#else
		[MethodImpl(MethodImplOptions.ForwardRef)]
#endif
		public static extern unsafe void UnsafeCopyBits(byte* pFrom, byte* pTo, int length);
		#endregion
	}
}
#endif
