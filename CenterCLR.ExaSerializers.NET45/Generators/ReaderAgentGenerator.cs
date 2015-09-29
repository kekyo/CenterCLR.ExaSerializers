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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using CenterCLR.ExaSerializers.Readers;

namespace CenterCLR.ExaSerializers.Generators
{
	internal static class ReaderAgentGenerator
	{
		#region Fields
		private static readonly Dictionary<Type, MethodInfo> readMethods_ =
			typeof(InternalReaderBase).GetInstanceMethods().
			Where(method => method.Name.StartsWith("Read") && (method.ReturnType.IsArray() == false)).
			ToDictionary(method => method.ReturnType, method => method);
		private static readonly Dictionary<Type, MethodInfo> readArrayMethods_ =
			typeof(InternalReaderBase).GetInstanceMethods().
			Where(method => method.Name.StartsWith("Read") && method.ReturnType.IsArray()).
			ToDictionary(method => method.ReturnType, method => method);
		private static readonly MethodInfo readBooleanMethod_ = readMethods_[typeof(bool)];
		private static readonly MethodInfo readInt32Method_ = readMethods_[typeof(int)];
		private static readonly MethodInfo createAndLoadListMethod_ =
			typeof(ReaderAgentGenerator).GetMethod("CreateAndLoadList");
		private static readonly MethodInfo createAndLoadArrayMethod_ =
			typeof(ReaderAgentGenerator).GetMethod("CreateAndLoadArray");
		private static readonly MethodInfo internalStreamingEnumerableMethod_ =
			typeof(ReaderAgentGenerator).GetMethod("InternalStreamingEnumerable");
		private static readonly MethodInfo internalStreamingEnumerableAsCountedMethod_ =
			typeof(ReaderAgentGenerator).GetMethod("InternalStreamingEnumerableAsCounted");
		private static readonly Type listType_ = typeof(List<>);
		private static readonly Type intType_ = typeof(int);
		private static readonly Type argumentOutOfRangeExceptionType_ = typeof(ArgumentOutOfRangeException);
		private static readonly ConstantExpression zeroConstant_ = Expression.Constant(0);
		private static readonly ConstantExpression minusOneConstant_ = Expression.Constant(-1);
		private static readonly ConstantExpression minusTwoConstant_ = Expression.Constant(-2);
		#endregion

		#region Type initializer
#if DEBUG
		static ReaderAgentGenerator()
		{
			Debug.Assert(readMethods_.Count >= 1);
			Debug.Assert(readBooleanMethod_ != null);
			Debug.Assert(readInt32Method_ != null);
			Debug.Assert(createAndLoadListMethod_ != null);
			Debug.Assert(createAndLoadArrayMethod_ != null);
			Debug.Assert(internalStreamingEnumerableMethod_ != null);
		}
#endif
		#endregion

		#region CreateReadArrayExpression
		public static TValue[] CreateAndLoadArray<TReader, TValue>(TReader reader, int length)
			where TReader : InternalReaderBase
		{
			Debug.Assert(reader != null);
			Debug.Assert((length != -1) && (length != 0));

			var run = InternalReaderAgentGenerator<TReader, TValue>.Run;
			var valueHolder = new ValueHolder<TValue>();

			if (length == -2)
			{
				var list = new List<TValue>();

				while (reader.ReadBoolean())
				{
					valueHolder.Value = default(TValue);
					run(reader, valueHolder);
					list.Add(valueHolder.Value);
				}

				return list.ToArray();
			}

			if (length >= 1)
			{
				var list = new TValue[length];

				for (var index = 0; index < length; index++)
				{
					valueHolder.Value = default(TValue);
					run(reader, valueHolder);
					list[index] = valueHolder.Value;
				}

				return list;
			}

			throw new ArgumentOutOfRangeException();
		}

