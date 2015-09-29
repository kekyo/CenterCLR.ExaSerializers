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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable 649		// non initialized field

namespace CenterCLR.ExaSerializers
{
	[TestClass]
	public sealed class ReadTypesTest
	{
		#region PrimitiveTypes
		private void PrimitiveTypeTestMethod<T>(byte[] data, T expected)
		{
			using (var ms = new MemoryStream(data))
			{
				var actual = ms.ReadValue<T>();
				if (expected == null)
				{
					Assert.IsNull(actual);
				}
				else
				{
					Assert.AreEqual(expected, actual);
				}
			}
		}

		[TestMethod]
		public void BooleanTest()
		{
			PrimitiveTypeTestMethod(new byte[] { 0 }, false);
			PrimitiveTypeTestMethod(new byte[] { 1 }, true);
		}

		[TestMethod]
		public void ByteTest()
		{
			PrimitiveTypeTestMethod(new byte[] { 0 }, (byte)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff }, byte.MaxValue);
		}

		[TestMethod]
		public void SByteTest()
		{
			PrimitiveTypeTestMethod(new byte[] { 0 }, (sbyte)0);
			PrimitiveTypeTestMethod(new byte[] { 0x7f }, sbyte.MaxValue);
			PrimitiveTypeTestMethod(new byte[] { 0xff }, (sbyte)-1);
		}

