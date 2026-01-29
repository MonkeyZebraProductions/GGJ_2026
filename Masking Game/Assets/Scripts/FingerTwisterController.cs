using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
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
    
    [Header("Penalty")]
    [SerializeField] private int mistakes = 0;
    [SerializeField] private float penaltyCooldown = 0.6f;

    [Header("SFX/VFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip missSfx;
    [SerializeField] private ParticleSystem missVfx;

    [Header("Yarn (optional)")]
    [SerializeField] private VariableStorageBehaviour yarnVars;
    [SerializeField] private string yarnMistakesVar = "$twister_mistakes";
    [SerializeField] private string yarnStageVar = "$twister_stage";

    [Header("Twist (when max keys reached)")]
    [SerializeField] private bool twistAtMax = true;
    [SerializeField] private float twistInterval = 1.0f; // раз на скільки секунд міняється ОДНА клавіша

    private bool twisting;
    private float twistTimer;
    private int twistIndex;

private Dictionary<KeyCode, KeyCode[]> neighbors;

private void BuildNeighbors()
{
    neighbors = new Dictionary<KeyCode, KeyCode[]>
    {
        // ряд QWER
        { KeyCode.Q, new[]{ KeyCode.W, KeyCode.A } },
        { KeyCode.W, new[]{ KeyCode.Q, KeyCode.E, KeyCode.A, KeyCode.S } },
        { KeyCode.E, new[]{ KeyCode.W, KeyCode.R, KeyCode.S, KeyCode.D } },
        { KeyCode.R, new[]{ KeyCode.E, KeyCode.F, KeyCode.D } },

        // ряд ASDF
        { KeyCode.A, new[]{ KeyCode.Q, KeyCode.W, KeyCode.S, KeyCode.Z } },
        { KeyCode.S, new[]{ KeyCode.A, KeyCode.W, KeyCode.E, KeyCode.D, KeyCode.Z, KeyCode.X } },
        { KeyCode.D, new[]{ KeyCode.S, KeyCode.E, KeyCode.R, KeyCode.F, KeyCode.X, KeyCode.C } },
        { KeyCode.F, new[]{ KeyCode.D, KeyCode.R, KeyCode.C, KeyCode.V } },

        // ряд ZXCV
        { KeyCode.Z, new[]{ KeyCode.A, KeyCode.S, KeyCode.X } },
        { KeyCode.X, new[]{ KeyCode.Z, KeyCode.S, KeyCode.D, KeyCode.C } },
        { KeyCode.C, new[]{ KeyCode.X, KeyCode.D, KeyCode.F, KeyCode.V } },
        { KeyCode.V, new[]{ KeyCode.C, KeyCode.F } },
    };
}

private KeyCode PickNeighbor(KeyCode fromKey)
{
    if (neighbors == null) BuildNeighbors();

    if (neighbors.TryGetValue(fromKey, out var list))
    {
        // беремо сусіда, якого ще НЕМає серед required
        // (інакше можуть бути дублікати)
        var candidates = new List<KeyCode>();
        foreach (var k in list)
            if (!required.Contains(k))
                candidates.Add(k);

        if (candidates.Count > 0)
            return candidates[Random.Range(0, candidates.Count)];
    }

    // fallback: будь-яка з пулу, якої ще нема
    for (int i = 0; i < 50; i++)
    {
        var k = pool[Random.Range(0, pool.Length)];
        if (!required.Contains(k))
            return k;
    }

    // якщо зовсім без варіантів (малоймовірно)
    return fromKey;
}

private void TwistOneKey()
{
    if (required.Count == 0) return;

    int i = twistIndex % required.Count;
    var oldKey = required[i];
    var newKey = PickNeighbor(oldKey);

    required[i] = newKey;
    twistIndex = (i + 1) % required.Count;

    graceTimer = stageGraceTime; // маленька поблажка після зміни
    UpdateUI();
    SyncYarn();
}

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
    private float lastPenaltyTime;

private void Awake() => Debug.Log("FingerTwisterController: Awake");
private void Start() => Debug.Log("FingerTwisterController: Start");

    public void StartTwister(int start = 1, int max = 4)
    {
        Debug.Log("FingerTwisterController: StartTwister called");
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
        twisting = false;

        twistTimer = 0f;
        twistIndex = 0;
        BuildNeighbors();
    }

    public void StopTwister()
    {
        active = false;
        required.Clear();
        UpdateUI();
        SyncYarn();
    }

    public void SetMaxCount(int max) => maxCount = Mathf.Clamp(max, 1, 4);

    private void Update()
    {
        if (!active) return;

        graceTimer -= Time.deltaTime;

        bool allHeld = AreAllRequiredHeld();

            if (allHeld)
{
    confirmTimer += Time.deltaTime;

    // 1) ще не дійшли до maxCount -> додаємо як раніше
    if (required.Count < maxCount)
    {
        if (confirmTimer >= holdConfirmTime)
        {
            confirmTimer = 0f;
            AddRandomKey();
            graceTimer = stageGraceTime;
            UpdateUI();
            SyncYarn();
        }
    }
    // 2) вже maxCount (4) -> вмикаємо "twist"
    else if (twistAtMax)
    {
        twisting = true;
        twistTimer += Time.deltaTime;

        if (twistTimer >= twistInterval)
        {
            twistTimer = 0f;
            TwistOneKey(); // міняємо ОДНУ клавішу по черзі
        }
    }
}
else
{
    confirmTimer = 0f;

    // якщо гравець зірвав утримання - твіст не зупиняємо назавжди,
    // але таймер можна скидати, щоб не було "стрибка"
    if (twisting)
        twistTimer = 0f;

    if (graceTimer <= 0f && Time.time - lastPenaltyTime >= penaltyCooldown)
    {
        lastPenaltyTime = Time.time;
        mistakes += 1;
        SyncYarn();

        if (missSfx && audioSource) audioSource.PlayOneShot(missSfx);
        if (missVfx) missVfx.Play();
    }
}

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
        Debug.Log(sb.ToString());
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
