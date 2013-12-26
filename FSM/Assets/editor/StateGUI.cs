using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM
{
	public class StateGUI
	{
		private State state_;
		private Rect position_;
		private const int initX = 50;
		private const int initY = 20;
		private const int transitionWidth = 5;
		private List<StateGUI> transitions_ = new List<StateGUI>();

		public Vector2 InputOrigin      
		{
			get { return new Vector2(position_.xMin, position_.center.y); }
		}

		public Vector2 OutputOrigin      
		{
			get { return new Vector2(position_.xMax, position_.center.y); }
		}

		public void Addtransition(StateGUI state)
		{
			if(!transitions_.Contains(state))
				transitions_.Add(state);
		}

		public StateGUI(State state, Vector2 position)
		{
			state_ = state;
			position_ = new Rect(position.x - initX / 2, position.y - initY / 2, initX, initY);
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
			foreach(StateGUI state in transitions_)
			{
				Drawing.DrawLine(this.OutputOrigin, state.InputOrigin, Color.red, 2, true);
			}
		}
	}
}

