using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Choose which scene to play")]
    public string sceneToPlay; // Assign this in the Inspector

    // Called by the Play button
    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(sceneToPlay))
        {
            SceneManager.LoadScene(sceneToPlay);
        }
        else
        {
            Debug.LogWarning("No scene assigned in MainMenu script!");
        }
    }

    // Called by the Quit button
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit button pressed"); // Just for editor testing
    }
}
