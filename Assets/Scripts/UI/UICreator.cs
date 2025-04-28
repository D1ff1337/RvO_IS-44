using UnityEngine;
using UnityEngine.UI;

public class UICreator : MonoBehaviour
{
    private Canvas canvas;
    private Button addButton;
    private Button showButton;
    private Button deleteButton;

    private void Start()
    {
        CreateCanvas();
        CreateButton("Add Upgrade", new Vector2(0, 100), AddUpgrade);
        CreateButton("Show All Upgrades", new Vector2(0, 0), ShowUpgrades);
        CreateButton("Delete First Upgrade", new Vector2(0, -100), DeleteFirstUpgrade);
    }

    private void CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Щоб UI працював
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void CreateButton(string text, Vector2 position, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject buttonGO = new GameObject(text + " Button");
        buttonGO.transform.SetParent(canvas.transform);

        Button button = buttonGO.AddComponent<Button>();
        Image image = buttonGO.AddComponent<Image>();
        image.color = Color.white; // Колір кнопки

        RectTransform rt = buttonGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 50);
        rt.anchoredPosition = position;
        rt.localScale = Vector3.one;

        // Створюємо текст на кнопці
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform);
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.black;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.localScale = Vector3.one;

        button.onClick.AddListener(onClickAction);

        // Зберігаємо посилання якщо треба
        if (text.Contains("Add"))
            addButton = button;
        else if (text.Contains("Show"))
            showButton = button;
        else if (text.Contains("Delete"))
            deleteButton = button;
    }

    private void AddUpgrade()
    {
        Debug.Log("➕ Натиснув Add Upgrade");
       
    }

    private void ShowUpgrades()
    {
        Debug.Log("📋 Натиснув Show All Upgrades");
       
    }

    private void DeleteFirstUpgrade()
    {
        Debug.Log("❌ Натиснув Delete First Upgrade");
        
    }
}
