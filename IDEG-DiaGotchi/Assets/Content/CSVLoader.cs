using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVLoader
{
	static string SPLIT_RE = @";(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	public static List<Dictionary<string, string>> ParseCSV(string fileContents)
	{
		var list = new List<Dictionary<string, string>>();

		var lines = Regex.Split(fileContents, LINE_SPLIT_RE);

		if (lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++)
		{

			var values = Regex.Split(lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "") continue;

			var entry = new Dictionary<string, string>();
			for (var j = 0; j < header.Length && j < values.Length; j++)
			{
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				string finalvalue = value;
				entry[header[j]] = finalvalue;
			}
			list.Add(entry);
		}
		return list;
	}

	public static List<Dictionary<string, string>> ReadResourceCSV(string assetFilePath)
	{
		TextAsset txt = Resources.Load(assetFilePath) as TextAsset;

		var parsed = CSVLoader.ParseCSV(txt.text);

		return parsed;
	}

	public static List<Dictionary<string, string>> ReadExternalAssetCSV(string assetFilePath)
	{
		string basePath = AppDomain.CurrentDomain.BaseDirectory;
#if UNITY_EDITOR
        string assetsPath = Application.dataPath;
        basePath = assetsPath.Replace("/Assets", "");
#endif
		var str = File.ReadAllText(basePath + "/ExternalData/" + assetFilePath);
		var parsed = CSVLoader.ParseCSV(str);

        return parsed;
    }
}
