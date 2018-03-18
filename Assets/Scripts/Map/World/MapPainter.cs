using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Voronoi2;
using Random = UnityEngine.Random;

public class MapPainter : MonoBehaviour
{
  public int relaxSteps=1, regions=100, width=1000, height=1000;
  
  public float scale=1;
  public Vector2 offset;

  int distanceTwixtSites = 1;
  SpriteRenderer myRenderer;
  Map map;
  Texture2D pixelData;

  void Awake()
  {
    myRenderer = GetComponent<SpriteRenderer>();
    BuildMap();
    //Camera.main.orthographicSize = .16f * mapWidth;
  }

  void BuildMap()
  {
    map = new Map(width, height, regions, distanceTwixtSites, relaxSteps);
    pixelData = RenderMap(map);
    Paint(pixelData);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      BuildMap();
    }
  }

  Texture2D CreateBlankMapTexture(int width, int height)
  {
    Texture2D output = new Texture2D(width,height);

    output.name = "Procedural Map";
    output.filterMode = FilterMode.Point;
    output.anisoLevel = 0;
    output.wrapMode = TextureWrapMode.Clamp;

    return output;
  }

  Texture2D RenderMap(Map m)
  {
    Texture2D output = CreateBlankMapTexture(m.width, m.height);
    


    // === Drawing ===

    // Draw circles around centers of vonoroi cells
    int circleRadius = 5;
    for (int k=0; k<m.sites.Count; k++)
    {
      int[] coords = new int[]{(int)m.sites[k].coordLocation.x, (int)m.sites[k].coordLocation.y};

      // Continue if circle will run over the edge of the map
      if (coords[0]-circleRadius<0 || coords[0]+circleRadius>m.width-1 || coords[1]-circleRadius<0 || coords[1]+circleRadius>m.height-1)
        continue;

      DrawCircle(m, coords, circleRadius, ref m.colors, Color.gray);
      DrawCircle(m, coords, circleRadius/2, ref m.colors, new Color(Random.value, Random.value, Random.value, 1));
    }

    // Draw cell edge graph
    foreach (Line l in m.edgeMap)
    {
      DrawLine(m, l, ref m.colors, Color.gray);
    }

    /* test
    for (int i=0; i<lines.Length; i++)
    {
      DrawLine(lines[i], ref m.colors, Color.white);
    }
    */

    output.SetPixels(m.colors, 0);
    output.Apply();
    return output;
  }

  void DrawCircle(Map m, int[] center, int radius, ref Color[] colors, Color circleColor)
  {
    int r2 = radius*radius;

    for(int y=-radius; y<=radius; y++)
    {
      for(int x=-radius; x<=radius; x++)
      {
        if(x*x+y*y <= r2)
        {
          colors[ m.GetIndex(center[0]+x,center[1]+y) ] = circleColor;
        }
      }
    }
  }

  void DrawLine(Map m, Line l, ref Color[] colors, Color lineColor)
  {
    float slope = (float)(l.y2-l.y1)/(float)(l.x2-l.x1);    // Rise over run

    if (Mathf.Abs(slope) < 1)
    {
      if (l.x2 < l.x1)
      {
        l = SwapPoints(l);
      }

      float y = l.y1;
      for (int x=l.x1; x<l.x2; x++)
      {
        y += slope;
        colors[ m.GetIndex(x, (int)Mathf.Round(y)) ] = lineColor;
      }
    }
    else
    {
      if (l.y2 < l.y1)
      {
        l = SwapPoints(l);
      }

      slope = (float)(l.x2-l.x1) / (float)(l.y2-l.y1);    // Run over rise

      float x = l.x1;
      for (int y=l.y1; y<l.y2; y++)
      {
        x += slope;
        colors[ m.GetIndex((int)Mathf.Round(x), y) ] = lineColor;
      }
    }
  }

  Line SwapPoints (Line l)
  {
    return new Line(l.x2, l.y2, l.x1, l.y1);
  }

  void Paint(Texture2D m)
  {
    myRenderer.sprite = Sprite.Create(m, new Rect(0,0,m.width, m.height), offset, scale);
    myRenderer.sharedMaterial.mainTexture = m;
  }
}
