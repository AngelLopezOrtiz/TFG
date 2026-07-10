using UnityEngine;
using System.Collections;

public class ParpadeoJugador : MonoBehaviour
{
    public float duracionParpadeo = 0.1f;
    public int vecesParpadeo = 3;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;
    private float invulnerableHasta = 0f;   // temporizador en vez de bool
    private Coroutine colorCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorOriginal = spriteRenderer.color;
    }

    // Es invulnerable mientras no se haya pasado el instante límite
    public bool EsInvulnerable()
    {
        return Time.time < invulnerableHasta;
    }

    // Daño: invulnerabilidad corta + parpadeo rojo
    public void Parpadear()
    {
        DarInvulnerabilidad(duracionParpadeo * vecesParpadeo * 2f);
        IniciarColor(Color.red, duracionParpadeo, vecesParpadeo);
    }

    // Dash: invulnerabilidad de la duración del dash + parpadeo azul
    public void ActivarInvulnerabilidad(float duracion)
    {
        Debug.Log($"ActivarInvulnerabilidad recibió duracion = {duracion}\n{System.Environment.StackTrace}");
        DarInvulnerabilidad(duracion);
        int veces = Mathf.Max(1, Mathf.RoundToInt(duracion / 0.1f));
        IniciarColor(Color.cyan, 0.05f, veces);
    }

    // Solo extiende la invulnerabilidad, nunca la acorta
    private void DarInvulnerabilidad(float duracion)
    {
        float fin = Time.time + duracion;
        if (fin > invulnerableHasta)
            invulnerableHasta = fin;
    }

    // Corta el parpadeo anterior antes de empezar otro (evita que peleen por el color)
    private void IniciarColor(Color color, float intervalo, int veces)
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(EfectoColor(color, intervalo, veces));
    }

    private IEnumerator EfectoColor(Color color, float intervalo, int veces)
    {
        for (int i = 0; i < veces; i++)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(intervalo);
            spriteRenderer.color = colorOriginal;
            yield return new WaitForSeconds(intervalo);
        }
        spriteRenderer.color = colorOriginal;
    }
}