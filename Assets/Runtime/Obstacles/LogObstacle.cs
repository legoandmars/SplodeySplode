using CrossyRoad.World;
using UnityEngine;

namespace CrossyRoad.Obstacles
{
    public class LogObstacle : MonoBehaviour
    {
        public bool Disabled = false;
        public bool Exploded = false;

        [SerializeField]
        public float Speed = 1f;

        [SerializeField]
        private Collider _collider;

        [SerializeField]
        public Animator Animator = null!;
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Loggin..");
            LogCollisionController.RegisterLogCollision(this, true);
        }
        private void OnTriggerExit(Collider other)
        {
            LogCollisionController.RegisterLogCollision(this, false);
            Debug.Log("Unloggin..");
        }

        private void OnDisable()
        {
            // attempt to make sure it won't fuck up collision upon despawning
            LogCollisionController.RegisterLogCollision(this, false, true);
        }

        public void SetExplodedState(bool exploded)
        {
            Exploded = exploded;
            _collider.enabled = !exploded;
        }
    }
}