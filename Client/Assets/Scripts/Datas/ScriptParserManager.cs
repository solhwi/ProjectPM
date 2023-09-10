using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScriptParserManager : SingletonComponent<ScriptParserManager>
{
	[System.Serializable]
	public class ScriptParserDictionary : SerializableDictionary<Type, ScriptParser> { }
	private ScriptParserDictionary dictionary = new ScriptParserDictionary();

	public T GetTable<T> () where T : ScriptParser
	{
		var key = typeof(T);

		if (dictionary.TryGetValue(key, out var table) == false)
			return null;

		return table as T;
	}

	public IEnumerator LoadAsyncScriptParsers()
	{
		var types = FMUtil.GetSubClassTypes<ScriptParser>();

		foreach(var type in types)
		{
			var handle = ResourceManager.Instance.LoadAsync<ScriptParser>(type);
			while (!handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded)
			{
				yield return null;
			}

			var scriptParser = handle.Result as ScriptParser;
			if (scriptParser == null)
				yield break;

			scriptParser.RuntimeParser();

			dictionary.Add(type, scriptParser);
		}
	}

}
