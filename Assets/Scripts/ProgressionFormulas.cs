using System;
using UnityEngine;

/// Formulas base de progresion

public static class ProgressionFormulas
{
    // Costo de mejora para subir un stat/nivel
    // crece de forma exponencial
    public static double CostoMejora(int nivel, double costoBase = 10, double multiplicador = 1.15)
    {
        return costoBase * Math.Pow(multiplicador, nivel);
    }

    // Poder del stat por nivel
    // este crece en funcion del nivel, tiene un crecimiento lineal
    // se combina con multiplicadores de equipo con prestigio
    public static double StatPorNivel(int nivel, double statBase = 5, double factorCrecimiento = 0.08)
    {
        return statBase * (1 + factorCrecimiento * nivel);
    }

    // ganancia pasiva por segundo
    // metodo para calcular la ganancia de recursos por segundo, esto incluye los multiplicadores de bonos etc
    public static double GananciaPorSegundo(double dañoPromedio, double tasaAtaque, double multiplicadores = 1.0)
    {
        return dañoPromedio * tasaAtaque * multiplicadores;
    }

    // Progreso offline
    // ganancia acumulada mientras el jugador estuvo offline, respetando un tope maximo de horas

    public static double GananciaOffline(double gananciaPorSegundo, double segundosTranscurridos, double topeHorasOffline = 8.0)
    {
        double topeSegundos = topeHorasOffline * 3600.0;
        double segundosEfectivos = Math.Min(segundosTranscurridos, topeSegundos);
        return gananciaPorSegundo * segundosEfectivos;
    }

    /// metodo para calcular los segundos transcurridos entre el ultimo guardado
    /// </summary>
    public static double SegundosDesdeUltimoGuardado(DateTime ultimoGuardadoUtc)
    {
        return (DateTime.UtcNow - ultimoGuardadoUtc).TotalSeconds;
    }

    // Prestigio (viendo si utilizar)
    // otorga una moneda al resetear
    public static long MonedaPrestige(double progresoTotalAcumulado, double divisorPrestige = 1000.0)
    {
        return (long)Math.Floor(Math.Sqrt(progresoTotalAcumulado / divisorPrestige));
    }

    /// bono permanente por prestigio
    /// </summary>
    public static double BonoPermanentePrestige(long monedaPrestigeTotal, double factorBonoPrestige = 0.02)
    {
        return 1.0 + (monedaPrestigeTotal * factorBonoPrestige);
    }
}
