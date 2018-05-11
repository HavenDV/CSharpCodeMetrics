using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeMetricsLibrary
{
    public class DzhilbMetrics
    {
        #region Properties

        public SyntaxTree SyntaxTree { get; }
        public SyntaxList<UsingDirectiveSyntax> Usings { get; }
        public List<ClassDeclarationSyntax> Classes { get; }
        public List<MethodDeclarationSyntax> Methods { get; }
        public List<ForStatementSyntax> Fors { get; }
        public List<DoStatementSyntax> Dos { get; }
        public List<VariableDeclarationSyntax> Variables { get; }
        public List<InvocationExpressionSyntax> Invokes { get; }
        public List<AssignmentExpressionSyntax> Assignments { get; }
        public List<IfStatementSyntax> Ifs { get; }
        public List<BinaryExpressionSyntax> BinaryOperators { get; }
        public List<GotoStatementSyntax> Gotos { get; }
        public List<SyntaxNode> Semicolons { get; }
        public List<VariableDeclaratorSyntax> Commas { get; }
        public List<SyntaxNode> Dots { get; }
        public List<SyntaxNode> Brackets { get; }

        public List<(string name, (int start, int end)[] lines, int count)> Lines { get; }
        public int OperatorsCount { get; }

        public int ClAbsolute { get; }
        public double Cl { get; }
        public int LLoop { get; }
        public int LIf { get; }
        public int LMod { get; }
        public double FMod { get; }
        public double FOp { get; }

        #endregion

        #region Constructors

        public DzhilbMetrics(string text)
        {
            SyntaxTree = CSharpSyntaxTree.ParseText(text);

            var root = (CompilationUnitSyntax)SyntaxTree.GetRoot();
            Usings = root.Usings;
            Classes = root.Members.Cast<ClassDeclarationSyntax>().ToList();
            Methods = Classes.SelectMany(i => i.Members).Cast<MethodDeclarationSyntax>().ToList();
            Variables = GetNodes<VariableDeclarationSyntax>(Methods);
            Invokes = GetNodes<InvocationExpressionSyntax>(Methods);
            Assignments = GetNodes<AssignmentExpressionSyntax>(Methods);
            Ifs = GetNodes<IfStatementSyntax>(Methods);
            Fors = GetNodes<ForStatementSyntax>(Methods);
            Dos = GetNodes<DoStatementSyntax>(Methods);
            //var compares = GetNodes<BinaryExpressionSyntax>(methods, i => 
            //    i.OperatorToken.Text == "<=" || i.OperatorToken.Text == ">=" ||
            //    i.OperatorToken.Text == "<" || i.OperatorToken.Text == ">");
            BinaryOperators = GetNodes<BinaryExpressionSyntax>(Methods);
            Gotos = GetNodes<GotoStatementSyntax>(Methods);
            Semicolons = GetNodes<ExpressionStatementSyntax>(Methods).Cast<SyntaxNode>().ToList();
            Semicolons.AddRange(Usings);
            Semicolons.AddRange(GetNodes<LocalDeclarationStatementSyntax>(Methods));
            Semicolons.AddRange(Gotos);
            Commas = Variables.Where(i => i.Variables.Count > 1).SelectMany(i => i.Variables.Skip(1)).ToList();

            Dots = GetNodes<LiteralExpressionSyntax>(Methods, i => i.Token.Text.Contains(".")).Cast<SyntaxNode>().ToList();
            Dots.AddRange(Invokes);

            Brackets = GetNodes<CastExpressionSyntax>(Methods).Cast<SyntaxNode>().ToList();
            Brackets.AddRange(GetNodes<ParenthesizedExpressionSyntax>(Methods));
            Brackets.AddRange(Ifs);
            Brackets.AddRange(Methods);
            Brackets.AddRange(Invokes);

            //foreach (var line in methods.SelectMany(i => i.DescendantNodes()))
            //{
            //Console.WriteLine($"{line.GetType()} {GetLine(line).start}     {line}");
            //}

            Lines = new List<(string name, (int start, int end)[] lines, int count)>
            {
                ("usings", GetLines(Usings), Usings.Count),
                ("classes", GetLines(Classes), Classes.Count),
                ("{}", GetBracersLines(Classes, Methods), Classes.Count + Methods.Count),
                ("methods", GetLines(Methods), Methods.Count),
                ("variables", GetLines(Variables), Variables.Count),
                ("invokes", GetLines(Invokes), Invokes.Count),
                ("assignments", GetLines(Assignments), Assignments.Count),
                ("ifs", GetLines(Ifs), Ifs.Count),
                ("binaryOperators", GetLines(BinaryOperators), BinaryOperators.Count),
                ("gotos", GetLines(Gotos), Gotos.Count),
                ("semicolons", GetLines(Semicolons), Semicolons.Count),
                ("commas", GetLines(Commas), Commas.Count),
                ("dots", GetLines(Dots), Dots.Count),
                ("brackets", GetLines(Brackets), Brackets.Count)
            };

            OperatorsCount = 0;
            foreach (var line in Lines)
            {
                OperatorsCount += line.count;
            }

            LIf = Ifs.Count;
            LMod = Methods.Count;
            LLoop = Fors.Count + Dos.Count;
            ClAbsolute = LIf + LLoop;
            Cl = 1.0 * ClAbsolute / OperatorsCount;
            FMod = Math.Pow(Invokes.Count(i => Methods.Any(j => j.Identifier.Text == i.Expression.ToString())), 4) / LMod;
        }

        #endregion

        #region Private methods

        public static int GetLine(SyntaxToken syntax) => syntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

        public static (int start, int end)[] GetBracersLinesFromClasses<T>(IEnumerable<T> enumerable) where T : BaseTypeDeclarationSyntax =>
            enumerable.Select(i => (GetLine(i.OpenBraceToken), GetLine(i.CloseBraceToken))).ToArray();

        public static (int start, int end)[] GetBracersLinesFromMethods<T>(IEnumerable<T> enumerable) where T : BaseMethodDeclarationSyntax =>
            enumerable.Select(i => (GetLine(i.Body.OpenBraceToken), GetLine(i.Body.CloseBraceToken))).ToArray();

        public static (int start, int end)[] GetBracersLines(IEnumerable<ClassDeclarationSyntax> classes,
            IEnumerable<MethodDeclarationSyntax> methods) =>
            GetBracersLinesFromClasses(classes).Concat(GetBracersLinesFromMethods(methods)).ToArray();

        public static (int start, int end) GetLine(SyntaxNode syntax)
        {
            var span = syntax.GetLocation().GetLineSpan();

            return (span.StartLinePosition.Line + 1, span.EndLinePosition.Line + 1);
        }

        public static (int start, int end)[] GetLines<T>(IEnumerable<T> enumerable) where T : SyntaxNode =>
            enumerable.Select(GetLine).ToArray();

        private static List<T> GetNodes<T>(IEnumerable<SyntaxNode> node) =>
            node.SelectMany(i => i.DescendantNodes().OfType<T>()).ToList();

        private static List<T> GetNodes<T>(IEnumerable<SyntaxNode> node, Func<T, bool> predicate) =>
            GetNodes<T>(node).Where(predicate).ToList();

        private static string ToString((int start, int end)[] lines) => string.Join(", ",
            lines.Select(i =>
                i.start == i.end
                    ? $"{i.start}"
                    : $"{i.start}({i.end})"));

        public string GetLinesText()
        {
            var builder = new StringBuilder();
            foreach (var (name, lines, count) in Lines)
            {
                builder.AppendLine($"{name} | {ToString(lines)} | {count}");
            }

            return builder.ToString();
        }

        #endregion
    }
}
