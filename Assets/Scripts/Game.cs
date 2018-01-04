using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

  public GameObject node, strand, windowFrame, spider;
  GameObject node1 = null, node2 = null;
  GameObject player;
  Plane plane;
  GameObject startNode, endNode;
  bool existingNode = false;
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
    positionToMove = spiderPos;

    plane = new Plane(Vector3.back, GameObject.FindGameObjectWithTag("GameController").transform.position);
  }

  // Update is called once per frame
  void Update () {
    Vector3 mPos;
    Ray ray;
    float ray_distance;

    if(player.transform.position != positionToMove) {
      Debug.Log(string.Format("Player at {0}", player.transform.position.ToString()));
      Debug.Log(string.Format("Destination = {0}", positionToMove.ToString()));
      player.transform.position = Vector3.Lerp(player.transform.position, positionToMove, 0.3f);
    }


    if (Input.GetMouseButtonDown(0)) {
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
          positionToMove = pos;
          node1 = Instantiate(node, pos, Quaternion.identity) as GameObject;
          node1.name = string.Format("Node{0}", (nodeList.Count - 4));

          node1.SetActive(false);
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
                node1.SetActive(true);
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

              // Physics2D.Raycast();
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
                  GameObject drawnStrand = Instantiate(
                  strand,
                  node1.transform.position,
                  Quaternion.AngleAxis(lineAngle, Vector3.forward)
                  ) as GameObject;

                  // Debug.Log(strandToAdd.PositionString());
                  strandList.Add(strandToAdd);
                  drawnStrand.name = string.Format("Strand{0}", (strandList.Count - 4));

                  Vector3 originalVector = drawnStrand.transform.localScale;
                    drawnStrand.transform.localScale = new Vector3(
                    scale,
                    drawnStrand.transform.localScale.y,
                    drawnStrand.transform.localScale.z
                  );

                  Vector3 intersectPt = Vector3.zero;
                  foreach(var str in strandList) {
                    if(str.Equals(startStrand) || str.Equals(endStrand) || str.Equals(strandToAdd)){
                      // Debug.Log(string.Format("Skipping {0}", str.PositionString()));
                      continue;
                    };
                    if(strandToAdd.Intersects(str, out intersectPt)) {
                      GameObject newNode = Instantiate(node, intersectPt, Quaternion.identity) as GameObject;
                      newNode.name = string.Format("Node{0}", (nodeList.Count - 4));
                      nodeList.Add(new Node(intersectPt.x, intersectPt.y));
                    }
                  }
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

        node1 = null;
        node2 = null;
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

  void SplitStrand(Strand str, Node n){
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

  public bool StrandBetween(Node otherStart, Node otherEnd) {
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
        if(StrandBetween(nodeList[i], nodeList[j])){
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
}
