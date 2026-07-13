using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Escena a cargar")]
    public string nombreEscenaJuego = "Overworld"; // Pon aquí el nombre EXACTO de tu escena del overworld

    [Header("Paneles")]
    public GameObject panelMenuPrincipal;
    public GameObject panelOpciones;

    [Header("Opciones - Audio")]
    public Slider sliderVolumen;

    private const string CLAVE_VOLUMEN = "VolumenGeneral";

    void Start()
    {
        panelMenuPrincipal.SetActive(true);
        panelOpciones.SetActive(false);

        float volumenGuardado = PlayerPrefs.GetFloat(CLAVE_VOLUMEN, 1f);
        AudioListener.volume = volumenGuardado;

        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    public void Jugar()
    {
        //PlayerStats.ReiniciarProgreso();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void AbrirOpciones()
    {
        panelMenuPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat(CLAVE_VOLUMEN, valor);
        PlayerPrefs.Save();
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}