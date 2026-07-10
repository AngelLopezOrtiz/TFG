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

    [Header("Música")]
    public AudioClip musicaFondo;

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

    // Reproduce un efecto de sonido (una sola vez)
    public void ReproducirSonido(AudioClip clip)
    {
        if (clip == null || audioSourceSFX == null) return;
        audioSourceSFX.PlayOneShot(clip);
    }

    // Reproduce música en bucle
    public void ReproducirMusica(AudioClip clip, bool enBucle = true)
    {
        if (clip == null || audioSourceMusica == null) return;

        audioSourceMusica.clip = clip;
        audioSourceMusica.loop = enBucle;
        audioSourceMusica.Play();
    }

    // Detiene la música
    public void PararMusica()
    {
        if (audioSourceMusica != null)
            audioSourceMusica.Stop();
    }
}