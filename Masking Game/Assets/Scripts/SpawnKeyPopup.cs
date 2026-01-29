using UnityEngine;
using UnityEngine.UI;
public class SpawnKeyPopup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject SpawnPrefab;
    public float MaxXOffset, MaxYOffset;
    private Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        InvokeRepeating(nameof(KeyPopupSpawn),1f,2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void KeyPopupSpawn()
    {
        if (canvas != null)
        {
            Instantiate(SpawnPrefab, canvas.transform);
            RectTransform rectTransform = SpawnPrefab.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(Random.Range(-MaxXOffset, MaxXOffset), Random.Range(-MaxYOffset, MaxYOffset), 0);
        }
    }
}
