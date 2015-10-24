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

//#ifdef USE_UNSAFE
.class private auto ansi abstract sealed beforefieldinit CenterCLR.ExaSerializers.Generators.UnsafeUtilities
	extends [mscorlib]System.Object
{
	.method public hidebysig static 
		void UnsafeCopyBits (
			uint8* pFrom,
			uint8* pTo,
			int32 length
		) cil managed
	{
		// Method begins at RVA 0x4e34
		// Code size 135 (0x87)
		.maxstack 4
		.locals init (
			[0] bool fromBound,
			[1] bool toBound,
			[2] uint64* ps,
			[3] uint64* pd,
			[4] int32 remains,
			[5] uint8* psl,
			[6] uint8* pdl,
			[7] uint8* ps,
			[8] uint8* pd,
			[9] int32 remains
		)

		IL_0000: ldarg.0
		IL_0001: conv.u1
		IL_0002: ldc.i4.7
		IL_0003: and
		IL_0004: ldc.i4.0
		IL_0005: ceq
		IL_0007: stloc.0
		IL_0008: ldarg.1
		IL_0009: conv.u1
		IL_000a: ldc.i4.7
		IL_000b: and
		IL_000c: ldc.i4.0
		IL_000d: ceq
		IL_000f: stloc.1
		IL_0010: ldloc.0
		IL_0011: brfalse.s IL_005e

		IL_0013: ldloc.1
		IL_0014: brfalse.s IL_005e

		IL_0016: ldarg.0
		IL_0017: stloc.2
		IL_0018: ldarg.1
		IL_0019: stloc.3
		IL_001a: ldarg.2
		IL_001b: stloc.s remains
		IL_001d: br.s IL_0033
		// loop start (head: IL_0033)
			IL_001f: ldloc.3
			IL_0020: dup
			IL_0021: ldc.i4.8
			IL_0022: conv.i
			IL_0023: add
			IL_0024: stloc.3
			IL_0025: ldloc.2
			IL_0026: dup
			IL_0027: ldc.i4.8
			IL_0028: conv.i
			IL_0029: add
			IL_002a: stloc.2
			IL_002b: ldind.i8
			IL_002c: stind.i8
			IL_002d: ldloc.s remains
			IL_002f: ldc.i4.8
			IL_0030: sub
			IL_0031: stloc.s remains

			IL_0033: ldloc.s remains
			IL_0035: ldc.i4.8
			IL_0036: bge.s IL_001f
		// end loop

		IL_0038: ldloc.2
		IL_0039: stloc.s psl
		IL_003b: ldloc.3
		IL_003c: stloc.s pdl
		IL_003e: br.s IL_0058
		// loop start (head: IL_0058)
			IL_0040: ldloc.s pdl
			IL_0042: dup
			IL_0043: ldc.i4.1
			IL_0044: conv.i
			IL_0045: add
			IL_0046: stloc.s pdl
			IL_0048: ldloc.s psl
			IL_004a: dup
			IL_004b: ldc.i4.1
			IL_004c: conv.i
			IL_004d: add
			IL_004e: stloc.s psl
			IL_0050: ldind.u1
			IL_0051: stind.i1
			IL_0052: ldloc.s remains
			IL_0054: ldc.i4.1
			IL_0055: sub
			IL_0056: stloc.s remains

			IL_0058: ldloc.s remains
			IL_005a: ldc.i4.1
			IL_005b: bge.s IL_0040
		// end loop

		IL_005d: ret

		IL_005e: ldarg.0
		IL_005f: stloc.s ps
		IL_0061: ldarg.1
		IL_0062: stloc.s pd
		IL_0064: ldarg.2
		IL_0065: stloc.s remains
		IL_0067: br.s IL_0081
		// loop start (head: IL_0081)
			IL_0069: ldloc.s pd
			IL_006b: dup
			IL_006c: ldc.i4.1
			IL_006d: conv.i
			IL_006e: add
			IL_006f: stloc.s pd
			IL_0071: ldloc.s ps
			IL_0073: dup
			IL_0074: ldc.i4.1
			IL_0075: conv.i
			IL_0076: add
			IL_0077: stloc.s ps
			IL_0079: ldind.u1
			IL_007a: stind.i1
			IL_007b: ldloc.s remains
			IL_007d: ldc.i4.1
			IL_007e: sub
			IL_007f: stloc.s remains

			IL_0081: ldloc.s remains
			IL_0083: ldc.i4.1
			IL_0084: bge.s IL_0069
		// end loop

		IL_0086: ret
	} // end of method ReflectionUtilities::UnsafeCopyBits
}
//#endif
