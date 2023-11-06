using CrossyRoad.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace CrossyRoad.Behaviour.Menu
{
    public class MenuBehaviour : MonoBehaviour
    {
        private async void OnTriggerEnter(Collider other)
        {
            Debug.Log("we menuing bois");
            PlayerController.FakeKillPlayer?.Invoke();
            await UniTask.Delay(2600+500);
            // quit
            SceneManager.LoadScene("Menu");
        }
    }
}