		private static Expression CreateReadArrayExpression(
			ParameterExpression readerParameter,
			MemberExpression assignTargetExpression,
			Type elementType)
		{
			Debug.Assert(readerParameter != null);
			Debug.Assert(assignTargetExpression != null);
			Debug.Assert(elementType != null);

			var lengthVariable = Expression.Variable(intType_, "l");
			MethodInfo readArrayMethod;
			readArrayMethods_.TryGetValue(assignTargetExpression.Type, out readArrayMethod);

			var realCreateAndLoadArrayMethod = createAndLoadArrayMethod_.MakeGenericMethod(
				readerParameter.Type,
				elementType);

			return Expression.Block(
				new[] { lengthVariable },
				Expression.Assign(
					lengthVariable,
					Expression.Call(
						readerParameter,
						readInt32Method_)),
				Expression.IfThenElse(
					Expression.Equal(
						lengthVariable,
						minusOneConstant_),
					Expression.Assign(
						assignTargetExpression,
						Expression.Default(assignTargetExpression.Type)),
					Expression.IfThenElse(
						Expression.Equal(
							lengthVariable,
							zeroConstant_),
						Expression.Assign(
							assignTargetExpression,
							Expression.NewArrayBounds(
								elementType,
								lengthVariable)),
						Expression.Assign(
							assignTargetExpression,
							(readArrayMethod != null) ?
								Expression.Call(
									readerParameter,
									readArrayMethod,
									lengthVariable) :
								Expression.Call(
									realCreateAndLoadArrayMethod,
									readerParameter,
									lengthVariable)))));
		}
		#endregion

		#region CreateReadListExpression
		public static TList CreateAndLoadList<TReader, TList, TValue>(TReader reader, int length)
			where TReader : InternalReaderBase
			where TList : class, IList<TValue>, new()
		{
			Debug.Assert(reader != null);
			Debug.Assert((length != -1) && (length != 0));

			var run = InternalReaderAgentGenerator<TReader, TValue>.Run;
			var valueHolder = new ValueHolder<TValue>();

			if (length == -2)
			{
				var list = new TList();

				while (reader.ReadBoolean())
				{
					valueHolder.Value = default(TValue);
					run(reader, valueHolder);
					list.Add(valueHolder.Value);
				}

				return list;
			}

			if (length >= 1)
			{
				var list = new TList();

				for (var index = 0; index < length; index++)
				{
					valueHolder.Value = default(TValue);
					run(reader, valueHolder);
					list.Add(valueHolder.Value);
				}

				return list;
			}

			throw new ArgumentOutOfRangeException();
		}

		private static Expression CreateReadListExpression(
			ParameterExpression readerParameter,
			MemberExpression assignTargetExpression,
			Type elementType)
		{
			Debug.Assert(readerParameter != null);
			Debug.Assert(assignTargetExpression != null);
			Debug.Assert(elementType != null);

			var lengthVariable = Expression.Variable(intType_, "l");
			var realListType = assignTargetExpression.Type.IsClass() ? assignTargetExpression.Type : listType_.MakeGenericType(elementType);
			MethodInfo readArrayMethod;
			readArrayMethods_.TryGetValue(assignTargetExpression.Type, out readArrayMethod);

			// TODO: use array reader into List<T>

			var realCreateAndLoadListMethod = createAndLoadListMethod_.MakeGenericMethod(
				readerParameter.Type,
				realListType,
				elementType);

			return Expression.Block(
				new[] { lengthVariable },
				Expression.Assign(
					lengthVariable,
					Expression.Call(
						readerParameter,
						readInt32Method_)),
				Expression.IfThenElse(
					Expression.Equal(
						lengthVariable,
						minusOneConstant_),
					Expression.Assign(
						assignTargetExpression,
						Expression.Default(assignTargetExpression.Type)),
					Expression.IfThenElse(
						Expression.Equal(
							lengthVariable,
							zeroConstant_),
						Expression.Assign(
							assignTargetExpression,
							Expression.New(realListType)),
						Expression.Assign(
							assignTargetExpression,
							(readArrayMethod != null) ?
								Expression.Call(
									readerParameter,
									readArrayMethod,
									lengthVariable) :
								Expression.Call(
									realCreateAndLoadListMethod,
									readerParameter,
									lengthVariable)))));
		}
		#endregion

		#region CreateReadStreamingExpression
		public static IEnumerable<TValue> InternalStreamingEnumerable<TReader, TValue>(
			TReader reader)
			where TReader : InternalReaderBase
		{
			Debug.Assert(reader != null);

			var run = InternalReaderAgentGenerator<TReader, TValue>.Run;
			var valueHolder = new ValueHolder<TValue>();

			while (reader.ReadBoolean())
			{
				valueHolder.Value = default(TValue);
				run(reader, valueHolder);
				yield return valueHolder.Value;
			}
		}

