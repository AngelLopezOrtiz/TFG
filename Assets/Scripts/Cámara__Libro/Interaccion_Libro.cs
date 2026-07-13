using UnityEngine;

public class InteraccionLibro : MonoBehaviour
{
    [Header("Jugadores")]
    public GameObject jugador3D;
    public GameObject jugador2D;

    [Header("Cámaras Cinemachine")]
    public GameObject camaraJugador3D;
    public GameObject camaraLibro;

    [Header("UI")]
    public GameObject canvasVidas;

    private bool jugadorCerca = false;
    private bool dentroDelLibro = false;

    void Start()
    {
        if (canvasVidas != null) canvasVidas.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (!dentroDelLibro) EntrarAlLibro();
            else SalirDelLibro();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }

    private void EntrarAlLibro()
    {
        dentroDelLibro = true;

        jugador3D.GetComponent<Movimiento3D>().enabled = false;
        jugador2D.GetComponent<Movimiento2D>().enabled = true;

        Rigidbody2D rb = jugador2D.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
        }

        camaraJugador3D.SetActive(false);
        camaraLibro.SetActive(true);

        if (canvasVidas != null) canvasVidas.SetActive(true);

        ControlLibro controlLibro = FindObjectOfType<ControlLibro>();
        if (controlLibro != null)
            controlLibro.GuardarPosicionInicial();

        // Iniciamos el tiempo al entrar al libro
        TiempoRun tiempoRun = FindObjectOfType<TiempoRun>();
        if (tiempoRun != null) tiempoRun.ReanudarTiempo();

        // Iniciamos la primera oleada al entrar al libro
        GeneradorHabitacion generador = FindObjectOfType<GeneradorHabitacion>();
        if (generador != null)
            generador.IniciarOleada(1);

        SetEnemigos(true);

        // Música de fondo solo dentro del libro
        SonidoManager.Instancia?.ReproducirMusica(SonidoManager.Instancia.musicaFondo);
    }

    private void SalirDelLibro()
    {
        dentroDelLibro = false;

        jugador2D.GetComponent<Movimiento2D>().enabled = false;
        jugador3D.GetComponent<Movimiento3D>().enabled = true;

        camaraLibro.SetActive(false);
        camaraJugador3D.SetActive(true);

        if (canvasVidas != null) canvasVidas.SetActive(false);

        // Paramos el tiempo al salir del libro
        TiempoRun tiempoRun = FindObjectOfType<TiempoRun>();
        if (tiempoRun != null) tiempoRun.PararTiempo();

        SetEnemigos(false);

        // Parar la música al volver al mundo 3D
        SonidoManager.Instancia?.PararMusica();
    }

    private void SetEnemigos(bool activo)
    {
        foreach (EnemyMago mago in FindObjectsByType<EnemyMago>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            mago.enabled = activo;
            Animator anim = mago.GetComponent<Animator>();
            if (anim != null) anim.enabled = activo;
        }

        foreach (EnemyAlquimista alquimista in FindObjectsByType<EnemyAlquimista>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            alquimista.enabled = activo;
            Animator anim = alquimista.GetComponent<Animator>();
            if (anim != null) anim.enabled = activo;
        }

        foreach (EnemyArana arana in FindObjectsByType<EnemyArana>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            arana.enabled = activo;
            Animator anim = arana.GetComponent<Animator>();
            if (anim != null) anim.enabled = activo;
        }
    }
}