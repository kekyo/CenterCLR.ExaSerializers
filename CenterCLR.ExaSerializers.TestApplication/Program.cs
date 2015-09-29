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
using System.Diagnostics;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace CenterCLR.ExaSerializers
{
	public static class Program
	{
		private static void ExaSerializer<T>(T[] values)
			where T : new()
		{
			using (var ms = new MemoryStream())
			{
				using (var bw = new ExaStreamWriter<T>())
				{
					foreach (var value in values)
					{
						ms.Position = 0;
						bw.Write(ms, value);
					}
				}
			}
		}

		private static void ExaDeserializer<T>(T[] values)
			where T : new()
		{
			using (var ms = new MemoryStream())
			{
				using (var bw = new ExaStreamWriter<T>())
				{
					bw.Write(ms, values[0]);
				}

				using (var br = new ExaStreamReader<T>())
				{
					foreach (var value in values)
					{
						ms.Position = 0;
						var retreive = br.Read(ms);
					}
				}
			}
		}

		private static void BsonSerializer<T>(T[] values)
			where T : new()
		{
			using (var ms = new MemoryStream())
			{
				var js = new JsonSerializer();

				foreach (var value in values)
				{
					ms.Position = 0;
					var bw = new BsonWriter(ms);
					js.Serialize(bw, value);
					bw.Flush();
				}
			}
		}

		private static void BsonDeserializer<T>(T[] values)
			where T : new()
		{
			using (var ms = new MemoryStream())
			{
				var js = new JsonSerializer();

				var bw = new BsonWriter(ms);
				js.Serialize(bw, values[0]);
				bw.Flush();

				foreach (var value in values)
				{
					ms.Position = 0;
					var br = new BsonReader(ms);
					var retreive = js.Deserialize<T>(br);
				}
			}
		}

		private static void ProtoBufSerializer<T>(T[] values)
			where T : new()
		{
			using (var ms = new MemoryStream())
			{
				var ps = ProtoBuf.Serializer.CreateFormatter<T>();

				foreach (var value in values)
				{
					ms.Position = 0;
					ps.Serialize(ms, value);
				}
			}
		}

		private static void ProtoBufDeserializer<T>(T[] values)
			where T : new()
		{
			using (var ms = new MemoryStream())
			{
				var ps = ProtoBuf.Serializer.CreateFormatter<T>();

				ps.Serialize(ms, values[0]);

				foreach (var value in values)
				{
					ms.Position = 0;
					var retreive = ps.Deserialize(ms);
				}
			}
		}

		private static void Execute<T>(string title, T[] values, Action<T[]> action)
		{
			var sw = new Stopwatch();
			sw.Start();

			action(values);

			sw.Stop();
			Console.WriteLine(string.Format(
				"{0},{1},{2},{3},{4:F10}", title, action.Method.Name, sw.Elapsed, values.Length, sw.Elapsed.TotalMilliseconds / values.Length * 1000.0));
		}

		public static void Main(string[] args)
		{
			var count = 1000000;

			var bytes = Enumerable.Range(0, 200).
				Select(index => (byte)index).
				ToArray();

			var fieldValuePOCOs = Enumerable.Range(0, count).
				Select(index => new FieldValuedPOCO
				{
					byteValue = 123,
					intValue = 12345678,
					shortValue = 12345,
					boolValue = true,
					stringValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
					enumByteValue = EnumDefsByte.BBB,
					bytesValue = bytes,
					enumInt16Value = EnumDefsInt16.CCC
				}).
				ToArray();
			var propertyValuePOCOs = Enumerable.Range(0, count).
				Select(index => new PropertyValuedPOCO
				{
					byteValue = 123,
					intValue = 12345678,
					shortValue = 12345,
					boolValue = true,
					stringValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
					enumByteValue = EnumDefsByte.BBB,
					bytesValue = bytes,
					enumInt16Value = EnumDefsInt16.CCC
				}).
				ToArray();

			var fieldValuePOSOs = Enumerable.Range(0, count).
				Select(index => new FieldValuedPOSO
				{
					byteValue = 123,
					intValue = 12345678,
					shortValue = 12345,
					boolValue = true,
					stringValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
					enumByteValue = EnumDefsByte.BBB,
					bytesValue = bytes,
					enumInt16Value = EnumDefsInt16.CCC
				}).
				ToArray();
			var propertyValuePOSOs = Enumerable.Range(0, count).
				Select(index => new PropertyValuedPOSO
				{
					byteValue = 123,
					intValue = 12345678,
					shortValue = 12345,
					boolValue = true,
					stringValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
					enumByteValue = EnumDefsByte.BBB,
					bytesValue = bytes,
					enumInt16Value = EnumDefsInt16.CCC
				}).
				ToArray();

			Execute("POCO,Field", fieldValuePOCOs, ExaSerializer);
			Execute("POCO,Field", fieldValuePOCOs, BsonSerializer);
			Execute("POCO,Field", fieldValuePOCOs, ProtoBufSerializer);
			Execute("POCO,Field", fieldValuePOCOs, ExaDeserializer);
			Execute("POCO,Field", fieldValuePOCOs, BsonDeserializer);
			Execute("POCO,Field", fieldValuePOCOs, ProtoBufDeserializer);
			Execute("POSO,Field", fieldValuePOSOs, ExaSerializer);
			Execute("POSO,Field", fieldValuePOSOs, BsonSerializer);
			Execute("POSO,Field", fieldValuePOSOs, ProtoBufSerializer);
			Execute("POSO,Field", fieldValuePOSOs, ExaDeserializer);
			Execute("POSO,Field", fieldValuePOSOs, BsonDeserializer);
			Execute("POSO,Field", fieldValuePOSOs, ProtoBufDeserializer);
			Execute("POCO,Property", propertyValuePOCOs, ExaSerializer);
			Execute("POCO,Property", propertyValuePOCOs, BsonSerializer);
			Execute("POCO,Property", propertyValuePOCOs, ProtoBufSerializer);
			Execute("POCO,Property", propertyValuePOCOs, ExaDeserializer);
			Execute("POCO,Property", propertyValuePOCOs, BsonDeserializer);
			Execute("POCO,Property", propertyValuePOCOs, ProtoBufDeserializer);
			Execute("POSO,Property", propertyValuePOSOs, ExaSerializer);
			Execute("POSO,Property", propertyValuePOSOs, BsonSerializer);
			Execute("POSO,Property", propertyValuePOSOs, ProtoBufSerializer);
			Execute("POSO,Property", propertyValuePOSOs, ExaDeserializer);
			Execute("POSO,Property", propertyValuePOSOs, BsonDeserializer);
			Execute("POSO,Property", propertyValuePOSOs, ProtoBufDeserializer);
		}
	}
}
