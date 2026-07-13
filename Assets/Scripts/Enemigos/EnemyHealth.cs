using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima = 30f;
    private float vidaActual;

    [Header("Inmunidad por aura")]
    public Color colorInmune = new Color(0.4f, 0f, 0.8f, 1f);

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;
    private Coroutine coroutinaInmune;

    void Start()
    {
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;
    }

    public void RecibirDanio(float cantidad)
    {
        bool soyElAlquimista = GetComponent<EnemyAlquimista>() != null;

        EnemyAlquimista[] alquimistas = FindObjectsByType<EnemyAlquimista>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        bool hayAlquimista = alquimistas.Length > 0;

        if (hayAlquimista && !soyElAlquimista)
        {
            if (coroutinaInmune != null) StopCoroutine(coroutinaInmune);
            coroutinaInmune = StartCoroutine(ParpadeoInmune());
            return;
        }

        vidaActual -= cantidad;
        Debug.Log($"{gameObject.name} recibe {cantidad} de daño. Vida restante: {vidaActual}");

        GetComponent<ParpadeoGolpe>()?.Parpadear();

        if (vidaActual <= 0)
            Morir();
        else
            SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoDanioEnemigo);
    }

    private IEnumerator ParpadeoInmune()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = colorInmune;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = colorOriginal;
    }

    private void Morir()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoMuerteEnemigo);
        Destroy(gameObject);
    }
}