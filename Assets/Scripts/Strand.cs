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

    m = (y2 - y1) / (x2 - x1);

    if(double.IsInfinity(m)){
      c = y1;
    } else {
      c = y2 - (m * x2);
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

    if(x2 > x1 && y2 > y1){
      if(x < x1 || x > x2 || y < y1 || y > y2){
        return false;
      }
    } else if(x2 > x1 && y2 < y1){
      if(x < x1 || x > x2 || y < y2 || y > y1){
        return false;
      }
    } else if(x2 < x1 && y2 > y1){
      if(x < x2 || x > x1 || y < y1 || y > y2){
        return false;
      }
    } else if(x2 < x1 && y2 < y1){
      if(x < x2 || x > x1 || y < y2 || y > y1){
        return false;
      }
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
      float m2 = -1 / m;
      float c2 = y - (m2 * x);

      pt[0] = (c2 + c) / (m2 - m);
      pt[1] = ((m2 * c) - (m * c2)) / (m2 - m);
      //Debug.Log(string.Format("line eqn y = {0}x + {1}", m, c));
      //Debug.Log(string.Format("perpendicular y = {0}x + {1}", m2, c2));
      float variance = pt[1] - (m2 * pt[0]) - c2;
      Debug.Log(string.Format("variance = {0}",variance));
    }
    return new Vector3(pt[0], pt[1], 0);
  }
}
