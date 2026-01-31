using DG.Tweening;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using Yarn.Unity.Attributes;
public class ManualNodeMovement : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    OptionsPresenter optionsPresenter;
    [SerializeField] KeyCode AdvanceKey;
    [SerializeField] internal YarnProject? yarnProject;
    [YarnNode(nameof(yarnProject))]
    [SerializeField] string NodeToAdvanceTo;

    private CanvasGroup fadePanelCanvasGroup;
    private UnityEngine.UI.Image fadePanelImage;

    [Header("UI References")]
    public GameObject fadePanel; // Panel negro para transiciones

    [Header("Audio")]
    public AudioClip fadeTransitionSound; // Sonido fade transition
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public float fadeToBlackDuration = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
        // Configurar fade panel
        SetupFadePanel();
        // Configurar AudioSource
        SetupAudioSource();
    }

    private void SetupFadePanel()
    {
        if (fadePanel != null)
        {
            Debug.Log("Configurando fade panel...");

            // Asegurar que el panel esté activo
            fadePanel.SetActive(true);

            // Configurar CanvasGroup para el fade effect
            fadePanelCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
            if (fadePanelCanvasGroup == null)
            {
                fadePanelCanvasGroup = fadePanel.AddComponent<CanvasGroup>();
                Debug.Log("CanvasGroup añadido al fade panel");
            }

            // Obtener el Image component
            fadePanelImage = fadePanel.GetComponent<UnityEngine.UI.Image>();
            if (fadePanelImage == null)
            {
                Debug.LogError("El fade panel no tiene un componente Image!");
            }
            else
            {
                // Desactivar raycast target cuando está invisible para no bloquear hover
                fadePanelImage.raycastTarget = false;
                Debug.Log("Raycast target desactivado en fade panel");
            }

            // Iniciar invisible (alpha 0)
            fadePanelCanvasGroup.alpha = 0f;
            Debug.Log($"Fade panel configurado. Alpha inicial: {fadePanelCanvasGroup.alpha}");
        }
        else
        {
            Debug.LogError("Fade Panel no está asignado en el Inspector. Arrastra el panel negro aquí.");
        }
    }

    private void SetupAudioSource()
    {
        // Obtener o añadir AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar AudioSource
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;
    }

    public void ManualAdvence()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.StartDialogue(NodeToAdvanceTo);
        }
    }

    [YarnCommand("goToMaingame")]
    public void GoToMainGame()
    {
        // Fade to black y luego cambiar escena
        if (fadePanelCanvasGroup != null)
        {
            Debug.Log($"Iniciando fade. Alpha actual: {fadePanelCanvasGroup.alpha}");

            // Activar raycast target para bloquear interacciones durante el fade
            if (fadePanelImage != null)
            {
                fadePanelImage.raycastTarget = true;
                Debug.Log("Raycast target activado para el fade");
            }

            // Asegurar que el panel esté activo
            fadePanel.SetActive(true);

            fadePanelCanvasGroup.DOFade(1f, fadeToBlackDuration)
                .SetEase(Ease.InOutQuad)
                .OnStart(() => {
                    Debug.Log("Fade iniciado");
                    PlayFadeTransitionSound();
                })
                .OnUpdate(() => Debug.Log($"Fade en progreso, alpha: {fadePanelCanvasGroup.alpha}"))
                .OnComplete(() => {
                    Debug.Log("Fade completado, cargando escena");
                    SceneManager.LoadScene("MainGameScene");
                });
        }
        else
        {
            Debug.LogError("fadePanelCanvasGroup es null!");
            SceneManager.LoadScene("MainGameScene");
        }
    }

    private void PlayFadeTransitionSound()
    {
        if (audioSource != null && fadeTransitionSound != null)
        {
            audioSource.PlayOneShot(fadeTransitionSound);
        }
    }
}
