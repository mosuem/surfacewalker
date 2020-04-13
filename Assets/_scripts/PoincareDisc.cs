using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PoincareDisc
{
	//	public static float Distance (Vector2 a, Vector2 b)
	//	{
	//		var aSqr = a.sqrMagnitude;
	//		var bSqr = b.sqrMagnitude;
	//		var dist = Vector2.Distance (a, b);
	//		return 2 * Mathf.Log ((dist + Mathf.Sqrt (aSqr * bSqr - 2 * Vector2.Dot (a, b) + 1)) / Mathf.Sqrt ((1 - aSqr) * (1 - bSqr)));
	//	}

	public static Circle largeCircle = new Circle (Vector2.zero, 1f);

	public static float Distance (Vector2 p1, Vector2 p2)
	{
		Vector2 p;
		Vector2 q;
		if (p1 != Vector2.zero && p2 != Vector2.zero) {
			var sum = p1.sqrMagnitude;
//			var p3 = MirrorPointAtCircle (largeCircle, p1);
			var p3 = p1.normalized;
			Circle c = CircleCenterThrough (p1, p2, p3);
			if (!float.IsNaN (c.radius)) {
				var solutions = CircleCircleIntersection (c, largeCircle);
				var sol1 = solutions.left;
				var sol2 = solutions.right;
				Debug.Log ("Solutions are " + solutions);

				if (Vector2.Distance (p1, sol1) < Vector2.Distance (p1, sol1)) {
					p = sol1;
					q = sol2;
				} else {
					p = sol2;
					q = sol1;
				}
			} else {
				Debug.Log ("Points are: ");
				Debug.Log (p1);
				Debug.Log (p2);
				Debug.Log (p3);
				var l = 1f / Vector2.Distance (p1, p2);
				if (float.IsNaN (l)) {
					return 0f;
				}
				p = -l * (p2 - p1);
				q = l * (p2 - p1);
			}
		} else {
			var l = 1f / Vector2.Distance (p1, p2);
			if (float.IsNaN (l)) {
				return 0f;
			}
			p = -l * (p2 - p1);
			q = l * (p2 - p1);
		}
		Debug.Log ("p: " + p);
		Debug.Log ("q: " + q);
		return Mathf.Log ((Vector2.Distance (p, p2) * Vector2.Distance (p1, q)) / (Vector2.Distance (p, p1) * Vector2.Distance (p2, q)));
	}

	// center and radius of 1st circle


	public static Pair<Vector2> CircleCircleIntersection (Circle c1, Circle c2)
	{

		/* dx and dy are the vertical and horizontal distances between
   * the circle centers.
   */
		Debug.Log ("Circle1: " + c1);
		Debug.Log ("Circle2: " + c2);
		var line = c2.center - c1.center;

		/* Determine the straight-line distance between the centers. */
		//d = sqrt((dy*dy) + (dx*dx));
		var d = line.magnitude; // Suggested by Keith Briggs
		Debug.Log ("d: " + d);
		/* Check for solvability. */
		if (d > (c1.radius + c2.radius)) {
			Debug.Log ("no solution. circles do not intersect.");
			/* no solution. circles do not intersect. */
			return null;
		}
		if (d < Mathf.Abs (c1.radius - c2.radius)) {
			Debug.Log ("no solution. one circle is contained in the other.");
			/* no solution. one circle is contained in the other */
			return null;
		}

		/* 'point 2' is the point where the line through the circle
   * intersection points crosses the line between the circle
   * centers.  
   */

		/* Determine the distance from point 0 to point 2. */
		float a = ((c1.radius * c1.radius) - (c2.radius * c2.radius) + (d * d)) / (2f * d);
		Debug.Log (a);
		/* Determine the coordinates of point 2. */
		var x2 = c1.center.x + (line.x * a / d);
		var y2 = c1.center.y + (line.y * a / d);

		/* Determine the distance from point 2 to either of the
   * intersection points.
   */
		var h = Mathf.Sqrt ((c1.radius * c1.radius) - (a * a));

		/* Now determine the offsets of the intersection points from
   * point 2.
   */
		var rx = -line.y * (h / d);
		var ry = line.x * (h / d);

		/* Determine the absolute intersection points. */
		var sol1 = new Vector2 (x2 + rx, y2 + ry);
		var sol2 = new Vector2 (x2 - rx, y2 - ry);
		return new Pair<Vector2> (sol1, sol2);
	}

	public static Circle CircleCenterThrough (Vector2 p1, Vector2 p2, Vector2 p3)
	{
		float x = (Vector2.SqrMagnitude (p1) * (p2.y - p3.y) + Vector2.SqrMagnitude (p2) * (p3.y - p1.y) + Vector2.SqrMagnitude (p3) * (p1.y - p2.y)) / (2 * (p1.x * (p2.y - p3.y) - p1.y * (p2.x - p3.x) + p2.x * p3.y - p3.x * p2.y));
		float y = (Vector2.SqrMagnitude (p1) * (p3.x - p2.x) + Vector2.SqrMagnitude (p2) * (p1.x - p3.x) + Vector2.SqrMagnitude (p3) * (p2.x - p1.x)) / (2 * (p1.x * (p2.y - p3.y) - p1.y * (p2.x - p3.x) + p2.x * p3.y - p3.x * p2.y));
		var center = new Vector2 (x, y);
		var radius = Vector2.Distance (center, p1);
		return new Circle (center, radius);
	}

	public static Vector2 MirrorPointAtCircle (Circle circle, Vector2 point)
	{
		var circleCenter = circle.center;
		var radius = circle.radius;
		return circleCenter + (radius * radius * (point - circleCenter)) / Vector2.SqrMagnitude (point - circleCenter);
	}
}

