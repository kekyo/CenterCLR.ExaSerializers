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
using System.Runtime.Serialization;

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Generators
{
	internal static class ReflectionCompatibilityLayer
	{
		private static readonly Type stringType_ = typeof(string);
		private static readonly Type booleanType_ = typeof(bool);
		private static readonly Type charType_ = typeof(char);
		private static readonly Type dataContractAttributeType_ = typeof(DataContractAttribute);
		private static readonly Type dataMemberAttributeType_ = typeof(DataMemberAttribute);
#if NET35 || NET4 || NET45
		private static readonly Type nonSerializedAttributeType_ = typeof(NonSerializedAttribute);
#endif

#if DEBUG
		static ReflectionCompatibilityLayer()
		{
			Debug.Assert(stringType_ != null);
			Debug.Assert(booleanType_ != null);
			Debug.Assert(charType_ != null);
			Debug.Assert(dataContractAttributeType_ != null);
			Debug.Assert(dataMemberAttributeType_ != null);
#if NET35 || NET4 || NET45
			Debug.Assert(nonSerializedAttributeType_ != null);
#endif
		}
#endif

#if NETFX_CORE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPrimitive(this Type type)
		{
			return type.GetTypeInfo().IsPrimitive;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsString(this Type type)
		{
			return type == stringType_;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsArray(this Type type)
		{
			return type.GetTypeInfo().IsArray;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsClass(this Type type)
		{
			return type.GetTypeInfo().IsClass;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInterface(this Type type)
		{
			return type.GetTypeInfo().IsInterface;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValueType(this Type type)
		{
			return type.GetTypeInfo().IsValueType;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullable(this Type type)
		{
			return type.GetTypeInfo().IsClass || (Nullable.GetUnderlyingType(type) != null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGenericType(this Type type)
		{
			return type.GetTypeInfo().IsGenericType;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.GetTypeInfo().IsGenericTypeDefinition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEnum(this Type type)
		{
			return type.GetTypeInfo().IsEnum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSerializable(this Type type)
		{
			return type.GetTypeInfo().IsSerializable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDataContract(this Type type)
		{
			return type.GetTypeInfo().IsDefined(dataContractAttributeType_, false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNonSerialized(this FieldInfo field)
		{
			return field.GetCustomAttributes(false).Any(attribute => attribute.GetType().FullName == "System.NonSerializedAttribute");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAssignableFrom(this Type type, Type fromType)
		{
			return type.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetBaseType(this Type type)
		{
			return type.GetTypeInfo().BaseType;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<Type> GetInterfaces(this Type type)
		{
			return type.GetTypeInfo().ImplementedInterfaces;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetGenericArguments(this Type type)
		{
			var typeInfo = type.GetTypeInfo();
			return typeInfo.IsGenericTypeDefinition ?
				typeInfo.GenericTypeParameters :
				typeInfo.GenericTypeArguments;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<FieldInfo> GetInstanceFields(this Type type)
		{
			return type.GetTypeInfo().DeclaredFields.
				Where(field => field.IsStatic == false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsInstanceMethodOrNull(MethodInfo method)
		{
			return (method == null) || (method.IsStatic == false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<PropertyInfo> GetInstanceProperties(this Type type)
		{
			return type.GetTypeInfo().DeclaredProperties.
				Where(property => IsInstanceMethodOrNull(property.GetMethod) && IsInstanceMethodOrNull(property.SetMethod));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<MethodInfo> GetInstanceMethods(this Type type)
		{
			return type.GetTypeInfo().DeclaredMethods.
				Where(method => method.IsStatic == false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo GetField(this Type type, string fieldName)
		{
			return type.GetTypeInfo().DeclaredFields.
				FirstOrDefault(field => field.Name == fieldName);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetMethod(this Type type, string methodName)
		{
			return type.GetTypeInfo().DeclaredMethods.
				FirstOrDefault(method => method.Name == methodName);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetGetMethod(this PropertyInfo property, bool nonPublic)
		{
			var method = property.GetMethod;
			if (method == null)
			{
				return null;
			}

			return (method.IsPublic || nonPublic) ? method : null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetSetMethod(this PropertyInfo property, bool nonPublic)
		{
			var method = property.SetMethod;
			if (method == null)
			{
				return null;
			}

			return (method.IsPublic || nonPublic) ? method : null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDataMember(this FieldInfo field)
		{
			return field.IsDefined(dataMemberAttributeType_, false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDataMember(this PropertyInfo property)
		{
			return property.IsDefined(dataMemberAttributeType_, false);
		}
#else
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsPrimitive(this Type type)
		{
			return type.IsPrimitive;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsString(this Type type)
		{
			return type == stringType_;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsArray(this Type type)
		{
			return type.IsArray;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsClass(this Type type)
		{
			return type.IsClass;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsNullable(this Type type)
		{
			return type.IsClass || (Nullable.GetUnderlyingType(type) != null);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.IsGenericTypeDefinition;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsSerializable(this Type type)
		{
#if PCL1 || PCL2
			return type.GetCustomAttributes(false).Any(attribute => attribute.GetType().FullName == "System.SerializableAttribute");
#else
			return type.IsSerializable;
#endif
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsDataContract(this Type type)
		{
			return type.IsDefined(dataContractAttributeType_, false);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsNonSerialized(this FieldInfo field)
		{
#if NET35 || NET4 || NET45
			return field.IsDefined(nonSerializedAttributeType_, false);
#else
			return field.GetCustomAttributes(false).Any(attribute => attribute.GetType().FullName == "System.NonSerializedAttribute");
#endif
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static Type GetBaseType(this Type type)
		{
			return type.BaseType;
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static FieldInfo[] GetInstanceFields(this Type type)
		{
			return type.GetFields(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static PropertyInfo[] GetInstanceProperties(this Type type)
		{
			return type.GetProperties(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static MethodInfo[] GetInstanceMethods(this Type type)
		{
			return type.GetMethods(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsDataMember(this FieldInfo field)
		{
			return field.IsDefined(dataMemberAttributeType_, false);
		}

#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static bool IsDataMember(this PropertyInfo property)
		{
			return property.IsDefined(dataMemberAttributeType_, false);
		}
#endif
	}
}
