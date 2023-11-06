using System;
using CrossyRoad.Data;
using CrossyRoad.Util;
using CrossyRoad.World;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrossyRoad.Behaviour
{
    public class RoadCarBehaviour : MonoBehaviour
    {
        public CarType CarType = CarType.None;

        [SerializeField]
        private float _carHeight = 0.784f;

        [SerializeField]
        private float _minTimeBetweenCars = 1f;
        
        [SerializeField]
        private float _maxTimeBetweenCars = 2f;
        
        private CarPool _carPool = null!;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        
        public void SetPool(CarPool carPool)
        {
            _carPool = carPool;
        }
        
        public async void Start()
        {
            if (_carPool == null) return;
            
            _startPosition = new Vector3(transform.position.x, transform.position.y + _carHeight, transform.position.z + Constants.RoadWidth / 2);
            _endPosition = new Vector3(transform.position.x, transform.position.y + _carHeight, transform.position.z - Constants.RoadWidth / 2);
            
            SpawnLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask SpawnLoop()
        {
            // random spawn delay to offset spawns and increase difficulty
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0f, 5f)));
            while (enabled)
            {
                // Debug.Log("Spawning car.");
                _carPool.SpawnCar(_startPosition, _endPosition, transform).Forget(); //awaiting this waits for the car to despawn, which we don't need 100%
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_minTimeBetweenCars, _maxTimeBetweenCars)));
            }
        }
    }
}