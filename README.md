# ExaSerializers
## What is this?
* A lightning fast & lightweight simple binary serializer for .NET
* Focused area:
  * Aiming fast serialization / deserialization.
  * Sensitive binary format / bits layout / endian. (NOT independent binary format)
  * Simple binary format, speed cost effective.
  * Supported multi platform on .NET .
  
## License
* Copyright (c) 2015 Kouji Matsui (@kekyo2)
* Under Apache v2

## Current status
* Still under construction (Performance improving now...)
* Support primitives / string / char / enum / class / struct types.
* Support basic .NET serialization scheme by public / private fields and DataContract'ed properties.
* Support Serializable / NotSerialized / DataContract / DataMember attributes.
* Support string encoder, and special tuned for 1-byte fixed format encoder.
* Support .NET 3.5 / 4.0 / 4.5 / PCL1 / PCL2 / NETFX by Expression-Tree based serializer / deserializer.
* Support .NET 3.5 / 4.0 / 4.5 / NETFX by unsafe pointer-based manipulator.
* Not supported recursive type referenced type (and recursive instance).
* Not supported ISerializable interface.
* Not supported type surrogators.
* Include unit tests.

## TODO
* Support Bulk-copy.
* More improvement copy-bits code using unmanaged code. (.NET 3.5 / 4.0 / 4.5 / NETFX)
* Support recursive type referenced type.
* Support fixed format string by UTF16 encoder.
* NuGet packaging.

## Usage
* Easy usage sample:

``` csharp
using CenterCLR.ExaSerializers;

public class StandardClassFieldModel
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

public void Test()
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
    ByteArrayValue = new byte[] { 0x12, 0x34, 0x56 },
    SingleValue = 123.456789f,
    DoubleValue = 123.45678901234567,
    CharValue = 'T'
  };
  
  using (var ms = new MemoryStream())
  {
    // Serialize
    ms.WriteValue(model);
    
    ms.Position = 0;
  
    // Deserialize
    var model = ms.ReadValue<StandardClassFieldModel>();
  }
}
```

* More samples, see unit tests (WriteTypesTest.cs, ReadTypesTests.cs etc...)

## History
* 0.6.2.0 : Initial public release.
