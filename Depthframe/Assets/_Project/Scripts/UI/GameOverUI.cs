using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
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
        SceneManager.Instance.LoadScene("MainMenu", false); // Don't save state when returning to menu
    }
}