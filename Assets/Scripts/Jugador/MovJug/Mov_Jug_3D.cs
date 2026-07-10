using UnityEngine;

public class Movimiento3D : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidad = 5f;
    public float velocidadGiro = 10f; // Qué tan rápido rota el personaje al cambiar de dirección

    [Header("Cámara")]
    public Transform camaraPrincipal; // Aquí conectaremos la cámara normal

    private Rigidbody rb;
    private Vector3 direccionMovimiento;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Si se nos olvida poner la cámara en el Inspector, el código la busca solo
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Recogemos las teclas (W, A, S, D o Flechas)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 2. Averiguamos hacia dónde mira la cámara
        Vector3 camaraAdelante = camaraPrincipal.forward;
        Vector3 camaraDerecha = camaraPrincipal.right;

        // Anulamos el eje Y para que el personaje no intente volar si miras al cielo o enterrarse si miras al suelo
        camaraAdelante.y = 0f;
        camaraDerecha.y = 0f;
        camaraAdelante.Normalize();
        camaraDerecha.Normalize();

        // 3. Calculamos la dirección final basándonos en la cámara
        direccionMovimiento = (camaraAdelante * vertical + camaraDerecha * horizontal).normalized;

        // 4. Hacemos que el personaje rote suavemente hacia donde está caminando
        if (direccionMovimiento != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccionMovimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadGiro * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // 5. Movemos al personaje con las físicas
        rb.MovePosition(rb.position + direccionMovimiento * velocidad * Time.fixedDeltaTime);
    }
}