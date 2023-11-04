using System.Collections.Generic;
using UnityEngine;

namespace CrossyRoad.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WorldSegmentScriptableObject", order = 1)]
    public class WorldSegmentScriptableObject : ScriptableObject
    {
        public string Name;
        public List<WorldSegment> Segments;
    }
}