using System.Collections.Generic;
using UnityEngine;

// Encargado de, calcular los bonos que se otorgan al equipar los items, y además carga los item disponible//
public class EquipmentManager
{
    private readonly Dictionary<string, ItemData> itemsPorId = new Dictionary<string, ItemData>();

    public EquipmentManager()
    {
        CargarTodosLosItems();
    }

    private void CargarTodosLosItems()
    {
        itemsPorId.Clear();
        ItemData[] items = Resources.LoadAll<ItemData>("Items");

        foreach (ItemData item in items)
        {
            if (string.IsNullOrEmpty(item.id))
            {
                Debug.LogWarning($"Item '{item.name}' no tiene 'id' asignado. Se ignoró.");
                continue;
            }

            if (itemsPorId.ContainsKey(item.id))
            {
                Debug.LogWarning($"Hay dos items con el mismo id '{item.id}'. Se usó el primero encontrado.");
                continue;
            }

            itemsPorId[item.id] = item;
        }

        Debug.Log($"EquipmentManager: {itemsPorId.Count} items cargados desde Resources/Items.");
    }

    // Obtiene todos los items creados actualmente//
    public IEnumerable<ItemData> ObtenerTodosLosItems()
    {
        return itemsPorId.Values;
    }

    // Encargado de buscar los items por id, en caso de no encontrar nada devuelve un null//
    public ItemData ObtenerItemPorId(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        itemsPorId.TryGetValue(id, out ItemData item);
        return item;
    }

    // Encargado de mostrar el item equipado en su slot correspondiente al tipo, en caso de no encontrar nadad marca null//
    public ItemData ObtenerEquipadoEnSlot(TipoItem tipo, EquippedItems equipados)
    {
        string id = tipo switch
        {
            TipoItem.Arma => equipados.armaId,
            TipoItem.Armadura => equipados.armaduraId,
            TipoItem.Accesorio => equipados.accesorioId,
            _ => null
        };
        return ObtenerItemPorId(id);
    }

    // Intenta equipar items en los slots existentes, en caso de que el item no exista o no es de su tipo devuelve false
    public bool EquiparItem(string itemId, EquippedItems equipados)
    {
        ItemData item = ObtenerItemPorId(itemId);
        if (item == null) return false;

        switch (item.tipo)
        {
            case TipoItem.Arma:
                equipados.armaId = itemId;
                return true;
            case TipoItem.Armadura:
                equipados.armaduraId = itemId;
                return true;
            case TipoItem.Accesorio:
                equipados.accesorioId = itemId;
                return true;
            default:
                return false;
        }
    }

    // ------------------------------------------------------------------
    // Calculo de bonos totales (suma de los 3 slots equipados)
    // ------------------------------------------------------------------

    public double ObtenerBonusDañoTotal(EquippedItems equipados)
    {
        double total = 0;
        total += ObtenerItemPorId(equipados.armaId)?.bonusDaño ?? 0;
        total += ObtenerItemPorId(equipados.armaduraId)?.bonusDaño ?? 0;
        total += ObtenerItemPorId(equipados.accesorioId)?.bonusDaño ?? 0;
        return total;
    }

    public double ObtenerBonusDañoPorSegundoTotal(EquippedItems equipados)
    {
        double total = 0;
        total += ObtenerItemPorId(equipados.armaId)?.bonusDañoPorSegundo ?? 0;
        total += ObtenerItemPorId(equipados.armaduraId)?.bonusDañoPorSegundo ?? 0;
        total += ObtenerItemPorId(equipados.accesorioId)?.bonusDañoPorSegundo ?? 0;
        return total;
    }

    public double ObtenerBonusOroPorSegundoTotal(EquippedItems equipados)
    {
        double total = 0;
        total += ObtenerItemPorId(equipados.armaId)?.bonusOroPorSegundo ?? 0;
        total += ObtenerItemPorId(equipados.armaduraId)?.bonusOroPorSegundo ?? 0;
        total += ObtenerItemPorId(equipados.accesorioId)?.bonusOroPorSegundo ?? 0;
        return total;
    }
}
