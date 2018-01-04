using UnityEngine;

public class Strand {
  Node startNode, endNode;
  float m, c;

  public Strand(Node _start, Node _end) {
    startNode = _start;
    endNode = _end;

    float x1, y1, x2, y2;
    x1 = startNode.xPos();
    y1 = startNode.yPos();
    x2 = endNode.xPos();
    y2 = endNode.yPos();

    m = (float) System.Math.Round((y2 - y1) / (x2 - x1), 3);

    if(double.IsInfinity(m)){
      c = (float) System.Math.Round(y1, 3);
    } else {
      c = (float) System.Math.Round(y2 - (m * x2), 3);
    }
  }

  public float getLineM(){
    return m;
  }

  public float getLineC(){
    return c;
  }

  public float StrandLength() {
    float x1, y1, x2, y2;

    x1 = startNode.xPos();
    y1 = startNode.yPos();
    x2 = endNode.xPos();
    y2 = endNode.yPos();

    return Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2));
  }

  public string PositionString() {
    return "Strand from "+startNode.PositionString()+" to "+endNode.PositionString();
  }

  public bool Equals(Strand other) {
    if (this.startNode.Equals(other.startNode)) {
      if (this.endNode.Equals(other.endNode)){
        return true;
      }
    } else if (this.startNode.Equals(other.endNode)) {
      if (this.endNode.Equals(other.startNode)){
        return true;
      }
    }

    return false;
  }

  public bool ContainsPoint(float x, float y) {
    float x1, y1, x2, y2, deltaX, deltaY, distFromLine;

    x1 = startNode.xPos();
    y1 = startNode.yPos();
    x2 = endNode.xPos();
    y2 = endNode.yPos();
    deltaX = x2 - x1;
    deltaY = y2 - y1;

    bool withinXBounds = false, withinYBounds = false;

    if(x2 > x1){
      if(x > x1 && x < x2){
        withinXBounds = true;
      }
    } else if(x2 < x1){
      if(x < x1 && x > x2){
        withinXBounds = true;
      }
    } else {
      if(Mathf.Abs(x - x1) < 0.1){
        withinXBounds = true;
      }
    }

    if(y2 > y1){
      if(y > y1 && y < y2){
        withinYBounds = true;
      }
    } else if(y2 < y1){
      if(y < y1 && y > y2){
        withinYBounds = true;
      }
    } else {
      if(Mathf.Abs(y - y1) < 0.1){
        withinYBounds = true;
      }
    }

    if(!withinXBounds || !withinYBounds){
      return false;
    }

    distFromLine = Mathf.Abs((deltaY * x) - (deltaX * y) + (x2 * y1) - (x1 * y2))/StrandLength();
    if(distFromLine > 0.1) {
      // Debug.Log(string.Format("({0},{1}) is {2} units away", x, y, distFromLine));
      return false;
    }
    return true;
  }

  public Vector3 ClosestPoint(Vector3 pos){
    float[] pt = new float[2];
    float x = pos.x, y = pos.y;
    if (double.IsInfinity(m)){
      //Debug.Log("vertical line");
      pt[0] = c;
      pt[1] = y;
    } else if (m == 0) {
      //Debug.Log("horizontal line");
      pt[0] = x;
      pt[1] = c;
    } else {
      float m2 = (float) System.Math.Round(-1 / m, 3);
      float c2 = (float) System.Math.Round(y - (m2 * x), 3);

      pt[0] = (float) System.Math.Round((c - c2) / (m2 - m), 3);
      pt[1] = (float) System.Math.Round(((m2 * c) - (m * c2)) / (m2 - m), 3);
      // Debug.Log(string.Format("line eqn y = {0}x + {1}", m, c));
      // Debug.Log(string.Format("perpendicular y = {0}x + {1}", m2, c2));
      // float variance = pt[1] - (m2 * pt[0]) - c2;
      // Debug.Log(string.Format("variance = {0}",variance));
    }
    return new Vector3(pt[0], pt[1], 0);
  }

  public bool Intersects(Strand other, out Vector3 intersectPt){
    Vector3 pt = Vector3.zero;
    intersectPt = pt;

    float m1, m2, c1, c2, x = 0, y = 0;
    m1 = this.m;
    c1 = this.c;
    m2 = other.getLineM();
    c2 = other.getLineC();

    if(double.IsInfinity(m1)){
      //vertical line x = c
      x = c1;
      y = (m2 * x) + c2;
    } else if(double.IsInfinity(m2)){
      x = c2;
      y = (m1 * x) + c1;
    } else if(m1 != m2) {
      x = (float) System.Math.Round((c1 - c2) / (m2 - m1), 3);
      y = (float) System.Math.Round(((m2 * c1) - (m1 * c2)) / (m2 - m1), 3);
    }

    if(this.ContainsPoint(x, y) && other.ContainsPoint(x, y)){
      // Debug.Log(string.Format("{0} contains ({1},{2})", other.PositionString(), x, y));
      intersectPt = new Vector3(x, y, 0);
      return true;
    }
    return false;
  }
}
