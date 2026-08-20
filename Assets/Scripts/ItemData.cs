using UnityEngine;

/// Definimos cada item del juego (arma, armadura, accesorio, entre otros)
[CreateAssetMenu(fileName = "NuevoItem", menuName = "Idle RPG/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identificación")]
    [Tooltip("ID único, usado para referenciar el item desde código o guardado. No debe repetirse.")]
    public string id;
    public string nombreMostrado;
    [TextArea(2, 4)]
    public string descripcion;

    [Header("Clasificación")]
    public TipoItem tipo;
    public Rareza rareza;

    [Header("Stats")]
    public double bonusDaño;
    public double bonusDañoPorSegundo; // si el item da regen o similar
    public double bonusOroPorSegundo;  // ej: un anillo que suma oro pasivo

    [Header("Economía")]
    public double costoCompra;
    public int nivelMinimoRequerido;

    [Header("Visual (opcional, se puede completar después)")]
    public Sprite icono;
}

//categorias de los items
public enum TipoItem
{
    Arma,
    Armadura,
    Accesorio,
    Consumible
}

/// rareza de los items, el orden si importa ya que se usa para ordenar en la ui
public enum Rareza
{
    Comun,
    PocoComun,
    Raro,
    Epico,
    Legendario
}
