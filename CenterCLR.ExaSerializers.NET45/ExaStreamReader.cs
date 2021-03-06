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
using CenterCLR.ExaSerializers.Readers;

namespace CenterCLR.ExaSerializers
{
	public sealed class ExaStreamReader<TValue> : IDisposable
	{
		private InternalStreamReader br_;

		public ExaStreamReader(int bufferSize = 655360)
			: this(Encoding.UTF8, bufferSize)
		{
		}

		public ExaStreamReader(Encoding encoding, int bufferSize = 655360)
		{
			br_ = new InternalStreamReader(encoding, bufferSize);
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
			if (br_ != null)
			{
				br_.Dispose();
				br_ = null;
			}
		}

		public TValue Read(Stream stream)
		{
			var valueHolder = new ValueHolder<TValue>();
			br_.SetStream(stream);
			ReaderAgentGenerator<InternalStreamReader, TValue>.Run(br_, valueHolder);
			return valueHolder.Value;
		}
	}
}
