using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CrossyRoad.Data
{
    [Serializable]
    public class WorldSegment
    {
        public float WeightedSpawnChance = 1f;
        public GameObject Prefab = null!;
    }
}