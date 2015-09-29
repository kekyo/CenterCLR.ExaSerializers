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
using System.IO;
using System.Text;

#if USE_UNSAFE
using System.Security;
#if NET35
using System.Security.Permissions;
#endif
#endif

using CenterCLR.ExaSerializers.Generators;
using CenterCLR.ExaSerializers.Writers;

namespace CenterCLR.ExaSerializers
{
	public sealed class ExaBinaryWriter<TValue> : IDisposable
	{
		private InternalBinaryWriter bw_;

		public ExaBinaryWriter(int bufferSize = 655360)
			: this(Encoding.UTF8, bufferSize)
		{
		}

		public ExaBinaryWriter(Encoding encoding, int bufferSize = 655360)
		{
			bw_ = new InternalBinaryWriter(encoding, bufferSize);
		}

#if USE_UNSAFE
#if NET35
		[SecurityPermission(SecurityAction.LinkDemand)] 
#else
		[SecurityCritical]
#endif
#endif
		public void Dispose()
		{
			if (bw_ != null)
			{
				bw_.Dispose();
				bw_ = null;
			}
		}

		public void Write(BinaryWriter bw, TValue value)
		{
			bw_.SetBinaryWriter(bw);
			WriterAgentGenerator<InternalBinaryWriter, TValue>.Run(bw_, value);
			bw_.Flush();
		}
	}
}
