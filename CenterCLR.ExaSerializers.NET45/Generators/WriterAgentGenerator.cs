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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using CenterCLR.ExaSerializers.Writers;

namespace CenterCLR.ExaSerializers.Generators
{
	internal static class WriterAgentGenerator
	{
		#region Fields
		private static readonly Dictionary<Type, MethodInfo> writeMethods_ =
			typeof(InternalWriterBase).GetInstanceMethods().
			Where(method => method.Name.StartsWith("Write")).
			ToDictionary(method => method.GetParameters()[0].ParameterType, method => method);
		private static readonly MethodInfo writeBooleanMethod_ = writeMethods_[typeof(bool)];
		private static readonly MethodInfo writeStreamingMethod_ =
			typeof(WriterAgentGenerator).GetMethod("WriteStreaming");
		#endregion

		#region Type initializer
#if DEBUG
		static WriterAgentGenerator()
		{
			Debug.Assert(writeMethods_.Count >= 1);
			Debug.Assert(writeBooleanMethod_ != null);
		}
#endif
		#endregion

		#region CreateWriteArrayExpression
		private static Expression CreateWriteArrayExpression(
			ParameterExpression writerParameter,
			Expression valueExpression)
		{
			Debug.Assert(writerParameter != null);
			Debug.Assert(valueExpression != null);

			var arrayType = valueExpression.Type;
			Debug.Assert(arrayType.IsArray());

			// TODO: Non primitive typed array
			throw new NotImplementedException("Not implemented: " + arrayType.FullName);
		}
		#endregion

		#region CreateWriteEnumerableExpression
		private static void WriteStreamingAsCounted<TWriter, TValue>(
			TWriter writer,
			IEnumerable<TValue> enumerable,
			int count)
			where TWriter : InternalWriterBase
		{
			Debug.Assert(writer != null);
			Debug.Assert(enumerable != null);

			var run = WriterAgentGenerator<TWriter, TValue>.Run;

			writer.WriteInt32(count);

			foreach (var value in enumerable)
			{
				run(writer, value);
			}
		}

		public static void WriteStreaming<TWriter, TValue>(
			TWriter writer,
			IEnumerable<TValue> enumerable)
			where TWriter : InternalWriterBase
		{
			Debug.Assert(writer != null);

			if (enumerable == null)
			{
				writer.WriteInt32(-1);
				return;
			}

			var arr = enumerable as TValue[];
			if (arr != null)
			{
				WriterAgentGenerator<TWriter, TValue[]>.Run(writer, arr);
				return;
			}

#if NET45 || NETFX_CORE
			var readonlyCollection = enumerable as IReadOnlyCollection<TValue>;
			if (readonlyCollection != null)
			{
				WriteStreamingAsCounted(writer, enumerable, readonlyCollection.Count);
				return;
			}
#endif
			var collection = enumerable as ICollection<TValue>;
			if (collection != null)
			{
				WriteStreamingAsCounted(writer, enumerable, collection.Count);
				return;
			}

			var collection2 = enumerable as ICollection;
			if (collection2 != null)
			{
				WriteStreamingAsCounted(writer, enumerable, collection2.Count);
				return;
			}

			var run = WriterAgentGenerator<TWriter, TValue>.Run;

			writer.WriteInt32(-2);

			foreach (var value in enumerable)
			{
				writer.WriteBoolean(true);
				run(writer, value);
			}

			writer.WriteBoolean(false);
		}

		private static Expression CreateWriteEnumerableExpression(
			ParameterExpression writerParameter,
			Expression valueExpression,
			Type elementType)
		{
			Debug.Assert(writerParameter != null);
			Debug.Assert(valueExpression != null);

			var realWriteStreamingMethod = writeStreamingMethod_.MakeGenericMethod(
				 writerParameter.Type,
				 elementType);

			return Expression.Call(
				realWriteStreamingMethod,
				writerParameter,
				valueExpression);
		}
		#endregion

