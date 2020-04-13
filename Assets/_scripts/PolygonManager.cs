using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PolygonManager
{
	public FundamentalPolygon polygon;

	public PolygonManager (int p, int q, Material mat)
	{
		polygon = new FundamentalPolygon (p, q, mat, null, null, null);
		polygon.toKleinBeltrami (true);
		polygon.foldOut (3);

//		foreach (var item in polygon.polygons) {
//			item.foldOut ();
//		}
	}
}