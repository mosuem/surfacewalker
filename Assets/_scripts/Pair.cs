using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Pair<T>
{
	public T left;
	public T right;

	public Pair (T i, T i2)
	{
		left = i;
		right = i2;
	}

	public override string ToString ()
	{
		return "(" + left + ", " + right + ")";
	}
}


