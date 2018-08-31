using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Unity.Generator;
public enum WorldSize {None, Small, Medium, Large};
public enum WorldType {None, Verdant, Frigid, Oceanic, Barren, Volcanic, Radiant, Gaseous};
public enum Season {None, Spring, Summer, Fall, Winter};
public enum AxisTilt { None, Slight, Moderate, Severe };      // Affects intensity of difficulty scaling during seasons

[System.Serializable]
public class World
{
  public int[] state;
  public const string cachePath = "currentWorld.save";
  public string name;
  public WorldSize size;
  public WorldType type;
  public Season season;
  public AxisTilt tilt;
  public TileType element;
  public float glyphProb = 0.01f; //distribution of glyphs

  [HideInInspector] public SerializableVector3 origin;
  [HideInInspector] public int circumferenceInTiles;
  [HideInInspector] public float circumference, radius;
  [HideInInspector] public int numberOfPlates; //Set by polysphere on cache
  [HideInInspector] public float seaLevel = 0;
  [HideInInspector] public List<HexTile> tiles;
  [HideInInspector] public List<TriTile> triTiles;
  [HideInInspector] public List<HexTile> pentagons;
  //[HideInInspector] public List<Plate> plates;
  [HideInInspector] public Dictionary<int, int> tileToPlate; //key hextile.index, value plate index

  private bool neighborInit;
  private List<List<HexTile>> _neighbors;
  public List<List<HexTile>> neighbors{
    get{
      if (!neighborInit)
      {
        if (tiles.Count < 1)
          Debug.LogError("Making neighbor list from null tiles");

        neighborInit = true;
        _neighbors = new List<List<HexTile>>();

        foreach (HexTile t in tiles)
        {
          List<HexTile> neighbs = new List<HexTile>();

          for (int i=0; i<t.hexagon.neighbors.Length; i++)
          {
            try
            {
              neighbs.Add(tiles[t.hexagon.neighbors[i]]);
            }
            catch (System.Exception e)
            {
              //Debug.LogError("tile "+t.index+"'s "+Direction.ToString(i)+" neighbor is bad: "+t.hexagon.neighbors[i]);
            }
          }
          _neighbors.Add(neighbs);
        }
      }

      return _neighbors;
    }
    set{}
  }

  public World()
  {
    origin = Vector3.zero;
  }

