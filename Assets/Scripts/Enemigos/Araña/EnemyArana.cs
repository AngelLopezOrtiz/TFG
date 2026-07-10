using UnityEngine;
using Pathfinding;

public class EnemyArana : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 4f;
    public float distanciaNextWaypoint = 0.5f;
    public float intervaloRecalculo = 0.5f;

    [Header("Atasco")]
    public float tiempoLimiteAtasco = 0.3f;

    [Header("Retroceso")]
    public float fuerzaRetroceso = 20f;
    public float duracionRetroceso = 2f;

    [Header("Daño al jugador")]
    public float danioContacto = 1f;
    public float cooldownDanio = 1f;

    [Header("Delay inicial")]
    public float delayInicio = 5f;

    private float tiempoActivacion = 0f;
    private bool listo = false;
    private float tiempoRetroceso = 0f;
    private bool enRetroceso = false;
    private float tiempoUltimoDanio = -999f;

    private Transform jugador;
    private Rigidbody2D rb;
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float tiempoUltimoPath = 0f;

    private Vector2 posicionAnterior;
    private float tiempoSinMoverse = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        GameObject obj = GameObject.FindGameObjectWithTag("Jugador");
        if (obj != null) jugador = obj.transform;
        enabled = false;
    }

    void OnEnable()
    {
        if (jugador == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Jugador");
            if (obj != null) jugador = obj.transform;
        }
        tiempoActivacion = Time.time;
        listo = false;
        path = null;
        posicionAnterior = rb.position;
        tiempoSinMoverse = 0f;
    }

    void Update()
    {
        if (jugador == null) return;

        if (!listo)
        {
            if (Time.time >= tiempoActivacion + delayInicio)
                listo = true;
            else
                return;
        }

        if (enRetroceso)
        {
            if (Time.time >= tiempoRetroceso + duracionRetroceso)
                enRetroceso = false;
            return;
        }

        // Detectar atasco y forzar recálculo
        float distanciaMovida = Vector2.Distance(rb.position, posicionAnterior);
        if (distanciaMovida < 0.01f)
        {
            tiempoSinMoverse += Time.deltaTime;
            if (tiempoSinMoverse >= tiempoLimiteAtasco)
            {
                tiempoSinMoverse = 0f;
                tiempoUltimoPath = 0f;
            }
        }
        else
        {
            tiempoSinMoverse = 0f;
        }
        posicionAnterior = rb.position;

        if (Time.time >= tiempoUltimoPath + intervaloRecalculo)
        {
            tiempoUltimoPath = Time.time;
            seeker.StartPath(rb.position, jugador.position, OnPathComplete);
        }

        Mover();
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Mover()
    {
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direccion = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.linearVelocity = direccion * velocidad;

        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < distanciaNextWaypoint)
            currentWaypoint++;

        // Flip sprite
        if (rb.linearVelocity.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (rb.linearVelocity.x < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void AplicarRetroceso(Vector2 fuerza)
    {
        enRetroceso = true;
        tiempoRetroceso = Time.time;
        path = null;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(fuerza, ForceMode2D.Impulse);
    }

    public void OnTriggerDanio(Collider2D other)
    {
        if (!other.CompareTag("Jugador")) return;
        if (Time.time < tiempoUltimoDanio + cooldownDanio) return;

        tiempoUltimoDanio = Time.time;
        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null)
            stats.RecibirDanio(danioContacto);
    }
}