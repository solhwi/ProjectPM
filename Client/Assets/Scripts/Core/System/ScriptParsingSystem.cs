using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class ScriptParsingSystem : MonoSystem
{
    [SerializeField] private AddressableResourceSystem resourceSystem = null;
	private Dictionary<Type, ScriptParser> dictionary = new Dictionary<Type, ScriptParser>();

    protected override void OnReset()
    {
        base.OnReset();

		resourceSystem = AssetLoadHelper.GetSystemAsset<AddressableResourceSystem>();
    }

    public T GetTable<T> () where T : ScriptParser
	{
		var key = typeof(T);

		if (dictionary.TryGetValue(key, out var table) == false)
			return null;

		return table as T;
	}

	public async UniTask<IEnumerable<ScriptParser>> LoadAsyncScriptParsers()
	{
		var types = Utility.GetSubClassTypes<ScriptParser>();
		dictionary.Clear();

        foreach (var type in types)
		{
			var scriptParser = await resourceSystem.LoadAsync<ScriptParser>(type);
			if (scriptParser == null)
				continue;

			scriptParser.RuntimeParser();
			dictionary.Add(type, scriptParser);
		}

		return dictionary.Values;

    }
}
