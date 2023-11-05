using CrossyRoad.Player;
using UnityEngine;

namespace CrossyRoad.Obstacles
{
    public class BombObstacle : MonoBehaviour
    {
        public bool Disabled = false;
        
        // drop speed would be nice but it is currently not implemented for bombs
        // [SerializeField]
        // public float Speed = 1f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (Disabled) return; // already dead probably
            Debug.Log("Trigger..");
            Disabled = true;
            PlayerController.KillPlayer?.Invoke();
            // fcuking died
        }        

    }
}