using System;
using System.Collections.Generic;
using System.IO;



public class ReadyConfig
{
	public ReadyConfig ()
	{
	}

	public Dictionary<string,string> datas = new Dictionary<string, string> ();

	public void ReadFile(string filePath){

		StreamReader sr = new StreamReader (filePath);

		char[] splits = new char[]{':'};

		string line = sr.ReadLine ();
		while (!string.IsNullOrEmpty(line)) {
			
			string[] strs = line.Split (splits, StringSplitOptions.RemoveEmptyEntries);
			datas [strs [0]] = strs [1];
			line = sr.ReadLine ();

		}


	}


	public static Dictionary<string ,ReadyConfig> reads = new Dictionary<string, ReadyConfig>();

	public static ReadyConfig GetReadConfig(string filePath){
		if (!reads.ContainsKey (filePath)) {
			reads [filePath] = new ReadyConfig ();
			reads [filePath].ReadFile (filePath);
		}
		return reads [filePath];
	}
}