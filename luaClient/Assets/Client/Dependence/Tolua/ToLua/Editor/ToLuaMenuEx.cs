#region Header
/**
 * 名称：生成用于ZeroBraneStudio的api文件
 * 作者：林洪伟
 * 日期：2016.12.1
 * 描述：
 * ZeroBraneStudio的api规则见:https://studio.zerobrane.com/doc-api-auto-complete
 **/
#endregion

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;
using LuaInterface;

using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
using Debugger = LuaInterface.Debugger;
using System.Threading;


public static class ToLuaMenuEx
{
	public class LuaApi
	{
		public string type = "class";//keyword,class ,lib,value,function,method
		public string description = null;
		public string args = null;//"(filename: string [, mode: string])"
		public string returns = null;
		public string valuetype = null;//值的类型，或者返回值类型
		public string inherits = null;

		public Dictionary<string, LuaApi> childs = null;

		public LuaApi AddClass(string name, string inherits=null)
		{
			if (childs == null) childs = new Dictionary<string, LuaApi>();

			var api = new LuaApi();
			childs[name] = api;
			api.type = "class";
			api.inherits = inherits;
			return api;
		}
		public LuaApi AddValue(string name, string valuetype = null)
		{
			if (childs == null) childs = new Dictionary<string, LuaApi>();

			var api = new LuaApi();
			childs[name] = api;
			api.type = "value";
			api.valuetype = valuetype != "void" ? valuetype : null;
			return api;
		}

		public LuaApi AddMethod(string name, string args = null, string returns = null, string valuetype = null)
		{
			if (childs == null) childs = new Dictionary<string, LuaApi>();

			var api = new LuaApi();
			childs[name] = api;
			api.type = "method";
			api.args = args;
			api.returns = returns;
			api.valuetype = valuetype != "void" ? valuetype : null;
			return api;
		}

		public LuaApi AddFunction(string name, string args = null, string returns = null, string valuetype = null)
		{
			if (childs == null) childs = new Dictionary<string, LuaApi>();

			var api = new LuaApi();
			childs[name] = api;
			api.type = "function";
			api.args = args;
			api.returns = returns;
			api.valuetype = valuetype!="void"? valuetype:null;
			return api;
		}
	}
	static List<ToLuaMenu.BindType> allTypes = new List<ToLuaMenu.BindType>();
	static LuaApi s_api;
	static Dictionary<string, LuaApi> s_apiIdx = new Dictionary<string, LuaApi>();//名字索引,是fullname，也就是带命名空间
	static Dictionary<Type, ToLuaMenu.BindType> s_apiTypeIdx = new Dictionary<Type, ToLuaMenu.BindType>();
	static StringBuilder s_paramName = new StringBuilder();

	[MenuItem("Lua/生成自动提示的文件")]
	public static void GenAutoComplete()
	{
		s_api = new LuaApi();
		s_apiIdx.Clear();
		s_apiTypeIdx.Clear();

		//收集要生成的类
		List<ToLuaMenu.BindType> btList = new List<ToLuaMenu.BindType>();
		allTypes.Clear();
		ToLuaExport.allTypes.Clear();
		ToLuaExport.allTypes.AddRange(ToLuaMenu.baseType);
		ToLuaExport.allTypes.AddRange(CustomSettings.staticClassTypes);
		for (int i = 0; i < ToLuaExport.allTypes.Count; i++)
		{
			btList.Add(new ToLuaMenu.BindType(ToLuaExport.allTypes[i]));
		}
		foreach(var bt in CustomSettings.customTypeList)
		{
			if (ToLuaExport.allTypes.Contains(bt.type)) continue;
			ToLuaExport.allTypes.Add(bt.type);
			btList.Add(bt);
		}
		GenBindTypes(btList.ToArray(), false);
		foreach(var bt in allTypes)//做最后的检查，进一步排除一些类
		{
			if (bt.type.IsInterface && bt.type != typeof(System.Collections.IEnumerator))
				continue;
			s_apiTypeIdx[bt.type] = bt; 
		}
		//一些类需要手动加
		{
			ToLuaMenu.BindType bt = new ToLuaMenu.BindType(typeof(Array));
			s_apiTypeIdx[bt.type] = bt;
			GetClassApi("System.Collections.IEnumerable").AddMethod("GetEnumerator", "()", "System.Collections.IEnumerator", "System.Collections.IEnumerator");
		}


		//生成信息
		foreach (var bt in s_apiTypeIdx.Values)
		{
			GenApi(bt);
		}

		//信息转lua类文件
		ToLuaExport.allTypes.Clear();
		string s;
		if (!LuaUtil.TryToLua(s_api.childs, out s))
			return;
		s = "return " + s;
		string path = LuaConst.zbsDir;
		path = path.Replace("lualibs/mobdebug", "api/lua/gameApiGen.lua");
		System.IO.File.WriteAllText(path, s, System.Text.Encoding.UTF8);
		Debug.Log("生成自动提示文件成功:"+ path);
	}

