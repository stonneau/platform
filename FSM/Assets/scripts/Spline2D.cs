using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DotNetMatrix;

[System.Serializable] 
struct cubic_function2D
{
	public cubic_function2D(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float min, float max)
	{
		t_max_ = max; t_min_ = min;
		a_ = a;b_ = b; c_ = c;d_ = d;
		if(t_min_ >= t_max_)
		{
			throw new SplineException("t_min  > t_max"); // TODO
		}
	}
	
	public Vector2 eval(float t)
	{
		if((t < t_min_ || t > t_max_)){throw new SplineException("t not in range");} // TODO
		float dt  = t-t_min_ ;
		return a_+ b_ * dt + c_ * dt*dt  + d_ * dt*dt*dt;
	}
	
	readonly Vector2 a_;
	readonly Vector2 b_;
	readonly Vector2 c_;
	readonly Vector2 d_;
	public readonly float t_min_;
	public readonly float t_max_;
}

[System.Serializable]
public class Spline2DControlpoint
{
	public Spline2DControlpoint(Spline2DControlpoint controlPoint)
	{
		t_ = controlPoint.t_; position_ = new Vector2(controlPoint.position_.x, controlPoint.position_.y);
	}

	public Spline2DControlpoint(float t, Vector2 position)
	{
		t_ = t; position_ = position;
	}
	public float t_;
	public Vector2 position_;
}

[System.Serializable]
public class Spline2D {
	public float maxTime;
	public float minTime;
	public List<Spline2DControlpoint> points_;
	
	private cubic_function2D[] subSplines_;
	
	public Spline2D()
	{
		points_ = new List<Spline2DControlpoint>();
		points_.Add(new Spline2DControlpoint(0,new Vector2(0,0)));
		points_.Add(new Spline2DControlpoint(1,new Vector2(0,0)));
		InitSpline();
	}
	
	public Spline2D(List<Spline2DControlpoint> points)
	{
		points_ = new List<Spline2DControlpoint>();
		foreach(Spline2DControlpoint point in points)
		{
			points_.Add(new Spline2DControlpoint(point));
		}
		InitSpline();
	}

	private Vector2 mv(GeneralMatrix m, int row)
	{
		return new Vector2((float)(m[row,0]), (float)(m[row,1]));
	}

	public void InitSpline()
	{
		int size = points_.Count;
		subSplines_ = new cubic_function2D[size];
		if(size < 1) { return;}
		
		GeneralMatrix a = new GeneralMatrix(size, 2);
		GeneralMatrix b = new GeneralMatrix(size, 2);
		GeneralMatrix c = new GeneralMatrix(size, 2);
		GeneralMatrix d = new GeneralMatrix(size, 2);
		GeneralMatrix x = new GeneralMatrix(size, 2);
		
		GeneralMatrix h1 = new GeneralMatrix(size, size);
		GeneralMatrix h2 = new GeneralMatrix(size, size);
		GeneralMatrix h3 = new GeneralMatrix(size, size);
		GeneralMatrix h4 = new GeneralMatrix(size, size);
		GeneralMatrix h5 = new GeneralMatrix(size, size);
		GeneralMatrix h6 = new GeneralMatrix(size, size);
		
		//In it(wayPointsBegin), next(wayPointsBegin);
		//++next;
		//Numeric t_previous((*it).first);
		List<Spline2DControlpoint>.Enumerator it = points_.GetEnumerator(); it.MoveNext();
		List<Spline2DControlpoint>.Enumerator next = points_.GetEnumerator(); 
		bool foundNext = next.MoveNext(); foundNext = next.MoveNext();
		for(int i = 0; foundNext; it.MoveNext(), ++i)
		{
			//num_t const dTi((*next).first  - (*it).first);
			float dTi = (next.Current.t_  - it.Current.t_);
			float dTi_sqr = (dTi * dTi);
			float  dTi_cube = (dTi_sqr * dTi);
			// filling matrices values
			h3[i,i]   = -3 / dTi_sqr;
			h3[i,i+1] =  3 / dTi_sqr;
			h4[i,i]   = -2 / dTi;
			h4[i,i+1] = -1 / dTi;
			h5[i,i]   =  2 / dTi_cube;
			h5[i,i+1] = -2 / dTi_cube;
			h6[i,i]   =  1 / dTi_sqr;
			h6[i,i+1] =  1 / dTi_sqr;
			if( i+2 < size)
			{
				//In it2(next); ++ it2;
				//num_t const dTi_1(1/((*it2).first - (*next).first));
				float currentNextTime = next.Current.t_;
				foundNext = next.MoveNext();
				float it2 = next.Current.t_;
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
			x[i,0] = it.Current.position_.x;
			x[i,1] = it.Current.position_.y;
		}
		// adding last x
		x[size-1,0] = it.Current.position_.x;
		x[size-1,1] = it.Current.position_.y;
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
			//subSplines_[i] = new cubic_function2D(a[i,0], b[i,0], c[i,0], d[i,0], it.Current.t_, next.Current.t_);
			subSplines_[i] = new cubic_function2D(mv(a,i), mv(b,i), mv(c,i), mv(d,i), it.Current.t_, next.Current.t_);
			foundNext = next.MoveNext();
		}
		subSplines_[size-1] = new cubic_function2D(mv(a,size-1), mv(b,size-1), mv(c,size-1), mv(d,size-1), it.Current.t_, it.Current.t_);
		minTime = subSplines_[0].t_min_;
		maxTime = subSplines_[size-1].t_max_;
	}
	
	public Vector2 eval(float t)
	{			
		foreach( cubic_function2D fun in subSplines_ )
		{
			if(t >= fun.t_min_ && t <= fun.t_max_)
			{
				return fun.eval(t);
			}
		}
		return new Vector2(0f,0f);
	}
	
	public virtual bool Equals(Spline2D spline)
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

