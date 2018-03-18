/*
 * Copyright (c) 2015 Colin James Currie.
 * All rights reserved.
 * Contact: cj@cjcurrie.net
 */

 // @INFO: This script is responsible for rendering zone data and performing simulation at the zone level

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ZoneManager : MonoBehaviour
{
  // === Public ===
  public TileSet regularTileSet;
	
  // === Private ===
  Transform boardHolder;
  List<Vector2> gridPositions;
  List<Vector2> topTilePositions;

  // === Cache ===
  //LayerMask layermask;

  public void Initialize (Zone z)
  {
    //layermask = 1<<8;   // Layer 8 is set up as "Chunk" in the Tags & Layers manager
  }

  public void OnTapInput(Vector2 tap)
  {
    
  }
}