	static ToLuaMenu.BindType[] GenBindTypes(ToLuaMenu.BindType[] list, bool beDropBaseType = true)
	{
		allTypes = new List<ToLuaMenu.BindType>(list);

		for (int i = 0; i < list.Length; i++)
		{
			for (int j = i + 1; j < list.Length; j++)
			{
				if (list[i].type == list[j].type)
					throw new NotSupportedException("Repeat BindType:" + list[i].type);
			}

			if (ToLuaMenu.dropType.IndexOf(list[i].type) >= 0)
			{
				Debug.LogWarning(list[i].type.FullName + " in dropType table, not need to export");
				allTypes.Remove(list[i]);
				continue;
			}
			else if (beDropBaseType && ToLuaMenu.baseType.IndexOf(list[i].type) >= 0)
			{
				Debug.LogWarning(list[i].type.FullName + " is Base Type, not need to export");
				allTypes.Remove(list[i]);
				continue;
			}
			else if (list[i].type.IsEnum)
			{
				continue;
			}

			AutoAddBaseType(list[i], beDropBaseType);
		}

		return allTypes.ToArray();
	}

	static void AutoAddBaseType(ToLuaMenu.BindType bt, bool beDropBaseType)
	{
		Type t = bt.baseType;

		if (t == null)
		{
			return;
		}

		if (t.IsInterface)
		{
			Debugger.LogWarning("{0} has a base type {1} is Interface, use SetBaseType to jump it", bt.name, t.FullName);
			bt.baseType = t.BaseType;
		}
		else if (ToLuaMenu.dropType.IndexOf(t) >= 0)
		{
			Debugger.LogWarning("{0} has a base type {1} is a drop type", bt.name, t.FullName);
			bt.baseType = t.BaseType;
		}
		else if (!beDropBaseType || ToLuaMenu.baseType.IndexOf(t) < 0)
		{
			int index = allTypes.FindIndex((iter) => { return iter.type == t; });

			if (index < 0)
			{
				#if JUMP_NODEFINED_ABSTRACT
				if (t.IsAbstract && !t.IsSealed)
				{
				Debugger.LogWarning("not defined bindtype for {0}, it is abstract class, jump it, child class is {1}", t.FullName, bt.name);
				bt.baseType = t.BaseType;
				}
				else
				{
				Debugger.LogWarning("not defined bindtype for {0}, autogen it, child class is {1}", t.FullName, bt.name);
				bt = new BindType(t);
				allTypes.Add(bt);
				}
				#else
				Debugger.LogWarning("not defined bindtype for {0}, autogen it, child class is {1}", t.FullName, bt.name);
				bt = new ToLuaMenu.BindType(t);
				allTypes.Add(bt);
				#endif
			}
			else
			{
				return;
			}
		}
		else
		{
			return;
		}
		AutoAddBaseType(bt, beDropBaseType);
	}
	static void GenApi(ToLuaMenu.BindType bt)
	{
		ToLuaExport.Clear();
		ToLuaExport.className = bt.name;
		ToLuaExport.type = bt.type;
		ToLuaExport.isStaticClass = bt.IsStatic;
		ToLuaExport.baseType = bt.baseType;
		ToLuaExport.wrapClassName = bt.wrapName;
		ToLuaExport.libClassName = bt.libName;
		if (bt.type.IsInterface && bt.type != typeof(System.Collections.IEnumerator))
			return;
		//如果是枚举
		if (bt.type.IsEnum)
		{
			GenEnumApi(bt);
		}
		//如果是类
		else
		{
			ToLuaExport.InitMethods();
			ToLuaExport.InitPropertyList();
			ToLuaExport.InitCtorList();
			GenClassApi(bt);
		}
	}

