﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class SplineOptionsCreator
{
	public GUIContent[] choices;
	public GUIContent[] choiceTextures;
	public Spline[] splines;

	private static bool init = false;
	private static SplineOptionsCreator options;

	private SplineOptionsCreator()
	{
		InitSplines();
	}
	
	public static SplineOptionsCreator GetInstance()
	{
		if(!init)
		{
			init = true;
			options = new SplineOptionsCreator();
		}
		return options;
	}

	private void InitSpline(List<Vector2> points, string name, ref int i)
	{
		Spline spline = new Spline(points);
		Texture2D splineText = new Texture2D(64, 64);
		splineText.hideFlags = HideFlags.HideAndDontSave;
		spline.points_ = points;
		DrawSpline(spline, splineText);
		choices[i] = new GUIContent(name);
		choiceTextures[i] = new GUIContent(splineText);
		splines[i] = spline;
		++i;
	}
	
	private void InitSplines()
	{
		choices = new GUIContent[3];
		choiceTextures = new GUIContent[3];
		splines = new Spline[3];
		int i = 0;
		List<Vector2> pointsUp = new List<Vector2>();
		pointsUp.Add(new Vector2(0,0));
		pointsUp.Add(new Vector2(1,1));
		
		List<Vector2> pointsDown = new List<Vector2>();
		pointsDown.Add(new Vector2(0,1));
		pointsDown.Add(new Vector2(1,0));
		
		List<Vector2> pointsNo = new List<Vector2>();
		pointsNo.Add(new Vector2(0,0));
		pointsNo.Add(new Vector2(1,0));
		
		InitSpline(pointsUp, "croissant", ref i);
		InitSpline(pointsDown, "decroissant", ref i);
		InitSpline(pointsNo, "rien", ref i);
	}
	
	private void DrawSpline(Spline spline, Texture2D texture)
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
	
	public int GetSplineId(Spline spline)
	{
		for(int i=0; i< splines.Length; ++i)
		{
			if(spline.Equals(splines[i]))
				return i;
		}
		return 0;
	}
}
