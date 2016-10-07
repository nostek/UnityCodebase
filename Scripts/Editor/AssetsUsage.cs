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

	[MenuItem("Assets/Find Where Used In Project", false, 30)]
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

		List<string> files = new List<string>();
		files.AddRange(Directory.GetFiles(UnityEngine.Application.dataPath, "*.prefab", SearchOption.AllDirectories));
		files.AddRange(Directory.GetFiles(UnityEngine.Application.dataPath, "*.unity", SearchOption.AllDirectories));
		files.AddRange(Directory.GetFiles(UnityEngine.Application.dataPath, "*.controller", SearchOption.AllDirectories));

		for (int i = 0; i < files.Count; i++)
		{
			string content = File.ReadAllText(files[i]);

			if (!content.StartsWith("%YAML"))
				continue;

			for (int j = 0; j < models.Length; j++)
				if (content.Contains(models[j].Guid))
					models[j].Files.Add(files[i]);
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine("Search Report");
		sb.AppendLine("");

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
}
