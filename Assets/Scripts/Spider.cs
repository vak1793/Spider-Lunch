using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour {

	public float energy;
	Vector3 positionToMove;

	// Use this for initialization
	void Start () {
		positionToMove = transform.position;
	}

	// Update is called once per frame
	void Update () {
	// 	if(transform.position != positionToMove) {
  //    transform.position = Vector3.Lerp (transform.position, positionToMove, 0.3f);
  //  }
	}

	public void SetPositionToMove(Vector3 pos){
		Debug.Log(string.Format("Spider should move to {0}", pos.ToString()));
		this.positionToMove = pos;
	}
}
