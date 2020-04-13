using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusScript : MonoBehaviour
{
	public Material mat;
	public GameObject player;
	public GameObject test;
	public GameObject tile;

	const int rad1 = 5;
	const int rad2 = 2;

	float size1;
	float size2;

	public static GameObject torus;

	// Use this for initialization
	void Start ()
	{
		size1 = tile.GetComponent<MeshRenderer> ().bounds.size.x;
		size2 = tile.GetComponent<MeshRenderer> ().bounds.size.z;
		if (Wrapper.isEuclidean) {
			torus = BuildTorus (mat, Color.gray, rad1, rad2);
		} else {
			torus = BuildDoubleTorus (mat, Color.gray, rad1, rad2);
		}
		torus.transform.position = new Vector3 (0, 0, 260);
	}

	// Update is called once per frame
	void Update ()
	{
//		val1 += 0.1f;
//		for (int i = 0; i < 50; i++) {
//			var test1 = Instantiate (test);
//			float val2 = (Random.value - 0.5f) * 20f;
//			test1.transform.position = toTorus (new Vector3 (val1, 0f, val2)) + torus.transform.position;
//		}
	}

	public static Vector3 toTorus (Vector3 position, Vector2 tileSize)
	{
		float t1 = (position.x + (tileSize.x / 2)) / tileSize.x;
		float t2 = (position.z + (tileSize.y / 2)) / tileSize.y;
		var angle1 = 2 * Mathf.PI * t1;
		var vec1 = 1.01f * rad1 * new Vector3 (Mathf.Cos (angle1), 0, Mathf.Sin (angle1));
		var angle2 = 2 * Mathf.PI * t2;
		var vec2 = 1.01f * rad2 * new Vector3 (Mathf.Cos (angle2), Mathf.Sin (angle2), 0);
//		Debug.Log ("Angle at t1=" + t1 + " is " + Mathf.Rad2Deg * angle1);
		return vec1 + Quaternion.AngleAxis (Mathf.Rad2Deg * angle1, Vector3.down) * vec2;
	}

	public static GameObject BuildTorus (Material mat, Color c, float radius1, float radius2)
	{
		var gameObject = new GameObject ("Torus1");
//		gameObject.tag = "Sphere";
		MeshFilter filter = gameObject.AddComponent< MeshFilter > ();
		Mesh mesh = filter.mesh;
		mesh.Clear ();

		int nbRadSeg = 240;
		int nbSides = 180;

		#region Vertices		
		Vector3[] vertices = new Vector3[(nbRadSeg + 1) * (nbSides + 1)];
		float _2pi = Mathf.PI * 2f;
		for (int seg = 0; seg <= nbRadSeg; seg++) {
			int currSeg = seg == nbRadSeg ? 0 : seg;

			float t1 = (float)currSeg / nbRadSeg * _2pi;
			Vector3 r1 = new Vector3 (Mathf.Cos (t1) * radius1, 0f, Mathf.Sin (t1) * radius1);

			for (int side = 0; side <= nbSides; side++) {
				int currSide = side == nbSides ? 0 : side;

				Vector3 normale = Vector3.Cross (r1, Vector3.up);
				float t2 = (float)currSide / nbSides * _2pi;
				Vector3 r2 = Quaternion.AngleAxis (-t1 * Mathf.Rad2Deg, Vector3.up) * new Vector3 (Mathf.Sin (t2) * radius2, Mathf.Cos (t2) * radius2);

				vertices [side + seg * (nbSides + 1)] = r1 + r2;
			}
		}
		#endregion

		#region Normales		
		Vector3[] normales = new Vector3[vertices.Length];
		for (int seg = 0; seg <= nbRadSeg; seg++) {
			int currSeg = seg == nbRadSeg ? 0 : seg;

			float t1 = (float)currSeg / nbRadSeg * _2pi;
			Vector3 r1 = new Vector3 (Mathf.Cos (t1) * radius1, 0f, Mathf.Sin (t1) * radius1);

			for (int side = 0; side <= nbSides; side++) {
				normales [side + seg * (nbSides + 1)] = (vertices [side + seg * (nbSides + 1)] - r1).normalized;
			}
		}
		#endregion

		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int seg = 0; seg <= nbRadSeg; seg++)
			for (int side = 0; side <= nbSides; side++)
				uvs [side + seg * (nbSides + 1)] = new Vector2 ((float)seg / nbRadSeg, (float)side / nbSides);
		#endregion

		#region Triangles
		int nbFaces = vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[ nbIndexes ];

		int i = 0;
		for (int seg = 0; seg <= nbRadSeg; seg++) {			
			for (int side = 0; side <= nbSides - 1; side++) {
				int current = side + seg * (nbSides + 1);
				int next = side + (seg < (nbRadSeg) ? (seg + 1) * (nbSides + 1) : 0);

				if (i < triangles.Length - 6) {
					triangles [i++] = current;
					triangles [i++] = next;
					triangles [i++] = next + 1;

					triangles [i++] = current;
					triangles [i++] = next + 1;
					triangles [i++] = current + 1;
				}
			}
		}
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds ();
		var renderer = gameObject.AddComponent <MeshRenderer> ();
		renderer.material = mat;
		renderer.material.color = c;

		var collider = gameObject.AddComponent <MeshCollider> ();
		//		collider.isTrigger = true;
		return gameObject;
	}

	public static GameObject BuildDoubleTorus (Material mat, Color c, float radius1, float radius2)
	{
		var doubleTorus = new GameObject ("DoubleTorus");
		var torus1 = BuildHoledTorus (mat, c, radius1, radius2);
//		var torus2 = BuildHoledTorus (mat, c, radius1, radius2);
//		torus2.transform.position += Vector3.right * (radius1 * 2 + radius2 * 2);
//		torus2.transform.parent = doubleTorus.transform;
		torus1.transform.parent = doubleTorus.transform;
		return doubleTorus;
	}

	public static GameObject BuildHoledTorus (Material mat, Color c, float radius1, float radius2)
	{
		var gameObject = new GameObject ("Holed Torus");
		//		gameObject.tag = "Sphere";
		MeshFilter filter = gameObject.AddComponent< MeshFilter > ();
		Mesh mesh = filter.mesh;
		mesh.Clear ();

		int nbRadSeg = 240;
		int nbSides = 180;

		#region Vertices		
		Vector3[] vertices = new Vector3[(nbRadSeg + 1) * (nbSides + 1)];
		float tao = Mathf.PI * 2f;
		for (int seg = 0; seg <= nbRadSeg; seg++) {
			int currSeg = seg == nbRadSeg ? 0 : seg;

			float t1 = (float)currSeg / nbRadSeg * tao;

			Vector3 r1 = new Vector3 (Mathf.Cos (t1) * radius1, 0f, Mathf.Sin (t1) * radius1);
			for (int side = 0; side <= nbSides; side++) {
				int currSide = side == nbSides ? 0 : side;
				Vector3 normal = Vector3.Cross (r1, Vector3.up);
				float t2 = (float)currSide / nbSides * tao;
				if (inCircle (tao, t1, t2, (float)rad2 / rad1, 1f)) {
					
				} else {
					var t2Prime = (t2 + tao / 4f) % tao;
					Vector3 r2 = Quaternion.AngleAxis (-t1 * Mathf.Rad2Deg, Vector3.up) * new Vector3 (Mathf.Sin (t2Prime) * radius2, Mathf.Cos (t2Prime) * radius2);
					vertices [side + seg * (nbSides + 1)] = r1 + r2;
				}

			}
		}
		#endregion

		#region Normales		
		Vector3[] normales = new Vector3[vertices.Length];
		for (int seg = 0; seg <= nbRadSeg; seg++) {
			int currSeg = seg == nbRadSeg ? 0 : seg;

			float t1 = (float)currSeg / nbRadSeg * tao;
			Vector3 r1 = new Vector3 (Mathf.Cos (t1) * radius1, 0f, Mathf.Sin (t1) * radius1);

			for (int side = 0; side <= nbSides; side++) {
				normales [side + seg * (nbSides + 1)] = (vertices [side + seg * (nbSides + 1)] - r1).normalized;
			}
		}
		#endregion

		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int seg = 0; seg <= nbRadSeg; seg++)
			for (int side = 0; side <= nbSides; side++)
				uvs [side + seg * (nbSides + 1)] = new Vector2 ((float)seg / nbRadSeg, (float)side / nbSides);
		#endregion

		#region Triangles
		int nbFaces = vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[ nbIndexes ];

		int i = 0;
		for (int seg = 0; seg <= nbRadSeg; seg++) {			
			for (int side = 0; side <= nbSides - 1; side++) {
				int current = side + seg * (nbSides + 1);
				int next = side + (seg < (nbRadSeg) ? (seg + 1) * (nbSides + 1) : 0);
				if (i < triangles.Length - 6) {
					triangles [i++] = current;
					triangles [i++] = next;
					triangles [i++] = next + 1;

					triangles [i++] = current;
					triangles [i++] = next + 1;
					triangles [i++] = current + 1;
				}
			}
		}
		#endregion
		List<int> indices = new List<int> ();
		for (int k = 0; k < nbTriangles; k++) {
			var tri1 = triangles [3 * k];
			var tri2 = triangles [3 * k + 1];
			var tri3 = triangles [3 * k + 2];
			if (vertices [tri1].Equals (Vector3.zero) || vertices [tri2].Equals (Vector3.zero) || vertices [tri3].Equals (Vector3.zero)) {
				indices.Add (tri1);
				indices.Add (tri2);
				indices.Add (tri3);
			}
		}
		List<int> tris = new List<int> ();
		foreach (var item in triangles) {
			if (!indices.Contains (item)) {
				tris.Add (item);
			}
		}

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = tris.ToArray ();

		mesh.RecalculateBounds ();
		var renderer = gameObject.AddComponent <MeshRenderer> ();
		renderer.material = mat;
		renderer.material.color = c;

		var collider = gameObject.AddComponent <MeshCollider> ();
		//		collider.isTrigger = true;
		return gameObject;
	}

	static bool inCircle (float tao, float t1, float t2, float a, float b)
	{
		var t1sqr = t1 * t1 / (a * a);
		var t2sqr = t2 * t2 / (b * b);
		var t1Psqr = (t1 - tao) * (t1 - tao) / (a * a);
		var t2Psqr = (t2 - tao) * (t2 - tao) / (b * b);
		var lowerright = t1sqr + t2sqr < tao / 32f;
		var upperleft = t1Psqr + t2Psqr < tao / 32f;
		var lowerleft = t1Psqr + t2sqr < tao / 32f;
		var upperright = t2Psqr + t1sqr < tao / 32f;
		return lowerright || upperleft || lowerleft || upperright;
	}
}
