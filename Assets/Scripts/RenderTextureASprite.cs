// RenderTextureASprite.cs
using UnityEngine;

public class RenderTextureASprite : MonoBehaviour
{
    [Header("Tu Render Texture de la escena 2D")]
    public RenderTexture renderTexture;

    [Header("El componente Book del asset")]
    public Book book;

    [Header("En qué índice del array va tu página jugable")]
    public int indicePagina = 0;

    private Texture2D texturaIntermedia;
    private Sprite spriteGenerado;

    void Start()
    {
        // Creamos una Texture2D del mismo tamaño que el Render Texture
        texturaIntermedia = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBA32,
            false
        );
    }

    void Update()
    {
        // Cada frame copiamos el Render Texture a la Texture2D
        RenderTexture.active = renderTexture;
        texturaIntermedia.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0
        );
        texturaIntermedia.Apply();
        RenderTexture.active = null;

        // Convertimos a Sprite y lo asignamos al array de páginas del Book
        spriteGenerado = Sprite.Create(
            texturaIntermedia,
            new Rect(0, 0, texturaIntermedia.width, texturaIntermedia.height),
            new Vector2(0.5f, 0.5f)
        );

        book.bookPages[indicePagina] = spriteGenerado;
    }
}