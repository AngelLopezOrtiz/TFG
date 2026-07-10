using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float velocidad = 8f;
    public float danio = 10f;
    public float tiempoVida = 5f;

    private Vector2 direccion;
    private bool inicializado = false;

    public void Inicializar(Vector2 dir, float dmg)
    {
        direccion = dir.normalized;
        danio = dmg;
        inicializado = true;

        // Rotar el prefab para que la BASE de la llama apunte hacia la dirección de viaje.
        // El +90 compensa que el sprite tiene la punta hacia arriba (+Y) por defecto.
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo + 90f);

        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        if (!inicializado) return;
        // Space.World para que la rotación no afecte a la dirección de movimiento.
        transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
                stats.RecibirDanio(danio);
            Destroy(gameObject);
        }
    }
}