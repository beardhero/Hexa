using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HexDirection{R, P, L, S, B, F}; // right port left starboard back front

[RequireComponent(typeof(WorldRenderer))]
public class WorldManager : MonoBehaviour
{
  // === Public ===
  public Transform textMeshPrefab;
  [HideInInspector] public World activeWorld;
  public TileSet regularTileSet;
  public float worldScale = 1;
  public int worldSubdivisions = 1;
  public static int uvWidth = 100;
  public static int uvHeight;
  public bool b;
  public bool randomAnt;
  public TileType element = TileType.Gray;
  public byte[] seed;
  
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
  [HideInInspector]public Ray ray;
  [HideInInspector]public RaycastHit hit;
  public TileType switchToType;
  public float heightToSet;
  public int frameDelay = 60;
  [HideInInspector]public int fB;
  [HideInInspector]public TileType sT;
  [HideInInspector]public int r;
  [HideInInspector]public bool[] hla;
  //float uvTileWidth = regularTileSet.tileWidth / texWidth;
  //float uvTileHeight = regularTileSet.tileWidth / texHeight;

  //Langston's ant
  public string sequence;
  public float antSpeed;
  public bool track;
  float lerpTime = 1f;
  float currentLerpTime;
  public GameObject newAnt;
  public HexTile startingTile;
  public HexTile forwardTile;

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.T))
    {
      track = !track;
    }
    //cyclical hex life
		if(Input.GetKeyDown(KeyCode.Return))
		{
			/*
			bool[] hlr = new bool[84];
			float modu = Random.Range (0.2f, 0.8f);
			for (int i = 0; i < 84; i++) {
				float rand = Random.Range (0f, 1.0f);
				if (rand > modu) {
					hlr [i] = true;
				}
			}
      /* 
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
			
			
			foreach(HexTile ht in activeWorld.tiles) {
				//int r = ht.plate % 3;
        //int r = Random.Range(0,2);
        int r = UnityEngine.Random.Range(2,8);
				ht.ChangeType ((TileType)r);
			}
      /* */
      //RandomWorldState();
      b = !b;
      if(b)
      {
        if(randomAnt){sequence = RandomAnt();}
        GameObject ant = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        StartCoroutine(LangstonsHex0(sequence, ant));
        //StartCoroutine(LangstonsHex2(sequence, newAnt, startingTile, forwardTile));
      }
			fB = 0;
		}	
    
		fB++;
		if (b) 
    {
		  //HLShift ();
      //CyclicalHexLife();
      //JoeLife();
      //TheDualityOfLife();
      //RandomWorldState();
      
		  fB = 0;
    }

    if(Input.GetKeyDown(KeyCode.M))
    {
      byte[] id = new byte[32];
      
      for (int i = 0; i < 32; i++)
      {
          id[i] = (byte)i;
      }
      
      MONTest(id);
    }

    if(Input.GetKeyDown(KeyCode.N))
    {
      if(randomAnt){sequence = RandomAnt();}
        GameObject ant = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        StartCoroutine(LangstonsHex0(sequence, ant));
    }
    //Type Changer
    /* 
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
    */
  }
  
  void MONTest(byte[] id)
  {
    Mon mon = new Mon(id);
  }

  public void RandomWorldState()
  {
    int[] st = new int[activeWorld.tiles.Count];
    for(int i = 0; i < activeWorld.tiles.Count; i++)
    {
      st[i] = UnityEngine.Random.Range(0,8);
    }
    activeWorld.SetState(st);
  }
  public IEnumerator LangstonsHex0(string seq, GameObject ant)
  {
    Debug.Log("Select starting tile");
    HexTile hitTile = new HexTile();
    bool selecting = true;
    while(selecting)
    {
      if(Input.GetKeyDown(KeyCode.Mouse0))
      {
        r++;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
	        GameObject plateO = hit.transform.gameObject;
	        Vector3 c = new Vector3 ();
	        Vector3 h = hit.point;
	        float test;
	        float dist = 9999999;
	        foreach (HexTile ht in activeWorld.tiles) 
	        {
			      c = ht.hexagon.center;
			      test = (c - h).sqrMagnitude;
			      if (test < dist) 
            {
			    	  dist = test;
				      hitTile = ht;
			      }
	        }
          selecting = false;
        } 
      }
      yield return null;
    }
    Debug.Log(hitTile.index);
    StartCoroutine(LangstonsHex1(seq, ant, hitTile));
    yield return null;
  }
  public IEnumerator LangstonsHex1(string seq, GameObject ant, HexTile onTile)
  {
    Debug.Log("Select forward tile");
    HexTile hitTile = new HexTile();
    bool selecting = true;
    while(selecting)
    {
      if(Input.GetKeyDown(KeyCode.Mouse0))
      {
        r++;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
	        GameObject plateO = hit.transform.gameObject;
	        Vector3 c = new Vector3 ();
	        Vector3 h = hit.point;
	        float test;
	        float dist = 9999999;
	        foreach (HexTile ht in activeWorld.tiles) 
	        {
            foreach(int i in onTile.neighbors)
            {
              if(ht.index == i)
              {
                 c = ht.hexagon.center;
			          test = (c - h).sqrMagnitude;
			          if (test < dist) 
                {
			    	      dist = test;
				          hitTile = ht;
			          }
                selecting = false;
              }
            }
	        }
        } 
      }
      yield return null;
    }
    Debug.Log(hitTile.index);
    StartCoroutine(LangstonsHex2(seq, ant, onTile, hitTile));
    yield return null;
  }
  public IEnumerator LangstonsHex2(string seq, GameObject ant, HexTile onTile, HexTile forwardTile)
  {
    Debug.Log(seq);
    int back, forward, right, left, port, starboard;
    back = forward = right = left = port = starboard = 0;
    char[] dna = seq.ToCharArray();
    //HexTile tOut = new HexTile();
    HexTile nextTile = new HexTile();
    int toSet = 1;
    Vector3 o = activeWorld.origin;
    Vector3 ve = Camera.main.transform.position - o;
    float camMag = ve.magnitude *.4f;
    Vector3 lVel = Vector3.zero;
    float smoothTime = 1;
    float dist;
    Vector3 from;
    Vector3 to;
    Transform antTrans = ant.transform;
    antTrans.position = onTile.hexagon.center;
    
    //Set up first tile
    forward = forwardTile.index;
    if(!onTile.hexagon.isPentagon)
      {
        Vector3 fVec = forwardTile.hexagon.center - onTile.hexagon.center; 
        Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
        for(int i = 0; i < 5; i++)
        {
          Vector3 nextVec = Quaternion.AngleAxis(60*(i+1), rotationAxis) * fVec;
          float test = 99999;
          int nextNei = 0;
          foreach(int nei in onTile.neighbors)
          {
            Vector3 v = activeWorld.tiles[nei].hexagon.center - onTile.hexagon.center;
            float tV = (v - nextVec).sqrMagnitude;
            if(tV < test)
            {
              nextNei = nei;
              test = tV;
            }
          }
          switch(i)
          {
            case 0: right = activeWorld.tiles[nextNei].index; break;
            case 1: starboard = activeWorld.tiles[nextNei].index; break;
            case 2: back = activeWorld.tiles[nextNei].index; break;
            case 3: port = activeWorld.tiles[nextNei].index; break;
            case 4: left = activeWorld.tiles[nextNei].index; break;
          }
        }
      }
        else
        {
          back = forward;
          Vector3 backVec = forwardTile.hexagon.center - onTile.hexagon.center; 
          Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
          for(int i = 0; i < 4; i++)
          {
            Vector3 nextVec = Quaternion.AngleAxis(72*(i+1), rotationAxis) * backVec;
            float testF = 99999;
            int nextNei = 0;
            foreach(int nei in onTile.neighbors)
            {
              Vector3 v = activeWorld.tiles[nei].hexagon.center - onTile.hexagon.center;
              float tV = (v - nextVec).sqrMagnitude;
              if(tV < testF)
              {
                nextNei = nei;
                testF = tV;
              }
            }
            switch(i)
            {
                case 0: port = activeWorld.tiles[nextNei].index; break;
                case 1: left = activeWorld.tiles[nextNei].index; break;
                case 2: right = activeWorld.tiles[nextNei].index; break;
                case 3: starboard = activeWorld.tiles[nextNei].index; break;
            }
          }
        }
    /*
    port = onTile.neighbors[1];
    left = onTile.neighbors[2];
    forward = onTile.neighbors[4];
    right = onTile.neighbors[5];
    starboard = onTile.neighbors[3];
    */
    /* 
    activeWorld.tiles[back].ChangeType(TileType.Dark);
    activeWorld.tiles[port].ChangeType(TileType.Fire);
    activeWorld.tiles[left].ChangeType(TileType.Earth);
    activeWorld.tiles[forward].ChangeType(TileType.Light);
    activeWorld.tiles[right].ChangeType(TileType.Water);
    activeWorld.tiles[starboard].ChangeType(TileType.Air);
    */
    
    while(b)
    {
      /* 
      currentLerpTime += Time.deltaTime;
      if (currentLerpTime > lerpTime) {
          currentLerpTime = lerpTime;
      }
      float lerp = currentLerpTime / lerpTime;
      */
      //Camera
      if(track){
        from = Camera.main.transform.position;
        to = (onTile.hexagon.center - activeWorld.origin)*2.4f;
        dist = (to - from).sqrMagnitude;
        smoothTime = .24f;//+(0.95f/dist);
        Camera.main.transform.position = Vector3.SmoothDamp(from, to, ref lVel, smoothTime);
        Camera.main.transform.LookAt(currentWorldTrans);
      }
      //Switch to next color
      if(onTile.type == TileType.None)
      {
        Debug.Log(onTile.index);
      }
      toSet = (int)onTile.type + 1;
      if(toSet > 7){toSet = 1;}
      if(sequence.Length < 6 && toSet > sequence.Length)
      {
        toSet = 1;
      }
      onTile.ChangeType((TileType)toSet);
      
      //Make next movement based on dna
      char seqChar = dna[onTile.antPasses];
      onTile.antPasses += 1;
      if(onTile.antPasses > dna.Length - 1)
      {
        onTile.antPasses = 0;
      }
      switch(seqChar) 
      {
        case 'B': nextTile = activeWorld.tiles[back]; break;
        case 'P': nextTile = activeWorld.tiles[port]; break;
        case 'L': nextTile = activeWorld.tiles[left]; break;
        case 'F': nextTile = activeWorld.tiles[forward]; break;
        case 'R': nextTile = activeWorld.tiles[right]; break;
        case 'S': nextTile = activeWorld.tiles[starboard]; break;
        default: Debug.Log("Invalid char" + dna[onTile.antPasses]); break;
      }

      if(!nextTile.hexagon.isPentagon)
      {
        back = onTile.index;
        Vector3 backVec = onTile.hexagon.center - nextTile.hexagon.center; 
        Vector3 rotationAxis = nextTile.hexagon.center - activeWorld.origin;
        for(int i = 0; i < 5; i++)
        {
          Vector3 nextVec = Quaternion.AngleAxis(60*(i+1), rotationAxis) * backVec;
          float test = 99999;
          int nextNei = 0;
          foreach(int nei in nextTile.neighbors)
          {
            Vector3 v = activeWorld.tiles[nei].hexagon.center - nextTile.hexagon.center;
            float tV = (v - nextVec).sqrMagnitude;
            if(tV < test)
            {
              nextNei = nei;
              test = tV;
            }
          }
          switch(i)
          {
            case 0: port = activeWorld.tiles[nextNei].index; break;
            case 1: left = activeWorld.tiles[nextNei].index; break;
            case 2: forward = activeWorld.tiles[nextNei].index; break;
            case 3: right = activeWorld.tiles[nextNei].index; break;
            case 4: starboard = activeWorld.tiles[nextNei].index; break;
          }
        }
      }
        else
        {
          //Debug.Log("Registered pentagon: " + nextTile.index);
          back = onTile.index;
          forward = onTile.index;
          Vector3 backVec = onTile.hexagon.center - nextTile.hexagon.center; 
          Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
          for(int i = 0; i < 4; i++)
          {
            Vector3 nextVec = Quaternion.AngleAxis(72*(i+1), rotationAxis) * backVec;
            float testF = 9999;
            int nextNei = 0;
            foreach(int nei in nextTile.neighbors)
            {
              Vector3 v = activeWorld.tiles[nei].hexagon.center - nextTile.hexagon.center;
              float tV = (v - nextVec).sqrMagnitude;
              if(tV < testF)
              {
                nextNei = nei;
                testF = tV;
              }
            }
            switch(i)
            {
                case 0: port = activeWorld.tiles[nextNei].index; break;
                case 1: left = activeWorld.tiles[nextNei].index; break;
                case 2: right = activeWorld.tiles[nextNei].index; break;
                case 3: starboard = activeWorld.tiles[nextNei].index; break;
            }
          }
        }
      antTrans.position = nextTile.hexagon.center;
      antTrans.up = nextTile.hexagon.center - activeWorld.origin;
      //Debug.DrawLine((onTile.hexagon.center-activeWorld.origin)*1.1f, (nextTile.hexagon.center-activeWorld.origin)*1.1f, Color.blue, 1000.0f, false);
      onTile = nextTile;
   
      if(antSpeed > 0)
      {
        yield return new WaitForSeconds(antSpeed);
      }
      yield return null;
    }

    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(TileType.Gray);
      ht.antPasses = 0;
      ht.generation = 0;
    }
    Debug.Log("Ant stopped");
    
    yield return null;
  }
 
  public string RandomAnt()
  {
    int length = Random.Range(3,7);
    string[] ant = new string[length];
    string s = "";
    for(int i = 0; i < length; i++)
    {
      switch(Random.Range(0,6))
      {
        case 0: ant[i] = "R"; break;
        case 1: ant[i] = "L"; break;
        case 2: ant[i] = "P"; break;
        case 3: ant[i] = "S"; break;
        case 4: ant[i] = "F"; break;
        case 5: ant[i] = "B"; break;
      }
      s += ant[i];
    }
    return s;
  }
  public void TheDualityOfLife()
  {
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.typeToSet = TileType.Gray;//ht.type;
      int s = 0;
      foreach(int i in ht.neighbors)
      {
        switch (activeWorld.tiles[i].type)
        {
            case TileType.Fire:
              s += 1;
              break;
            case TileType.Water:
              s -= 1;
              break;
            case TileType.Air:
              s += 2;
              break;
            case TileType.Earth:
              s -= 2;
              break;
            case TileType.Light:
              s += 3;
              break;
            case TileType.Dark:
              s -= 3;
              break;
            default: break;
        }
      }
      if(ht.type == TileType.Water || ht.type == TileType.Earth || ht.type == TileType.Dark)
      {
        if(s > 0)
        {
         s = s % 3;
          switch(s)
          {
           case 0: ht.typeToSet = TileType.Fire; break;
           case 1: ht.typeToSet = TileType.Air; break;
           case 2: ht.typeToSet = TileType.Light; break;
           default: break;
         }
       }
       if(s<0)
       {
        // ht.typeToSet = ht.type;
       }
      }
      if(ht.type == TileType.Fire || ht.type == TileType.Air || ht.type == TileType.Light)
      if(s < 0)
      {
        s = (s % 3)*-1;
        switch(s)
        {
          case 0: ht.typeToSet = TileType.Water; break;
          case 1: ht.typeToSet = TileType.Earth; break;
          case 2: ht.typeToSet = TileType.Dark; break;
          default: break;
        }
      }
      if(s>0)
      {
       // ht.typeToSet = ht.type;
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.typeToSet);
    }
  }
  public void GenerationalJoeLife()
  {
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.typeToSet = TileType.Gray;
      TileType nextTile = ht.type;
      int s = 0;
      foreach(int i in ht.neighbors)
      {
      switch (activeWorld.tiles[i].type)
      {
          case TileType.Fire:
            s += 1;
            break;
          case TileType.Water:
            s += 2;
            break;
          default: break;
      }
      }
      if(ht.type == TileType.Gray && s == 4)
      {
        ht.typeToSet = TileType.Fire;
      }
      if(ht.type == TileType.Fire && s != 0 && s != 5 && s <= 6)
      {
        ht.typeToSet = TileType.Water;
      }
      if(ht.type == TileType.Water && (s == 1 || s == 2))
      {
        ht.typeToSet = TileType.Water;
      }
      if(ht.type == TileType.Water && s == 4)
      {
        ht.typeToSet = TileType.Fire;
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.typeToSet);
    }
  }
  public void JoeLife()
  {
     foreach(HexTile ht in activeWorld.tiles)
    {
      ht.typeToSet = TileType.Gray;
      TileType nextTile = ht.type;
      int s = 0;
      foreach(int i in ht.neighbors)
      {
      switch (activeWorld.tiles[i].type)
      {
          case TileType.Fire:
            s += 1;
            break;
          case TileType.Water:
            s += 2;
            break;
          default: break;
      }
      }
      if(ht.type == TileType.Gray && s == 4)
      {
        ht.typeToSet = TileType.Fire;
      }
      if(ht.type == TileType.Fire && s != 0 && s != 5 && s <= 6)
      {
        ht.typeToSet = TileType.Water;
      }
      if(ht.type == TileType.Water && (s == 1 || s == 2))
      {
        ht.typeToSet = TileType.Water;
      }
      if(ht.type == TileType.Water && s == 4)
      {
        ht.typeToSet = TileType.Fire;
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.typeToSet);
    }
  }

  public void CyclicalHexLife()
  {
    foreach(HexTile ht in activeWorld.tiles)
    {
      TileType nextTile = ht.type;
    
      switch (ht.type)
      {
          case TileType.Water:
            nextTile = TileType.Dark;
            break;
          case TileType.Dark:
            nextTile = TileType.Earth;
            break;
          case TileType.Earth:
            nextTile = TileType.Water;
            break;
          default: break;
      }
      foreach(int nei in ht.neighbors)
      {
        if(activeWorld.tiles[nei].type == nextTile)
        {
          ht.typeToSet = nextTile;
        }
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      if(ht.type != ht.typeToSet)
      {
        ht.ChangeType(ht.typeToSet);
      }
    }
  }

  public World Initialize(bool loadWorld = false)
  {
    if (loadWorld)
    {
      //random world test
      seed = new byte[32];
      for(int i = 0; i < 32; i++)
      {
        seed[i] = (byte)UnityEngine.Random.Range(0,256);
      }
      activeWorld = LoadWorld();
      activeWorld.Populate(seed);
    }
    else
    {
      activeWorld = new World();
      activeWorld.PrepForCache(worldScale, worldSubdivisions);
    }

    currentWorldObject = new GameObject("World");
    currentWorldTrans = currentWorldObject.transform;
    worldRenderer = GetComponent<WorldRenderer>();
    //changed this to run TriPlates instead of HexPlates
    foreach (GameObject g in worldRenderer.HexPlates(activeWorld, regularTileSet))
    {
      g.transform.parent = currentWorldTrans;
    }
    Debug.Log(activeWorld.tiles.Count);
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.type);
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
 /* }
   
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
	}/*
  public void HL() //automata for 2 types
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
					
					if (activeWorld.tiles [i].type == TileType.Luna)
					{
						luna++;
					}
					 
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
	}     */
}
