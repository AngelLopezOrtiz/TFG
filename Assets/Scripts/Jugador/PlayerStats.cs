using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales;

    [Header("Combate")]
    public float danio = 10f;
    public float defensa = 5f;

    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Puntos")]
    public int puntos = 0;

    [Header("Mejoras acumuladas")]
    public int nivelDanio = 0;
    public int nivelDefensa = 0;
    public int nivelVelocidad = 0;

    [Header("Revivir")]
    public int revivesMaximos = 0;
    public int revivesRestantes = 0;

    private static bool juegoIniciado = false;

    void Awake()
    {
        vidasActuales = vidasMaximas;

        if (!juegoIniciado)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("Puntos", 150);
            PlayerPrefs.SetFloat("Danio", 10f);
            PlayerPrefs.SetFloat("Defensa", 5f);
            PlayerPrefs.SetFloat("Velocidad", 5f);
            PlayerPrefs.SetInt("NivelDanio", 0);
            PlayerPrefs.SetInt("NivelDefensa", 0);
            PlayerPrefs.SetInt("NivelVelocidad", 0);
            PlayerPrefs.SetInt("RevivesMaximos", 0);
            PlayerPrefs.SetInt("DashDesbloqueado", 0);
            PlayerPrefs.Save();
            juegoIniciado = true;
        }

        // Siempre cargar los stats actuales desde PlayerPrefs (persisten entre escenas)
        puntos = PlayerPrefs.GetInt("Puntos", 0);
        danio = PlayerPrefs.GetFloat("Danio", 10f);
        defensa = PlayerPrefs.GetFloat("Defensa", 5f);
        velocidad = PlayerPrefs.GetFloat("Velocidad", 5f);
        nivelDanio = PlayerPrefs.GetInt("NivelDanio", 0);
        nivelDefensa = PlayerPrefs.GetInt("NivelDefensa", 0);
        nivelVelocidad = PlayerPrefs.GetInt("NivelVelocidad", 0);
        revivesMaximos = PlayerPrefs.GetInt("RevivesMaximos", 0);
        revivesRestantes = revivesMaximos;

        Debug.Log($"Stats cargados - Puntos: {puntos}, Daño: {danio}, Defensa: {defensa}, Velocidad: {velocidad}, Revives: {revivesMaximos}");
    }

    public void RecibirDanio(float cantidad)
    {
        ParpadeoJugador parpadeo = GetComponent<ParpadeoJugador>();
        Debug.Log($"RecibirDanio. Invulnerable: {parpadeo?.EsInvulnerable()}, Time: {Time.time}");
        if (parpadeo != null && parpadeo.EsInvulnerable()) return;

        vidasActuales--;
        Debug.Log($"Vidas restantes: {vidasActuales}");

        parpadeo?.Parpadear();

        if (vidasActuales <= 0)
        {
            if (revivesRestantes > 0)
            {
                revivesRestantes--;
                vidasActuales = 1;
                Debug.Log($"Revivido! Revives restantes: {revivesRestantes}");
                parpadeo?.Parpadear();
                SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoDanioJugador);
            }
            else
            {
                Morir();
            }
        }
        else
        {
            SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoDanioJugador);
        }
    }

    public void Curar()
    {
        if (vidasActuales < vidasMaximas)
            vidasActuales++;
    }

    void Morir()
    {
        Debug.Log("El jugador ha muerto");
        SonidoManager.Instancia?.ReproducirSonido(SonidoManager.Instancia.sonidoMuerteJugador);
        SonidoManager.Instancia?.PararMusica();
        PlayerPrefs.SetInt("Puntos", puntos);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AnadirPuntos(int cantidad)
    {
        puntos += cantidad;
        PlayerPrefs.SetInt("Puntos", puntos);
        PlayerPrefs.Save();
        Debug.Log($"Puntos guardados: {puntos}");
    }

    public void MejorarStat(string stat, float cantidad)
    {
        switch (stat)
        {
            case "danio":
                danio += cantidad;
                nivelDanio++;
                PlayerPrefs.SetFloat("Danio", danio);
                PlayerPrefs.SetInt("NivelDanio", nivelDanio);
                break;
            case "defensa":
                defensa += cantidad;
                nivelDefensa++;
                PlayerPrefs.SetFloat("Defensa", defensa);
                PlayerPrefs.SetInt("NivelDefensa", nivelDefensa);
                break;
            case "velocidad":
                velocidad += cantidad;
                nivelVelocidad++;
                PlayerPrefs.SetFloat("Velocidad", velocidad);
                PlayerPrefs.SetInt("NivelVelocidad", nivelVelocidad);
                break;
            case "revivir":
                revivesMaximos++;
                revivesRestantes++;
                PlayerPrefs.SetInt("RevivesMaximos", revivesMaximos);
                break;
            case "tiempo":
                {
                    TiempoRun tiempo = FindObjectOfType<TiempoRun>();
                    if (tiempo != null) tiempo.AnadirTiempo(cantidad);
                    break;
                }
            case "dash":
                PlayerPrefs.SetInt("DashDesbloqueado", 1);
                break;
        }
        PlayerPrefs.Save();
    }
}