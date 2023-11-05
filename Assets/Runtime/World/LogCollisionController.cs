using System.Collections.Generic;
using System.Linq;
using CrossyRoad.Obstacles;
using UnityEngine;

namespace CrossyRoad.World
{
    public class LogCollisionController : MonoBehaviour
    {
        public static List<LogObstacle> ActiveLogs = new();
        public static bool CollidingWithLog => ActiveLogs.Any();

        private List<int> _waterIndexes = new();
        
        public void Awake()
        {
            ActiveLogs = new();
        }

        public static void RegisterLogCollision(LogObstacle log, bool started, bool supressWarnings = false)
        {
            if (started && ActiveLogs.Contains(log))
            {
                if (!supressWarnings) Debug.Log("Attempting to start log collision with already added log...");
            }
            else if (started)
            {
                ActiveLogs.Add(log);
            }

            if (!started && ActiveLogs.Contains(log))
            {
                ActiveLogs.Remove(log);
            }
            else if (!started)
            {
                if (!supressWarnings) Debug.Log("Attempting to stop log collision with missing log...");
            }
        }

        public void RegisterWaterIndex(int index)
        {
            if (!_waterIndexes.Contains(index)) _waterIndexes.Add(index);
        }

        public bool PlayerIsOnWater(int index) => _waterIndexes.Contains(index);

        public bool PlayerIsOnWaterWithoutLog(int index)
        {
            if (!_waterIndexes.Contains(index)) return false;

            return !CollidingWithLog;
        }
    }
}