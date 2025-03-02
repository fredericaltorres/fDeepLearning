using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class InterpreterTests
{
    private Lexer _lexer;
    private Parser _parser;
    private Interpreter _interpreter;

    [SetUp]
    public void Setup()
    {
        _interpreter = new Interpreter();
    }

    private List<ASTNode> ParseCode(string code)
    {
        _lexer = new Lexer(code);
        var tokens = _lexer.Tokenize();
        _parser = new Parser(tokens);
        return _parser.Parse();
    }

    [Test]
    public void TestArithmeticOperations()
    {
        var ast = ParseCode("x = 5 + 3 * 2; print(x);");
        _interpreter.Interpret(ast);
        Assert.AreEqual(11, _interpreter.LastEvaluated);
    }

    [Test]
    public void TestVariableAssignment()
    {
        var ast = ParseCode("x = 10; y = x + 5; print(y);");
        _interpreter.Interpret(ast);
        Assert.AreEqual(15, _interpreter.LastEvaluated);
    }

    [Test]
    public void TestWhileLoop()
    {
        var ast = ParseCode(@"
            i = 0;
            while (i < 5) {
                print(i);
                i = i + 1;
            }
        ");
        _interpreter.Interpret(ast);
        Assert.AreEqual(5, _interpreter.LastEvaluated);
        // Expected output: 0 1 2 3 4
    }

    [Test]
    public void TestForLoop()
    {
        var ast = ParseCode(@"
            for (i = 0; i < 5; i = i + 1) {
                print(i);
            }
        ");
        _interpreter.Interpret(ast);
        // Expected output: 0 1 2 3 4
    }

    [Test]
    public void TestFunction()
    {
        var ast = ParseCode(@"
            function add(a, b) {
                return a + b;
            }
            result = add(3, 4);
            print(result);
        ");
        _interpreter.Interpret(ast);
        // Expected output: 7
    }

    [Test]
    public void TestComplexExpression()
    {
        var ast = ParseCode(@"
            x = 10;
            y = 5;
            z = (x + y) * (x - y);
            print(z);
        ");
        _interpreter.Interpret(ast);
        // Expected output: 150
    }

    [Test]
    public void TestNestedLoops()
    {
        var ast = ParseCode(@"
            for (i = 0; i < 3; i = i + 1) {
                for (j = 0; j < 3; j = j + 1) {
                    print(i * 3 + j);
                }
            }
        ");
        _interpreter.Interpret(ast);
        // Expected output: 0 1 2 3 4 5 6 7 8
    }

    [Test]
    public void TestFunctionWithLoop()
    {
        var ast = ParseCode(@"
            function sumUpTo(n) {
                sum = 0;
                for (i = 1; i <= n; i = i + 1) {
                    sum = sum + i;
                }
                return sum;
            }
            result = sumUpTo(5);
            print(result);
        ");
        _interpreter.Interpret(ast);
        // Expected output: 15
    }
}
