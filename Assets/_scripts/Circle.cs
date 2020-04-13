using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
	public Vector2 center;
	public float radius;

	public Circle (Vector2 vector2, float radius)
	{
		center = vector2;
		this.radius = radius;
	}

	public override string ToString ()
	{
		return "(center: " + center + ", r: " + radius + ")";
	}
}


