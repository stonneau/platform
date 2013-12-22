// MyScriptEditor.cs
using UnityEditor;
using UnityEngine;
using System.Collections;

[System.Serializable] 
[CustomEditor(typeof(State))]
public class StateEditor : Editor {
	State state;
	public void OnEnable()
	{
		state = (State)target;
//		SceneView.onSceneGUIDelegate += SplineUpdate;
	}
	
	public void OnDisable()
	{
//		SceneView.onSceneGUIDelegate -= SplineUpdate;
	}

	public override void OnInspectorGUI()
	{	
		SplineOptionsCreator options = SplineOptionsCreator.GetInstance();
		int oldChoiceX = options.GetSplineId(state.xSpline);
		int oldChoiceY = options.GetSplineId(state.ySpline);
		int currentChoiceX; int currentChoiceY;
		EditorUtility.SetDirty(target);
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField("X speed", GUILayout.Width(50));
				currentChoiceX = EditorGUILayout.Popup(oldChoiceX, options.choices);
				EditorGUILayout.LabelField("Y speed", GUILayout.Width(50));
				currentChoiceY = EditorGUILayout.Popup(oldChoiceY, options.choices);
			}
			EditorGUILayout.EndVertical();
			{
				GUILayout.Label(options.choiceTextures[currentChoiceX]);
				GUILayout.Label(options.choiceTextures[currentChoiceY]);
			}
		}
		EditorGUILayout.EndHorizontal();

		if(oldChoiceX != currentChoiceX)
		{
			state.xSpline = new Spline(options.splines[currentChoiceX].points_);
		}
		if(oldChoiceY != currentChoiceY)
		{
			state.ySpline = new Spline(options.splines[currentChoiceY].points_);
		}
	}
}