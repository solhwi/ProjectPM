using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptParserAttribute : ResourceAttribute
{
	public ScriptParserAttribute(string path) : base("Datas/Parser/" + path, ResourceType.UnityAsset)
	{

	}
}

public class ScriptAssetInfo
{
	public Type AssetType { get; set; }
	public ScriptParserAttribute Attribute { get; set; }
}

public abstract class ScriptParser : ScriptableObject
{
	// �⺻ Parser, List, Dictionary�� ����� ��
	public abstract void Parser();
}