﻿/////////////////////////////////////////////////////////////////////////////////////
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

using System.IO;
using System.Text;

#if USE_UNSAFE
using System.Security;
#if NET35
using System.Security.Permissions;
#endif
#endif

namespace CenterCLR.ExaSerializers.Writers
{
	internal sealed class InternalBinaryWriter : InternalWriterBase
	{
		private BinaryWriter bw_;

#if USE_UNSAFE
#if NET35
		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
#else
		[SecurityCritical]
#endif
#endif
		public InternalBinaryWriter(Encoding encoding, int bufferSize)
			: base(encoding, bufferSize)
		{
		}

		public void SetBinaryWriter(BinaryWriter bw)
		{
			bw_ = bw;
			base.ResetPosition();
		}

		protected override void OnFlush(byte[] remainsBuffer, int length)
		{
			bw_.Write(remainsBuffer, 0, length);
		}
	}
}
