using UnityEngine;

public class SpawnKeyPopup : MonoBehaviour
{
    public GameObject SpawnPrefab;
    public float MaxXOffset = 1100f, MaxYOffset = 600f;

    [Header("Popup settings")]
    public float popupLifetime = 1.2f;

    [Header("References")]
    public KeyGameController controller;  

    private Canvas canvas;

    private string[] chars;

    void Start()
    {
        canvas = GetComponent<Canvas>();

        chars = new string[36] {
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "1","2","3","4","5","6","7","8","9","0"
        };

        // якщо не призначила в інспекторі — спробуємо знайти
        if (controller == null)
            controller = FindFirstObjectByType<KeyGameController>();

        InvokeRepeating(nameof(KeyPopupSpawn), 1f, 2f);
    }

    void KeyPopupSpawn()
    {
        if (canvas == null || SpawnPrefab == null) return;

        if (controller == null)
        {
            Debug.LogError("SpawnKeyPopup: controller is NULL. Додай KeyGameController в сцену і підв'яжи його сюди.");
            return;
        }

        GameObject instance = Instantiate(SpawnPrefab, canvas.transform);

        RectTransform rectTransform = instance.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(
                Random.Range(-MaxXOffset, MaxXOffset),
                Random.Range(-MaxYOffset, MaxYOffset)
            );
        }

        // Random key
        int index = Random.Range(0, chars.Length);
        string randomChar = chars[index];

        KeyCode keyCode = (KeyCode)System.Enum.Parse(
            typeof(KeyCode),
            (index >= 26 ? "Alpha" : "") + randomChar
        );

        var popup = instance.GetComponent<KeyPopup>();
        if (popup == null)
        {
            Debug.LogError("SpawnPrefab не має компонента KeyPopup!");
            Destroy(instance);
            return;
        }

        popup.Init(keyCode, randomChar, popupLifetime, controller);
    }
}
