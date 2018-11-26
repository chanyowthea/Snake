using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameCSV : CSVBaseData
{
    public int _ID;
    public string _Name;

    public override string GetPrimaryKey()
    {
        return _ID.ToString();
    }

    public override void ParseData(long index, int fieldCount, string[] headers, string[] values)
    {
        _ID = ReadInt("ID", headers, values);
        _Name = ReadString("Name", headers, values);
    }
}