		#region CreateWriteStructExpression
		private static Expression CreateWriteStructExpression(
			ParameterExpression writerParameter,
			Expression valueExpression)
		{
			Debug.Assert(writerParameter != null);
			Debug.Assert(valueExpression != null);

			var structType = valueExpression.Type;
			Debug.Assert(structType.IsValueType());

			var fieldWrites =
				ReflectionUtilities.GetTargetFields(structType).
				Select(field => CreateWriteValueExpression(writerParameter, Expression.Field(valueExpression, field)));

			var propertyWrites =
				ReflectionUtilities.GetTargetProperties(structType).
				Select(property => CreateWriteValueExpression(writerParameter, Expression.Property(valueExpression, property)));

			return Expression.Block(
				fieldWrites.
				Concat(propertyWrites));
		}
		#endregion

		#region CreateWriteClassExpression
		private static Expression CreateWriteClassExpression(
			ParameterExpression writerParameter,
			Expression valueExpression)
		{
			Debug.Assert(writerParameter != null);
			Debug.Assert(valueExpression != null);

			var classType = valueExpression.Type;
			Debug.Assert(classType.IsClass());

			var fieldWrites =
				ReflectionUtilities.GetTargetFields(classType).
				Select(field => CreateWriteValueExpression(writerParameter, Expression.Field(valueExpression, field)));

			var propertyWrites =
				ReflectionUtilities.GetTargetProperties(classType).
				Select(property => CreateWriteValueExpression(writerParameter, Expression.Property(valueExpression, property)));

			return Expression.IfThenElse(
				Expression.NotEqual(
					valueExpression,
					Expression.Default(valueExpression.Type)),
				Expression.Block(
					new Expression[] { Expression.Call(writerParameter, writeBooleanMethod_, Expression.True()) }.
					Concat(fieldWrites).
					Concat(propertyWrites)),
				Expression.Call(
					writerParameter,
					writeBooleanMethod_,
					Expression.False()));
		}
		#endregion

		#region CreateWriteValueExpression
		private static Expression CreateWriteValueExpression(
			ParameterExpression writerParameter,
			Expression valueExpression)
		{
			Debug.Assert(writerParameter != null);
			Debug.Assert(valueExpression != null);

			var valueType = valueExpression.Type;
			MethodInfo method;
			if (writeMethods_.TryGetValue(valueType, out method) == true)
			{
				Debug.Assert(method != null);

				return Expression.Call(
					writerParameter,
					method,
					valueExpression);
			}

			if (valueType.IsEnum())
			{
				var underlyingType = Enum.GetUnderlyingType(valueType);

				return Expression.Call(
					writerParameter,
					writeMethods_[underlyingType],
					Expression.Convert(valueExpression, underlyingType));
			}

			if (valueType.IsArray())
			{
				return CreateWriteArrayExpression(
					writerParameter,
					valueExpression);
			}

			var elementType = ReflectionUtilities.GetEnumerableElementType(valueType);
			if (elementType != null)
			{
				return CreateWriteEnumerableExpression(
					writerParameter,
					valueExpression,
					elementType);
			}

			if (valueType.IsClass())
			{
				return CreateWriteClassExpression(
					writerParameter,
					valueExpression);
			}
			else
			{
				return CreateWriteStructExpression(
					writerParameter,
					valueExpression);
			}
		}
		#endregion

		#region CreateWriterAgent
		public static Action<TWriter, TValue> CreateWriterAgent<TWriter, TValue>()
		{
			var writerParameter = Expression.Parameter(typeof(TWriter), "w");
			var instanceParameter = Expression.Parameter(typeof(TValue), "v");

			var agentBodyExpression = WriterAgentGenerator.CreateWriteValueExpression(
				writerParameter,
				instanceParameter);

			var expression = Expression.Lambda<Action<TWriter, TValue>>(
				agentBodyExpression,
				writerParameter,
				instanceParameter);

			return expression.Compile();
		}
		#endregion
	}

	internal static class WriterAgentGenerator<TWriter, TValue>
		where TWriter : InternalWriterBase
	{
		public static readonly Action<TWriter, TValue> Run =
			WriterAgentGenerator.CreateWriterAgent<TWriter, TValue>();
	}
}
