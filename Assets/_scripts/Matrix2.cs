using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Matrix2
{
	public Complex a;
	public Complex b;
	public Complex c;
	public Complex d;

	public Matrix2 (Complex a, Complex b, Complex c, Complex d)
	{
		this.a = a;	
		this.b = b;
		this.c = c;
		this.d = d;
	}

	public static Matrix2 operator * (Matrix2 x, Matrix2 y)
	{
		return new Matrix2 (x.a * y.a + x.b * y.c, x.a * y.b + x.b * y.d, x.c * y.a + x.d * y.c, x.c * y.b + x.d * y.d);
	}

	public static Matrix2 operator * (Complex x, Matrix2 y)
	{
		return new Matrix2 (x * y.a, x * y.b, x * y.c, x * y.d);
	}

	public static Complex operator * (Matrix2 A, Complex z)
	{
		return (A.a * z + A.b) / (A.c * z + A.d);
	}

	public Complex det ()
	{
		return a * d - b * c;
	}

	public Matrix2 inv ()
	{
		return (Complex.one / det ()) * new Matrix2 (this.d, -this.b, -this.c, this.a);
	}

	public override string ToString ()
	{
		return "[" + a + ", " + b + "," + c + "," + d + "]";
	}

}


