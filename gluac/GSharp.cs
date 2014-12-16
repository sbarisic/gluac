using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Collections.Generic;
//using System.Dynamic;

namespace GSharp {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int LuaFunc(IntPtr L);

	public static class Debug {
		[DllImport("kernel32")]
		public static extern bool AllocConsole();

		public static void Msg(object Msg) {
			MessageBox.Show(Msg.ToString(), "G# Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public static void Error(object Msg) {
			MessageBox.Show(Msg.ToString(), "G# Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void Warning(object Msg) {
			MessageBox.Show(Msg.ToString(), "G# Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}

	public static unsafe class Lua {
		static string PtrToStr(IntPtr StrPtr) {
			return Marshal.PtrToStringAnsi(StrPtr);
		}

		static T StrToPtr<T>(string Str, Func<IntPtr, T> F) {
			T R;
			IntPtr StrPtr = Marshal.StringToHGlobalAnsi(Str);
			R = F(StrPtr);
			Marshal.FreeHGlobal(StrPtr);
			return R;
		}

		static List<LuaFunc> LuaFuncs = new List<LuaFunc>();
		const string LIBNAME = "lua_shared.dll";
		const CallingConvention CConv = CallingConvention.Cdecl;
		const CharSet CSet = CharSet.Auto;

		public const int TNONE = -1;
		public const int TNIL = 0;
		public const int TBOOLEAN = 1;
		public const int TLIGHTUSERDATA = 2;
		public const int TNUMBER = 3;
		public const int TSTRING = 4;
		public const int TTABLE = 5;
		public const int TFUNCTION = 6;
		public const int TUSERDATA = 7;
		public const int TTHREAD = 8;

		public const int REGISTRYINDEX = -10000;
		public const int ENVIRONINDEX = -10001;
		public const int GLOBALSINDEX = -10002;

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_getmetafield")]
		public static extern int GetMetaField(IntPtr State, int I, IntPtr S);

		public static int GetMetaField(IntPtr State, int I, string S) {
			return StrToPtr(S, (Sp) => {
				return GetMetaField(State, I, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_callmeta")]
		public static extern int CallMeta(IntPtr State, int I, IntPtr S);

		public static int CallMeta(IntPtr State, int I, string S) {
			return StrToPtr<int>(S, (Sp) => {
				return CallMeta(State, I, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_newstate")]
		public static extern IntPtr NewState();

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_openlibs")]
		public static extern void OpenLibs(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_base")]
		public static extern int OpenBase(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_bit")]
		public static extern int OpenBit(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_debug")]
		public static extern int OpenDebug(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_jit")]
		public static extern int OpenJit(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_math")]
		public static extern int OpenMath(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_os")]
		public static extern int OpenOS(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_package")]
		public static extern int OpenPackage(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_string")]
		public static extern int OpenString(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaopen_table")]
		public static extern int OpenTable(IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_typerror")]
		public static extern int TypeError(IntPtr State, int I, IntPtr S);

		public static int TypeError(IntPtr State, int I, string S) {
			return StrToPtr<int>(S, (Sp) => {
				return TypeError(State, I, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_argerror")]
		public static extern int ArgError(IntPtr State, int I, IntPtr S);

		public static int ArgError(IntPtr State, int I, string S) {
			return StrToPtr<int>(S, (Sp) => {
				return ArgError(State, I, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checklstring")]
		public static extern string CheckLString(IntPtr State, int I, int L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_optlstring")]
		public static extern string OptLString(IntPtr State, int I, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checknumber")]
		public static extern double CheckNumber(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_optnumber")]
		public static extern double OptNumber(IntPtr State, int I, double Def);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkinteger")]
		public static extern int CheckInt(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_optinteger")]
		public static extern int OptInt(IntPtr State, int I, int Def);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkstack")]
		public static extern void CheckStack(IntPtr State, int I, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checktype")]
		public static extern void CheckType(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkany")]
		public static extern void CheckAny(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_newmetatable")]
		public static extern int NewMetatable(IntPtr State, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_checkudata")]
		public static extern IntPtr CheckUData(IntPtr State, int I, string S);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_where")]
		public static extern void Where(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_error")]
		public static extern int Error(IntPtr State, IntPtr S);

		public static int Error(IntPtr State, string S) {
			return StrToPtr<int>(S, (Sp) => {
				return Error(State, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_ref")]
		public static extern int Ref(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_unref")]
		public static extern void Unref(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadfile")]
		public static extern int LoadFile(IntPtr State, IntPtr S);

		public static int LoadFile(IntPtr State, string S) {
			return StrToPtr<int>(S, (Sp) => {
				return LoadFile(State, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadbuffer")]
		public static extern int LoadBuffer(IntPtr State, string S, int I, string S2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_loadstring")]
		public static extern int LoadString(IntPtr State, IntPtr S);

		public static int LoadString(IntPtr State, string S) {
			return StrToPtr<int>(S, (Sp) => {
				return LoadString(State, Sp);
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_gsub")]
		public static extern string GSub(IntPtr State, string S, string S2, string S3);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "luaL_findtable")]
		public static extern string FindTable(IntPtr State, int I, string S, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_close")]
		public static extern void Close(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_newthread")]
		public static extern IntPtr NewThread(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_atpanic")]
		public static extern LuaFunc AtPanic(IntPtr State, LuaFunc F);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gettop")]
		public static extern int GetTop(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_settop")]
		public static extern void SetTop(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushvalue")]
		public static extern void PushValue(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_remove")]
		public static extern void Remove(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_insert")]
		public static extern void Insert(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_replace")]
		public static extern void Replace(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_xmove")]
		public static extern void XMove(IntPtr State, IntPtr L, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_isnumber")]
		public static extern bool IsNumber(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_isstring")]
		public static extern bool IsString(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_iscfunction")]
		public static extern bool IsCFunction(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_isuserdata")]
		public static extern bool IsUserdata(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_type")]
		public static extern int Type(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_typename")]
		public static extern IntPtr _TypeName(IntPtr State, int I);

		public static string TypeName(IntPtr State, int I = -1) {
			return PtrToStr(_TypeName(State, I));
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_equal")]
		public static extern int Equal(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawequal")]
		public static extern int RawEqual(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_lessthan")]
		public static extern int LessThan(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tonumber")]
		public static extern double ToNumber(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tointeger")]
		public static extern int ToInteger(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_toboolean")]
		public static extern bool ToBoolean(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tolstring")]
		public static extern IntPtr ToLString(IntPtr State, int I, IntPtr L);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_objlen")]
		public static extern int ObjLen(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tocfunction")]
		public static extern LuaFunc ToCFunction(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_touserdata")]
		public static extern IntPtr ToUserdata(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_tothread")]
		public static extern IntPtr ToThread(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_topointer")]
		public static extern IntPtr ToPointer(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushnil")]
		public static extern void PushNil(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushnumber")]
		public static extern void PushNumber(IntPtr State, double D);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushinteger")]
		public static extern void PushInteger(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushlstring")]
		public static extern void PushLString(IntPtr State, string S, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushstring")]
		public static extern void PushString(IntPtr State, IntPtr S);

		public static void PushString(IntPtr State, string S) {
			StrToPtr<int>(S, (Sp) => {
				PushString(State, Sp);
				return 0;
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushcclosure")]
		public static extern void PushCClosure(IntPtr State, LuaFunc F, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushboolean")]
		public static extern void PushBoolean(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushlightuserdata")]
		public static extern void PushLightUserdata(IntPtr State, IntPtr P);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pushthread")]
		public static extern int PushThread(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gettable")]
		public static extern void GetTable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getfield")]
		public static extern void GetField(IntPtr State, int I, IntPtr S);

		public static void GetField(IntPtr State, int I, string S) {
			StrToPtr<int>(S, (Sp) => {
				GetField(State, I, Sp);
				return 0;
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawget")]
		public static extern void RawGet(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawgeti")]
		public static extern void RawGetI(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_createtable")]
		public static extern void CreateTable(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_newuserdata")]
		public static extern IntPtr NewUserdata(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getmetatable")]
		public static extern bool GetMetatable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getfenv")]
		public static extern void GetFEnv(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_settable")]
		public static extern void SetTable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setfield")]
		public static extern void SetField(IntPtr State, int I, IntPtr S);

		public static void SetField(IntPtr State, int I, string S) {
			StrToPtr<int>(S, (Sp) => {
				SetField(State, I, Sp);
				return 0;
			});
		}

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawset")]
		public static extern void RawSet(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_rawseti")]
		public static extern void RawSetI(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setmetatable")]
		public static extern int SetMetatable(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_setfenv")]
		public static extern int SetFEnv(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_call")]
		public static extern void Call(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_pcall")]
		public static extern int PCall(IntPtr State, int I, int I2, int I3);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_cpcall")]
		public static extern int CPCall(IntPtr State, LuaFunc F, IntPtr P);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_yield")]
		public static extern int Yield(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_resume_real")]
		public static extern int Resume(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_status")]
		public static extern int Status(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gc")]
		public static extern int GC(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_error")]
		public static extern int Error(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_next")]
		public static extern int Next(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_concat")]
		public static extern void Concat(IntPtr State, int I);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_getupvalue")]
		public static extern string GetUpValue(IntPtr State, int I, int I2);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gethookmask")]
		public static extern int GetHookMask(IntPtr State);

		[DllImport(LIBNAME, CallingConvention = CConv, CharSet = CSet, EntryPoint = "lua_gethookcount")]
		public static extern int GetHookCount(IntPtr State);

		// Custom

		public static string ToString(IntPtr State, int I) {
			return PtrToStr(Lua.ToLString(State, I, IntPtr.Zero));
		}

		public static void Pop(IntPtr State, int I = 1) {
			SetTop(State, -(I) - 1);
		}

		public static bool IsTable(IntPtr State, int I) {
			return Type(State, I) == TTABLE;
		}

		public static void PushCFunction(IntPtr State, LuaFunc F) {
			if (!LuaFuncs.Contains(F))
				LuaFuncs.Add(F);
			PushCClosure(State, F, 0);
		}

		/*
		public static void Push(IntPtr L, object O) {
			if (O == null)
				Lua.PushNil(L);
			else if (O is string)
				Lua.PushString(L, O.ToString());
			else if (O is int)
				Lua.PushInteger(L, (int)O);
			else if (O is float || O is double)
				Lua.PushNumber(L, (double)O);
			else if (O is LuaFunc)
				Lua.PushCFunction(L, (LuaFunc)O);
			else if (O is bool)
				Lua.PushBoolean(L, (bool)O ? 1 : 0);
			else if (O is LuaTable) {
				LuaTable T = (LuaTable)O;
				if (T.Path.Count == 0)
					Lua.CreateTable(L, 0, 0);
				else
					Lua.GetGlobal(L, T.Path.ToArray());
			} else if (O is LuaObject) {
				LuaObject T = (LuaObject)O;
				Lua.GetGlobal(L, T.Path.ToArray());
			} else
				throw new Exception("Invalid type " + O.GetType().FullName);
		}
		//*/

		/*
		public static object To(IntPtr L, int I = -1, params string[] Pth) {
			int T = Lua.Type(L, I);

			switch (T) {
				case TSTRING:
					return Lua.ToString(L, I);
				case TNUMBER:
					return Lua.ToNumber(L, I);
				case TFUNCTION: {
						LuaFunc LF = Lua.ToCFunction(L, I);
						if (!LuaFuncs.Contains(LF))
							LuaFuncs.Add(LF);
						return LF;
					}
				case TBOOLEAN:
					return Lua.ToBoolean(L, I);

				case TNIL: {
						LuaObject O = new LuaObject(L);
						O.Type = T;
						O.Path.AddRange(Pth);
						return O;
					}
				case TTABLE: {
						LuaTable Tbl = new LuaTable(L, Pth);
						Tbl.Type = T;
						return Tbl;
					}
			}

			return null;
		}
		//*/

		public static void GetGlobal(IntPtr L, string Key) {
			Lua.GetField(L, Lua.GLOBALSINDEX, Key);
		}

		public static int GetGlobal(IntPtr L, params string[] Tables) {
			Lua.GetField(L, Lua.GLOBALSINDEX, "_G");
			int Pops = 1;

			for (int i = 0; i < Tables.Length; i++) {
				if (Lua.Type(L, -1) != Lua.TTABLE)
					throw new Exception(Tables[i - 1] + " is not a table");
				Lua.PushString(L, Tables[i]);
				Lua.GetTable(L, -2);
				Lua.Remove(L, -2);
				Pops++;
			}

			return Pops;
		}

		public static void RegisterCFunction(IntPtr State, string TableName, string FuncName, LuaFunc F) {
			GetField(State, GLOBALSINDEX, TableName);
			if (!IsTable(State, -1)) {
				CreateTable(State, 0, 1);
				SetField(State, GLOBALSINDEX, TableName);
				Pop(State);

				GetField(State, GLOBALSINDEX, TableName);
			}

			PushString(State, FuncName);
			PushCFunction(State, F);
			SetTable(State, -3);
			Pop(State);
		}

		public static string[] GetStack(IntPtr State) {
			int Cnt = Lua.GetTop(State);
			List<string> R = new List<string>();

			for (int i = Cnt; i >= 1; i--)
				R.Add(string.Format("{0}, {1} = {2}", -i, Cnt - i + 1, Lua.TypeName(State, Lua.Type(State, -i))));

			return R.ToArray();
		}

		public static void PrintStack(IntPtr State, string Title = "") {
			string[] St = GetStack(State);
			Console.WriteLine("{0} {1}", Title, "{");
			foreach (var S in St)
				Console.WriteLine("   {0}", S);
			Console.WriteLine("{0}", "}");
		}
	}

	public static unsafe class GMod {
		private static IntPtr State;
		public static object Lock = new object();

		public static void Init(IntPtr State) {
			GMod.State = State;
		}

		public static void Print(object O) {
			lock (Lock) {
				Lua.GetField(State, Lua.GLOBALSINDEX, "print");
				Lua.PushString(State, O != null ? O.ToString() : "NULL");
				Lua.Call(State, 1, 0);
			}
		}

		public static void MsgC(int R, int G, int B, string Msg) {
			lock (Lock) {
				Lua.GetField(State, Lua.GLOBALSINDEX, "MsgC");

				Lua.CreateTable(State, 0, 3);
				Lua.PushString(State, "r");
				Lua.PushNumber(State, R);
				Lua.SetTable(State, -3);

				Lua.PushString(State, "g");
				Lua.PushNumber(State, G);
				Lua.SetTable(State, -3);

				Lua.PushString(State, "b");
				Lua.PushNumber(State, B);
				Lua.SetTable(State, -3);

				Lua.PushString(State, "a");
				Lua.PushNumber(State, 255);
				Lua.SetTable(State, -3);

				Lua.PushString(State, Msg);
				Lua.Call(State, 2, 0);
			}
		}
	}

	/*
	public class LuaObject : DynamicObject {
		public IntPtr L;

		public int Type;
		public List<string> Path;

		public LuaObject(IntPtr L, bool Value = false) {
			this.L = L;
			this.Type = Lua.TTABLE;
			Path = new List<string>();

			if (Value) {
				this.Type = Lua.Type(L, -1);
			}
		}

		public override bool TryGetMember(GetMemberBinder b, out object r) {
			List<string> Pth = new List<string>();
			Pth.AddRange(Path);
			Pth.Add(b.Name);

			Lua.GetGlobal(L, Pth.ToArray());
			r = Lua.To(L, -1, Pth.ToArray());
			Lua.Pop(L);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder b, object Val) {
			Lua.GetGlobal(L, Path.ToArray());

			if (Lua.Type(L, -1) != Lua.TTABLE)
				return false;

			Lua.PushString(L, b.Name);
			Lua.Push(L, Val);
			Lua.SetTable(L, -3);

			Lua.Pop(L);
			return true;
		}
	}

	public class LuaTable : LuaObject {
		public LuaTable(IntPtr L, params string[] Path)
			: base(L) {
			this.Path = new List<string>(Path);
		}

		public object this[object Idx] {
			get {
				Lua.GetGlobal(L, Path.ToArray());
				Lua.Push(L, Idx);
				Lua.GetTable(L, -2);
				LuaObject LObj = new LuaObject(L, true);
				Lua.Pop(L);
				return LObj;
			}

			set {
				Lua.GetGlobal(L, Path.ToArray());
				Lua.Push(L, Idx);
				Lua.Push(L, value);
				Lua.SetTable(L, -3);
				Lua.Pop(L);
			}
		}
	}
	//*/
}