using UnityEngine;

public class Node {
  float x, y;

  public Node(float _x, float _y) {
    x = _x;
    y = _y;
  }

  public Vector3 Position() {
    return new Vector3(x, y, 0);
  }

  public float xPos() {
    return x;
  }

  public float yPos() {
    return y;
  }

  public string PositionString() {
    return "("+x+","+y+")";
  }

  public virtual bool Equals(Node other) {
    if(this.x == other.x && this.y == other.y) {
      return true;
    }
    return false;
  }

  public float DistanceFrom(Node other){
    float x1, y1, x2, y2;

    x1 = x;
    y1 = y;
    x2 = other.xPos();
    y2 = other.yPos();

    return Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2));
  }
}
