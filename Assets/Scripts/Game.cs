using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

  public GameObject node, strand, windowFrame, spider;
  GameObject node1 = null, node2 = null, drawnStrand = null;
  GameObject player;
  List<GameObject> intersectNodes, splitStrands;
  Plane plane;
  GameObject startNode, endNode;
  bool playerIsMoving, newStrandDrawn, drawnStrandIsSplit, existingNode = false;
  Strand startStrand = null, endStrand = null;
  List<Node> nodeList;
  List<Strand> strandList;
  Vector3 positionToMove;
  int[,] adjacencyMatrix;

  // Use this for initialization
  void Start () {
    System.Random rnd = new System.Random();
    float spiderStartX, spiderStartY;
    spiderStartX = rnd.Next(-8, 8);
    spiderStartY = 8;
    Vector3 spiderPos = new Vector3(spiderStartX, spiderStartY, 0);

    nodeList = new List<Node>();
    strandList = new List<Strand>();
    intersectNodes = new List<GameObject>();
    splitStrands = new List<GameObject>();
    UpdateAdjacencyMatrix();

    // as per camera othographic size
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

    Instantiate(windowFrame, Vector3.zero, Quaternion.identity);

    player = Instantiate(spider, spiderPos, Quaternion.identity) as GameObject;
    player.name = "Player";
    positionToMove = spiderPos;
    // Debug.Log("Initialized player, nowhere to move yet");
    playerIsMoving = false;
    newStrandDrawn = true;

    plane = new Plane(Vector3.back, GameObject.FindGameObjectWithTag("GameController").transform.position);
  }

  // Update is called once per frame
  void Update () {
    Vector3 mPos;
    Ray ray;
    float ray_distance;

    if (Input.GetMouseButtonDown(0)) {
      // if(playerIsMoving){ return; }

      ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (plane.Raycast(ray, out ray_distance)) {
        mPos = ray.GetPoint(ray_distance);
        mPos = new Vector3((float) System.Math.Round(mPos.x, 3), (float)System.Math.Round(mPos.y, 3), 0);
        // Debug.Log(string.Format("Clicked at ({0},{1})", mPos.x, mPos.y));
        Strand clickedStrand = null;
        bool clickedOnExistingStrand = strandListContainsPoint(mPos, out clickedStrand);
        bool clickedOnExistingNode = GetOverlappingObjects(out node1, "Vertex", mPos, 0.1f);

        // Spider spiderScript = spider.GetComponent<Spider>();

        if(clickedOnExistingNode) {
          // Debug.Log(string.Format("Clicked on existing node: {0}", node1.name));
          existingNode = true;
        } else if(clickedOnExistingStrand) {
          startStrand = clickedStrand;
          // Debug.Log(string.Format("Clicked on {0}", clickedStrand.PositionString()));
          //Debug.Log(string.Format("mPos = {0}", mPos.ToString()));
          Vector3 pos = clickedStrand.ClosestPoint(mPos);
          // Debug.Log(string.Format("Closest point to mPos on strand: {0}", pos.ToString()));
          // Debug.Log(
          //   string.Format(
          //     "Approximated {0} as {1} on {2}",
          //     mPos.ToString(),
          //     pos.ToString(),
          //     clickedStrand.PositionString()
          //   )
          // );
          // spiderScript.SetPositionToMove(pos);
          node1 = Instantiate(node, pos, Quaternion.identity) as GameObject;
          node1.name = string.Format("Node{0}", (nodeList.Count - 4));

          // node1.SetActive(false);
          node1.GetComponent<Renderer>().enabled = false;
          Node newNode = new Node(pos.x, pos.y);
          nodeList.Add(newNode);
          existingNode = false;
          // Debug.Log("Created new node at: " + newNode.PositionString());
        }
      }
    }

    if (Input.GetMouseButtonUp(0)) {
      ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (plane.Raycast(ray, out ray_distance)) {
        mPos = ray.GetPoint(ray_distance);
        mPos = new Vector3((float) System.Math.Round(mPos.x, 3), (float)System.Math.Round(mPos.y, 3), 0);

        // Debug.Log(string.Format("node1 at: ({0},{1})", node1.transform.position.x, node1.transform.position.y));
        if (node1) {
          positionToMove = node1.transform.position;
          Renderer nodeRenderer = node1.GetComponent<Renderer>();
          Bounds nodeBounds = nodeRenderer.bounds;

          if (!nodeBounds.Contains(mPos)) {
            float startX, startY, endX, endY;

            Strand clickedStrand = null;
            bool clickedOnExistingStrand = strandListContainsPoint(mPos, out clickedStrand);
            if(clickedOnExistingStrand) {
              bool clickedOnExistingNode = GetOverlappingObjects(out node2, "Vertex", mPos, 0.1f);
              if(!clickedOnExistingNode) {
                endStrand = clickedStrand;
                //Debug.Log(string.Format("mPos = {0}", mPos.ToString()));
                Vector3 pos = clickedStrand.ClosestPoint(mPos);
                //Debug.Log(string.Format("Closest point to mPos on strand: {0}", pos.ToString()));
                node2 = Instantiate(node, pos, Quaternion.identity) as GameObject;
                node2.name = string.Format("Node{0}", (nodeList.Count - 4));
                // node2.SetActive(false);
                node2.GetComponent<Renderer>().enabled = false;
                Node newNode = new Node(pos.x, pos.y);
                nodeList.Add(newNode);
                // Debug.Log("Created new node at: " + newNode.PositionString());
              }

              startX = node1.transform.position.x;
              startY = node1.transform.position.y;
              endX = node2.transform.position.x;
              endY = node2.transform.position.y;

              float lineDistance = Mathf.Sqrt(Mathf.Pow(endX - startX, 2) + Mathf.Pow(endY - startY, 2));
              float lineAngle = Mathf.Atan2(endY - startY, endX - startX) * (180 / Mathf.PI);
              float spriteSize = strand.GetComponent<SpriteRenderer>().bounds.size.x;
              float scale = lineDistance / spriteSize;

              if(node2) {
                Node n1 = new Node(startX, startY);
                Node n2 = new Node(endX, endY);
                Strand strandToAdd = new Strand(n1, n2);

                bool strandPresent = false;
                foreach(var str in strandList) {
                  if(strandToAdd.Equals(str)) {
                    strandPresent = true;
                  }
                }

                if(!strandPresent) {
                  drawnStrand = Instantiate(
                    strand,
                    node1.transform.position,
                    Quaternion.AngleAxis(lineAngle, Vector3.forward)
                  ) as GameObject;

                  // Debug.Log(strandToAdd.PositionString());
                  strandList.Add(strandToAdd);
                  drawnStrand.name = string.Format("Strand{0}", (strandList.Count - 4));
                  drawnStrand.GetComponent<Renderer>().enabled = false;
                  // drawnStrand.SetActive(false);

                  Vector3 originalVector = drawnStrand.transform.localScale;
                  drawnStrand.transform.localScale = new Vector3(
                    scale,
                    drawnStrand.transform.localScale.y,
                    drawnStrand.transform.localScale.z
                  );

                  Vector3 intersectPt = Vector3.zero;
                  drawnStrandIsSplit = false;
                  for(int i = strandList.Count-1; i >= 0; i--) {
                    Strand str = strandList[i];
                    if(str.Equals(startStrand) || str.Equals(endStrand) || str.Equals(strandToAdd)){
                      // Debug.Log(string.Format("Skipping {0}", str.PositionString()));
                      continue;
                    };
                    if(strandToAdd.Intersects(str, out intersectPt)) {
                      splitStrands = SplitStrand(str, new Node[] { new Node(intersectPt.x, intersectPt.y) });
                      GameObject newNode = Instantiate(node, intersectPt, Quaternion.identity) as GameObject;
                      newNode.GetComponent<Renderer>().enabled = false;
                      intersectNodes.Add(newNode);
                      newNode.name = string.Format("Node{0}", (nodeList.Count - 4));
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
                    splitStrands = SplitStrand(strandToAdd, iNodes, true);
                    Destroy(drawnStrand);
                  }

                  newStrandDrawn = false;
                }
              }
            } else {
              // Debug.Log("Did not click on an existing strand for node2");
              DeleteFirstNode();
            }
          } else {
            DeleteFirstNode();
          }
        }

        // node1 = null;
        // node2 = null;
      }

      //ShowNodeList();
      //ShowStrandList();
    }

    if (Input.GetMouseButton(0)) {
      ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (node1) {
      // TODO: add intermediate animation
      }
    }

    if(player.transform.position != positionToMove) {
      // Debug.Log(string.Format("Player at {0}", player.transform.position.ToString()));
      // Debug.Log(string.Format("Destination = {0}", positionToMove.ToString()));
      player.transform.position = Vector3.Lerp(player.transform.position, positionToMove, 0.075f);
      // Debug.Log("player is moving");
      if((player.transform.position - positionToMove).magnitude < 0.15){
        playerIsMoving = false;
      } else {
        playerIsMoving = true;
      }
    } else {
      playerIsMoving = false;
    }

    if(!playerIsMoving && !newStrandDrawn){
      // Debug.Log("player has reached clicked point, time to draw strand");
      if(!node1 || !node2){
        return;
      }
      // node1.SetActive(true);
      node1.GetComponent<Renderer>().enabled = true;
      // node2.SetActive(true);
      node2.GetComponent<Renderer>().enabled = true;
      // drawnStrand.SetActive(true);
      if(!drawnStrandIsSplit){
        // Debug.Log("Strand with no intersection");
        drawnStrand.GetComponent<Renderer>().enabled = true;
      } else {
        // Debug.Log(string.Format("Strand split into {0}", splitStrands.Count));
        foreach (GameObject go in splitStrands) {
          go.GetComponent<Renderer>().enabled = true;
        }
      }
      foreach (GameObject go in intersectNodes) {
        go.GetComponent<Renderer>().enabled = true;
      }
      UpdateAdjacencyMatrix();
      newStrandDrawn = true;

      // reset nodes and strands for next click
      node1 = null;
      node2 = null;
      drawnStrand = null;
      intersectNodes.Clear();
      splitStrands.Clear();
    }
  }

  bool GetOverlappingObjects (out GameObject obj, string tagName, Vector3 position, float radius = 0.15f) {
    Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
    GameObject go;

    if(colliders.Length > 1) {
      foreach(var collider in colliders) {
        go = collider.gameObject;
        if(go == gameObject) continue;
        if(go.tag == tagName) {
          obj = go;
          return true;
        }
      }
    }

    obj = null;
    return false;
  }

  float Distance(Vector3 p1, Vector3 p2) {
    float x1, y1, x2, y2;

    x1 = p1.x;
    y1 = p1.y;
    x2 = p2.x;
    y2 = p2.y;

    return Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2));
  }

  bool strandListContainsPoint(Vector3 pos, out Strand str){
    bool pointOnStrand = false;
    str = null;
    foreach (Strand s in strandList){
      if(s.ContainsPoint(pos.x, pos.y)){
        pointOnStrand = true;
        //Debug.Log(string.Format("Clicked on {0}", s.PositionString()));
        str = s;
      }
    }
    return pointOnStrand;
  }

  void DeleteFirstNode() {
    if(!existingNode){
      Node toDestroy = new Node(node1.transform.position.x, node1.transform.position.y);

      for (int i = nodeList.Count-1; i >=0; i--) {
        if (nodeList[i].Equals(toDestroy)){
          nodeList.RemoveAt(i);
        }
      }

      // Debug.Log("Deleted node at "+toDestroy.PositionString());
      Destroy(node1);
    }
  }

  List<GameObject> SplitStrand(Strand str, Node[] n, bool hidden = false){
    List<GameObject> splits = new List<GameObject>();
    int strandIndex = strandList.IndexOf(str);
    float startX, startY, endX, endY;

    GameObject strandGO = GameObject.Find(string.Format("Strand{0}", strandIndex - 3));
    // Debug.Log(string.Format("Splitting Strand{0}", strandIndex - 3));
    Node startN = str.GetStartNode(), endN = str.GetEndNode();

    // string listOfStrandGOs = "Strands currently in game: ";
    // Debug.Log(string.Format("strandList size = {0}", strandList.Count));
    // // listOfStrandGOs = "Strands currently in game:";
    // foreach(GameObject sgo in GameObject.FindGameObjectsWithTag("Edge")){
    //   listOfStrandGOs += string.Format("{0}, ", sgo.name);
    // }
    // Debug.Log(listOfStrandGOs);

    // Debug.Log(string.Format("Removing Strand{0} from strandList", strandIndex - 3));
    strandList.RemoveAt(strandIndex);
    Destroy(strandGO);
    strandGO = null;

    for(int i = strandIndex; i < strandList.Count; i++){
      GameObject go = GameObject.Find(string.Format("Strand{0}", i - 2));
      go.name = string.Format("Strand{0}", i - 3);
    }

    // Debug.Log(string.Format("strandList size = {0}", strandList.Count));
    // listOfStrandGOs = "Strands currently in game: ";
    // foreach(GameObject sgo in GameObject.FindGameObjectsWithTag("Edge")){
    //   listOfStrandGOs += string.Format("{0}, ", sgo.name);
    // }
    // Debug.Log(listOfStrandGOs);

    startX = startN.xPos();
    startY = startN.yPos();
    endX = endN.xPos();
    endY = endN.yPos();

    GameObject ds;
    ds = DrawScaledStrand(startX, startY, n[0].xPos(), n[0].yPos());
    ds.GetComponent<Renderer>().enabled = !hidden;
    splits.Add(ds);

    for(int i = 0; i < n.Length - 1; i++){
      ds = DrawScaledStrand(n[i].xPos(), n[i].yPos(), n[i+1].xPos(), n[i+1].yPos());
      ds.GetComponent<Renderer>().enabled = !hidden;
      splits.Add(ds);
    }

    ds = DrawScaledStrand(n[n.Length-1].xPos(), n[n.Length-1].yPos(), endX, endY);
    ds.GetComponent<Renderer>().enabled = !hidden;
    splits.Add(ds);

    return splits;
  }

  void ShowNodeList() {
    Debug.Log(string.Format("{0} nodes at:", nodeList.Count));

    foreach(var n in nodeList){
      Debug.Log(n.PositionString());
    }
  }

  void ShowStrandList() {
    Debug.Log(string.Format("{0} strands :", strandList.Count));

    foreach(var s in strandList){
      Debug.Log(s.PositionString());
    }
  }

  public bool StrandExistsBetween(Node otherStart, Node otherEnd) {
    Strand strandToCompare = new Strand(otherStart, otherEnd);
    bool strandExists = false;

    foreach(Strand s in strandList) {
      if(s.Equals(strandToCompare)){
        strandExists = true;
      }
    }

    return strandExists;
  }

  public void UpdateAdjacencyMatrix(){
    int matrixSize = nodeList.Count;
    adjacencyMatrix = new int[matrixSize, matrixSize];

    for (int i = 0; i < matrixSize; i++) {
      for (int j = 0; j < matrixSize; j++) {
        adjacencyMatrix[i, j] = 0;
      }
    }

    for (int i = 0; i < matrixSize; i++) {
      for (int j = 0; j < matrixSize; j++) {
        if(StrandExistsBetween(nodeList[i], nodeList[j])){
          adjacencyMatrix[i, j] = 1;
        }
      }
    }
  }

  public void DisplayAdjacencyMatrix(){
    int matrixSize = nodeList.Count;
    for (int i = 0; i < matrixSize; i++) {
      for (int j = 0; j < matrixSize; j++) {
        // Debug.LogadjacencyMatrix[i, j] = 0;
      }
    }
  }

  void CalculatePlayerMovePath(){
    // TODO
  }

  GameObject DrawScaledStrand(float startX, float startY, float endX, float endY) {
    float spriteSize = strand.GetComponent<SpriteRenderer>().bounds.size.x;
    float lineDistance = Mathf.Sqrt(Mathf.Pow(endX - startX, 2) + Mathf.Pow(endY - startY, 2));
    float lineAngle = Mathf.Atan2(endY - startY, endX - startX) * (180 / Mathf.PI);
    float scale = lineDistance / spriteSize;

    Strand strandToAdd = new Strand(
      new Node(startX, startY),
      new Node(endX, endY)
    );
    GameObject newStrand = Instantiate(
      strand,
      new Vector3(startX, startY, 0),
      Quaternion.AngleAxis(lineAngle, Vector3.forward)
    ) as GameObject;

    strandList.Add(strandToAdd);
    newStrand.name = string.Format("Strand{0}", (strandList.Count - 4));
    Vector3 originalRotation = newStrand.transform.localScale;
    newStrand.transform.localScale = new Vector3(
      scale,
      originalRotation.y,
      originalRotation.z
    );
    return newStrand;
  }
}
