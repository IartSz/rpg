using System;
using System.IO;
using UnityEngine;

/// Encargado de: cargar/crear un save al iniciar y ademas calcular ganancias offline
/// Actualiza las ganancias pasiva con un timer acumulado
/// Guarda el estado
/// Núcleo del core loop. Se encarga de:

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado actual (solo lectura en Inspector, para debug)")]
    [SerializeField] private PlayerSaveData saveData;

    [Header("Config de progresión (mismos parámetros que ProgressionTester)")]
    public double costoBase = 10;
    public double multiplicadorCosto = 1.15;
    public double statBase = 5;
    public double factorCrecimiento = 0.08;
    public double tasaAtaque = 1.0;
    public double topeHorasOffline = 8.0;
    public double divisorPrestige = 1000.0;
    public double factorBonoPrestige = 0.02;

    [Header("Guardado automático")]
    public float intervaloAutoguardadoSegundos = 30f;

    // Maneja que items existen y muestra lo equipado//
    private EquipmentManager equipmentManager;

    // Timer acumulado para el tick de ganancia pasiva.
    //Acumula el tiempo real y lo procesa en ticks de 1 segundo

    private float acumuladorTick = 0f;
    private float acumuladorAutoguardado = 0f;
    private const float TICK_SEGUNDOS = 1f;

    private string RutaGuardado => Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        // Singleton simple. Si ya existe uno, destruye este duplicado.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        equipmentManager = new EquipmentManager();
    }

    void Start()
    {
        CargarOCrear();
        AplicarGananciaOffline();
    }

    void Update()
    {
        //Tick de ganancia pasiva, cada 1 segundo real//
        acumuladorTick += Time.deltaTime;
        while (acumuladorTick >= TICK_SEGUNDOS)
        {
            acumuladorTick -= TICK_SEGUNDOS;
            ProcesarTick(TICK_SEGUNDOS);
        }

        //Simple autoguardado
        acumuladorAutoguardado += Time.deltaTime;
        if (acumuladorAutoguardado >= intervaloAutoguardadoSegundos)
        {
            acumuladorAutoguardado = 0f;
            Guardar();
        }
    }

    void OnApplicationPause(bool pausado)
    {
 
        if (pausado)
        {
            Guardar();
        }
    }

    void OnApplicationQuit()
    {
        Guardar();
    }

    // Calculo de daño y ganancia (esto incluye los bonos de equipamiento)

    private double CalcularDañoTotal()
    {
        double dañoBase = ProgressionFormulas.StatPorNivel(saveData.nivel, statBase, factorCrecimiento);
        double bonusEquipo = equipmentManager.ObtenerBonusDañoTotal(saveData.equipados);
        return dañoBase + bonusEquipo;
    }

    // Ganancia de oro por segundo que pasa, ocupa formula base (daño * tasa de ataque) además los bonos de daño por segudo y oro por segundo que te pueden otorgar los items
    // y finalmente suma el oro pasivo si necesidad de pasar por combate)
    private double CalcularGananciaPorSegundoTotal()
    {
        double dañoTotal = CalcularDañoTotal();
        double gananciaBase = ProgressionFormulas.GananciaPorSegundo(dañoTotal, tasaAtaque);

        double bonusDañoPorSegundo = equipmentManager.ObtenerBonusDañoPorSegundoTotal(saveData.equipados);
        double bonusOroPorSegundo = equipmentManager.ObtenerBonusOroPorSegundoTotal(saveData.equipados);

        return gananciaBase + bonusDañoPorSegundo + bonusOroPorSegundo;
    }

    // Tick de ganancia pasiva
    private void ProcesarTick(float segundosDelTick)
    {
        double gananciaPorSegundo = CalcularGananciaPorSegundoTotal();

        double ganado = gananciaPorSegundo * segundosDelTick;
        saveData.oro += ganado;
        saveData.progresoTotalAcumulado += ganado;

    }

    // Metodo para intentar subir de nivel si es que el jugador posee el oro suficiente
    // en caso de que pueda devuelve un true

    public bool IntentarMejorarNivel()
    {
        double costo = ProgressionFormulas.CostoMejora(saveData.nivel, costoBase, multiplicadorCosto);
        if (saveData.oro < costo) return false;

        saveData.oro -= costo;
        saveData.nivel++;
        return true;
    }

    // Prestigio (ta bug)
    // Resetea el nivel, y convierte el progreso acumulado en moneda permanente
    // y da un bono por prestigio

    public void EjecutarPrestige()
    {
        long monedaGanada = ProgressionFormulas.MonedaPrestige(saveData.progresoTotalAcumulado, divisorPrestige);
        saveData.monedaPrestige += monedaGanada;

        saveData.nivel = 1;
        saveData.oro = 0;
       

        Guardar();
    }

    public double ObtenerBonoPermanentePrestige()
    {
        return ProgressionFormulas.BonoPermanentePrestige(saveData.monedaPrestige, factorBonoPrestige);
    }

    // Para guardar y cargar partidas
    private void CargarOCrear()
    {
        if (File.Exists(RutaGuardado))
        {
            string json = File.ReadAllText(RutaGuardado);
            saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            Debug.Log($"Save cargado desde {RutaGuardado}");
        }
        else
        {
            saveData = PlayerSaveData.Nuevo();
            Debug.Log("No había save previo. Se creó uno nuevo.");
        }
    }

    public void Guardar()
    {
        saveData.ultimoGuardadoUtc = DateTime.UtcNow.ToString("o");
        string json = JsonUtility.ToJson(saveData, prettyPrint: true);
        File.WriteAllText(RutaGuardado, json);
        Debug.Log($"Guardado en {RutaGuardado}");
    }

    // Metodo para calcular ganancia offline
    private void AplicarGananciaOffline()
    {
        if (string.IsNullOrEmpty(saveData.ultimoGuardadoUtc)) return;

        DateTime ultimoGuardado = DateTime.Parse(
            saveData.ultimoGuardadoUtc,
            null,
            System.Globalization.DateTimeStyles.RoundtripKind
        );

        double segundosTranscurridos = ProgressionFormulas.SegundosDesdeUltimoGuardado(ultimoGuardado);

        if (segundosTranscurridos < 5) return;

        double gananciaPorSegundo = CalcularGananciaPorSegundoTotal();

        double gananciaOffline = ProgressionFormulas.GananciaOffline(
            gananciaPorSegundo, segundosTranscurridos, topeHorasOffline
        );

        saveData.oro += gananciaOffline;
        saveData.progresoTotalAcumulado += gananciaOffline;

        double horasReales = segundosTranscurridos / 3600.0;
        Debug.Log($"Bienvenido de vuelta. Estuviste offline {horasReales:N1}h " +
                  $"(tope {topeHorasOffline}h) → Ganancia offline: {gananciaOffline:N0} oro");

    }

    // Equipamiento sirve para que la UI del inventario los muestre
    // además muestra todos los items
    public System.Collections.Generic.IEnumerable<ItemData> ObtenerTodosLosItems()
    {
        return equipmentManager.ObtenerTodosLosItems();
    }

    /// metodo para intentar equipar items por id, en caso de que tiene exito devuelve un true
    public bool EquiparItem(string itemId)
    {
        bool exito = equipmentManager.EquiparItem(itemId, saveData.equipados);
        if (exito) Guardar();
        return exito;
    }

    /// Muestra el item actualmente equipado en cada slot, en caso de no tener nada devuelve un null
    public ItemData ObtenerEquipadoEnSlot(TipoItem tipo)
    {
        return equipmentManager.ObtenerEquipadoEnSlot(tipo, saveData.equipados);
    }

    // getters para que la ui pueda leer el estado 
    public int Nivel => saveData.nivel;
    public double Oro => saveData.oro;
    public long MonedaPrestige => saveData.monedaPrestige;
    public double ProximoCosto => ProgressionFormulas.CostoMejora(saveData.nivel, costoBase, multiplicadorCosto);
}