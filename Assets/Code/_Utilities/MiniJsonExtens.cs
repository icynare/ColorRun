using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class MiniJsonExtens
{
    public static ArrayList JsonToList(this string json)
    {
        return (MiniJSON.jsonDecode(json) as ArrayList);
    }

    public static Hashtable JsonToHashtable(this string json)
    {
        return (MiniJSON.jsonDecode(json) as Hashtable);
    }

    public static string HashTableToJson(this Hashtable obj)
    {
        return MiniJSON.jsonEncode(obj);
    }

    public static string DictionaryToJson(this Dictionary<string, string> obj)
    {
        return MiniJSON.jsonEncode(obj);
    }
}

