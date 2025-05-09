using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanityUI : MonoBehaviour
{
    [Header("UI References")]
    public Image sanityBar;
    public TextMeshProUGUI sanityText;
    public Image sanityImage; // Add this line
    public Sprite highSanitySprite; // Add this line
    public Sprite mediumSanitySprite; // Add this line
    public Sprite lowSanitySprite; // Add this line

    private void OnEnable()
    {
        SanitySystem.SanityChanged += UpdateSanityUI;
    }
    
    private void OnDisable()
    {
        SanitySystem.SanityChanged -= UpdateSanityUI;
    }
    
    private void UpdateSanityUI(float currentSanity)
    {
        if (sanityBar != null)
            sanityBar.fillAmount = currentSanity / 100f;
            
        if (sanityText != null)
            sanityText.text = $"Sanity: {Mathf.Round(currentSanity)}%";

        // Update the image based on sanity level
        if (sanityImage != null)
        {
            if (currentSanity >= 75)
            {
                sanityImage.sprite = highSanitySprite;
            }
            else if (currentSanity >= 50)
            {
                sanityImage.sprite = mediumSanitySprite;
            }
            else
            {
                sanityImage.sprite = lowSanitySprite;
            }
        }
    }
}