using System.Collections.Generic;
using UnityEngine;

namespace CrossyRoad.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WorldSegmentScriptableObject", order = 1)]
    public class WorldSegmentScriptableObject : ScriptableObject
    {
        public int MinAmount = 1;
        public int MaxAmount = 1;
        public CarType CarType = CarType.None;
        public string Name;
        public float Weight = 1f;
        public List<WorldSegment> Segments;
    }
}