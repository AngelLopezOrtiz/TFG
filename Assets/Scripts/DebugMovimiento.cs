using UnityEngine;

public class DebugMovimiento : MonoBehaviour
{
    private Vector3 posicionAnterior;
    private Vector3 escalaAnterior;

    void Start()
    {
        posicionAnterior = transform.position;
        escalaAnterior = transform.localScale;
    }

    void Update()
    {
        if (transform.position != posicionAnterior)
        {
            Debug.Log($"POSICION CAMBIADA: {posicionAnterior} → {transform.position}", gameObject);
            Debug.Log($"Stack: {System.Environment.StackTrace}");
            posicionAnterior = transform.position;
        }

        if (transform.localScale != escalaAnterior)
        {
            Debug.Log($"ESCALA CAMBIADA: {escalaAnterior} → {transform.localScale}", gameObject);
            Debug.Log($"Stack: {System.Environment.StackTrace}");
            escalaAnterior = transform.localScale;
        }
    }
}