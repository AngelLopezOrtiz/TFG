using UnityEngine;

public class SonidoManager : MonoBehaviour
{
    public static SonidoManager Instancia;

    [Header("Fuente de audio para efectos")]
    public AudioSource audioSourceSFX;

    [Header("Fuente de audio para música")]
    public AudioSource audioSourceMusica;

    [Header("Sonidos generales")]
    public AudioClip sonidoPaso;
    public AudioClip sonidoAtaqueJugador;
    public AudioClip sonidoDanioJugador;
    public AudioClip sonidoMuerteJugador;

    [Header("Sonidos boss")]
    public AudioClip sonidoAtaqueMeleeBoss;
    public AudioClip sonidoAtaqueDistanciaBoss;
    public AudioClip sonidoDanioBoss;
    public AudioClip sonidoMuerteBoss;

    void Awake()
    {
        // Patrón Singleton: si ya existe uno, destruye este
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ReproducirSonido(AudioClip clip)
    {
        if (clip == null || audioSourceSFX == null) return;
        audioSourceSFX.PlayOneShot(clip);
    }
}