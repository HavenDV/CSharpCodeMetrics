using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeMetricsLibrary.Tests
{
    [TestClass]
    public class DzhlibMetricsTests
    {
        [TestMethod]
        public void FirstTest() => BaseTest(@"using System;

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
}", 94);

        [TestMethod]
        public void SecondTest() => BaseTest(@"using System;

class Exempl
{
    public static int[] Copy(int[] a)
    {
        int[] b = new int[a.Length];
        int j = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] >= 0)
            {
                b[j] = a[i];
                j++;
            }
        }

        return b;
    }

    public static void Zapoln(ref int[] a, Random g)
    {
        for (int i = 0; i < a.Length; i++)
        {
            a[i] = g.Next(-100, 300);
        }
    }

    public static void Print(int[] a, string str, string strl)
    {
        Console.WriteLine(strl);
        for (int i = 0; i < a.Length; i++)
        {

            if (a[i] != 0) Console.Write(str, a[i]);

            Console.WriteLine();
        }
    }

    public static void Main()
    {
        int[] a, b, c, p;
        Random g = new Random();
        char r;
        do
        {
            Console.Clear();



            Console.WriteLine(""OnpefleaHTe размер первого массива!"");
            a = new int[int.Parse(Console.ReadLine())];
            Console.WriteLine(""OnpeflenHTe размер второго массива!"");
            b = new int[int.Parse(Console.ReadLine())];
            c = new int[a.Length + b.Length];
            Exempl.Zapoln(ref a, g);
            Exempl.Zapoln(ref b, g);
            Exempl.Print(a, "" {0,5}"", ""Первый исходный массив!!!"");
            Exempl.Print(b, "" {0,5}"", ""Второй исходный массив!!!"");
            p = Exempl.Copy(a);
            Array.Copy(p, 0, c, 0, a.Length);
            p = Exempl.Copy(b);
            Array.Copy(p, 0, c, a.Length, b.Length);
            Exempl.Print(c, "" {0,5}"", ""Результатный массив!!!"");
            Console.WriteLine();
            Console.WriteLine(""BbinonHHTb повтор программы? Y/N"");
            r = char.Parse(Console.ReadLine());
        } while (r == 'Y' || r == 'у');
    }
}", 94);
        
        private static void BaseTest(string text, int expected)
        {
            var metrics = new DzhilbMetrics(text);

            foreach (var line in metrics.Methods.SelectMany(i => i.DescendantNodes()))
            {
                Console.WriteLine($"{line.GetType()} {DzhilbMetrics.GetLine(line).start}     {line}");
            }

            Console.WriteLine(metrics.GetLinesText());

            Assert.AreEqual(expected, metrics.OperatorsCount);
        }
    }
}
