using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM.GUI
{
	class Tran
	{
		public Tran(Vector2 from, Vector2 to)
		{
			from_ = from; to_ = to;
		}
		public Vector2 from_;
		public Vector2 to_;
	}

	[System.Serializable]
	internal class StateMachineGUI : ScriptableObject
	{
		public List<StateGUI> states_ = new List<StateGUI>();
		// index of currently dragged object
		public int currentlyDragged = -1;
		// index of currentTransitionOrigin
		public int currentOutput = -1;
		// index of currentTransitionOrigin
		public int currentStateSelected = -1;

		void OnEnable()
		{
			if(states_ == null)
				states_ = new List<StateGUI>();
		}

		public int GrabState(Vector2 mousePos)
		{
			int i =0;
			foreach(StateGUI state in states_)
			{
				if(state.Grab(mousePos))
				{
					return i;
				}
				++i;
			}
			return -1;
		}

		public int GrabOutput(Vector2 mousePos)
		{
			int i =0;
			foreach(StateGUI state in states_)
			{
				if(state.GrabOutput(mousePos))
				{
					return i;
				}
				++i;
			}
			return -1;
		}
	}

	public class StateMachineEditorWindow : EditorWindow
	{
		private FSMCommand fsmCommand_;
		internal StateMachineGUI stateMachineGUI_;
		internal StateMachine machine_;

		public List<StateGUI> states_
		{
			get {return stateMachineGUI_.states_;}
		}

		float initX = 150f;
		float initY = 150f;

		private Tran currentTransition = null; 

		// Use this for initialization
		public void Init(StateMachine machine)
		{
			stateMachineGUI_ = StateMachineGUI.CreateInstance<StateMachineGUI>();
			if(fsmCommand_ == null)
			{
				fsmCommand_ = new FSMCommand(this);
			}
			if(machine_ == null)
			{
				machine_ = machine;
				Dictionary<State, int> indexes_ = new Dictionary<State, int>();
				int i = 0;
				foreach(State state in machine_.states_)
				{
					StateGUI stateGUI = StateGUI.CreateInstance<StateGUI>();
					stateGUI.Init(state);
					states_.Add(stateGUI);
					indexes_.Add(state, i);
					++i;
				}
				foreach(KeyValuePair<State, int> entry in indexes_)
				{
					foreach(State state in entry.Key.transitions_)
					{
						if(indexes_.ContainsKey(state))
						{
							states_[entry.Value].AddTransition(states_[indexes_[state]]);
						}
					}
				}
			}
		}

		private void AddState()
		{
			fsmCommand_.AddState(new Vector2(initX, initY));
			initX += 100; initY += 100;
		}

		private static void SaveFSM(StateMachine machine)
		{
			CustomAssetUtility.SaveAsset(machine);
		}

		public void OnGUI()
		{	
			if (GUILayout.Button ("Add State"))
				AddState();
			if (GUILayout.Button ("Save FSM"))
				SaveFSM(machine_);
			HandleMouseEvents();
			DrawStates();
			this.Repaint();
		}
#region mouseEvents
		private void HandleMouseEvents()
		{
			wantsMouseMove = true;
			Event e = Event.current;
			if(e == null) return;
			if (DrawTransition(e)) return;
			if (MoveState(e)) return;
			if (SelectNode(e)) return;
		}

		private bool NothingGoingOn()
		{
			return stateMachineGUI_.currentOutput < 0 && stateMachineGUI_.currentlyDragged < 0;
		}

		private bool SelectNode(Event e)
		{
			if((e.type == EventType.MouseUp) || (e.type == EventType.MouseDown))
			{
				int i = stateMachineGUI_.GrabState(e.mousePosition);
				if(i > -1 && i != stateMachineGUI_.currentStateSelected)
				{
					fsmCommand_.SelectState(i);
					return true;
				}
			}
			return false;
		}
		


		private bool DrawTransition(Event e)
		{
			bool res = false;
			if(e.type == EventType.MouseDrag)
			{
				if(NothingGoingOn())
				{
					int i = stateMachineGUI_.GrabOutput(e.mousePosition);
					if(i > -1)
					{
						stateMachineGUI_.currentOutput = i;
						res = true;
					}
				}
				else if(stateMachineGUI_.currentOutput >= 0)
				{
					currentTransition = new Tran(states_[stateMachineGUI_.currentOutput].OutputOrigin, e.mousePosition);
				}
			}
			else if((e.type == EventType.MouseUp) || (e.type == EventType.MouseDown))
			{
				if(stateMachineGUI_.currentOutput >= 0)
				{
					int i = stateMachineGUI_.GrabState(e.mousePosition);
					if(i > -1 && states_[stateMachineGUI_.currentOutput] != states_[i])
					{
						fsmCommand_.AddTransition(states_[stateMachineGUI_.currentOutput],states_[i]);
						res = true;
					}
					stateMachineGUI_.currentOutput = -1;
				}
				currentTransition = null;
			}
			return res;
		}

		private bool MoveState(Event e)
		{
			bool res = false;
			bool moved = false;
			if(e.type == EventType.MouseDrag)
			{
				if(NothingGoingOn())
				{
					int i = stateMachineGUI_.GrabState(e.mousePosition);
					if(i > -1)
					{
						moved = states_[i].Move(e.delta, states_);
						stateMachineGUI_.currentlyDragged = i;
						res = true;
					}
				}
				else if(stateMachineGUI_.currentlyDragged >= 0)
				{
					moved = states_[stateMachineGUI_.currentlyDragged].Move(e.delta, states_);
					res = true;
				}
				if(res &! moved)
				{
					states_[stateMachineGUI_.currentlyDragged].MoveTo(e.mousePosition, states_);
				}
			}
			else if((e.type == EventType.MouseUp) || (e.type == EventType.MouseDown))
			{
				stateMachineGUI_.currentlyDragged = -1;
			}
			return res;
		}
#endregion //mouseEvents
		

		private void DrawStates()
		{
			if(states_ == null) return;
			foreach(StateGUI state in states_)
			{
				state.DrawTransitions();
			}
			int i = 0;
			foreach(StateGUI state in states_)
			{
				state.Draw(i==stateMachineGUI_.currentStateSelected);
				++i;
			}
			if(currentTransition != null)
			{
				Drawing.DrawLine(currentTransition.from_, currentTransition.to_, Color.red, 2, false);
			}
		}
	}
}
