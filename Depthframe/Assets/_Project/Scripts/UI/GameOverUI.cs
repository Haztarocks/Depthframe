using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        // Disable any automatic loading of saved games
        if (SaveManager.Instance != null)
        {
            // Prevent auto-loading
            Debug.Log("Game Over screen - preventing auto-load");
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        else
        {
            Debug.LogError("Main Menu button reference is missing in GameOverUI!");
        }
    }

    private void OnDestroy()
    {
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
        }
    }

    private void ReturnToMainMenu()
    {
        if (SceneManager.Instance != null)
        {
            // Use the exact scene name as it appears in the build settings
            SceneManager.Instance.LoadScene("Main Menu", false);
        }
        else
        {
            Debug.LogWarning("SceneManager instance is null, using Unity's SceneManager instead.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    }
}