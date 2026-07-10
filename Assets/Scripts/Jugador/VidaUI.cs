using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [Header("Sprites de vidas")]
    public Sprite sprite0Vidas;
    public Sprite sprite1Vida;
    public Sprite sprite2Vidas;
    public Sprite sprite3Vidas;

    [Header("Referencias")]
    public Image imagenVidas;
    public PlayerStats playerStats;

    void Update()
    {
        Debug.Log($"Vidas actuales: {playerStats.vidasActuales}");
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        switch (playerStats.vidasActuales)
        {
            case 0:
                imagenVidas.sprite = sprite0Vidas;
                break;
            case 1:
                imagenVidas.sprite = sprite1Vida;
                break;
            case 2:
                imagenVidas.sprite = sprite2Vidas;
                break;
            case 3:
                imagenVidas.sprite = sprite3Vidas;
                break;
        }
    }
}