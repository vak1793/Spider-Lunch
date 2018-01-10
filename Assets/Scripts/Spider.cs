using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour {

	public float playerMoveSpeed;
	bool playerIsMoving;
	Vector3 positionToMove, nextNode, mousePosition, moveDirection;
	List<Node> playerMovePath;
	Plane plane;
	Web web;
	Animator animator;

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
        mousePosition = new Vector3((float) System.Math.Round(mPos.x, 3), (float)System.Math.Round(mPos.y, 3), 0);
				Strand clickedStrand;

				if(web.StrandListContainsPoint(mousePosition, out clickedStrand) || web.PointIsOnFrame(mousePosition)) {
					// if mouseDownPosition is on a strand then move
					// positionToMove = mousePosition;
					positionToMove = clickedStrand.ClosestPoint(mPos);
					playerMovePath = web.GetPlayerMovePath(transform.position, positionToMove);
				} else {
					// else draw web from current position along direction of click
					Vector3 finish = web.LineIntersectsWindowframeAt(transform.position, mousePosition); //mousePosition - transform.position;
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

	bool PlayerIsMoving() {
		return playerIsMoving;
	}
}
