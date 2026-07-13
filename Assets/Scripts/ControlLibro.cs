using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ControlLibro : MonoBehaviour
{
    [Header("Referencias")]
    public Canvas canvasBook;
    public GeneradorHabitacion generadorHabitacion;
    public AutoFlip autoFlip;
    public Transform jugador2D;
    public Transform targetLibro;
    public CinemachineCamera camaraLibro;
    public TiempoRun tiempoRun;

    [Header("Camara")]
    public float fovNormal = 60f;
    public float fovAnimacion = 90f;
    public float velocidadZoom = 2f;

    private bool animando = false;
    private Vector3 posicionInicialJugador;
    private Vector3 posicionInicialTarget;

    void Start()
    {
        if (canvasBook != null)
            canvasBook.enabled = false;
    }

    public void GuardarPosicionInicial()
    {
        posicionInicialJugador = jugador2D.position;
        posicionInicialTarget = targetLibro.position;
    }

    public void MostrarLibro()
    {
        if (!animando)
        {
            animando = true;

            // Iniciamos el tiempo al entrar al libro
            if (tiempoRun != null) tiempoRun.ReanudarTiempo();

            Movimiento2D mov = jugador2D.GetComponent<Movimiento2D>();
            if (mov != null) mov.enabled = false;

            Rigidbody2D rb = jugador2D.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            StartCoroutine(SecuenciaLibro());
        }
    }

    private IEnumerator SecuenciaLibro()
    {
        yield return StartCoroutine(InterpolarFOV(fovNormal, fovAnimacion, velocidadZoom));
        yield return new WaitForSeconds(0.3f);
        canvasBook.enabled = true;
        autoFlip.AutoStartFlip = false;
        autoFlip.FlipRightPage();

        // Sonido al pasar de página
        SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoPasarPagina);
    }

    public void OnAnimacionTerminada()
    {
        StartCoroutine(SecuenciaFin());
    }

    private IEnumerator SecuenciaFin()
    {
        canvasBook.enabled = false;
        jugador2D.position = posicionInicialJugador;
        targetLibro.position = posicionInicialTarget;

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(InterpolarFOV(fovAnimacion, fovNormal, velocidadZoom));

        Movimiento2D mov = jugador2D.GetComponent<Movimiento2D>();
        if (mov != null) mov.enabled = true;

        // Paramos el tiempo al salir del libro
        if (tiempoRun != null) tiempoRun.PararTiempo();

        generadorHabitacion.IniciarOleada(1);
        animando = false;
    }

    private IEnumerator InterpolarFOV(float desde, float hasta, float velocidad)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;
            camaraLibro.Lens.FieldOfView = Mathf.Lerp(desde, hasta, t);
            yield return null;
        }
        camaraLibro.Lens.FieldOfView = hasta;
    }
}