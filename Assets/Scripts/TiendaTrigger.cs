using UnityEngine;
using Unity.Cinemachine;

public class TiendaTrigger : MonoBehaviour
{
    public GameObject canvasTienda;
    public CinemachineCamera camaraJugador;

    private bool jugadorDentro = false;
    private CinemachineInputAxisController inputController;

    void Start()
    {
        canvasTienda.SetActive(false);
        if (camaraJugador != null)
            inputController = camaraJugador.GetComponent<CinemachineInputAxisController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            jugadorDentro = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            CerrarTienda();
        }
    }

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (!canvasTienda.activeSelf)
                AbrirTienda();
            else
                CerrarTienda();
        }
    }

    void AbrirTienda()
    {
        canvasTienda.SetActive(true);
        if (inputController != null) inputController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CerrarTienda()
    {
        canvasTienda.SetActive(false);
        if (inputController != null) inputController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}