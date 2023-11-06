using CrossyRoad.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace CrossyRoad.Behaviour.Menu
{
    public class QuitBehaviour : MonoBehaviour
    {
        private async void OnTriggerEnter(Collider other)
        {
            Debug.Log("we quittin bois");
            PlayerController.FakeKillPlayer?.Invoke();
            await UniTask.Delay(2600+500);
            // quit
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}