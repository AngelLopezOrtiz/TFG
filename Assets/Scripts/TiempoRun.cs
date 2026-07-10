using UnityEngine;
using TMPro;

public class TiempoRun : MonoBehaviour
{
    [Header("Tiempo")]
    public float tiempoBase = 90f;
    public float tiempoActual;

    [Header("UI")]
    public TextMeshProUGUI txtTiempo;

    private bool corriendo = false;
    private PlayerStats playerStats;

    void Start()
    {
        tiempoActual = tiempoBase;
        corriendo = false;
        playerStats = FindObjectOfType<PlayerStats>();
        ActualizarUI();
    }

    void Update()
    {
        Debug.Log($"corriendo: {corriendo}, tiempo: {tiempoActual}");

        if (!corriendo) return;

        tiempoActual -= Time.deltaTime;

        if (tiempoActual <= 0)
        {
            tiempoActual = 0;
            corriendo = false;
            playerStats?.RecibirDanio(999f);
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (txtTiempo == null) return;
        int minutos = Mathf.FloorToInt(tiempoActual / 60);
        int segundos = Mathf.FloorToInt(tiempoActual % 60);
        txtTiempo.text = $"{minutos}:{segundos:00}";
    }

    public void AnadirTiempo(float segundos)
    {
        tiempoActual += segundos;
    }

    public void PararTiempo()
    {
        corriendo = false;
    }

    public void ReanudarTiempo()
    {
        corriendo = true;
    }
}