using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.SceneManagement;

public class GeneradorHabitacion : MonoBehaviour
{
    [Header("Prefabs enemigos")]
    public GameObject prefabMelee;
    public GameObject prefabRango;
    public GameObject prefabEscudo;
    public GameObject prefabArana;

    [Header("Spawn")]
    public float distanciaMinEntreEnemigos = 3f;
    public float distanciaMinAlJugador = 5f;

    [Header("Escala enemigos")]
    public float escalaEnemigo = 2f;

    [Header("Habitacion")]
    public int numeroHabitacion = 1;

    [Header("Composicion de la oleada")]
    public int cantidadMagos = 3;
    public int cantidadMelee = 0;
    public int cantidadEscudos = 1;
    public int habitacionMinEscudo = 2;
    public int cantidadAranas = 1;
    public int habitacionMinArana = 3;

    [Header("Boss")]
    public int habitacionesParaBoss = 3;

    private int habitacionesCompletadas = 0;
    private List<GameObject> enemigosActuales = new List<GameObject>();
    private bool habitacionCompletada = false;
    private bool oleadaActiva = false;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        if (habitacionCompletada) return;
        if (!oleadaActiva) return;
        ComprobarOleada();
    }

    private void ComprobarOleada()
    {
        enemigosActuales.RemoveAll(e => e == null);

        if (enemigosActuales.Count == 0)
            CompletarHabitacion();
    }

    public void IniciarOleada(int numeroOleada)
    {
        LimpiarHabitacion();
        habitacionCompletada = false;
        oleadaActiva = true;

        List<GameObject> prefabsASpawnear = new List<GameObject>();

        for (int i = 0; i < cantidadMagos; i++)
            if (prefabRango != null) prefabsASpawnear.Add(prefabRango);

        for (int i = 0; i < cantidadMelee; i++)
            if (prefabMelee != null) prefabsASpawnear.Add(prefabMelee);

        if (numeroHabitacion >= habitacionMinEscudo)
            for (int i = 0; i < cantidadEscudos; i++)
                if (prefabEscudo != null) prefabsASpawnear.Add(prefabEscudo);

        if (numeroHabitacion >= habitacionMinArana)
            for (int i = 0; i < cantidadAranas; i++)
                if (prefabArana != null) prefabsASpawnear.Add(prefabArana);

        int totalEnemigos = prefabsASpawnear.Count;
        List<Vector3> posicionesElegidas = ObtenerPosicionesValidas(totalEnemigos);

        for (int i = 0; i < posicionesElegidas.Count; i++)
        {
            GameObject enemigo = Instantiate(prefabsASpawnear[i], posicionesElegidas[i], Quaternion.identity);
            enemigo.transform.localScale = new Vector3(escalaEnemigo, 1f, 1f);

            EnemyMago mago = enemigo.GetComponent<EnemyMago>();
            if (mago != null) mago.enabled = true;

            EnemyAlquimista alquimista = enemigo.GetComponent<EnemyAlquimista>();
            if (alquimista != null) alquimista.enabled = true;

            EnemyArana arana = enemigo.GetComponent<EnemyArana>();
            if (arana != null) arana.enabled = true;

            enemigosActuales.Add(enemigo);
        }

        Debug.Log($"Habitación {numeroHabitacion} — Oleada iniciada con {posicionesElegidas.Count} enemigos " +
                  $"({cantidadMagos} magos, {cantidadMelee} melee, " +
                  $"{(numeroHabitacion >= habitacionMinEscudo ? cantidadEscudos : 0)} escudos, " +
                  $"{(numeroHabitacion >= habitacionMinArana ? cantidadAranas : 0)} arañas).");
    }

    private List<Vector3> ObtenerPosicionesValidas(int cantidad)
    {
        List<Vector3> posicionesElegidas = new List<Vector3>();
        List<GraphNode> nodosTransitables = new List<GraphNode>();

        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Jugador");
        Vector3 posJugador = jugadorObj != null ? jugadorObj.transform.position : Vector3.zero;

        AstarPath.active.data.gridGraph.GetNodes(node =>
        {
            if (node.Walkable)
                nodosTransitables.Add(node);
        });

        for (int i = 0; i < nodosTransitables.Count; i++)
        {
            int randomIndex = Random.Range(i, nodosTransitables.Count);
            GraphNode temp = nodosTransitables[i];
            nodosTransitables[i] = nodosTransitables[randomIndex];
            nodosTransitables[randomIndex] = temp;
        }

        foreach (GraphNode nodo in nodosTransitables)
        {
            if (posicionesElegidas.Count >= cantidad) break;

            Vector3 pos = (Vector3)nodo.position;

            if (Vector3.Distance(pos, posJugador) < distanciaMinAlJugador)
                continue;

            bool demasiadoCerca = false;
            foreach (Vector3 posElegida in posicionesElegidas)
            {
                if (Vector3.Distance(pos, posElegida) < distanciaMinEntreEnemigos)
                {
                    demasiadoCerca = true;
                    break;
                }
            }

            if (!demasiadoCerca)
                posicionesElegidas.Add(pos);
        }

        return posicionesElegidas;
    }

    private void CompletarHabitacion()
    {
        habitacionCompletada = true;
        oleadaActiva = false;

        habitacionesCompletadas++;
        int puntosGanados = CalcularPuntos();
        playerStats?.AnadirPuntos(puntosGanados);

        Debug.Log($"Habitacion {numeroHabitacion} completada. Puntos ganados: {puntosGanados}. Total completadas: {habitacionesCompletadas}/{habitacionesParaBoss}");

        if (habitacionesCompletadas >= habitacionesParaBoss)
        {
            Debug.Log("Cargando escena del boss...");
            SceneManager.LoadScene(1);
        }
        else
        {
            ControlLibro controlLibro = FindObjectOfType<ControlLibro>();
            if (controlLibro != null)
                controlLibro.MostrarLibro();
        }
    }

    private int CalcularPuntos()
    {
        return 10 + (numeroHabitacion * 5);
    }

    private void LimpiarHabitacion()
    {
        foreach (GameObject enemigo in enemigosActuales)
        {
            if (enemigo != null)
                Destroy(enemigo);
        }
        enemigosActuales.Clear();
    }
}