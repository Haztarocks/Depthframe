using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    
    [Header("Transition Settings")]
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    
    private string currentSceneName;
    private bool isTransitioning;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadScene(string sceneName, bool saveCurrentState = true)
    {
        if (isTransitioning || string.IsNullOrEmpty(sceneName)) return;
        StartCoroutine(LoadSceneRoutine(sceneName, saveCurrentState));
    }
    
    private IEnumerator LoadSceneRoutine(string sceneName, bool saveCurrentState)
    {
        isTransitioning = true;
        
        // Save current scene state if needed
        if (saveCurrentState && SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(0); // Use default slot or implement scene-specific slots
        }
        
        // Fade out
        if (fadeCanvasGroup != null)
        {
            yield return FadeRoutine(0f, 1f);
        }
        
        // Load new scene
        var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        while (operation.progress < 0.9f)
        {
            yield return null;
        }
        
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
        }
        
        currentSceneName = sceneName;
        
        // Load scene state if exists
        if (SaveManager.Instance != null)
        {
            if (SaveManager.Instance.DoesSaveExist(0)) // Check default slot or implement scene-specific slots
            {
                SaveManager.Instance.LoadGame(0);
            }
        }
        
        // Fade in
        if (fadeCanvasGroup != null)
        {
            yield return FadeRoutine(1f, 0f);
        }
        
        isTransitioning = false;
    }
    
    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            fadeCanvasGroup.alpha = currentAlpha;
            yield return null;
        }
        
        fadeCanvasGroup.alpha = targetAlpha;
    }
    
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}