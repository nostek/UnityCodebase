using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class FileLoadSave<T>
	where T : class, new()
{
	const string FilePath = "Assets/Resources/";
	const string FileExt = ".json";

	string fileName = null;
	string remoteUrl = null;

	T data = null;

	bool isLoaded = false;

	public FileLoadSave(string fileName, string remoteUrl = null)
	{
		this.fileName = fileName;
		this.remoteUrl = remoteUrl;
	}

	public bool IsLoaded
	{
		get
		{
			return isLoaded;
		}
	}

	public T Data
	{
		get
		{
			return data;
		}
	}

	public void Load()
	{
		isLoaded = false;

		if (remoteUrl != null)
			Log.W("Can not load remote data with Load, use LoadAsync.");

		TextAsset text = Resources.Load<TextAsset>(fileName);

		isLoaded = true;

		if (text == null)
			return;

		data = JsonUtility.FromJson<T>(text.text);
	}

	public void LoadAsync(MonoBehaviour behaviour)
	{
		behaviour.StartCoroutine(LoadAsyncNow());
	}

	IEnumerator LoadAsyncNow()
	{
		if (remoteUrl == null)
		{
			ResourceRequest json = Resources.LoadAsync<TextAsset>(fileName);

			yield return new WaitUntil(() => json.isDone);

			isLoaded = true;

			Log.D("Load complete:", fileName);

			TextAsset text = (TextAsset)json.asset;

			if (text == null)
				yield break;

			data = JsonUtility.FromJson<T>(text.text);
		}
		else
		{
			WWW json = new WWW(remoteUrl);

			yield return new WaitUntil(() => json.isDone);

			isLoaded = true;

			Log.D("Remote complete:", remoteUrl);

			if (string.IsNullOrEmpty(json.text))
				yield break;

			data = JsonUtility.FromJson<T>(json.text);
		}
	}

	public void Save()
	{
		string json = JsonUtility.ToJson(data, true);

		using (FileStream fs = new FileStream(SavePath, FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(fs))
			{
				writer.Write(json);		
			}	
		}

		#if UNITY_EDITOR

		if (!UnityEditor.EditorApplication.isPlaying)
		{
			UnityEditor.AssetDatabase.Refresh();
		}

		#endif
	}

	string SavePath
	{
		get
		{
			return FilePath + fileName + FileExt;
		}
	}
}
