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
using System.Reflection;
using System.Reflection.Emit;

namespace CenterCLR.ExaSerializers.HelperGenerator
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var assemblyName = new AssemblyName("CenterCLR.ExaSerializers.Helpers");
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				assemblyName,
				AssemblyBuilderAccess.Save);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(
				"CenterCLR.ExaSerializers.Helpers.dll");

			var typeBuilder = moduleBuilder.DefineType(
				"CenterCLR.ExaSerializers.Helpers.Pinner",
				TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Class);

			typeBuilder.DefineDefaultConstructor(MethodAttributes.Family);

			var onPinnedMethod = typeBuilder.DefineMethod(
				"OnPinned",
				MethodAttributes.Family | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.NewSlot,
				typeof(void),
				new[] { typeof(byte*) });

			onPinnedMethod.DefineParameter(1, ParameterAttributes.None, "pValue");

			DefinePinningByValueMethod(typeBuilder, onPinnedMethod);
			DefinePinningByReferenceMethod(typeBuilder, onPinnedMethod);

			typeBuilder.CreateType();

			assemblyBuilder.Save("CenterCLR.ExaSerializers.Helpers.dll");
		}

		private static void DefinePinningByValueMethod(
			TypeBuilder typeBuilder,
			MethodInfo onPinnedMethod)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				"PinningByValue",
				MethodAttributes.Public);

			var genericParameter = methodBuilder.DefineGenericParameters("TValue")[0];
			genericParameter.SetGenericParameterAttributes(
				GenericParameterAttributes.NotNullableValueTypeConstraint);
			var referencedGenericParameter = genericParameter.MakeByRefType();

			methodBuilder.SetParameters(referencedGenericParameter);
			methodBuilder.DefineParameter(1, ParameterAttributes.None, "value");

			EmitBody(methodBuilder, referencedGenericParameter, onPinnedMethod);
		}

		private static void DefinePinningByReferenceMethod(
			TypeBuilder typeBuilder,
			MethodInfo onPinnedMethod)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				"PinningByReference",
				MethodAttributes.Public);

			var genericParameter = methodBuilder.DefineGenericParameters("TValue")[0];
			genericParameter.SetGenericParameterAttributes(
				GenericParameterAttributes.ReferenceTypeConstraint);

			methodBuilder.SetParameters(genericParameter);
			methodBuilder.DefineParameter(1, ParameterAttributes.None, "value");

			EmitBody(methodBuilder, genericParameter, onPinnedMethod);
		}

		private static void EmitBody(
			MethodBuilder methodBuilder,
			Type parameterType,
			MethodInfo onPinnedMethod)
		{
			var il = methodBuilder.GetILGenerator();

			var localBuilder = il.DeclareLocal(parameterType, true);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Callvirt, onPinnedMethod);
			il.Emit(OpCodes.Ret);
		}
	}
}
