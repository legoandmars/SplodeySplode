using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrossyRoad.World
{
    public class ObstacleCoordinatesController : MonoBehaviour
    {
        public static List<(int, int)> ExistingCoordinates = new();
        
        // always remove 0,0 so a tree doesnt spawn on the player
        // and like 0,5 to make the first bit easier
        public static List<(int, int)> DontSpawnCoordinates = new()
        {
            (0, 0),
            (1, 0),
            (2, 0),
            (3, 0),
            (4, 0),
            (5, 0),
        };

        public void Awake()
        {
            ExistingCoordinates = new(); // reset between sessions
        }
    }
}