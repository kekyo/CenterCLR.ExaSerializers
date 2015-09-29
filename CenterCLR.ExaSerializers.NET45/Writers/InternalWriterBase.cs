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

using System;
using System.Text;

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Writers
{
	internal abstract partial class InternalWriterBase : IDisposable
	{
		#region Fields
		private readonly Encoding encoding_;
		private readonly bool isSingleByte_;
		private readonly byte[] buffer_;
		private int nextPosition_;
		#endregion

		#region ResetPosition
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		protected void ResetPosition()
		{
			nextPosition_ = 0;
		}
		#endregion

		#region OnFlush
		protected abstract void OnFlush(byte[] remainsBuffer, int length);
		#endregion

		#region Flush
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Flush()
		{
			if (nextPosition_ >= 1)
			{
				this.OnFlush(buffer_, nextPosition_);
			}
		}
		#endregion

		#region TryFlush
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private void TryFlush(int requiredSize)
		{
			if (nextPosition_ > (buffer_.Length - requiredSize))
			{
				this.OnFlush(buffer_, nextPosition_);
				nextPosition_ = 0;
			}
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region WriteBoolean
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteBoolean(bool value)
#else
		public void WriteBoolean(bool value)
#endif
		{
#if USE_STRICT_EXPRESSION
			return
#endif
			this.WriteByte(value ? (byte)1 : (byte)0);
		}
		#endregion

		#region WriteSByte
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteSByte(sbyte value)
#else
		public void WriteSByte(sbyte value)
#endif
		{
#if USE_STRICT_EXPRESSION
			return
#endif
			this.WriteByte((byte)value);
		}
		#endregion

		#region WriteUInt16
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteUInt16(ushort value)
#else
		public void WriteUInt16(ushort value)
#endif
		{
#if USE_STRICT_EXPRESSION
			return
#endif
			this.WriteInt16((short)value);
		}
		#endregion

		#region WriteUInt32
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteUInt32(uint value)
#else
		public void WriteUInt32(uint value)
#endif
		{
			this.WriteInt32((int)value);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteUInt64
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteUInt64(ulong value)
#else
		public void WriteUInt64(ulong value)
#endif
		{
#if USE_STRICT_EXPRESSION
			return
#endif
			this.WriteInt64((long)value);
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region WriteChar
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteChar(char value)
#else
		public void WriteChar(char value)
#endif
		{
			if (isSingleByte_)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteByte((byte)value);
#else
				this.WriteByte((byte)value);
				return;
#endif
			}

			var encoded = encoding_.GetBytes(value.ToString());

			this.WriteByte((byte)encoded.Length);
			this.InternalWriteBytes(encoded);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteString
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteString(string value)
#else
		public void WriteString(string value)
#endif
		{
			if (value == null)
			{
#if USE_STRICT_EXPRESSION
				return this.WriteUInt16(0xffff);
#else
				this.WriteUInt16(0xffff);
				return;
#endif
			}

			var encoded = encoding_.GetBytes(value);
			var rawLength = encoded.Length;

			this.WriteUInt16((ushort)rawLength);
			this.InternalWriteBytes(encoded);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion

		#region WriteByteArray
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#if USE_STRICT_EXPRESSION
		public bool WriteByteArray(byte[] values)
#else
		public void WriteByteArray(byte[] values)
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
			this.InternalWriteBytes(values);

#if USE_STRICT_EXPRESSION
			return true;
#endif
		}
		#endregion
	}
}
