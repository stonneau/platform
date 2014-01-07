using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SpeedFSM;
using SpeedFSM.GUI;

namespace SpeedFSM.GUI
{
	public class FSMCommand
	{
		private StateMachineEditorWindow window_;

		public FSMCommand(StateMachineEditorWindow window)
		{
			window_ = window;
		}

		public void AddState(Vector2 position)
		{
			State s = ScriptableObject.CreateInstance<State>();
			Undo.RegisterCreatedObjectUndo(s, "add state");
			Undo.RecordObject(s, "add state");
			s.x = position.x;
			s.y = position.y;
			Undo.RecordObject(window_.machine_, "add state"); 
			window_.machine_.states_.Add(s);
			Undo.RegisterCreatedObjectUndo(s, "add state");
			StateGUI stateGUI = StateGUI.CreateInstance<StateGUI>();
			stateGUI.Init(s);
			Undo.RegisterCreatedObjectUndo(stateGUI, "add state");
			Undo.RecordObject(window_.stateMachineGUI_, "add state");
			window_.states_.Add(stateGUI);
			EditorUtility.SetDirty(window_.stateMachineGUI_);
		}

		public void RemoveState(int index)
		{
			StateGUI stateGUI = window_.states_[index];
			Undo.RecordObject(window_.stateMachineGUI_, "remove state");
			window_.states_.RemoveAt(index);
			Undo.RecordObject(window_.machine_, "remove state");
			foreach(State state in window_.machine_.states_)
			{
				if(state != stateGUI.state_)
					Undo.RecordObject(state, "remove state");
			}
			window_.machine_.RemoveState(stateGUI.state_);
			Undo.DestroyObjectImmediate(stateGUI);
			EditorUtility.SetDirty(window_.stateMachineGUI_);
		}

		public void AddTransition(StateGUI from, StateGUI to)
		{
			Undo.RecordObject(window_.stateMachineGUI_, "add transition");
			Undo.RecordObject(from, "add transition");
			Undo.RecordObject(from.state_, "add transition");
			from.AddTransition(to);
			EditorUtility.SetDirty(window_.stateMachineGUI_);
		}

		public void SelectState(int index)
		{
			Undo.RecordObject(window_.stateMachineGUI_, "select state");
			window_.stateMachineGUI_.currentStateSelected = index;
			if(index > 0 && index < window_.states_.Count)
			{
				Selection.activeObject = window_.states_[index].state_;
			}
		}

	}
}

