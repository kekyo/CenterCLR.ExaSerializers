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

#if !USE_UNSAFE
using System;
using System.Diagnostics;
using System.Text;

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Readers
{
	internal abstract partial class InternalReaderBase
	{
		#region Constructors
		protected InternalReaderBase(Encoding encoding, int bufferSize)
		{
			encoding_ = encoding;
			isSingleByte_ = encoding_.GetMaxByteCount(1) == 1;
			buffer_ = new byte[bufferSize];
		}
		#endregion

		#region Dispose
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Dispose()
		{
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region ReadByte
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public byte ReadByte()
		{
			this.TryFetch(sizeof(byte));
			return buffer_[nextPosition_++];
		}
		#endregion

		#region ReadInt16
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public short ReadInt16()
		{
			this.TryFetch(sizeof(short));
			var value = BitConverter.ToInt16(buffer_, nextPosition_);
			nextPosition_ += sizeof(short);
			return value;
		}
		#endregion

		#region ReadInt32
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public int ReadInt32()
		{
			this.TryFetch(sizeof(int));
			var value = BitConverter.ToInt32(buffer_, nextPosition_);
			nextPosition_ += sizeof(int);
			return value;
		}
		#endregion

		#region ReadInt64
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public long ReadInt64()
		{
			this.TryFetch(sizeof(long));
			var value = BitConverter.ToInt64(buffer_, nextPosition_);
			nextPosition_ += sizeof(long);
			return value;
		}
		#endregion

		#region ReadSingle
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public float ReadSingle()
		{
			this.TryFetch(sizeof(float));
			var value = BitConverter.ToSingle(buffer_, nextPosition_);
			nextPosition_ += sizeof(float);
			return value;
		}
		#endregion

		#region ReadDouble
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public double ReadDouble()
		{
			var int64Value = this.ReadInt64();
			return BitConverter.Int64BitsToDouble(int64Value);
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region ReadByteArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public byte[] ReadByteArray(int length)
		{
			Debug.Assert(length >= 1);

			this.TryFetch(length);

			var values = new byte[length];
			Array.Copy(buffer_, nextPosition_, values, 0, length);
			nextPosition_ += length;
			return values;
		}
		#endregion
	}
}
#endif
