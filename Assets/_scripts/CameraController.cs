using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public GameObject player;
	private Vector3 offset;

	private Vector3 mousePos;
	// Use this for initialization
	void Start ()
	{
		offset = transform.position - player.transform.position;
		mousePos = Input.mousePosition;
		var vector = (player.transform.position - transform.position).normalized;
		player.GetComponent<PlayerController> ().direction = new Vector3 (vector.x, 0, vector.z);
	}

	void Update ()
	{
		var direction = mousePos - Input.mousePosition;
		mousePos = Input.mousePosition;
		transform.position = player.transform.position + offset;
		transform.RotateAround (player.transform.position, Vector3.up, direction.x);
//		transform.RotateAround (player.transform.position, Vector3.Cross (offset, Vector3.down), direction.y);
		offset = transform.position - player.transform.position;
		var vector = -offset.normalized;
		player.GetComponent<PlayerController> ().direction = new Vector3 (vector.x, 0, vector.z);
	}
}
