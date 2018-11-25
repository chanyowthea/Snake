
public static class StringConverter
{
    public static float ToFloat(string str, float defaultValue = 0f)
    {
        float value;
        if (string.IsNullOrEmpty(str) || !float.TryParse(str, out value))
        {
            return defaultValue;
        }
        return value;
    }
    public static int ToInt(string str, int defaultValue = 0)
    {
        int value;
        if (string.IsNullOrEmpty(str) || !int.TryParse(str, out value))
        {
            return defaultValue;
        }
        return value;
    }
    public static uint ToUInt(string str, uint defaultValue = 0)
    {
        uint value;
        if (string.IsNullOrEmpty(str) || !uint.TryParse(str, out value))
        {
            return defaultValue;
        }
        return value;
    }
    public static bool ToBool(string str, bool defaultValue = false)
    {
        bool value;
        if (string.IsNullOrEmpty(str) || !bool.TryParse(str, out value))
        {
            return defaultValue;
        }
        return value;
    }
}