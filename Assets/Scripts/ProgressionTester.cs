using System;
using UnityEngine;

/// <summary>
/// Script de prueba rápida. Creá un GameObject vacío en una escena,
/// pegale este componente, y dale Play. Los resultados aparecen en la
/// Consola (Window > General > Console).
///
/// Objetivo: poder tunear los parámetros de ProgressionFormulas SIN
/// tocar UI ni arte, solo mirando estos logs, hasta que la curva
/// "se sienta bien".
/// </summary>
public class ProgressionTester : MonoBehaviour
{
    [Header("Costo de mejora")]
    public double costoBase = 10;
    public double multiplicadorCosto = 1.15;

    [Header("Stat por nivel")]
    public double statBase = 5;
    public double factorCrecimiento = 0.08;

    [Header("Ganancia pasiva")]
    public double tasaAtaque = 1.0; // ataques por segundo
    public double multiplicadoresActivos = 1.0;

    [Header("Offline")]
    public double topeHorasOffline = 8.0;

    [Header("Prestige")]
    public double divisorPrestige = 1000.0;
    public double factorBonoPrestige = 0.02;

    void Start()
    {
        Debug.Log("===== TABLA DE COSTOS DE MEJORA =====");
        int[] nivelesAProbar = { 1, 5, 10, 25, 50, 100 };
        foreach (int nivel in nivelesAProbar)
        {
            double costo = ProgressionFormulas.CostoMejora(nivel, costoBase, multiplicadorCosto);
            double stat = ProgressionFormulas.StatPorNivel(nivel, statBase, factorCrecimiento);
            Debug.Log($"Nivel {nivel}: Costo = {costo:N0} | Stat (daño) = {stat:N1}");
        }

        Debug.Log("===== GANANCIA POR SEGUNDO (nivel 50 como ejemplo) =====");
        double dañoNivel50 = ProgressionFormulas.StatPorNivel(50, statBase, factorCrecimiento);
        double gps = ProgressionFormulas.GananciaPorSegundo(dañoNivel50, tasaAtaque, multiplicadoresActivos);
        Debug.Log($"Ganancia/seg = {gps:N2}");

        Debug.Log("===== PROGRESO OFFLINE (simulando distintos tiempos) =====");
        double[] horasSimuladas = { 1, 4, 8, 12, 24 };
        foreach (double horas in horasSimuladas)
        {
            double segundos = horas * 3600.0;
            double ganancia = ProgressionFormulas.GananciaOffline(gps, segundos, topeHorasOffline);
            Debug.Log($"Si estuvo offline {horas}h (tope {topeHorasOffline}h) → Ganancia = {ganancia:N0}");
        }

        Debug.Log("===== PRESTIGE (simulando distintos progresos acumulados) =====");
        double[] progresosSimulados = { 1000, 10000, 100000, 1000000 };
        foreach (double progreso in progresosSimulados)
        {
            long moneda = ProgressionFormulas.MonedaPrestige(progreso, divisorPrestige);
            double bono = ProgressionFormulas.BonoPermanentePrestige(moneda, factorBonoPrestige);
            Debug.Log($"Progreso acumulado {progreso:N0} → Moneda prestige = {moneda} | Bono permanente = x{bono:N2}");
        }

        Debug.Log("===== FIN DEL TEST — ajustá los valores en el Inspector y volvé a dar Play =====");
    }
}
