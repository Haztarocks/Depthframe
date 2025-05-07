using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private CanvasGroup canvasGroup; // Add reference to canvas group

    private void Start()
    {
        // Set up button listeners
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OpenCredits);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        // Verify SceneManager exists
        if (SceneManager.Instance == null)
        {
            Debug.LogError("SceneManager instance not found! Make sure there's a SceneManager in your scene.");
        }

        // Initialize canvas group if not assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (startButton != null) startButton.onClick.RemoveListener(StartGame);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OpenSettings);
        if (creditsButton != null) creditsButton.onClick.RemoveListener(OpenCredits);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
    }

    private void StartGame()
    {
        if (SceneManager.Instance == null)
        {
            Debug.LogError("Cannot start game: SceneManager instance is missing!");
            return;
        }

        // Disable all buttons to prevent multiple clicks during transition
        DisableAllButtons();
        
        // Start scene transition
        SceneManager.Instance.LoadScene("Intro", false);
    }

    private void DisableAllButtons()
    {
        if (startButton != null) startButton.interactable = false;
        if (settingsButton != null) settingsButton.interactable = false;
        if (creditsButton != null) creditsButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
    }

    private void OpenSettings()
    {
        // Implement settings menu functionality
        Debug.Log("Settings button clicked");
    }

    private void OpenCredits()
    {
        // Implement credits functionality
        Debug.Log("Credits button clicked");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}