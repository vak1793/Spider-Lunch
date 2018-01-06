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
        edges += string.Format("{0}, ", i);
        i++;
        // edges += string.Format("{0}: {1},", edge.Key.PositionString(), edge.Value);
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
    List<Node> path = new List<Node>();

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

    while(nodes.Count != 0){
      nodes.Sort((x, y) => (int)(distances[x] - distances[y]));

      var smallest = nodes[0];
      nodes.RemoveAt(0);
      if(smallest.Equals(finish)){
        // Debug.Log("Initialized path");
        path = new List<Node>();
        while (previous.ContainsKey(smallest)){
          path.Add(smallest);
          smallest = previous[smallest];
        }

        continue;
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
          distances[neighbor.Key] = alt;
          previous[neighbor.Key] = smallest;
        }
      }
    }
    // Debug.Log(string.Format("path.count = {0}", path.Count));
    return path;
  }
}
