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

#if NET35
using System.Security.Permissions;
#endif

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

using CenterCLR.ExaSerializers.Generators;

namespace CenterCLR.ExaSerializers.Readers
{
	internal abstract partial class InternalReaderBase
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
		protected InternalReaderBase(Encoding encoding, int bufferSize)
		{
			encoding_ = encoding;
			isSingleByte_ = encoding_.GetMaxByteCount(1) == 1;
			buffer_ = new byte[bufferSize];
			handle_ = GCHandle.Alloc(buffer_, GCHandleType.Pinned);
			basePointer_ = handle_.AddrOfPinnedObject();
		}
		#endregion

		#region Finalizer
		~InternalReaderBase()
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

		#region ReadByte
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe byte ReadByte()
		{
			this.TryFetch(sizeof(byte));
			return *(((byte*)basePointer_) + nextPosition_++);
		}
		#endregion

		#region ReadInt16
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe short ReadInt16()
		{
			this.TryFetch(sizeof(short));

			short* ps = (short*)(((byte*)basePointer_) + nextPosition_);
			nextPosition_ += sizeof(short);
			return *ps;
		}
		#endregion

		#region ReadInt32
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe int ReadInt32()
		{
			this.TryFetch(sizeof(int));

			int* ps = (int*)(((byte*)basePointer_) + nextPosition_);
			nextPosition_ += sizeof(int);
			return *ps;
		}
		#endregion

		#region ReadInt64
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe long ReadInt64()
		{
			this.TryFetch(sizeof(long));

			long* ps = (long*)(((byte*)basePointer_) + nextPosition_);
			nextPosition_ += sizeof(long);
			return *ps;
		}
		#endregion

		#region ReadSingle
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe float ReadSingle()
		{
			this.TryFetch(sizeof(float));

			float* ps = (float*)(((byte*)basePointer_) + nextPosition_);
			nextPosition_ += sizeof(float);
			return *ps;
		}
		#endregion

		#region ReadDouble
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe double ReadDouble()
		{
			this.TryFetch(sizeof(double));

			double* ps = (double*)(((byte*)basePointer_) + nextPosition_);
			nextPosition_ += sizeof(double);
			return *ps;
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region ReadByteArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe byte[] ReadByteArray(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(sbyte);
			this.TryFetch(rawLength);

			var values = new byte[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (byte* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadSByteArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe sbyte[] ReadSByteArray(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(sbyte);
			this.TryFetch(rawLength);

			var values = new sbyte[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (sbyte* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadInt16Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe short[] ReadInt16Array(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(short);
			this.TryFetch(rawLength);

			var values = new short[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (short* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadUInt16Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe ushort[] ReadUInt16Array(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(ushort);
			this.TryFetch(rawLength);

			var values = new ushort[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (ushort* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadInt32Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe int[] ReadInt32Array(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(int);
			this.TryFetch(rawLength);

			var values = new int[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (int* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadUInt32Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe uint[] ReadUInt32Array(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(uint);
			this.TryFetch(rawLength);

			var values = new uint[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (uint* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadInt64Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe long[] ReadInt64Array(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(long);
			this.TryFetch(rawLength);

			var values = new long[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (long* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadUInt64Array
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe ulong[] ReadUInt64Array(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(ulong);
			this.TryFetch(rawLength);

			var values = new ulong[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (ulong* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadSingleArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe float[] ReadSingleArray(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(float);
			this.TryFetch(rawLength);

			var values = new float[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (float* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadDoubleArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe double[] ReadDoubleArray(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(double);
			this.TryFetch(rawLength);

			var values = new double[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (double* pd = &values[0])
			{
				ReflectionUtilities.UnsafeCopyBits(ps, (byte*)pd, rawLength);
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion

		#region ReadBooleanArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe bool[] ReadBooleanArray(int length)
		{
			Debug.Assert(length >= 1);

			var rawLength = length * sizeof(byte);
			this.TryFetch(rawLength);

			var values = new bool[length];
			byte* ps = ((byte*)basePointer_) + nextPosition_;

			fixed (bool* pv = &values[0])
			{
				bool* pd = pv;
				for (var index = 0; index < rawLength; index++)
				{
					*(pd++) = *(ps++) != 0;
				}
			}

			nextPosition_ += rawLength;
			return values;
		}
		#endregion
	}
}
#endif
