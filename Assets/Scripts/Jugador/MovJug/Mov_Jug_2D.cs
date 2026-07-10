using UnityEngine;

public class Movimiento2D : MonoBehaviour
{
    public float velocidad = 5f;
    private Animator _animator;
    private Rigidbody2D rb;
    private Vector3 escalaOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        escalaOriginal = transform.localScale;
        _animator = GetComponent<Animator>();

        // Empieza desactivado hasta que entres al libro
        enabled = false;
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(x, y) * velocidad;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        bool estaCorriendo = x != 0 || y != 0;
        _animator.SetBool("EstaCorriendo", estaCorriendo);

        if (x > 0)
            transform.localScale = new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        else if (x < 0)
            transform.localScale = escalaOriginal;
    }
}