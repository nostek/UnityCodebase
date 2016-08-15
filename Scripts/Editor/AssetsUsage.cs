using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public static class AssetsUsage
{
	class UsageModel
	{
		public string Guid;
		public List<string> Files;
	}

	[MenuItem("Assets/Check Asset Usage", false, 30)]
	static void CheckUsage()
	{
		float start = Time.realtimeSinceStartup;

		UnityEngine.Object[] objs = Selection.objects;

		UsageModel[] models = new UsageModel[objs.Length];

		for (int i = 0; i < objs.Length; i++)
		{
			string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objs[i]));

			models[i] = new UsageModel(){ Guid = string.Format("guid: {0}", guid), Files = new List<string>() };
		}

		CheckUsageRecursive(models, UnityEngine.Application.dataPath);

		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < models.Length; i++)
		{
			sb.AppendFormat("Searching for {0}...\n", objs[i].name);

			for (int j = 0; j < models[i].Files.Count; j++)
				sb.AppendFormat("Found in: {0}\n", models[i].Files[j]);

			sb.AppendLine("");
		}

		float stop = Time.realtimeSinceStartup;

		sb.AppendFormat("Elapsed time: {0}\n", (stop - start));

		Debug.Log(sb.ToString());
	}

	static void CheckUsageRecursive(UsageModel[] models, string path)
	{
		string[] folders = Directory.GetDirectories(path);
		string[] files = Directory.GetFiles(path);

		bool hasFolders = (folders != null && folders.Length > 0);
		bool hasFiles = (files != null && files.Length > 0);

		if (hasFiles)
		{
			for (int i = 0; i < files.Length; i++)
			{
				bool usable = (	
				                  files[i].ToLower().EndsWith(".prefab") ||
				                  files[i].ToLower().EndsWith(".unity") ||
				                  files[i].ToLower().EndsWith(".controller")
				              );

				if (!usable)
					continue;

				string content = File.ReadAllText(files[i]);

				if (!content.StartsWith("%YAML"))
					continue;

				for (int j = 0; j < models.Length; j++)
					if (content.Contains(models[j].Guid))
						models[j].Files.Add(files[i]);
			}
		}

		if (hasFolders)
			for (int i = 0; i < folders.Length; i++)
				CheckUsageRecursive(models, folders[i]);
	}
}
