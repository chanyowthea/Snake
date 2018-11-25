using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ConfigDataManager : TSingleton<ConfigDataManager>
{
    private Dictionary<string, List<CSVBaseData>> m_ConfigDataLists = new Dictionary<string, List<CSVBaseData>>();
    private Dictionary<string, CSVBaseData> m_ConfigDataItemss = new Dictionary<string, CSVBaseData>();

    public override void Init()
    {
    }
    public override void Clear()
    {
        m_ConfigDataLists = new Dictionary<string, List<CSVBaseData>>();
        m_ConfigDataItemss = new Dictionary<string, CSVBaseData>();
    }
    public void LoadCSV<T>(string tableName) where T : CSVBaseData
    {
        string typeKey = GetDataListsKey<T>();
        if (m_ConfigDataLists.ContainsKey(typeKey))
        {
            Debug.LogWarning("duplicated key in config: " + typeKey);
            return;
        }

        string csvText = "";
        TextAsset csvTextAsset = Resources.Load<TextAsset>("CSV/" + tableName);
        byte[] csv_bs;

#if ENABLE_ENCRYPTION
            ResDecryption.Decryption(csvTextAsset.bytes, out csv_bs);
#else
        csv_bs = csvTextAsset.bytes;
#endif

        csvText = Encoding.UTF8.GetString(csv_bs);

        if (string.IsNullOrEmpty(csvText))
            return;

        List<T> result = ParseCSV<T>(csvText);
        if (result != null)
        {
            List<CSVBaseData> list = new List<CSVBaseData>();
            for (int i = 0; i < result.Count; i++)
            {
                CSVBaseData item = result[i];
                list.Add(item);
                string pKey = item.GetPrimaryKey();
                if (string.IsNullOrEmpty(pKey) == false)
                {
                    string itemKey = GetDataItemKey<T>(pKey);
                    if (m_ConfigDataItemss.ContainsKey(itemKey) == false)
                    {
                        m_ConfigDataItemss.Add(itemKey, item);
                    }
                    else
                    {
                        Debug.Log("duplicate item key: " + itemKey);
                    }
                }
            }
            m_ConfigDataLists.Add(typeKey, list);
        }
    }
    public List<CSVBaseData> GetDataList<T>() where T : CSVBaseData
    {
        string typeKey = GetDataListsKey<T>();
        List<CSVBaseData> result;
        if (m_ConfigDataLists.TryGetValue(typeKey, out result))
        {
            return result;
        }
        return null;
    }
    public T GetData<T>(string key) where T : CSVBaseData
    {
        string typeKey = GetDataItemKey<T>(key);
        CSVBaseData result;
        if (m_ConfigDataItemss.TryGetValue(typeKey, out result))
        {
            return result.As<T>();
        }
        return null;
    }
    private string GetDataListsKey<T>() where T : CSVBaseData
    {
        return typeof(T).ToString();
    }
    private string GetDataItemKey<T>(string pKey) where T : CSVBaseData
    {
        return string.Format("{0}|{1}", GetDataListsKey<T>(), pKey);
    }
    static public List<T> ParseCSV<T>(string csvText) where T : CSVBaseData
    {
        TextReader reader = new StringReader(csvText);
        CsvReader csv = new CsvReader(reader, true);
        int fieldCount = csv.FieldCount;
        string[] headers = csv.GetFieldHeaders();
        csv.ReadNextRecord();//skip desc
        List<T> result = new List<T>();
        while (csv.ReadNextRecord())
        {
            long index = csv.CurrentRecordIndex;
            string[] values = new string[fieldCount];
            csv.CopyCurrentRecordTo(values);

            T obj = (T)Activator.CreateInstance(typeof(T));
            obj.ParseData(index, fieldCount, headers, values);
            result.Add(obj);
        }
        return result;
    }
}
