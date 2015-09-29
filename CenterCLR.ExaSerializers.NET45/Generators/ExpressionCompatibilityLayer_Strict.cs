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

#if USE_STRICT_EXPRESSION
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Generators
{
	partial class Expression
	{
		private static readonly MethodInfo truenizer_ =
			typeof(Expression).GetMethod("Truenizer", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static bool Truenizer<TValue>(TValue value)
		{
			return true;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static System.Linq.Expressions.Expression ToTruenize(System.Linq.Expressions.Expression expression)
		{
			Debug.Assert(expression != null);
			Debug.Assert(truenizer_ != null);

			var realTruenizer = truenizer_.MakeGenericMethod(expression.Type);
			return System.Linq.Expressions.Expression.Call(
				realTruenizer,
				expression);
		}
	}

	public sealed class DefaultExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal DefaultExpression(Type type)
			: base(type)
		{
			Debug.Assert(type != null);
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return (this.Type.IsClass || this.Type.IsInterface) ?
				(System.Linq.Expressions.Expression)System.Linq.Expressions.Expression.Constant(null, this.Type) :
				(System.Linq.Expressions.Expression)System.Linq.Expressions.Expression.New(this.Type);
		}
	}

	public sealed class ThrowExpression : UnaryExpression
	{
		private static readonly MethodInfo throwerMethod_ =
			typeof(ThrowExpression).GetMethod("Thrower", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

		static ThrowExpression()
		{
			Debug.Assert(throwerMethod_ != null);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static bool Thrower(Exception ex)
		{
			Debug.Assert(ex != null);

			throw ex;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ThrowExpression(Expression value)
			: base(value, boolType_)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Call(
				throwerMethod_,
				this.Expression.RawExpression);
		}
	}

	public sealed class AssignExpression : BinaryExpression
	{
		private static readonly MethodInfo fieldSetter_ =
			typeof(AssignExpression).GetMethod("FieldSetter", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
		private static readonly MethodInfo byPass_ =
			typeof(AssignExpression).GetMethod("ByPass", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
		private static readonly Type actionType_ = typeof(Action<>);

		static AssignExpression()
		{
			Debug.Assert(fieldSetter_ != null);
			Debug.Assert(byPass_ != null);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static TValue FieldSetter<TValue>(out TValue field, TValue value)
		{
			field = value;
			return value;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static TValue ByPass<TValue>(Action<TValue> action, TValue value)
		{
			Debug.Assert(action != null);

			action(value);
			return value;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal AssignExpression(Expression left, Expression right)
			: base(left.Type, left, right)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			var leftExpression = (System.Linq.Expressions.MemberExpression)this.Left.RawExpression;
			var property = leftExpression.Member as PropertyInfo;
			if (property != null)
			{
				var valueExpression = this.Right.RawExpression;

				var setter = property.GetSetMethod(true);
				var targetExpression = leftExpression.Expression;

				var targetType = valueExpression.Type;
				var realActionType = actionType_.MakeGenericType(targetType);
				var valueParameter = System.Linq.Expressions.Expression.Parameter(targetType, "v");

				var action = System.Linq.Expressions.Expression.Lambda(
					realActionType,
					System.Linq.Expressions.Expression.Call(
						targetExpression,
						setter,
						valueParameter),
					valueParameter);

				var realByPass = byPass_.MakeGenericMethod(targetType);

				return System.Linq.Expressions.Expression.Call(
					realByPass,
					action,
					valueExpression);
			}

			Debug.Assert(leftExpression.Member is FieldInfo);

			var realFieldSetter = fieldSetter_.MakeGenericMethod(leftExpression.Type);

			return System.Linq.Expressions.Expression.Call(
				realFieldSetter,
				this.Left.RawExpression,
				this.Right.RawExpression);
		}
	}

	public sealed class BlockExpression : Expression
	{
		private static readonly MethodInfo byPass_ =
			typeof(BlockExpression).GetMethod("ByPass", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
		private static readonly Type funcType_ = typeof(Func<,>);

		static BlockExpression()
		{
			Debug.Assert(byPass_ != null);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal static TValue ByPass<TValue>(bool combinedResult, TValue value)
		{
			Debug.Assert(combinedResult == true);

			return value;
		}

		private readonly System.Linq.Expressions.ParameterExpression boundVariables_;

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal BlockExpression(VariableExpression[] variables, Expression[] expressions)
			: base(expressions.Last().Type)
		{
			Debug.Assert(variables != null);
			Debug.Assert(expressions != null);

			this.Variables = variables;
			this.Expressions = expressions;

			if (variables.Length == 0)
			{
				return;
			}

			boundVariables_ = System.Linq.Expressions.Expression.Parameter(objectArrayType_, "l");

			for (var index = 0; index < variables.Length; index++)
			{
				var variable = variables[index];

				Debug.Assert(variable.BoundVariables == null);
				Debug.Assert(variable.BoundVariablesIndex == -1);

				variable.BoundVariables = boundVariables_;
				variable.BoundVariablesIndex = index;
			}
		}

		public readonly VariableExpression[] Variables;
		public readonly Expression[] Expressions;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			var lastExpression = this.Expressions.Last().RawExpression;
			if (this.Expressions.Length == 1)
			{
				return lastExpression;
			}

			var realByPass = byPass_.MakeGenericMethod(lastExpression.Type);

			var statementCombined = System.Linq.Expressions.Expression.Call(
				realByPass,
				this.Expressions.Take(this.Expressions.Length - 1).
					Select(expression => ToTruenize(expression.RawExpression)).
					Aggregate(System.Linq.Expressions.Expression.AndAlso),
				lastExpression);

			if (this.Variables.Length == 0)
			{
				return statementCombined;
			}

			Debug.Assert(boundVariables_ != null);

			var partialLambdaDelegateType = funcType_.MakeGenericType(
				boundVariables_.Type,
				statementCombined.Type);

			return System.Linq.Expressions.Expression.Invoke(
				System.Linq.Expressions.Expression.Lambda(
					partialLambdaDelegateType,
					statementCombined,
					boundVariables_),
				System.Linq.Expressions.Expression.NewArrayInit(
					objectType_,
					this.Variables.Select(variable =>
						(System.Linq.Expressions.Expression)System.Linq.Expressions.Expression.New(
							valueHolderType_.MakeGenericType(variable.Type)))));
		}
	}

	public sealed class IfThenElseExpression : ConditionalExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal IfThenElseExpression(Expression test, Expression ifTrue, Expression ifFalse)
			: base(test, ifTrue, ifFalse, boolType_)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Condition(
				this.Test.RawExpression,
				ToTruenize(this.IfTrue.RawExpression),
				(this.IfFalse != null) ? ToTruenize(this.IfFalse.RawExpression) : True().RawExpression);
		}
	}

	public sealed class VariableExpression : ParameterExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal VariableExpression(Type type, string name)
			: base(type, name)
		{
		}

		internal System.Linq.Expressions.ParameterExpression BoundVariables;
		internal int BoundVariablesIndex = -1;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			Debug.Assert(this.BoundVariables != null);
			Debug.Assert(this.BoundVariablesIndex >= 0);

			var realValueHolderType = valueHolderType_.MakeGenericType(this.Type);
			var valueField = realValueHolderType.GetField("Value");

			return System.Linq.Expressions.Expression.Field(
				System.Linq.Expressions.Expression.Convert(
					System.Linq.Expressions.Expression.ArrayIndex(
						this.BoundVariables,
						System.Linq.Expressions.Expression.Constant(this.BoundVariablesIndex)),
					realValueHolderType),
				valueField);
		}
	}
}
#endif
