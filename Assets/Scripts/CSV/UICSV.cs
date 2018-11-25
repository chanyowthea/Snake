using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICSV : CSVBaseData
{
    public string _Type;
    public string _Path;

    public override string GetPrimaryKey()
    {
        return _Type.ToString();
    }

    public override void ParseData(long index, int fieldCount, string[] headers, string[] values)
    {
        _Type = ReadString("Type", headers, values);
        _Path = ReadString("Path", headers, values);
    }
}
