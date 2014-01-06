using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM
{
	[System.Serializable] 
	public class StateMachine : MonoBehaviour
	{
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
			s.location = location;
			states_.Add(s);
			return s;
		}
	}
}
