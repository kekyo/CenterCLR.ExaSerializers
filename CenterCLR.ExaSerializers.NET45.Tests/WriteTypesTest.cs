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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable 649		// non initialized field

namespace CenterCLR.ExaSerializers
{
	[TestClass]
	public sealed class WriteTypesTest
	{
		#region PrimitiveTypes
		private void PrimitiveTypeTestMethod<T>(T value, byte[] except)
		{
			using (var ms = new MemoryStream())
			{
				ms.WriteValue(value);
				Assert.IsTrue(ms.ToArray().SequenceEqual(except));
			}
		}

		[TestMethod]
		public void BooleanTest()
		{
			PrimitiveTypeTestMethod(true, new byte[] { 1 });
			PrimitiveTypeTestMethod(false, new byte[] { 0 });
		}

		[TestMethod]
		public void ByteTest()
		{
			PrimitiveTypeTestMethod((byte)0, new byte[] { 0 });
			PrimitiveTypeTestMethod(byte.MaxValue, new byte[] { 0xff });
		}

		[TestMethod]
		public void SByteTest()
		{
			PrimitiveTypeTestMethod((sbyte)0, new byte[] { 0 });
			PrimitiveTypeTestMethod(sbyte.MaxValue, new byte[] { 0x7f });
			PrimitiveTypeTestMethod((sbyte)-1, new byte[] { 0xff });
		}

		[TestMethod]
		public void Int16Test()
		{
			PrimitiveTypeTestMethod((short)0, new byte[] { 0, 0 });
			PrimitiveTypeTestMethod(short.MaxValue, new byte[] { 0xff, 0x7f });
			PrimitiveTypeTestMethod((short)-1, new byte[] { 0xff, 0xff });
		}

		[TestMethod]
		public void UInt16Test()
		{
			PrimitiveTypeTestMethod((ushort)0, new byte[] { 0, 0 });
			PrimitiveTypeTestMethod(ushort.MaxValue, new byte[] { 0xff, 0xff });
		}

		[TestMethod]
		public void Int32Test()
		{
			PrimitiveTypeTestMethod((int)0, new byte[] { 0, 0, 0, 0 });
			PrimitiveTypeTestMethod(int.MaxValue, new byte[] { 0xff, 0xff, 0xff, 0x7f });
			PrimitiveTypeTestMethod((int)-1, new byte[] { 0xff, 0xff, 0xff, 0xff });
		}

		[TestMethod]
		public void UInt32Test()
		{
			PrimitiveTypeTestMethod((uint)0, new byte[] { 0, 0, 0, 0 });
			PrimitiveTypeTestMethod(uint.MaxValue, new byte[] { 0xff, 0xff, 0xff, 0xff });
		}

