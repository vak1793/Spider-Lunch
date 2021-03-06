using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insect : MonoBehaviour {

	public float insectMoveSpeed, breakOutTime, energyValue;
	bool isStuck = false, hasGottenStuck = false;
	Time collideTime;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if(!isStuck){
			Vector3 destination = transform.position;
			destination = new Vector3(destination.x, destination.y, 15);

			// if(transform.position.z < -5){
			// 	GetComponent<Renderer>().enabled = false;
			// } else {
			// 	GetComponent<Renderer>().enabled = true;
			// }
			if(transform.position.z > 0){
				float newScale = 1 - (transform.position.z / 18.75f);
				// Debug.Log(string.Format("newScale = {0}", newScale));
				transform.localScale = new Vector3(newScale, newScale, 1);
			} else {
				float newScale = 1 - (4 * transform.position.z / 11);
				transform.localScale = new Vector3(newScale, newScale, 1);
			}
			transform.position = Vector3.MoveTowards(transform.position, destination, (Time.deltaTime * insectMoveSpeed));
		}

		if(transform.position.z >= 10){
			Destroy(gameObject);
		}
	}

	void OnTriggerStay2D(){
		// insect gets stuck on strand
		if(Mathf.Abs(transform.position.z) < 0.2 && !hasGottenStuck){
			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);

			GameObject go;

	    if(colliders.Length > 1) {
	      foreach(var collider in colliders) {
	        go = collider.gameObject;
	        if(go == gameObject) continue;
	        if(go.tag == "Edge") {
						// Debug.Log(string.Format("{0} hit {1}", gameObject.name, go.name));
						string[] splitName = go.name.Split(new string[] {"d"}, StringSplitOptions.RemoveEmptyEntries);
						int strandIndex = Int32.Parse(splitName[1]) + 3;
						// Debug.Break();
						// transform.position =
	          isStuck = true;
						hasGottenStuck = true;
						Invoke("ReleaseInsect", breakOutTime);
	        }
	      }
	    }
		}

		if(isStuck){
			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);

			GameObject go;

	    if(colliders.Length > 1) {
	      foreach(var collider in colliders) {
	        go = collider.gameObject;
	        if(go == gameObject) continue;
	        if(go.tag == "Player") {
						// TODO: Update player score
						go.GetComponent<Spider>().RestoreHealth(energyValue);
	          Destroy(gameObject);
	        }
	      }
	    }
		}
	}

	public void ReleaseInsect(){
		Debug.Log("Released insect");
		isStuck = false;
	}
}
