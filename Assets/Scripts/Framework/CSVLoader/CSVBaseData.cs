using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class CSVBaseData
{
    public abstract void ParseData
        (long index, int fieldCount, string[] headers, string[] values);
    public abstract string GetPrimaryKey();
    public T As<T>() where T : CSVBaseData
    {
        return (T)this;
    }
    protected static float ReadFloat(string fieldName, string[] headers, string[] values, float defaultValue = 0f)
    {
        string value = GetFieldValueWithFieldName(fieldName, headers, values);
        return StringConverter.ToFloat(value, defaultValue);
    }
    protected static int ReadInt(string fieldName, string[] headers, string[] values, int defaultValue = 0)
    {
        string value = GetFieldValueWithFieldName(fieldName, headers, values);
        return StringConverter.ToInt(value, defaultValue);
    }
    protected static uint ReadUInt(string fieldName, string[] headers, string[] values, uint defaultValue = 0)
    {
        string value = GetFieldValueWithFieldName(fieldName, headers, values);
        return StringConverter.ToUInt(value, defaultValue);
    }

    protected static string ReadString(string fieldName, string[] headers, string[] values, string defaultValue = null)
    {
        return GetFieldValueWithFieldName(fieldName, headers, values);
    }

    protected static string[] ReadStringArray(string fieldName, string[] headers, string[] values, string defaultValue = null)
    {
        string value = GetFieldValueWithFieldName(fieldName, headers, values);
        string[] result = value.Split(',');
        return result;
    }

    protected static uint[] ReadUIntArray(string fieldName, string[] headers, string[] values, uint defaultValue = 0, char InSplitChar = ',')
    {
        string value = GetFieldValueWithFieldName(fieldName, headers, values);
        string[] strResult = value.Split(InSplitChar);
        uint[] result = new uint[strResult.Length];
        for (int i = 0; i < strResult.Length; i++)
        {
            result[i] = StringConverter.ToUInt(strResult[i]);
        }
        return result;
    }
    protected static bool ReadBoolean(string fieldName, string[] headers, string[] values, bool defaultValue = false)
    {
        string value = GetFieldValueWithFieldName(fieldName, headers, values);
        return StringConverter.ToBool(value, defaultValue);
    }

    //protected static ResourceID ReadResourceID(string fieldName, string[] headers, string[] values, Type resVarType )
    //{
    //    string strResID = ReadString(fieldName, headers, values);
    //    if (strResID != "")
    //        return ResourceManager.instance.GetResourceIDByName(resVarType, strResID, false);
    //    return ResourceID.INVALID;
    //}

    static string GetFieldValueWithFieldName(string fieldName, string[] headers, string[] values)
    {
        string result = null;
        int index = -1;
        if (headers != null && headers.Length > 0 && !string.IsNullOrEmpty(fieldName))
        {
            for (int i = 0, count = headers.Length; i < count; i++)
            {
                string head = headers[i];
                if (string.Equals(fieldName, head))
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0 && index < values.Length)
            {
                result = values[index];
            }
        }
        return result;
    }

    public static implicit operator bool(CSVBaseData exists)
    {
        return exists != null;
    }

}