		[TestMethod]
		public void Int64Test()
		{
			PrimitiveTypeTestMethod((long)0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
			PrimitiveTypeTestMethod(long.MaxValue, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f });
			PrimitiveTypeTestMethod((long)-1, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
		}

		[TestMethod]
		public void UInt64Test()
		{
			PrimitiveTypeTestMethod((ulong)0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
			PrimitiveTypeTestMethod(ulong.MaxValue, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
		}

		[TestMethod]
		public void SingleTest()
		{
			PrimitiveTypeTestMethod((float)0, BitConverter.GetBytes((float)0));
			PrimitiveTypeTestMethod(float.MaxValue, BitConverter.GetBytes(float.MaxValue));
			PrimitiveTypeTestMethod(float.MinValue, BitConverter.GetBytes(float.MinValue));
			PrimitiveTypeTestMethod(float.Epsilon, BitConverter.GetBytes(float.Epsilon));
			PrimitiveTypeTestMethod(float.NaN, BitConverter.GetBytes(float.NaN));
			PrimitiveTypeTestMethod(float.PositiveInfinity, BitConverter.GetBytes(float.PositiveInfinity));
			PrimitiveTypeTestMethod(float.NegativeInfinity, BitConverter.GetBytes(float.NegativeInfinity));
		}

		[TestMethod]
		public void DoubleTest()
		{
			PrimitiveTypeTestMethod((double)0, BitConverter.GetBytes((double)0));
			PrimitiveTypeTestMethod(double.MaxValue, BitConverter.GetBytes(double.MaxValue));
			PrimitiveTypeTestMethod(double.MinValue, BitConverter.GetBytes(double.MinValue));
			PrimitiveTypeTestMethod(double.Epsilon, BitConverter.GetBytes(double.Epsilon));
			PrimitiveTypeTestMethod(double.NaN, BitConverter.GetBytes(double.NaN));
			PrimitiveTypeTestMethod(double.PositiveInfinity, BitConverter.GetBytes(double.PositiveInfinity));
			PrimitiveTypeTestMethod(double.NegativeInfinity, BitConverter.GetBytes(double.NegativeInfinity));
		}

		[TestMethod]
		public void StringTest()
		{
			PrimitiveTypeTestMethod((string)null, new byte[] { 0xff, 0xff });
			PrimitiveTypeTestMethod(string.Empty, new byte[] { 0, 0 });
			PrimitiveTypeTestMethod("ABCDEFG", new byte[] { 0x07, 0x00, (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G' });

			var data = Encoding.UTF8.GetBytes("あいうえお");
			PrimitiveTypeTestMethod("あいうえお", BitConverter.GetBytes((ushort)data.Length).Concat(data).ToArray());
		}

		[TestMethod]
		public void CharTest()
		{
			PrimitiveTypeTestMethod('T', new byte[] { 1, (byte)'T' });

			var data = Encoding.UTF8.GetBytes("あ");
			PrimitiveTypeTestMethod('あ', new byte[] { (byte)data.Length }.Concat(data).ToArray());
		}
		#endregion

		#region CombinedPrimitiveTypesTest
		[TestMethod]
		public void CombinedPrimitiveTypesTest()
		{
			using (var ms = new MemoryStream())
			{
				ms.WriteValue(true);
				ms.WriteValue((byte)0xc7);
				ms.WriteValue((sbyte)0x7c);
				ms.WriteValue((short)0x1234);
				ms.WriteValue((ushort)0xfedc);
				ms.WriteValue((int)0x76543210);
				ms.WriteValue((uint)0xffbb7733);
				ms.WriteValue((long)0xabcdef012345678);
				ms.WriteValue((ulong)0xabcdef0123456789);
				ms.WriteValue("ABCDEFG");
				ms.WriteValue(123.456789f);
				ms.WriteValue(123.45678901234567);
				ms.WriteValue('T');

				var floatValueOctets = BitConverter.GetBytes(123.456789f);
				var doubleValueOctets = BitConverter.GetBytes(123.45678901234567);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
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
					Concat(new[] { (byte)0x01, (byte)'T' })));
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
				using (var ms = new MemoryStream())
				{
					ms.WriteValue(entry.EnumValue);

					Assert.IsTrue(ms.ToArray().SequenceEqual(entry.Data));
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
			using (var ms = new MemoryStream())
			{
				var arrayValue = TestDataGenerator<TValue>.TestData;

				ms.WriteValue(arrayValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}

			using (var ms = new MemoryStream())
			{
				var arrayValue = new TValue[0];

				ms.WriteValue(arrayValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x00, 0x00, 0x00, 0x00,
					}));
			}

			using (var ms = new MemoryStream())
			{
				ms.WriteValue((TValue[])null);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0xff, 0xff, 0xff, 0xff,
					}));
			}
		}

		[TestMethod]
		public void ByteArrayTest()
		{
			ArrayTypesTestMethod<byte>();
		}

		[TestMethod]
		public void SByteArrayTest()
		{
			ArrayTypesTestMethod<sbyte>();
		}

		[TestMethod]
		public void Int16ArrayTest()
		{
			ArrayTypesTestMethod<short>();
		}

		[TestMethod]
		public void UInt16ArrayTest()
		{
			ArrayTypesTestMethod<ushort>();
		}

		[TestMethod]
		public void Int32ArrayTest()
		{
			ArrayTypesTestMethod<int>();
		}

		[TestMethod]
		public void UInt32ArrayTest()
		{
			ArrayTypesTestMethod<uint>();
		}

		[TestMethod]
		public void Int64ArrayTest()
		{
			ArrayTypesTestMethod<long>();
		}

		[TestMethod]
		public void UInt64ArrayTest()
		{
			ArrayTypesTestMethod<ulong>();
		}

		[TestMethod]
		public void SingleArrayTest()
		{
			ArrayTypesTestMethod<float>();
		}

		[TestMethod]
		public void DoubleArrayTest()
		{
			ArrayTypesTestMethod<double>();
		}

		[TestMethod]
		public void BooleanArrayTest()
		{
			ArrayTypesTestMethod<bool>();
		}
		#endregion

		#region CollectionTypes
#if NET45 || NETFX_CORE
		private sealed class TestReadOnlyCollection<T> : IReadOnlyCollection<T>
		{
			private readonly IReadOnlyCollection<T> values_;

			public TestReadOnlyCollection(IReadOnlyCollection<T> values)
			{
				values_ = values;
			}

			public int Count
			{
				get
				{
					return values_.Count;
				}
			}

			public IEnumerator<T> GetEnumerator()
			{
				return values_.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}

		private void ReadOnlyCollectionTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var collectionValue = new TestReadOnlyCollection<TValue>(TestDataGenerator<TValue>.TestData);

				ms.WriteValue(collectionValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}
#endif

		private sealed class TestGenericCollection<T> : ICollection<T>
		{
			private readonly IReadOnlyCollection<T> values_;

			public TestGenericCollection(IReadOnlyCollection<T> values)
			{
				values_ = values;
			}

			public int Count
			{
				get
				{
					return values_.Count;
				}
			}

			public IEnumerator<T> GetEnumerator()
			{
				return values_.GetEnumerator();
			}

			#region Not implemented
			public void Add(T item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(T item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool IsReadOnly
			{
				get { throw new NotImplementedException(); }
			}

			public bool Remove(T item)
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
			#endregion
		}

		private void GenericCollectionTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var collectionValue = new TestGenericCollection<TValue>(TestDataGenerator<TValue>.TestData);

				ms.WriteValue(collectionValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}

		private sealed class TestCollection<T> : ICollection, IEnumerable<T>
		{
			private readonly IReadOnlyCollection<T> values_;

			public TestCollection(IReadOnlyCollection<T> values)
			{
				values_ = values;
			}

			public int Count
			{
				get
				{
					return values_.Count;
				}
			}

			public IEnumerator<T> GetEnumerator()
			{
				return values_.GetEnumerator();
			}

			#region Not implemented
			public void Add(T item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(T item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool IsReadOnly
			{
				get { throw new NotImplementedException(); }
			}

			public bool Remove(T item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(Array array, int index)
			{
				throw new NotImplementedException();
			}

			public bool IsSynchronized
			{
				get { throw new NotImplementedException(); }
			}

			public object SyncRoot
			{
				get { throw new NotImplementedException(); }
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}

		private void CollectionTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var collectionValue = new TestCollection<TValue>(TestDataGenerator<TValue>.TestData);

				ms.WriteValue(collectionValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}

		private void CombinedCollectionTestMethod<TValue>()
		{
#if NET45 || NETFX_CORE
			ReadOnlyCollectionTestMethod<TValue>();
#endif
			GenericCollectionTestMethod<TValue>();
			CollectionTestMethod<TValue>();
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
			using (var ms = new MemoryStream())
			{
				var arrayValue = TestDataGenerator<TValue>.TestData;

				ms.WriteValue(arrayValue.Select(value => value));

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0xfe, 0xff, 0xff, 0xff,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytesWithFlags)));
			}

			using (var ms = new MemoryStream())
			{
				var byteArrayValue = new TValue[0];

				ms.WriteValue(byteArrayValue.Select(value => value));

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0xfe, 0xff, 0xff, 0xff,
						0x00
					}));
			}

			using (var ms = new MemoryStream())
			{
				ms.WriteValue<IEnumerable<TValue>>(null);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0xff, 0xff, 0xff, 0xff,
					}));
			}
		}

