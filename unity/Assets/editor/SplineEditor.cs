// MyScriptEditor.cs
using UnityEditor;
using UnityEngine;
using System.Collections;

//[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor {
	
	SerializedProperty valueProperty;
	
	void OnEnable() {
		// Setup serialized property
		valueProperty = serializedObject.FindProperty("tg");
	}
	
	public override void OnInspectorGUI() {
		// Update the serializedProperty
		// always do this at the start of OnInspectorGUI
		serializedObject.Update();
		EditorGUILayout.IntSlider(valueProperty, 1, 10, new GUIContent("Tg"));
		serializedObject.ApplyModifiedProperties();
	}
	
}