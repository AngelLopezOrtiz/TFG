using UnityEngine;
using System.Collections;

public class ParpadeoGolpe : MonoBehaviour
{
    public float duracionParpadeo = 0.1f;
    public int vecesParpadeo = 3;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorOriginal = spriteRenderer.color;
    }

    public void Parpadear()
    {
        StartCoroutine(EfectoParpadeo());
    }

    private IEnumerator EfectoParpadeo()
    {
        for (int i = 0; i < vecesParpadeo; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(duracionParpadeo);
            spriteRenderer.color = colorOriginal;
            yield return new WaitForSeconds(duracionParpadeo);
        }
    }
}