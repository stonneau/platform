using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

#region SplineController
[System.Serializable] 
class SplineController
{
	public SplineController()
	{
		spline = new Spline();
		texture = new Texture2D(255,255);
		DrawSpline();
		minY_ = 0f;
		maxY_ = 0f;
		maxTime = 1f;
	}
	public void Update()
	{
		if(needsUpdate)
		{
			needsUpdate = false;
			DrawSpline();
		}
	}

	private static int CompareVectorByX(Vector2 x, Vector2 y)
	{
		float res = x.x - y.x;
		if (res == 0f) return 0;
		return res < 0 ? -1 : 1;
	}

	public bool AddPoint(Vector2 point)
	{
		needsUpdate = true;
		for(int i = 0; i!= spline.points_.Count; ++i)
		{
			if(spline.points_[i].x == point.x)
			{
				return false;
			}
			else if(spline.points_[i].x > point.x)
			{
				spline.points_.Insert(i, point);
				spline.InitSpline();
				return true;
			}
		}
		spline.points_.Add(point);
		spline.InitSpline();
		return true;
	}

	public bool EditPoint(int index, Vector2 point)
	{
		spline.points_.RemoveAt(index);
		return AddPoint(point);
	}

	public void DrawSpline()
	{
		//first make it white
		for(int i = 0; i < texture.width; ++i)
		{
			for(int j = 0; j < texture.height; ++j)
			{
				texture.SetPixel(i,j,Color.white);
			}
		}
		float[] values = new float[texture.width];
		//ClearSpline();
		float dx = maxTime / texture.width;
		float t = 0;
		int zeroLine = texture.height / 2;
		// retrieving min and max values for spline
		for(int i = 0; i < texture.width; ++i, t = t + dx)
		{
			float val = spline.eval(t);
			minY_ = minY_ > val ? val : minY_;
			maxY_ = maxY_ > val ? maxY_ : val;
			values[i] = val;
		}
		float dy = Mathf.Max(Mathf.Max(Mathf.Abs(maxY_), Mathf.Abs(minY_)), 1);
		dy = (dy == 0) ? 1 : dy;
		dy = (float)(texture.height / 2) / dy;
		for(int i = 0; i < texture.height; ++i)
		{
			texture.SetPixel(0,i,Color.black);
		}
		for(int i = 0; i < texture.width; ++i)
		{
			texture.SetPixel(i,zeroLine, Color.black);
			texture.SetPixel(i, zeroLine + (int)(values[i] * (dy != 0 ? dy : 1f / (float)zeroLine)), Color.red);
		}
		texture.Apply();
	}
	
	public Vector2 GetSplinePoint(Vector2 screenPosition)
	{
		Vector2 res = new Vector2(0,0);
		int zero = texture.height / 2;
		float dx = maxTime / texture.width;
		float dy = Mathf.Max(Mathf.Max(Mathf.Abs(maxY_), Mathf.Abs(minY_)), 1);
		dy /= (float)(texture.height / 2);
		dy = (dy != 0 ? dy : 1f / (float)zero);
		res.x = screenPosition.x * dx;
		res.y = (screenPosition.y - zero) * dy;
		return res;
	}

	public List<Vector2> Points      
	{
		get { return spline.points_; }
	}

	public Spline spline;
	public Texture2D texture;
	public float maxTime;
	public float minY_;
	public float maxY_;
	private bool needsUpdate;
}
#endregion // SplineController

public class SplineEditorWindow : EditorWindow
{
	SplineController splineController;
	float currentX= -1;
	Rect texturePos;

	public void Init()
	{
		splineController = new SplineController();
		texturePos = new Rect(15, 15, splineController.texture.width, splineController.texture.height);
	}

	public void OnGUI()
	{	
		wantsMouseMove = true;
		Event e = Event.current;
		if(e != null && e.type == EventType.MouseDrag)
		{
			if(texturePos.Contains(e.mousePosition))
			{
				if(currentX < 0)
				{
					currentX = e.mousePosition.x - 15;
				}
			}
		}
		if(e != null && e.type == EventType.MouseUp)
		{
			if(texturePos.Contains(e.mousePosition))
			{
				Vector2 values = splineController.GetSplinePoint(new Vector2(currentX > 0 ? currentX : (e.mousePosition.x - 15), splineController.texture.height - (e.mousePosition.y - 15)));
				splineController.AddPoint(values);
				this.Repaint();
			}
			currentX = -1;
		}
		splineController.Update();
		GUI.Label(texturePos, splineController.texture);
	}
}

