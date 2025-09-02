using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Playground"); // Change "GameScene" to your actual scene name
    }
}