		public static IEnumerable<TValue> InternalStreamingEnumerableAsCounted<TReader, TValue>(
			TReader reader,
			int length)
			where TReader : InternalReaderBase
		{
			Debug.Assert(reader != null);

			var run = InternalReaderAgentGenerator<TReader, TValue>.Run;
			var valueHolder = new ValueHolder<TValue>();

			for (var index = 0; index < length; index++)
			{
				valueHolder.Value = default(TValue);
				run(reader, valueHolder);
				yield return valueHolder.Value;
			}
		}

		private static Expression CreateReadStreamingExpression(
			ParameterExpression readerParameter,
			MemberExpression assignTargetExpression,
			Type elementType)
		{
			Debug.Assert(readerParameter != null);
			Debug.Assert(assignTargetExpression != null);
			Debug.Assert(elementType != null);

			var lengthVariable = Expression.Variable(intType_, "l");

			var realInternalStreamingEnumerableMethod = internalStreamingEnumerableMethod_.MakeGenericMethod(
				readerParameter.Type,
				elementType);
			var realInternalStreamingEnumerableAsCountedMethod = internalStreamingEnumerableAsCountedMethod_.MakeGenericMethod(
				readerParameter.Type,
				elementType);

			return Expression.Block(
				new[] { lengthVariable },
				Expression.Assign(
					lengthVariable,
					Expression.Call(
						readerParameter,
						readInt32Method_)),
				Expression.IfThenElse(
					Expression.Equal(
						lengthVariable,
						minusOneConstant_),
					Expression.Assign(
						assignTargetExpression,
						Expression.Default(assignTargetExpression.Type)),
					Expression.IfThenElse(
						Expression.Equal(
							lengthVariable,
							minusTwoConstant_),
						Expression.Assign(
							assignTargetExpression,
							Expression.Call(
								realInternalStreamingEnumerableMethod,
								readerParameter)),
						Expression.IfThenElse(
							Expression.GreaterThanOrEqual(
								lengthVariable,
								zeroConstant_),
							Expression.Assign(
								assignTargetExpression,
								Expression.Call(
									realInternalStreamingEnumerableAsCountedMethod,
									readerParameter,
									lengthVariable)),
							Expression.Throw(
								Expression.New(argumentOutOfRangeExceptionType_))))));
		}
		#endregion

		#region CreateReadStructExpression
		private static Expression CreateReadStructExpression(
			ParameterExpression readerParameter,
			MemberExpression assignTargetExpression)
		{
			Debug.Assert(readerParameter != null);
			Debug.Assert(assignTargetExpression != null);

			var structType = assignTargetExpression.Type;
			Debug.Assert(structType.IsValueType());

			var fieldReads =
				ReflectionUtilities.GetTargetFields(structType).
				Select(field => CreateReadValueExpression(
					readerParameter,
					Expression.Field(assignTargetExpression, field),
					false));

			var propertyReads =
				ReflectionUtilities.GetTargetProperties(structType).
				Select(property => CreateReadValueExpression(
					readerParameter,
					Expression.Property(assignTargetExpression, property),
					false));

			var reads = fieldReads.
				Concat(propertyReads).
				ToList();

			if (reads.Count >= 1)
			{
				return Expression.Block(reads);
			}
			else
			{
				return Expression.Empty();
			}
		}
		#endregion

		#region CreateReadClassExpression
		private static Expression CreateReadClassExpression(
			ParameterExpression readerParameter,
			MemberExpression assignTargetExpression)
		{
			Debug.Assert(readerParameter != null);
			Debug.Assert(assignTargetExpression != null);

			var classType = assignTargetExpression.Type;
			Debug.Assert(classType.IsClass());

			var fieldReads =
				ReflectionUtilities.GetTargetFields(classType).
				Select(field => CreateReadValueExpression(
					readerParameter,
					Expression.Field(assignTargetExpression, field),
					false));

			var propertyReads =
				ReflectionUtilities.GetTargetProperties(classType).
				Select(property => CreateReadValueExpression(
					readerParameter,
					Expression.Property(assignTargetExpression, property),
					false));

			var reads =
				new[] { Expression.Assign(assignTargetExpression, Expression.New(classType)) }.
				Concat(fieldReads).
				Concat(propertyReads).
				ToList();

			var readBoolean = Expression.Call(readerParameter, readBooleanMethod_);

			if (reads.Count == 1)
			{
				return Expression.IfThen(
					readBoolean,
					reads[0]);
			}
			else
			{
				return Expression.IfThen(
					readBoolean,
					Expression.Block(reads));
			}
		}
		#endregion

