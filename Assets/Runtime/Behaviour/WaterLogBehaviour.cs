using System;
using CrossyRoad.Util;
using CrossyRoad.World;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrossyRoad.Behaviour
{
    public class WaterLogBehaviour : MonoBehaviour
    {
        public bool DirectionInverted = false;

        // for menu (demo purposes, etc)
        [SerializeField]
        private bool _startInstantly = false;
        
        [SerializeField]
        public bool HasShortBombLogs = false;
        
        [SerializeField]
        private float _logHeight = 0f;

        [SerializeField]
        private float _minTimeBetweenLogs = 1f;
        
        [SerializeField]
        private float _maxTimeBetweenLogs = 2f;
        
        private LogPool _logPool = null!;
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private float _offsetToExplode = 4f;
        
        public void SetPool(LogPool logPool)
        {
            _logPool = logPool;
        }
        
        public async void Start()
        {
            if (_logPool == null) return;

            var halfRoadWidth = Constants.RoadWidth / 2;
            var directionalStartZ = DirectionInverted ? transform.position.z + halfRoadWidth : transform.position.z - halfRoadWidth;
            var directionalEndZ = DirectionInverted ? transform.position.z - halfRoadWidth + _offsetToExplode : transform.position.z + halfRoadWidth - _offsetToExplode;
            
            _startPosition = new Vector3(transform.position.x, transform.position.y + _logHeight, directionalStartZ);
            _endPosition = new Vector3(transform.position.x, transform.position.y + _logHeight, directionalEndZ);
            transform.rotation = Quaternion.Euler(0, DirectionInverted ? 180f : 0f, 0);
            SpawnLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask SpawnLoop()
        {
            // random spawn delay to offset spawns and increase difficulty
            if(!_startInstantly) await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0f, 5f)));
            while (enabled)
            {
                // Debug.Log("Spawning car.");
                _logPool.SpawnLog(_startPosition, _endPosition, DirectionInverted).Forget(); //awaiting this waits for the car to despawn, which we don't need 100%
                
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(_minTimeBetweenLogs, _maxTimeBetweenLogs)));
            }
        }
    }
}