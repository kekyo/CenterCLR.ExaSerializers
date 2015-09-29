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

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Generators
{
	public abstract partial class Expression
	{
		#region Static fields
		internal static readonly Type boolType_ = typeof(bool);
		internal static readonly Type voidType_ = typeof(void);
		internal static readonly Type objectType_ = typeof(object);
		internal static readonly Type objectArrayType_ = typeof(object[]);
		internal static readonly Type valueHolderType_ = typeof(ValueHolder<>);

		internal static readonly ConstantExpression trueExpression_ = Constant(true);
		internal static readonly ConstantExpression falseExpression_ = Constant(false);

#if USE_STRICT_EXPRESSION
		internal static readonly Expression emptyExpression_ = trueExpression_;
#else
		internal static readonly Expression emptyExpression_ = Default(voidType_);
#endif

		private static readonly VariableExpression[] emptyLocalVariables_ = new VariableExpression[0];

		#endregion

		#region Type initializer
#if DEBUG
		static Expression()
		{
			Debug.Assert(boolType_ != null);
			Debug.Assert(voidType_ != null);
			Debug.Assert(objectType_ != null);
			Debug.Assert(trueExpression_ != null);
			Debug.Assert(falseExpression_ != null);
			Debug.Assert(emptyExpression_ != null);
		}
#endif
		#endregion

		#region Private fields
		private System.Linq.Expressions.Expression rawExpression_;
		#endregion

		#region Constructors
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal Expression(Type type)
		{
			this.Type = type;
		}
		#endregion

		#region Fields
		public readonly Type Type;
		#endregion

		#region Properties
		internal System.Linq.Expressions.Expression RawExpression
		{
#if USE_INLINING
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
			get
			{
				return this.rawExpression_ ?? (this.rawExpression_ = this.CreateExpression());
			}
		}
		#endregion

		#region Methods
		internal abstract System.Linq.Expressions.Expression CreateExpression();

		public override string ToString()
		{
			return this.RawExpression.ToString();
		}
		#endregion

		#region Static methods
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ConstantExpression True()
		{
			return trueExpression_;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ConstantExpression False()
		{
			return falseExpression_;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static Expression Empty()
		{
			return emptyExpression_;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static DefaultExpression Default(Type type)
		{
			return new DefaultExpression(type);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ConstantExpression Constant(object value)
		{
			return new ConstantExpression(value);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ConvertExpression Convert(Expression expression, Type type)
		{
			return new ConvertExpression(expression, type);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ArrayLengthExpression ArrayLength(Expression array)
		{
			return new ArrayLengthExpression(array);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static NewExpression New(Type type)
		{
			return new NewExpression(type);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
		{
			return new NewArrayExpression(type, bounds);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static MemberExpression Field(Expression expression, FieldInfo field)
		{
			return new FieldExpression(expression, field);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static MemberExpression Property(Expression expression, PropertyInfo property)
		{
			return new PropertyExpression(expression, property);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
		{
			return new MethodCallExpression(null, method, arguments);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static MethodCallExpression Call(Expression expression, MethodInfo method, params Expression[] arguments)
		{
			return new MethodCallExpression(expression, method, arguments);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static EqualExpression Equal(Expression left, Expression right)
		{
			return new EqualExpression(left, right);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static NotEqualExpression NotEqual(Expression left, Expression right)
		{
			return new NotEqualExpression(left, right);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static GreaterThanOrEqualExpression GreaterThanOrEqual(Expression left, Expression right)
		{
			return new GreaterThanOrEqualExpression(left, right);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static AssignExpression Assign(Expression left, Expression right)
		{
			return new AssignExpression(left, right);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static BlockExpression Block(IEnumerable<Expression> expressions)
		{
			return new BlockExpression(emptyLocalVariables_, expressions.ToArray());
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static BlockExpression Block(IEnumerable<VariableExpression> variables, params Expression[] expressions)
		{
			return new BlockExpression(variables.ToArray(), expressions);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
		{
			return new ConditionalExpression(test, ifTrue, ifFalse, ifTrue.Type);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IfThenElseExpression IfThen(Expression test, Expression ifTrue)
		{
			return new IfThenElseExpression(test, ifTrue, null);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IfThenElseExpression IfThenElse(Expression test, Expression ifTrue, Expression ifFalse)
		{
			return new IfThenElseExpression(test, ifTrue, ifFalse);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ParameterExpression Parameter(Type type, string name)
		{
			return new ParameterExpression(type, name);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static VariableExpression Variable(Type type, string name)
		{
			return new VariableExpression(type, name);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static LambdaExpression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
		{
			return new LambdaExpression<TDelegate>(body, parameters);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static LambdaExpression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
		{
			return new LambdaExpression<TDelegate>(body, parameters.ToArray());
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static ThrowExpression Throw(Expression value)
		{
			return new ThrowExpression(value);
		}
		#endregion
	}

	public sealed class ConstantExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ConstantExpression(object value)
			: base((value != null) ? value.GetType() : null)
		{
			this.Value = value;
		}

		public readonly object Value;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Constant(
				this.Value,
				this.Type);
		}
	}

	public abstract class UnaryExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal UnaryExpression(Expression expression, Type type)
			: base(type)
		{
			Debug.Assert(expression != null);
			Debug.Assert(type != null);

			this.Expression = expression;
		}

		public readonly Expression Expression;
	}

	public sealed class ConvertExpression : UnaryExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ConvertExpression(Expression expression, Type type)
			: base(expression, type)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Convert(
				this.Expression.RawExpression,
				this.Type);
		}
	}

	public sealed class ArrayLengthExpression : UnaryExpression
	{
		private static readonly Type intType_ = typeof(int);

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ArrayLengthExpression(Expression array)
			: base(array, intType_)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.ArrayLength(
				this.Expression.RawExpression);
		}
	}

	public sealed class NewExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal NewExpression(Type type)
			: base(type)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.New(
				this.Type);
		}
	}

	public sealed class NewArrayExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal NewArrayExpression(Type type, Expression[] bounds)
			: base(type)
		{
			Debug.Assert(type != null);
			Debug.Assert(bounds != null);

			this.Bounds = bounds;
		}

		public readonly Expression[] Bounds;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.NewArrayBounds(
				this.Type,
				this.Bounds.Select(bound => bound.RawExpression));
		}
	}

	public abstract class BinaryExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal BinaryExpression(Type type, Expression left, Expression right)
			: base(type)
		{
			Debug.Assert(left != null);
			Debug.Assert(right != null);

			this.Left = left;
			this.Right = right;
		}

		public readonly Expression Left;
		public readonly Expression Right;
	}

	public sealed class EqualExpression : BinaryExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal EqualExpression(Expression left, Expression right)
			: base(boolType_, left, right)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Equal(
				this.Left.RawExpression,
				this.Right.RawExpression);
		}
	}

	public sealed class NotEqualExpression : BinaryExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal NotEqualExpression(Expression left, Expression right)
			: base(boolType_, left, right)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.NotEqual(
				this.Left.RawExpression,
				this.Right.RawExpression);
		}
	}

	public sealed class GreaterThanOrEqualExpression : BinaryExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal GreaterThanOrEqualExpression(Expression left, Expression right)
			: base(boolType_, left, right)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.GreaterThanOrEqual(
				this.Left.RawExpression,
				this.Right.RawExpression);
		}
	}

	public abstract class MemberExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal MemberExpression(Expression expression, MemberInfo member, Type type)
			: base(type)
		{
			Debug.Assert(member != null);
			Debug.Assert(type != null);

			this.Expression = expression;
			this.Member = member;
		}

		public readonly Expression Expression;
		public readonly MemberInfo Member;
	}

	public sealed class FieldExpression : MemberExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal FieldExpression(Expression expression, FieldInfo field)
			: base(expression, field, field.FieldType)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Field(
				this.Expression.RawExpression,
				(FieldInfo)this.Member);
		}
	}

	public sealed class PropertyExpression : MemberExpression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal PropertyExpression(Expression expression, PropertyInfo property)
			: base(expression, property, property.PropertyType)
		{
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Property(
				this.Expression.RawExpression,
				(PropertyInfo)this.Member);
		}
	}

	public sealed class MethodCallExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal MethodCallExpression(Expression expression, MethodInfo method, Expression[] arguments)
			: base(method.ReturnType)
		{
			Debug.Assert(method != null);
			Debug.Assert(arguments != null);

			this.Object = expression;
			this.Method = method;
			this.Arguments = arguments;
		}

		public readonly Expression Object;
		public readonly MethodInfo Method;
		public readonly Expression[] Arguments;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Call(
				(this.Object != null) ? this.Object.RawExpression : null,
				this.Method,
				this.Arguments.Select(parameter => parameter.RawExpression));
		}
	}

	public class ConditionalExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse, Type type)
			: base(type)
		{
			Debug.Assert(type != null);
			Debug.Assert(test != null);
			Debug.Assert(ifTrue != null);

			this.Test = test;
			this.IfTrue = ifTrue;
			this.IfFalse = ifFalse;
		}

		public readonly Expression Test;
		public readonly Expression IfTrue;
		public readonly Expression IfFalse;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			Debug.Assert(this.IfFalse != null);

			return System.Linq.Expressions.Expression.Condition(
				this.Test.RawExpression,
				this.IfTrue.RawExpression,
				this.IfFalse.RawExpression);
		}
	}

	public class ParameterExpression : Expression
	{
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal ParameterExpression(Type type, string name)
			: base(type)
		{
			Debug.Assert(string.IsNullOrEmpty(name) == false);
			Debug.Assert(type != null);

			this.Name = name;
		}

		public readonly string Name;

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Parameter(
				this.Type,
				this.Name);
		}
	}

	public sealed class LambdaExpression<TDelegate> : Expression
	{
		private static readonly Type type_ = typeof(TDelegate);

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		internal LambdaExpression(Expression body, params ParameterExpression[] parameters)
			: base(type_)
		{
			Debug.Assert(body != null);

			this.Body = body;
			this.Parameters = parameters;
		}

		public readonly Expression Body;
		public readonly ParameterExpression[] Parameters;

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public TDelegate Compile()
		{
			return ((System.Linq.Expressions.Expression<TDelegate>)this.RawExpression).Compile();
		}

		internal override System.Linq.Expressions.Expression CreateExpression()
		{
			return System.Linq.Expressions.Expression.Lambda<TDelegate>(
				this.Body.RawExpression,
				this.Parameters.Select(parameter => (System.Linq.Expressions.ParameterExpression)parameter.RawExpression));
		}
	}
}
