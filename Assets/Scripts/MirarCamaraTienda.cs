using UnityEngine;

public class MirarJugador : MonoBehaviour
{
    private Transform jugador;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 direccion = transform.position - jugador.position;
        direccion.y = 0;
        transform.rotation = Quaternion.LookRotation(direccion);
    }
}