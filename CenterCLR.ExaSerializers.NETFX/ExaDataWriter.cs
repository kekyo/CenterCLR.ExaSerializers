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
using CenterCLR.ExaSerializers.Generators;
using CenterCLR.ExaSerializers.Writers;

namespace CenterCLR.ExaSerializers
{
	public sealed class ExaDataWriter<TValue> : IDisposable
	{
		private InternalDataWriter dw_;

		public ExaDataWriter(int bufferSize = 655360)
			: this(Encoding.UTF8, bufferSize)
		{
		}

		public ExaDataWriter(Encoding encoding, int bufferSize = 655360)
		{
			dw_ = new InternalDataWriter(encoding, bufferSize);
		}

		[SecurityCritical]
		public void Dispose()
		{
			if (dw_ != null)
			{
				dw_.Dispose();
				dw_ = null;
			}
		}

		public void Write(IDataWriter dw, TValue value)
		{
			dw_.SetDataWriter(dw);
			WriterAgentGenerator<InternalDataWriter, TValue>.Run(dw_, value);
			dw_.Flush();
		}
	}
}
