using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yarn.Unity;

public class FingerTwisterController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI holdText; // "HOLD: A + S + D"
    
    [Header("Progression")]
    public int startCount = 1;
    public int maxCount = 4;
    [SerializeField] private float holdConfirmTime = 5f; 
    [SerializeField] private float pauseTime = 5f;
    [SerializeField] public UnityEvent OnGuageFull;

    [Header("Penalty")]
    [SerializeField] private int mistakes = 0;
    [SerializeField] public UnityEvent OnGaugeEmpty;

    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip missSfx;
    [SerializeField] private AudioClip addSfx;

    [Header("Gauge")]
    [SerializeField] private Slider gauge;       
    [SerializeField] private float fillPerSecond = 0.35f;
    [SerializeField] private float drainPerSecond = 0.55f;
    [SerializeField] private float startGaugeValue = 1f; 
    private float gaugeValue;
    private bool gaugeEmptyTriggered;

    private readonly KeyCode[] pool = new[]
    {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V
    };

    private readonly List<KeyCode> required = new();
    private bool active;

    private float confirmTimer;
    private float pauseTimer;

    private void Awake()
    {
        HideUI();
        active = false;
    }

    private void HideUI()
    {
        gauge.gameObject.SetActive(false);
        holdText.enabled = false;
    }

    public void StartTwister(int start = 1, int max = 4)
    {
        gauge.enabled = true;
        gaugeEmptyTriggered = false;
        gauge.gameObject.SetActive(true);
        holdText.enabled = true;

        startCount = Mathf.Clamp(start, 1, 4);
        maxCount = Mathf.Clamp(max, startCount, 4);

        required.Clear();
        mistakes = 0;
        confirmTimer = 0f;

        for (int i = 0; i < startCount; i++)
            AddRandomKey();

        active = true;
        UpdateUI();

        gaugeValue = Mathf.Clamp01(startGaugeValue);
        UpdateGaugeUI();
    }

    private void UpdateGaugeUI()
    {
        if (gauge != null)
            gauge.value = gaugeValue;
    }


    public void StopTwister()
    {
        pauseTimer = pauseTime;
        active = false;
        required.Clear();
        HideUI();

        UpdateUI();
    }

    public void SetMaxCount(int max) => maxCount = Mathf.Clamp(max, 1, 4);

    private void Update()
    {
        if (!active) 
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                StartTwister(startCount, maxCount);
            }
            return;
        }

        bool allHeld = AreAllRequiredHeld();

        if (allHeld)
        {
            gaugeValue += fillPerSecond * Time.deltaTime;

            confirmTimer += Time.deltaTime;

            if (required.Count == maxCount && gaugeValue>=1)
            {
                OnGuageFull.Invoke();
                StopTwister(); 

            }

            if (confirmTimer >= holdConfirmTime + UnityEngine.Random.Range(0, 5) && required.Count < maxCount)
            {
                confirmTimer = 0f;
                AddRandomKey();
                UpdateUI();
            }
        }
        else
        {
            gaugeValue -= drainPerSecond * Time.deltaTime;

            confirmTimer = 0f;

            if (gaugeValue <= 0f)
            {
                mistakes += 1;
                gaugeValue = 0f;          
                TriggerGaugeEmpty();      
                if (missSfx && audioSource) audioSource.PlayOneShot(missSfx);

                return;
            }
        }

        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();
    }

    private bool AreAllRequiredHeld()
    {
        for (int i = 0; i < required.Count; i++)
        {
            if (!Input.GetKey(required[i]))
                return false;
        }
        return true;
    }

    private void AddRandomKey()
    {
        int safety = 100;
        while (safety-- > 0)
        {
            var k = pool[UnityEngine.Random.Range(0, pool.Length)];
            if (!required.Contains(k))
            {
                required.Add(k);
                break;
            }
        }
        if (addSfx && audioSource) audioSource.PlayOneShot(addSfx);
    }

    private void UpdateUI()
    {
        if (holdText == null) return;

        if (!active || required.Count == 0)
        {
            holdText.text = "";
            return;
        }

        var sb = new StringBuilder();
        sb.Append("HOLD: ");

        for (int i = 0; i < required.Count; i++)
        {
            sb.Append(KeyToLabel(required[i]));
            if (i < required.Count - 1) sb.Append(" + ");
        }

        holdText.text = sb.ToString();
    }

    private string KeyToLabel(KeyCode key)
    {
        return key.ToString();
    }

    private void TriggerGaugeEmpty()
    {
        if (gaugeEmptyTriggered) return;

        gaugeEmptyTriggered = true;
        mistakes += 1;

        if (missSfx && audioSource) audioSource.PlayOneShot(missSfx);
            OnGaugeEmpty?.Invoke();

        StopTwister();
    }

    //public event Action<FingerTwisterController> OnGaugeEmpty;

}
