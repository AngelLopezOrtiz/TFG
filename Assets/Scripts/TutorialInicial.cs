using UnityEngine;

public class TutorialInicial : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject canvasTutorial;

    [Header("Opciones")]
    public bool pausarJuego = true;
    public bool liberarCursor = true;

    void Start()
    {
        bool yaVisto = PlayerPrefs.GetInt("TutorialVisto", 0) == 1;

        if (yaVisto)
        {
            if (canvasTutorial != null) canvasTutorial.SetActive(false);
        }
        else
        {
            MostrarTutorial();
        }
    }

    void Update()
    {
        if (canvasTutorial != null && canvasTutorial.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                CerrarTutorial();
        }
    }

    private void MostrarTutorial()
    {
        canvasTutorial.SetActive(true);

        if (pausarJuego)
            Time.timeScale = 0f;

        if (liberarCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CerrarTutorial()
    {
        canvasTutorial.SetActive(false);

        if (pausarJuego)
            Time.timeScale = 1f;

        if (liberarCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        PlayerPrefs.SetInt("TutorialVisto", 1);
        PlayerPrefs.Save();
    }
}