using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creditsPanel; // Arrastra aquí el panel de créditos desde el Inspector
    public GameObject fadePanel; // Panel negro para transiciones
    
    [Header("Audio")]
    public AudioClip hoverSound; // Sonido hover
    public AudioClip clickSound; // Sonido click
    public AudioClip fadeTransitionSound; // Sonido fade transition
    private AudioSource audioSource;
    
    [Header("Animation Settings")]
    public float fadeToBlackDuration = 1.0f;
    
    [Header("External Links")]
    [SerializeField] private UnityEngine.UI.Button linkButton1;
    [SerializeField] private UnityEngine.UI.Button linkButton2;
    [SerializeField] private UnityEngine.UI.Button linkButton3;
    [SerializeField] private UnityEngine.UI.Button linkButton4;
    [SerializeField] private UnityEngine.UI.Button linkButton5;
    
    [SerializeField] private string url1 = "";
    [SerializeField] private string url2 = "";
    [SerializeField] private string url3 = "";
    [SerializeField] private string url4 = "";
    [SerializeField] private string url5 = "";
    
    private CanvasGroup creditsPanelCanvasGroup;
    private CanvasGroup fadePanelCanvasGroup;
    private UnityEngine.UI.Image fadePanelImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Configurar AudioSource
        SetupAudioSource();
        
        // Asegurar que el panel de créditos esté cerrado al inicio
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
            
            // Obtener o añadir CanvasGroup para el fade effect
            creditsPanelCanvasGroup = creditsPanel.GetComponent<CanvasGroup>();
            if (creditsPanelCanvasGroup == null)
            {
                creditsPanelCanvasGroup = creditsPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Configurar fade panel
        SetupFadePanel();
        
        // Configurar botones de enlaces
        SetupLinkButtons();
        
        // Configurar hover effects en todos los botones
        SetupHoverEffects();
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
    
    private void SetupHoverEffects()
    {
        // Encontrar todos los botones en la escena y añadir hover effects
        Button[] allButtons = FindObjectsOfType<Button>();
        
        foreach (Button button in allButtons)
        {
            AddHoverEffect(button);
        }
        
        // Asegurar que los botones de links específicos tengan hover effect
        if (linkButton1 != null) AddHoverEffect(linkButton1);
        if (linkButton2 != null) AddHoverEffect(linkButton2);
        if (linkButton3 != null) AddHoverEffect(linkButton3);
        if (linkButton4 != null) AddHoverEffect(linkButton4);
        if (linkButton5 != null) AddHoverEffect(linkButton5);
    }
    
    private void AddHoverEffect(Button button)
    {
        if (button == null) return;
        
        // Obtener o añadir EventTrigger
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        
        // Verificar si ya tiene un hover effect
        bool hasHoverEffect = false;
        foreach (var triggerEvent in trigger.triggers)
        {
            if (triggerEvent.eventID == EventTriggerType.PointerEnter)
            {
                hasHoverEffect = true;
                break;
            }
        }
        
        // Si no tiene hover effect, añadirlo
        if (!hasHoverEffect)
        {
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(hoverEntry);
        }
    }
    
    private void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
    
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    
    private void PlayFadeTransitionSound()
    {
        if (audioSource != null && fadeTransitionSound != null)
        {
            audioSource.PlayOneShot(fadeTransitionSound);
        }
    }

    // Método para el botón Play
    public void PlayButton()
    {
        PlayClickSound();
        Debug.Log("PlayButton presionado");
        
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
                    SceneManager.LoadScene("Tutorial Scene");
                });
        }
        else
        {
            Debug.LogError("fadePanelCanvasGroup es null!");
            SceneManager.LoadScene("Tutorial Scene");
        }
    }

    // Método para el botón Quit
    public void QuitButton()
    {
        PlayClickSound();
        #if UNITY_EDITOR
            // Si estamos en el editor, para el modo play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si estamos en build, cierra la aplicación
            Application.Quit();
        #endif
    }

    // Método para abrir el panel de créditos
    public void OpenCreditsPanel()
    {
        PlayClickSound();
        if (creditsPanel != null && creditsPanelCanvasGroup != null)
        {
            // Activar el panel y hacer fade in
            creditsPanel.SetActive(true);
            creditsPanelCanvasGroup.alpha = 0f;
            creditsPanelCanvasGroup.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad);
        }
    }

    // Método para cerrar el panel de créditos (opcional, para un botón "X" en el panel)
    public void CloseCreditsPanel()
    {
        PlayClickSound();
        if (creditsPanel != null && creditsPanelCanvasGroup != null)
        {
            creditsPanelCanvasGroup.DOFade(0f, 0.5f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => {
                    creditsPanel.SetActive(false);
                });
        }
    }
    
    // Configurar los botones de enlaces
    private void SetupLinkButtons()
    {
        if (linkButton1 != null) linkButton1.onClick.AddListener(() => OpenURL(url1));
        if (linkButton2 != null) linkButton2.onClick.AddListener(() => OpenURL(url2));
        if (linkButton3 != null) linkButton3.onClick.AddListener(() => OpenURL(url3));
        if (linkButton4 != null) linkButton4.onClick.AddListener(() => OpenURL(url4));
        if (linkButton5 != null) linkButton5.onClick.AddListener(() => OpenURL(url5));
    }
    
    // Método para abrir URL
    private void OpenURL(string url)
    {
        PlayClickSound();
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("URL está vacía o no asignada");
        }
    }
    
    // Métodos públicos para usar directamente en botones (alternativa)
    public void OpenURL1() => OpenURL(url1);
    public void OpenURL2() => OpenURL(url2);
    public void OpenURL3() => OpenURL(url3);
    public void OpenURL4() => OpenURL(url4);
    public void OpenURL5() => OpenURL(url5);
}
