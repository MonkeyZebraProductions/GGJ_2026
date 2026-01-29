using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class KeyGameController : MonoBehaviour
{
    public enum MissReason { WrongKey, Timeout }

    [Header("Spawn")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float spawnEverySeconds = 2f;
    [SerializeField] private float popupLifetime = 1.2f;
    [SerializeField] private float maxXOffset = 600f;
    [SerializeField] private float maxYOffset = 300f;

    [Header("Allowed keys")]
    [SerializeField] private bool useLetters = true;
    [SerializeField] private bool useDigits = true;

    [Header("Penalty")]
    [SerializeField] private int mistakes = 0;

    [Header("SFX/VFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip missSfx;
    [SerializeField] private AudioClip correctSfx;
    //[SerializeField] private ParticleSystem missVfx;
    //[SerializeField] private ParticleSystem correctVfx;

    [Header("Yarn (optional)")]
    [SerializeField] private VariableStorageBehaviour yarnVars;
    [SerializeField] private string yarnMistakesVarName = "$mistakes";

    private readonly List<KeyCode> allowed = new();
    private bool running;

    private void Awake()
    {
        BuildAllowedKeys();
    }

    private void Start()
    {
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
    }

    public void StartGame()
    {
        if (running) return;
        running = true;
        InvokeRepeating(nameof(SpawnPopup), 0.5f, spawnEverySeconds);
        SyncYarn();
    }

    public void StopGame()
    {
        if (!running) return;
        running = false;
        CancelInvoke(nameof(SpawnPopup));
    }

    private void BuildAllowedKeys()
    {
        allowed.Clear();

        if (useLetters)
        {
            for (char c = 'A'; c <= 'Z'; c++)
                allowed.Add((KeyCode)System.Enum.Parse(typeof(KeyCode), c.ToString()));
        }

        if (useDigits)
        {
            for (int i = 0; i <= 9; i++)
                allowed.Add((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i));
        }
    }

    private void SpawnPopup()
    {
        if (!running || popupPrefab == null || canvas == null || allowed.Count == 0) return;

        var go = Instantiate(popupPrefab, canvas.transform);
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(
                Random.Range(-maxXOffset, maxXOffset),
                Random.Range(-maxYOffset, maxYOffset)
            );
        }

        // choose random key
        int index = Random.Range(0, allowed.Count);
        KeyCode key = allowed[index];

        string label = KeyCodeToLabel(key);

        // init Keypopup
        var popup = go.GetComponent<KeyPopup>();
        if (popup == null)
        {
            Debug.LogError("popupPrefab має містити компонент KeyPopup!");
            Destroy(go);
            return;
        }

        popup.Init(key, label, popupLifetime, this);
    }

    private string KeyCodeToLabel(KeyCode key)
    {
        string s = key.ToString();
        if (s.StartsWith("Alpha")) return s.Replace("Alpha", "");
        return s;
    }

    public bool WasAnyAllowedKeyPressedExcept(KeyCode expected)
    {
        for (int i = 0; i < allowed.Count; i++)
        {
            var k = allowed[i];
            if (k == expected) continue;
            if (Input.GetKeyDown(k)) return true;
        }
        return false;
    }

    public void OnCorrect(KeyPopup popup)
    {
        if (correctSfx && audioSource) audioSource.PlayOneShot(correctSfx);
        //if (correctVfx) correctVfx.Play();
    }

    public void OnMiss(KeyPopup popup, MissReason reason)
    {
        mistakes += 1;
        SyncYarn();

        if (missSfx && audioSource) audioSource.PlayOneShot(missSfx);
        //if (missVfx) missVfx.Play();
    }

    private void SyncYarn()
    {
        if (yarnVars == null) return;
        yarnVars.SetValue(yarnMistakesVarName, (float)mistakes);
    }
}
