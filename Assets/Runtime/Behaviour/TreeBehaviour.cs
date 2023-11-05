using System;
using System.Collections.Generic;
using CrossyRoad.World;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrossyRoad.Behaviour
{
    public class TreeBehaviour : MonoBehaviour
    {
        [SerializeField]
        private int _minTreesSpawned = 0;

        [SerializeField]
        private int _maxTreesSpawned = 3;

        private GameObjectPool _treePool = null!;
        private int x;
        private Vector3 _endPosition;
        private List<GameObject> SpawnedTrees = new();        
        private int maxSpawnDistance = 5;

        public void SetPool(GameObjectPool treePool)
        {
            _treePool = treePool;
        }

        public void Start()
        {
            if (_treePool == null) return;

            x = (int)transform.position.x;
            
            // spawn trees
            var amount = Random.Range(_minTreesSpawned, _maxTreesSpawned + 1);

            for (int i = 0; i < amount; i++)
            {
                SpawnTree();
            }
        }

        private (int, int)? RandomSpawnCoordinates(int tries = 0)
        {
            if (tries > 20) return null;
            var randomIndex = Random.Range(-maxSpawnDistance, maxSpawnDistance + 1);
            var coords = (x, randomIndex);
            if (ObstacleCoordinatesController.ExistingCoordinates.Contains(coords) || ObstacleCoordinatesController.DontSpawnCoordinates.Contains(coords)) return RandomSpawnCoordinates(tries + 1);
            return (x, randomIndex);
        }
        
        private void SpawnTree()
        {
            var pos = RandomSpawnCoordinates();
            if (pos == null) return;
            var tree = _treePool.Pool.Get();
            SpawnedTrees.Add(tree);
            ObstacleCoordinatesController.ExistingCoordinates.Add(pos.Value);
            tree.transform.position = new Vector3(pos.Value.Item1, 1, pos.Value.Item2);
            Debug.Log($"SPAWNING AT: {pos}, {tree.transform.position}");
        }

        private void OnDestroy()
        {
            foreach (var tree in SpawnedTrees)
            {
                if (tree != null) _treePool.ReleaseWhileRemovingCoordinates(tree);
            }
        }
    }
}