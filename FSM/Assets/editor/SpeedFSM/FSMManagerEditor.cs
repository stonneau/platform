using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM.GUI
{
	[CustomEditor(typeof(FSMManager))]
	[System.Serializable]
	public class FSMManagerEditor : Editor {

	  	[SerializeField]
		private FSMManager manager_;
		 
		public void OnEnable()
		{
			if(manager_ == null)
			{
				manager_ = (FSMManager)target;
			}
		}
		
		public override void OnInspectorGUI()
		{	
			DrawDefaultInspector ();
			if (GUILayout.Button("Open FSM Editor", GUILayout.Width(200)))
			{   
				StateMachineEditorWindow window = (StateMachineEditorWindow) EditorWindow.GetWindow(typeof(StateMachineEditorWindow));
				window.Init(manager_.stateMachine);
			}
			SceneView.RepaintAll();
		}
	}
}
