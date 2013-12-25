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
		texture.hideFlags = HideFlags.HideAndDontSave;
		controlPointsScreenCoord = new List<Vector2>();
		minY_ = 0f;
		maxY_ = 0f;
		maxTime = 1f;
		DrawSpline();
	}
	public void Update()
	{
		if(needsUpdate)
		{
			spline.InitSpline();
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

	public int AddPoint(Vector2 point)
	{
		needsUpdate = true;
		for(int i = 0; i!= spline.points_.Count; ++i)
		{
			if(spline.points_[i].x == point.x)
			{
				return -1;
			}
			else if(spline.points_[i].x > point.x)
			{
				spline.points_.Insert(i, point);
				spline.InitSpline();
				return i;
			}
		}
		spline.points_.Add(point);
		return spline.points_.Count -1;
	}

	public int EditPoint(int index, Vector2 point)
	{
		if(!point.Equals(spline.points_[index]))
		{
			spline.points_.RemoveAt(index);
			return AddPoint(point);
		}
		return index;
	}
	
	public bool RemovePoint(int index)
	{
		needsUpdate = true;
		if(index >= spline.points_.Count) return false;
		spline.points_.RemoveAt(index);
		return true;
	}

	public int Grab(int x, int y)
	{
		Vector2 test = new Vector2(x,y);
		int i = 0;
		foreach(Vector2 point in controlPointsScreenCoord)
		{
			if(Vector2.Distance(point, test) < grabDistance)
			{
				return i;
			}
			++i;
		}
		return -1;
	}

	public void DrawSpline()
	{
		maxY_ = float.NegativeInfinity; minY_ = float.PositiveInfinity;
		maxTime = spline.maxTime;
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
		// now drawing control Points
		controlPointsScreenCoord.Clear();
		foreach(Vector2 point in Points)
		{
			int x = (int)(point.x * 1/ dx);
			int y = zeroLine + (int)(point.y * (dy != 0 ? dy : 1f / (float)zeroLine));
			controlPointsScreenCoord.Add(new Vector2(x, y));
			for(int i = x-grabDistance / 2; i<= x+grabDistance / 2; ++i)
			{
				for(int j = y-grabDistance / 2; j<= y+grabDistance / 2; ++j)
				{
					if (i >= 0 && i < texture.width && j >0 && j < texture.height)
					{
						texture.SetPixel(i, j, Color.blue);
					}
				}
			}
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
	private const int grabDistance = 6;
	private List<Vector2> controlPointsScreenCoord;
}
#endregion // SplineController

[System.Serializable] 
public class SplineEditorWindow : EditorWindow
{
	SplineController splineController;
	private int currentIndex= -1;
	private Rect texturePos;
	private string curveName;
	int currentChoiceX = 0;
	Vector2 scrollPos = new Vector2(0,0);
	
	public void Init()
	{
		splineController = new SplineController();
		SplineOptionsCreator opt = SplineOptionsCreator.GetInstance();
		int id = opt.splines.Length - 3;
		curveName = "Curve" + id;
		texturePos = new Rect(15, 15, splineController.texture.width, splineController.texture.height);
	}

	private void DrawChoiceRef()
	{
		SplineOptionsCreator options = SplineOptionsCreator.GetInstance();
		GUI.Label(new Rect(15 + splineController.texture.width + 5, 75, 70, 15 ), "Base Curve");
		currentChoiceX = EditorGUI.Popup(new Rect(15 + splineController.texture.width + 5, 90, 70, 15 ), currentChoiceX, options.choices);
		GUI.Label(new Rect(15 + splineController.texture.width + 90, 75, 32, 32 ), options.choiceTextures[currentChoiceX].image);
		if(GUI.Button(new Rect(15 + splineController.texture.width + 5, 110, 130, 15 ), "Reset to base curve"))
		{
			splineController.spline = new Spline(options.splines[currentChoiceX].points_);
			splineController.spline.InitSpline();
			splineController.DrawSpline();
		}
	}

	private void DrawSave()
	{
		curveName = GUI.TextField(new Rect(15 + splineController.texture.width + 5, 145, 130, 15), curveName);
		if(GUI.Button(new Rect(15 + splineController.texture.width + 5, 160, 130, 15), "Save Curve"))
		{
			SplineOptionsCreator options = SplineOptionsCreator.GetInstance();
			options.AddSpline(splineController.spline, curveName);
		}
	}

	private void DrawPoints()
	{
		int initY = 0;
		int initX = 0;
		Vector2[] points = new Vector2[splineController.Points.Count];
		bool[] deletes = new bool[splineController.Points.Count];
		int i = 0;
		scrollPos = GUI.BeginScrollView(new Rect(0, 15 + splineController.texture.height + 10, 250, 300),scrollPos,new Rect(0, 0, 240, 30 * points.Length));
		foreach(Vector2 point in splineController.Points)
		{
			points[i] = EditorGUI.Vector2Field(new Rect(initX, initY, 100, 15), "point " + i + ": ",  point);
			deletes[i] = points.Length > 2 && GUI.Button(new Rect(initX + 120, initY + 15, 100, 15), "Delete point");
			initY += 30;
			++i;
		}
		GUI.EndScrollView();
		for(int j=0; j!=points.Length; ++j)
		{
			splineController.EditPoint(j,points[j]);
		}
		// only one can be destroyed at a time
		for(int j=0; j!=deletes.Length; ++j)
		{
			if (deletes[j]) splineController.RemovePoint(j);
		}
	}

	private void DrawAndEditCurve()
	{
		wantsMouseMove = true;
		Event e = Event.current;
		if(e != null && e.type == EventType.MouseDrag)
		{
			if(texturePos.Contains(e.mousePosition))
			{
				if(currentIndex < 0)
				{
					int x = (int)(e.mousePosition.x - 15);
					int y = (int)(splineController.texture.height - (e.mousePosition.y - 15));
					currentIndex = splineController.Grab(x, y);
					Vector2 values = splineController.GetSplinePoint(new Vector2(x, y));
					if(currentIndex < 0)
					{
						currentIndex = splineController.AddPoint(values);
					}
					this.Repaint();
				}
				else
				{
					Vector2 values = splineController.GetSplinePoint(new Vector2(e.mousePosition.x - 15, splineController.texture.height - (e.mousePosition.y - 15)));
					currentIndex = splineController.EditPoint(currentIndex, values);
					this.Repaint();
				}
			}
		}
		else if(e != null && e.type == EventType.MouseUp)
		{
			currentIndex = -1;
			if(texturePos.Contains(e.mousePosition))
			{
				Vector2 values = splineController.GetSplinePoint(new Vector2(e.mousePosition.x - 15, splineController.texture.height - (e.mousePosition.y - 15)));
				splineController.AddPoint(values);
				this.Repaint();
			}
		}
		GUI.Label(texturePos, splineController.texture);
		GUI.Label( new Rect(15 + splineController.texture.width + 5, 15, 120, 15 ), "min speed: " + decimal.Round((decimal)(splineController.minY_), 2,System.MidpointRounding.AwayFromZero));
		GUI.Label( new Rect(15 + splineController.texture.width + 5, 30, 120, 15 ), "max speed: " + decimal.Round((decimal)(splineController.maxY_), 2,System.MidpointRounding.AwayFromZero));
		GUI.Label( new Rect(15 + splineController.texture.width + 5, 45, 120, 15 ), "time: " + decimal.Round((decimal)(splineController.maxTime), 2,System.MidpointRounding.AwayFromZero));
	}

	public void OnGUI()
	{	
		DrawAndEditCurve();
		DrawChoiceRef();
		DrawPoints();
		DrawSave();
		splineController.Update();
	}
}

