// MyScriptEditor.cs
using UnityEditor;
using UnityEngine;
using System.Collections;

[System.Serializable] 
[CustomEditor(typeof(State))]
public class StateEditor : Editor {
	State state;
	string xName ="";
	string yName ="";
	SplineOptionsCreator options = SplineOptionsCreator.GetInstance();
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
		int oldChoiceX = options.GetSplineId(xName);
		int oldChoiceY = options.GetSplineId(yName);
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
			xName = options.choices[currentChoiceX].text;
		}
		if(oldChoiceY != currentChoiceY)
		{
			state.ySpline = new Spline(options.splines[currentChoiceY].points_);
			yName = options.choices[currentChoiceY].text;
		}
	}
}