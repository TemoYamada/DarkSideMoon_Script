using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MyUtil
{
    // 渡した文字列群を連結する
    public static string Append(params string[] strings)
    {
        return string.Join("", strings);

        // StringBuilder sb = new StringBuilder();

        // foreach(string str in strings)
        // {
        //     sb.Append(str);
        // }

        // return sb.ToString();
    }

    // 渡した文字列群をカンマ区切りなど指定の文字で連結する
    public static string Join(string separater, params string[] strings)
    {
        return string.Join(separater, strings);
    }

    // 1つの文字列をカンマ区切りなどのセパレーターをもとに分割する
    public static string[] Split(string str, params char[] separater)
    {
        return str.Split(separater);
    }

    // 文字列内に指定の文字列が含まれているか判定する
    public static bool Contains(string str, string target)
    {
        return str.Contains(target);
    }

    // 文字列内の指定の文字列位置（インデックス）を取得する
    public static int GetIndex(string str, string target)
    {
        return str.IndexOf(target);
    }

    // 文字列内の一部を取得する
    public static string GetPart(string str, int startIndex, int length = 0)
    {
        if (length == 0)
            length = str.Length - startIndex;

        return str.Substring(startIndex, length);
    }

    // 文字列内の最後の数文字を取得する
    public static string GetLastPart(string str, int length = 1)
    {
        int startIndex = str.Length - length;

        return str.Substring(startIndex, length);
    }

    // 他 参考（適宜 追加する）
    // https://www.sejuku.net/blog/45730
}
