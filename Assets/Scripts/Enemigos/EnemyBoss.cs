using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyBoss : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima = 100f;
    private float vidaActual;

    [Header("Ataque Melee")]
    public float rangoMelee = 150f;
    public float danioMelee = 15f;
    public float cooldownMelee = 2f;
    public float rangoGolpeMelee = 3f;

    [Header("Ataque Distancia")]
    public float rangoDistancia = 8f;
    public float danioDistancia = 10f;
    public float cooldownDistancia = 3f;
    public GameObject prefabProyectil;
    public Color colorProyectil = Color.red; // color del proyectil del boss

    [Header("Debug Golpe")]
    public float duracionDebugGolpe = 0.5f; // cuánto se ve el círculo tras golpear

    private float tiempoUltimoMelee = 0f;
    private float tiempoUltimaDistancia = 0f;
    private float tiempoGolpeDibujado = -999f; // cuándo fue el último GolpeMelee
    private bool muerto = false;

    private Transform jugador;
    private Animator _animator;
    private PlayerStats playerStats;

    void Start()
    {
        vidaActual = vidaMaxima;
        _animator = GetComponent<Animator>();

        GameObject obj = GameObject.FindGameObjectWithTag("Jugador");
        if (obj != null)
        {
            jugador = obj.transform;
            playerStats = obj.GetComponent<PlayerStats>();
            Debug.Log("Jugador encontrado: " + obj.name);
        }
        else
            Debug.Log("ERROR: Jugador NO encontrado con tag Jugador");
    }

    void Update()
    {
        if (muerto) return;
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoMelee && Time.time >= tiempoUltimoMelee + cooldownMelee)
        {
            AtacarMelee();
        }
        else if (distancia <= rangoDistancia && Time.time >= tiempoUltimaDistancia + cooldownDistancia)
        {
            AtacarDistancia();
        }
    }

    private void AtacarMelee()
    {
        tiempoUltimoMelee = Time.time;
        _animator.SetTrigger("AtacarMelee");
        // El daño se aplica desde GolpeMelee() via Animation Event
    }

    // Llamar este método desde un Animation Event en el frame del golpe
    public void GolpeMelee()
    {
        tiempoGolpeDibujado = Time.time; // marca el momento para el gizmo

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangoGolpeMelee);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Jugador"))
            {
                PlayerStats stats = hit.GetComponent<PlayerStats>();
                stats?.RecibirDanio(danioMelee);
                Debug.Log("Golpe melee conectado al jugador");
                return;
            }
        }
        Debug.Log("Golpe melee fallado - jugador fuera de rango");
    }

    private void AtacarDistancia()
    {
        tiempoUltimaDistancia = Time.time;
        _animator.SetTrigger("AtacarDistancia");

        if (prefabProyectil != null)
        {
            Vector2 direccion = (jugador.position - transform.position).normalized;
            GameObject proyectil = Instantiate(prefabProyectil, transform.position, Quaternion.identity);

            // Teñir solo el proyectil del boss, sin tocar el prefab
            SpriteRenderer sr = proyectil.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = colorProyectil;

            Proyectil p = proyectil.GetComponent<Proyectil>();
            if (p != null) p.Inicializar(direccion, danioDistancia);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        if (muerto) return;

        vidaActual -= cantidad;
        GetComponent<ParpadeoGolpe>()?.Parpadear();

        if (vidaActual <= 0)
            Morir();
    }

    private void Morir()
    {
        muerto = true;
        Debug.Log("Boss derrotado - volviendo al overworld");
        PlayerPrefs.SetInt("BossCompletado", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

    void OnDrawGizmos()
    {
        // Círculo del golpe: gris siempre, rojo unos instantes tras golpear
        bool golpeReciente = Time.time <= tiempoGolpeDibujado + duracionDebugGolpe;
        Gizmos.color = golpeReciente ? Color.red : new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, rangoGolpeMelee);
    }
}