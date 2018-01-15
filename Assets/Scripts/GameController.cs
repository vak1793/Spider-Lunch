using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject spiderPrefab, nodePrefab, strandPrefab, windowPrefab, healthbarPrefab, smallInsect;
	public float playerMoveSpeed;
	float playerHealth;
	Web web;
	GameObject healthBar, currentProgress;

	// Use this for initialization
	void Start () {
		System.Random rndGen = new System.Random();
		float xPos = rndGen.Next(-8, 8), yPos = 8;
		GameObject spider = Instantiate(spiderPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
		healthBar = Instantiate(healthbarPrefab, new Vector3(9, 7, 0), Quaternion.identity);
		currentProgress = healthBar.transform.Find("healthbar_progress").gameObject;

		web = spider.GetComponent<Web>();
		web.setPrefabs(nodePrefab, strandPrefab, smallInsect);

		Instantiate(windowPrefab, Vector3.zero, Quaternion.identity);

		// InvokeRepeating("CreateInsect", 3.0f, 0f);
		Invoke("CreateInsect", 5);
	}

	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player){
			playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Spider>().GetHealth();
			float scale = playerHealth >= 0 ? (playerHealth / 100) : 0;
			Color progressColor = Color.Lerp(
				new Color(210, 20, 4, 1),
				new Color(57, 255, 20, 1),
				scale
			);
			currentProgress.GetComponent<SpriteRenderer>().color = progressColor;
			currentProgress.transform.localScale = new Vector3(scale, 1, 1);
		}
	}

	void CreateInsect(){
		web.CreateInsect();
	}
}
