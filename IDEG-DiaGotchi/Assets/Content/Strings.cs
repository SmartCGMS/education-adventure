using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Strings
{
    private static Strings _Current;

    public static Strings Current
    {
        get
        {
            if (_Current == null)
            {
                _Current = new Strings();
                _Current.Initialize();
            }
            return _Current;
        }
    }

    private class StringObject
    {
        public int id;
        public string value;
    }

    private Dictionary<int, StringObject> _Strings = new Dictionary<int, StringObject>();

    public void Initialize()
    {
        var res = CSVLoader.ReadResourceCSV("strings.en");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            string value = r["value"];

            _Strings.Add(id, new StringObject { id = id, value = value });
        }
    }

    public string GetString(int id)
    {
        if (_Strings.ContainsKey(id))
            return _Strings[id].value;
        return "<UNKNOWN>";
    }

    public static string Get(int id)
    {
        return Current.GetString(id);
    }
}
