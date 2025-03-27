using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Calculation
{
    public string Input { get; set; }
    public string Output { get; }
    public string Error {
        get => error switch
        {
            0 => string.Empty,
            1 => "2 dots in 1 num",
            2 => "Syntax Error",
            3 => "InfinityLoops / Math Errors",
            _ => string.Empty,
        };
    }
    public bool Success { get => this.Error == string.Empty; }

    byte Stop { get; set; }
    byte error = 0;
    
    public Calculation(string input)
    {
        this.Input = input;
        this.Output = Calc(true);
        if (this.Output == string.Empty && error == 0)
            error = 2;
    }

    public string OutputMath()
    {
        return Calc(false);
    }

    private string Calc(bool Linear)
    {
        this.Stop = 0;
        string output = this.Input;
        Match check;

        //Linear check and replace pi
        if (Linear)
        {
            while ((check = Regex.Match(output, @"([\d.])?\\pi")).Success)
            {
                output = output.Replace(check.Value, check.Groups[1].Value + "×" + Mathf.PI.ToString());
            }
            output = output.Replace("\\pi", Mathf.PI.ToString());
        }

        //Syntax check
        //bracket
        string[] left = output.Split('(');
        string[] right = output.Split(')');
        if (left.Length > right.Length)
            output += ")";
        else if (right.Length > left.Length)
            output = "(" + output;
        if (Regex.Match(output, @"[0-9]*\.[0-9]*\.").Success)
        {
            this.error = 1;
            return string.Empty;
        }
        //missing number fix
        while ((check = Regex.Match(output, @"(\d\.)\D")).Success)
            output = output.Replace(check.Groups[1].Value, check.Groups[1].Value + "0");
        while ((check = Regex.Match(output, @"\D(\.\d)")).Success)
            output = output.Replace(check.Groups[1].Value, "0" + check.Groups[1].Value);
        //multiple and bracket fix
        while ((check = Regex.Match(output, @"(\d|})(\(|\|)")).Success)
            output = output.Replace(check.Value, $"{check.Groups[1].Value}×{check.Groups[2].Value}");
        while ((check = Regex.Match(output, @"(\)|\|)(\d|\\)")).Success)
            output = output.Replace(check.Value, $"{check.Groups[1].Value}×{check.Groups[2].Value}");
        //multiple minus or plus
        while ((check = Regex.Match(output, @"([×÷])?([+-]{2,})")).Success)
        {
            GroupCalc i = new();
            if (i.SimplerPosNeg(check.Groups[2].Value) == "-")
                output = output.Replace(check.Groups[2].Value, "-");
            else if (i.SimplerPosNeg(check.Groups[2].Value) == "+")
            {
                if (!check.Groups[1].Success)
                    output = output.Replace(check.Groups[2].Value, "+");
                else
                    output = output.Replace(check.Groups[2].Value, "");
            }
        }
        //+ (+ and {+ check
        if (output.StartsWith('+'))
            output = output.Remove(0, 1);
        output = output.Replace("(+", "(");
        output = output.Replace("{+", "{");

        //Syntax Error
        if (Regex.Match("" + output[^1], @"[×÷^]").Success)
        {
            this.error = 2;
            return string.Empty;
        }
        if (Regex.Match(output, @"[+\-×÷^][\)}|]").Success)
        {
            this.error = 2;
            return string.Empty;
        }
        if (output.Contains(@"{}"))
        {
            this.error = 2;
            return string.Empty;
        }
        if (output.Contains(@"()"))
            output = output.Replace("()", "0");
        if (output.Contains(@"||"))
            output = output.Replace("||", "0");

        if (Linear)
        {
        CalcGroup:
            //GroupCalc calc
            MatchCollection groups = Regex.Matches(output, @"(\([^\(\)]+\)|{[^\(\){}]+}|\|[^\|]+\|)");
            foreach (Match group in groups)
            {
                GroupCalc i = new(group.Value[1..^1]);
                if (i.Success)
                {
                    if (group.Value.StartsWith('{'))
                        output = output.Replace(i.Input, i.Output);
                    else if (group.Value.StartsWith('('))
                        output = output.Replace(group.Value, i.Output);
                    else if (group.Value.StartsWith('|'))
                    {
                        if (i.Output.StartsWith('-'))
                            output = output.Replace(group.Value, i.Output[1..]);
                        else output = output.Replace(group.Value, i.Output);
                    }
                }
            }
            List<Regex> list = new MathTexDraw().Methods;
            foreach (Regex method in list)
            {
                groups = method.Matches(output);
                foreach (Match group in groups)
                {
                    MathTexDraw i = new(group.Value, method);
                    if (i.Success)
                    {
                        output = output.Replace(i.Input, i.Output.ToString());
                    }
                }
            }

            this.Stop += 1;

            if (this.Stop >= 250)
            {
                error = 3;
                return string.Empty;
            }

            if (Regex.Match(output, @"[(){}|]").Success)
                goto CalcGroup;

            //TexCalc
            return new GroupCalc(output).Success ? new GroupCalc(output).Output : string.Empty;
        }
        else //Math Edition
        {

            return "In developing";
        }
    }

    internal struct GroupCalc
    {
        public string Input { get; set; }
        public bool Success {
            get
            {
                if (Output.Contains("NaN") || !IsABlock) return false; else return true;
            }
        }
        public bool IsABlock { get => !Input.Contains('{') && !Input.Contains('('); }
        public GroupCalc(string input)
        {
            Input = input;
        }

        public string Output
        {
            get
            {
                string output = Input;
                Match match;
                //Multi and Divide
                while ((match = Regex.Match(output, @"(-?[0-9.]+)([×÷])(-?[0-9.]+)")).Success)
                    output = output.Replace(match.Value, SimpleMath(match.Groups[1].Value, match.Groups[3].Value, match.Groups[2].Value).ToString());
                //Plus and Minus
                while ((match = Regex.Match(output, @"(-?[0-9.]+)([-+])(-?[0-9.]+)")).Success)
                    output = output.Replace(match.Value, SimpleMath(match.Groups[1].Value, match.Groups[3].Value, match.Groups[2].Value).ToString());
                return output;
            }
        }

        public string SimplerPosNeg(string symbols)
        {
            try
            {
                byte pos = 0, neg = 0;
                foreach (char i in symbols)
                {
                    if (i == '+')
                        pos++;
                    else if (i == '-')
                        neg++;
                    else
                        return "NaN";
                }
                if (neg % 2 != 0)
                    return "-";
                else return "+";
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return "NaN";
            }
        }

        float SimpleMath(string a, string b, string methods)
        {
            try
            {
                return methods switch
                {
                    "+" => float.Parse(a) + float.Parse(b),
                    "-" => float.Parse(a) - float.Parse(b),
                    "×" => float.Parse(a) * float.Parse(b),
                    "÷" => float.Parse(a) / float.Parse(b),
                    _ => float.NaN,
                };
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        
    }

    public struct MathTexDraw
    {
        public Regex Frac { get => new(@"([0-9.]+)?\\frac{(-?[0-9.]+)}{(-?[0-9.]+)}"); }
        public Regex Log { get => new(@"\\log(_\{[0-9.]+\}×)?(-?[0-9.]+)"); }
        public Regex Pow { get => new(@"([0-9.]+)\^{(-?[0-9.]+)}"); }
        public Regex SinCosTan { get => new(@"\\(sin|cos|tan|asin|acos|atan)(-?[0-9.]+)"); }
        public Regex Factor { get => new(@"{([0-9.]+)}!"); }
        public List<Regex> Methods { get => new() {
            Frac, Log, Pow, SinCosTan, Factor }; }
        public Regex Method { get; set; }
        public string Input { get; set; }
        public bool Success { get => Output != float.NaN; }
        public MathTexDraw(string input, Regex method)
        {
            Input = input;
            Method = method;
        }
        public float Output
        {
            get
            {
                List<string> SMethods = new();
                foreach (Regex item in Methods)
                {
                    SMethods.Add(item.ToString());
                }
                int index = SMethods.IndexOf(Method.ToString());
                return index switch
                {
                    0 => Fraction(Input),
                    1 => LogaritOut(Input),
                    2 => Power(Input),
                    3 => SinCosTanSolve(Input),
                    4 => Factorial(Input),
                    _ => float.NaN,
                };
            }
        }

        float Factorial(string input)
        {
            try
            {
                Match match = Factor.Match(input);
                if (!match.Success)
                    return float.NaN;
                int a = int.Parse(match.Groups[1].Value);
                int output = 1;
                if (a == 0) return 1;
                if (a < 0)
                    return float.NaN;
                for (int i = 1; i <= a; i++)
                {
                    output *= i;
                }
                return output;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        float SinCosTanSolve(string input)
        {
            try
            {
                Match match = SinCosTan.Match(input);
                if (match.Success)
                {
                    return match.Groups[1].Value switch
                    {
                        "sin" => Mathf.Sin(float.Parse(match.Groups[2].Value)),
                        "cos" => Mathf.Cos(float.Parse(match.Groups[2].Value)),
                        "tan" => Mathf.Tan(float.Parse(match.Groups[2].Value)),
                        "asin" => Mathf.Asin(float.Parse(match.Groups[2].Value)),
                        "acos" => Mathf.Acos(float.Parse(match.Groups[2].Value)),
                        "atan" => Mathf.Atan(float.Parse(match.Groups[2].Value)),
                        _ => float.NaN,
                    };
                }
                return float.NaN;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        float Fraction(string input)
        {
            Match match = Frac.Match(input);
            if (match.Success)
            {
                if (match.Groups[1].Success)
                {                        
                    return Fraction(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
                }
                else if (!match.Groups[1].Success)
                {
                    return Fraction(match.Groups[2].Value, match.Groups[3].Value);
                }
            }
            return float.NaN;
        }

        float LogaritOut(string input)
        {
            Match match = Log.Match(input);
            if (match.Success)
            {
                if (match.Groups[1].Success)
                    return Logarit(match.Groups[2].Value, match.Groups[1].Value[2..^2]);
                else if (!match.Groups[1].Success)
                    return Logarit(match.Groups[2].Value);
            }
            return float.NaN;
        }

        float Power(string input)
        {
            try
            {
                Match match = Pow.Match(input);
                if (match.Success)
                    return Mathf.Pow(float.Parse(match.Groups[1].Value), float.Parse(match.Groups[2].Value));
                return float.NaN;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        float Logarit(string a)
        {
            try
            {
                float f = float.Parse(a);
                return Mathf.Log10(f);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        float Logarit(string a, string sub)
        {
            try
            {
                float f = float.Parse(a);
                float p = float.Parse(sub);
                return Mathf.Log(f, p);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        float Fraction(string top, string bottom)
        {
            try
            {
                float topf = float.Parse(top);
                float botf = float.Parse(bottom);
                return topf / botf;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }

        float Fraction(string mod, string top, string bottom)
        {
            try
            {
                float modf = float.Parse(mod);
                float topf = float.Parse(top);
                float botf = float.Parse(bottom);
                return ((modf * botf) + topf) / botf;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return float.NaN;
            }
        }
    }
}
