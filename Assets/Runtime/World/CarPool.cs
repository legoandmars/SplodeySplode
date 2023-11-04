using AuraTween;
using CrossyRoad.Behaviour;
using CrossyRoad.Obstacles;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace CrossyRoad.World
{
    public class CarPool : MonoBehaviour
    {
        [SerializeField]
        private CarObstacle _template = null!;
        
        [SerializeField]
        private TweenManager _tweenManager = null!;
        
        IObjectPool<CarObstacle> m_Pool;
        
        
        public IObjectPool<CarObstacle> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<CarObstacle>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 10);
                }
                return m_Pool;
            }
        }
        
        CarObstacle CreatePooledItem()
        {
            var car = Instantiate(_template, this.transform);
            car.gameObject.SetActive(true);
            // whatever init stuff
            return car;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(CarObstacle car)
        {
            car.gameObject.transform.position = Vector3.zero; // idk if necessary but i dont wanna cause weird ghost collisions
            car.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(CarObstacle car)
        {
            car.transform.localPosition = Vector3.zero;
            car.transform.localRotation = Quaternion.identity;
            car.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(CarObstacle car)
        {
            Destroy(car.gameObject);
        }

        public async UniTask SpawnCar(Vector3 startPosition, Vector3 endPosition, Transform parent)
        {
            var car = Pool.Get();
            //car.transform.SetParent(parent);
            car.transform.position = startPosition;
            
            await _tweenManager.Run(startPosition, endPosition, car.Speed,
                (t) =>
                {
                    if (car != null && !car.Disabled) car.transform.position = t;
                }, Easer.Linear);
            
            /*await _tweenManager.Run(_startAlpha, 0f, _fadeDuration,
                (t) =>
                {
                    if(text != null) text.alpha = t;
                }, Easer.Linear);
*/
            //text.transform.SetParent(parent);
            // set random position.. hardcoded, whatever
            //text.transform.position = parent.position + new Vector3(Random.Range(-_posMaxRandom, _posMaxRandom), Random.Range(-_posMaxRandom, _posMaxRandom), Random.Range(-_posMaxRandom, _posMaxRandom));
            //text.SetText(inputText);
  /*          await _tweenManager.Run(_startAlpha, 0f, _fadeDuration,
                (t) =>
                {
                    if(text != null) text.alpha = t;
                }, Easer.Linear);
*/
            if (car != null) Pool.Release(car);
        }
    }
}