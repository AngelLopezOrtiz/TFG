using UnityEngine;

public class SeguimientoCamaraLibro : MonoBehaviour
{
    [Header("Referencia al jugador 2D")]
    public Transform jugador2D;

    [Header("Referencia al plano del libro en la escena 3D")]
    public Transform planoLibro;

    [Header("Objeto vacío que Cinemachine seguirá")]
    public Transform targetVirtual;

    [Header("Ajuste de sensibilidad")]
    public float sensibilidad = 0.01f;

    public float limiteX = 0.5f;
    public float limiteY = 0.5f;

    private Vector3 offsetCamaraPlano;
    private Vector2 posicionInicialJugador;

    void Start()
    {
        offsetCamaraPlano = targetVirtual.position - planoLibro.position;
        posicionInicialJugador = new Vector2(jugador2D.position.x, jugador2D.position.y);
    }

    void LateUpdate()
    {
        if (jugador2D == null || planoLibro == null || targetVirtual == null) return;

        float offsetX = Mathf.Clamp((jugador2D.position.x - posicionInicialJugador.x) * sensibilidad, -limiteX, limiteX);
        float offsetY = Mathf.Clamp((jugador2D.position.y - posicionInicialJugador.y) * sensibilidad, -limiteY, limiteY);

        targetVirtual.position = planoLibro.position
            + offsetCamaraPlano
            - planoLibro.right * offsetX
            - planoLibro.forward * offsetY;
    }
}