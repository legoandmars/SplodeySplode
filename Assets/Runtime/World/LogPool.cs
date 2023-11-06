using System;
using AuraTween;
using CrossyRoad.Behaviour;
using CrossyRoad.Obstacles;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace CrossyRoad.World
{
    public class LogPool : MonoBehaviour
    {
        [SerializeField]
        private LogObstacle _template = null!;
        
        [SerializeField]
        private TweenManager _tweenManager = null!;

        private int _explodeTriggerId = Animator.StringToHash("Explode");

        IObjectPool<LogObstacle> m_Pool;

        public IObjectPool<LogObstacle> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<LogObstacle>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 10);
                }
                return m_Pool;
            }
        }
        
        LogObstacle CreatePooledItem()
        {
            var log = Instantiate(_template, this.transform);
            log.gameObject.SetActive(true);
            // whatever init stuff
            return log;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(LogObstacle log)
        {
            log.gameObject.transform.position = Vector3.zero; // idk if necessary but i dont wanna cause weird ghost collisions
            log.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(LogObstacle log)
        {
            log.transform.localPosition = Vector3.zero;
            log.transform.localRotation = Quaternion.identity;
            log.SetExplodedState(false);
            log.ExplosionImminent = false;
            log.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(LogObstacle log)
        {
            Destroy(log.gameObject);
        }

        public async UniTask SpawnLog(Vector3 startPosition, Vector3 endPosition, bool inverted)
        {
            var log = Pool.Get();
            //car.transform.SetParent(parent);
            log.transform.position = startPosition;
            log.transform.rotation = Quaternion.Euler(0, inverted ? 180f : 0f, 0);
            // ExplosionImminent
            await _tweenManager.Run(0, 1, log.Speed, (t) =>
            {
                var timeRemaining = log.Speed - (t * log.Speed);
                if (timeRemaining < 1.5f)
                {
                    log.ExplosionImminent = true;
                }
                log.transform.position = new Vector3(log.transform.position.x, log.transform.position.y, Mathf.Lerp(startPosition.z, endPosition.z, t));
            }, Easer.Linear);

            if (log == null) return;
            log.Animator.SetTrigger(_explodeTriggerId);
            log.SetExplodedState(true);
            await UniTask.Delay(1000);
            // TODO Explode somewhere here
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
            if (log != null && !log.Disabled) Pool.Release(log);
        }
    }
}