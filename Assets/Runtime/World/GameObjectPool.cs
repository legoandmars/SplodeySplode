using System;
using AuraTween;
using CrossyRoad.Behaviour;
using CrossyRoad.Obstacles;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace CrossyRoad.World
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject _template = null!;
        
        IObjectPool<GameObject> m_Pool;
        
        public IObjectPool<GameObject> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 10);
                }
                return m_Pool;
            }
        }
        
        GameObject CreatePooledItem()
        {
            var bomb = Instantiate(_template, this.transform);
            bomb.gameObject.SetActive(true);
            // whatever init stuff
            return bomb;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(GameObject pooledObject)
        {
            pooledObject.gameObject.transform.position = Vector3.zero; // idk if necessary but i dont wanna cause weird ghost collisions
            pooledObject.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(GameObject pooledObject)
        {
            pooledObject.transform.localPosition = Vector3.zero;
            pooledObject.transform.localRotation = Quaternion.identity;
            pooledObject.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(GameObject pooledObject)
        {
            Destroy(pooledObject.gameObject);
        }

        public void ReleaseWhileRemovingCoordinates(GameObject pooledObject)
        {
            var coords = ((int)pooledObject.transform.position.x, (int)pooledObject.transform.position.z);
            if (ObstacleCoordinatesController.ExistingCoordinates.Contains(coords))
            {
                ObstacleCoordinatesController.ExistingCoordinates.Remove(coords);
            }
            else
            {
                Debug.Log($"FAILED TO REMOVE OBJECT AT COORDINATES!!!! {coords}");
            }
        }
    }
}