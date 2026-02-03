using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Mask : MonoBehaviour
{
    private Image maskImage;
    private bool _gameEnded;

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

    [SerializeField]
    private ManualNodeMovement NodeMovement;

    [Header("UI Image")]
    [SerializeField]
    private Image uiImageToActivate;

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

    private void ActivateUIImage()
    {
        if (uiImageToActivate != null)
        {
            uiImageToActivate.gameObject.SetActive(true);
            Debug.Log("UI Image Activated");
        }
    }

    [YarnCommand("hide_Mask")]
    public void HideMask()
    {
        if (maskImage != null) { maskImage.enabled = false; }
    }

    public void LoseMaskHealth(int maskHealthLoss)
    {
        if (_gameEnded)
        { return; }

        currentMaskHealth -= maskHealthLoss;
        if (yarnVars != null) 
        {
            yarnVars.SetValue(yarnMaskVarName, currentMaskHealth);
        }
        //When Health is empty
        if(currentMaskHealth<=0)
        {
            Debug.Log("GameEnd");
            if(NodeMovement != null)
            {
                NodeMovement.ManualAdvence();
            }
            _gameEnded = true;
        }
        //When Health is low
        if (currentMaskHealth<=maskHealthCheckpointValue || currentMaskHealth <= maskHealthLoss)
        {
            if (maskAudioSource != null && !maskAudioSource.isPlaying)
            {
                maskAudioSource.Play();
                //maskImage.color = new Color(1f, 0.5f, 0.5f);
                ActivateUIImage();
            }
        }


        if (maskSprites != null && maskImage != null)
        {
            if(currentMaskHealth<=maxMaskHealth-maskHealthCheckpointValue*maskIndex)
            {
                maskIndex++;
                maskImage.sprite = maskSprites[maskIndex];
            }
        }
    }
}
