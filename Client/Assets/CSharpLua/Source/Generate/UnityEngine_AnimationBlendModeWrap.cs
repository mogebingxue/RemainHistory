﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_AnimationBlendModeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(UnityEngine.AnimationBlendMode));
		L.RegConstant("Blend", UnityEngine.AnimationBlendMode.Blend);
		L.RegConstant("Additive", UnityEngine.AnimationBlendMode.Additive);
		L.EndEnum();
		TypeTraits<UnityEngine.AnimationBlendMode>.Check = CheckType;
		StackTraits<UnityEngine.AnimationBlendMode>.Push = Push;
	}

	static void Push(IntPtr L, UnityEngine.AnimationBlendMode arg)
	{
		ToLua.Push(L, arg);
	}

	static bool CheckType(IntPtr L, int pos)
	{
		return TypeChecker.CheckEnumType(typeof(UnityEngine.AnimationBlendMode), L, pos);
	}
}

