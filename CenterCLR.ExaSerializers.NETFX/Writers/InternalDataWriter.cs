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
using System.Security;
using System.Text;

using Windows.Storage.Streams;

namespace CenterCLR.ExaSerializers.Writers
{
	internal sealed class InternalDataWriter : InternalWriterBase
	{
		private IDataWriter dw_;

		[SecurityCritical]
		public InternalDataWriter(Encoding encoding, int bufferSize)
			: base(encoding, bufferSize)
		{
		}

		public void SetDataWriter(IDataWriter dw)
		{
			dw_ = dw;
			base.ResetPosition();
		}

		protected override void OnFlush(byte[] remainsBuffer, int length)
		{
			var temporaryBuffer = new byte[length];
			Array.Copy(remainsBuffer, 0, temporaryBuffer, 0, temporaryBuffer.Length);
			dw_.WriteBytes(temporaryBuffer);
		}
	}
}
