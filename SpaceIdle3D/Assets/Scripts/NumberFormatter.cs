using System;
using System.Collections.Generic;

public enum FormattingTypes
{
    Iridium,
    IridiumCost,
    IridiumPerSecond,
    DarkElixir,
    DarkElixirCost,
    DarkElixirPerSecond,
    Cosmium,
    CosmiumCost,
    BoostMultiplier,
    BoostTimer,
    Stocks,
    StocksPrice,
    StocksTimer,
    AFKTime,
    Level,
    Owned,
}

public class NumberFormatter
{
    private static Dictionary<FormattingTypes, string> formattingDictionary = new Dictionary<FormattingTypes, string>
    {
        { FormattingTypes.Iridium, "0"},
        { FormattingTypes.IridiumCost, "0"},
        { FormattingTypes.IridiumPerSecond, "0.0"},

        { FormattingTypes.DarkElixir, "0.000"},
        { FormattingTypes.DarkElixirCost, "0.000"},
        { FormattingTypes.DarkElixirPerSecond, "0.000"},

        { FormattingTypes.Cosmium, "0.00"},
        { FormattingTypes.CosmiumCost, "0.00"},

        { FormattingTypes.BoostMultiplier, "0.0"},

        { FormattingTypes.Stocks, "0"},
        { FormattingTypes.StocksPrice, "0.00"},

        { FormattingTypes.Level, "0"},
        { FormattingTypes.Owned, "0"},
    };

    public static string FormatNumber(double number, FormattingTypes format)
    {
        if (format == FormattingTypes.BoostTimer)
        {
            TimeSpan time = TimeSpan.FromSeconds(number);

            if (time.Hours > 0)
            {
                return string.Format("{0:0}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
            }
            else
            {
                if (time.Minutes > 0)
                {
                    return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
                }
                else
                {
                    return string.Format("{0:D2}", time.Seconds);
                }
            }

        }
        else if (format == FormattingTypes.StocksTimer)
        {
            TimeSpan time = TimeSpan.FromSeconds(number);
            return string.Format("{0:0}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }
        else if(format == FormattingTypes.AFKTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(number);
            return string.Format("{0:0}h:{1:D2}m:{2:D2}s", time.Hours, time.Minutes, time.Seconds);
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

        number = Math.Truncate(number * 100) / 100;
        string formattedNumber = number.ToString("###.00") + suffixes[suffixIndex - 1];
        return formattedNumber;
    }
}
