using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeMetricsLibrary.Tests
{
    [TestClass]
    public class DzhlibMetricsTests
    {
        [TestMethod]
        public void FirstTest()
        {
            var metrics = new DzhilbMetrics(@"using System;

class Operator
{
    public static void Main()
    {
        float x,
        y;
        char rep;
        string str;

        REPEAT:
        Console.Clear();
        Console.Write(""Введите аргумент функции: "");
        str = Console.ReadLine();
        x = float.Parse(str);
        if (x < -0)
            if (x <= -0.5)
                y = (float)0.5;
            else
                y = (float)(x + 1.0);
        else
            if (x <= 1.0)
                y = (float)(x * x - 1.0);
            else
                y = (float)(x - 1.0);

        str = ""F("" + x + "")="" + y;
        Console.WriteLine(str);

        Console.Write(""Для повтора вычислений намите клавишу Y :"");
        rep = char.Parse(Console.ReadLine());
        if (rep == 'Y' || rep == 'у') goto REPEAT;
    }
}");

            //foreach (var line in metrics.Methods.SelectMany(i => i.DescendantNodes()))
            {
                //Console.WriteLine($"{line.GetType()} {DzhilbMetrics.GetLine(line).start}     {line}");
            }

            ShowDictionary(metrics);

            Assert.AreEqual(94, metrics.OperatorsCount);
        }

        private static string ToString((int start, int end)[] lines) => string.Join(", ",
            lines.Select(i =>
                i.start == i.end
                    ? $"{i.start}"
                    : $"{i.start}({i.end})"));

        private static void ShowDictionary(DzhilbMetrics metrics)
        {
            foreach (var (name, lines, count) in metrics.Lines)
            {
                Console.WriteLine($"{name} | {ToString(lines)} | {count}");
            }
        }
    }
}
