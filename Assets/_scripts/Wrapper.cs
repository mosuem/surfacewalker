using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper : MonoBehaviour
{
	public GameObject mainTile;
	public GameObject mainPlayer;
	static public bool isTorus = false;
	static public bool isEuclidean = true;
	public static Material mat;
	// Update is called once per frame
	void Update ()
	{
//		var halfSize = mainTile.GetComponent<MeshRenderer> ().bounds.size.x / 2f;
//		if (isTorus) {
//			if (mainPlayer.transform.position.x >= halfSize) {
//				mainPlayer.transform.position = new Vector3 (-halfSize, mainPlayer.transform.position.y, mainPlayer.transform.position.z);
//			} else if (mainPlayer.transform.position.x <= -halfSize) {
//				mainPlayer.transform.position = new Vector3 (halfSize, mainPlayer.transform.position.y, mainPlayer.transform.position.z);
//			}
//			if (mainPlayer.transform.position.z >= halfSize) {
//				mainPlayer.transform.position = new Vector3 (mainPlayer.transform.position.x, mainPlayer.transform.position.y, -halfSize);
//			} else if (mainPlayer.transform.position.z <= -halfSize) {
//				mainPlayer.transform.position = new Vector3 (mainPlayer.transform.position.x, mainPlayer.transform.position.y, halfSize);
//			}
//		} else {
//			if (mainPlayer.transform.position.x >= halfSize) {
//				mainPlayer.transform.position = new Vector3 (-halfSize, mainPlayer.transform.position.y, mainPlayer.transform.position.z);
//			} else if (mainPlayer.transform.position.x <= -halfSize) {
//				mainPlayer.transform.position = new Vector3 (halfSize, mainPlayer.transform.position.y, mainPlayer.transform.position.z);
//			}
//			if (mainPlayer.transform.position.z >= halfSize) {
//				var velocity = mainPlayer.GetComponent<Rigidbody> ().velocity;
//				mainPlayer.GetComponent<Rigidbody> ().velocity = new Vector3 (-velocity.x, velocity.y, velocity.z);
//				mainPlayer.transform.position = new Vector3 (-mainPlayer.transform.position.x, mainPlayer.transform.position.y, -halfSize);
//			} else if (mainPlayer.transform.position.z <= -halfSize) {
//				var velocity = mainPlayer.GetComponent<Rigidbody> ().velocity;
//				mainPlayer.GetComponent<Rigidbody> ().velocity = new Vector3 (-velocity.x, velocity.y, velocity.z);
//				mainPlayer.transform.position = new Vector3 (-mainPlayer.transform.position.x, mainPlayer.transform.position.y, halfSize);
//			}
//		}
	}
}
