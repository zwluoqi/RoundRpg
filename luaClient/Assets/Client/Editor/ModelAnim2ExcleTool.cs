using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
using UnityHelp;



    public class ModelAnim2ExcleTool
    {



	string xlsxPath = Application.dataPath + "/../../tools/xls2lua/excles/hero/hero_animation.xls";

		private TableStruct[] GetAnimLengthXlsx(){
			TableStruct[] tss = new TableStruct[4];

			TableStruct ts = new TableStruct ();
			ts.fieldDesc = "主键ID";
			ts.fieldNameClient = "id";
			ts.fieldNameServer = "id";
			ts.fieldType = "int";
			ts.primary = "primary";
			tss [0] = ts;

			ts = new TableStruct ();
			ts.fieldDesc = "模型名称";
			ts.fieldNameClient = "modelName";
			ts.fieldNameServer = "modelName";
			ts.fieldType = "string";
			ts.primary = "index";
			tss [1] = ts;

			ts = new TableStruct ();
			ts.fieldDesc = "动画名称";
			ts.fieldNameClient = "animName";
			ts.fieldNameServer = "animName";
			ts.fieldType = "string";
			tss [2] = ts;

			ts = new TableStruct ();
			ts.fieldDesc = "动画时长";
			ts.fieldNameClient = "animLength";
			ts.fieldNameServer = "animLength";
			ts.fieldType = "float";
			tss [3] = ts;
			return tss;
		}

	public void Build(string parentSrcDir)
        {
		Dictionary<int,AnimLengthData> animLengths = ReadModel(parentSrcDir);


			ExcelTableReader.Inst ().WriteTable (xlsxPath, "_", animLengths, GetAnimLengthXlsx ());
            
		sb.AppendLine (xlsxPath);
			Debug.LogWarning (sb.ToString());

        }


//        public List<RoleResData> GetDataList()
//        {
//            List<RoleResData> resu = new List<RoleResData>();
//
//            RoleResData data = null;
//            DictDataManager.Instance.dictCombineResRole.Load("Dict/",null);
//            foreach (DictCombineResRole.Model row in DictDataManager.Instance.dictCombineResRole.Dict.Values)
//            {
//                resu.Add(data = new RoleResData());
//                data.model = row;
//            }
//            resu.Sort((a, b) =>
//            {
//                return string.Compare(a.model.prefab, b.model.prefab);
//            });
//            return resu;
//        }



	private Dictionary<int,AnimLengthData> ReadModel(string srcDir)
        {
			Dictionary<int,AnimLengthData> dicts  = new Dictionary<int, AnimLengthData>();


			srcDir = string.Concat("Assets/", srcDir);

//			DirectoryInfo dirInfo = new DirectoryInfo(srcDir);

				
			BuildAnimData(ref dicts, srcDir);

			return dicts;
        }


	private void BuildAnimDir(ref Dictionary<int,AnimLengthData> dicts , string srcDir, DirectoryInfo dirInfo){

//		string currPrefabName = "";
//		string currPrefabAnimPath = "";
//
//		DirectoryInfo[] dirs = dirInfo.GetDirectories ();
//		foreach (DirectoryInfo dir in dirs) {
//			currPrefabName = dir.Name;
//			currPrefabAnimPath = srcDir + "/" + dir.Name;
//			BuildAnimData(ref dicts,currPrefabName, currPrefabAnimPath);
//			BuildAnimDir (ref dicts, currPrefabAnimPath, dir);
//		}
	}

		public StringBuilder sb = new StringBuilder();
        public int sum = 1;
		public void BuildAnimData(ref Dictionary<int,AnimLengthData> dicts, string currAnimPath)
        {
            string[] animsGUID = AssetDatabase.FindAssets("t:AnimationClip", new string[] { currAnimPath });
            for (int animIter = 0; animIter < animsGUID.Length; ++animIter)
            {
                string animDir = AssetDatabase.GUIDToAssetPath(animsGUID[animIter]);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animDir);
                if (clip != null)
                {
				string[] splits = animDir.Split (new char[]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
				string currPrefabName = "";
				if (splits.Length > 1) {
					currPrefabName = splits [splits.Length - 2];
				}
				if (currPrefabName == "" || currPrefabName == "ani") {
					if (splits.Length > 2) {
						currPrefabName = splits [splits.Length - 3];
					}
				}

					dicts [sum] = new AnimLengthData ();
					dicts [sum].id = sum;
					dicts [sum].animLength = clip.length;
					dicts [sum].modelName = currPrefabName;
					dicts [sum].animName = clip.name;

                    string data = (++sum) + "," + currPrefabName + "," + clip.name + "," + clip.length;
					sb.AppendLine (data);
                }
            }
        }


		public class AnimLengthData:UnityHelp.DataBase{
				public string modelName;
				public string animName;
				public float animLength;
		}
    }