  public World(WorldSize s, WorldType t, Season se, AxisTilt at)
  {
    size = s;
    type = t;
    season = se;
    tilt = at;
    origin = Vector3.zero;
  }
  public void Populate(byte[] seed)
  {
    Object[] airBiome = Resources.LoadAll("Air/");
    Object[] earthBiome = Resources.LoadAll("Earth/");
    Object[] waterBiome = Resources.LoadAll("Water/");
    Object[] fireBiome = Resources.LoadAll("Fire/");
    Object[] darkBiome = Resources.LoadAll("Dark/");
    Object[] lightBiome = Resources.LoadAll("Light/");
    Object[] misc = Resources.LoadAll("Misc/");

    Perlin perlin = new Perlin();
    float sc = 99.0124f; //99.0f
    float objSc = 4204f;
    float genSc = .124f;
    double tiers = 42; //42
    int h = 0;
    float seaLevel = tiles[0].hexagon.scale - 1;
    float maxHeight = tiles[0].hexagon.scale + 24;
    float minHeight = seaLevel - 6;
    
    for(int i = 0; i < seed.Length; i++)
    {
      
      //Heightmap
      UnityEngine.Random.InitState(seed[i]);
      int typeShift = Random.Range(0,13);
      perlin.Seed = seed[i];

      perlin.Frequency = .0000024;//.01618;// * Random.Range(0.5f,1.5f);// * i; //.0000024
      perlin.Lacunarity = 1.24;// * Random.Range(0.5f,1.5f);//2.4;
      perlin.Persistence = .24;// * Random.Range(0.5f,1.5f); //.24
      perlin.OctaveCount = 6;// + i;
      if(i > 24)
      {
        sc = 124f;
        perlin.Frequency = .000001618;// * Random.Range(0.5f,1.5f);// * i; //.0000024
        perlin.Lacunarity = 2.24;// * Random.Range(0.5f,1.5f);//2.4;
        perlin.Persistence = .24;// * Random.Range(0.5f,1.5f); //.24
        perlin.OctaveCount = 3;// + i;
      }
      
      //float s = Random.Range(-99999, 99999);
      foreach (HexTile ht in tiles)
      {
        double perlinVal = perlin.GetValue(ht.hexagon.center.x * sc, ht.hexagon.center.y * sc, ht.hexagon.center.z * sc);
        double v1 = perlinVal*tiers;//*i; 
        h = (int)v1;
        //Debug.Log(v1);
        ht.hexagon.scale += h;

        int v = Mathf.Abs((int)ht.type + h + typeShift);
        int t = (v % 11) + 2; //using 12 types
        //Debug.Log(t);
        //t++;
        ht.type = (TileType)t;

        if(ht.hexagon.scale > maxHeight)
        {
          ht.hexagon.scale = seaLevel;
        }
        if(ht.hexagon.scale < minHeight || ht.hexagon.scale < 1)
        {
          ht.hexagon.scale = seaLevel;
        }
        double g = perlin.GetValue(ht.hexagon.center.x * genSc, ht.hexagon.center.y * genSc, ht.hexagon.center.z * genSc);
       
        int gen = (int)g;
        ht.generation += gen;
        ht.generation = Mathf.Abs((ht.generation % 3)) + 2;

        if(i == 31 && 100f/tiles.Count > Random.Range(0f,1.0f))
        {
          ht.generation = Random.Range(5,21);
        } 
      }
    }
    
    
    
    //biomes and ocean
  
    int water = 0; 
    int fire = 0;
    int vapor = 0;
    int crystal = 0;

    seaLevel = AverageTileHeight() + 0.1f;
    //water world
    foreach(HexTile ht in tiles)
    {
      if(ht.type == TileType.Water){water++;}
      if(ht.type == TileType.Fire){fire++;}
      if(ht.type == TileType.Vapor){vapor++;}
      if(ht.type == TileType.Crystal){crystal++;}
    }

    if(water >= fire && water >= vapor && water >= crystal)
    {
      element = TileType.Water;
      foreach(HexTile ht in tiles)
      {
        /* 
        if(ht.type == TileType.Water || ht.type == TileType.Fire)
        {
          ht.hexagon.scale = seaLevel;
        }
        */
      }
      LightToDark();
    }
    //fire world
    if(fire > water && fire > vapor && fire > crystal)
    {
      element = TileType.Fire;
      foreach(HexTile ht in tiles)
      {
        /* 
        if(ht.type == TileType.Water || ht.type == TileType.Fire)
        {
          ht.hexagon.scale = seaLevel;
        }
        */
      }
      DarkToLight();
    }
    //vapor
    if(vapor >= water && vapor >= fire && vapor >= crystal)
    {
      element = TileType.Vapor;
      foreach(HexTile ht in tiles)
      {
        /* 
        if(ht.type == TileType.Vapor || ht.type == TileType.Crystal)
        {
          ht.hexagon.scale = seaLevel;
        }
        */
      }
      DarkToLight();
    }
    //crystal
    if(crystal > fire && crystal > vapor && crystal > water)
    {
      element = TileType.Crystal;
      foreach(HexTile ht in tiles)
      {
        /* 
        if(ht.type == TileType.Vapor || ht.type == TileType.Crystal)
        {
          ht.hexagon.scale = seaLevel;
        }
        */
      }
      LightToDark();
    }

    foreach(HexTile ht in tiles)
    {
      if(ht.hexagon.scale <= seaLevel)
      {
        ht.type = element;
        ht.oceanTile = true;
        ht.passable = false;
        ht.hexagon.scale = seaLevel;
      }
    }

    //biome objects
    foreach(HexTile ht in tiles)
    {
      double v2 =  Mathf.Abs((float)perlin.GetValue(ht.hexagon.center.x * objSc, ht.hexagon.center.y * objSc, ht.hexagon.center.z * objSc));
      double v3 = .01618;//Random.Range(0.0f,1.0f);
      if(v3 > v2 && !ht.oceanTile)
      {
        ht.passable = false;
        switch(ht.type)
        {
          case TileType.Gray: ht.objectToPlace = Random.Range(0,misc.Length); break;
          case TileType.Water: ht.objectToPlace = Random.Range(0,waterBiome.Length); break;
          case TileType.Fire: ht.objectToPlace = Random.Range(0,fireBiome.Length); break;
          case TileType.Earth: ht.objectToPlace = Random.Range(0,earthBiome.Length); break;
          case TileType.Air: ht.objectToPlace = Random.Range(0,airBiome.Length); break;
          case TileType.Dark: ht.objectToPlace = Random.Range(0,darkBiome.Length); break;
          case TileType.Light: ht.objectToPlace = Random.Range(0,lightBiome.Length); break;
          default: break;
        }
      }
      
    }
  }
  public void ReadState()
  {
    //state of tiletypes
    state = new int[tiles.Count];
    for (int i = 0; i < tiles.Count; i++)
    {
      state[i] = (int)tiles[i].type;
    }
  }

