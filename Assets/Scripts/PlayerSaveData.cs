using System;

/// para ver el estado del jugador que necesita persistencia
[Serializable]
public class PlayerSaveData
{
    // Progresión
    public int nivel = 1;
    public double progresoTotalAcumulado = 0; // usado para calcular prestige

    // Monedas
    public double oro = 0;
    public long monedaPrestige = 0;

    // Timestamp del ultimo guardado, como string
    public string ultimoGuardadoUtc = "";

    // guarda el id del equipamiento del item equipado en cada slot
    public EquippedItems equipados = new EquippedItems();

    // crea un save para un jugador nuevo

    public static PlayerSaveData Nuevo()
    {
        return new PlayerSaveData
        {
            nivel = 1,
            progresoTotalAcumulado = 0,
            oro = 0,
            monedaPrestige = 0,
            ultimoGuardadoUtc = DateTime.UtcNow.ToString("o"),
            equipados = new EquippedItems()
        };
    }
}

/// muestra un slot por cada tipo de item equipable, guarda el id (string) del ItemData equipado, en caso de no tener nada muestra ""
/// los consumibles no cuentan con slot, usarlos se descuentan automaicamente del inventario
[Serializable]
public class EquippedItems
{
    public string armaId = "";
    public string armaduraId = "";
    public string accesorioId = "";
}
