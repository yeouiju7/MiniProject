using UnityEngine;
using UnityEngine.SceneManagement; 


public class SceneSwitcher : MonoBehaviour
{
    public string startSceneName = "StartScene";

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void GoToStartScene()
    {
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}