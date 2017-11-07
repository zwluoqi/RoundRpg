using System;
using System.Collections.Generic;
using UnityEngine;




    public class StringUtil
    {

        public static string[] GetSplitString(string source, string split)
        {
            char[] sp = { split[0] };
            string[] data = source.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
            return data;
        }

        public static Dictionary<string, string> GetDynamicParams(List<string> sourceParam)
        {
            Dictionary<string, string> vals = new Dictionary<string, string>();
            foreach (var data in sourceParam)
            {
                string[] key_val = StringUtil.GetSplitString(data, ":");
                vals[key_val[0]] = key_val[1];
            }
            return vals;
        }


    }

