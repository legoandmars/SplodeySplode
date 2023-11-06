using CrossyRoad.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CrossyRoad.Behaviour.Menu
{
    public class PlayBehaviour : MonoBehaviour
    {
        private async void OnTriggerEnter(Collider other)
        {
            Debug.Log("we playin bois");
            PlayerController.FakeKillPlayer?.Invoke();
            await UniTask.Delay(2600+500);
            // quit
            SceneManager.LoadScene("Game");
        }
    }
}