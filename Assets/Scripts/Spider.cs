using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour {

	public float playerMoveSpeed, playerHealth, healthDecreaseRate;
	bool playerIsMoving, spiderIsAlive = true;
	Vector3 positionToMove, nextNode, mousePosition, moveDirection;
	List<Node> playerMovePath;
	Plane plane;
	Web web;
	Animator animator;
	Vector3 touchStart, touchEnd;

	// Use this for initialization
	void Start () {
		web = GetComponent<Web>();
		playerMovePath = new List<Node>();
		animator = GetComponent<Animator>();
		positionToMove = transform.position;
		plane = new Plane(Vector3.back, Vector3.zero);
	}

	// Update is called once per frame
	void Update () {
		Vector3 mPos;
    Ray ray;
    float ray_distance;

		// Reduce player health with time
		playerHealth -= (Time.deltaTime * healthDecreaseRate);
		// Debug.Log(string.Format("Player health = {0}", playerHealth));

		if(spiderIsAlive){
			// Player health check
			if(playerHealth <= 0){
				spiderIsAlive = false;
				animator.Play("spider_die");
				Debug.Log("Game over!");
				StartCoroutine(KillSpider());
			}

			// Player movement
			if(transform.position != positionToMove) {
				animator.Play("spider_walk_down");
				playerIsMoving = true;

				if(playerMovePath.Count > 0){
	        playerIsMoving = true;
	        Node nodeToMove = playerMovePath[0];
	        nextNode = new Vector3(nodeToMove.xPos(), nodeToMove.yPos(), 0);
	        transform.position = Vector3.MoveTowards(transform.position, nextNode, (Time.deltaTime * playerMoveSpeed));

					moveDirection = nextNode - transform.position;
					transform.rotation = Quaternion.AngleAxis(
						Vector3.SignedAngle(Vector3.down, moveDirection, Vector3.forward),
						Vector3.forward
					);

	        if((transform.position - nextNode).magnitude < 0.1){
	          playerMovePath.RemoveAt(0);
	        }
	      } else {
	        transform.position = Vector3.MoveTowards(transform.position, positionToMove, (Time.deltaTime * playerMoveSpeed));
					moveDirection = positionToMove - transform.position;
					transform.rotation = Quaternion.AngleAxis(
						Vector3.SignedAngle(Vector3.down, moveDirection, Vector3.forward),
						Vector3.forward
					);
	        if((transform.position - positionToMove).magnitude < 0.1){
	          playerIsMoving = false;
	        } else {
	          playerIsMoving = true;
	        }
	      }

				// transform.position = Vector3.MoveTowards(transform.position, positionToMove, (Time.deltaTime * playerMoveSpeed));
				// moveDirection = positionToMove - transform.position;
				// transform.rotation = Quaternion.AngleAxis(
				// 	Vector3.SignedAngle(Vector3.down, moveDirection, Vector3.forward),
				// 	Vector3.forward
				// );
			}

			if(Vector3.Distance(transform.position, positionToMove) < 0.1f){
				transform.position = positionToMove;
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
				animator.Play("spider_idle");
				playerIsMoving = false;
			}

			// On user input
			if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
				// if(playerIsMoving) { return; }
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (plane.Raycast(ray, out ray_distance)) {
					mPos = ray.GetPoint(ray_distance);
	        touchStart = new Vector3((float) System.Math.Round(mPos.x, 3), (float)System.Math.Round(mPos.y, 3), 0);
				}
			}

			if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)){
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (plane.Raycast(ray, out ray_distance)) {
					mPos = ray.GetPoint(ray_distance);
	        touchEnd = new Vector3((float) System.Math.Round(mPos.x, 3), (float)System.Math.Round(mPos.y, 3), 0);
					Strand clickedStrand, clickedFrame;
					// Debug.Log(string.Format("start = {0}, end = {1}", touchStart.ToString(), touchEnd.ToString()));
					// Debug.Log(string.Format("start == end? {0}", touchStart == touchEnd));

					if(touchStart == touchEnd){
						// player tapped screen, move to position
						if(web.PointIsOnFrame(touchEnd, out clickedFrame)) {
							// if mouseDownPosition is on a strand then move
							// Debug.Log(string.Format("frame -> {0}", clickedFrame.PositionString()));
							positionToMove = clickedFrame.ClosestPoint(touchEnd);
							// Debug.Log(string.Format("clicked = {0}, positionToMove = {1}", touchEnd.ToString(), positionToMove.ToString()));
							playerMovePath = web.GetPlayerMovePath(transform.position, positionToMove);
						} else if(web.StrandListContainsPoint(touchEnd, out clickedStrand)) {
							// if mouseDownPosition is on a strand then move
							// Debug.Log(string.Format("strandList -> {0}", clickedStrand.PositionString()));
							positionToMove = clickedStrand.ClosestPoint(touchEnd);
							playerMovePath = web.GetPlayerMovePath(transform.position, positionToMove);
						}
					} else {
						// player has swiped, draw web along swiped direction
						if(playerIsMoving) { return; }
						Vector3 finish = web.LineIntersectsWindowframeAt(transform.position, touchEnd); //mousePosition - transform.position;
						if(finish.magnitude != 0){
							web.nameTBD(transform.position, finish);
						}
						// Debug.Log("back in spider script");
						// string debugString = "";
						// foreach (var g in GameObject.FindGameObjectsWithTag("Edge")) {
						// 	debugString += string.Format("{0}, ", g.name);
						// }
						// Debug.Log(debugString);
						// web.ShowStrandList();
					}
				}
			}
		}

	}

	IEnumerator KillSpider() {
		yield return new WaitForSeconds(2);
		Destroy(gameObject);
	}

	bool PlayerIsMoving() {
		return playerIsMoving;
	}

	public void SetMoveSpeed(float speed){
		playerMoveSpeed = speed;
	}

	public float GetHealth(){
		return playerHealth;
	}

	public void RestoreHealth(float amount){
		playerHealth += amount;
		if(playerHealth > 100){
			playerHealth = 100;
		}
	}
}
