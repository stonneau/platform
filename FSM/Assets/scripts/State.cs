using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SpeedFSM
{
	[System.Serializable] 
	public class State : ScriptableObject
	{
		public AnimationCurve x_;
		public AnimationCurve y_;
		public float maxSpeedX_;
		public float maxSpeedY_;
		public float maxTime_;
		public float gravity;
		public Transform target;
		public float speedY_;
		public List<State> transitions_;
		public Vector2 location;


		public List<State> Transitions
		{
			get {return transitions_;}
		}

		public void AddTransition(State state)
		{
			if(!transitions_.Contains(state))
			{
				transitions_.Add(state);
			}
		}

		public void OnEnable()
		{
			if(transitions_ == null) transitions_ = new List<State>();
		}
		
		// Update is called once per frame
		void Update () {
			/*currentTime_ += Time.deltaTime;
			float speedX = x_.Evaluate(currentTime_) * maxSpeedX_;
			speedY_ = speedY_ + y_.Evaluate(currentTime_) * Time.deltaTime  / maxTime_ * maxSpeedY_;
			speedY_ = speedY_ + gravity * Time.deltaTime / maxTime_;
			target.Translate( new Vector3(speedX * Time.deltaTime, speedY_ * Time.deltaTime, 0));*/
		}
	}
}
