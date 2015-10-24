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
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using CenterCLR.ExaSerializers.Generators;
#if NET35
using System.Security.Permissions;
#endif

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Writers
{
	internal abstract partial class InternalWriterBase
	{
		#region Fields
		private GCHandle handle_;
		private readonly IntPtr basePointer_;
		#endregion

		#region Constructors
#if NET35
		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
#else
		[SecurityCritical]
#endif
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		protected InternalWriterBase(Encoding encoding, int bufferSize)
		{
			encoding_ = encoding;
			isSingleByte_ = encoding.GetMaxByteCount(1) == 1;
			buffer_ = new byte[bufferSize];
			handle_ = GCHandle.Alloc(buffer_, GCHandleType.Pinned);
			basePointer_ = handle_.AddrOfPinnedObject();
		}
		#endregion

		#region Finalizer
		~InternalWriterBase()
		{
			this.Dispose();
		}
		#endregion

		#region Dispose
#if NET35
		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
#else
		[SecurityCritical]
#endif
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Dispose()
		{
			if (handle_.IsAllocated == true)
			{
				handle_.Free();
			}
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region InternalWriteBytes
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private unsafe void InternalWriteBytes(byte[] values)
		{
			Debug.Assert(values != null);

			var rawLength = values.Length;
			if (rawLength == 0)
			{
				return;
			}

			this.TryFlush(rawLength);

			fixed (byte* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits(pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region WriteByte
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteByte(byte value)
#else
		public unsafe void WriteByte(byte value)
#endif
		{
			this.TryFlush(sizeof(byte));
			byte* p = (byte*)basePointer_;
			p[nextPosition_++] = value;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteInt16
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteInt16(short value)
#else
		public unsafe void WriteInt16(short value)
#endif
		{
			this.TryFlush(sizeof(short));
			byte* p = ((byte*)basePointer_) + nextPosition_;
			if ((((byte)p) & 1) == 0)
			{
				*((short*)p) = value;
			}
			else
			{
				*(p++) = (byte)value;
				*(p) = (byte)(value >> 8);
			}
			nextPosition_ += sizeof(short);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteInt32
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteInt32(int value)
#else
		public unsafe void WriteInt32(int value)
#endif
		{
			this.TryFlush(sizeof(int));
			byte* p = ((byte*)basePointer_) + nextPosition_;
			if ((((byte)p) & 3) == 0)
			{
				*((int*)p) = value;
			}
			else
			{
				*(p++) = (byte)value;
				*(p++) = (byte)(value >> 8);
				*(p++) = (byte)(value >> 16);
				*(p) = (byte)(value >> 24);
			}
			nextPosition_ += sizeof(int);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteInt64
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteInt64(long value)
#else
		public unsafe void WriteInt64(long value)
#endif
		{
			this.TryFlush(sizeof(long));
			byte* p = ((byte*)basePointer_) + nextPosition_;
			if ((((byte)p) & 7) == 0)
			{
				*((long*)p) = value;
			}
			else
			{
				*(p++) = (byte)value;
				*(p++) = (byte)(value >> 8);
				*(p++) = (byte)(value >> 16);
				*(p++) = (byte)(value >> 24);
				*(p++) = (byte)(value >> 32);
				*(p++) = (byte)(value >> 40);
				*(p++) = (byte)(value >> 48);
				*(p) = (byte)(value >> 56);
			}
			nextPosition_ += sizeof(long);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteSingle
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteSingle(float value)
#else
		public unsafe void WriteSingle(float value)
#endif
		{
			this.TryFlush(sizeof(float));
			byte* p = ((byte*)basePointer_) + nextPosition_;
			if ((((byte)p) & 3) == 0)
			{
				*((float*)p) = value;
			}
			else
			{
				var v = *(int*)&value;
				*(p++) = (byte)v;
				*(p++) = (byte)(v >> 8);
				*(p++) = (byte)(v >> 16);
				*(p) = (byte)(v >> 24);
			}
			nextPosition_ += sizeof(float);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteDouble
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteDouble(double value)
#else
		public unsafe void WriteDouble(double value)
#endif
		{
			byte* p = ((byte*)basePointer_) + nextPosition_;
			if ((((byte)p) & 7) == 0)
			{
				*((double*)p) = value;
			}
			else
			{
				var v = *(long*)&value;
				*(p++) = (byte)v;
				*(p++) = (byte)(v >> 8);
				*(p++) = (byte)(v >> 16);
				*(p++) = (byte)(v >> 24);
				*(p++) = (byte)(v >> 32);
				*(p++) = (byte)(v >> 40);
				*(p++) = (byte)(v >> 48);
				*(p) = (byte)(v >> 56);
			}
			nextPosition_ += sizeof(double);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region WriteSByteArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteSByteArray(sbyte[] values)
#else
		public unsafe void WriteSByteArray(sbyte[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length;
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (sbyte* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteInt16Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteInt16Array(short[] values)
#else
		public unsafe void WriteInt16Array(short[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(short);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (short* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteUInt16Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteUInt16Array(ushort[] values)
#else
		public unsafe void WriteUInt16Array(ushort[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(short);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (ushort* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteInt32Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteInt32Array(int[] values)
#else
		public unsafe void WriteInt32Array(int[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(int);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (int* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteUInt32Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteUInt32Array(uint[] values)
#else
		public unsafe void WriteUInt32Array(uint[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(uint);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (uint* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteInt64Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteInt64Array(long[] values)
#else
		public unsafe void WriteInt64Array(long[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(long);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (long* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteUInt64Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteUInt64Array(ulong[] values)
#else
		public unsafe void WriteUInt64Array(ulong[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(ulong);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (ulong* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteSingleArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteSingleArray(float[] values)
#else
		public unsafe void WriteSingleArray(float[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(float);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (float* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteDoubleArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteDoubleArray(double[] values)
#else
		public unsafe void WriteDoubleArray(double[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length * sizeof(double);
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			fixed (double* pValues = &values[0])
			{
				UnsafeUtilities.UnsafeCopyBits((byte*)pValues, ((byte*)basePointer_) + nextPosition_, rawLength);
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteBooleanArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public unsafe bool WriteBooleanArray(bool[] values)
#else
		public unsafe void WriteBooleanArray(bool[] values)
#endif
		{
			if (values == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteInt32(-1);
#else
				this.WriteInt32(-1);
				return;
#endif
			}

			this.WriteInt32(values.Length);

			var rawLength = values.Length;
			if (rawLength == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(rawLength);

			byte* pd = ((byte*)basePointer_) + nextPosition_;

			fixed (bool* pv = &values[0])
			{
				bool* ps = pv;
				for (var index = 0; index < rawLength; index++)
				{
					*(pd++) = *(ps++) ? (byte)1 : (byte)0;
				}
			}

			nextPosition_ += rawLength;

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion
	}
}
#endif
