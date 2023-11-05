using System;
using System.Collections.Generic;
using CrossyRoad.Util;
using CrossyRoad.World;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrossyRoad.Behaviour
{
    public class BombGrassBehaviour : MonoBehaviour
    {

        [SerializeField]
        private float _minTimeBetweenBombs = 2.5f;
        
        [SerializeField]
        private float _maxTimeBetweenBombs = 3f;
        
        private BombPool _bombPool = null!;
        private List<int> _takenIndexes = new(); // should work w/ obstacles

        private int maxSpawnDistance = 3;
        private int x;
        
        public void SetPool(BombPool bombPool)
        {
            _bombPool = bombPool;
        }
        
        public async void Start()
        {
            if (_bombPool == null) return;
            
            x = (int)transform.position.x;

            SpawnLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask SpawnLoop()
        {
            // random spawn delay to offset spawns and increase difficulty
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0f, 5f)));
            while (enabled)
            {
                // Debug.Log("Spawning bomb.");
                SpawnBomb().Forget();
                // _carPool.SpawnCar(_startPosition, _endPosition, transform).Forget(); //awaiting this waits for the car to despawn, which we don't need 100%
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_minTimeBetweenBombs, _maxTimeBetweenBombs)));
            }
        }

        private int? RandomIndex(int tries = 0)
        {
            if (tries > 20) return null;
            var randomIndex = Random.Range(-maxSpawnDistance, maxSpawnDistance + 1);
            if (_takenIndexes.Contains(randomIndex) || ObstacleCoordinatesController.ExistingCoordinates.Contains((x, randomIndex))) return RandomIndex(tries + 1);
            return randomIndex;
        }
        
        private async UniTask SpawnBomb()
        {
            var zPos = RandomIndex();
            if (zPos == null) return;
            await _bombPool.SpawnBomb(new Vector3(transform.position.x, 0, zPos.Value));
        }
    }
}