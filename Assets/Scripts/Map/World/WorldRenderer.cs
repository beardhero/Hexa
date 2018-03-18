﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldRenderer : MonoBehaviour
{
  public GameObject worldPrefab;
  public float tileWidth;
  public float tileHeight;
  public int tileCountW;
  public int tileCountH;
  public bool hexagonal; // false for triangle uvs
  public List<List<SphereTile>> hPlates;
  bool controlx;
  bool controly;
  bool controlz;

  public static Vector2 uv0,uv1,uv2,uv3,uv4,uv5,uv6;
  public static float uvTileWidth;
  public static float uvTileHeight;

  PolySphere activePolySphere;
  /*
  //Going to make a new render function to render plates instead
  public List<GameObject> TriPlates(World world, TileSet tileSet)
  {
    List<GameObject> output = new List<GameObject>();
    //Populate polysphere.hPlates based on hextile plate index
    //First find number of plates
    /*
    hPlates = new List<List<HexTile>>();
    for (int i = 0; i <= world.numberOfPlates; i++)
    {
      hPlates.Add(new List<HexTile>());
    }
    */
		/*

    //Create a mesh for each plate and put it in the list of outputs
    for (int i = 0; i < PolySphere.maxPlates; i++)
    {
      output.Add(TriPlate(world, tileSet, i));
    }
    return output;
  }

  public GameObject TriPlate(World world, TileSet tileSet, int i)
  {
    GameObject output = (GameObject)Instantiate(worldPrefab, Vector3.zero, Quaternion.identity);

    MeshFilter myFilter = output.GetComponent<MeshFilter>();
    MeshCollider myCollider = output.GetComponent<MeshCollider>();

    SerializableVector3 origin = world.origin;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    //Debug.Log("triangle uvs"); 
    float uv2x = 1.0f / tileCountW;
    float uv1x = uv2x / 2;
    float uv1y = 1.0f / tileCountH;
    Vector2 uv0 = Vector2.zero,
            uv2 = new Vector2(uv2x, 0),
            uv1 = new Vector2(uv1x, uv1y);
    //Generate plate
    foreach (TriTile tt in world.triTiles)
    {
			if (!tt.rendered) {
				if (tt.plate == i) {
					tt.rendered = true;
					IntCoord uvCoord = tileSet.GetUVForType (tt.type);
					//Debug.Log("xCoord: "+ uvCoord.x + "  type: "+ ht.type);
					Vector2 uvOffset = new Vector2 ((uvCoord.x * uv2.x), (uvCoord.y * uv1.y));
					Vector3 org = new Vector3 (origin.x, origin.y, origin.z);
					//Origin point
					vertices.Add (origin);
					uvs.Add (uv1 + uvOffset);
					normals.Add (tt.center - origin);
					int originIndex = vertices.Count - 1;

					// Triangle
					Vector3 v1 = tt.v1;
					Vector3 v2 = tt.v2;
					Vector3 v3 = tt.v3;

					vertices.Add (v1);
					normals.Add ((org + v1));
					uvs.Add (uv0 + uvOffset);

					vertices.Add (v2);
				normals.Add ((org + v2));
					uvs.Add (uv1 + uvOffset);

					vertices.Add (v3);
					normals.Add ((org + v3));
					uvs.Add (uv2 + uvOffset);

					//face triangle
					triangles.Add (vertices.Count - 1); //3
					triangles.Add (vertices.Count - 3); //1
					triangles.Add (vertices.Count - 2); //2

					//three more triangles to connect the origin
					triangles.Add (vertices.Count - 3);
					//triangles.Add(vertices.Count - 4); 
					triangles.Add (originIndex);
					triangles.Add (vertices.Count - 2);

					triangles.Add (vertices.Count - 2);
					//triangles.Add(vertices.Count - 4);
					triangles.Add (originIndex);
					triangles.Add (vertices.Count - 1);

					triangles.Add (vertices.Count - 1);
					//triangles.Add(vertices.Count - 4);
					triangles.Add (originIndex);
					triangles.Add (vertices.Count - 3);
				}
			}
    }
    Mesh m = new Mesh();
    m.vertices = vertices.ToArray();
    m.triangles = triangles.ToArray();
    m.normals = normals.ToArray();
    m.uv = uvs.ToArray();

    myCollider.sharedMesh = m;
    myFilter.sharedMesh = m;

    return output;
  }
	*/
  public List<GameObject> HexPlates(World world, TileSet tileSet)
  {
    List<GameObject> output = new List<GameObject>();
    //Populate polysphere.hPlates based on hextile plate index
    //First find number of plates
    /*
    hPlates = new List<List<HexTile>>();
    for (int i = 0; i <= world.numberOfPlates; i++)
    {
      hPlates.Add(new List<HexTile>());
    }
    */
    

    //Create a mesh for each plate and put it in the list of outputs
    for (int i = 0; i < PolySphere.maxPlates; i++)
    {
      output.Add(HexPlate(world, tileSet, i));
    }
    return output;
  }

  public GameObject HexPlate(World world, TileSet tileSet, int i)
  {
    GameObject output = (GameObject)Instantiate(worldPrefab, Vector3.zero, Quaternion.identity);

    MeshFilter myFilter = output.GetComponent<MeshFilter>();
    MeshCollider myCollider = output.GetComponent<MeshCollider>();

    SerializableVector3 origin = world.origin;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    //Switch between UV Modes
    if (hexagonal) //Hexagonal uvs
    {
      //Copypasta from worldrenderer
      float texHeight = tileSet.texture.height;
      float texWidth = tileSet.texture.width;
      float root3 = Mathf.Sqrt(3);
			uvTileWidth = 1.0f / 7.0f; //tileSet.tileWidth / texWidth;
			uvTileHeight = 1.0f / 7.0f; //tileSet.tileHeight / texHeight;
      //float side = uvTileWidth / 2.0f;
     // float radius = Mathf.Sqrt((3.0f * side * side) / 4.0f);
	  /*
      Vector2 uv0 = new Vector2(side, side),
              uv1 = new Vector2(side, side + side),
              uv2 = new Vector2(side + radius, side + side / 2.0f),
              uv3 = new Vector2(side + radius, side / 2.0f),
              uv4 = new Vector2(side, 0),
              uv5 = new Vector2(side - radius, side / 2.0f),
              uv6 = new Vector2(side - radius, side + side / 2.0f);
		*/
			uv0 = new Vector2 (uvTileWidth/2.0f, uvTileHeight / 2.0f);
			uv1 = new Vector2 (6/texWidth, 74/texHeight);
			uv2 = new Vector2 (40/texWidth, 16/texHeight);
			uv3 = new Vector2 (106/texWidth, 16/texHeight);
			uv4 = new Vector2 (140/texWidth, 74/texHeight);
			uv5 = new Vector2 (106/texWidth, 132/texHeight);
			uv6 = new Vector2 (40/texWidth, 132/texHeight);

			//Debug.Log (uv0.x + " " + uv0.y);
			//Debug.Log(uv1.x + " " + uv1.y);
			//Debug.Log(uv2.x + " " + uv2.y);
			//Debug.Log(uv3.x + " " + uv3.y);
			//Debug.Log(uv4.x + " " + uv4.y);
			//Debug.Log(uv5.x + " " + uv5.y);
			//Debug.Log (uv6.x + " " + uv6.y);
			foreach (HexTile ht in world.tiles)
			{
				if (ht.plate == i)
				{
					IntCoord uvCoord = tileSet.GetUVForType(ht.type);
					//Debug.Log("xCoord: "+ uvCoord.x + "  type: "+ ht.type);
					Vector2 uvOffset = new Vector2(uvCoord.x * uvTileWidth, uvCoord.y * uvTileHeight);

					// Origin point, every tile unfortunately repeats origin for uv purposes
					int originIndex = vertices.Count;
					vertices.Add(origin);
					uvs.Add(uv0 + uvOffset);
					normals.Add(ht.hexagon.center - origin);

					// Center of hexagon
					int centerIndex = vertices.Count;
					ht.hexagon.uv0i = uvs.Count; 
					// Triangle 1
					vertices.Add(ht.hexagon.center);
					normals.Add((origin + ht.hexagon.center));
					uvs.Add (uv0 + uvOffset);

					ht.hexagon.uv1i = uvs.Count;

					vertices.Add(ht.hexagon.v1);
					normals.Add((origin + ht.hexagon.v1));
					uvs.Add(uv1 + uvOffset);

					ht.hexagon.uv2i = uvs.Count;

					vertices.Add(ht.hexagon.v2);
					normals.Add((origin + ht.hexagon.v2));
					uvs.Add(uv2 + uvOffset);

					triangles.Add(centerIndex);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1);

					// T2
					ht.hexagon.uv3i = uvs.Count;
					vertices.Add(ht.hexagon.v3);
					normals.Add((origin + ht.hexagon.v3));
					uvs.Add(uv3 + uvOffset);

					triangles.Add(centerIndex);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1);

					// T3
					ht.hexagon.uv4i = uvs.Count;
					vertices.Add(ht.hexagon.v4);
					normals.Add((origin + ht.hexagon.v4));
					uvs.Add(uv4 + uvOffset);

					triangles.Add(centerIndex);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1);

					// T4
					ht.hexagon.uv5i = uvs.Count;
					vertices.Add(ht.hexagon.v5);
					normals.Add((origin + ht.hexagon.v5));
					uvs.Add(uv5 + uvOffset);

					triangles.Add(centerIndex);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1);

					// T5
					ht.hexagon.uv6i = uvs.Count;
					vertices.Add(ht.hexagon.v6);
					normals.Add((origin + ht.hexagon.v6));
					uvs.Add(uv6 + uvOffset);

					triangles.Add(centerIndex);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1);

					// T6

					triangles.Add(centerIndex);
					triangles.Add(vertices.Count - 1);
					triangles.Add(vertices.Count - 6);


					// Side 1
					triangles.Add(originIndex);
					triangles.Add(vertices.Count - 1);
					triangles.Add(vertices.Count - 2);

					// Side 2
					triangles.Add(originIndex);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 3);

					// Side 3
					triangles.Add(originIndex);
					triangles.Add(vertices.Count - 3);
					triangles.Add(vertices.Count - 4);

					// Side 4
					triangles.Add(originIndex);
					triangles.Add(vertices.Count - 4);
					triangles.Add(vertices.Count - 5);

					// Side 5
					triangles.Add(originIndex);
					triangles.Add(vertices.Count - 5);
					triangles.Add(vertices.Count - 6);

					// Side 6
					triangles.Add(originIndex);
					triangles.Add(vertices.Count - 6);
					triangles.Add(vertices.Count - 1);
				}
			}
			/*
      //Generate quadrant
      foreach (HexTile ht in world.tiles)
      {
        if (ht.plate == i)
        {
          IntCoord uvCoord = tileSet.GetUVForType(ht.type);
          //Debug.Log("xCoord: "+ uvCoord.x + "  type: "+ ht.type);
          Vector2 uvOffset = new Vector2(uvCoord.x * uvTileWidth, uvCoord.y * uvTileHeight);

          // Origin point
          int originIndex = vertices.Count;
          vertices.Add(origin);
          uvs.Add(uv0 + uvOffset);
          normals.Add(ht.hexagon.center - origin);

          // Center of hexagon
          int centerIndex = vertices.Count;

          // Triangle 1
          vertices.Add(ht.hexagon.center);
          normals.Add((origin + ht.hexagon.center));
          uvs.Add(uv1 + uvOffset);

          vertices.Add(ht.hexagon.v1);
          normals.Add((origin + ht.hexagon.v1));
          uvs.Add(uv1 + uvOffset);

          vertices.Add(ht.hexagon.v2);
          normals.Add((origin + ht.hexagon.v2));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T2
          vertices.Add(ht.hexagon.v3);
          normals.Add((origin + ht.hexagon.v3));
          uvs.Add(uv3 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T3
          vertices.Add(ht.hexagon.v4);
          normals.Add((origin + ht.hexagon.v4));
          uvs.Add(uv4 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T4
          vertices.Add(ht.hexagon.v5);
          normals.Add((origin + ht.hexagon.v5));
          uvs.Add(uv5 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T5
          vertices.Add(ht.hexagon.v6);
          normals.Add((origin + ht.hexagon.v6));
          uvs.Add(uv6 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T6
          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 6);


          // Side 1
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 2);

          // Side 2
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 3);

          // Side 3
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 3);
          triangles.Add(vertices.Count - 4);

          // Side 4
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 4);
          triangles.Add(vertices.Count - 5);

          // Side 5
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 5);
          triangles.Add(vertices.Count - 6);

          // Side 6
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 6);
          triangles.Add(vertices.Count - 1);
        }
      }*/
    }
    else //Triangle, assumed that the texture's tiles have equilateral triangle dimensions
    {
      Debug.Log("triangle uvs"); 
      float uv2x = 1.0f / tileCountW;
      float uv1x = uv2x / 2;
      float uv1y = 1.0f / tileCountH;
      Vector2 uv0 = Vector2.zero,
              uv2 = new Vector2(uv2x, 0),
              uv1 = new Vector2(uv1x, uv1y);
      //Generate quadrant
      foreach (HexTile ht in world.tiles)
      {
        if (ht.plate == i)
        {
          IntCoord uvCoord = tileSet.GetUVForType(ht.type);
          //Debug.Log("xCoord: "+ uvCoord.x + "  type: "+ ht.type);
          Vector2 uvOffset = new Vector2((uvCoord.x * uv2.x), (uvCoord.y * uv1.y));

          // Origin point
          int originIndex = vertices.Count;
          vertices.Add(origin);
          uvs.Add(uv1 + uvOffset);
          normals.Add(ht.hexagon.center - origin);

          // Center of hexagon
          int centerIndex = vertices.Count;

          // Triangle 1
          vertices.Add(ht.hexagon.center);
          normals.Add((origin + ht.hexagon.center));
          uvs.Add(uv1 + uvOffset);

          vertices.Add(ht.hexagon.v1);
          normals.Add((origin + ht.hexagon.v1));
          uvs.Add(uv0 + uvOffset);

          vertices.Add(ht.hexagon.v2);
          normals.Add((origin + ht.hexagon.v2));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T2
          vertices.Add(ht.hexagon.v3);
          normals.Add((origin + ht.hexagon.v3));
          uvs.Add(uv0 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T3
          vertices.Add(ht.hexagon.v4);
          normals.Add((origin + ht.hexagon.v4));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T4
          vertices.Add(ht.hexagon.v5);
          normals.Add((origin + ht.hexagon.v5));
          uvs.Add(uv0 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T5
          vertices.Add(ht.hexagon.v6);
          normals.Add((origin + ht.hexagon.v6));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T6
          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 6);


          // Side 1
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 2);

          // Side 2
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 3);

          // Side 3
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 3);
          triangles.Add(vertices.Count - 4);

          // Side 4
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 4);
          triangles.Add(vertices.Count - 5);

          // Side 5
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 5);
          triangles.Add(vertices.Count - 6);

          // Side 6
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 6);
          triangles.Add(vertices.Count - 1);
        }
      }
    }
    //Debug.Log(uv1);
    //Debug.Log(uv2);
    //Debug.Log(uv0);
    //LabelCenters(sphere.finalTris);
    //LabelNeighbors(sphere);



    //GameObject centerMarker = (GameObject)GameObject.Instantiate(centerMarkerPrefab, tri.center, Quaternion.identity);
    Mesh m = new Mesh();
    m.vertices = vertices.ToArray();
    m.triangles = triangles.ToArray();
    m.normals = normals.ToArray();
    m.uv = uvs.ToArray();

    myCollider.sharedMesh = m;
    myFilter.sharedMesh = m;

    return output; 
  }
  //Redacted functions
  /*
  public List<GameObject> RenderWorld(World world, TileSet tileSet)
  {
    List<GameObject> output = new List<GameObject>();

    for (int i=0; i<8; i++)
    {
      //StartCoroutine(RecursiveRender(world, tileSet, controlx, controly, controlz, i));
      output.Add(Quadrant(world, tileSet, controlx, controly, controlz, i));
      //Call our control function, which will iterate through the cyclic permutations to define 8 quadrants
      Cycle(controlx, controly, controlz);
    }
    return output;
  }
  public GameObject Quadrant(World world, TileSet tileSet, bool cx, bool cy, bool cz, int it)
  {
    //Debug.Log("ITERATION "+it);
    //currentZone = GameManager.currentZone;
    GameObject output = (GameObject)Instantiate(worldPrefab, Vector3.zero, Quaternion.identity);

    MeshFilter myFilter = output.GetComponent<MeshFilter>();
    MeshCollider myCollider = output.GetComponent<MeshCollider>();

    SerializableVector3 origin = world.origin;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    //Switch between UV Modes
    if (hexagonal) //Hexagonal uvs
    {
      //Copypasta from worldrenderer
      float texHeight = tileSet.texture.height;
      float texWidth = tileSet.texture.width;
      //float root3 = Mathf.Sqrt(3);
      float uvTileWidth = tileSet.tileWidth / texWidth;
      float uvTileHeight = tileSet.tileWidth / texHeight;
      float side = uvTileWidth / 2;
      float radius = Mathf.Sqrt((3 * side * side) / 4);

      Vector2 uv0 = new Vector2(side, side),
              uv1 = new Vector2(side, side + side),
              uv2 = new Vector2(side + radius, side + side / 2),
              uv3 = new Vector2(side + radius, side / 2),
              uv4 = new Vector2(side, 0),
              uv5 = new Vector2(side - radius, side / 2),
              uv6 = new Vector2(side - radius, side + side / 2);
      //Generate quadrant
      foreach (HexTile ht in world.tiles)
      {
        if (ControlX(ht.hexagon.center.x) && ControlY(ht.hexagon.center.y) && ControlZ(ht.hexagon.center.z))
        {
          IntCoord uvCoord = tileSet.GetUVForType(ht.type);
          //Debug.Log("xCoord: "+ uvCoord.x + "  type: "+ ht.type);
          Vector2 uvOffset = new Vector2(uvCoord.x*uvTileWidth, uvCoord.y*uvTileHeight);

          // Origin point
          int originIndex = vertices.Count;
          vertices.Add(origin);
          uvs.Add(uv0 + uvOffset);
          normals.Add(ht.hexagon.center - origin);

          // Center of hexagon
          int centerIndex = vertices.Count;

          // Triangle 1
          vertices.Add(ht.hexagon.center);
          normals.Add((origin + ht.hexagon.center));
          uvs.Add(uv1 + uvOffset);

          vertices.Add(ht.hexagon.v1);
          normals.Add((origin + ht.hexagon.v1));
          uvs.Add(uv1 + uvOffset);

          vertices.Add(ht.hexagon.v2);
          normals.Add((origin + ht.hexagon.v2));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T2
          vertices.Add(ht.hexagon.v3);
          normals.Add((origin + ht.hexagon.v3));
          uvs.Add(uv3 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T3
          vertices.Add(ht.hexagon.v4);
          normals.Add((origin + ht.hexagon.v4));
          uvs.Add(uv4 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T4
          vertices.Add(ht.hexagon.v5);
          normals.Add((origin + ht.hexagon.v5));
          uvs.Add(uv5 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T5
          vertices.Add(ht.hexagon.v6);
          normals.Add((origin + ht.hexagon.v6));
          uvs.Add(uv6 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T6
          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 6);


          // Side 1
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 2);

          // Side 2
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 3);

          // Side 3
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 3);
          triangles.Add(vertices.Count - 4);

          // Side 4
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 4);
          triangles.Add(vertices.Count - 5);

          // Side 5
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 5);
          triangles.Add(vertices.Count - 6);

          // Side 6
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 6);
          triangles.Add(vertices.Count - 1);
        }
      }
    }
    else //Triangle
    {
      float uv2x = 1.0f / tileCountW;
      float uv1x = uv2x / 2;
      float uv1y = 1.0f / tileCountH;
      Vector2 uv0 = Vector2.zero,
              uv2 = new Vector2(uv2x, 0),
              uv1 = new Vector2(uv1x, uv1y);
      //Generate quadrant
      foreach (HexTile ht in world.tiles)
      { 
        if (ControlX(ht.hexagon.center.x) && ControlY(ht.hexagon.center.y) && ControlZ(ht.hexagon.center.z))
        {
          IntCoord uvCoord = tileSet.GetUVForType(ht.type);
          //Debug.Log("xCoord: "+ uvCoord.x + "  type: "+ ht.type);
          Vector2 uvOffset = new Vector2((uvCoord.x * uv2.x), (uvCoord.y * uv1.y));

          // Origin point
          int originIndex = vertices.Count;
          vertices.Add(origin);
          uvs.Add(uv1 + uvOffset);
          normals.Add(ht.hexagon.center - origin);

          // Center of hexagon
          int centerIndex = vertices.Count;

          // Triangle 1
          vertices.Add(ht.hexagon.center);
          normals.Add((origin + ht.hexagon.center));
          uvs.Add(uv1 + uvOffset);

          vertices.Add(ht.hexagon.v1);
          normals.Add((origin + ht.hexagon.v1));
          uvs.Add(uv0 + uvOffset);

          vertices.Add(ht.hexagon.v2);
          normals.Add((origin + ht.hexagon.v2));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T2
          vertices.Add(ht.hexagon.v3);
          normals.Add((origin + ht.hexagon.v3));
          uvs.Add(uv0 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T3
          vertices.Add(ht.hexagon.v4);
          normals.Add((origin + ht.hexagon.v4));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T4
          vertices.Add(ht.hexagon.v5);
          normals.Add((origin + ht.hexagon.v5));
          uvs.Add(uv0 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T5
          vertices.Add(ht.hexagon.v6);
          normals.Add((origin + ht.hexagon.v6));
          uvs.Add(uv2 + uvOffset);

          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 1);

          // T6
          triangles.Add(centerIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 6);


          // Side 1
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 1);
          triangles.Add(vertices.Count - 2);

          // Side 2
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 2);
          triangles.Add(vertices.Count - 3);

          // Side 3
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 3);
          triangles.Add(vertices.Count - 4);

          // Side 4
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 4);
          triangles.Add(vertices.Count - 5);

          // Side 5
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 5);
          triangles.Add(vertices.Count - 6);

          // Side 6
          triangles.Add(originIndex);
          triangles.Add(vertices.Count - 6);
          triangles.Add(vertices.Count - 1);
        }
      }
    }
    //Debug.Log(uv1);
    //Debug.Log(uv2);
    //Debug.Log(uv0);
    //LabelCenters(sphere.finalTris);
    //LabelNeighbors(sphere);



    //GameObject centerMarker = (GameObject)GameObject.Instantiate(centerMarkerPrefab, tri.center, Quaternion.identity);
    Mesh m = new Mesh();
    m.vertices = vertices.ToArray();
    m.triangles = triangles.ToArray();
    m.normals = normals.ToArray();
    m.uv = uvs.ToArray();

    myCollider.sharedMesh = m;
    myFilter.sharedMesh = m;

    return output;
  }

  public void Cycle(bool x, bool y, bool z)
  {
    //8 quadrants
    if (!x && !y && !z)
    {      
      controlz = true;
      return;
    }
    if (!x && !y && z)
    {
      controlz = false;
      controly = true;
      return;
    }
    if (!x && y && !z)
    {
      controlz = true;
      return;
    }
    if (!x && y && z)
    {
      controlx = true;
      controly = false;
      controlz = false;
      return;
    }
    if (x && !y && !z)
    {
      controlz = true;
      return;
    }
    if (x && !y && z)
    {
      controly = true;
      controlz = false;
      return;
    }
    if (x && y && !z)
    {
      controlz = true;
      return;
    }
    if (x && y && z)
    {
      controlx = controly = controlz = false;
    }
  }
  

  /*
  bool QuadrantActive(int iteration)
  {
    switch (iteration)
    {
      case 0:
        return !controlx && !controly && !controlz;
      case 1:
        return !controlx && !controly && controlz;
      case 2:
        return !controlx && controly && !controlz;
      case 3:
        return !controlx && controly && controlz;
      case 4:
        return controlx && !controly && !controlz;
      case 5:
        return controlx && !controly && controlz;
      case 6:
        return controlx && controly && !controlz;
      default:
        return controlx && controly && controlz;
    }
  }
  */
  //Debug
  /*
  void LabelNeighbors(PolySphere sphere)
  {
    
    Dictionary<Triangle, bool> neighborsLabeled = new Dictionary<Triangle, bool>();

    foreach (Triangle tri in sphere.finalTris)
    {
      if (neighborsLabeled.ContainsKey(tri) )
        continue;

      neighborsLabeled.Add(tri.instance, true);

      // Do the three immediate neighbors
      if (!neighborsLabeled.ContainsKey(tri.top))
      {
        //neighborsLabeled.Add(tri.top.instance, true);

        Vector3 midPointNX = (tri.center+tri.top.center) / 2;
        GameObject textObj = (GameObject)Instantiate(textMeshPrefab, midPointNX * 1.03f,
                                Quaternion.LookRotation(-midPointNX, tri.center-tri.top.center));
        textObj.GetComponent<TextMesh>().text = "|";
      }

      if (!neighborsLabeled.ContainsKey(tri.right))
      {
        //neighborsLabeled.Add(tri.right.instance, true);

        Vector3 midPointNY = (tri.center+tri.right.center) / 2;
        GameObject textObj = (GameObject)Instantiate(textMeshPrefab, midPointNY * 1.03f,
                                Quaternion.LookRotation(-midPointNY, tri.center-tri.right.center));
        textObj.GetComponent<TextMesh>().text = "|";
      }

      if (!neighborsLabeled.ContainsKey(tri.left))
      {
        //neighborsLabeled.Add(tri.left.instance, true);

        Vector3 midPointNZ = (tri.center+tri.left.center) / 2;
        GameObject textObj = (GameObject)Instantiate(textMeshPrefab, midPointNZ * 1.03f,
                                Quaternion.LookRotation(-midPointNZ, tri.center-tri.left.center));
        textObj.GetComponent<TextMesh>().text = "|";
      }
    }

    foreach (Triangle tri in sphere.finalTris)
    {
      float scale = tri.subdivisionLevel>0?1.055f : 1.36f;

      Vector3 midPointTop = (tri.center+tri.top.center) / 2,
              midPointRight = (tri.center+tri.right.center) / 2,
              midPointLeft = (tri.center+tri.left.center) / 2;

      GameObject textObj = (GameObject)Instantiate(textMeshPrefab, midPointTop * scale,
                              Quaternion.LookRotation(-midPointTop, tri.center-tri.top.center));
      textObj.GetComponent<TextMesh>().text = "|";

      Instantiate(textObj, midPointRight * scale, Quaternion.LookRotation(-midPointRight, tri.center - tri.right.center));
      Instantiate(textObj, midPointLeft * scale, Quaternion.LookRotation(-midPointLeft, tri.center-tri.left.center));
    }

  }
  */
  /*
  void LabelCenters(List<Triangle> tris)
  {
    foreach (Triangle t in tris)
    {
      GameObject textObj = (GameObject)Instantiate(textMeshPrefab, t.center * 1.01f, Quaternion.LookRotation(-1f*t.center));
      textObj.name = "Face "+t.index;
      textObj.GetComponent<TextMesh>().text = t.index.ToString();
    }
  }
  */
  
  public bool ControlX(float centerx)
  {
    if (controlx)
      return (centerx >= 0);
    else
      return (centerx < 0);
  }
  public bool ControlY(float centery)
  {
    if (controly)
      return (centery >= 0);
    else
      return (centery < 0);
  }
  public bool ControlZ(float centerz)
  {
    if (controlz)
      return (centerz >= 0);
    else
      return (centerz < 0);
  }
}

