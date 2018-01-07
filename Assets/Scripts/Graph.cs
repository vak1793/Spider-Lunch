using System;
using UnityEngine;
using System.Collections.Generic;

// from https://github.com/mburst/dijkstras-algorithm/blob/master/dijkstras.cs
class Graph{
  Dictionary<Node, Dictionary<Node, float>> vertices;

  public Graph(){
    vertices = new Dictionary<Node, Dictionary<Node, float>>();
  }

  public Graph(Dictionary<Node, Dictionary<Node, float>> other){
    vertices = other;
  }

  public void addVertex(Node name, Dictionary<Node, float> edges){
    vertices[name] = edges;
    // DisplayVertices();
  }

  public void ClearGraph(){
    vertices.Clear();
  }

  public void DisplayVertices(){
    string output = "Vertices:\n";
    int i = 1;
    foreach (var vertex in vertices) {
      string edges = "[";
      i = 1;
      foreach (var edge in vertex.Value) {
        // edges += string.Format("{0}, ", i);
        edges += string.Format("{0}: {1}, ", edge.Key.PositionString(), edge.Value);
        i++;
      }
      edges += "]";
      output += string.Format("Vertex: {0}, Edges: {1}\n", vertex.Key.PositionString(), edges);
    }
    Debug.Log(output);
  }

  public List<Node> ShortestPath(Node start, Node finish){
    var previous = new Dictionary<Node, Node>();
    var distances = new Dictionary<Node, float>();
    var nodes = new List<Node>();
    List<Node> path = null;//new List<Node>();

    // Debug.Log("inside Graph.ShortestPath method");

    // DisplayVertices();
    foreach (var vertex in vertices){
      if (vertex.Key.Equals(start)){
        distances[vertex.Key] = 0;
      } else {
        distances[vertex.Key] = float.MaxValue;
      }
      nodes.Add(vertex.Key);
    }

    // List<Node> keyList = new List<Node>(nodes.Keys);
    // foreach(var key in nodes){
    //   Debug.Log(string.Format("{0}, node position = {1}", key, key.PositionString()));
    // }

    while(nodes.Count > 0){
      // nodes.Sort((x, y) => distances[x] - distances[y]);

      // nodeString += "\nAfter sort: ";
      // foreach(var n in nodes){
      //   nodeString += string.Format("{0} => {1}, ", n.PositionString(), distances[n]);
      // }
      // Debug.Log(nodeString);
      int minNodePos = -1;
      float minNodeVal = float.MaxValue;
      for(int i = 0; i < nodes.Count; i++){
        if(distances[nodes[i]] < minNodeVal){
          minNodeVal = distances[nodes[i]];
          minNodePos = i;
        }
      }

      var smallest = nodes[minNodePos];
      // Debug.Log(string.Format("Smallest = {0}", smallest.PositionString()));
      nodes.RemoveAt(minNodePos);
      if(smallest.Equals(finish)){
        // Debug.Log("Initialized path");
        path = new List<Node>();
        while (previous.ContainsKey(smallest)){
          path.Add(smallest);
          smallest = previous[smallest];
        }
        path.Add(start);
        path.Reverse();

        // string pathString = "nodes to traverse = ";
        // foreach (Node fn in path) {
        //   pathString += string.Format("{0} -> ", fn.PositionString());
        // }
        // Debug.Log(pathString);

        break;
      }
      // Debug.Log(string.Format("{0} != {1}", smallest.PositionString(), finish.PositionString()));

      if (distances[smallest] == float.MaxValue){
        // Debug.Log(string.Format("infinity"));
        continue;
      }

      // Debug.Log(string.Format("distance = {0}", distances[smallest]));
      // Debug.Log(string.Format("smallest = {0}", smallest.PositionString()));

      foreach (var neighbor in vertices[smallest]){
        // Debug.Log(string.Format("Inside neighbor loop"));
        var alt = distances[smallest] + neighbor.Value;
        // Debug.Log(string.Format("alt = {0}", alt));
        if (alt < distances[neighbor.Key]){
          // Debug.Log(string.Format("Reassigning distance of {0} to {1}", neighbor.Key.PositionString(), alt));
          distances[neighbor.Key] = alt;
          previous[neighbor.Key] = smallest;
        }
      }

      // string debugString = "";
      // foreach(var d in distances){
      //   debugString += string.Format("Node: {0}, Distance: {1}\n", d.Key.PositionString(), d.Value);
      // }
      // debugString += "\n";
      // foreach(var p in previous){
      //   debugString += string.Format("Node: {0}, Previous: {1}\n", p.Key.PositionString(), p.Value.PositionString());
      // }
      // debugString += "\n";
      // foreach(var n in nodes){
      //   debugString += string.Format("{0}, ", n.PositionString());
      // }
      // Debug.Log(debugString);
    }
    // Debug.Log(string.Format("path.count = {0}", path.Count));

    return new List<Node>(path);
  }
}
