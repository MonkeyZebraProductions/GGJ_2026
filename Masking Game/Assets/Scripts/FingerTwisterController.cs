using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class FingerTwisterController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI holdText; // "HOLD: A + S + D"
    
    [Header("Progression")]
    [SerializeField] private int startCount = 1;
    [SerializeField] private int maxCount = 4;
    [SerializeField] private float holdConfirmTime = 0.25f; 
    [SerializeField] private float stageGraceTime = 0.35f; 
    [SerializeField] private float pauseTime = 5f; 
    [Header("Penalty")]
    [SerializeField] private int mistakes = 0;
    [SerializeField] private float penaltyCooldown = 0.6f;

    [Header("SFX/VFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip missSfx;
    [SerializeField] private ParticleSystem missVfx;

    [Header("Yarn")]
    [SerializeField] private VariableStorageBehaviour yarnVars;
    [SerializeField] private string yarnMistakesVar = "$twister_mistakes";
    [SerializeField] private string yarnStageVar = "$twister_stage";

    [Header("Gauge")]
    [SerializeField] private Slider gauge;       
    [SerializeField] private float fillPerSecond = 0.35f;
    [SerializeField] private float drainPerSecond = 0.55f;
    [SerializeField] private float startGaugeValue = 1f; 
    private float gaugeValue;

    private readonly KeyCode[] pool = new[]
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V
    };

    private readonly List<KeyCode> required = new();
    private bool active;

    private float confirmTimer;
    private float graceTimer;

    private float pauseTimer;
    private float lastPenaltyTime;

    public void StartTwister(int start = 1, int max = 4)
    {
        gauge.enabled = true;
        gauge.gameObject.SetActive(true);
        holdText.enabled = true;

        startCount = Mathf.Clamp(start, 1, 4);
        maxCount = Mathf.Clamp(max, startCount, 4);

        required.Clear();
        mistakes = 0;
        confirmTimer = 0f;
        graceTimer = stageGraceTime;
        lastPenaltyTime = -999f;

        for (int i = 0; i < startCount; i++)
            AddRandomKey();

        active = true;
        UpdateUI();
        SyncYarn();

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
        gauge.gameObject.SetActive(false);
        holdText.enabled = false;

        UpdateUI();
        SyncYarn();
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

        graceTimer -= Time.deltaTime;

        bool allHeld = AreAllRequiredHeld();

        if (allHeld)
        {
            gaugeValue += fillPerSecond * Time.deltaTime;

            confirmTimer += Time.deltaTime;

            if (required.Count == maxCount)
                StopTwister(); 

            if (confirmTimer >= holdConfirmTime && required.Count < maxCount)
            {
                confirmTimer = 0f;
                AddRandomKey();
                graceTimer = stageGraceTime;
                UpdateUI();
                SyncYarn();
            }
        }
        else
        {
            gaugeValue -= drainPerSecond * Time.deltaTime;

            confirmTimer = 0f;

            if (graceTimer <= 0f && Time.time - lastPenaltyTime >= penaltyCooldown)
            {
                lastPenaltyTime = Time.time;
                mistakes += 1;
                SyncYarn();

                if (missSfx && audioSource) audioSource.PlayOneShot(missSfx);
                if (missVfx) missVfx.Play();
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
            var k = pool[Random.Range(0, pool.Length)];
            if (!required.Contains(k))
            {
                required.Add(k);
                break;
            }
        }
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

    private void SyncYarn()
    {
        if (yarnVars == null) return;

        yarnVars.SetValue(yarnMistakesVar, (float)mistakes);
        yarnVars.SetValue(yarnStageVar, (float)required.Count);
    }
}
