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
using System.Diagnostics;
using System.Text;

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Readers
{
	internal abstract partial class InternalReaderBase : IDisposable
	{
		#region Fields
		private readonly Encoding encoding_;
		private readonly bool isSingleByte_;
		private readonly byte[] buffer_;
		private int nextPosition_;
		#endregion

		#region OnFetch
		protected abstract int OnFetch(byte[] buffer, int offset, int length);
		#endregion

		#region TryFetch
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private void TryFetch(int requiredSize)
		{
			Debug.Assert(requiredSize >= 1);

			// TODO: fool strategy....

			var position = 0;
			while (position < requiredSize)
			{
				var read = this.OnFetch(buffer_, position, requiredSize - position);
				if (read <= 0)
				{
					throw new ArgumentException();
				}

				position += read;
			}

			nextPosition_ = 0;
		}
		#endregion

		//////////////////////////////////////////////////////////

		#region ReadBoolean
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public bool ReadBoolean()
		{
			return this.ReadByte() != 0x00;
		}
		#endregion

		#region ReadSByte
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public sbyte ReadSByte()
		{
			return (sbyte)this.ReadByte();
		}
		#endregion

		#region ReadUInt16
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public ushort ReadUInt16()
		{
			return (ushort)this.ReadInt16();
		}
		#endregion

		#region ReadUInt32
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public uint ReadUInt32()
		{
			return (uint)this.ReadInt32();
		}
		#endregion

		#region ReadUInt64
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public ulong ReadUInt64()
		{
			return (ulong)this.ReadInt64();
		}
		#endregion

		#region ReadChar
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public char ReadChar()
		{
			if (isSingleByte_)
			{
				return (char)this.ReadByte();
			}

			var rawLength = this.ReadByte();
			if (rawLength == 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			this.TryFetch(rawLength);

			var value = encoding_.GetString(buffer_, 0, rawLength)[0];
			nextPosition_ += rawLength;
			return value;
		}
		#endregion

		#region ReadString
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public string ReadString()
		{
			var rawLength = this.ReadUInt16();
			if (rawLength == 0xffff)
			{
				return null;
			}

			if (rawLength == 0)
			{
				return string.Empty;
			}

			this.TryFetch(rawLength);

			var value = encoding_.GetString(buffer_, nextPosition_, rawLength);
			nextPosition_ += rawLength;
			return value;
		}
		#endregion
	}
}
