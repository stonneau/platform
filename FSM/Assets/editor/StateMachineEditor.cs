using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM.GUI
{
	[CustomEditor(typeof(StateMachine))]
	[System.Serializable]
	public class StateMachineEditor : Editor {

	  	[SerializeField]
		private StateMachine stateMachine_;
		 
		public void OnEnable()
		{
			if(stateMachine_ == null)
			{
				stateMachine_ = (StateMachine)target;
			}
		}
		
		public override void OnInspectorGUI()
		{	
			if (GUILayout.Button("Open FSM Editor", GUILayout.Width(200)))
			{   
				StateMachineEditorWindow window = (StateMachineEditorWindow) EditorWindow.GetWindow(typeof(StateMachineEditorWindow));
				window.Init(stateMachine_);
			}
			SceneView.RepaintAll();
		}
	}
}
