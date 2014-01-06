using UnityEngine;
using UnityEditor;
using System.IO;
using SpeedFSM;

public static class CustomAssetUtility
{
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension (path) != "")
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		//EditorUtility.FocusProjectWindow ();
		//Selection.activeObject = asset;
	}

	public static void CreateStateMachineAsset(StateMachine asset)
	{
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "")
		{
			path = "Assets/StateMachines";
		}
		else if (Path.GetExtension (path) != "")
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "stateMachine.asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		foreach(State state in asset.states_)
		{
			AssetDatabase.AddObjectToAsset(state, asset);
			foreach(State transition in state.transitions_)
			{
				AssetDatabase.AddObjectToAsset(transition, asset);
			}
		}
		AssetDatabase.SaveAssets ();
	}
}