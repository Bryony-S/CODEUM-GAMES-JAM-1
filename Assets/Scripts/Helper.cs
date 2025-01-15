using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static class Helper
{
    public static Random random = new Random();
    public static int difficultyModifier = 100;

    /// <summary>
    /// Whether a random event happens or not based on it's chance.
    /// </summary>
    /// <param name="chanceRates">All the rates of chance (in percent) that apply to the event.</param>
    /// <returns>Whether the event happens or not.</returns>
    public static bool ChanceEvent(List<int> chanceRates)
    {
        float chanceRate = 1f;
        foreach (int cr in chanceRates) chanceRate *= (float)cr / 100;
        chanceRate *= (float)difficultyModifier / 100;
        if (chanceRate * 100 >= random.Next(1, 100)) return true;
        return false;
    }

    /// <summary>
    /// Normalizes looping integer values.
    /// Source: https://stackoverflow.com/a/61572168
    /// </summary>
    /// <param name="value">The integer value to be normalized.</param>
    /// <param name="modulo">The length of the loop.</param>
    /// <returns>The normalized integer value.</returns>
    public static int NormalizeLoopingInt(int value, int modulo)
    {
        int remainder = value % modulo;
        return (remainder < 0) ? (modulo + remainder) : remainder;
    }
}
