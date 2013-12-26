using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SpeedFSM
{
[System.Serializable] 
[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor {
	
	public override void OnInspectorGUI()
	{	
		if (GUILayout.Button("Open FSM Editor", GUILayout.Width(200)))
		{   
			StateMachineEditorWindow window = (StateMachineEditorWindow) EditorWindow.GetWindow(typeof(StateMachineEditorWindow));
			window.Init();
		}
		SceneView.RepaintAll();
	}
}
}
