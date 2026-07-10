using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorBoss : MonoBehaviour
{
    [Header("Configuracion")]
    public int habitacionesParaBoss = 3;

    private int habitacionesCompletadas = 0;

    public void HabitacionCompletada()
    {
        habitacionesCompletadas++;
        Debug.Log($"Habitaciones completadas: {habitacionesCompletadas}/{habitacionesParaBoss}");

        if (habitacionesCompletadas >= habitacionesParaBoss)
        {
            Debug.Log("Cargando escena del boss...");
            SceneManager.LoadScene(1);
        }
    }
}