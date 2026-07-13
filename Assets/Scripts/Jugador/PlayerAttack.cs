using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque")]
    public float cooldown = 0.5f;
    public float rangoAtaque = 1.5f;
    public float fuerzaRetroceso = 5f;

    private float tiempoUltimoAtaque = 0f;
    private PlayerStats playerStats;
    private Rigidbody2D rb;
    private Vector2 ultimaDireccion = Vector2.down;
    private Animator _animator;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (rb.linearVelocity != Vector2.zero)
            ultimaDireccion = rb.linearVelocity.normalized;

        if (Input.GetMouseButtonDown(0) && PuedoAtacar())
            Atacar();
    }

    private bool PuedoAtacar()
    {
        return Time.time >= tiempoUltimoAtaque + cooldown;
    }

    private void Atacar()
    {
        tiempoUltimoAtaque = Time.time;
        _animator.SetTrigger("Atacar");

        // Sonido de ataque del jugador
        SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoAtaqueJugador);

        RaycastHit2D[] golpes = Physics2D.CircleCastAll(
            transform.position,
            rangoAtaque,
            ultimaDireccion,
            0.5f
        );

        foreach (RaycastHit2D golpe in golpes)
        {
            if (golpe.collider.CompareTag("Enemigo"))
            {
                EnemyHealth enemyHealth = golpe.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.RecibirDanio(playerStats.danio);

                EnemyBoss boss = golpe.collider.GetComponent<EnemyBoss>();
                if (boss != null)
                    boss.RecibirDanio(playerStats.danio);

                EnemyMago mago = golpe.collider.GetComponent<EnemyMago>();
                if (mago != null)
                {
                    Vector2 direccionRetroceso = (golpe.collider.transform.position - transform.position).normalized;
                    mago.AplicarRetroceso(direccionRetroceso * fuerzaRetroceso);
                }

                EnemyArana arana = golpe.collider.GetComponent<EnemyArana>();
                if (arana != null)
                {
                    Vector2 direccionRetroceso = (golpe.collider.transform.position - transform.position).normalized;
                    arana.AplicarRetroceso(direccionRetroceso * fuerzaRetroceso);
                }
            }
        }

        Debug.Log($"Ataque en dirección {ultimaDireccion}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}