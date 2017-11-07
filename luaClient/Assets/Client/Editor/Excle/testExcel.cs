using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Data;
using System.Text;

public class testExcel : EditorWindow
{
	[MenuItem("Totem_H1/testExcel")]
    static void OpenWindow()
    {
		ExcelHelper excel = new ExcelHelper(Application.dataPath + "/test.xls");
		DataTable data = excel.ExcelToDataTable("_", true);
		StringBuilder sr = new StringBuilder();
		for (int i = 0; i < data.Rows.Count; ++i)
		{
			sr = new StringBuilder ();
			for (int j = 0; j < data.Rows [i].ItemArray.Length; ++j)
				sr.Append (data.Rows [i].ItemArray [j] + ",");
			//Debug.LogError(data.Rows[i].ItemArray[j].ToString());
			Debug.LogWarning(sr.ToString());
		}
	}

}
