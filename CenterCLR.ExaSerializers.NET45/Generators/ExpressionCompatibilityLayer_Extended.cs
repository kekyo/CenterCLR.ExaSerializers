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

#if !USE_STRICT_EXPRESSION
using System;
using System.Diagnostics;
using System.Linq;

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Generators
{
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
			return System.Linq.Expressions.Expression.Default(this.Type);
		}
	}

	public sealed class ThrowExpression : UnaryExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ThrowExpression(Expression value)
			: base(value, voidType_)
		{
			Debug.Assert(value != null);
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Throw(
				this.Expression.RawExpression);
		}
	}

	public sealed class AssignExpression : BinaryExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal AssignExpression(Expression left, Expression right)
			: base(left.Type, left, right)
		{
			Debug.Assert(left != null);
			Debug.Assert(right != null);
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Assign(
				this.Left.RawExpression,
				this.Right.RawExpression);
		}
	}

	public sealed class BlockExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal BlockExpression(VariableExpression[] variables, Expression[] expressions)
			: base(expressions.Last().Type)
		{
			this.Variables = variables;
			this.Expressions = expressions;
		}

		public readonly VariableExpression[] Variables;
		public readonly Expression[] Expressions;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			if (this.Variables.Length >= 1)
			{
				return System.Linq.Expressions.Expression.Block(
					this.Variables.Select(variable => (System.Linq.Expressions.ParameterExpression)variable.RawExpression),
					this.Expressions.Select(expression => expression.RawExpression));
			}

			return (this.Expressions.Length >= 2) ?
				System.Linq.Expressions.Expression.Block(
					this.Expressions.Select(expression => expression.RawExpression)) :
				this.Expressions[0].RawExpression;
		}
	}

	public sealed class IfThenElseExpression : ConditionalExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal IfThenElseExpression(Expression test, Expression ifTrue, Expression ifFalse)
			: base(test, ifTrue, ifFalse, voidType_)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return (this.IfFalse != null) ?
				System.Linq.Expressions.Expression.IfThenElse(
					this.Test.RawExpression,
					this.IfTrue.RawExpression,
					this.IfFalse.RawExpression) :
				System.Linq.Expressions.Expression.IfThen(
					this.Test.RawExpression,
					this.IfTrue.RawExpression);
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

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Variable(
				this.Type,
				this.Name);
		}
	}
}
#endif
