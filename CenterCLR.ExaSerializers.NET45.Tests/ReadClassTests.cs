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
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable 649		// non initialized field

namespace CenterCLR.ExaSerializers
{
	[TestClass]
	public sealed class ReadClassTest
	{
		#region StandardClassModel
		private class StandardClassModel
		{
			public int IntValue;
			public string StringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G'
				}))
			{
				var model = ms.ReadValue<StandardClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
			}
		}
		#endregion

		#region StandardSealedClassModel
		private sealed class StandardSealedClassModel
		{
			public int IntValue;
			public string StringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardSealedClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G'
				}))
			{
				var model = ms.ReadValue<StandardSealedClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
			}
		}
		#endregion

		#region StandardNonPublicClassModel
		private class StandardNonPublicClassModel
		{
			public int IntValue;
			internal int InternalIntValue;
			public string StringValue
			{
				get;
				set;
			}
			internal string InternalStringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardNonPublicClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G'
				}))
			{
				var model = ms.ReadValue<StandardNonPublicClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual(0, model.InternalIntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.IsNull(model.InternalStringValue);
			}
		}
		#endregion

		#region StandardSerializableClassModel
		[Serializable]
		private class StandardSerializableClassModel
		{
			public int IntValue;
			public string StringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardSerializableClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G'
				}))
			{
				var model = ms.ReadValue<StandardSerializableClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
			}
		}
		#endregion

		#region StandardNonPublicSerializableClassModel
		[Serializable]
		private class StandardNonPublicSerializableClassModel
		{
			public int IntValue;
			internal int InternalIntValue;
			public string StringValue
			{
				get;
				set;
			}
			internal string InternalStringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardNonPublicSerializableClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x10, 0x32, 0x54, 0x76,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x07, 0x00,
					(byte)'G', (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A'
				}))
			{
				var model = ms.ReadValue<StandardNonPublicSerializableClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual(0x76543210, model.InternalIntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.AreEqual("GFEDCBA", model.InternalStringValue);
			}
		}
		#endregion

		#region StandardNonSerializedSerializableClassModel
		[Serializable]
		private class StandardNonSerializedSerializableClassModel
		{
			[NonSerialized]
			public int IntValue;
			internal int InternalIntValue;
			public string StringValue
			{
				get;
				set;
			}
			internal string InternalStringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardNonSerializedSerializableClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x10, 0x32, 0x54, 0x76,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x07, 0x00,
					(byte)'G', (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A'
				}))
			{
				var model = ms.ReadValue<StandardNonSerializedSerializableClassModel>();

				Assert.AreEqual(0, model.IntValue);
				Assert.AreEqual(0x76543210, model.InternalIntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.AreEqual("GFEDCBA", model.InternalStringValue);
			}
		}
		#endregion

		#region StandardDataContractClassModel
		[DataContract]
		private class StandardDataContractClassModel
		{
			[DataMember]
			public int IntValue;
			[DataMember]
			public string StringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardDataContractClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G'
				}))
			{
				var model = ms.ReadValue<StandardDataContractClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
			}
		}
		#endregion

		#region StandardNonPublicDataContractClassModel
		[DataContract]
		private class StandardNonPublicDataContractClassModel
		{
			[DataMember]
			public int IntValue;
			[DataMember]
			internal int InternalIntValue;
			[DataMember]
			public string StringValue
			{
				get;
				set;
			}
			[DataMember]
			internal string InternalStringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardNonPublicDataContractClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x78, 0x56, 0x34, 0x12,
					0x10, 0x32, 0x54, 0x76,
					0x07, 0x00,
					(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
					0x07, 0x00,
					(byte)'G', (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A'
				}))
			{
				var model = ms.ReadValue<StandardNonPublicDataContractClassModel>();

				Assert.AreEqual(0x12345678, model.IntValue);
				Assert.AreEqual(0x76543210, model.InternalIntValue);
				Assert.AreEqual("ABCDEFG", model.StringValue);
				Assert.AreEqual("GFEDCBA", model.InternalStringValue);
			}
		}
		#endregion

		#region StandardNonMemberDataContractClassModel
		[DataContract]
		private class StandardNonMemberDataContractClassModel
		{
			public int IntValue;
			[DataMember]
			internal int InternalIntValue;
			public string StringValue
			{
				get;
				set;
			}
			[DataMember]
			internal string InternalStringValue
			{
				get;
				set;
			}
		}

		[TestMethod]
		public void StandardNonMemberDataContractClassTest()
		{
			using (var ms = new MemoryStream(new byte[]
				{
					0x01,
					0x10, 0x32, 0x54, 0x76,
					0x07, 0x00,
					(byte)'G', (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A'
				}))
			{
				var model = ms.ReadValue<StandardNonMemberDataContractClassModel>();

				Assert.AreEqual(0, model.IntValue);
				Assert.AreEqual(0x76543210, model.InternalIntValue);
				Assert.IsNull(model.StringValue);
				Assert.AreEqual("GFEDCBA", model.InternalStringValue);
			}
		}
		#endregion
	}
}
