using UnityEngine;
using UnityEditor;

static class PlayerUnityIntegration {

	[MenuItem("Assets/Create/PlayerAsset")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility2.CreateAsset<PlayerAsset>();
	}

}
