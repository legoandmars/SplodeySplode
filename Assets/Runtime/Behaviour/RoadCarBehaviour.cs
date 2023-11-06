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
        public bool DirectionInverted = false;
        
        public CarType CarType = CarType.None;

        [SerializeField]
        private float _carHeight = 0.784f;

        [SerializeField]
        private float _minTimeBetweenCars = 1f;
        
        [SerializeField]
        private float _maxTimeBetweenCars = 2f;

        [SerializeField]
        private Transform _frontPlaneTransform = null!;
        
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
            
            var halfRoadWidth = Constants.RoadWidth / 2;
            var directionalStartZ = DirectionInverted ? transform.position.z - halfRoadWidth : transform.position.z + halfRoadWidth;
            var directionalEndZ = DirectionInverted ? transform.position.z + halfRoadWidth : transform.position.z - halfRoadWidth;

            _startPosition = new Vector3(transform.position.x, transform.position.y + _carHeight, directionalStartZ);
            _endPosition = new Vector3(transform.position.x, transform.position.y + _carHeight, directionalEndZ);
            if (_frontPlaneTransform == null)
            {
                Debug.Log("Front plane transform not set on prefab!");
            }
            else
            {
                _frontPlaneTransform.localPosition = new Vector3(DirectionInverted ? 0.45f : -0.45f, _frontPlaneTransform.localPosition.y, 0);
                _frontPlaneTransform.localRotation = Quaternion.Euler(90, 0 , DirectionInverted ? -90f : 90f);
            }
            SpawnLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask SpawnLoop()
        {
            // random spawn delay to offset spawns and increase difficulty
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0f, 5f)));
            while (enabled)
            {
                // Debug.Log("Spawning car.");
                _carPool.SpawnCar(_startPosition, _endPosition, DirectionInverted).Forget(); //awaiting this waits for the car to despawn, which we don't need 100%
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_minTimeBetweenCars, _maxTimeBetweenCars)));
            }
        }
    }
}