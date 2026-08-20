using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Igual que ItemImporter pero para EnemigoData. Ver comentarios en
/// ItemImporter.cs para el flujo completo — este script es análogo.
/// </summary>
public static class EnemigoImporter
{
    private const string CARPETA_DESTINO = "Assets/Resources/Enemigos";

    [MenuItem("Idle RPG/Importar Enemigos desde JSON")]
    public static void ImportarEnemigos()
    {
        string rutaJson = EditorUtility.OpenFilePanel("Seleccionar JSON de enemigos", "Assets", "json");
        if (string.IsNullOrEmpty(rutaJson)) return;

        string contenidoJson = File.ReadAllText(rutaJson);
        EnemigoImportWrapper wrapper;

        try
        {
            wrapper = JsonUtility.FromJson<EnemigoImportWrapper>(contenidoJson);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error al parsear JSON",
                $"No se pudo leer el JSON. Verificá que tenga la forma " +
                $"{{ \"enemigos\": [...] }} y que sea válido.\n\nDetalle: {e.Message}",
                "OK");
            return;
        }

        if (wrapper?.enemigos == null || wrapper.enemigos.Length == 0)
        {
            EditorUtility.DisplayDialog("Sin enemigos",
                "El JSON no contenía ningún enemigo bajo la clave \"enemigos\".", "OK");
            return;
        }

        if (!AssetDatabase.IsValidFolder(CARPETA_DESTINO))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateFolder("Assets/Resources", "Enemigos");
        }

        int creados = 0;
        int actualizados = 0;

        foreach (EnemigoImportDTO dto in wrapper.enemigos)
        {
            if (string.IsNullOrEmpty(dto.id))
            {
                Debug.LogWarning("Se encontró un enemigo sin 'id' en el JSON. Se salteó.");
                continue;
            }

            string rutaAsset = $"{CARPETA_DESTINO}/{dto.id}.asset";
            EnemigoData enemigo = AssetDatabase.LoadAssetAtPath<EnemigoData>(rutaAsset);
            bool esNuevo = enemigo == null;

            if (esNuevo)
            {
                enemigo = ScriptableObject.CreateInstance<EnemigoData>();
            }

            enemigo.id = dto.id;
            enemigo.nombreMostrado = dto.nombreMostrado;
            enemigo.vida = dto.vida;
            enemigo.daño = dto.daño;
            enemigo.defensa = dto.defensa;
            enemigo.oroOtorgado = dto.oroOtorgado;
            enemigo.experienciaOtorgada = dto.experienciaOtorgada;
            enemigo.zona = dto.zona;
            enemigo.nivelRecomendado = dto.nivelRecomendado;
            enemigo.esJefe = dto.esJefe;

            if (esNuevo)
            {
                AssetDatabase.CreateAsset(enemigo, rutaAsset);
                creados++;
            }
            else
            {
                EditorUtility.SetDirty(enemigo);
                actualizados++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Importación completa",
            $"Enemigos creados: {creados}\nEnemigos actualizados: {actualizados}\n\n" +
            $"Ubicación: {CARPETA_DESTINO}", "OK");
    }
}
