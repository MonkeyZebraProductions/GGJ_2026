using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creditsPanel; // Arrastra aquí el panel de créditos desde el Inspector
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Asegurar que el panel de créditos esté cerrado al inicio
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    // Método para el botón Play
    public void PlayButton()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    // Método para el botón Quit
    public void QuitButton()
    {
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
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    // Método para cerrar el panel de créditos (opcional, para un botón "X" en el panel)
    public void CloseCreditsPanel()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }
}
