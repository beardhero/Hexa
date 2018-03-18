﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WorldRenderer))]
public class WorldManager : MonoBehaviour
{
  // === Public ===
  public Transform textMeshPrefab;
  [HideInInspector] public World activeWorld;
  public TileSet regularTileSet;
  public float maxMag = 10;
  public float worldScale = 1;
  public int worldSubdivisions = 1;
  public static SimplexNoise simplex;
  public static int uvWidth = 100;
  public static int uvHeight;
  public bool b;
  public bool b1,b2,b3,b4,b5,b6,s1,s2,s3,s4,s5,s6;
  public bool customize;
 
  // === Private ===
  bool labelDirections;
  //@TODO: These are for creating the heights, and are properties which should be serialized when we go to persistent galaxy.
  private int octaves, multiplier;
  private float amplitude, lacunarity, dAmplitude;

  // === Properties ===
  private float _averageScale;
  public float averageScale
  {
    get {
      _averageScale = 0;
      foreach (HexTile tt in activeWorld.tiles)
      {
        _averageScale += tt.height;
      }
      _averageScale /= activeWorld.tiles.Count;
      return _averageScale; }
    set { _averageScale = value; }
  }
  		
  // === Cache ===
  WorldRenderer worldRenderer;
  GameObject currentWorldObject;
  Transform currentWorldTrans;
  //int layermask; @TODO: stuff

  //for type changer
  public Ray ray;
  public RaycastHit hit;
  public TileType switchToType;
  public float heightToSet;
  public int frameDelay = 60;
  public int fB;
  public TileType sT;
  [HideInInspector]public int r;
  public bool[] hla;
  //float uvTileWidth = regularTileSet.tileWidth / texWidth;
  //float uvTileHeight = regularTileSet.tileWidth / texHeight;

