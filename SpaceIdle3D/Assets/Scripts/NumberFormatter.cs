using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FormattingTypes
{
    Iridium,
    IridiumPerSecond,
    BoostDuration,
    DarkElixer,
    Stocks,
    Level,
    Owned,
    Cost,
    Time
}

public class NumberFormatter
{
    private static Dictionary<FormattingTypes, string> formattingDictionary = new Dictionary<FormattingTypes, string>
    {
        { FormattingTypes.Iridium, "0"},
        { FormattingTypes.IridiumPerSecond, "0.0"},
        { FormattingTypes.BoostDuration, "0.0"},
        { FormattingTypes.DarkElixer, "0.000"},
        { FormattingTypes.Stocks, "0.00"},
        { FormattingTypes.Level, "0"},
        { FormattingTypes.Owned, "0"},
        { FormattingTypes.Cost, "0"},
        { FormattingTypes.Time, "0"}
    };

    public static string FormatNumber(double number, FormattingTypes format)
    {
        if(format == FormattingTypes.BoostDuration)
        {
            TimeSpan time = TimeSpan.FromSeconds(number);

            return string.Format("{0:D2}m:{1:D2}s", time.Minutes, time.Seconds);
        }

        if(format == FormattingTypes.Time)
        {
            TimeSpan time = TimeSpan.FromSeconds(number);

            return string.Format("{0:0}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }

        if (number < 1000f)
        {
            return number.ToString(formattingDictionary[format]);
        }

        string[] suffixes = { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "De" };
        int suffixIndex = 0;

        while (number >= 1000f && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000f;
            suffixIndex++;
        }

        string formattedNumber = number.ToString("F2") + suffixes[suffixIndex - 1];
        return formattedNumber;
    }
}
