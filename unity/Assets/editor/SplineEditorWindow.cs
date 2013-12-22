using UnityEngine;
using UnityEditor;
using System.Collections;

public class SplineEditorWindow : EditorWindow
{
	//Spline spline;
	//Texture2D texture;
	
	public void Init()
	{
//		spline = (Spline)FindObjectOfType(typeof(Spline));
	}
	
	void OnEnable()
	{
		/*texture = new Texture2D(64,32);
		Color color = new Color(0,0,0);
		for(int i = 0; i < 32; ++i)
		{
			texture.SetPixel(i,i,color);
		}
		texture.Apply();*/
	}
	
	void OnGUI()
	{
		//grid.color = EditorGUILayout.ColorField(grid.color, GUILayout.Width(200));
		//GUILayout.Label(texture);
	}
}