  void Update()
  {
		if(Input.GetKeyDown(KeyCode.Return))
		{
			
			bool[] hlr = new bool[84];
			float modu = Random.Range (0.2f, 0.8f);
			for (int i = 0; i < 84; i++) {
				float rand = Random.Range (0f, 1.0f);
				if (rand > modu) {
					hlr [i] = true;
				}
			}
			foreach (HexTile ht in activeWorld.tiles) {
				if (customize) {
					ht.SetRule (hla);
				} else {
					ht.SetRule (hlr);
				}
			}
			/*
			activeWorld.tiles [r].ChangeType (TileType.Sol);
			activeWorld.tiles[activeWorld.tiles[r].neighbors[0]].ChangeType(TileType.Sol);
			activeWorld.tiles[activeWorld.tiles[r].neighbors[1]].ChangeType(TileType.Sol);
			activeWorld.tiles[activeWorld.tiles[r].neighbors[2]].ChangeType(TileType.Air);
			activeWorld.tiles[activeWorld.tiles[r].neighbors[3]].ChangeType(TileType.Earth);
			activeWorld.tiles[activeWorld.tiles[r].neighbors[4]].ChangeType(TileType.Water);
			activeWorld.tiles[activeWorld.tiles[r].neighbors[5]].ChangeType(TileType.Fire);
			*/
			TileType[] tta = new TileType[6] {
				TileType.Luna,
				TileType.Sol,
				TileType.Water,
				TileType.Air,
				TileType.Earth,
				TileType.Fire,
			};
			foreach (HexTile ht in activeWorld.tiles) {
				int r = Random.Range (0, 6);
				ht.ChangeType (tta [r]);
			}
			b = true;
			fB = 0;
		}	
		fB++;
		if (fB > frameDelay && b) {
		HLShift ();
		fB = 0;
  }
	
	
    //Type Changer
    if (Input.GetKeyDown(KeyCode.Mouse1))
    {
			r++;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit, 100.0f))
      {
        StartCoroutine(TypeChange(hit));
      }
    }
  }

  void OnDrawGizmos()
  {
    //Debug.Log("going here");
    //DrawAxes();
  }

  public World Initialize(bool loadWorld = false)
  {
    simplex = new SimplexNoise(GameManager.gameSeed);
    octaves = Random.Range(4, 4);
    multiplier = Random.Range(10, 10);
    amplitude = Random.Range(0.6f, 1f);
    lacunarity = Random.Range(0.7f, .9f);
    dAmplitude = Random.Range(0.5f, .1f);


    if (loadWorld)
    {
      activeWorld = LoadWorld();
    }
    else
    {
      activeWorld = new World();
      activeWorld.PrepForCache(worldScale, worldSubdivisions);
    }
    
    //Seed the world heights
    //SetHeights();
    
    currentWorldObject = new GameObject("World");
    currentWorldTrans = currentWorldObject.transform;

    //currentWorld = new World(WorldSize.Small, WorldType.Verdant, Season.Spring, AxisTilt.Slight);

    worldRenderer = GetComponent<WorldRenderer>();
    //changed this to run TriPlates instead of HexPlates
    foreach (GameObject g in worldRenderer.HexPlates(activeWorld, regularTileSet))
    {
      g.transform.parent = currentWorldTrans;
    }

    //layermask = 1 << 8;   // Layer 8 is set up as "Chunk" in the Tags & Layers manager

    //labelDirections = true;

    //DrawHexIndices();

    return activeWorld;
  }

  World LoadWorld()
  {
    return BinaryHandler.ReadData<World>(World.cachePath);
  }

  void SetHeights() //@TODO we should be reading heights from hextile (based on the worldseed?)
  {
    //Alright, let's expand on the simplex with some height adjustments from the plate tectonics
    //Each plate has has two axes it's moving on with a small velocity, each tile shares this movement
    foreach (TriTile tt in activeWorld.triTiles)
    {
	    //Debug.Log (ht.height);
      tt.height = 1f + tt.height;
    }

    /*
    float s = Random.Range(-99999,99999);
    foreach (HexTile ht in activeWorld.tiles)
    {
      ht.hexagon.Scale(1f + (int)(100 * (0.7f * Mathf.Abs(simplex.coherentNoise(ht.hexagon.center.x, ht.hexagon.center.y, ht.hexagon.center.z, octaves, multiplier, amplitude, lacunarity, dAmplitude) //))) / 100f);
                           + 0.3f * Mathf.Abs(simplex.coherentNoise(s*ht.hexagon.center.x, s*ht.hexagon.center.y, s*ht.hexagon.center.z, octaves, multiplier, amplitude, lacunarity, dAmplitude)))))/100f);
      //Debug.Log(1f + (int)(100 * (0.7f * Mathf.Abs(simplex.coherentNoise(ht.hexagon.center.x, ht.hexagon.center.y, ht.hexagon.center.z, octaves, multiplier, amplitude, lacunarity, dAmplitude)
      //                      + 0.3f * Mathf.Abs(simplex.coherentNoise(s * ht.hexagon.center.x, s * ht.hexagon.center.y, s * ht.hexagon.center.z, octaves, multiplier, amplitude, lacunarity, dAmplitude)))))/100f);
      //Debug.Log(ht.hexagon.scale);
    }
    */
  }
  //@TODO: This is preliminary, it sets the ocean tiles using average scale 
  //by making any tile close to the average or below blue, then scaling the blue tiles up to the average.
	/*
  void CreateOcean()
  {
    foreach (TriTile ht in activeWorld.triTiles)
    {
      ht.type = TileType.Blue;
    }
    TileType typeToSet = TileType.Tan;
    foreach (TriTile ht in activeWorld.triTiles)
    {
      float rand = Random.Range(0, 1f);
      //@TODO: this is just a preliminary variation of the land types
      if (rand <= 0.4f)
        typeToSet = TileType.Brown;
      if (rand > 0.4f)
        typeToSet = TileType.Red;
      if (ht.height >= averageScale*0.99f)
      {
        ht.type = typeToSet;
      }
    }
    foreach (TriTile ht in activeWorld.triTiles)
    {
      if (ht.type == TileType.Blue)
      {
        ht.height *= (averageScale*0.99f / ht.height);
      }
    }
  }
  */
  //So now with the land masses, we're going to make the "biomes" more coherent like we did in Zone -> SpreadGround and RefineGround
  void RefineTypes()
  {
    foreach (TriTile ht in activeWorld.triTiles)
    {
      //int i = 0;
      //foreach (HexTile h in ht.ne) ;
    }
  }
  void DrawAxes()
  {
    if (!labelDirections || activeWorld.tiles.Count == 0)
      return;

    //int currentTileX = 13, currentTileY = 0, currentTileXY = 0;  

    // === Draw axes on all tiles ===
    for (int i=0; i<activeWorld.tiles.Count; i++)
    {
      DrawHexAxes(activeWorld.tiles, activeWorld.origin, i);
    }

    /*
    // === Draw Bands Only ===
    // Y-band
    for (int y=0; y<activeWorld.circumferenceInTiles; y++)
    {
      if (currentTileY != -1)
      {
        DrawHexAxes(activeWorld.tiles, activeWorld.origin, currentTileY);
        currentTileY = activeWorld.tiles[currentTileY].GetNeighborID(Direction.Y);
      }
    }
    // XY-band
    for (int xy=0; xy<activeWorld.circumferenceInTiles; xy++)
    {
      if (currentTileXY != -1)
      {
        DrawHexAxes(activeWorld.tiles, activeWorld.origin, currentTileXY);
        currentTileXY = activeWorld.tiles[currentTileXY].GetNeighborID(Direction.XY);
      }
    }
    // X-band
    for (int x=0; x<activeWorld.circumferenceInTiles; x++)
    {
      if (currentTileX != -1)
      {
        DrawHexAxes(activeWorld.tiles, activeWorld.origin, currentTileX);
        currentTileX = activeWorld.tiles[currentTileX].GetNeighborID(Direction.X);
      }
    }
    */
  }

  void DrawHexAxes(List<HexTile> tiles, Vector3 worldOrigin, int index, float scale = .1f, bool suppressWarnings = true)
  {
    if (index == -1)
    {
      Debug.LogError("Invalid index: -1");
      return;
    }

    SerializableVector3 origin = new SerializableVector3();
    try
    {
      origin = (tiles[index].hexagon.center + (SerializableVector3)worldOrigin) * 1.05f;
    }
    catch(System.Exception e)
    {
      Debug.LogError("Error accessing tile "+ index+": "+e);
      return;
    }

    for (int dir = 0; dir<Direction.Count && dir<tiles[index].hexagon.neighbors.Length; dir++)
    {
      int y = tiles[index].GetNeighborID(dir);
      if (y != -1)
      {
        Gizmos.color = Direction.ToColor(dir);
        SerializableVector3 direction = tiles[tiles[index].GetNeighborID(dir)].hexagon.center - tiles[index].hexagon.center;

        float finalScale = scale;
        if (dir == Direction.X || dir == Direction.Y || dir == Direction.NegXY)   // Prime directions
          finalScale *= 2;

        Gizmos.DrawRay((Vector3)origin, (Vector3)direction*finalScale);
      }
    }
  }

  void DrawHexIndices()
  {
    foreach (HexTile ht in activeWorld.tiles)
    {
      Transform t = (Transform)Instantiate(textMeshPrefab, (ht.hexagon.center-activeWorld.origin)*1.01f, Quaternion.LookRotation(activeWorld.origin-ht.hexagon.center));
      TextMesh x = t.GetComponent<TextMesh>();
      x.text = ht.index.ToString();
      t.parent = currentWorldTrans;
    }
  }

  public IEnumerator TypeChange(RaycastHit hit)
  {
    HexTile hitTile = new HexTile();
	GameObject plateO = hit.transform.gameObject;
	Vector3 c = new Vector3 ();
	Vector3 h = hit.point;
	float test;
	float dist = 9999999;
	foreach (HexTile ht in activeWorld.tiles) 
	{
			c = ht.hexagon.center;
			test = (c - h).sqrMagnitude;
			if (test < dist) {
				test = dist;
				hitTile = ht;
			}
	}
	hitTile.ChangeType(switchToType);
    yield return null;
  }
  
  public void HLShift ()
	{
		foreach (HexTile ht in activeWorld.tiles) {
			List<HexTile> htl = new List<HexTile>();
			foreach (int n in ht.neighbors) {
				htl.Add(activeWorld.tiles[n]);
			}
			HexTile[] hta = new HexTile[htl.Count];
			for (int i = 0; i < htl.Count; i++) {
				hta [i] = htl [i];
			}
			ht.HexLifeShift (hta);
		}
	}
  public void HL()
	{
		bool[] ba = new bool[activeWorld.tiles.Count];
		//List<HexTile> tiles = new List<HexTile>(activeWorld.tiles);
		for (int i = 0; i < activeWorld.tiles.Count; i++) {
			int sol = 0;
			int luna = 0;
			HexTile ht = activeWorld.tiles [i];
			foreach (int n in ht.neighbors) {
				if (activeWorld.tiles [n].type == TileType.Sol) {
					sol++;
				}
					/*
					if (activeWorld.tiles [i].type == TileType.Luna)
					{
						luna++;
					}
					*/
				}
			//survive
			if (s1 && sol ==  1) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s2 && sol == 2) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s3 && sol == 3) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			} 
			if (s4 && sol == 4) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s5 && sol == 5) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s6 && sol == 6) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			//born
			if (b1 && sol == 1) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b2 && sol == 2) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b3 && sol == 3) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b4 && sol == 4) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b5 && sol == 5) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b6 && sol == 6) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
		}
		//now change the types
		for (int i = 0; i < activeWorld.tiles.Count; i++)
		{
			HexTile ht = activeWorld.tiles [i];
			if (ba [i]) {
				ht.ChangeType (TileType.Sol);
			} 
			else 
			{
				ht.ChangeType (TileType.Luna);
			}
		}
	}
}
