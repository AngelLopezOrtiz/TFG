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
            danio = 10f;
            defensa = 5f;
            velocidad = 5f;
            revivesMaximos = 0;
            revivesRestantes = 0;
            nivelDanio = 0;
            nivelDefensa = 0;
            nivelVelocidad = 0;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("Puntos", 150);
            PlayerPrefs.Save();
            juegoIniciado = true;
        }

        // Siempre cargar los puntos actuales de PlayerPrefs
        puntos = PlayerPrefs.GetInt("Puntos", 0);

        revivesRestantes = revivesMaximos;
        Debug.Log($"Puntos cargados al inicio: {puntos}");
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
            }
            else
            {
                Morir();
            }
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
                break;
            case "defensa":
                defensa += cantidad;
                nivelDefensa++;
                break;
            case "velocidad":
                velocidad += cantidad;
                nivelVelocidad++;
                break;
            case "revivir":
                revivesMaximos++;
                revivesRestantes++;
                break;
            case "tiempo":
                {
                    TiempoRun tiempo = FindObjectOfType<TiempoRun>();
                    if (tiempo != null) tiempo.AnadirTiempo(cantidad);
                    break;
                }
            case "dash":
                break;
        }
    }
}