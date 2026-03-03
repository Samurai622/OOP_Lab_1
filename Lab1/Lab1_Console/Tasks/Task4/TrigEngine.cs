using System;

namespace Lab1
{
    public static class TrigEngine
    {
        public static readonly string[] Names =
        {
            "sin",
            "cos",
            "tan",
            "cot",
            "exit"
        };

        public static readonly Func<double, double>[] Operations =
        {
            x => Math.Sin(x),
            x => Math.Cos(x),
            x => Math.Tan(x),
            x => 1 / Math.Tan(x)
        };

        public static double Execute(string op, double value)
        {
            int index = Array.IndexOf(Names, op);
            return Operations[index](value);
        }
    }
}