		#region CreateReadValueExpression
		private static Expression CreateReadValueExpression(
			ParameterExpression readerParameter,
			MemberExpression assignTargetExpression,
			bool isRoot)
		{
			Debug.Assert(readerParameter != null);
			Debug.Assert(assignTargetExpression != null);

			var valueType = assignTargetExpression.Type;
			MethodInfo method;
			if (readMethods_.TryGetValue(valueType, out method) == true)
			{
				Debug.Assert(method != null);

				return Expression.Assign(
					assignTargetExpression,
					Expression.Call(readerParameter, method));
			}

			if (valueType.IsEnum())
			{
				var underlyingType = Enum.GetUnderlyingType(valueType);

				return Expression.Assign(
					assignTargetExpression,
					Expression.Convert(
						Expression.Call(readerParameter, readMethods_[underlyingType]),
						valueType));
			}

			if (valueType.IsArray())
			{
				var arrayElementType = valueType.GetElementType();

				return CreateReadArrayExpression(
					readerParameter,
					assignTargetExpression,
					arrayElementType);
			}

			var elementType = ReflectionUtilities.GetListElementType(valueType);
			if (elementType != null)
			{
				return CreateReadListExpression(
					readerParameter,
					assignTargetExpression,
					elementType);
			}

#if NET45 || NETFX_CORE
			elementType = ReflectionUtilities.GetReadOnlyCollectionElementType(valueType);
			if (elementType != null)
			{
				return CreateReadArrayExpression(
					readerParameter,
					assignTargetExpression,
					elementType);
			}
#endif

			elementType = ReflectionUtilities.GetEnumerableElementType(valueType);
			if (elementType != null)
			{
				return isRoot ?
					CreateReadStreamingExpression(
						readerParameter,
						assignTargetExpression,
						elementType) :
					CreateReadArrayExpression(
						readerParameter,
						assignTargetExpression,
						elementType);
			}

			if (valueType.IsClass())
			{
				return CreateReadClassExpression(
					readerParameter,
					assignTargetExpression);
			}
			else
			{
				return CreateReadStructExpression(
					readerParameter,
					assignTargetExpression);
			}
		}
		#endregion

		#region CreateReaderAgent
		public static Action<TReader, ValueHolder<TValue>> CreateReaderAgent<TReader, TValue>(bool isRoot)
		{
			var readerParameter = Expression.Parameter(typeof(TReader), "r");
			var valueHolderParameter = Expression.Parameter(typeof(ValueHolder<TValue>), "h");
			var valueFieldExpression = Expression.Field(valueHolderParameter, ValueHolder<TValue>.ValueField);

			var agentBodyExpression = CreateReadValueExpression(
				readerParameter,
				valueFieldExpression,
				isRoot);

			var expression = Expression.Lambda<Action<TReader, ValueHolder<TValue>>>(
				agentBodyExpression,
				readerParameter,
				valueHolderParameter);

			return expression.Compile();
		}
		#endregion

		#region InternalReaderAgentGenerator
		private static class InternalReaderAgentGenerator<TReader, TValue>
			where TReader : InternalReaderBase
		{
			public static readonly Action<TReader, ValueHolder<TValue>> Run =
				CreateReaderAgent<TReader, TValue>(false);
		}
		#endregion
	}

	internal static class ReaderAgentGenerator<TReader, TValue>
		where TReader : InternalReaderBase
	{
		public static readonly Action<TReader, ValueHolder<TValue>> Run =
			ReaderAgentGenerator.CreateReaderAgent<TReader, TValue>(true);
	}

	internal sealed class ValueHolder<TValue>
	{
		public static readonly FieldInfo ValueField =
			typeof(ValueHolder<TValue>).GetField("Value");
		public static readonly TValue[] Empty =
			new TValue[0];

		public ValueHolder()
		{
		}

#pragma warning disable 649	// Field value not assigned

		public TValue Value;
	}
}
