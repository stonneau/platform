using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM
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

	public class StateMachineEditorWindow : EditorWindow {
	
		private List<StateGUI> states_ = new List<StateGUI>();
		// index of currently dragged object
		int currentlyDragged = -1;
		// index of currentTransitionOrigin
		int currentOutput = -1;
		private Tran currentTransition = null; 

		// Use this for initialization
		public void Init()
		{
			State s = new State();
			states_.Add(new StateGUI(s, new Vector2(300, 200)));

			State s2 = new State();
			states_.Add(new StateGUI(s2, new Vector2(60, 50)));

			
			State s3 = new State();
			states_.Add(new StateGUI(s3, new Vector2(200, 50)));
		}

		public void OnGUI()
		{	
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
						if(state != states_[currentOutput] && state.GrabInput(e.mousePosition))
						{
							states_[currentOutput].Addtransition(state);
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
