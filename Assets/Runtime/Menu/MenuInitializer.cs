using CrossyRoad.Behaviour;
using CrossyRoad.World;
using UnityEngine;

namespace CrossyRoad.Menu
{
    public class MenuInitializer : MonoBehaviour
    {
        [SerializeField]
        private LogPool _logPool;
        
        [SerializeField]
        private LogPool _shortLogPool;
        
        [SerializeField]
        private WaterLogBehaviour _logBehaviour;

        [SerializeField]
        private WaterLogBehaviour _shortLogBehaviour;

        private void Awake()
        {
            _logBehaviour.SetPool(_logPool);
            _shortLogBehaviour.SetPool(_shortLogPool);
        }
    }
}