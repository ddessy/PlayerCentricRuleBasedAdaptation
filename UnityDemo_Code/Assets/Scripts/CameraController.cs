using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject GamePlayer;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - GamePlayer.transform.position;
	}
	
	void LateUpdate () {
		transform.position = GamePlayer.transform.position + offset;
	}
}
