using AuraTween;
using UnityEngine;

namespace CrossyRoad.Behaviour
{
    public abstract class WorldSegmentTweenBehaviour : MonoBehaviour
    {
        internal TweenManager _tweenManager = null!;
        
        public void SetTweenManager(TweenManager tweenManager)
        {
            _tweenManager = tweenManager;
            Debug.Log("Setting tween manager..");
        }
    }
}