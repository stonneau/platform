using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DotNetMatrix;

[System.Serializable] 
struct cubic_function
{
	public cubic_function(float a, float b, float c, float d, float min, float max)
	{
		t_max_ = max; t_min_ = min;
		a_ = a;b_ = b; c_ = c;d_ = d;
		if(t_min_ >= t_max_)
		{
			throw new SplineException("t_min  > t_max"); // TODO
		}
	}

	public cubic_function(double a, double b, double c, double d, float min, float max)
	{
		t_max_ = max; t_min_ = min;
		a_ = (float)a;b_ = (float)b; c_ = (float)c;d_ = (float)d;
		if(t_min_ > t_max_)
		{
			throw new SplineException("t_min  > t_max"); // TODO
		}
	}

	public float eval(float t)
	{
		if((t < t_min_ || t > t_max_)){throw new SplineException("t not in range");} // TODO
		float dt  = t-t_min_ ;
		return a_+ b_ * dt + c_ * dt*dt  + d_ * dt*dt*dt;
	}

	readonly float a_;
	readonly float b_;
	readonly float c_;
	readonly float d_;
	public readonly float t_min_;
	public readonly float t_max_;
}

[System.Serializable]
public class Spline
{
	public float maxTime;
	public float minTime;
	public List<Vector2> points_;

	private cubic_function[] subSplines_;

	public Spline()
	{
		points_ = new List<Vector2>();
		points_.Add(new Vector2(0,0));
		points_.Add(new Vector2(1,0));
		InitSpline();
	}

	public Spline(List<Vector2> points)
	{
		points_ = new List<Vector2>();
		foreach(Vector2 point in points)
		{
			points_.Add(new Vector2(point.x, point.y));
		}
		InitSpline();
	}

	public void InitSpline()
	{
		int size = points_.Count;
		subSplines_ = new cubic_function[size];
		if(size < 1) {return;}
		
		GeneralMatrix  a = new GeneralMatrix(size, 1);
		GeneralMatrix  b = new GeneralMatrix(size, 1);
		GeneralMatrix  c = new GeneralMatrix(size, 1);
		GeneralMatrix  d = new GeneralMatrix(size, 1);
		GeneralMatrix  x = new GeneralMatrix(size, 1);

		GeneralMatrix h1 = new GeneralMatrix(size, size);
		GeneralMatrix h2 = new GeneralMatrix(size, size);
		GeneralMatrix h3 = new GeneralMatrix(size, size);
		GeneralMatrix h4 = new GeneralMatrix(size, size);
		GeneralMatrix h5 = new GeneralMatrix(size, size);
		GeneralMatrix h6 = new GeneralMatrix(size, size);

		//In it(wayPointsBegin), next(wayPointsBegin);
		//++next;
		//Numeric t_previous((*it).first);
		List<Vector2>.Enumerator it = points_.GetEnumerator(); it.MoveNext();
		List<Vector2>.Enumerator next = points_.GetEnumerator(); 
		bool foundNext = next.MoveNext(); foundNext = next.MoveNext();
		for(int i = 0; foundNext; it.MoveNext(), ++i)
		{
			//num_t const dTi((*next).first  - (*it).first);
			float dTi = (next.Current.x  - it.Current.x);
			float dTi_sqr = (dTi * dTi);
			float dTi_cube = (dTi_sqr * dTi);
			// filling matrices values
			h3[i,i]   = -3 / dTi_sqr;
		    h3[i,i+1] =  3 / dTi_sqr;
		    h4[i,i]   = -2 / dTi;
		    h4[i,i+1] = -1 / dTi;
		    h5[i,i]   =  2 / dTi_cube;
		    h5[i,i+1] = -2 / dTi_cube;
		    h6[i,i]   =  1 / dTi_sqr;
		    h6[i,i+1] =  1 / dTi_sqr;
			if(i+2 < size)
			{
				//In it2(next); ++ it2;
				//num_t const dTi_1(1/((*it2).first - (*next).first));
				float currentNextTime = next.Current.x;
				foundNext = next.MoveNext();
				float it2 = next.Current.x;
				float dTi_1 = (1/(it2 - currentNextTime));
				float dTi_1sqr = (dTi_1 * dTi_1);
				// this can be optimized but let's focus on clarity as long as not needed
				h1[i+1, i]   =  2 / dTi;
				h1[i+1, i+1] =  4 / dTi + 4 / dTi_1;
				h1[i+1, i+2] =  2 / dTi_1;
				h2[i+1, i]   = -6 / dTi_sqr;
				h2[i+1, i+1] = (6 / dTi_1sqr) - (6 / dTi_sqr);
				h2[i+1, i+2] =  6 / dTi_1sqr;
			}
       		else
       		{
				foundNext = next.MoveNext();
			}
			x[i,0] = it.Current.y;
		}
		// adding last x
		x[size-1,0] = it.Current.y;
		a= x;

		GeneralMatrix inverse = h1.PseudoInverse();
		b = inverse * h2 * x; //h1 * b = h2 * x => b = (h1)^-1 * h2 * x
		c = h3 * x + h4 * b;
		d = h5 * x + h6 * b;

		it = points_.GetEnumerator(); it.MoveNext(); 
		next = points_.GetEnumerator(); next.MoveNext();
		foundNext = next.MoveNext();
		for(int i=0; foundNext; ++i, it.MoveNext())
		{
			subSplines_[i] = new cubic_function(a[i,0], b[i,0], c[i,0], d[i,0], it.Current.x, next.Current.x);
			foundNext = next.MoveNext();
		}
		subSplines_[size-1] = new cubic_function(a[size-1,0], b[size-1,0], c[size-1,0], d[size-1,0], it.Current.x, it.Current.x);
		minTime = subSplines_[0].t_min_;
		maxTime = subSplines_[size-1].t_max_;
	}

	public float eval(float t)
	{			
		foreach(cubic_function fun in subSplines_)
		{
			if(t >= fun.t_min_ && t <= fun.t_max_)
			{
				return fun.eval(t);
			}
		}
		return 0f;
	}

	public virtual bool Equals(Spline spline)
	{
		if(spline.points_.Count != this.points_.Count) return false;
		for(int i = 0; i!= points_.Count; ++i)
		{
			if(points_[i] != spline.points_[i])
			{
				return false;
			}
		}
		return true;
	}
}

public class SplineException : Exception
{
	public SplineException(string Message)
		: base(Message)
	{}
}
