using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSInterpreter
{
    public class FredInterpreter
    {
        public static object Run(string source)
        {
            Interpreter Interpreter = new Interpreter();
            var lexer = new Lexer(source);
            List<Token> tokens = lexer.ScanTokens();

            var parser = new Parser(tokens);
            List<Statment> statements = parser.Parse();

            // Stop if there was a syntax error.
            if (statements == null) return null;

            Interpreter.Interpret(statements);
            return Interpreter.LastEvaluated;
        }

        public static void RunFile(string path)
        {
            string source = System.IO.File.ReadAllText(path);
            Run(source);
        }

        public static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line == null) break;
                Run(line);
            }
        }
    }

    public class Tests
    {
        private class TestOutput : System.IO.TextWriter
        {
            public readonly StringBuilder Output = new StringBuilder();

            public override void Write(char value)
            {
                Output.Append(value);
            }

            public override Encoding Encoding => Encoding.UTF8;

            public void Clear()
            {
                Output.Clear();
            }

            public override string ToString()
            {
                return Output.ToString();
            }
        }

        public static void RunTests()
        {
            TestOutput output = new TestOutput();
            //Console.SetOut(output);
            // Test arithmetic operations

            Console.WriteLine("===== Testing Arithmetic Operations =====");
            var r = FredInterpreter.Run("print(2 + 3);"); // Should print 5

            FredInterpreter.Run("print 5 - 2;"); // Should print 3
            FredInterpreter.Run("print 3 * 4;"); // Should print 12
            FredInterpreter.Run("print 10 / 2;"); // Should print 5
            FredInterpreter.Run("print (2 + 3) * 4;"); // Should print 20

            Console.WriteLine("\n===== Testing Variables =====");
            FredInterpreter.Run(@"
                var a = 10;
                var b = 20;
                print a + b;  // Should print 30
                a = 5;
                print a;  // Should print 5
            ");

            Console.WriteLine("\n===== Testing Comparison Operations =====");
            FredInterpreter.Run(@"
                print 5 > 3;    // Should print true
                print 5 < 3;    // Should print false
                print 5 >= 5;   // Should print true
                print 5 <= 3;   // Should print false
                print 5 == 5;   // Should print true
                print 5 != 5;   // Should print false
            ");

            Console.WriteLine("\n===== Testing Logical Operators =====");
            FredInterpreter.Run(@"
                print true and true;    // Should print true
                print true and false;   // Should print false
                print false or true;    // Should print true
                print false or false;   // Should print false
                print !true;            // Should print false
                print !false;           // Should print true
            ");

            Console.WriteLine("\n===== Testing If Statements =====");
            FredInterpreter.Run(@"
                var a = 10;
                if (a > 5) {
                    print ""a is greater than 5"";
                } else {
                    print ""a is less than or equal to 5"";
                }
                a = 3;
                if (a > 5) {
                    print ""a is greater than 5"";
                } else {
                    print ""a is less than or equal to 5"";
                }
            ");

            Console.WriteLine("\n===== Testing While Loop =====");
            FredInterpreter.Run(@"
                var a = 1;
                while (a <= 5) {
                    print a;
                    a = a + 1;
                }
            ");

            Console.WriteLine("\n===== Testing For Loop =====");
            FredInterpreter.Run(@"
                for (var i = 1; i <= 5; i = i + 1) {
                    print i;
                }
            ");

            Console.WriteLine("\n===== Testing Functions =====");
            FredInterpreter.Run(@"
                function add(a, b) {
                    return a + b;
                }
                print add(5, 3);  // Should print 8
                function factorial(n) {
                    if (n <= 1) return 1;
                    return n * factorial(n - 1);
                }
                print factorial(5);  // Should print 120
            ");

            Console.WriteLine("\n===== Testing Scope =====");
            FredInterpreter.Run(@"
                var a = 1;
                {
                    var a = 2;
                    print a;  // Should print 2
                }
                print a;  // Should print 1
                
                {
                    var b = 3;
                    print b;  // Should print 3
                }
                // print b;  // Would cause an error because b is out of scope
            ");

            Console.WriteLine("\n===== Fibonacci Example =====");
            FredInterpreter.Run(@"
                function fibonacci(n) {
                    if (n <= 1) return n;
                    return fibonacci(n - 1) + fibonacci(n - 2);
                }
                
                // Calculate first 10 Fibonacci numbers
                for (var i = 0; i < 10; i = i + 1) {
                    print fibonacci(i);
                }
            ");

            string testOutput = output.ToString();
            Console.SetOut(Console.Out);
            Console.WriteLine("Unit Tests Output:");
            Console.WriteLine(testOutput);
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: JSInterpreter [script]");
                return;
            }
            else if (args.Length == 1 && args.Contains("--test"))
            {
                Tests.RunTests();
            }
            else if (args.Length == 1)
            {
                FredInterpreter.RunFile(args[0]);
            }
            else
            {
                FredInterpreter.RunPrompt();
            }
        }
    }
}