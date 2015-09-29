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

#if USE_INLINING
using System.Runtime.CompilerServices;
#endif

namespace CenterCLR.ExaSerializers.Generators
{
	internal static class ReflectionUtilities
	{
		#region Fields
		private static readonly Type enumerableType_ = typeof(IEnumerable);
		private static readonly Type genericEnumerableType_ = typeof(IEnumerable<>);
		private static readonly Type genericListType_ = typeof(IList<>);
#if NET45 || NETFX_CORE
		private static readonly Type genericReadOnlyCollectionType_ = typeof(IReadOnlyCollection<>);
#endif
		#endregion

		#region TraverseBaseTypes
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IEnumerable<Type> TraverseBaseTypes(Type type)
		{
			var currentType = type;
			while (currentType != null)
			{
				yield return currentType;
				currentType = currentType.GetBaseType();
			}
		}
		#endregion

		#region GetGenericElementType
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static Type GetGenericElementType(Type genericDefinitionType, Type type)
		{
			Debug.Assert(genericDefinitionType != null);
			Debug.Assert(genericDefinitionType.IsGenericTypeDefinition());
			Debug.Assert(genericDefinitionType.GetGenericArguments().Length == 1);
			Debug.Assert(type != null);

			if (type.IsGenericType() &&
				(type.GetGenericTypeDefinition() == genericDefinitionType))
			{
				return type.GetGenericArguments()[0];
			}

			var realType =
				type.GetInterfaces().
				FirstOrDefault(interfaceType =>
					interfaceType.IsGenericType() &&
					interfaceType.GetGenericTypeDefinition() == genericDefinitionType);

			return (realType != null) ? realType.GetGenericArguments()[0] : null;
		}
		#endregion

		#region GetEnumerableElementType
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static Type GetEnumerableElementType(Type type)
		{
			Debug.Assert(type != null);

			if (enumerableType_.IsAssignableFrom(type) == false)
			{
				return null;
			}

			return GetGenericElementType(genericEnumerableType_, type);
		}
		#endregion

		#region GetListElementType
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static Type GetListElementType(Type type)
		{
			Debug.Assert(type != null);

			if (enumerableType_.IsAssignableFrom(type) == false)
			{
				return null;
			}

			return GetGenericElementType(genericListType_, type);
		}
		#endregion

		#region GetReadOnlyCollectionElementType
#if NET45 || NETFX_CORE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetReadOnlyCollectionElementType(Type type)
		{
			Debug.Assert(type != null);

			if (enumerableType_.IsAssignableFrom(type) == false)
			{
				return null;
			}

			return GetGenericElementType(genericReadOnlyCollectionType_, type);
		}
#endif
		#endregion

		#region IsTargetField
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private static bool IsTargetField(FieldInfo field, bool isSerializable, bool isDataContract)
		{
			if (field.IsInitOnly)
			{
				return false;
			}

			if (isDataContract)
			{
				return field.IsDataMember();
			}
			else if (isSerializable)
			{
				return !field.IsNonSerialized();
			}
			else
			{
				return field.IsPublic;
			}
		}
		#endregion

		#region IsTargetProperty
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private static bool IsTargetProperty(PropertyInfo property, bool isSerializable, bool isDataContract)
		{
			var getter = property.GetGetMethod(true);
			var setter = property.GetSetMethod(true);

			if ((getter == null) ||
				(setter == null) ||
				(property.GetIndexParameters().Length >= 1))
			{
				return false;
			}

			if (isDataContract)
			{
				return property.IsDataMember();
			}
			else if (isSerializable)
			{
				return false;
			}
			else
			{
				return getter.IsPublic && setter.IsPublic;
			}
		}
		#endregion

		#region GetTargetFields
		public static IEnumerable<FieldInfo> GetTargetFields(Type type)
		{
			return TraverseBaseTypes(type).
				Reverse().
				SelectMany(t =>
					{
						var isSerializable = t.IsSerializable();
						var isDataContract = t.IsDataContract();
						return t.GetInstanceFields().
							Where(field => IsTargetField(field, isSerializable, isDataContract));
					});
		}
		#endregion

		#region GetTargetProperties
		public static IEnumerable<PropertyInfo> GetTargetProperties(Type type)
		{
			return TraverseBaseTypes(type).
				Reverse().
				SelectMany(t =>
					{
						var isSerializable = t.IsSerializable();
						var isDataContract = t.IsDataContract();
						return t.GetInstanceProperties().
							Where(property => IsTargetProperty(property, isSerializable, isDataContract));
					});
		}
		#endregion

		#region UnsafeCopyBits
#if USE_UNSAFE
#if USE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static unsafe void UnsafeCopyBits(byte* pFrom, byte* pTo, int length)
		{
			Debug.Assert(pFrom != (byte*)0);
			Debug.Assert(pTo != (byte*)0);
			Debug.Assert(length >= 1);

			var fromBound = (((byte)pFrom) & 0x07) == 0x00;
			var toBound = (((byte)pTo) & 0x07) == 0x00;

			if (fromBound && toBound)
			{
				var ps = (ulong*)pFrom;
				var pd = (ulong*)pTo;
				var remains = length;
				while (remains >= sizeof(ulong))
				{
					*(pd++) = *(ps++);
					remains -= sizeof(ulong);
				}

				var psl = (byte*)ps;
				var pdl = (byte*)pd;
				while (remains >= 1)
				{
					*(pdl++) = *(psl++);
					remains--;
				}
			}
			else
			{
				var ps = pFrom;
				var pd = pTo;
				var remains = length;
				while (remains >= 1)
				{
					*(pd++) = *(ps++);
					remains--;
				}
			}
		}
#endif
		#endregion
	}
}
