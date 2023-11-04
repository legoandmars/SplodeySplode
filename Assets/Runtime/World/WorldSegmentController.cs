using System.Collections.Generic;
using System.Linq;
using CrossyRoad.Data;
using UnityEngine;

namespace CrossyRoad.World
{
    public class WorldSegmentController : MonoBehaviour
    {
        [SerializeField]
        private List<WorldSegmentScriptableObject> _worldSegments = new();

        [SerializeField]
        private GameObject _roadSeperator = null!;
        
        [SerializeField]
        private Transform _worldSegmentContainer = null!;

        [SerializeField]
        private int _spawnChunkSize = 10;

        [SerializeField]
        private int _idealChunksOnScreen = 3;

        private int _currentWorldSegmentIndex = -5;
        private int _bestPlayerTile = 0;
        
        private int _worldSegmentTypeIndex = 0;
        private int _remainingWorldSegmentsUntilNewTypeNeeded = 8; // always start us off with some grass!
        private bool _lastWasRoad = false;
        
        // TODO Pool if time had it will massively increase performance
        private List<GameObject> _spawnedWorldSegments = new();
        
        // TODO Make sure this won't break when moving left right or down and then back up. only do on new records
        public void PlayerLocationUpdated(int playerWorldSegment)
        {
            if (playerWorldSegment <= _bestPlayerTile) return;
            _bestPlayerTile = playerWorldSegment;

            if (_bestPlayerTile % _spawnChunkSize != 0) return;
            // despawn last 10 from list
            foreach (var segment in _spawnedWorldSegments.Take(_spawnChunkSize))
            {
                Destroy(segment);
            }
            
            // remove from list since no longer necessary
            _spawnedWorldSegments.RemoveRange(0, _spawnChunkSize);
            
            GenerateChunk(_spawnChunkSize);
        }
        
        // my generic weighted list method from CMIYC
        private int IndexFromWeights(List<float> weights)
        {
            var totalWeight = weights.Sum(x => x);
            var weightedRandom = Random.Range(0f, totalWeight);

            var traversedWeight = 0f;
            for (int i = 0; i < weights.Count; i++)
            {
                var currentWeightedIndex = traversedWeight + weights[i];

                if (weightedRandom <= currentWeightedIndex)
                {
                    return i;
                }
                traversedWeight = currentWeightedIndex;
            }

            Debug.Log("Weighted random failed");
            return 0;
        }
        
        void Start()
        {
            Debug.Log("Generating world...");
            GenerateChunk(_spawnChunkSize * _idealChunksOnScreen);
        }

        void GenerateChunk(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (_remainingWorldSegmentsUntilNewTypeNeeded <= 0)
                {
                    // get new world segment
                    List<float> weights = _worldSegments.Select(x => x.Weight).ToList();
                    if (_currentWorldSegmentIndex != 0)
                    {
                        // make lower odds of rerolling into the same segment (it should still be possible)
                        // TODO: Short circuit when it's over like 10 to avoid making the game unplayable lol
                        weights[_worldSegmentTypeIndex] *= 0.25f;
                    }

                    _worldSegmentTypeIndex = IndexFromWeights(weights);
                    // set amount until we reroll
                    _remainingWorldSegmentsUntilNewTypeNeeded = Random.Range(_worldSegments[_worldSegmentTypeIndex].MinAmount, _worldSegments[_worldSegmentTypeIndex].MaxAmount + 1);
                }
                // spawn
                
                var segment = SpawnRandomWorldSegment(_worldSegments[_worldSegmentTypeIndex]);
                segment.transform.SetParent(_worldSegmentContainer);
                segment.transform.localPosition = new Vector3(_currentWorldSegmentIndex, 0, 0);
                _spawnedWorldSegments.Add(segment);

                if (_worldSegmentTypeIndex == 1) //hardcoded road lol
                {
                    if (_lastWasRoad)
                    {
                        Instantiate(_roadSeperator, segment.transform, false);
                    }
                }
                
                _lastWasRoad = _worldSegmentTypeIndex == 1;
                _currentWorldSegmentIndex += 1;
                _remainingWorldSegmentsUntilNewTypeNeeded -= 1;
            }
        }

        GameObject SpawnRandomWorldSegment(WorldSegmentScriptableObject worldSegment)
        {
            // get weighted world segment
            var segmentIndex = IndexFromWeights(worldSegment.Segments.Select(x => x.WeightedSpawnChance).ToList());
            var segment = Instantiate(worldSegment.Segments[segmentIndex].Prefab);

            return segment;
        }
    }
}