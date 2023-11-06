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
            await UniTask.Delay(500);
            // quit
            SceneManager.LoadScene("Game");
        }
    }
}