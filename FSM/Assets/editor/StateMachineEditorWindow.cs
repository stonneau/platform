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

	public class StateMachineEditorWindow : EditorWindow
	{
		private StateMachine machine_;
		private List<StateGUI> states_ = new List<StateGUI>();
		float initX = 150f;
		float initY = 150f;

		// index of currently dragged object
		int currentlyDragged = -1;
		// index of currentTransitionOrigin
		int currentOutput = -1;
		private Tran currentTransition = null; 

		// Use this for initialization
		public void Init(StateMachine machine)
		{
			if(machine_ == null)
			{
				machine_ = machine;
				Dictionary<State, int> indexes_ = new Dictionary<State, int>();
				int i = 0;
				foreach(State state in machine_.states_)
				{
					StateGUI stateGUI = CreateInstance<StateGUI>();
					stateGUI.state_ = state;
					stateGUI.MoveTo(state.location, states_);
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
			Vector2 position = new Vector2(initX, initY);
			StateGUI stateGUI = CreateInstance<StateGUI>();
			stateGUI.state_ = machine_.AddState(position);
			stateGUI.MoveTo(position, states_);
			states_.Add(stateGUI);
			initX += 100; initY += 100;
		}

		private static void SaveFSM(StateMachine machine)
		{
			AssetDatabase.CreateAsset(machine, "Assets/fsm.asset");
		}

		public void OnGUI()
		{	
			if (GUILayout.Button ("Add State"))
				AddState();
			HandleMouseEvents();
			DrawStates();
			this.Repaint();
			//transitions_.Clear();
		}
#region mouseEvents
		private void HandleMouseEvents()
		{
			wantsMouseMove = true;
			Event e = Event.current;
			if(e == null) return;
			if (MoveState(e)) return;
			if (DrawTransition(e)) return;
		}

		private bool NothingGoingOn()
		{
			return currentOutput < 0 && currentlyDragged < 0;
		}

		private bool DrawTransition(Event e)
		{
			bool res = false;
			if(e.type == EventType.MouseDrag)
			{
				if(NothingGoingOn())
				{
					int i = 0;
					foreach(StateGUI state in states_)
					{
						if(state.GrabOutput(e.mousePosition))
						{
							currentOutput = i;
							res = true;
							break;
						}
						++i;
					}
				}
				else if(currentOutput >= 0)
				{
					currentTransition = new Tran(states_[currentOutput].OutputOrigin, e.mousePosition);
				}
			}
			else if((e.type == EventType.MouseUp) || (e.type == EventType.MouseDown))
			{
				if(currentOutput >= 0)
				{
					foreach(StateGUI state in states_)
					{
						if(state != states_[currentOutput] && state.Grab(e.mousePosition))
						{
							states_[currentOutput].AddTransition(state);
							//machine_.AddTransition(currentOutput, state);
							res = true;
							break;
						}
					}
					currentOutput = -1;
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
					int i = 0;
					foreach(StateGUI state in states_)
					{
						if(state.GrabState(e.mousePosition))
						{
							moved = state.Move(e.delta, states_);
							currentlyDragged = i;
							res = true;
							break;
						}
						++i;
					}
				}
				else if(currentlyDragged >= 0)
				{
					moved = states_[currentlyDragged].Move(e.delta, states_);
					res = true;
				}
				if(res &! moved)
				{
					states_[currentlyDragged].MoveTo(e.mousePosition, states_);
				}
			}
			else if((e.type == EventType.MouseUp) || (e.type == EventType.MouseDown))
			{
				currentlyDragged = -1;
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
			foreach(StateGUI state in states_)
			{
				state.Draw();
			}
			if(currentTransition != null)
			{
				Drawing.DrawLine(currentTransition.from_, currentTransition.to_, Color.red, 2, false);
			}
		}
	}
}
