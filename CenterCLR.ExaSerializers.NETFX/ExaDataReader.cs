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
using CenterCLR.ExaSerializers.Readers;

namespace CenterCLR.ExaSerializers
{
	public sealed class ExaDataReader<TValue> : IDisposable
	{
		private InternalDataReader dr_;

		public ExaDataReader(int bufferSize = 655360)
			: this(Encoding.UTF8, bufferSize)
		{
		}

		public ExaDataReader(Encoding encoding, int bufferSize = 655360)
		{
			dr_ = new InternalDataReader(encoding, bufferSize);
		}

		[SecurityCritical]
		public void Dispose()
		{
			if (dr_ != null)
			{
				dr_.Dispose();
				dr_ = null;
			}
		}

		public TValue Read(IDataReader dr)
		{
			var valueHolder = new ValueHolder<TValue>();
			dr_.SetDataReader(dr);
			ReaderAgentGenerator<InternalDataReader, TValue>.Run(dr_, valueHolder);
			return valueHolder.Value;
		}
	}
}
