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

using ProtoBuf;

namespace CenterCLR.ExaSerializers
{
	public enum EnumDefsByte : byte
	{
		AAA = 0,
		BBB = 11,
		CCC = 123
	}

	public enum EnumDefsInt16 : short
	{
		AAA = 0,
		BBB = 12345,
		CCC = 23456
	}

	[ProtoContract]
	public sealed class FieldValuedPOCO
	{
		[ProtoMember(1)]
		public int intValue;
		[ProtoMember(2)]
		public string stringValue;
		[ProtoMember(3)]
		public byte byteValue;
		[ProtoMember(4)]
		public bool boolValue;
		[ProtoMember(5)]
		public byte[] bytesValue;
		[ProtoMember(6)]
		public EnumDefsByte enumByteValue;
		[ProtoMember(7)]
		public short shortValue;
		[ProtoMember(8)]
		public EnumDefsInt16 enumInt16Value;
	}

	[ProtoContract]
	public struct FieldValuedPOSO
	{
		[ProtoMember(1)]
		public int intValue;
		[ProtoMember(2)]
		public string stringValue;
		[ProtoMember(3)]
		public byte byteValue;
		[ProtoMember(4)]
		public bool boolValue;
		[ProtoMember(5)]
		public byte[] bytesValue;
		[ProtoMember(6)]
		public EnumDefsByte enumByteValue;
		[ProtoMember(7)]
		public short shortValue;
		[ProtoMember(8)]
		public EnumDefsInt16 enumInt16Value;
	}

	[ProtoContract]
	public sealed class PropertyValuedPOCO
	{
		[ProtoMember(1)]
		public int intValue { get; set; }
		[ProtoMember(2)]
		public string stringValue { get; set; }
		[ProtoMember(3)]
		public byte byteValue { get; set; }
		[ProtoMember(4)]
		public bool boolValue { get; set; }
		[ProtoMember(5)]
		public byte[] bytesValue { get; set; }
		[ProtoMember(6)]
		public EnumDefsByte enumByteValue { get; set; }
		[ProtoMember(7)]
		public short shortValue { get; set; }
		[ProtoMember(8)]
		public EnumDefsInt16 enumInt16Value { get; set; }
	}

	[ProtoContract]
	public struct PropertyValuedPOSO
	{
		[ProtoMember(1)]
		public int intValue { get; set; }
		[ProtoMember(2)]
		public string stringValue { get; set; }
		[ProtoMember(3)]
		public byte byteValue { get; set; }
		[ProtoMember(4)]
		public bool boolValue { get; set; }
		[ProtoMember(5)]
		public byte[] bytesValue { get; set; }
		[ProtoMember(6)]
		public EnumDefsByte enumByteValue { get; set; }
		[ProtoMember(7)]
		public short shortValue { get; set; }
		[ProtoMember(8)]
		public EnumDefsInt16 enumInt16Value { get; set; }
	}
}
