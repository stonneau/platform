using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpeedFSM
{
	[System.Serializable] 
	public class FSMManager : MonoBehaviour
	{
		public StateMachine stateMachine;

		void OnEnable()
		{
			if(stateMachine == null)
			{
				stateMachine = ScriptableObject.CreateInstance<StateMachine>();
			}
		}
	}
}
