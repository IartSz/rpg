using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Lista todos los items disponibles los cuales fueron cargados por el GameManager
/// siguiendo la ruta de Resources/Items


public class InventoryUIManager : MonoBehaviour
{
    [Header("Referencias (arrastrar desde la jerarquía/proyecto)")]
    [Tooltip("El objeto 'Content' dentro de tu Scroll View, donde se instancian los botones.")]
    public Transform contenedorItems;

    [Tooltip("Prefab de un botón con un TextMeshProUGUI hijo. Se instancia una vez por item.")]
    public Button prefabBotonItem;

    void Start()
    {
        GenerarListaDeItems();
    }

    /// Sirve para borrar los botones actuales y en caso de ser necesario, los crea de nuevo
    /// en caso de querer refrescar aparece marcado como "equipado" en el texto de cada boton
    public void GenerarListaDeItems()
    {
        if (contenedorItems == null || prefabBotonItem == null)
        {
            Debug.LogWarning("InventoryUIManager: falta asignar 'contenedorItems' o 'prefabBotonItem' en el Inspector.");
            return;
        }

        foreach (Transform hijo in contenedorItems)
        {
            Destroy(hijo.gameObject);
        }

        foreach (ItemData item in GameManager.Instance.ObtenerTodosLosItems())
        {
            CrearBotonParaItem(item);
        }
    }

    private void CrearBotonParaItem(ItemData item)
    {
        Button boton = Instantiate(prefabBotonItem, contenedorItems);
        boton.gameObject.SetActive(true);

        bool estaEquipado = EsElItemEquipadoEnSuSlot(item);

        TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
        {
            string sufijoEquipado = estaEquipado ? " (equipado)" : "";
            texto.text = $"{item.nombreMostrado}{sufijoEquipado}\n" +
                         $"[{item.rareza}]\n" +
                         $"Dañ:+{item.bonusDaño:N0}  Oro/s:+{item.bonusOroPorSegundo:N1}  Costo:{item.costoCompra:N0}";
        }

        boton.interactable = !estaEquipado;
        boton.onClick.AddListener(() => OnClickEquipar(item.id));
    }

    private bool EsElItemEquipadoEnSuSlot(ItemData item)
    {
        ItemData equipadoEnEseSlot = GameManager.Instance.ObtenerEquipadoEnSlot(item.tipo);
        return equipadoEnEseSlot != null && equipadoEnEseSlot.id == item.id;
    }

    private void OnClickEquipar(string itemId)
    {
        bool exito = GameManager.Instance.EquiparItem(itemId);
        if (!exito)
        {
            Debug.LogWarning($"No se pudo equipar el item '{itemId}' (¿es de tipo Consumible?).");
            return;
        }


        GenerarListaDeItems();
    }
}