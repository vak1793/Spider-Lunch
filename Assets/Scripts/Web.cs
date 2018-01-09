using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour {

	public GameObject nodePrefab, strandPrefab;

	List<Node> nodeList;
	List<Strand> strandList;
	GameObject startNode = null, endNode = null, drawnStrand = null;
	bool existingStartNode = false, existingEndNode = false;
	float[,] adjacencyMatrix;
	Graph webState;

	// Use this for initialization
	void Start () {
		nodeList = new List<Node>();
		strandList = new List<Strand>();

		float frameX = 8;
    float frameY = 8;

    Node topRight, topLeft, bottomRight, bottomLeft;
    topRight = new Node(frameX, frameY);
    topLeft = new Node(-1 * frameX, frameY);
    bottomRight = new Node(frameX, -1 * frameY);
    bottomLeft = new Node(-1 * frameX, -1 * frameY);

    nodeList.Add(topRight);
    nodeList.Add(topLeft);
    nodeList.Add(bottomRight);
    nodeList.Add(bottomLeft);

    Strand topWall, rightWall, bottomWall, leftWall;

    topWall = new Strand(topLeft, topRight);
    rightWall = new Strand(topRight, bottomRight);
    bottomWall = new Strand(bottomRight, bottomLeft);
    leftWall = new Strand(bottomLeft, topLeft);

    strandList.Add(topWall);
    strandList.Add(rightWall);
    strandList.Add(bottomWall);
    strandList.Add(leftWall);

		UpdateAdjacencyMatrix();
	}

	// Update is called once per frame
	void Update () {

	}

	public void nameTBD(Vector3 begin, Vector3 finish) {
		drawnStrand = drawStrandBetween(begin, finish, true);

		int strandIndex;
		string[] splitName = drawnStrand.name.Split(new string[] {"d"}, StringSplitOptions.RemoveEmptyEntries);
		// foreach(string s in splitName){
		// 	Debug.Log(s);
		// }
		strandIndex = Int32.Parse(splitName[1]) + 3;
		// Debug.Log(string.Format("strandList.Count = {0}, strandIndex = {1}", strandList.Count, strandIndex));

		if(strandIndex > strandList.Count){
			return;
		}
		Strand addedStrand = strandList[strandIndex];

		Strand startStrand, endStrand;
		bool startStrandValid = StrandListContainsPoint(begin, out startStrand);
		bool endStrandValid = StrandListContainsPoint(finish, out endStrand);
		List<GameObject> intersectNodes = new List<GameObject>(), splitStrands = null;

		bool existingStartNodeCopy = existingStartNode, existingEndNodeCopy =  existingEndNode;

		Vector3 intersectPt = Vector3.zero;
		bool drawnStrandIsSplit = false;
		for(int i = strandList.Count-1; i >= 0; i--) {
			Strand str = strandList[i];
			// Skipping self
			if(str.Equals(addedStrand)){ continue; };
			if(str.Equals(startStrand) && !existingStartNodeCopy){
				splitStrands = SplitStrand(str, new Node[] { addedStrand.GetStartNode() });
			}
			if(str.Equals(endStrand) && !existingEndNodeCopy){
				splitStrands = SplitStrand(str, new Node[] { addedStrand.GetEndNode() });
			}
			if(addedStrand.Intersects(str, out intersectPt)) {
				splitStrands = SplitStrand(str, new Node[] { new Node(intersectPt.x, intersectPt.y) });
				GameObject newNode = Instantiate(nodePrefab, intersectPt, Quaternion.identity) as GameObject;
				// newNode.GetComponent<Renderer>().enabled = false;
				intersectNodes.Add(newNode);

				newNode.name = string.Format("Node{0}", nodeList.Count - 3);
				nodeList.Add(new Node(intersectPt.x, intersectPt.y));
			}
		}

		if(intersectNodes.Count > 0){
			drawnStrandIsSplit = true;
			Node[] iNodes = new Node[intersectNodes.Count];
			// Debug.Log(string.Format("{0} intersection points", iNodes.Length));
			for(int j = 0; j < intersectNodes.Count; j++){
				GameObject goTemp = intersectNodes[j];

				iNodes[j] = new Node(goTemp.transform.position.x, goTemp.transform.position.y);
			}
			// string debugString = "Split strands are: ";
			splitStrands = SplitStrand(addedStrand, iNodes, false);
			// foreach(var split in splitStrands){
			// 	debugString +=  string.Format("{0} at {1}", split.name, split.transform.position.ToString());
			// }
			// Debug.Log(debugString);

			// string dString = "";
			// foreach (var g in GameObject.FindGameObjectsWithTag("Edge")) {
			// 	dString += string.Format("{0}, ", g.name);
			// }
			// Debug.Log(dString);
			// Destroy(drawnStrand);
		}
		// Debug.Log("after destroying original");
		// string debugString = "";
		// foreach (var g in GameObject.FindGameObjectsWithTag("Edge")) {
		// 	debugString += string.Format("{0}, ", g.name);
		// }
		// Debug.Log(debugString);

		drawnStrand = null;
		UpdateAdjacencyMatrix();
	}

	public GameObject drawStrandBetween(Vector3 begin, Vector3 finish, bool addEndNodes = true) {
		Node startPoint = new Node(begin.x, begin.y), endPoint = new Node(finish.x, finish.y);

		// Drawing to screen
		if(addEndNodes){
			int startIndex, endIndex;

			if(nodeListContainsAt(startPoint, out startIndex)){
				startPoint = nodeList[startIndex];
				existingStartNode = true;
			} else {
				startNode = Instantiate(nodePrefab, begin, Quaternion.identity) as GameObject;
				startNode.name = string.Format("Node{0}", nodeList.Count - 3);
				nodeList.Add(startPoint);
			}

			if(nodeListContainsAt(endPoint, out endIndex)){
				endPoint = nodeList[endIndex];
				existingEndNode = true;
			} else {
				endNode = Instantiate(nodePrefab, finish, Quaternion.identity) as GameObject;
				endNode.name = string.Format("Node{0}", nodeList.Count - 3);
				nodeList.Add(endPoint);
			}

			UpdateAdjacencyMatrix();
		}

		float startX = startPoint.xPos(), startY = startPoint.yPos(), endX = endPoint.xPos(), endY = endPoint.yPos();
		float lineDistance = Mathf.Sqrt(Mathf.Pow(endX - startX, 2) + Mathf.Pow(endY - startY, 2));
		float lineAngle = Mathf.Atan2(endY - startY, endX - startX) * (180 / Mathf.PI);
		float spriteSize = strandPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
		float scale = lineDistance / spriteSize;

		Strand strandToAdd = new Strand(startPoint, endPoint);

		drawnStrand = Instantiate(strandPrefab, begin, Quaternion.AngleAxis(lineAngle, Vector3.forward)) as GameObject;
		// Debug.Log(string.Format("strandList.Count = {0}, naming new strand as {1}", strandList.Count, strandList.Count - 3));
		drawnStrand.name = string.Format("Strand{0}", strandList.Count - 3);
		strandList.Add(strandToAdd);
		drawnStrand.transform.localScale = new Vector3(
			scale,
			drawnStrand.transform.localScale.y,
			drawnStrand.transform.localScale.z
		);
		return drawnStrand;
	}

	public Vector3 LineIntersectsWindowframeAt(Vector3 begin, Vector3 finish) {
		Strand segment = new Strand(
			new Node(begin.x, begin.y),
			new Node(finish.x, finish.y)
		);

		Vector3 direction = (finish - begin).normalized;
		float m = segment.getLineM(), c = segment.getLineC();

		Vector3 topIntersect, leftIntersect, bottomIntersect, rightIntersect, result = Vector3.zero;

		topIntersect = new Vector3((float) (8 - c) / m, 8, 0);
		leftIntersect = new Vector3(-8 , (float) (m * -8) + c, 0);
		bottomIntersect = new Vector3((float) (-8 - c) / m, -8, 0);
		rightIntersect = new Vector3(8 , (float) (m * 8) + c, 0);

		Vector3[] intersects = new Vector3[] { topIntersect, leftIntersect, bottomIntersect, rightIntersect };

		float angle;
		foreach(Vector3 pt in intersects) {
			Vector3 otherDirection = (pt - finish).normalized;
			angle = Vector3.Angle(otherDirection, direction);

			if(angle == 0 && (pt.x <= 8 && pt.x >= -8) && (pt.y <= 8 && pt.y >= -8)){
				result = pt;
			}
		}

		return result;
	}

	public bool PointIsOnFrame(Vector3 point) {
		bool onFrame = false;
		if(point.x == 8 || point.y == 8 || point.x == -8 || point.y == -8){
			onFrame = true;
		}
		return onFrame;
	}

	public Vector3 ClosestDirectionAxis(Vector3 direction) {
		Vector3 upLeft = new Vector3(-1, 1, 0).normalized;
		Vector3 upRight = new Vector3(1, 1, 0).normalized;
		Vector3 downLeft = new Vector3(-1, -1, 0).normalized;
		Vector3 downRight = new Vector3(1, -1, 0).normalized;

		Vector3[] axes = new Vector3[] {
			Vector3.up, upRight,
			Vector3.right, downRight,
			Vector3.down, downLeft,
			Vector3.left, upLeft
		};

		Vector3 closest = Vector3.zero;

		float leastAngle = float.MaxValue;
		foreach(var axis in axes) {
			if(Vector3.Angle(direction, axis) < leastAngle) {
				leastAngle = Vector3.Angle(direction, axis);
				closest = axis;
			}
		}
		return closest;
	}

	public bool StrandListContainsPoint(Vector3 pos, out Strand str){
		bool pointOnStrand = false;
    str = null;
    foreach (Strand s in strandList){
      if(s.ContainsPointWithEnds(pos.x, pos.y)){
        pointOnStrand = true;
        str = s;
      }
    }
    return pointOnStrand;
  }

	bool nodeListContainsAt(Node other, out int index){
    index = -1;

    for(int i = 0; i< nodeList.Count; i++){
      if(other.Equals(nodeList[i])){
        index = i;
        return true;
      }
    }

    return false;
  }

	public bool StrandExistsBetween(Node otherStart, Node otherEnd) {
    Strand strandToCompare = new Strand(otherStart, otherEnd);
    bool strandExists = false;

    foreach(Strand s in strandList) {
      bool strandContainsStartPoint = s.ContainsPointWithEnds(otherStart.xPos(), otherStart.yPos());
      bool strandContainsEndPoint = s.ContainsPointWithEnds(otherEnd.xPos(), otherEnd.yPos());
      bool isReqdStrand = s.Equals(strandToCompare);

      if(isReqdStrand || (strandContainsStartPoint && strandContainsEndPoint)){
        strandExists = true;
      }
    }

    return strandExists;
  }

	public void UpdateAdjacencyMatrix(){
    int matrixSize = nodeList.Count;
    adjacencyMatrix = new float[matrixSize, matrixSize];

    for (int i = 0; i < matrixSize; i++) {
      for (int j = 0; j < matrixSize; j++) {
        adjacencyMatrix[i, j] = 0;
      }
    }

    for (int i = 0; i < matrixSize - 1; i++) {
      for (int j = i + 1; j < matrixSize; j++) {
        bool validPath = StrandExistsBetween(nodeList[i], nodeList[j]);
        if(validPath){
          // multiplying with distance for greedy algorithm
          float dist = nodeList[i].DistanceFrom(nodeList[j]);
          adjacencyMatrix[i, j] = dist;
          adjacencyMatrix[j, i] = dist;
        }
      }
    }
  }

	public List<Node> GetPlayerMovePath(Vector3 playerStart, Vector3 playerDestination){
    Strand startStrand, endStrand;
    Node s1, s2, e1, e2;
    int startIndex, finishIndex;
    List<Node> path1, path2, path3, path4, playerMovePath;

    path1 = new List<Node>();
    path2 = new List<Node>();
    path3 = new List<Node>();
    path4 = new List<Node>();
		playerMovePath = new List<Node>();

    int matrixSize = adjacencyMatrix.GetLength(0);

    Dictionary<Node, Dictionary<Node, float>> vertexList = new Dictionary<Node, Dictionary<Node, float>>();
    Dictionary<Node, float> nodeMap = new Dictionary<Node, float>();
    for(int i = 0; i < matrixSize; i++){
      for(int j = 0; j < matrixSize; j++){
        if(adjacencyMatrix[i,j] > 0){
          nodeMap.Add(nodeList[j], adjacencyMatrix[i,j]);
        }
      }
      vertexList[nodeList[i]] = new Dictionary<Node, float>(nodeMap);
      nodeMap.Clear();
    }

    if(StrandListContainsPoint(playerStart, out startStrand)
      && StrandListContainsPoint(playerDestination, out endStrand)){

        if(startStrand.Equals(endStrand)){
          return playerMovePath;
        }

        s1 = startStrand.GetStartNode();
        s2 = startStrand.GetEndNode();
        e1 = endStrand.GetStartNode();
        e2 = endStrand.GetEndNode();

        bool startOnFrame, endOnFrame, startStrandIsFrame = false, endStrandIsFrame = false;

        for(int i = 0; i < 4; i++){
          startOnFrame = strandList[i].ContainsPointWithEnds(s1.xPos(), s1.yPos());
          endOnFrame = strandList[i].ContainsPointWithEnds(s2.xPos(), s2.yPos());
          if(startOnFrame && endOnFrame){
            startStrandIsFrame = true;
          }
          startOnFrame = strandList[i].ContainsPointWithEnds(e1.xPos(), e1.yPos());
          endOnFrame = strandList[i].ContainsPointWithEnds(e2.xPos(), e2.yPos());
          if(startOnFrame && endOnFrame){
            endStrandIsFrame = true;
          }
        }

        Node temp;
        if(startStrandIsFrame && nodeList.Count > 4){
          // Debug.Log("start is frame");
          temp = startStrand.GetClosestNodeOnStrand(playerStart, nodeList);
          if(temp.xPos() != float.MaxValue && temp.yPos() != float.MaxValue){
            s1 = temp;
            s2 = startStrand.GetClosestEnd(playerStart);
          }
        }
        if(endStrandIsFrame && nodeList.Count > 4){
          // Debug.Log("end is frame");
          temp = endStrand.GetClosestNodeOnStrand(playerDestination, nodeList);
          if(temp.xPos() != float.MaxValue && temp.yPos() != float.MaxValue){
            e1 = temp;
            e2 = endStrand.GetClosestEnd(playerDestination);
          }
        }

        Node playerStartNode = new Node(playerStart.x, playerStart.y),
             playerDestinationNode = new Node(playerDestination.x, playerDestination.y);

        if(nodeListContainsAt(playerStartNode, out startIndex)){
          // Debug.Log(string.Format("Player position on existing node at {0}", playerStartNode.PositionString()));
          s1 = playerStartNode;
          s2 = playerStartNode;
        }
        // end node is always existing!
        if(nodeListContainsAt(playerDestinationNode, out finishIndex)){
          // Debug.Log(string.Format("Destination is existing node at {0}", playerDestinationNode.PositionString()));
          e1 = playerDestinationNode;
          e2 = playerDestinationNode;
        }

        webState = new Graph(vertexList);

        if(nodeListContainsAt(s1, out startIndex) && nodeListContainsAt(e1, out finishIndex)){
          if(s1.Equals(e1)){
            path1.Add(s1);
          } else {
            path1 = webState.ShortestPath(s1, e1);
          }
        }

        if(nodeListContainsAt(s1, out startIndex) && nodeListContainsAt(e2, out finishIndex)){
          if(s1.Equals(e2)){
            path2.Add(s1);
          } else {
            path2 = webState.ShortestPath(s1, e2);
          }
        }

        if(nodeListContainsAt(s2, out startIndex) && nodeListContainsAt(e1, out finishIndex)){
          if(s2.Equals(e1)){
            path3.Add(s2);
          } else {
            path3 = webState.ShortestPath(s2, e1);
          }
        }

        if(nodeListContainsAt(s2, out startIndex) && nodeListContainsAt(e2, out finishIndex)){
          if(s2.Equals(e2)){
            path4.Add(s2);
          } else {
            path4 = webState.ShortestPath(s2, e2);
          }
        }

        List<Node>[] paths = new List<Node>[] { path1, path2, path3, path4 };
        playerMovePath = ShortestDistance(paths, playerStart, playerDestination);

        // string pathlist = "";
        // foreach(var moveNode in playerMovePath){
        //   if(playerMovePath.FindIndex(a => a.Equals(moveNode)) == playerMovePath.Count-1){
        //     pathlist += string.Format("{0}", moveNode.PositionString());
        //   } else {
        //     pathlist += string.Format("{0} => ", moveNode.PositionString());
        //   }
        // }
        // Debug.Log(pathlist);
    }

		return playerMovePath;
  }

	List<Node> ShortestDistance(List<Node>[] paths, Vector3 start, Vector3 finish){
    float minDistance = float.MaxValue;
    int positionOfPath = -1, counter = 0;
    List<Node>[] pathsToCheck = paths;

    if(pathsToCheck.Length == 0){
      return new List<Node>();
    }

    foreach (List<Node> nodes in pathsToCheck){
      if(nodes == null) { continue; }
      List<Node> path = new List<Node>(nodes);
      if(path.Count > 0){
        float pathDistance = 0;
        // Vector3 playerPos = player.transform.position, destPos = positionToMove;
        Node pStart = new Node(start.x, start.y), pEnd = new Node(finish.x, finish.y);
        for(int i=0; i < path.Count-1; i++){
         pathDistance += path[i].DistanceFrom(path[i+1]);
        }
        pathDistance += path[0].DistanceFrom(pStart);
        pathDistance += path[path.Count-1].DistanceFrom(pEnd);

        if(pathDistance <= minDistance && path.Count > 0){
          positionOfPath = counter;
          minDistance = pathDistance;
        }
      }
      counter++;
    }

    return pathsToCheck[positionOfPath];
  }

	List<GameObject> SplitStrand(Strand str, Node[] n, bool hidden = false){
    List<GameObject> splits = new List<GameObject>();
    int strandIndex = strandList.IndexOf(str);

    if(strandIndex < 4){
      return splits;
    }
    // float startX, startY, endX, endY;

    GameObject strandGO = GameObject.Find(string.Format("Strand{0}", strandIndex - 3));
		Debug.Log(string.Format("Found {0}", strandGO.name));
		strandGO.name = "Delete_Me";
    Node startN = str.GetStartNode(), endN = str.GetEndNode();

		string debugString = "Before delete\n";
		foreach (var g in GameObject.FindGameObjectsWithTag("Edge")) {
			debugString += string.Format("{0}, ", g.name);
		}
		Debug.Log(debugString);

    strandList.RemoveAt(strandIndex);
    Destroy(strandGO);
    strandGO = null;

		debugString = "After delete\n";
		foreach (var g in GameObject.FindGameObjectsWithTag("Edge")) {
			debugString += string.Format("{0}, ", g.name);
		}
		Debug.Log(debugString);

    for(int i = strandIndex; i < strandList.Count; i++){
      GameObject go = GameObject.Find(string.Format("Strand{0}", i - 2));
      go.name = string.Format("Strand{0}", i - 3);
    }

		debugString = "After Rename\n";
		foreach (var g in GameObject.FindGameObjectsWithTag("Edge")) {
			debugString += string.Format("{0}, ", g.name);
		}
		Debug.Log(debugString);

		// GameObject[] renamedStrands = GameObject.FindGameObjectsWithTag("Edge");
		// foreach(GameObject rgo in renamedStrands){
		// 	Debug.Log(rgo.name);
		// }

    // startX = startN.xPos();
    // startY = startN.yPos();
    // endX = endN.xPos();
    // endY = endN.yPos();

    GameObject ds;
    ds = drawStrandBetween(startN.Position(), n[0].Position(), false);
    ds.GetComponent<Renderer>().enabled = !hidden;
    splits.Add(ds);

    for(int i = 0; i < n.Length - 1; i++){
      ds = drawStrandBetween(n[i].Position(), n[i+1].Position(), false);
      ds.GetComponent<Renderer>().enabled = !hidden;
      splits.Add(ds);
    }

    ds = drawStrandBetween(n[n.Length-1].Position(), endN.Position(), false);
    ds.GetComponent<Renderer>().enabled = !hidden;
    splits.Add(ds);

    return splits;
  }

	public void ShowStrandList() {
		string debugString = string.Format("{0} strands :", strandList.Count);

    foreach(var s in strandList){
      debugString += string.Format("{0}\n", s.PositionString());
    }
		Debug.Log(debugString);
  }
}
