using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public static class StringHelpers
{
    public static string ToCamelCase(this string incoming)
    {
        string[] words = incoming.Split(' ', '-', '.', '_');
        var sb = new StringBuilder();

        sb.Append(words[0].ToLower());
        words[0] = string.Empty;

        foreach (String word in words)
        {
            char[] letters = word.ToCharArray();
            if (letters.Length > 0)
            {
                letters[0] = ((new String(letters[0], 1)).ToUpper().ToCharArray())[0];
            }
            sb.Append(new String(letters));
        }
        return sb.ToString();
    }

    public static string ToPascalCase(this string incoming)
    {
        string[] words = incoming.Split(' ', '-', '.', '_');
        var sb = new StringBuilder();

        foreach (String word in words)
        {
            char[] letters = word.ToCharArray();
            if (letters.Length > 0)
            {
                letters[0] = ((new String(letters[0], 1)).ToUpper().ToCharArray())[0];
            }
            sb.Append(new String(letters));
        }
        return sb.ToString();
    }
}
