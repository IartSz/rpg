using UnityEngine;

/// <summary>
/// Define un enemigo del juego. Igual que ItemData, normalmente se
/// genera en lote con el importador en vez de crearse a mano.
/// </summary>
[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "Idle RPG/Enemigo")]
public class EnemigoData : ScriptableObject
{
    [Header("Identificación")]
    public string id;
    public string nombreMostrado;

    [Header("Stats de combate")]
    public double vida;
    public double daño;
    public double defensa;

    [Header("Recompensas al derrotarlo")]
    public double oroOtorgado;
    public double experienciaOtorgada;

    [Header("Contexto")]
    [Tooltip("Zona o capítulo donde aparece, ej: 'Bosque Oscuro', 'Mazmorra 3'")]
    public string zona;
    public int nivelRecomendado;
    public bool esJefe;

    [Header("Visual (opcional, se puede completar después)")]
    public Sprite sprite;
}
