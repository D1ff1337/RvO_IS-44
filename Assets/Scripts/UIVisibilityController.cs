using UnityEngine;

public class UIVisibilityController : MonoBehaviour
{
    public CanvasGroup uiGroup;  // <<< Ось це поле має бути ПУБЛІЧНИМ

    public KeyCode toggleKey = KeyCode.Tab;
    public float fadeSpeed = 5f;

    private bool isVisible = false;

    private void Start()
    {
        if (uiGroup == null)
        {
            Debug.LogError("❌ CanvasGroup не призначено!");
        }

        HideInstantly(); // Почати схованим
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
        }

        float targetAlpha = isVisible ? 1f : 0f;
        uiGroup.alpha = Mathf.MoveTowards(uiGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        uiGroup.interactable = (uiGroup.alpha > 0.9f);
        uiGroup.blocksRaycasts = (uiGroup.alpha > 0.9f);
    }

    private void HideInstantly()
    {
        uiGroup.alpha = 0f;
        uiGroup.interactable = false;
        uiGroup.blocksRaycasts = false;
    }
}