		[TestMethod]
		public void Int16Test()
		{
			PrimitiveTypeTestMethod(new byte[] { 0, 0 }, (short)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0x7f }, short.MaxValue);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff }, (short)-1);
		}

		[TestMethod]
		public void UInt16Test()
		{
			PrimitiveTypeTestMethod(new byte[] { 0, 0 }, (ushort)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff }, ushort.MaxValue);
		}

		[TestMethod]
		public void Int32Test()
		{
			PrimitiveTypeTestMethod(new byte[] { 0, 0, 0, 0 }, (int)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff, 0xff, 0x7f }, int.MaxValue);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff, 0xff, 0xff }, (int)-1);
		}

		[TestMethod]
		public void UInt32Test()
		{
			PrimitiveTypeTestMethod(new byte[] { 0, 0, 0, 0 }, (uint)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff, 0xff, 0xff }, uint.MaxValue);
		}

		[TestMethod]
		public void Int64Test()
		{
			PrimitiveTypeTestMethod(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, (long)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, long.MaxValue);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, (long)-1);
		}

		[TestMethod]
		public void UInt64Test()
		{
			PrimitiveTypeTestMethod(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, (ulong)0);
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, ulong.MaxValue);
		}

		[TestMethod]
		public void SingleTest()
		{
			PrimitiveTypeTestMethod(BitConverter.GetBytes((float)0), (float)0);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(float.MinValue), float.MinValue);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(float.MaxValue), float.MaxValue);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(float.Epsilon), float.Epsilon);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(float.NaN), float.NaN);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(float.PositiveInfinity), float.PositiveInfinity);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(float.NegativeInfinity), float.NegativeInfinity);
		}

		[TestMethod]
		public void DoubleTest()
		{
			PrimitiveTypeTestMethod(BitConverter.GetBytes((double)0), (double)0);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(double.MinValue), double.MinValue);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(double.MaxValue), double.MaxValue);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(double.Epsilon), double.Epsilon);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(double.NaN), double.NaN);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(double.PositiveInfinity), double.PositiveInfinity);
			PrimitiveTypeTestMethod(BitConverter.GetBytes(double.NegativeInfinity), double.NegativeInfinity);
		}

		[TestMethod]
		public void StringTest()
		{
			PrimitiveTypeTestMethod(new byte[] { 0xff, 0xff }, (string)null);
			PrimitiveTypeTestMethod(new byte[] { 0, 0 }, string.Empty);
			PrimitiveTypeTestMethod(new byte[] { 0x07, 0x00, (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G' }, "ABCDEFG");

			var data = Encoding.UTF8.GetBytes("あいうえお");
			PrimitiveTypeTestMethod(BitConverter.GetBytes((ushort)data.Length).Concat(data).ToArray(), "あいうえお");
		}

		[TestMethod]
		public void CharTest()
		{
			PrimitiveTypeTestMethod(new byte[] { 0x01, (byte)'A' }, 'A');

			var data = Encoding.UTF8.GetBytes("あ");
			PrimitiveTypeTestMethod(new byte[] { (byte)data.Length }.Concat(data).ToArray(), 'あ');
		}
		#endregion

		#region CombinedPrimitiveTypesTest
		[TestMethod]
		public void CombinedPrimitiveTypesTest()
		{
			var floatValueOctets = BitConverter.GetBytes(123.456789f);
			var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0xc7,
					0x7c,
					0x34, 0x12,
					0xdc, 0xfe,
					0x10, 0x32, 0x54, 0x76,
					0x33, 0x77, 0xbb, 0xff,
					0x78, 0x56, 0x34, 0x12, 0xf0, 0xde, 0xbc, 0x0a,
					0x89, 0x67, 0x45, 0x23, 0x01, 0xef, 0xcd, 0xab,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
				}.
				Concat(floatValueOctets).
				Concat(doubleValueOctets).
				Concat(new[] { (byte)0x01, (byte)'T' }).
				ToArray()))
			{
				Assert.AreEqual(true, ms.ReadValue<bool>());
				Assert.AreEqual((byte)0xc7, ms.ReadValue<byte>());
				Assert.AreEqual((sbyte)0x7c, ms.ReadValue<sbyte>());
				Assert.AreEqual((short)0x1234, ms.ReadValue<short>());
				Assert.AreEqual((ushort)0xfedc, ms.ReadValue<ushort>());
				Assert.AreEqual((int)0x76543210, ms.ReadValue<int>());
				Assert.AreEqual((uint)0xffbb7733, ms.ReadValue<uint>());
				Assert.AreEqual((long)0xabcdef012345678, ms.ReadValue<long>());
				Assert.AreEqual((ulong)0xabcdef0123456789, ms.ReadValue<ulong>());
				Assert.AreEqual("ABCDEFG", ms.ReadValue<string>());
				Assert.AreEqual(123.456789f, ms.ReadValue<float>());
				Assert.AreEqual(123.45678901234567, ms.ReadValue<double>());
				Assert.AreEqual('T', ms.ReadValue<char>());

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion

		#region EnumTypes
		private static void EnumTypeTestMethod<T>()
		{
			Func<object, byte[]> converter;
			var ut = Enum.GetUnderlyingType(typeof(T));
			if (ut == typeof(byte))
			{
				converter = obj => new[] { (byte)obj };
			}
			else if (ut == typeof(sbyte))
			{
				converter = obj => new[] { (byte)(sbyte)obj };
			}
			else if (ut == typeof(short))
			{
				converter = obj => BitConverter.GetBytes((short)obj);
			}
			else if (ut == typeof(ushort))
			{
				converter = obj => BitConverter.GetBytes((ushort)obj);
			}
			else if (ut == typeof(int))
			{
				converter = obj => BitConverter.GetBytes((int)obj);
			}
			else if (ut == typeof(uint))
			{
				converter = obj => BitConverter.GetBytes((uint)obj);
			}
			else if (ut == typeof(long))
			{
				converter = obj => BitConverter.GetBytes((long)obj);
			}
			else if (ut == typeof(ulong))
			{
				converter = obj => BitConverter.GetBytes((ulong)obj);
			}
			else
			{
				Assert.Fail();
				return;
			}

			foreach (var entry in
				Enum.GetNames(typeof(T)).
				Select(name => (T)Enum.Parse(typeof(T), name)).
				Zip(Enum.GetValues(typeof(T)).Cast<object>(), (ev, nv) => new { EnumValue = ev, Data = converter(nv) }))
			{
				using (var ms = new MemoryStream(entry.Data))
				{
					var value = ms.ReadValue<T>();

					Assert.AreEqual(entry.EnumValue, value);
				}
			}
		}

		private enum EnumByteValues : byte
		{
			Value0 = 0,
			ValueMax = byte.MaxValue
		}

		[TestMethod]
		public void EnumByteTypeTest()
		{
			EnumTypeTestMethod<EnumByteValues>();
		}

		private enum EnumSByteValues : sbyte
		{
			Value0 = 0,
			ValueMax = sbyte.MaxValue,
			ValueMin = sbyte.MinValue
		}

		[TestMethod]
		public void EnumSByteTypeTest()
		{
			EnumTypeTestMethod<EnumSByteValues>();
		}

		private enum EnumShortValues : short
		{
			Value0 = 0,
			ValueMax = short.MaxValue,
			ValueMin = short.MinValue
		}

		[TestMethod]
		public void EnumShortTypeTest()
		{
			EnumTypeTestMethod<EnumShortValues>();
		}

		private enum EnumUShortValues : ushort
		{
			Value0 = 0,
			ValueMax = ushort.MaxValue
		}

		[TestMethod]
		public void EnumUShortTypeTest()
		{
			EnumTypeTestMethod<EnumUShortValues>();
		}

		private enum EnumIntValues : int
		{
			Value0 = 0,
			ValueMax = int.MaxValue,
			ValueMin = int.MinValue
		}

		[TestMethod]
		public void EnumIntTypeTest()
		{
			EnumTypeTestMethod<EnumIntValues>();
		}

		private enum EnumUIntValues : uint
		{
			Value0 = 0,
			ValueMax = uint.MaxValue
		}

		[TestMethod]
		public void EnumUIntTypeTest()
		{
			EnumTypeTestMethod<EnumUIntValues>();
		}

		private enum EnumLongValues : long
		{
			Value0 = 0,
			ValueMax = long.MaxValue,
			ValueMin = long.MinValue
		}

		[TestMethod]
		public void EnumLongTypeTest()
		{
			EnumTypeTestMethod<EnumLongValues>();
		}

		private enum EnumULongValues : ulong
		{
			Value0 = 0,
			ValueMax = ulong.MaxValue
		}

		[TestMethod]
		public void EnumULongTypeTest()
		{
			EnumTypeTestMethod<EnumULongValues>();
		}
		#endregion

		#region ArrayTypes
		private void ArrayTypesTestMethod<TValue>()
		{
			var arrayValue = TestDataGenerator<TValue>.TestData;

			using (var ms = new MemoryStream(new byte[]
				{
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(TestDataGenerator<TValue>.TestDataBytes).
				ToArray()))
			{
				Assert.IsTrue(arrayValue.SequenceEqual(ms.ReadValue<TValue[]>()));
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xff, 0xff, 0xff, 0xff,
				}))
			{
				Assert.IsNull(ms.ReadValue<TValue[]>());
				Assert.AreEqual(ms.Length, ms.Position);
			}
		}

		[TestMethod]
		public void ArrayByteTest()
		{
			this.ArrayTypesTestMethod<byte>();
		}

		[TestMethod]
		public void ArraySByteTest()
		{
			this.ArrayTypesTestMethod<sbyte>();
		}

		[TestMethod]
		public void ArrayInt16Test()
		{
			this.ArrayTypesTestMethod<short>();
		}

		[TestMethod]
		public void ArrayUInt16Test()
		{
			this.ArrayTypesTestMethod<ushort>();
		}

		[TestMethod]
		public void ArrayInt32Test()
		{
			this.ArrayTypesTestMethod<int>();
		}

		[TestMethod]
		public void ArrayUInt32Test()
		{
			this.ArrayTypesTestMethod<uint>();
		}

		[TestMethod]
		public void ArrayInt64Test()
		{
			this.ArrayTypesTestMethod<long>();
		}

		[TestMethod]
		public void ArrayUInt64Test()
		{
			this.ArrayTypesTestMethod<ulong>();
		}

		[TestMethod]
		public void ArraySingleTest()
		{
			this.ArrayTypesTestMethod<float>();
		}

		[TestMethod]
		public void ArrayDoubleTest()
		{
			this.ArrayTypesTestMethod<double>();
		}

		[TestMethod]
		public void ArrayBooleanTest()
		{
			this.ArrayTypesTestMethod<bool>();
		}
		#endregion

		#region CollectionTypes
		private void CollectionTestMethod<TCollection, TRealCollection, TValue>()
			where TCollection : class, IEnumerable<TValue>
			where TRealCollection : class, IList<TValue>
		{
			var arrayValue = TestDataGenerator<TValue>.TestData;

			using (var ms = new MemoryStream(new byte[]
				{
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(TestDataGenerator<TValue>.TestDataBytes).
				ToArray()))
			{
				var list = ms.ReadValue<TCollection>();

				Assert.IsInstanceOfType(list, typeof(TRealCollection));
				Assert.IsTrue(arrayValue.SequenceEqual(list));
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0x00, 0x00, 0x00, 0x00
				}))
			{
				var list = ms.ReadValue<TCollection>();

				Assert.IsInstanceOfType(list, typeof(TRealCollection));
				Assert.AreEqual(0, list.Count());
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xfe, 0xff, 0xff, 0xff,
				}.
				Concat(TestDataGenerator<TValue>.TestDataBytesWithFlags).
				ToArray()))
			{
				var list = ms.ReadValue<TCollection>();

				Assert.IsInstanceOfType(list, typeof(TRealCollection));
				Assert.IsTrue(arrayValue.SequenceEqual(list));
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xfe, 0xff, 0xff, 0xff,
					0x00
				}))
			{
				var list = ms.ReadValue<TCollection>();

				Assert.IsInstanceOfType(list, typeof(TRealCollection));
				Assert.AreEqual(0, list.Count());
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xff, 0xff, 0xff, 0xff,
				}))
			{
				var list = ms.ReadValue<TCollection>();

				Assert.IsNull(list);
				Assert.AreEqual(ms.Length, ms.Position);
			}
		}

		private void CombinedCollectionTestMethod<TValue>()
		{
			CollectionTestMethod<List<TValue>, List<TValue>, TValue>();
			CollectionTestMethod<ObservableCollection<TValue>, ObservableCollection<TValue>, TValue>();
			CollectionTestMethod<IList<TValue>, List<TValue>, TValue>();
#if NET45 || NETFX_CORE
			CollectionTestMethod<IReadOnlyCollection<TValue>, TValue[], TValue>();
#endif
		}

		[TestMethod]
		public void CollectionByteTest()
		{
			CombinedCollectionTestMethod<byte>();
		}

		[TestMethod]
		public void CollectionSByteTest()
		{
			CombinedCollectionTestMethod<sbyte>();
		}

		[TestMethod]
		public void CollectionInt16Test()
		{
			CombinedCollectionTestMethod<short>();
		}

		[TestMethod]
		public void CollectionUInt16Test()
		{
			CombinedCollectionTestMethod<ushort>();
		}

		[TestMethod]
		public void CollectionInt32Test()
		{
			CombinedCollectionTestMethod<int>();
		}

		[TestMethod]
		public void CollectionUInt32Test()
		{
			CombinedCollectionTestMethod<uint>();
		}

		[TestMethod]
		public void CollectionInt64Test()
		{
			CombinedCollectionTestMethod<long>();
		}

		[TestMethod]
		public void CollectionUInt64Test()
		{
			CombinedCollectionTestMethod<ulong>();
		}

		[TestMethod]
		public void CollectionSingleTest()
		{
			CombinedCollectionTestMethod<float>();
		}

		[TestMethod]
		public void CollectionDoubleTest()
		{
			CombinedCollectionTestMethod<double>();
		}

		[TestMethod]
		public void CollectionBooleanTest()
		{
			CombinedCollectionTestMethod<bool>();
		}
		#endregion

		#region EnumerableTypes
		private void EnumerableTestMethod<TValue>()
		{
			var arrayValue = TestDataGenerator<TValue>.TestData;

			using (var ms = new MemoryStream(new byte[]
				{
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(TestDataGenerator<TValue>.TestDataBytes).
				ToArray()))
			{
				foreach (var entry in ms.EnumerateValues<TValue>().
					Select((sav, index) => new { sav, index }).
					Zip(arrayValue, (entry, arv) => new { entry.index, entry.sav, arv }))
				{
					Assert.AreEqual(entry.sav, entry.arv);
					Assert.AreEqual(ms.Position, (entry.index + 1) * TestDataGenerator<TValue>.ElementSize + 4);
				}

				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0x00, 0x00, 0x00, 0x00
				}))
			{
				var enumerable = ms.EnumerateValues<TValue>();

				Assert.IsNotNull(enumerable);
				Assert.IsFalse(enumerable.Any());
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xfe, 0xff, 0xff, 0xff,
				}.
				Concat(TestDataGenerator<TValue>.TestDataBytesWithFlags).
				ToArray()))
			{
				foreach (var entry in ms.EnumerateValues<TValue>().
					Select((sav, index) => new { sav, index }).
					Zip(arrayValue, (entry, arv) => new { entry.index, entry.sav, arv }))
				{
					Assert.AreEqual(entry.sav, entry.arv);
					Assert.AreEqual(ms.Position, (entry.index + 1) * (TestDataGenerator<TValue>.ElementSize + 1) + 4);
				}

				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xfe, 0xff, 0xff, 0xff,
					0x00
				}))
			{
				var enumerable = ms.EnumerateValues<TValue>();

				Assert.IsNotNull(enumerable);
				Assert.IsFalse(enumerable.Any());
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xff, 0xff, 0xff, 0xff,
				}))
			{
				var enumerable = ms.EnumerateValues<TValue>();

				Assert.IsNotNull(enumerable);
				Assert.IsFalse(enumerable.Any());
				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0xff, 0xff, 0xff, 0xff,
				}))
			{
				using (var sr = new ExaStreamReader<IEnumerable<TValue>>())
				{
					var enumerable = sr.Read(ms);

					Assert.IsNull(enumerable);
					Assert.AreEqual(ms.Length, ms.Position);
				}
			}
		}

		[TestMethod]
		public void EnumerableByteTest()
		{
			EnumerableTestMethod<byte>();
		}

		[TestMethod]
		public void EnumerableSByteTest()
		{
			EnumerableTestMethod<sbyte>();
		}

		[TestMethod]
		public void EnumerableInt16Test()
		{
			EnumerableTestMethod<short>();
		}

		[TestMethod]
		public void EnumerableUInt16Test()
		{
			EnumerableTestMethod<ushort>();
		}

		[TestMethod]
		public void EnumerableInt32Test()
		{
			EnumerableTestMethod<int>();
		}

		[TestMethod]
		public void EnumerableUInt32Test()
		{
			EnumerableTestMethod<uint>();
		}

		[TestMethod]
		public void EnumerableInt64Test()
		{
			EnumerableTestMethod<long>();
		}

		[TestMethod]
		public void EnumerableUInt64Test()
		{
			EnumerableTestMethod<ulong>();
		}

		[TestMethod]
		public void EnumerableSingleTest()
		{
			EnumerableTestMethod<float>();
		}

		[TestMethod]
		public void EnumerableDoubleTest()
		{
			EnumerableTestMethod<double>();
		}

		[TestMethod]
		public void EnumerableBooleanTest()
		{
			EnumerableTestMethod<bool>();
		}
		#endregion

		#region StandardClassFieldModel
		private class StandardClassFieldModel
		{
			public bool BoolValue;
			public byte ByteValue;
			public sbyte SByteValue;
			public short Int16Value;
			public ushort UInt16Value;
			public int Int32Value;
			public uint UInt32Value;
			public long Int64Value;
			public ulong UInt64Value;
			public string StringValue;
			public byte[] ByteArrayValue;
			public float SingleValue;
			public double DoubleValue;
			public char CharValue;
		}

		[TestMethod]
		public void StandardClassFieldModelTest()
		{
			var byteArrayValue = TestDataGenerator<byte>.TestData;
			var floatValueOctets = BitConverter.GetBytes(123.456789f);
			var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x01,
					0xc7,
					0x7c,
					0x34, 0x12,
					0xdc, 0xfe,
					0x10, 0x32, 0x54, 0x76,
					0x33, 0x77, 0xbb, 0xff,
					0x78, 0x56, 0x34, 0x12, 0xf0, 0xde, 0xbc, 0x0a,
					0x89, 0x67, 0x45, 0x23, 0x01, 0xef, 0xcd, 0xab,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(byteArrayValue).
				Concat(floatValueOctets).
				Concat(doubleValueOctets).
				Concat(new[] { (byte)0x01, (byte)'T' }).
				ToArray()))
			{
				var model = ms.ReadValue<StandardClassFieldModel>();

				Assert.AreEqual(true, model.BoolValue);
				Assert.AreEqual((byte)0xc7, model.ByteValue);
				Assert.AreEqual((sbyte)0x7c, model.SByteValue);
				Assert.AreEqual((short)0x1234, model.Int16Value);
				Assert.AreEqual((ushort)0xfedc, model.UInt16Value);
				Assert.AreEqual(0x76543210, model.Int32Value);
				Assert.AreEqual((uint)0xffbb7733, model.UInt32Value);
				Assert.AreEqual(0xabcdef012345678, model.Int64Value);
				Assert.AreEqual((ulong)0xabcdef0123456789, model.UInt64Value);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.IsTrue(byteArrayValue.SequenceEqual(model.ByteArrayValue));
				Assert.AreEqual(123.456789f, model.SingleValue);
				Assert.AreEqual(123.45678901234567, model.DoubleValue);
				Assert.AreEqual('T', model.CharValue);

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion

		#region StandardClassPropertyModel
		private class StandardClassPropertyModel
		{
			public bool BoolValue { get; set; }
			public byte ByteValue { get; set; }
			public sbyte SByteValue { get; set; }
			public short Int16Value { get; set; }
			public ushort UInt16Value { get; set; }
			public int Int32Value { get; set; }
			public uint UInt32Value { get; set; }
			public long Int64Value { get; set; }
			public ulong UInt64Value { get; set; }
			public string StringValue { get; set; }
			public byte[] ByteArrayValue { get; set; }
			public float SingleValue { get; set; }
			public double DoubleValue { get; set; }
			public char CharValue { get; set; }
		}

		[TestMethod]
		public void StandardClassPropertyModelTest()
		{
			var byteArrayValue = TestDataGenerator<byte>.TestData;
			var floatValueOctets = BitConverter.GetBytes(123.456789f);
			var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x01,
					0xc7,
					0x7c,
					0x34, 0x12,
					0xdc, 0xfe,
					0x10, 0x32, 0x54, 0x76,
					0x33, 0x77, 0xbb, 0xff,
					0x78, 0x56, 0x34, 0x12, 0xf0, 0xde, 0xbc, 0x0a,
					0x89, 0x67, 0x45, 0x23, 0x01, 0xef, 0xcd, 0xab,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(byteArrayValue).
				Concat(floatValueOctets).
				Concat(doubleValueOctets).
				Concat(new[] { (byte)0x01, (byte)'T' }).
				ToArray()))
			{
				var model = ms.ReadValue<StandardClassPropertyModel>();

				Assert.AreEqual(true, model.BoolValue);
				Assert.AreEqual((byte)0xc7, model.ByteValue);
				Assert.AreEqual((sbyte)0x7c, model.SByteValue);
				Assert.AreEqual((short)0x1234, model.Int16Value);
				Assert.AreEqual((ushort)0xfedc, model.UInt16Value);
				Assert.AreEqual(0x76543210, model.Int32Value);
				Assert.AreEqual((uint)0xffbb7733, model.UInt32Value);
				Assert.AreEqual(0xabcdef012345678, model.Int64Value);
				Assert.AreEqual((ulong)0xabcdef0123456789, model.UInt64Value);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.IsTrue(byteArrayValue.SequenceEqual(model.ByteArrayValue));
				Assert.AreEqual(123.456789f, model.SingleValue);
				Assert.AreEqual(123.45678901234567, model.DoubleValue);
				Assert.AreEqual('T', model.CharValue);

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion

		#region EmptyClassModel
		private class EmptyClassModel
		{
		}

		[TestMethod]
		public void EmptyClassModelTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
				}))
			{
				var model = ms.ReadValue<EmptyClassModel>();

				Assert.IsNotNull(model);

				Assert.AreEqual(ms.Length, ms.Position);
			}

			using (var ms = new MemoryStream(new byte[]
				{
					0x00,
				}))
			{
				var model = ms.ReadValue<EmptyClassModel>();

				Assert.IsNull(model);

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion

		#region StandardStructFieldModel
		private struct StandardStructFieldModel
		{
			public bool BoolValue;
			public byte ByteValue;
			public sbyte SByteValue;
			public short Int16Value;
			public ushort UInt16Value;
			public int Int32Value;
			public uint UInt32Value;
			public long Int64Value;
			public ulong UInt64Value;
			public string StringValue;
			public byte[] ByteArrayValue;
			public float SingleValue;
			public double DoubleValue;
			public char CharValue;
		}

		[TestMethod]
		public void StandardStructFieldModelTest()
		{
			var byteArrayValue = TestDataGenerator<byte>.TestData;
			var floatValueOctets = BitConverter.GetBytes(123.456789f);
			var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0xc7,
					0x7c,
					0x34, 0x12,
					0xdc, 0xfe,
					0x10, 0x32, 0x54, 0x76,
					0x33, 0x77, 0xbb, 0xff,
					0x78, 0x56, 0x34, 0x12, 0xf0, 0xde, 0xbc, 0x0a,
					0x89, 0x67, 0x45, 0x23, 0x01, 0xef, 0xcd, 0xab,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(byteArrayValue).
				Concat(floatValueOctets).
				Concat(doubleValueOctets).
				Concat(new[] { (byte)0x01, (byte)'T' }).
				ToArray()))
			{
				var model = ms.ReadValue<StandardStructFieldModel>();

				Assert.AreEqual(true, model.BoolValue);
				Assert.AreEqual((byte)0xc7, model.ByteValue);
				Assert.AreEqual((sbyte)0x7c, model.SByteValue);
				Assert.AreEqual((short)0x1234, model.Int16Value);
				Assert.AreEqual((ushort)0xfedc, model.UInt16Value);
				Assert.AreEqual(0x76543210, model.Int32Value);
				Assert.AreEqual((uint)0xffbb7733, model.UInt32Value);
				Assert.AreEqual(0xabcdef012345678, model.Int64Value);
				Assert.AreEqual((ulong)0xabcdef0123456789, model.UInt64Value);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.IsTrue(byteArrayValue.SequenceEqual(model.ByteArrayValue));
				Assert.AreEqual(123.456789f, model.SingleValue);
				Assert.AreEqual(123.45678901234567, model.DoubleValue);
				Assert.AreEqual('T', model.CharValue);

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion

		#region StandardStructPropertyModel
		private struct StandardStructPropertyModel
		{
			public bool BoolValue { get; set; }
			public byte ByteValue { get; set; }
			public sbyte SByteValue { get; set; }
			public short Int16Value { get; set; }
			public ushort UInt16Value { get; set; }
			public int Int32Value { get; set; }
			public uint UInt32Value { get; set; }
			public long Int64Value { get; set; }
			public ulong UInt64Value { get; set; }
			public string StringValue { get; set; }
			public byte[] ByteArrayValue { get; set; }
			public float SingleValue { get; set; }
			public double DoubleValue { get; set; }
			public char CharValue { get; set; }
		}

		[TestMethod]
		public void StandardStructPropertyModelTest()
		{
			var byteArrayValue = TestDataGenerator<byte>.TestData;
			var floatValueOctets = BitConverter.GetBytes(123.456789f);
			var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0xc7,
					0x7c,
					0x34, 0x12,
					0xdc, 0xfe,
					0x10, 0x32, 0x54, 0x76,
					0x33, 0x77, 0xbb, 0xff,
					0x78, 0x56, 0x34, 0x12, 0xf0, 0xde, 0xbc, 0x0a,
					0x89, 0x67, 0x45, 0x23, 0x01, 0xef, 0xcd, 0xab,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(byteArrayValue).
				Concat(floatValueOctets).
				Concat(doubleValueOctets).
				Concat(new[] { (byte)0x01, (byte)'T' }).
				ToArray()))
			{
				var model = ms.ReadValue<StandardStructPropertyModel>();

				Assert.AreEqual(true, model.BoolValue);
				Assert.AreEqual((byte)0xc7, model.ByteValue);
				Assert.AreEqual((sbyte)0x7c, model.SByteValue);
				Assert.AreEqual((short)0x1234, model.Int16Value);
				Assert.AreEqual((ushort)0xfedc, model.UInt16Value);
				Assert.AreEqual(0x76543210, model.Int32Value);
				Assert.AreEqual((uint)0xffbb7733, model.UInt32Value);
				Assert.AreEqual(0xabcdef012345678, model.Int64Value);
				Assert.AreEqual((ulong)0xabcdef0123456789, model.UInt64Value);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.IsTrue(byteArrayValue.SequenceEqual(model.ByteArrayValue));
				Assert.AreEqual(123.456789f, model.SingleValue);
				Assert.AreEqual(123.45678901234567, model.DoubleValue);
				Assert.AreEqual('T', model.CharValue);

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion

		#region NestedClassModel
		private class NestedClassModel
		{
			public StandardClassFieldModel NestedField;
		}

		[TestMethod]
		public void NestedClassModelTest()
		{
			var byteArrayValue = TestDataGenerator<byte>.TestData;
			var floatValueOctets = BitConverter.GetBytes(123.456789f);
			var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x01,
					0x01,
					0xc7,
					0x7c,
					0x34, 0x12,
					0xdc, 0xfe,
					0x10, 0x32, 0x54, 0x76,
					0x33, 0x77, 0xbb, 0xff,
					0x78, 0x56, 0x34, 0x12, 0xf0, 0xde, 0xbc, 0x0a,
					0x89, 0x67, 0x45, 0x23, 0x01, 0xef, 0xcd, 0xab,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x45, 0x23, 0x01, 0x00,
				}.
				Concat(byteArrayValue).
				Concat(floatValueOctets).
				Concat(doubleValueOctets).
				Concat(new[] { (byte)0x01, (byte)'T' }).
				ToArray()))
			{
				var model = ms.ReadValue<NestedClassModel>();

				Assert.AreEqual(true, model.NestedField.BoolValue);
				Assert.AreEqual((byte)0xc7, model.NestedField.ByteValue);
				Assert.AreEqual((sbyte)0x7c, model.NestedField.SByteValue);
				Assert.AreEqual((short)0x1234, model.NestedField.Int16Value);
				Assert.AreEqual((ushort)0xfedc, model.NestedField.UInt16Value);
				Assert.AreEqual(0x76543210, model.NestedField.Int32Value);
				Assert.AreEqual((uint)0xffbb7733, model.NestedField.UInt32Value);
				Assert.AreEqual(0xabcdef012345678, model.NestedField.Int64Value);
				Assert.AreEqual((ulong)0xabcdef0123456789, model.NestedField.UInt64Value);
				Assert.AreEqual("ABCDEFG", model.NestedField.StringValue);
				Assert.IsTrue(byteArrayValue.SequenceEqual(model.NestedField.ByteArrayValue));
				Assert.AreEqual(123.456789f, model.NestedField.SingleValue);
				Assert.AreEqual(123.45678901234567, model.NestedField.DoubleValue);
				Assert.AreEqual('T', model.NestedField.CharValue);

				Assert.AreEqual(ms.Length, ms.Position);
			}
		}
		#endregion
	}
}
