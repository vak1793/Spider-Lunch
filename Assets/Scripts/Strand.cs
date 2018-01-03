using UnityEngine;

public class Strand {
  Node startNode, endNode;

  public Strand(Node _start, Node _end) {
    startNode = _start;
    endNode = _end;
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
    if(distFromLine > 1) {
      // Debug.Log(string.Format("({0},{1}) is {2} units away", x, y, distFromLine));
      return false;
    }
    return true;
  }
}
