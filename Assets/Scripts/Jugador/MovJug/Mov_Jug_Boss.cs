using UnityEngine;

public class Mov_Jug_Boss : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Animator _animator;
    private Vector3 escalaOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(x * velocidad, rb.linearVelocity.y);
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        bool corriendo = x != 0;
        _animator.SetBool("EstaCorriendo", corriendo);

        if (x > 0)
            transform.localScale = new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        else if (x < 0)
            transform.localScale = escalaOriginal;
    }
}