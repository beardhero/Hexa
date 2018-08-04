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
    for(int i = 0; i < 32; i++)
    {
      if(i == 0)
      {
        seaLevel = tiles[0].hexagon.scale - 1;
      }
    // height seed.
    int h = 0;
    Perlin perlin = new Perlin();
    perlin.Seed = seed[i];
    perlin.Frequency = .0000024;
    perlin.Lacunarity = 2.4;
    perlin.Persistence = .24;
    perlin.OctaveCount = 6;
    float sc = 99.0f;
    int tiers = 42; 
    //float s = Random.Range(-99999, 99999);
    foreach (HexTile ht in tiles)
    {
      double v1 = (tiers * perlin.GetValue(ht.hexagon.center.x * sc, ht.hexagon.center.y * sc, ht.hexagon.center.z * sc));
      h = (int)v1;
      ht.hexagon.scale += h;
      int v = ((int)ht.type + h);
      int t = v % 7;
      t++;
      ht.type = (TileType)t;
    }
    }
    int water = 0; 
    int fire = 0;
    foreach(HexTile ht in tiles)
    {
      if(ht.type == TileType.Water){water++;}
      if(ht.type == TileType.Fire){fire++;}
    }
    if(water >= fire)
    {
      element = TileType.Water;
      foreach(HexTile ht in tiles)
      {
        if(ht.type == TileType.Water || ht.type == TileType.Fire)
        {
          ht.hexagon.scale = seaLevel;
        }
        if(ht.type == TileType.Light){ht.type = TileType.Dark;};
      }
    }
    else
    {
      element = TileType.Fire;
      foreach(HexTile ht in tiles)
      {
        if(ht.type == TileType.Water || ht.type == TileType.Fire)
        {
          ht.hexagon.scale = seaLevel;
        }
        if(ht.type == TileType.Dark){ht.type = TileType.Light;};
      }
    }
    foreach(HexTile ht in tiles)
    {
      if(ht.hexagon.scale <= seaLevel)
      {
        ht.type = element;
        ht.hexagon.scale = seaLevel;
      }
    }
    //biome objects
    foreach(HexTile ht in tiles)
    {
      switch(ht.type)
      {
        case TileType.Gray:
        case TileType.Water:
        case TileType.Fire:
        case TileType.Earth:
        case TileType.Air:
        case TileType.Sol:
        case TileType.Luna:
        case TileType.Dark:
        case TileType.Light:
        default: break;
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
}
