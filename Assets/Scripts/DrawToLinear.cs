using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DrawToLinear
{
    public string Input { get; private set; }
    public string Output { get; private set;}
    public DrawToLinear(string input)
    {
        this.Input = input;
        this.Output = "";
    }
    string ConvertToLinear(string input)
    {
        MatchCollection match;
        string[] X = new string[0]{};

        return "";
    }
    public Regex Groups { get => new(@"(\{|\()[0-9.X+\-*/]+(\}|\()"); }
    public Regex Frac { get => new(@"([0-9.]+)?\\frac{(-?[0-9.]+)}{(-?[0-9.]+)}"); }
    public Regex Log { get => new(@"\\log(_\{[0-9.]+\})?(-?[0-9.]+)"); }
    public Regex Pow { get => new(@"([0-9.]+)\^{(-?[0-9.]+)}"); }
    public Regex SinCosTan { get => new(@"\\(sin|cos|tan|asin|acos|atan)(-?[0-9.]+)"); }
}
