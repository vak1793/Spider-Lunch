using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public GameObject node, strand;
    GameObject node1 = null, node2 = null;
    Plane plane;
    GameObject startNode, endNode;

    // Use this for initialization
    void Start () {
        plane = new Plane(Vector3.back, GameObject.FindGameObjectWithTag("GameController").transform.position);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mPos;
        Ray ray;
        float ray_distance;

        if (Input.GetMouseButtonDown(0)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out ray_distance)) {
                mPos = ray.GetPoint(ray_distance);
                node1 = Instantiate(node, mPos, Quaternion.identity) as GameObject;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out ray_distance)) {
                mPos = ray.GetPoint(ray_distance);
                if (node1) {
                    Renderer nodeRenderer = node1.GetComponent<Renderer>();
                    Bounds nodeBounds = nodeRenderer.bounds;

                    if (!nodeBounds.Contains(mPos)) {
                        float startX, startY, endX, endY;

                        startX = node1.transform.position.x;
                        startY = node1.transform.position.y;
                        endX = mPos.x;
                        endY = mPos.y;

                        float lineDistance = Mathf.Sqrt(Mathf.Pow(endX - startX, 2) + Mathf.Pow(endY - startY, 2));
                        Debug.Log("linedistance = " + lineDistance);
                        float lineAngle = Mathf.Atan2(endY - startY, endX - startX) * (180 / Mathf.PI);

                        node2 = Instantiate(node, mPos, Quaternion.identity) as GameObject;

                        //GameObject strandToDraw = new GameObject();
                        //SpriteRenderer strandRenderer = strandToDraw.AddComponent<SpriteRenderer>();
                        //strandRenderer.sprite = strand.GetComponent<SpriteRenderer>().sprite;
                        //  / strand.transform.localScale.y
                        float spriteSize = strand.GetComponent<SpriteRenderer>().bounds.size.x;
                        Debug.Log("Spritesize = " + spriteSize);
                        float scale = lineDistance / spriteSize;

                        GameObject drawnStrand = Instantiate(
                            strand,
                            node1.transform.position,
                            Quaternion.AngleAxis(lineAngle, Vector3.forward)
                        ) as GameObject;

                        Vector3 originalVector = drawnStrand.transform.localScale;
                        drawnStrand.transform.localScale = new Vector3(scale, drawnStrand.transform.localScale.y, drawnStrand.transform.localScale.z);

                        //drawnStrand.transform.localScale
                        //Renderer strandRenderer = drawnStrand.GetComponent<Renderer>();
                        //Bounds strandBounds = strandRenderer.bounds;


                    }
                }
            }
        }

        if (Input.GetMouseButton(0)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (node1) {
                //float startX, startY, endX, endY;

                //startX = node1.transform.position.x;
                //startY = node1.transform.position.y;
                //endX = startX;
                //endY = startY;

                //if (plane.Raycast(ray, out ray_distance)) {
                //    mPos = ray.GetPoint(ray_distance);
                //    endX = mPos.x;
                //    endY = mPos.y;
                //}

                //float line_distance = Mathf.Sqrt(Mathf.Pow(endX - startX, 2) + Mathf.Pow(endY - startY, 2));

                //GameObject a = Instantiate(strand, node1.transform.position, Quaternion.identity) as GameObject;
                //Debug.DrawLine(
                //    new Vector3(startX, startY, 0),
                //    new Vector3(endX, endY, 0),
                //    Color.white,
                //    2
                //);
            }
        }
    }
}
