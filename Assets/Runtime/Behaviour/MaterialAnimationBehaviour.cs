using UnityEngine;

namespace CrossyRoad.Behaviour
{
    public class MaterialAnimationBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Material _material = null!;

        [SerializeField]
        private float _speed = 1f;
        private void Update()
        {
            _material.mainTextureOffset = new Vector2(0, Time.realtimeSinceStartup * _speed);
        }
    }
}