﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;
using Object = UnityEngine.Object;

public class CSharpLua_BridgeMonoBehaviourWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(CSharpLua.BridgeMonoBehaviour), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("Bind", Bind);
		L.RegFunction("RegisterUpdate", RegisterUpdate);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("LuaClass", get_LuaClass, set_LuaClass);
		L.RegVar("SerializeData", get_SerializeData, set_SerializeData);
		L.RegVar("SerializeObjects", get_SerializeObjects, set_SerializeObjects);
		L.RegVar("Table", get_Table, null);
		L.RegFunction("getTable", get_Table);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Bind(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)ToLua.CheckObject(L, 1, typeof(CSharpLua.BridgeMonoBehaviour));
				LuaTable arg0 = ToLua.CheckLuaTable(L, 2);
				obj.Bind(arg0);
				return 0;
			}
			else if (count == 3)
			{
				CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)ToLua.CheckObject(L, 1, typeof(CSharpLua.BridgeMonoBehaviour));
				LuaTable arg0 = ToLua.CheckLuaTable(L, 2);
				string arg1 = ToLua.CheckString(L, 3);
				obj.Bind(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: CSharpLua.BridgeMonoBehaviour.Bind");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterUpdate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)ToLua.CheckObject(L, 1, typeof(CSharpLua.BridgeMonoBehaviour));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			LuaFunction arg1 = ToLua.CheckLuaFunction(L, 3);
			obj.RegisterUpdate(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaClass(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			string ret = obj.LuaClass;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index LuaClass on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SerializeData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			string ret = obj.SerializeData;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index SerializeData on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SerializeObjects(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			UnityEngine.Object[] ret = obj.SerializeObjects;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index SerializeObjects on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Table(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			LuaInterface.LuaTable ret = obj.Table;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Table on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_LuaClass(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.LuaClass = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index LuaClass on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SerializeData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.SerializeData = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index SerializeData on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SerializeObjects(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CSharpLua.BridgeMonoBehaviour obj = (CSharpLua.BridgeMonoBehaviour)o;
			UnityEngine.Object[] arg0 = ToLua.CheckObjectArray<UnityEngine.Object>(L, 2);
			obj.SerializeObjects = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index SerializeObjects on a nil value");
		}
	}
}

