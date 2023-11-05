using AuraTween;
using CrossyRoad.Behaviour;
using CrossyRoad.Obstacles;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace CrossyRoad.World
{
    public class BombPool : MonoBehaviour
    {
        [SerializeField]
        private BombObstacle _template = null!;
        
        IObjectPool<BombObstacle> m_Pool;
        
        public IObjectPool<BombObstacle> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<BombObstacle>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 10);
                }
                return m_Pool;
            }
        }
        
        BombObstacle CreatePooledItem()
        {
            var bomb = Instantiate(_template, this.transform);
            bomb.gameObject.SetActive(true);
            // whatever init stuff
            return bomb;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(BombObstacle bomb)
        {
            bomb.gameObject.transform.position = Vector3.zero; // idk if necessary but i dont wanna cause weird ghost collisions
            bomb.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(BombObstacle bomb)
        {
            bomb.transform.localPosition = Vector3.zero;
            bomb.transform.localRotation = Quaternion.identity;
            bomb.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(BombObstacle bomb)
        {
            Destroy(bomb.gameObject);
        }

        public async UniTask SpawnBomb(Vector3 startPosition)
        {
            var bomb = Pool.Get();
            //car.transform.SetParent(parent);
            bomb.transform.position = startPosition;
            
            /*await _tweenManager.Run(startPosition, endPosition, car.Speed,
                (t) =>
                {
                    if (car != null && !car.Disabled) car.transform.position = t;
                }, Easer.Linear);*/
            await UniTask.Delay(3500); // wait until right after the bomb-ish
            
            
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
            if (bomb != null && !bomb.Disabled) Pool.Release(bomb);
        }
    }
}