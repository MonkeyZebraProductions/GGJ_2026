using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Mask : MonoBehaviour
{
    private Image maskImage;

    [Header("MaskHealth")]
    [SerializeField]
    private int maxMaskHealth = 100;
    private int currentMaskHealth;
    private int maskHealthCheckpointValue;
    [SerializeField]
    private Sprite[] maskSprites;
    private int maskIndex;

    [SerializeField]
    AudioSource maskAudioSource;

    [Header("Yarn (optional)")]
    [SerializeField] private VariableStorageBehaviour yarnVars;
    [SerializeField] private string yarnMaskVarName = "$maskHealth";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maskImage = GetComponent<Image>();
        currentMaskHealth = maxMaskHealth;
        maskHealthCheckpointValue = currentMaskHealth/maskSprites.Length+1;
        maskAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoseMaskHealth(int maskHealthLoss)
    {
        currentMaskHealth -= maskHealthLoss;
        if (yarnVars != null) 
        {
            yarnVars.SetValue(yarnMaskVarName, currentMaskHealth);
        }
        if(currentMaskHealth<=0)
        {
            Debug.Log("GameEnd");
            return;
        }
        if (currentMaskHealth<=maskHealthCheckpointValue)
        {
            if (maskAudioSource != null && !maskAudioSource.isPlaying)
            {
                maskAudioSource.Play();
            }
        }
        if (maskSprites != null && maskImage != null)
        {
            if(currentMaskHealth<=maxMaskHealth-maskHealthCheckpointValue*maskIndex)
            {
                maskImage.sprite = maskSprites[maskIndex];
                maskIndex++;
            }
        }
    }
}
