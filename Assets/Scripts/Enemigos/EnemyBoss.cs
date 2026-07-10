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

    private float tiempoUltimoMelee = 0f;
    private float tiempoUltimaDistancia = 0f;

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
        Collider2D hit = Physics2D.OverlapCircle(transform.position, rangoGolpeMelee, LayerMask.GetMask("Jugador"));
        if (hit != null)
        {
            PlayerStats stats = hit.GetComponent<PlayerStats>();
            stats?.RecibirDanio(danioMelee);
            Debug.Log("Golpe melee conectado al jugador");
        }
        else
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
            Proyectil p = proyectil.GetComponent<Proyectil>();
            if (p != null) p.Inicializar(direccion, danioDistancia);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        GetComponent<ParpadeoGolpe>()?.Parpadear();

        if (vidaActual <= 0)
            Morir();
    }

    public void OnBossMuerto()
    {
        Debug.Log("Boss derrotado - volviendo al overworld");
        PlayerPrefs.SetInt("BossCompletado", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

    private void Morir()
    {
        Debug.Log("Boss derrotado");
        Destroy(gameObject);
    }
}