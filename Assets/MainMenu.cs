using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame"); // Replace with your game scene name
        Debug.Log("X!");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!"); // This line is for testing in the editor
    }
}
