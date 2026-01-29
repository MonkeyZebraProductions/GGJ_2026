using System.Collections;
using UnityEngine;
using TMPro;

public class KeyPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI keyPopupText;

    private KeyCode expectedKey;
    private float timeLimit;
    private KeyGameController controller;

    private bool finished;

    public void Init(KeyCode key, string label, float lifetime, KeyGameController owner)
    {
        expectedKey = key;
        timeLimit = lifetime;
        controller = owner;

        if (keyPopupText == null)
            keyPopupText = GetComponent<TextMeshProUGUI>();

        if (keyPopupText != null)
            keyPopupText.text = label;
        else
            Debug.LogError("KeyPopup: додай TextMeshProUGUI на цей об’єкт!");

        StartCoroutine(TimeoutRoutine());
    }

    private void Update()
    {
        if (finished) return;

        // right key
        if (Input.GetKeyDown(expectedKey))
        {
            finished = true;
            controller.OnCorrect(this);
            Destroy(gameObject);
            return;
        }

        // wrong key
        if (Input.anyKeyDown)
        {
            if (controller.WasAnyAllowedKeyPressedExcept(expectedKey))
            {
                finished = true;
                controller.OnMiss(this, KeyGameController.MissReason.WrongKey);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator TimeoutRoutine()
    {
        yield return new WaitForSeconds(timeLimit);

        if (finished) yield break;

        finished = true;
        controller.OnMiss(this, KeyGameController.MissReason.Timeout);
        Destroy(gameObject);
    }
}
