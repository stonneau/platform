using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SpeedFSM
{
	[System.Serializable] 
	public class StateMachine : ScriptableObject
	{
		[SerializeField]
		public List<State> states_;// = new List<State>();
		// Use this for initialization
		void OnEnable ()
		{
			if(states_ == null)
				states_ = new List<State>();
			//hideFlags = HideFlags.HideAndDontSave;
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		public State AddState(Vector2 location)
		{
			State s = ScriptableObject.CreateInstance<State>();
			s.x = location.x;
			s.y = location.y;
			states_.Add(s);
			return s;
		}

		public void RemoveState(State state)
		{
			foreach(State transition in states_)
			{
				transition.RemoveTransition(state);
			}
			states_.Remove(state);
		}
	}
}
