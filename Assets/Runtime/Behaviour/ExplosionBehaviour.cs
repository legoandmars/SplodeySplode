using UnityEngine;

namespace CrossyRoad.Behaviour
{
    // literally only to make the crater look better
    public class ExplosionBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform _craterTransform = null!;
        
        public void OnEnable()
        {
            _craterTransform.rotation = Quaternion.Euler(0,
                Random.Range(0f, 360f), 0);
        }
    }
}