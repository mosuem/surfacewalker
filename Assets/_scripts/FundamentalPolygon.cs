using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundamentalPolygon
{
	public MeshFilter filter;
	public int p;
	public int q;
	public Circle[] inversionCircleCenters;
	float s;
	float h;
	public Vector2[] originalPositions;
	public static float factor = 50f;
	int isSide = -1;

	Material mat;

	Circle largeCircle = new Circle (Vector2.zero, 1f);

	GameObject gameObject;

	public FundamentalPolygon (int p1, int q1, Material mat1, GameObject myParent, Vector2[] poriginalPositions, Circle[] pinversionCircleCenters, int side = -1, string name = "")
	{
		p = p1;
		q = q1;
		mat = mat1;
		originalPositions = new Vector2[p + 1];
		inversionCircleCenters = new Circle[p];
		gameObject = new GameObject ("Polygon " + name);

		filter = gameObject.AddComponent< MeshFilter > ();
		Mesh mesh = filter.mesh;
		mesh.Clear ();

		if (myParent != null) {
			gameObject.transform.SetParent (myParent.transform);
			this.originalPositions = (Vector2[])poriginalPositions.Clone ();
			this.inversionCircleCenters = (Circle[])pinversionCircleCenters.Clone ();
			isSide = side;
		} else {
			var vertices = new Vector2[p + 1];

			var tanp = Mathf.Tan (Mathf.PI / p);
			var tanq = Mathf.Tan (Mathf.PI / q);
			s = Mathf.Sqrt ((1 - tanp * tanq) / (1 + tanp * tanq));
			h = s / (1 - tanp * tanq);
			vertices [0] = new Vector2 (s, 0);
			originalPositions [0] = vertices [0];
			var radius = Vector2.Distance (new Vector2 (s, 0), new Vector2 (h, h * Mathf.Tan (Mathf.PI / p)));
			inversionCircleCenters [0] = new Circle (new Vector2 (h, h * tanp), radius);
			for (int i = 1; i < p; i++) {
				vertices [i] = reflect (vertices [i - 1], inversionCircleCenters [i - 1].center);
				inversionCircleCenters [i] = new Circle (reflect (inversionCircleCenters [i - 1].center, vertices [i]), radius);
				originalPositions [i] = vertices [i];
			}
			vertices [p] = GetCentroid (vertices);
			originalPositions [p] = vertices [p];
		}

		var vertices3d = new Vector3[p + 1];
		for (int i = 0; i < p + 1; i++) {
			vertices3d [i] = ToVector3 (originalPositions [i]);
		}
		mesh.vertices = vertices3d;

		var tri = new int[p * 3];
		for (int i = 0; i < p - 1; i++) {
			tri [3 * i] = i + 1;
			tri [3 * i + 1] = i;
			tri [3 * i + 2] = p;
		}
		tri [3 * (p - 1)] = 0;
		tri [3 * (p - 1) + 1] = p - 1;
		tri [3 * (p - 1) + 2] = p;

		mesh.triangles = tri;

		var normals = new Vector3[p + 1];
		for (int i = 0; i < p + 1; i++) {
			normals [i] = Vector3.forward;
		}

		mesh.normals = normals;

		var uv = new Vector2[p + 1];
		for (int i = 0; i < p; i++) {
			uv [i] = new Vector2 (0.5f + (originalPositions [i].x) / (2 * s), 
				0.5f + (originalPositions [i].y) / (2 * s));
		}
		uv [p] = new Vector2 (0.5f, 0.5f);

		mesh.uv = uv;

		mesh.RecalculateBounds ();
		var renderer = gameObject.AddComponent <MeshRenderer> ();
		renderer.material = mat;
		renderer.material.color = Random.ColorHSV ();

		var collider = gameObject.AddComponent <MeshCollider> ();
		filter.gameObject.transform.localScale = filter.gameObject.transform.localScale * factor;
	}

	public static Vector2 Scale (Vector2 v)
	{
		return v * (1f / factor);
	}

	public static Vector3 Scale (Vector3 v)
	{
		return v * (1f / factor);
	}

	Vector2 reflect (Vector2 v, Vector2 l)
	{
		return 2 * (Vector2.Dot (v, l) / Vector2.Dot (l, l)) * l - v;
	}

	public int toKleinBeltrami (bool addNumbers = false, bool isMoving = false)
	{
		int b = -1;
		Mesh mesh = filter.mesh;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < p; i++) {
			vertices [i] = 2 * vertices [i] / (1 + Vector3.Dot (vertices [i], vertices [i]));
			if (isMoving) {
				if (i > 0) {
					var v1 = vertices [i - 1] * factor;
					var v2 = vertices [i] * factor;
					var f = v2.x * v1.z - v1.x * v2.z;
					if (f > 0) {
						Debug.Log ("Is outside on side " + (i - 1));
						b = i - 1;
					}
//				else
//					Debug.Log ("Vertex " + (i - 1) + ": " + v1 + ", Vertex " + i + ": " + v2 + ", f = " + f);
				} else if (vertices [0].x * vertices [p - 1].y - vertices [p - 1].x * vertices [0].y > 0) {
					Debug.Log ("Is outside on side " + (p - 1));
					b = p - 1;
				}
			}
		}
		vertices [p] = GetCentroid (vertices);
		mesh.vertices = vertices;
		if (addNumbers) {
			AddCornerNumbers ();
		}
		return b;
	}

	void AddCornerNumbers ()
	{
		var obj = GameObject.FindGameObjectWithTag ("Finish");
		for (int l = 0; l < p; l++) {
			var corner1 = GameObject.Instantiate (obj);
			corner1.transform.parent = gameObject.transform;
			var vertices2 = filter.mesh.vertices;
			corner1.transform.position = vertices2 [l] * factor + (vertices2 [p] - vertices2 [l]) * factor * 0.1f;
			corner1.GetComponent<TextMesh> ().text = "" + l;
		}
	}

	void refreshCornerNumbers ()
	{
		var corners = gameObject.GetComponentsInChildren<TextMesh> ();
		for (int l = 0; l < p; l++) {
			var vertices2 = filter.mesh.vertices;
			corners [l].transform.position = vertices2 [l] * factor + (vertices2 [p] - vertices2 [l]) * factor * 0.1f;
			corners [l].GetComponent<TextMesh> ().text = "" + l;
		}
	}

	Vector3 GetCentroid (Vector3[] vertices)
	{
		var vertexSum = Vector3.zero;
		for (int i = 0; i < p; i++) {
			vertexSum += vertices [i];
		}
		var centroid = (1f / p) * vertexSum;
		return centroid;
	}

	Vector2 GetCentroid (Vector2[] vertices)
	{
		var vertexSum = Vector2.zero;
		for (int i = 0; i < p; i++) {
			vertexSum += vertices [i];
		}
		var centroid = (1f / (float)p) * vertexSum;
		return centroid;
	}

	public FundamentalPolygon[] polygons;

	public void foldOut (int k)
	{
		if (k > 0) {
			polygons = new FundamentalPolygon[p];
			for (int i = 0; i < p; i++) {
				if (i != isSide) {
					polygons [i] = new FundamentalPolygon (p, q, mat, gameObject, originalPositions, inversionCircleCenters, i, "" + i);
					polygons [i].mirrorAt (i, true);
					polygons [i].toKleinBeltrami (true);
					polygons [i].foldOut (k - 1);
				}
			}
		}
//		var obj = GameObject.FindGameObjectWithTag ("Finish");
//		polygons [i] = new FundamentalPolygon (p, q, mat, gameObject, originalPositions, inversionCircleCenters, "" + i);
//		if (i == 1) {
//			polygons [i].mirrorAt (1);
//			polygons [i].toKleinBeltrami ();
//			polygons [i].foldOut (2);
//
//			
//		}
//		if (i == 0) {
//			polygons [i].mirrorAt (0);
//			polygons [i].toKleinBeltrami ();
//			polygons [i].foldOut (1);
//
//			for (int k = 0; k < p; k++) {
//				var corner1 = GameObject.Instantiate (obj);
//				corner1.transform.position = polygons [i].filter.mesh.vertices [k] * factor + (polygons [i].filter.mesh.vertices [p] - polygons [i].filter.mesh.vertices [k]) * factor * 0.1f;
//				corner1.GetComponent <TextMesh> ().text = "" + k;
//			}
//		}
//		if (i == 2) {
//			polygons [i].mirrorAt (2);
//			polygons [i].toKleinBeltrami ();
//
//			for (int k = 0; k < p; k++) {
//				var corner1 = GameObject.Instantiate (obj);
//				corner1.transform.position = polygons [i].filter.mesh.vertices [k] * factor + (polygons [i].filter.mesh.vertices [p] - polygons [i].filter.mesh.vertices [k]) * factor * 0.1f;
//				corner1.GetComponent <TextMesh> ().text = "" + k;
//			}
//		}

	}

	public int getPairedSide (int side)
	{
		return (side + 2) % p;
	}

	public void mirrorAt (int side, bool flip = true)
	{
		if (side < p) {
			isSide = side;
			var vertices = filter.mesh.vertices;
			var circle = inversionCircleCenters [side];
			DrawDebugCircle (circle);
			var radius = Vector2.Distance (new Vector2 (s, 0), new Vector2 (h, h * Mathf.Tan (Mathf.PI / p)));
			for (int i = 0; i < p; i++) {
				var point = originalPositions [i];
				var newPoint = PoincareDisc.MirrorPointAtCircle (circle, point);
				originalPositions [i] = newPoint;
				vertices [i] = new Vector3 (newPoint.x, 0, newPoint.y);
			}

			var centroid = GetCentroid (vertices);
			originalPositions [p] = GetCentroid (vertices);
			vertices [p] = centroid;
			filter.mesh.vertices = vertices;

			if (flip) {
				mirrorPolygon (side);
				turnPolygon (p / 4);
			}


			for (int i = 0; i < p; i++) {
				var j = i == p - 1 ? 0 : i + 1;
				var vector2 = PoincareDisc.MirrorPointAtCircle (largeCircle, originalPositions [i]);
//				Debug.DrawLine (new Vector3 (originalPositions [i].x, 0, originalPositions [i].y) * factor, new Vector3 (originalPositions [j].x, 0, originalPositions [j].y) * factor, Color.red, 5f);
//				Debug.DrawLine (new Vector3 (originalPositions [j].x, 0, originalPositions [j].y) * factor, new Vector3 (vector2.x, 0, vector2.y) * factor, Color.red, 100f);
				var circle2 = PoincareDisc.CircleCenterThrough (originalPositions [i], originalPositions [j], vector2);
				inversionCircleCenters [i] = circle2;
			}
			var vector3 = PoincareDisc.MirrorPointAtCircle (largeCircle, originalPositions [0]);
			var circle3 = PoincareDisc.CircleCenterThrough (originalPositions [p - 1], originalPositions [0], vector3);
			inversionCircleCenters [p - 1] = circle3;
		}
	}

	void turnPolygon (int numberTurns)
	{
		var vertices = filter.mesh.vertices;
		for (int j = 0; j < numberTurns; j++) {
			var tempZero = originalPositions [0];
			for (int i = 0; i < p - 1; i++) {
				originalPositions [i] = originalPositions [i + 1];
			}
			originalPositions [p - 1] = tempZero;
			isSide = (isSide - 1 + p) % p;
			Debug.Log ("Turned " + (j + 1) + "/" + numberTurns + ", isSide = " + isSide);
		}
		filter.mesh.vertices = ToVector3 (originalPositions);
	}

	void mirrorPolygon (int side)
	{
		var vertices = filter.mesh.vertices;
		Vector3[] newVertices = new Vector3[p + 1];
		var numberSwitches = p / 2;
		var counter = side - (p / 2 - 2) / 2;
		Debug.Log ("For side " + side);
		for (int i = 0; i < numberSwitches; i++) {
			var i1 = (counter + i + p) % p;
			var i2 = (counter - i - 1 + p) % p;
			Debug.Log ("Switch " + i1 + " and " + i2 + " (i1 is " + i1 + ")");
			var temp = vertices [i2];
			newVertices [i2] = vertices [i1];
			newVertices [i1] = temp;
			var temp2 = originalPositions [i2];
			originalPositions [i2] = originalPositions [i1];
			originalPositions [i1] = temp2;
		}
		isSide = (isSide + p + (p / 2)) % p;
		filter.mesh.vertices = newVertices;
	}

	void DrawDebugCircle (Circle c)
	{
		var angle = 0.1f;
		var first = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * c.radius + c.center;
		for (int i = 0; i < 1000; i++) {
			angle += 0.1f;
			var next = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * c.radius + c.center;
			Debug.DrawLine (ToVector3 (first) * factor, ToVector3 (next) * factor, Color.red, 100f);
			first = next;
		}
	}

	void DrawDebugPoint (Vector2 c)
	{
		DrawDebugCircle (new Circle (c, 0.05f));
	}

	static float sqr (float x)
	{
		return x * x;
	}

	public Vector2 center = Vector2.zero;

	public void MoveTo (Vector3 position, bool checkPosition = false)
	{
		var scaledPosition = toVector2 (position) * -1 / factor;
		var posSqr = Vector2.SqrMagnitude (scaledPosition);
		var vertices = filter.mesh.vertices;
		for (int i = 0; i < p; i++) {
			var point = originalPositions [i];
			var pointSqr = Vector2.SqrMagnitude (point);
			var posTimeDot = Vector2.Dot (scaledPosition, point);
			var newPoint = ((1f + 2f * posTimeDot + pointSqr) * scaledPosition + (1f - posSqr) * point) / (1f + 2 * posTimeDot + posSqr * pointSqr);
			vertices [i] = ToVector3 (newPoint);
		}
		filter.mesh.vertices = vertices;
		if (polygons != null) {
			for (int i = 0; i < p; i++) {
				if (polygons [i] != null) {
					polygons [i].MoveTo (position);
				}
			}
		}
		int outside = toKleinBeltrami (false, checkPosition);
		if (outside > -1 && false) {
			var pairedSide = getPairedSide (outside);
			var zVec = new Vector2[3];
			var wVec = new Vector2[3];

			var originalPositions2 = polygons [0].originalPositions;
			var originalPositions3 = polygons [6].originalPositions;
			zVec [0] = originalPositions2 [outside];
			wVec [0] = originalPositions2 [pairedSide];

			zVec [1] = originalPositions2 [mod (outside - 1)];
			wVec [1] = originalPositions2 [getPairedSide (mod (outside - 1))];

			zVec [2] = originalPositions2 [mod (outside + 1)];
			wVec [2] = originalPositions2 [getPairedSide (mod (outside + 1))];

			var m = getMoebius (zVec, wVec);
			for (int i = 0; i < p; i++) {
				var newPos = toVector2 (m * ToComplex (originalPositions2 [i]));
				int j = -1;
				for (int k = 0; k < p; k++) {
					if (originalPositions2 [k] == newPos) {
						j = k;
						break;
					}
				}
				Debug.Log ("Move " + i + " from " + originalPositions2 [i] + " to " + newPos + ", which is " + j);
				originalPositions2 [i] = newPos;
			}

			for (int i = 0; i < p; i++) {
				vertices [i] = ToVector3 (originalPositions2 [i]);
			}

			toKleinBeltrami (false, false);
			refreshCornerNumbers ();
			Debug.Log (m);
			// UnityEditor.EditorApplication.isPaused = true;
		}
		refreshCornerNumbers ();
	}

	Matrix2 getMoebius (Vector2[] zVec, Vector2[] wVec)
	{
		var z = ToComplex (zVec);
		var w = ToComplex (wVec);
		Matrix2 h1 = new Matrix2 (z [1] - z [2], -z [0] * (z [1] - z [2]), z [1] - z [0], -z [2] * (z [1] - z [0]));
		Matrix2 h2 = new Matrix2 (w [1] - w [2], -w [0] * (w [1] - w [2]), w [1] - w [0], -w [2] * (w [1] - w [0]));
		return h2.inv () * h1;
	}

	static Complex[] ToComplex (Vector2[] zVec)
	{
		var comp = new Complex[zVec.Length];
		for (int i = 0; i < zVec.Length; i++) {
			comp [i] = ToComplex (zVec [i]);
		}
		return comp;
	}

	static Complex ToComplex (Vector2 zVec)
	{
		return new Complex (zVec.x, zVec.y);
	}

	Vector2 toVector2 (Vector3 vector3)
	{
		return new Vector2 (vector3.x, vector3.z);
	}

	Vector2 toVector2 (Complex c)
	{
		return new Vector2 (c.a, c.b);
	}

	static Vector3 ToVector3 (Vector2 next)
	{
		return new Vector3 (next.x, 0, next.y);
	}

	static Vector3[] ToVector3 (Vector2[] next)
	{
		Vector3[] vectors = new Vector3[next.Length];
		for (int i = 0; i < next.Length; i++) {
			vectors [i] = ToVector3 (next [i]);
		}
		return vectors;
	}

	int mod (int i)
	{
		return (i + p) % p;
	}
}