	static void GenEnumApi(ToLuaMenu.BindType bt)
	{
		//创建api类
		var api = GetClassApi(bt.name);
		foreach (var f in ToLuaExport.type.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static))
		{
			if (ToLuaExport.IsObsolete(f))
				continue;
			api.AddValue(f.Name, bt.name);
		}
		api.AddMethod("IntToEnum");
	}

	static void GenClassApi(ToLuaMenu.BindType bt)
	{
		Type type = bt.type;
		if (type.IsGenericType)//泛型类被tolua处理成全局库了，如果都加成api自动提示的时候反而不方便
			return;

		//计算lua中的全局名和继承者
		string name = bt.name;
		string inherits = null;
		if (bt.baseType != null && !bt.baseType.IsGenericType)
			inherits = ToLuaExport.GetBaseTypeStr(bt.baseType);

		//创建api类
		var api = GetClassApi(name, inherits);

		string returns = null, valueType = null;
		//注册成员函数，参考的是ToLuaExport.GenRegisterFuncItems
		for (int i = 0; i < ToLuaExport.methods.Count; i++)
		{
			MethodInfo m = ToLuaExport.methods[i];
			int count = 1;
			string methodName = ToLuaExport.GetMethodName(m);
			if (ToLuaExport.nameCounter.TryGetValue(methodName, out count))
			{
				ToLuaExport.nameCounter[methodName] = count + 1;
				continue;
			}
			ToLuaExport.nameCounter[methodName] = 1;


			if (m.IsGenericMethod || methodName == "set_Item" || methodName == "get_Item" || methodName.StartsWith("op_"))
				continue;

			//获取返回值信息
			GetReturnTypeStr(m.ReturnType, ref returns, ref valueType);

			//获取参数信息
			ParameterInfo[] paramInfos = m.GetParameters();
			s_paramName.Clear();
			s_paramName.Append('(');
			for(int j= 0;j< paramInfos.Length;++j)
			{
				s_paramName.Append(paramInfos[j].Name);
				if (j != paramInfos.Length - 1) s_paramName.Append(',');
			}
			s_paramName.Append(')');
			string param = s_paramName.ToString();

			if (m.IsStatic)
				api.AddFunction(methodName, param, returns, valueType);
			else
				api.AddMethod(methodName, param, returns, valueType);
		}

		//注册成员变量和属性
		for (int i = 0; i < ToLuaExport.fields.Length; i++)
		{
			GetReturnTypeStr(ToLuaExport.fields[i].FieldType, ref returns, ref valueType);
			api.AddValue(ToLuaExport.fields[i].Name, valueType);
		}
		for (int i = 0; i < ToLuaExport.props.Length; i++)
		{
			GetReturnTypeStr(ToLuaExport.props[i].PropertyType, ref returns, ref valueType);
			api.AddValue(ToLuaExport.props[i].Name, valueType);
		}

		//注册操作符,暂时不需要
		//注册构造函数，暂时不需要
		//注册索引器，暂时不需要
	}

	static void GetReturnTypeStr(Type t,ref string returns,ref string valueType)
	{
		returns = null;
		valueType = null;
		if (t == typeof(void))
		{
			returns = "void";
			return;
		}


		if(t.IsByRef)
			t = t.GetElementType();

		//如果是基础类型，不提示
		if (t.IsPrimitive){returns =t.Name;return;}
		else if (t == typeof(string)) { returns = "System.String"; return; }
		else if (t == typeof(Vector3)) { returns = "Vector3"; return; }
		else if (t == typeof(Quaternion)) { returns = "Quaternion"; return; }
		else if (t == typeof(Vector2)) { returns = "Vector2"; return; }
		else if (t == typeof(Vector4)) { returns = "Vector4"; return; }
		else if (t == typeof(Color)) { returns = "Color"; return; }
		else if (t == typeof(Ray)) { returns = "Ray"; return; }
		else if (t == typeof(Bounds)) { returns = "Bounds"; return; }
		else if (t == typeof(LayerMask)) { returns = "LayerMask"; return; }
		else if(t.IsSubclassOf(typeof(UnityEngine.Events.UnityEvent))) { returns = valueType = "UnityEngine.Events.UnityEvent"; return; }

		//如果是集合类，那么转下
		if (t.IsArray) { returns = valueType = "System.Array"; return; }
		if (t.GetInterface("System.Collections.IList") != null){
			returns = valueType = "List";
			return;
		}
		if (t.GetInterface("System.Collections.IDictionary") != null)
		{
			returns = valueType = "Dictionary";
			return;
		}
		if (t.GetInterface("System.Collections.IEnumerable") != null)
		{
			returns = valueType = "System.Collections.IEnumerable";
			return;
		}
		if (t.GetInterface("System.Collections.IEnumerator") != null)
		{
			returns = valueType = "System.Collections.IEnumerator";
			return;
		}

		//如果不是集合类，但又是泛型，tolua是处理成全局库的，这里直接忽略
		if (t.IsGenericType)
			return;

		if (!s_apiTypeIdx.ContainsKey(t))
			return ;

		//最后剩下的类型就只有c#导出到lua的类型了
		returns = valueType = ToLuaExport.GetBaseTypeStr(t);
	}

	static LuaApi GetClassApi(string name, string inherits = null)
	{
		LuaApi api;
		if (s_apiIdx.TryGetValue(name, out api))
		{
			if (api.inherits == null && inherits != null)
				api.inherits = inherits;
			return api;
		}

		LuaApi parent = s_api;
		//先获取父命名空间
		int at = name.LastIndexOf('.');
		if (at != -1)
			parent = GetClassApi(name.Substring(0, at));

		api = parent.AddClass(at != -1 ? name.Substring(at + 1) : name, inherits);
		s_apiIdx[name] = api;
		return api;
	}
}