using UnityEngine;
using UnityEditor;

static class TileUnityIntegration {

	[MenuItem("Assets/Create/TileAsset")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility2.CreateAsset<TileAsset>();
	}

}
