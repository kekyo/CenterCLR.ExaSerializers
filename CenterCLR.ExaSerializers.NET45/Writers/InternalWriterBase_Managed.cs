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

namespace CenterCLR.ExaSerializers.Writers
{
	internal abstract partial class InternalWriterBase
	{
#region Constructors
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		protected InternalWriterBase(Encoding encoding, int bufferSize)
		{
			encoding_ = encoding;
			isSingleByte_ = encoding.GetMaxByteCount(1) == 1;
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

#region InternalWriteRawBytes
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private void InternalWriteBytes(byte[] values)
		{
			Debug.Assert(values != null);

			var rawLength = values.Length;
			if (rawLength == 0)
			{
				return;
			}

			this.TryFlush(rawLength);

			Array.Copy(values, 0, buffer_, nextPosition_, values.Length);
			nextPosition_ += values.Length;
		}
		#endregion

#region InternalWriteArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private void InternalWriteArray<TValue>(TValue[] values, int elementSize, Func<TValue, byte[]> converter)
		{
			if (values == null)
			{
				this.WriteInt32(-1);
				return;
			}

			this.WriteInt32(values.Length);
			if (values.Length == 0)
			{
				return;
			}

			var rawLength = values.Length * elementSize;
			this.TryFlush(rawLength);

			for (var index = 0; index < values.Length; index++)
			{
				var data = converter(values[index]);
				Array.Copy(data, 0, buffer_, nextPosition_, data.Length);
				nextPosition_ += data.Length;
			}
		}
		#endregion

		//////////////////////////////////////////////////////////

#region WriteByte
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteByte(byte value)
#else
		public void WriteByte(byte value)
#endif
		{
			this.TryFlush(sizeof(byte));
			buffer_[nextPosition_++] = value;

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
		public bool WriteInt16(short value)
#else
		public void WriteInt16(short value)
#endif
		{
			this.TryFlush(sizeof(short));
			buffer_[nextPosition_++] = (byte)value;
			buffer_[nextPosition_++] = (byte)(value >> 8);

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
		public bool WriteInt32(int value)
#else
		public void WriteInt32(int value)
#endif
		{
			this.TryFlush(sizeof(int));
			buffer_[nextPosition_++] = (byte)value;
			buffer_[nextPosition_++] = (byte)(value >> 8);
			buffer_[nextPosition_++] = (byte)(value >> 16);
			buffer_[nextPosition_++] = (byte)(value >> 24);

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
		public bool WriteInt64(long value)
#else
		public void WriteInt64(long value)
#endif
		{
			this.TryFlush(sizeof(long));
			buffer_[nextPosition_++] = (byte)value;
			buffer_[nextPosition_++] = (byte)(value >> 8);
			buffer_[nextPosition_++] = (byte)(value >> 16);
			buffer_[nextPosition_++] = (byte)(value >> 24);
			buffer_[nextPosition_++] = (byte)(value >> 32);
			buffer_[nextPosition_++] = (byte)(value >> 40);
			buffer_[nextPosition_++] = (byte)(value >> 48);
			buffer_[nextPosition_++] = (byte)(value >> 56);

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
		public bool WriteSingle(float value)
#else
		public void WriteSingle(float value)
#endif
		{
			this.TryFlush(sizeof(float));
			var rawValue = BitConverter.GetBytes(value);
			Array.Copy(rawValue, 0, buffer_, nextPosition_, sizeof(float));
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
		public bool WriteDouble(double value)
#else
		public void WriteDouble(double value)
#endif
		{
			var rawValue = BitConverter.DoubleToInt64Bits(value);
			this.WriteInt64(rawValue);

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
		public bool WriteSByteArray(sbyte[] values)
#else
		public void WriteSByteArray(sbyte[] values)
#endif
		{
			if (values == null)
			{
				this.WriteInt32(-1);
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.WriteInt32(values.Length);
			if (values.Length == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(values.Length);

			for (var index = 0; index < values.Length; index++)
			{
				buffer_[nextPosition_++] = (byte)values[index];
			}

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
		public bool WriteInt16Array(short[] values)
#else
		public void WriteInt16Array(short[] values)
#endif
		{
			InternalWriteArray(values, sizeof(short), BitConverter.GetBytes);

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
		public bool WriteUInt16Array(ushort[] values)
#else
		public void WriteUInt16Array(ushort[] values)
#endif
		{
			InternalWriteArray(values, sizeof(ushort), BitConverter.GetBytes);

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
		public bool WriteInt32Array(int[] values)
#else
		public void WriteInt32Array(int[] values)
#endif
		{
			InternalWriteArray(values, sizeof(int), BitConverter.GetBytes);

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
		public bool WriteUInt32Array(uint[] values)
#else
		public void WriteUInt32Array(uint[] values)
#endif
		{
			InternalWriteArray(values, sizeof(uint), BitConverter.GetBytes);

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
		public bool WriteInt64Array(long[] values)
#else
		public void WriteInt64Array(long[] values)
#endif
		{
			InternalWriteArray(values, sizeof(long), BitConverter.GetBytes);

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
		public bool WriteUInt64Array(ulong[] values)
#else
		public void WriteUInt64Array(ulong[] values)
#endif
		{
			InternalWriteArray(values, sizeof(ulong), BitConverter.GetBytes);

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
		public bool WriteSingleArray(float[] values)
#else
		public void WriteSingleArray(float[] values)
#endif
		{
			InternalWriteArray(values, sizeof(float), BitConverter.GetBytes);

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
		public bool WriteDoubleArray(double[] values)
#else
		public void WriteDoubleArray(double[] values)
#endif
		{
			InternalWriteArray(values, sizeof(double), BitConverter.GetBytes);

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
		public bool WriteBooleanArray(bool[] values)
#else
		public void WriteBooleanArray(bool[] values)
#endif
		{
			if (values == null)
			{
				this.WriteInt32(-1);
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.WriteInt32(values.Length);
			if (values.Length == 0)
			{
#if USE_STRICT_EXPRESSION
				return true;
#else
				return;
#endif
			}

			this.TryFlush(values.Length);

			for (var index = 0; index < values.Length; index++)
			{
				buffer_[nextPosition_++] = values[index] ? (byte)1 : (byte)0;
			}

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion
	}
}
#endif
