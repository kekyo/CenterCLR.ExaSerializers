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
using System.Linq;
using System.Runtime.InteropServices;

namespace CenterCLR.ExaSerializers
{
	public static class TestDataGenerator<TValue>
	{
		private static readonly Type type_ = typeof(TValue);

		public static readonly int ElementSize = GetElementSize();

		public static readonly TValue[] TestData = Enumerable.Range(0, 0x12345).Select(GetValue).ToArray();

		public static readonly byte[] TestDataBytes = TestData.SelectMany(GetBytes).ToArray();

		public static readonly byte[] TestDataBytesWithFlags =
			TestData.SelectMany(value => new[] { (byte)0x01 }.Concat(GetBytes(value))).Concat(new[] { (byte)0 }).ToArray();

		private static int GetElementSize()
		{
			if (type_ == typeof(bool))
			{
				return 1;
			}
			return Marshal.SizeOf(type_);
		}

		private static TValue GetValue(int index)
		{
			if (type_ == typeof(bool))
			{
				return (TValue)(object)((index % 2) == 1);
			}
			if (type_ == typeof(byte))
			{
				return (TValue)(object)(byte)index;
			}
			if (type_ == typeof(sbyte))
			{
				return (TValue)(object)(sbyte)index;
			}
			if (type_ == typeof(short))
			{
				return (TValue)(object)(short)index;
			}
			if (type_ == typeof(ushort))
			{
				return (TValue)(object)(ushort)index;
			}
			if (type_ == typeof(int))
			{
				return (TValue)(object)(int)index;
			}
			if (type_ == typeof(uint))
			{
				return (TValue)(object)(uint)index;
			}
			if (type_ == typeof(long))
			{
				return (TValue)(object)(long)index;
			}
			if (type_ == typeof(ulong))
			{
				return (TValue)(object)(ulong)index;
			}
			if (type_ == typeof(float))
			{
				return (TValue)(object)(float)index;
			}
			if (type_ == typeof(double))
			{
				return (TValue)(object)(double)index;
			}

			throw new ArgumentException();
		}

		private static byte[] GetBytes(TValue value)
		{
			if (type_ == typeof(bool))
			{
				return new[] { ((bool)(object)value) ? (byte)1 : (byte)0 };
			}
			if (type_ == typeof(byte))
			{
				return new[] { (byte)(object)value };
			}
			if (type_ == typeof(sbyte))
			{
				return new[] { (byte)(sbyte)(object)value };
			}
			if (type_ == typeof(short))
			{
				return BitConverter.GetBytes((short)(object)value);
			}
			if (type_ == typeof(ushort))
			{
				return BitConverter.GetBytes((ushort)(object)value);
			}
			if (type_ == typeof(int))
			{
				return BitConverter.GetBytes((int)(object)value);
			}
			if (type_ == typeof(uint))
			{
				return BitConverter.GetBytes((uint)(object)value);
			}
			if (type_ == typeof(long))
			{
				return BitConverter.GetBytes((long)(object)value);
			}
			if (type_ == typeof(ulong))
			{
				return BitConverter.GetBytes((ulong)(object)value);
			}
			if (type_ == typeof(float))
			{
				return BitConverter.GetBytes((float)(object)value);
			}
			if (type_ == typeof(double))
			{
				return BitConverter.GetBytes((double)(object)value);
			}

			throw new ArgumentException();
		}
	}
}
