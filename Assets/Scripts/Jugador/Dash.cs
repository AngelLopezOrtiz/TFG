using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    [Header("Dash")]
    public float fuerzaDash = 15f;
    public float duracionDash = 1f;
    public float cooldownDash = 1f;

    private bool puedeHacerDash = false; // se activa al comprar
    private bool estaDasheando = false;
    private float tiempoUltimoDash = -999f;

    private Rigidbody2D rb;
    private Movimiento2D movimiento;
    private ParpadeoJugador parpadeo;
    private Animator _animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<Movimiento2D>();
        parpadeo = GetComponent<ParpadeoJugador>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!puedeHacerDash || estaDasheando) return;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= tiempoUltimoDash + cooldownDash)
            StartCoroutine(HacerDash());
    }

    IEnumerator HacerDash()
    {
        Debug.Log("Dash arranca, poniendo EstaDasheando = true");
        estaDasheando = true;
        tiempoUltimoDash = Time.time;

        // Desactiva movimiento normal durante el dash
        movimiento.enabled = false;

        // Dirección del dash según input
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direccion = new Vector2(x, y).normalized;

        // Si no hay input, dashea hacia donde mira
        if (direccion == Vector2.zero)
            direccion = transform.localScale.x < 0 ? Vector2.right : Vector2.left;

        // El Animator se encarga del visual
        _animator.SetBool("EstaDasheando", true);

        rb.linearVelocity = direccion * fuerzaDash;

        // Invulnerabilidad durante el dash
        Debug.Log($"duracionDash = {duracionDash} en objeto: {gameObject.name}", gameObject);
        if (parpadeo != null)
            parpadeo.ActivarInvulnerabilidad(duracionDash + 0.8f);

        yield return new WaitForSeconds(duracionDash);

        rb.linearVelocity = Vector2.zero;

        _animator.SetBool("EstaDasheando", false);

        movimiento.enabled = true;
        estaDasheando = false;
    }

    public void DesbloquearDash()
    {
        puedeHacerDash = true;
        Debug.Log("¡Dash desbloqueado!");
    }
}