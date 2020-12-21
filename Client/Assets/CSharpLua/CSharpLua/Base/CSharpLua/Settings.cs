using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CSharpLua
{
    public static class Settings
    {
#if UNITY_EDITOR
        public static class Paths
        {
            public static readonly string CompiledScriptDir = Application.dataPath + "/HotFix/Scripts/CSharp";
            public static readonly string CompiledOutDir = Application.dataPath + "/HotFix/Scripts/Lua/Compiled";
            public static readonly string ToolsDir = Application.dataPath + "/../Tools";
            public const string kTempDir = "Assets/CSharpLuaTemp";
            public const string kCompiledScripts = "Assembly-CSharp";
            public static readonly string SettingFilePath = Application.dataPath + "/CSharpLua/CSharpLua/Base/CSharpLua/Settings.cs";
        }

        public static class Menus
        {
            public const string kCompile = "CharpLua/编译";
            public const string kRunFromCSharp = "CharpLua/运行CSharp脚本";
            public const string kRunFromLua = "CharpLua/运行Lua脚本";
            public const string kGenProtobuf = "CharpLua/生成protobuf";
        }
#endif

        public const bool kIsRunFromLua = true;
    }
}
