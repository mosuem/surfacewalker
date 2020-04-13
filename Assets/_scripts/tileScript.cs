using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileScript : MonoBehaviour
{
	public List<GameObject> pickUps = new List<GameObject> ();
	// Use this for initialization
	void Start ()
	{
		var container = transform.Find ("Pick Ups");
		foreach (var child in container) {
			Debug.Log (child);
			if (((Transform)child).gameObject.CompareTag ("Pick Up")) {
				pickUps.Add (((Transform)child).gameObject);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
