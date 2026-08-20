using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Importador de contenido generado por IA. Lee un archivo JSON con la
/// forma { "items": [ {...}, {...} ] } y crea/actualiza un ItemData
/// (ScriptableObject) por cada entrada.
///
/// IMPORTANTE: este script debe vivir en una carpeta llamada "Editor"
/// en algún lugar de Assets (ej: Assets/Scripts/Editor/ItemImporter.cs).
/// Unity excluye automáticamente todo lo que está en carpetas "Editor"
/// del build final — es herramienta de desarrollo, no código de juego.
///
/// Uso:
/// 1. Generá el JSON (con Claude, ChatGPT, o a mano) siguiendo el
///    esquema de ItemImportDTO.
/// 2. Guardalo en algún lugar del proyecto, ej: Assets/Data/items.json
/// 3. Menú superior: Idle RPG > Importar Items desde JSON
/// 4. Elegís el archivo, y listo: aparecen los assets en Assets/Resources/Items/
/// </summary>
public static class ItemImporter
{
    private const string CARPETA_DESTINO = "Assets/Resources/Items";

    [MenuItem("Idle RPG/Importar Items desde JSON")]
    public static void ImportarItems()
    {
        string rutaJson = EditorUtility.OpenFilePanel("Seleccionar JSON de items", "Assets", "json");
        if (string.IsNullOrEmpty(rutaJson)) return; // el usuario canceló

        string contenidoJson = File.ReadAllText(rutaJson);
        ItemImportWrapper wrapper;

        try
        {
            wrapper = JsonUtility.FromJson<ItemImportWrapper>(contenidoJson);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error al parsear JSON",
                $"No se pudo leer el JSON. Verificá que tenga la forma " +
                $"{{ \"items\": [...] }} y que sea válido.\n\nDetalle: {e.Message}",
                "OK");
            return;
        }

        if (wrapper?.items == null || wrapper.items.Length == 0)
        {
            EditorUtility.DisplayDialog("Sin items",
                "El JSON no contenía ningún item bajo la clave \"items\".", "OK");
            return;
        }

        if (!AssetDatabase.IsValidFolder(CARPETA_DESTINO))
        {
            CrearCarpetasNecesarias();
        }

        int creados = 0;
        int actualizados = 0;

        foreach (ItemImportDTO dto in wrapper.items)
        {
            if (string.IsNullOrEmpty(dto.id))
            {
                Debug.LogWarning("Se encontró un item sin 'id' en el JSON. Se salteó.");
                continue;
            }

            string rutaAsset = $"{CARPETA_DESTINO}/{dto.id}.asset";
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(rutaAsset);
            bool esNuevo = item == null;

            if (esNuevo)
            {
                item = ScriptableObject.CreateInstance<ItemData>();
            }

            // Copiamos los campos planos directamente.
            item.id = dto.id;
            item.nombreMostrado = dto.nombreMostrado;
            item.descripcion = dto.descripcion;
            item.bonusDaño = dto.bonusDaño;
            item.bonusDañoPorSegundo = dto.bonusDañoPorSegundo;
            item.bonusOroPorSegundo = dto.bonusOroPorSegundo;
            item.costoCompra = dto.costoCompra;
            item.nivelMinimoRequerido = dto.nivelMinimoRequerido;

            // Los enums necesitan conversión manual desde string.
            item.tipo = ParsearEnumSeguro<TipoItem>(dto.tipo, TipoItem.Arma, dto.id, "tipo");
            item.rareza = ParsearEnumSeguro<Rareza>(dto.rareza, Rareza.Comun, dto.id, "rareza");

            if (esNuevo)
            {
                AssetDatabase.CreateAsset(item, rutaAsset);
                creados++;
            }
            else
            {
                EditorUtility.SetDirty(item); // marca el asset existente como modificado
                actualizados++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Importación completa",
            $"Items creados: {creados}\nItems actualizados: {actualizados}\n\n" +
            $"Ubicación: {CARPETA_DESTINO}", "OK");
    }

    /// <summary>
    /// Convierte un string a un valor de enum, con un fallback seguro y
    /// un warning en consola si el valor no coincide con ninguna opción
    /// (esto pasa seguido cuando la IA generó un valor con typo o en
    /// mayúsculas/minúsculas distintas).
    /// </summary>
    private static T ParsearEnumSeguro<T>(string valor, T valorPorDefecto, string idItem, string nombreCampo) where T : struct
    {
        if (Enum.TryParse(valor, ignoreCase: true, out T resultado))
        {
            return resultado;
        }

        Debug.LogWarning($"Item '{idItem}': el valor '{valor}' en el campo '{nombreCampo}' " +
                          $"no coincide con ningún enum válido. Se usó '{valorPorDefecto}' por defecto.");
        return valorPorDefecto;
    }

    private static void CrearCarpetasNecesarias()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        if (!AssetDatabase.IsValidFolder("Assets/Resources/Items"))
            AssetDatabase.CreateFolder("Assets/Resources", "Items");
    }
}
