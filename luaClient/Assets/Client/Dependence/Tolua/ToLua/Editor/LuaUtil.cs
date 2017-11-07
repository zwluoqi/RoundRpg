#region Header
/**
 * 名称：lua工具扩展
 * 作者：林洪伟
 * 日期：2016.12.1
 * 描述：
 * 可以把一个c#对象转成一个lua table字符串(用于保存成配置文件)
 **/
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;

public static class LuaUtil  {
	static StringBuilder sb =new StringBuilder();
	static int indent = 0;

	public static bool TryToLua(object obj,out string s)
	{
		s = "";
		if (obj == null)
			return false;
		sb.Remove(0, sb.Length);
		indent = 0;
		if (!InternalToLua(obj))
			return false;
		s = sb.ToString();
		return true;
	}

	static bool IsObjectType(System.Type t)
	{
		if (t.IsPrimitive || t == typeof(string))
			return false;

		return true;
	}

	static bool InternalToLua(object obj)
	{
		var type =obj.GetType();
		if (type.Equals(typeof(string)))
		{
			sb.Append("\"");
			sb.Append((string)obj);
			sb.Append("\"");
		}   
		else if (type.Equals(typeof(bool)))
		{
			sb.Append((bool)obj?"true":"false");
		}
		else if (type.Equals(typeof(int)))
		{
			sb.Append(((int)obj).ToString());
		}
		else if (type.Equals(typeof(float)))
		{
			sb.Append(((float)obj).ToString());
		}
		else if (type.Equals(typeof(long)))
		{
			sb.Append(((long)obj).ToString());
		}
		else if (type.Equals(typeof(double)))
		{
			sb.Append(((double)obj).ToString());
		}
		else if (type.IsArray)//数组
		{
			Array a = (Array)obj;
			sb.Append('[');
			++indent;
			for (int i =0;i< a.Length;++i)
			{       
				object e =a.GetValue(i);
				if (e == null)
					continue;

				//打印值，这里非原生类型换行下
				if (IsObjectType(e.GetType()))
				{
					sb.Append('\n');
					sb.Append('\t', indent);
					if (!InternalToLua(e))
						return false;
				}
				else
				{
					if (!InternalToLua(e))
						return false;
				}
				//加上逗号
				if (i != a.Length - 1)
					sb.Append(',');

			}
			--indent;
			sb.Append("]");
		}
		else if (type.GetInterface("System.Collections.IList") != null)//list
		{
			IList a = (IList)obj;
			sb.Append('[');
			++indent;
			bool hasNewLine=false;
			for (int i = 0; i < a.Count; ++i)
			{
				object e = a[i];
				if (e == null)
					continue;

				//打印值，这里非原生类型换行下
				if (IsObjectType(e.GetType()))
				{
					sb.Append('\n');
					sb.Append('\t', indent);
					hasNewLine = true;
					if (!InternalToLua(e))
						return false;
				}
				else
				{
					if (!InternalToLua(e))
						return false;
				}
				//加上逗号
				sb.Append(',');

			}
			--indent;
			if(hasNewLine)
			{
				sb.Append('\n');
				sb.Append('\t', indent);
				sb.Append(']');
			}
			else
				sb.Append(']');
		}
		else if (type.GetInterface("System.Collections.IDictionary") != null)//dict
		{
			MethodInfo m = type.GetMethod("get_Item");//list的索引器的返回值就是对应类型
			if (m == null)
			{
				Debug.LogError("找不到索引器 不能确定类型:" + type.Name);
				return false;
			}
			if(m.GetParameters()[0].ParameterType != typeof(string))
			{
				Debug.LogError("字典的key值必须是string:" + type.Name);
				return false;
			}

			IDictionary a = (IDictionary)obj;
			sb.Append('{');
			++indent;
			bool hasNewLine = false;
			foreach (DictionaryEntry pair in a)
			{
				if (pair.Value == null)
					continue;

				//key
				sb.Append('\n');
				sb.Append('\t', indent);
				sb.Append((string)pair.Key);
				sb.Append(" = ");
				hasNewLine = true;

				//value
				if (!InternalToLua(pair.Value))
					return false;

				sb.Append(',');
			}

			--indent;
			if (hasNewLine)
			{
				sb.Append('\n');
				sb.Append('\t', indent);
				sb.Append('}');
			}
			else
				sb.Append('}');
		}
		else if(IsObjectType(type)&& !type.IsGenericType)//类，这里严格点，暂时不考虑模板
		{
			sb.Append('{');
			++indent;
			bool hasNewLine = false;
			foreach (FieldInfo info in type.GetFields())
			{
				var field =info.GetValue(obj);
				if (field == null)
					continue;

				//key
				sb.Append('\n');
				sb.Append('\t', indent);
				sb.Append(info.Name);
				sb.Append(" = ");
				hasNewLine = true;

				//value
				if (!InternalToLua(field))
					return false;

				sb.Append(',');
			}
			--indent;
			if (hasNewLine)
			{
				sb.Append('\n');
				sb.Append('\t', indent);
				sb.Append('}');
			}
			else
				sb.Append('}');
		}
		else
		{
			Debug.LogError("不能转成lua的类型:"+type.Name);
			return false;
		}


		return true;
	}


}