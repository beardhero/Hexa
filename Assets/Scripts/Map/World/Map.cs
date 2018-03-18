/*
 * Copyright (c) 2015 Colin James Currie.
 * All rights reserved.
 * Contact: cj@cjcurrie.net
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Voronoi2;

public class Map
{
  public class Site
  {
    public int id;
    public string name;
    public IntCoord coordLocation;
    public List<int> neighbors;
    public List<Line> edges;

    public Site(int i, string n, IntCoord c, List<int> ne, List<Line> e)
    {
      id = i;
      name = n;
      coordLocation = c;
      neighbors = ne;
      edges = e;
    }
  }


  // === Map vars ===

  public string name;
  public int regions=100, width=1000, height=1000, distanceTwixtSites=1;

  public Dictionary<int, Site> sites;

  public Color[] colors;             // Raw Pixel data for rendering
  public Line[] edgeMap;             // Initialized in RelaxVoronoi 
  public Line[] adjacencyMap;   // Initialized in RelaxVoronoi

  // === Map methods ===

  public Map(int w_in, int h_in, int regions_in, int dist, int relaxSteps)
  {
    width = w_in;
    height = h_in;
    regions = regions_in;
    distanceTwixtSites = dist;
    colors = new Color[width*height];
    ClearColors(Color.black);

    double[] xVal = new double[regions], yVal = new double[regions];  // used by voronoi below

    // === Generating ===

    // Generate random points
    for (int j=0;j<regions;j++)
    {
      xVal[j] = Random.Range(0,width-1);
      yVal[j] = Random.Range(0,height-1);
    }

    // Generate voronoi cells graph
    Voronoi voroObject = new Voronoi ( distanceTwixtSites );
    List<GraphEdge> regionBorders = voroObject.generateVoronoi ( xVal, yVal, 0, width-1, 0, height-1 );

    for (int i=0; i<relaxSteps; i++)
    {
      // Relax voronoi cell centers
      sites = RelaxVoronoi(regionBorders);
      foreach (Site s in sites.Values)
      {
        xVal[s.id] = s.coordLocation.x;
        yVal[s.id] = s.coordLocation.y;
      }

      // Regenerate cells
      regionBorders = voroObject.generateVoronoi ( xVal, yVal, 0, width-1, 0, height-1 );
    }
    // Final smoothing
    regionBorders = voroObject.generateVoronoi ( xVal, yVal, 0, width-1, 0, height-1 );

    sites = RelaxVoronoi(regionBorders);

    GenerateEdgeMap();
    // GenerateAdjacencyMap();
  }


  void GenerateEdgeMap()
  {
    List<Line> edges = new List<Line>();

    foreach (Site s in sites.Values)
    {
      foreach (Line l in s.edges)
      {
        if (edges.Contains(l))
          continue;
        else
        {
          edges.Add(l);
        }
      }
    }

    edgeMap = edges.ToArray();
  }




  Dictionary<int, Site> RelaxVoronoi(List<Voronoi2.GraphEdge> voronoiEdges)
  {
    Dictionary<int, Site> output = new Dictionary<int, Site>();
    Dictionary<int, List<Vector2>> corners = new Dictionary<int, List<Vector2>>();
    Dictionary<int, List<int>> neighbors = new Dictionary<int, List<int>>();
    Dictionary<int, List<Line>> edges = new Dictionary<int, List<Line>>();

    // Parse GraphEdge voronoi data
    foreach (GraphEdge e in voronoiEdges)
    {
      Vector2 v1 = new Vector2((float)e.x1,(float)e.y1),
              v2 = new Vector2((float)e.x2,(float)e.y2);

      if (!corners.ContainsKey(e.site1))
        corners.Add(e.site1,new List<Vector2>());
      if (!corners.ContainsKey(e.site2))
        corners.Add(e.site2,new List<Vector2>());
      if (!neighbors.ContainsKey(e.site1))
        neighbors.Add(e.site1, new List<int>());
      if (!neighbors.ContainsKey(e.site2))
        neighbors.Add(e.site2, new List<int>());
      if (!edges.ContainsKey(e.site1))
        edges.Add(e.site1, new List<Line>());
      if (!edges.ContainsKey(e.site2))
        edges.Add(e.site2, new List<Line>());

      corners[e.site1].Add(v1);
      corners[e.site1].Add(v2);
      corners[e.site2].Add(v1);
      corners[e.site2].Add(v2);
      neighbors[e.site1].Add(e.site2);    // Introduce one neighbor to the other
      neighbors[e.site2].Add(e.site1);
      Line edge = new Line((int)v1.x,(int)v1.y,(int)v2.x,(int)v2.y);
      edges[e.site1].Add(edge);
      edges[e.site2].Add(edge);
    }

    // Collect distinct sites
    foreach(int i in corners.Keys)
    {
      float x=0, y=0;

      foreach (Vector2 v in corners[i])
      {
        x += v.x;
        y += v.y;
      }

      int c = corners[i].Count;
      x /= c;
      y /= c;

      if (i > regions-1)
        break;



      Site s = new Site(i, "unnamed region", new IntCoord((int)Mathf.Round(x), (int)Mathf.Round(y)), neighbors[i], edges[i]);

      output.Add(i, s);
    }

    return output;
  }

  public void ClearColors(Color c)
  {
    for(int i=0;i<width*height;i++)
    {
      colors[i] = c;
    }
  }

  public int GetIndex(int x, int y)
  {
    return x + width*y;
  }

}
