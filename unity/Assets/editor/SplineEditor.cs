// MyScriptEditor.cs
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor {

	Texture2D texture;
	Spline spline;
	GUIContent[] _choices = new GUIContent[2];
	int _choiceIndex = 0;
	bool redraw = true;
	bool init = true;
	public void OnEnable()
	{
		SceneView.onSceneGUIDelegate += SplineUpdate;
	}
	
	public void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= SplineUpdate;
	}

	public void InitSplines()
	{
		texture = new Texture2D(64,64);
		spline = (Spline)target;
		if(spline.points_.Count == 0)
		{
			spline.points_.Add(new Vector2(1,-10));
			spline.points_.Add(new Vector2(2,24));
			spline.points_.Add(new Vector2(3,-10));
		}
		spline.InitSpline();
		DrawSpline();
	}

	public void InitEditor()
	{
		_choices [0] = new GUIContent("first", texture);
		_choices [1] = new GUIContent("second", texture);
	}

	public void ClearSpline()
	{
		for(int i = 0; i < texture.width; ++i)
		{
			for(int j = 0; j < texture.height; ++j)
			{
				texture.SetPixel(i,j,Color.white);
			}
		}
		texture.Apply();
	}

	public void DrawSpline()
	{
		float[] values = new float[texture.width];
		ClearSpline();
		float dt = spline.maxTime;
		dt /= texture.width;
		float t = 0;
		float dToZero = float.PositiveInfinity;
		int zeroLine = 0;
		// retrieving min and max values for spline
		float min = float.PositiveInfinity; float max = float.NegativeInfinity;
		for(int i = 0; i < texture.width; ++i, t = t + dt)
		{
			float val = spline.eval(t);
			min = min > val ? val : min;
			max = max > val ? max : val;
			values[i] = val;
			if(Mathf.Abs(val)<dToZero)
			{
				dToZero = Mathf.Abs(val);
				zeroLine = i;
			}
		}
		float amplitude = Mathf.Abs(max - min);
		// Find out what is bigger, height or width ?
		// locating the zero y (depends on whether negative values exist)
		//int zero = (min < 0) ? (texture.height / 2) : 0;
		//zero = (max < 0) ? 0 : zero;
		//float window = Mathf.Max(Mathf.Abs(max), Mathf.Abs(min));
		// drawing x and y now
		for(int i = 0; i < texture.height; ++i)
		{
			texture.SetPixel(0,i,Color.black);
		}
		zeroLine = (int)(- min / amplitude * texture.height);
		for(int i = 0; i < texture.width; ++i)
		{
			texture.SetPixel(i,zeroLine, Color.black);
			texture.SetPixel(i, (int)((values[i] - min) / amplitude * texture.height ), Color.red);
		}
		texture.Apply();
	}

	void SplineUpdate(SceneView sceneview)
	{
		if(redraw)
		{
			redraw = false;
			DrawSpline();
		}
	}
	
	public override void OnInspectorGUI()
	{	
		if(init)
		{
			init = false;
			InitSplines();
			InitEditor();
			DrawSpline();
		}
		_choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

		EditorUtility.SetDirty(target);
		GUILayout.Button(texture);
		GUILayout.Label(texture);
		SceneView.RepaintAll();
	}
}