		private void EnumerableByArrayTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var arrayValue = TestDataGenerator<TValue>.TestData;

				ms.WriteValue<IEnumerable<TValue>>(arrayValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}

#if NET45 || NETFX_CORE
		private void EnumerableByReadOnlyCollectionTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var byteArrayListValue = new TestReadOnlyCollection<TValue>(TestDataGenerator<TValue>.TestData);

				ms.WriteValue<IEnumerable<TValue>>(byteArrayListValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}
#endif

		private void EnumerableByGenericCollectionTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var byteArrayListValue = new TestGenericCollection<TValue>(TestDataGenerator<TValue>.TestData);

				ms.WriteValue<IEnumerable<TValue>>(byteArrayListValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}

		private void EnumerableByCollectionTestMethod<TValue>()
		{
			using (var ms = new MemoryStream())
			{
				var byteArrayListValue = new TestCollection<TValue>(TestDataGenerator<TValue>.TestData);

				ms.WriteValue<IEnumerable<TValue>>(byteArrayListValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x45, 0x23, 0x01, 0x00,
					}.
					Concat(TestDataGenerator<TValue>.TestDataBytes)));
			}
		}

		private void CombinedEnumerableTestMethod<TValue>()
		{
			EnumerableByArrayTestMethod<TValue>();
#if NET45 || NETFX_CORE
			EnumerableByReadOnlyCollectionTestMethod<TValue>();
#endif
			EnumerableByGenericCollectionTestMethod<TValue>();
			EnumerableByCollectionTestMethod<TValue>();
		}

		[TestMethod]
		public void EnumerableByteTest()
		{
			CombinedEnumerableTestMethod<byte>();
		}

		[TestMethod]
		public void EnumerableSByteTest()
		{
			CombinedEnumerableTestMethod<sbyte>();
		}

		[TestMethod]
		public void EnumerableInt16Test()
		{
			CombinedEnumerableTestMethod<short>();
		}

		[TestMethod]
		public void EnumerableUInt16Test()
		{
			CombinedEnumerableTestMethod<ushort>();
		}

		[TestMethod]
		public void EnumerableInt32Test()
		{
			CombinedEnumerableTestMethod<int>();
		}

		[TestMethod]
		public void EnumerableUInt32Test()
		{
			CombinedEnumerableTestMethod<uint>();
		}

		[TestMethod]
		public void EnumerableInt64Test()
		{
			CombinedEnumerableTestMethod<long>();
		}

		[TestMethod]
		public void EnumerableUInt64Test()
		{
			CombinedEnumerableTestMethod<ulong>();
		}

		[TestMethod]
		public void EnumerableSingleTest()
		{
			CombinedEnumerableTestMethod<float>();
		}

		[TestMethod]
		public void EnumerableDoubleTest()
		{
			CombinedEnumerableTestMethod<double>();
		}

		[TestMethod]
		public void EnumerableBooleanTest()
		{
			CombinedEnumerableTestMethod<bool>();
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
			var model = new StandardClassFieldModel
			{
				BoolValue = true,
				ByteValue = 0xc7,
				SByteValue = 0x7c,
				Int16Value = 0x1234,
				UInt16Value = 0xfedc,
				Int32Value = 0x76543210,
				UInt32Value = 0xffbb7733,
				Int64Value = 0xabcdef012345678,
				UInt64Value = 0xabcdef0123456789,
				StringValue = "ABCDEFG",
				ByteArrayValue = TestDataGenerator<byte>.TestData,
				SingleValue = 123.456789f,
				DoubleValue = 123.45678901234567,
				CharValue = 'T'
			};

			using (var ms = new MemoryStream())
			{
				ms.WriteValue(model);

				var floatValueOctets = BitConverter.GetBytes(model.SingleValue);
				var doubleValueOctets = BitConverter.GetBytes(model.DoubleValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
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
					Concat(model.ByteArrayValue).
					Concat(floatValueOctets).
					Concat(doubleValueOctets).
					Concat(new[] { (byte)0x01, (byte)'T' })));
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
			var model = new StandardClassPropertyModel
			{
				BoolValue = true,
				ByteValue = 0xc7,
				SByteValue = 0x7c,
				Int16Value = 0x1234,
				UInt16Value = 0xfedc,
				Int32Value = 0x76543210,
				UInt32Value = 0xffbb7733,
				Int64Value = 0xabcdef012345678,
				UInt64Value = 0xabcdef0123456789,
				StringValue = "ABCDEFG",
				ByteArrayValue = TestDataGenerator<byte>.TestData,
				SingleValue = 123.456789f,
				DoubleValue = 123.45678901234567,
				CharValue = 'T'
			};

			using (var ms = new MemoryStream())
			{
				ms.WriteValue(model);

				var floatValueOctets = BitConverter.GetBytes(model.SingleValue);
				var doubleValueOctets = BitConverter.GetBytes(model.DoubleValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
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
					Concat(model.ByteArrayValue).
					Concat(floatValueOctets).
					Concat(doubleValueOctets).
					Concat(new[] { (byte)0x01, (byte)'T' })));
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
			var model = new EmptyClassModel
			{
			};

			using (var ms = new MemoryStream())
			{
				ms.WriteValue(model);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x01,
					}));
			}

			using (var ms = new MemoryStream())
			{
				ms.WriteValue((EmptyClassModel)null);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x00
					}));
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
			var model = new StandardStructFieldModel
			{
				BoolValue = true,
				ByteValue = 0xc7,
				SByteValue = 0x7c,
				Int16Value = 0x1234,
				UInt16Value = 0xfedc,
				Int32Value = 0x76543210,
				UInt32Value = 0xffbb7733,
				Int64Value = 0xabcdef012345678,
				UInt64Value = 0xabcdef0123456789,
				StringValue = "ABCDEFG",
				ByteArrayValue = TestDataGenerator<byte>.TestData,
				SingleValue = 123.456789f,
				DoubleValue = 123.45678901234567,
				CharValue = 'T'
			};

			using (var ms = new MemoryStream())
			{
				ms.WriteValue(model);

				var floatValueOctets = BitConverter.GetBytes(model.SingleValue);
				var doubleValueOctets = BitConverter.GetBytes(model.DoubleValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
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
					Concat(model.ByteArrayValue).
					Concat(floatValueOctets).
					Concat(doubleValueOctets).
					Concat(new[] { (byte)0x01, (byte)'T' })));
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
			var model = new StandardStructPropertyModel
			{
				BoolValue = true,
				ByteValue = 0xc7,
				SByteValue = 0x7c,
				Int16Value = 0x1234,
				UInt16Value = 0xfedc,
				Int32Value = 0x76543210,
				UInt32Value = 0xffbb7733,
				Int64Value = 0xabcdef012345678,
				UInt64Value = 0xabcdef0123456789,
				StringValue = "ABCDEFG",
				ByteArrayValue = TestDataGenerator<byte>.TestData,
				SingleValue = 123.456789f,
				DoubleValue = 123.45678901234567,
				CharValue = 'T'
			};

			using (var ms = new MemoryStream())
			{
				ms.WriteValue(model);

				var floatValueOctets = BitConverter.GetBytes(model.SingleValue);
				var doubleValueOctets = BitConverter.GetBytes(model.DoubleValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
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
					Concat(model.ByteArrayValue).
					Concat(floatValueOctets).
					Concat(doubleValueOctets).
					Concat(new[] { (byte)0x01, (byte)'T' })));
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
			var model = new NestedClassModel
			{
				NestedField = new StandardClassFieldModel
				{
					BoolValue = true,
					ByteValue = 0xc7,
					SByteValue = 0x7c,
					Int16Value = 0x1234,
					UInt16Value = 0xfedc,
					Int32Value = 0x76543210,
					UInt32Value = 0xffbb7733,
					Int64Value = 0xabcdef012345678,
					UInt64Value = 0xabcdef0123456789,
					StringValue = "ABCDEFG",
					ByteArrayValue = TestDataGenerator<byte>.TestData,
					SingleValue = 123.456789f,
					DoubleValue = 123.45678901234567,
					CharValue = 'T'
				}
			};

			using (var ms = new MemoryStream())
			{
				ms.WriteValue(model);

				var floatValueOctets = BitConverter.GetBytes(model.NestedField.SingleValue);
				var doubleValueOctets = BitConverter.GetBytes(model.NestedField.DoubleValue);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
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
					Concat(model.NestedField.ByteArrayValue).
					Concat(floatValueOctets).
					Concat(doubleValueOctets).
					Concat(new[] { (byte)0x01, (byte)'T' })));
			}

			using (var ms = new MemoryStream())
			{
				ms.WriteValue((NestedClassModel)null);

				Assert.IsTrue(
					ms.ToArray().SequenceEqual(new byte[]
					{
						0x00
					}));
			}
		}
		#endregion
	}
}
