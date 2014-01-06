using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM.GUI
{
	public class StateGUI
	{
		private Rect position_;
		private List<StateGUI> transitions_;
		public State state_;

		private const int initX = 50;
		private const int initY = 20;
		private const int transitionWidth = 5;

		public StateGUI(State state)
		{
			state_ = state;
			transitions_ = new List<StateGUI>();
			position_ = new Rect(state.x, state.y, initX, initY);
		}

		public Vector2 InputOrigin      
		{
			get { return new Vector2(position_.xMin, position_.center.y); }
		}

		public Vector2 OutputOrigin      
		{
			get { return new Vector2(position_.xMax, position_.center.y); }
		}

		public void AddTransition(StateGUI state)
		{
			if(!transitions_.Contains(state))
			{
				transitions_.Add(state);
				state_.AddTransition(state.state_);
			}
		}

		public bool Move(Vector2 translation, List<StateGUI> states)
		{
			Rect rect = new Rect(position_);
			rect.center += translation;
			foreach(StateGUI state in states)
			{
				if(state!=this && state.position_.Overlaps(rect))
				{
					return false;
				}
			}
			position_.center += translation;
			state_.x = position_.x;
			state_.y = position_.y;
			return true;
		}

		public bool MoveTo(Vector2 position, List<StateGUI> states)
		{
			Rect rect = new Rect(position_);
			rect.center = position;
			foreach(StateGUI state in states)
			{
				if(state!=this && state.position_.Overlaps(rect))
				{
					return false;
				}
			}
			position_.center = position;
			state_.x = position.x;
			state_.y = position.y;
			return true;
		}
		
		public bool Grab(Vector2 mousePosition)
		{
			return position_.Contains(mousePosition);
		}
		
		public bool GrabState(Vector2 mousePosition)
		{
			Rect state = new Rect(position_.xMin + transitionWidth, position_.yMin, position_.width - 2 * transitionWidth, position_.height);
			return state.Contains(mousePosition);
		}

		public bool GrabInput(Vector2 mousePosition)
		{
			Rect input = new Rect(position_.xMin, position_.yMin, transitionWidth, position_.height);
			return input.Contains(mousePosition);
		}

		public bool GrabOutput(Vector2 mousePosition)
		{
			Rect output = new Rect(position_.xMax - transitionWidth, position_.yMin, transitionWidth, position_.height);
			return output.Contains(mousePosition);
		}

		public void Draw()
		{
			Rect input = new Rect(position_.xMin, position_.yMin, transitionWidth, position_.height);
			Rect output = new Rect(position_.xMax - transitionWidth, position_.yMin, transitionWidth, position_.height);
			EditorGUI.DrawRect(position_, Color.black);
			EditorGUI.DrawRect(input, Color.white);
			EditorGUI.DrawRect(output, Color.white);
		}

		public void DrawTransitions()
		{
			Vector2 delta = new Vector2(initX * 1.5f, 0);
			Vector2 deltaY = new Vector2(0, initY * 3);
			foreach(StateGUI state in transitions_)
			{
				if(this.OutputOrigin.x < state.InputOrigin.x)
				{
					Drawing.DrawLine(this.OutputOrigin, state.InputOrigin, Color.red, 1, true);
				}
				else if(this.OutputOrigin.y < state.OutputOrigin.y)
				{
					Drawing.bezierLine(this.OutputOrigin, this.OutputOrigin + delta + deltaY, state.InputOrigin, state.InputOrigin - delta - deltaY, Color.red, 1, true, 30);
				}
				else
				{
					Drawing.bezierLine(this.OutputOrigin, this.OutputOrigin + delta - deltaY, state.InputOrigin, state.InputOrigin - delta + deltaY, Color.red, 1, true, 30);
				}
			}
		}
	}
}

