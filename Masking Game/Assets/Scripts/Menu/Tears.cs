using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tears : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fallDistance = 30f; // Distancia que cae la lágrima
    [SerializeField] private float fallDuration = 1.5f; // Duración del descenso y fade out
    [SerializeField] private float fadeInDuration = 0.8f; // Duración del fade in al reaparecer
    [SerializeField] private float pauseBetweenCycles = 0.5f; // Pausa entre ciclos
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private Image tearImage;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Color originalColor;
    private Coroutine animationCoroutine;
    
    void Start()
    {
        InitializeComponents();
        StartTearAnimation();
    }
    
    void InitializeComponents()
    {
        // Obtener componentes necesarios
        tearImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        
        if (tearImage == null)
        {
            Debug.LogError("Tears script requires an Image component on the GameObject.");
            return;
        }
        
        if (rectTransform == null)
        {
            Debug.LogError("Tears script requires a RectTransform component on the GameObject.");
            return;
        }
        
        // Guardar valores originales
        originalPosition = rectTransform.anchoredPosition;
        originalColor = tearImage.color;
    }
    
    void StartTearAnimation()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
            
        animationCoroutine = StartCoroutine(TearAnimationLoop());
    }
    
    IEnumerator TearAnimationLoop()
    {
        while (true)
        {
            yield return StartCoroutine(TearFallAndFadeOut());
            yield return StartCoroutine(TearFadeIn());
            yield return new WaitForSeconds(pauseBetweenCycles);
        }
    }
    
    IEnumerator TearFallAndFadeOut()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fallDuration)
        {
            float normalizedTime = elapsedTime / fallDuration;
            
            // Calcular posición usando la curva de movimiento
            float movementProgress = movementCurve.Evaluate(normalizedTime);
            Vector3 currentPosition = originalPosition + Vector3.down * (fallDistance * movementProgress);
            rectTransform.anchoredPosition = currentPosition;
            
            // Calcular alfa usando la curva de fade out
            float fadeProgress = fadeOutCurve.Evaluate(normalizedTime);
            Color currentColor = originalColor;
            currentColor.a = originalColor.a * fadeProgress;
            tearImage.color = currentColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Asegurar que está completamente invisible al final del fade out
        Color invisibleColor = originalColor;
        invisibleColor.a = 0f;
        tearImage.color = invisibleColor;
    }
    
    IEnumerator TearFadeIn()
    {
        // Restaurar posición original pero mantener invisibilidad
        rectTransform.anchoredPosition = originalPosition;
        Color invisibleColor = originalColor;
        invisibleColor.a = 0f;
        tearImage.color = invisibleColor;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            float normalizedTime = elapsedTime / fadeInDuration;
            
            // Calcular alfa usando la curva de fade in
            float fadeProgress = fadeInCurve.Evaluate(normalizedTime);
            Color currentColor = originalColor;
            currentColor.a = originalColor.a * fadeProgress;
            tearImage.color = currentColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Asegurar que termina completamente visible
        tearImage.color = originalColor;
    }
    
    // Métodos públicos para control externo
    public void StartAnimation()
    {
        StartTearAnimation();
    }
    
    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        // Restaurar estado original
        rectTransform.anchoredPosition = originalPosition;
        tearImage.color = originalColor;
    }
    
    public void PauseAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }
    
    // Configurar parámetros en runtime
    public void SetAnimationSettings(float fallDist, float fallDur, float fadeInDur, float pauseBetween)
    {
        fallDistance = fallDist;
        fallDuration = fallDur;
        fadeInDuration = fadeInDur;
        pauseBetweenCycles = pauseBetween;
    }
    
    void OnDisable()
    {
        StopAnimation();
    }
    
    void OnDestroy()
    {
        StopAnimation();
    }
}
