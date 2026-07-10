using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string nombre;
        public string descripcion;
        public string stat;
        public float cantidadMejora;
        public int nivelMaximo = 2;
        public int[] precios;
        public GameObject objetoMundo;

        [HideInInspector] public int nivelActual = 0;
    }

    [Header("Items de la tienda")]
    public ShopItem[] items;

    [Header("UI Referencias")]
    public GameObject[] itemRows;
    public TextMeshProUGUI[] txtNombres;
    public TextMeshProUGUI[] txtPrecios;
    public Button[] btnComprar;
    public TextMeshProUGUI txtDescripcion;
    public TextMeshProUGUI txtMonedas;
    public Button btnArriba;
    public Button btnAbajo;

    [Header("Jugador")]
    public PlayerStats playerStats;

    private int offset = 0;
    private int monedas = 0;

    void Awake()
{
    monedas = PlayerPrefs.GetInt("Puntos", 0);
    CargarNiveles();
    ActualizarUI();
    if (txtDescripcion != null) txtDescripcion.text = "";  // <-- añade esto
}

    void CargarNiveles()
    {
        // Ocultar todos los objetos de mundo al inicio
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].objetoMundo != null)
                items[i].objetoMundo.SetActive(false);
        }

        // Reactivar solo los que ya fueron comprados
        for (int i = 0; i < items.Length; i++)
        {
            int nivelGuardado = PlayerPrefs.GetInt($"Item_{i}_Nivel", 0);
            items[i].nivelActual = nivelGuardado;

            for (int n = 0; n < nivelGuardado; n++)
            {
                if (playerStats != null)
                    playerStats.MejorarStat(items[i].stat, items[i].cantidadMejora);

                if (items[i].stat == "dash" && n == 0)
                {
                    Dash dash = playerStats?.GetComponent<Dash>();
                    if (dash != null) dash.DesbloquearDash();
                }

                if (items[i].objetoMundo != null && n == 0)
                    items[i].objetoMundo.SetActive(true);
            }
        }
    }

    void GuardarNiveles()
    {
        for (int i = 0; i < items.Length; i++)
        {
            PlayerPrefs.SetInt($"Item_{i}_Nivel", items[i].nivelActual);
        }
        PlayerPrefs.Save();
    }

    public void SubirLista()
    {
        if (offset > 0) { offset--; ActualizarUI(); }
    }

    public void BajarLista()
    {
        if (offset + itemRows.Length < items.Length) { offset++; ActualizarUI(); }
    }

    void ActualizarUI()
    {
        monedas = PlayerPrefs.GetInt("Puntos", 0);

        txtMonedas.text = monedas.ToString();
        btnArriba.interactable = offset > 0;
        btnAbajo.interactable = offset + itemRows.Length < items.Length;

        for (int i = 0; i < itemRows.Length; i++)
        {
            int idx = offset + i;
            bool visible = idx < items.Length;
            itemRows[i].SetActive(visible);

            if (!visible) continue;

            ShopItem item = items[idx];
            bool maxNivel = item.nivelActual >= item.nivelMaximo;
            int precioActual = !maxNivel ? item.precios[item.nivelActual] : 0;
            bool puedeComprar = !maxNivel && monedas >= precioActual;

            txtNombres[i].text = item.nombre + $" (Nv {item.nivelActual}/{item.nivelMaximo})";
            txtPrecios[i].text = maxNivel ? "MAX" : precioActual + " ";

            int captura = idx;
            btnComprar[i].interactable = puedeComprar;
            btnComprar[i].onClick.RemoveAllListeners();
            btnComprar[i].onClick.AddListener(() => Comprar(captura));

            // Añadimos listener al panel para mostrar descripcion al hacer clic
            Button btnFila = itemRows[i].GetComponent<Button>();
            if (btnFila != null)
            {
                btnFila.onClick.RemoveAllListeners();
                btnFila.onClick.AddListener(() => MostrarDescripcion(captura));
            }
        }
    }

    public void MostrarDescripcion(int idx)
    {
        if (idx < items.Length)
            txtDescripcion.text = items[idx].descripcion;
    }

    void Comprar(int idx)
    {
        ShopItem item = items[idx];
        if (item.nivelActual >= item.nivelMaximo) return;

        int precio = item.precios[item.nivelActual];
        if (monedas < precio) return;

        monedas -= precio;
        PlayerPrefs.SetInt("Puntos", monedas);
        if (playerStats != null) playerStats.puntos = monedas;

        item.nivelActual++;

        if (playerStats != null)
        {
            playerStats.MejorarStat(item.stat, item.cantidadMejora);

            if (item.stat == "dash")
            {
                Dash dash = playerStats.GetComponent<Dash>();
                if (dash != null) dash.DesbloquearDash();
            }
        }

        if (item.objetoMundo != null && item.nivelActual == 1)
            item.objetoMundo.SetActive(true);

        txtDescripcion.text = item.descripcion;
        GuardarNiveles();
        ActualizarUI();
    }
}