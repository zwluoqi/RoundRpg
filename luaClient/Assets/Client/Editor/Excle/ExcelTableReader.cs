using UnityEngine;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System;
using NPOI.HSSF.UserModel;

namespace UnityHelp
{
	[Serializable]
	public class DataBase{
		public int id;
	}

    public class TableStruct
    {
        public string fieldDesc = "";
        public string fieldType = "";
        public string fieldNameClient = "";
        public string fieldNameServer = "";
		public string primary = "";
    }

    public class ExcelTableReader
    {
        public static ExcelTableReader Inst()
        {
            if (null == inst)
            {
                inst = new ExcelTableReader();
            }
            return inst;
        }

        public HSSFWorkbook WriteTable<T>(string path, string sheet, Dictionary<int, T> dic, TableStruct[] ts, HSSFWorkbook sfworkbook = null) where T : DataBase, new()
        {
            ExcelHelper excel = new ExcelHelper(path);
            DataTable table = new DataTable("_");

            for (int i = 0; i < ts.Length; ++i)
            {
                table.Columns.Add(ts[i].fieldDesc);
            }

            for (int colIdx = 1; colIdx < 5; ++colIdx)
            {
                object[] dataRow = new object[ts.Length];
                for (int i = 0; i < ts.Length; ++i)
                {
                    string fieldName = "";
                    if (colIdx == 0)
                    {
                        fieldName = ts[i].fieldDesc;
                    }
                    if (colIdx == 1)
                    {
                        fieldName = ts[i].fieldType;
                    }
                    if (colIdx == 2)
                    {
                        fieldName = ts[i].fieldNameClient;
                    }
                    if (colIdx == 3)
                    {
                        fieldName = ts[i].fieldNameServer;
                    }
					if (colIdx == 4) {
						fieldName = ts [i].primary;
					}

                    dataRow[i] = fieldName;
                }
                table.Rows.Add(dataRow);
            }

            foreach (var kv in dic)
            {
                T data = kv.Value;
                object[] dataRow = new object[ts.Length];
                for (int colIdx = 0; colIdx < ts.Length; ++colIdx)
                {
                    string fieldName = ts[colIdx].fieldNameClient;
                    switch (ts[colIdx].fieldType)
                    {
                        case "int":
                        case "float":
                        case "string":
                            dataRow[colIdx] = data.GetType().GetField(fieldName).GetValue(data);
                            break;
                        case "array_int":
                            {
                                string _str = "";
                                List<int> _list = new List<int>();
                                _list = data.GetType().GetField(fieldName).GetValue(data) as List<int>;
                                if (_list != null)
                                {
                                    for (int i = 0; i < _list.Count; i++)
                                    {
                                        if (i == 0)
                                        {
                                            _str += _list[i];
                                        }
                                        else
                                        {
                                            _str += "&&" + _list[i];
                                        }
                                    }
                                }
                                dataRow[colIdx] = _str;
                            }
                            break;
                        case "array_string":
                            {
                                string _str = "";
                                List<string> _list = new List<string>();
                                _list = data.GetType().GetField(fieldName).GetValue(data) as List<string>;
                                if (_list != null)
                                {
                                    for (int i = 0; i < _list.Count; i++)
                                    {
                                        if (i == 0)
                                        {
                                            _str += _list[i];
                                        }
                                        else
                                        {
                                            _str += "&&" + _list[i];
                                        }
                                    }
                                }
                                dataRow[colIdx] = _str;
                            }
                            break;
                        case "array_float":
                            break;
                        default:
                            Debug.LogError("error! unknown type=" + ts[colIdx].fieldType);
                            break;
                    }
                }
                table.Rows.Add(dataRow);
            }

            return excel.DataTableToExcel(table, sheet, true, sfworkbook);
        }

        public void ReadTable<T>(string path, string sheet, Dictionary<int, T> dic, out TableStruct[] ts) where T : DataBase, new()
        {
            ts = null;
            ExcelHelper excel = new ExcelHelper(path);
            DataTable table = excel.ExcelToDataTable(sheet, true);
            if (null == table)
            {
				Debug.LogError("error! table[" + path + "] sheet[" + sheet + "] not found!");
                return;
            }
            //read table struct
            ts = new TableStruct[table.Columns.Count];
            for (int i = 0; i < table.Columns.Count; ++i)
            {
                ts[i] = new TableStruct();
            }
            for (int j = 0; j < table.Columns.Count; ++j)
            {
                for (int i = 0; i < 4; ++i)
                {
                    switch (i)
                    {
                        case 0://field desc
                            ts[j].fieldDesc = table.Rows[i].ItemArray[j].ToString();
                            break;
                        case 1://type
                            ts[j].fieldType = table.Rows[i].ItemArray[j].ToString();
                            break;
                        case 2://field name of client
                            ts[j].fieldNameClient = table.Rows[i].ItemArray[j].ToString();
                            break;
                        case 3://field name of server
                            ts[j].fieldNameServer = table.Rows[i].ItemArray[j].ToString();
                            break;
                        default://data
                            //data = table.Rows[i][j].ToString();
                            break;
                    }
                }
            }
            //return;
            //read table data
            for (int i = 4; i < table.Rows.Count; ++i)
            {
                T rowData = new T();
                rowData.id = -1;
                for (int j = 0; j < table.Columns.Count; ++j)
                {
                    string fieldType = ts[j].fieldType;
                    string fieldNameClient = ts[j].fieldNameClient;
                    string data = table.Rows[i].ItemArray[j].ToString();
                    if (string.IsNullOrEmpty(data))
                        continue;
                    FieldInfo pi = rowData.GetType().GetField(fieldNameClient);
                    if (null == pi)
                    {
						Debug.LogError("error! not find fieldName=" + fieldNameClient + "in class=" + rowData.GetType().Name + " from table=" + path);
                        continue;
                    }
                    switch (fieldType)
                    {
                        case "int":
                            pi.SetValue(rowData, (object)(int.Parse(data)));
                            break;
                        case "string":
                            pi.SetValue(rowData, (object)data);
                            break;
                        case "float":
                            pi.SetValue(rowData, (object)(float.Parse(data)));
                            break;
                        case "array_int":
                            {
                                if (string.IsNullOrEmpty(data))
                                    break;
                                string[] array = data.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                                List<int> int_array = new List<int>();
                                for (int ii = 0; ii < array.Length; ++ii)
                                    int_array.Add(int.Parse(array[ii]));
                                pi.SetValue(rowData, (object)(int_array));
                            }
                            break;
                        case "array_string":
                            {
                                if (string.IsNullOrEmpty(data))
                                    break;
                                string[] array = data.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                                List<string> str_array = new List<string>();
                                for (int ii = 0; ii < array.Length; ++ii)
                                    str_array.Add(array[ii]);
                                pi.SetValue(rowData, (object)(str_array));
                            }
                            break;
                        case "array_float":
                            {
                                if (string.IsNullOrEmpty(data))
                                    break;
                                string[] array = data.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                                List<float> float_array = new List<float>();
                                for (int ii = 0; ii < array.Length; ++ii)
                                    float_array.Add(float.Parse(array[ii]));
                                pi.SetValue(rowData, (object)(float_array));
                            }
                            break;
                    }
                }
                if (rowData.id >= 0)
                {
                    if (dic.ContainsKey(rowData.id))
                    {
						Debug.LogError("error! duplicate id=" + rowData.id);
                    }
                    else
                        dic.Add(rowData.id, rowData);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////
        private static ExcelTableReader inst = null;
    }
}