using System;

[Serializable]
public struct JsonDateTime
{
    public long value;

    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTime(jdt.value);
    }

    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTime();
        return jdt;
    }
}