using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AuraTween;
using CrossyRoad.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CrossyRoad.Obstacles
{
    public class CarObstacle : MonoBehaviour
    {
        public bool Disabled = false;
        
        [SerializeField]
        public float Speed = 1f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (Disabled) return; // already dead probably
            Debug.Log("Trigger..");
            Disabled = true;
            PlayerController.KillPlayer?.Invoke();
            // fcuking died
        }        
        
        // Start is called before the first frame update
        async void Start()
        {
            /*_tween = _tweenManager.Run(_startPosition, _endPosition, _speed, value =>
            {
                //guh
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);
            }, Easer.Linear, this);*/
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
