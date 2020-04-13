using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Complex
{
	public static Complex one = new Complex (1, 0);

	public float a;
	public float b;

	public Complex (float x, float y)
	{
		a = x;
		b = y;
	}

	public static Complex operator * (Complex x, Complex y)
	{
		return new Complex (x.a * y.a - x.b * y.b, x.b * y.a + x.a * y.b);
	}

	public static Complex operator / (Complex x, Complex y)
	{
		var sqr = y.a * y.a + y.b * y.b;
		return new Complex ((x.a * y.a + x.b * y.b) / sqr, (x.b * y.a + x.a * y.b) / sqr);
	}

	public static Complex operator - (Complex x, Complex y)
	{
		return new Complex (x.a - y.a, x.b - y.b);
	}

	public static Complex operator - (Complex x)
	{
		return new Complex (-x.a, -x.b);
	}

	public static Complex operator + (Complex x, Complex y)
	{
		return new Complex (x.a + y.a, x.b + y.b);
	}

	public override string ToString ()
	{
		return "(" + a + ", " + b + ")";
	}
}


