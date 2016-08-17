using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ParseRequest
{

}

[Serializable]
public class ParseResponse
{

}

[Serializable]
public class ParseErrorResponse
{
	public int code = 0;

	public string error = null;

	public T Decode<T>()
	{
		if (!error.StartsWith("{"))
			return default(T);
		
		return JsonUtility.FromJson<T>(error);
	}
}

public class Parse
{
	[Serializable]
	class ParseResponseHandler<T>
	{
		public T result = default(T);
	}

	class ParseDummy : MonoBehaviour
	{
		
	}

	//Does not change, create in constructor and mark read-only.
	readonly Dictionary<string, string> Headers;

	readonly ParseDummy behaviour = null;

	readonly ParseSettings settings = null;

	private Parse()
	{
		settings = Resources.Load<ParseSettings>("Parse");

		if (settings == null)
		{
			settings = ScriptableObject.CreateInstance<ParseSettings>();

			#if UNITY_EDITOR

			AssetDatabase.CreateAsset(settings, "Assets/Resources/Parse.asset");
			AssetDatabase.SaveAssets();

			Debug.LogError("Fill in your information in Resources/Parse.asset");

			#endif
		}

		GameObject go = new GameObject();
		go.name = "[Parse]";
		behaviour = go.AddComponent<ParseDummy>();
		GameObject.DontDestroyOnLoad(go);

		Headers = new Dictionary<string, string>()
		{
			{ "X-Parse-Application-Id", settings.AppId },
			{ "X-Parse-REST-API-Key", settings.RESTKey },
			{ "Content-Type", "application/json" }
		};
	}

	public YieldInstruction CloudCode<T_Request, T_Response>(string function, T_Request req, System.Action<T_Response> onComplete, System.Action<ParseErrorResponse> onError = null) 
		where T_Request : ParseRequest
		where T_Response : ParseResponse
	{
		return behaviour.StartCoroutine(CoRequest<T_Request, T_Response>(function, req, onComplete, onError));
	}

	IEnumerator CoRequest<T_Request, T_Response>(string function, T_Request req, System.Action<T_Response> onComplete, System.Action<ParseErrorResponse> onError)
		where T_Request : ParseRequest
		where T_Response : ParseResponse
	{
		string json = JsonUtility.ToJson(req);

		Debug.Log("[Parse] Request to '" + function + "' with data: " + json);

		WWW www = new WWW(settings.Host + "/functions/" + function, Encoding.UTF8.GetBytes(json), Headers);

		yield return www;

		if (!String.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("[Parse] Error from '" + function + "' with data: " + www.text);

			if (www.text.StartsWith("{"))
			{
				ParseErrorResponse resp = JsonUtility.FromJson<ParseErrorResponse>(www.text);

				if (onError != null)
					onError(resp);
			}
			else
			{
				ParseErrorResponse resp = new ParseErrorResponse{ code = 0, error = "No answer from server." };

				if (onError != null)
					onError(resp);
			}
		}
		else
		{
			Debug.Log("[Parse] Response from '" + function + "' with data: " + www.text);

			ParseResponseHandler<T_Response> resp = JsonUtility.FromJson<ParseResponseHandler<T_Response>>(www.text);

			if (onComplete != null)
				onComplete(resp.result);
		}
	}

	#region SINGLETON

	static Parse instance = null;

	public static Parse Instance
	{
		get
		{
			if (instance == null)
				instance = new Parse();

			return instance;
		}
	}

	#endregion
}

/*
[Serializable]
public class ParseRequestList : ParseRequest
{
	public string id;
}

[Serializable]
public class ParseResponseList : ParseResponse
{
	[Serializable]
	public class ParseResponseListItem
	{
		public string gameId;
	}

	public ParseResponseListItem[] result;
}

Example1,

ParseRequestList req = new ParseRequestList(){ id = "uhALEnmaaZ" };

Instance.CloudCode<ParseRequestList, ParseResponseList>("list", req, (result) =>
	{
		Log.D(result.result[0].gameId);
	}, null);

Example2,

ParseRequestList req = new ParseRequestList(){ id = "uhALEnmaaZ" };

ParseResponseList resp = null;

yield return Parse.Instance.CloudCode<ParseRequestList, ParseResponseList>("list", req, (result) =>
	{
		resp = result;
	});

if (resp != null)
	Debug.Log("answer " + resp.result[0].gameId);
else
	Debug.LogWarning("error");
*/