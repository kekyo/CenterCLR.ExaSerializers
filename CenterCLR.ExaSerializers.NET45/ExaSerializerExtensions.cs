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

using System.Collections.Generic;
using System.IO;

namespace CenterCLR.ExaSerializers
{
	public static class ExaSerializerExtensions
	{
		public static void WriteValue<TValue>(this Stream stream, TValue value, int bufferSize = 655360)
		{
			using (var ebw = new ExaStreamWriter<TValue>(bufferSize))
			{
				ebw.Write(stream, value);
			}
		}

		public static void WriteValue<TValue>(this BinaryWriter bw, TValue value, int bufferSize = 655360)
		{
			using (var ebw = new ExaBinaryWriter<TValue>(bufferSize))
			{
				ebw.Write(bw, value);
			}
		}

		public static TValue ReadValue<TValue>(this Stream stream, int bufferSize = 655360)
		{
			using (var ebr = new ExaStreamReader<TValue>(bufferSize))
			{
				return ebr.Read(stream);
			}
		}

		public static TValue ReadValue<TValue>(this BinaryReader br, int bufferSize = 655360)
		{
			using (var ebr = new ExaBinaryReader<TValue>(bufferSize))
			{
				return ebr.Read(br);
			}
		}

		public static IEnumerable<TValue> EnumerateValues<TValue>(this Stream stream, int bufferSize = 655360)
		{
			using (var ebr = new ExaStreamReader<IEnumerable<TValue>>(bufferSize))
			{
				var enumerable = ebr.Read(stream);
				if (enumerable != null)
				{
					foreach (var value in enumerable)
					{
						yield return value;
					}
				}
			}
		}

		public static IEnumerable<TValue> EnumerateValues<TValue>(this BinaryReader br, int bufferSize = 655360)
		{
			using (var ebr = new ExaBinaryReader<IEnumerable<TValue>>(bufferSize))
			{
				var enumerable = ebr.Read(br);
				if (enumerable != null)
				{
					foreach (var value in enumerable)
					{
						yield return value;
					}
				}
			}
		}
	}
}
