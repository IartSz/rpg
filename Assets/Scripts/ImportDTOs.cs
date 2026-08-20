using System;

//importar items previo JSON
[Serializable]
public class ItemImportDTO
{
    public string id;
    public string nombreMostrado;
    public string descripcion;
    public string tipo;    // "Arma", "Armadura", "Accesorio", "Consumible"
    public string rareza;  // "Comun", "PocoComun", "Raro", "Epico", "Legendario"
    public double bonusDaño;
    public double bonusDañoPorSegundo;
    public double bonusOroPorSegundo;
    public double costoCompra;
    public int nivelMinimoRequerido;
}

[Serializable]
public class ItemImportWrapper
{
    public ItemImportDTO[] items;
}

[Serializable]
public class EnemigoImportDTO
{
    public string id;
    public string nombreMostrado;
    public double vida;
    public double daño;
    public double defensa;
    public double oroOtorgado;
    public double experienciaOtorgada;
    public string zona;
    public int nivelRecomendado;
    public bool esJefe;
}

/// 
/// Wrapper para el JSON de enemigos
/// </summary>
[Serializable]
public class EnemigoImportWrapper
{
    public EnemigoImportDTO[] enemigos;
}
