using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Text;

public static class EditorUtils
{
	static public void ClearDeveloperConsole()
	{
		var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries, UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		clearMethod.Invoke(null, null);
	}

	[MenuItem("Tools/Clear Empty Folders")]
	static void ClearEmptyFolders()
	{
		ClearDeveloperConsole();

		Debug.Log("Checking for empty folders...");

		ClearEmptyFoldersRec(UnityEngine.Application.dataPath);
	}

	static bool ClearEmptyFoldersRec(string path)
	{
		string[] folders = Directory.GetDirectories(path);
		string[] files = Directory.GetFiles(path);

		bool hasFolders = (folders != null && folders.Length > 0);
		bool hasFiles = (files != null && files.Length > 0);

		if (!hasFolders && hasFiles && files.Length == 1 && files[0].ToLower().EndsWith(".ds_store"))
			hasFiles = false;

		if (!hasFolders && !hasFiles)
		{
			foreach (var f in files)
				File.Delete(f);
			
			return true;
		}

		if (hasFolders)
			for (int i = 0; i < folders.Length; i++)
			{
				if (ClearEmptyFoldersRec(folders[i]))
				{
					Debug.Log("Delete empty folder: " + folders[i]);

					Directory.Delete(folders[i]);

					AssetDatabase.Refresh();
				}
			}

		return false;
	}
}
