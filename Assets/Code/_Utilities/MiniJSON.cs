using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MiniJSON
{
    private const int BUILDER_CAPACITY = 0x7d0;
    protected static string lastDecode = string.Empty;
    protected static int lastErrorIndex = -1;
    private const int TOKEN_COLON = 5;
    private const int TOKEN_COMMA = 6;
    private const int TOKEN_CURLY_CLOSE = 2;
    private const int TOKEN_CURLY_OPEN = 1;
    private const int TOKEN_FALSE = 10;
    private const int TOKEN_NONE = 0;
    private const int TOKEN_NULL = 11;
    private const int TOKEN_NUMBER = 8;
    private const int TOKEN_SQUARED_CLOSE = 4;
    private const int TOKEN_SQUARED_OPEN = 3;
    private const int TOKEN_STRING = 7;
    private const int TOKEN_TRUE = 9;

    protected static void eatWhitespace(char[] json, ref int index)
    {
        while (index < json.Length)
        {
            if (" \t\n\r".IndexOf(json[index]) == -1)
            {
                break;
            }
            index++;
        }
    }

    public static int getLastErrorIndex()
    {
        return lastErrorIndex;
    }

    public static string getLastErrorSnippet()
    {
        if (lastErrorIndex == -1)
        {
            return string.Empty;
        }
        int startIndex = lastErrorIndex - 5;
        int num2 = lastErrorIndex + 15;
        if (startIndex < 0)
        {
            startIndex = 0;
        }
        if (num2 >= lastDecode.Length)
        {
            num2 = lastDecode.Length - 1;
        }
        return lastDecode.Substring(startIndex, (num2 - startIndex) + 1);
    }

    protected static int getLastIndexOfNumber(char[] json, int index)
    {
        int num = index;
        while (num < json.Length)
        {
            if ("0123456789+-.eE".IndexOf(json[num]) == -1)
            {
                break;
            }
            num++;
        }
        return (num - 1);
    }

    public static object jsonDecode(string json)
    {
        lastDecode = json;
        if (json == null)
        {
            return null;
        }
        char[] chArray = json.ToCharArray();
        int index = 0;
        bool success = true;
        object obj2 = parseValue(chArray, ref index, ref success);
        if (success)
        {
            lastErrorIndex = -1;
            return obj2;
        }
        lastErrorIndex = index;
        return obj2;
    }

    public static string jsonEncode(object json)
    {
        StringBuilder builder = new StringBuilder(0x7d0);
        return (!serializeValue(json, builder) ? null : builder.ToString());
    }

    public static bool lastDecodeSuccessful()
    {
        return (lastErrorIndex == -1);
    }

    protected static int lookAhead(char[] json, int index)
    {
        int num = index;
        return nextToken(json, ref num);
    }

    protected static int nextToken(char[] json, ref int index)
    {
        eatWhitespace(json, ref index);
        if (index != json.Length)
        {
            char ch = json[index];
            index++;
            switch (ch)
            {
                case '"':
                    return 7;

                case ',':
                    return 6;

                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return 8;

                case ':':
                    return 5;

                case '[':
                    return 3;

                case ']':
                    return 4;

                case '{':
                    return 1;

                case '}':
                    return 2;
            }
            index--;
            int num = json.Length - index;
            if ((((num >= 5) && (json[index] == 'f')) && ((json[index + 1] == 'a') && (json[index + 2] == 'l'))) && ((json[index + 3] == 's') && (json[index + 4] == 'e')))
            {
                index += 5;
                return 10;
            }
            if ((((num >= 4) && (json[index] == 't')) && ((json[index + 1] == 'r') && (json[index + 2] == 'u'))) && (json[index + 3] == 'e'))
            {
                index += 4;
                return 9;
            }
            if ((((num >= 4) && (json[index] == 'n')) && ((json[index + 1] == 'u') && (json[index + 2] == 'l'))) && (json[index + 3] == 'l'))
            {
                index += 4;
                return 11;
            }
        }
        return 0;
    }

    protected static ArrayList parseArray(char[] json, ref int index)
    {
        ArrayList list = new ArrayList();
        nextToken(json, ref index);
        bool flag = false;
        while (!flag)
        {
            int num = lookAhead(json, index);
            if (num == 0)
            {
                return null;
            }
            if (num == 6)
            {
                nextToken(json, ref index);
            }
            else
            {
                if (num == 4)
                {
                    nextToken(json, ref index);
                    return list;
                }
                bool success = true;
                object obj2 = parseValue(json, ref index, ref success);
                if (!success)
                {
                    return null;
                }
                list.Add(obj2);
            }
        }
        return list;
    }

    protected static double parseNumber(char[] json, ref int index)
    {
        eatWhitespace(json, ref index);
        int num = getLastIndexOfNumber(json, index);
        int length = (num - index) + 1;
        char[] destinationArray = new char[length];
        Array.Copy(json, index, destinationArray, 0, length);
        index = num + 1;
        return double.Parse(new string(destinationArray));
    }

    protected static Hashtable parseObject(char[] json, ref int index)
    {
        Hashtable hashtable = new Hashtable();
        nextToken(json, ref index);
        bool flag = false;
        while (!flag)
        {
            switch (lookAhead(json, index))
            {
                case 0:
                    return null;

                case 6:
                {
                    nextToken(json, ref index);
                    continue;
                }
                case 2:
                    nextToken(json, ref index);
                    return hashtable;
            }
            string str = parseString(json, ref index);
            if (str == null)
            {
                return null;
            }
            if (nextToken(json, ref index) != 5)
            {
                return null;
            }
            bool success = true;
            object obj2 = parseValue(json, ref index, ref success);
            if (!success)
            {
                return null;
            }
            hashtable[str] = obj2;
        }
        return hashtable;
    }

    protected static string parseString(char[] json, ref int index)
    {
        string str = string.Empty;
        eatWhitespace(json, ref index);
        char ch = json[index++];
        bool flag = false;
        while (!flag)
        {
            if (index == json.Length)
            {
                break;
            }
            ch = json[index++];
            if (ch == '"')
            {
                flag = true;
                break;
            }
            if (ch == '\\')
            {
                if (index == json.Length)
                {
                    break;
                }
                ch = json[index++];
                if (ch == '"')
                {
                    str = str + '"';
                }
                else
                {
                    if (ch == '\\')
                    {
                        str = str + '\\';
                        continue;
                    }
                    if (ch == '/')
                    {
                        str = str + '/';
                        continue;
                    }
                    if (ch == 'b')
                    {
                        str = str + '\b';
                        continue;
                    }
                    if (ch == 'f')
                    {
                        str = str + '\f';
                        continue;
                    }
                    if (ch == 'n')
                    {
                        str = str + '\n';
                        continue;
                    }
                    if (ch == 'r')
                    {
                        str = str + '\r';
                        continue;
                    }
                    if (ch == 't')
                    {
                        str = str + '\t';
                    }
                    else if (ch == 'u')
                    {
                        int num = json.Length - index;
                        if (num < 4)
                        {
                            break;
                        }
                        char[] destinationArray = new char[4];
                        Array.Copy(json, index, destinationArray, 0, 4);
                        str = str + "&#x" + new string(destinationArray) + ";";
                        index += 4;
                    }
                }
            }
            else
            {
                str = str + ch;
            }
        }
        if (!flag)
        {
            return null;
        }
        return str;
    }

    protected static object parseValue(char[] json, ref int index, ref bool success)
    {
        switch (lookAhead(json, index))
        {
            case 1:
                return parseObject(json, ref index);

            case 3:
                return parseArray(json, ref index);

            case 7:
                return parseString(json, ref index);

            case 8:
                return parseNumber(json, ref index);

            case 9:
                nextToken(json, ref index);
                return bool.Parse("TRUE");

            case 10:
                nextToken(json, ref index);
                return bool.Parse("FALSE");

            case 11:
                nextToken(json, ref index);
                return null;
        }
        success = false;
        return null;
    }

    protected static bool serializeArray(ArrayList anArray, StringBuilder builder)
    {
        builder.Append("[");
        bool flag = true;
        for (int i = 0; i < anArray.Count; i++)
        {
            object obj2 = anArray[i];
            if (!flag)
            {
                builder.Append(", ");
            }
            if (!serializeValue(obj2, builder))
            {
                return false;
            }
            flag = false;
        }
        builder.Append("]");
        return true;
    }

    protected static bool serializeDictionary(Dictionary<string, string> dict, StringBuilder builder)
    {
        builder.Append("{");
        bool flag = true;
        foreach (KeyValuePair<string, string> pair in dict)
        {
            if (!flag)
            {
                builder.Append(", ");
            }
            serializeString(pair.Key, builder);
            builder.Append(":");
            serializeString(pair.Value, builder);
            flag = false;
        }
        builder.Append("}");
        return true;
    }

    protected static void serializeNumber(double number, StringBuilder builder)
    {
        builder.Append(Convert.ToString(number));
    }

    protected static bool serializeObject(Hashtable anObject, StringBuilder builder)
    {
        builder.Append("{");
        IDictionaryEnumerator enumerator = anObject.GetEnumerator();
        for (bool flag = true; enumerator.MoveNext(); flag = false)
        {
            string aString = enumerator.Key.ToString();
            object obj2 = enumerator.Value;
            if (!flag)
            {
                builder.Append(", ");
            }
            serializeString(aString, builder);
            builder.Append(":");
            if (!serializeValue(obj2, builder))
            {
                return false;
            }
        }
        builder.Append("}");
        return true;
    }

    protected static bool serializeObjectOrArray(object objectOrArray, StringBuilder builder)
    {
        if (objectOrArray is Hashtable)
        {
            return serializeObject((Hashtable) objectOrArray, builder);
        }
        return ((objectOrArray is ArrayList) && serializeArray((ArrayList) objectOrArray, builder));
    }

    protected static void serializeString(string aString, StringBuilder builder)
    {
        builder.Append("\"");
        foreach (char ch in aString.ToCharArray())
        {
            switch (ch)
            {
                case '"':
                    builder.Append("\\\"");
                    break;

                case '\\':
                    builder.Append(@"\\");
                    break;

                case '\b':
                    builder.Append(@"\b");
                    break;

                case '\f':
                    builder.Append(@"\f");
                    break;

                case '\n':
                    builder.Append(@"\n");
                    break;

                case '\r':
                    builder.Append(@"\r");
                    break;

                case '\t':
                    builder.Append(@"\t");
                    break;

                default:
                {
                    int num2 = Convert.ToInt32(ch);
                    if ((num2 >= 0x20) && (num2 <= 0x7e))
                    {
                        builder.Append(ch);
                    }
                    else
                    {
                        builder.Append(@"\u" + Convert.ToString(num2, 0x10).PadLeft(4, '0'));
                    }
                    break;
                }
            }
        }
        builder.Append("\"");
    }

    protected static bool serializeValue(object value, StringBuilder builder)
    {
        if (value == null)
        {
            builder.Append("null");
        }
        else if (value.GetType().IsArray)
        {
            serializeArray(new ArrayList((ICollection) value), builder);
        }
        else if (value is string)
        {
            serializeString((string) value, builder);
        }
        else if (value is char)
        {
            serializeString(Convert.ToString((char) value), builder);
        }
        else if (value is Hashtable)
        {
            serializeObject((Hashtable) value, builder);
        }
        else if (value is Dictionary<string, string>)
        {
            serializeDictionary((Dictionary<string, string>) value, builder);
        }
        else if (value is ArrayList)
        {
            serializeArray((ArrayList) value, builder);
        }
        else if ((value is bool) && ((bool) value))
        {
            builder.Append("true");
        }
        else if ((value is bool) && !((bool) value))
        {
            builder.Append("false");
        }
        else if (value.GetType().IsPrimitive)
        {
            serializeNumber(Convert.ToDouble(value), builder);
        }
        else
        {
            return false;
        }
        return true;
    }
}