  public void SetState(int[] st)
  {
    state = st;
    foreach(HexTile ht in tiles)
    {
      ht.ChangeType((TileType)state[ht.index]);
    }
  }

  public void Clear()
  {
    foreach(HexTile ht in tiles)
    {
      if(ht.type != TileType.Gray){ht.ChangeType(TileType.Gray);}
      ht.antPasses = 0;
      ht.generation = 0;
    }
  }
/*
  public void Imbue(int[] glyph, HexTile origin)
  {

  }
*/

  public float AverageTileHeight()
  {
    float h = 0;
    foreach(HexTile ht in tiles)
    {
      h += ht.hexagon.scale;
    }
    h /= tiles.Count;
    return h;
  }

  public void PrepForCache(float scale, int subdivisions)
  {
    if (tiles == null || tiles.Count == 0)
    {
      neighborInit = false;
      PolySphere sphere = new PolySphere(Vector3.zero, scale, subdivisions);
      //make the tileToPlate dict
      tileToPlate = new Dictionary<int, int>();
      CacheHexes(sphere);
      //CacheTriangles(sphere);
    }
    else
      Debug.Log("tiles not null during cache prep");
  }
  public void CacheTriangles(PolySphere s)
  {
    triTiles = new List<TriTile>(s.triTiles);
  }
  public void CacheHexes(PolySphere s)  // Executed by the cacher.  @CHANGE: Now directly converting spheretiles to hextiles
  { 
    tiles = new List<HexTile>(s.hexTiles);
    neighborInit = false;
  }

  public void LightToDark()
    {
      foreach(HexTile ht in tiles)
      {
        if(ht.type == TileType.Light){ht.type = TileType.Dark;}
        if(ht.type == TileType.Metal){ht.type = TileType.Ice;}
        if(ht.type == TileType.Fire){ht.type = TileType.Water;}
        if(ht.type == TileType.Astral){ht.type = TileType.Arbor;}
        if(ht.type == TileType.Air){ht.type = TileType.Earth;}
        if(ht.type == TileType.Crystal){ht.type = TileType.Vapor;}
        if(ht.type == TileType.Sol){ht.type = TileType.Luna;}
      }
    }
    public void DarkToLight()
    {
      foreach(HexTile ht in tiles)
      {
        if(ht.type == TileType.Dark){ht.type = TileType.Light;}
        if(ht.type == TileType.Ice){ht.type = TileType.Metal;}
        if(ht.type == TileType.Water){ht.type = TileType.Fire;}
        if(ht.type == TileType.Arbor){ht.type = TileType.Astral;}
        if(ht.type == TileType.Earth){ht.type = TileType.Air;}
        if(ht.type == TileType.Vapor){ht.type = TileType.Crystal;}
        if(ht.type == TileType.Luna){ht.type = TileType.Sol;}
      }
    }

}
