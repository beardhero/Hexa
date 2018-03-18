﻿using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

[Serializable]
public enum TileType {
  None, 
  Water, Fire, Earth, Air, Sol, Luna, Metal, Ice, Vapor, Crystal, Arbor, Astral,
  Brown, Tan, Red, White, Black, Blue, Green, Gray,
  Sand, PinkSand, Mud, Dirt, Grass,
  Stone, SmoothStone
};

[Serializable]
public class Tile
{
  public float height;

  public bool border;
  public bool posBorderCheck= false;

  public TileType type;

  public Tile(){}

  public Tile(float startingHeight)
  {
    height = startingHeight;
  }
  
  public Tile(float x, float y, int width, float lacunarity, float probability, float height_in = -1)
  {
	type = TileType.None;
    if (height_in == -1)
      height = 0;
    else
      height = height_in;
  }

  public virtual void OnUnitEnter(){}
}

public class Tile_Grass : Tile
{ 
  public override void OnUnitEnter()
  {
    Debug.Log("The grass rustles as a unit enters.");
    // Some custom tile logic here
  }
}