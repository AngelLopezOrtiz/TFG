using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyAlquimista : MonoBehaviour
{
    [Header("Aura")]
    public float radioAura = 6f;
    public Color colorAura = new Color(0.4f, 0f, 0.8f, 0.4f);

    [Header("Movimiento")]
    public float velocidad = 1.5f;
    public float distanciaHuida = 8f;
    public float distanciaNextWaypoint = 0.5f;
    public float intervaloRecalculo = 0.5f;

    [Header("Atasco")]
    public float tiempoLimiteAtasco = 0.3f;

    [Header("Retroceso")]
    public float duracionRetroceso = 0.15f;

    [Header("Delay inicial")]
    public float delayInicio = 1f;

    private float tiempoActivacion = 0f;
    private bool listo = false;
    private float tiempoRetroceso = 0f;
    private bool enRetroceso = false;
    private Transform jugador;
    private Rigidbody2D rb;

    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float tiempoUltimoPath = 0f;

    private Vector2 posicionAnterior;
    private float tiempoSinMoverse = 0f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;
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
            CalcularRuta();
        }

        Mover();
    }

    private void CalcularRuta()
    {
        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Solo huye si el jugador está suficientemente cerca
        if (distancia > distanciaHuida)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direccionHuida = (transform.position - jugador.position).normalized;
        Vector2 destino = rb.position + direccionHuida * distanciaHuida;

        seeker.StartPath(rb.position, destino, OnPathComplete);
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
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(fuerza, ForceMode2D.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0f, 0.8f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radioAura);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, distanciaHuida);
    }
}