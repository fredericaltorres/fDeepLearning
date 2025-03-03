using NUnit.Framework;
using System;
using System.Collections.Generic;
using static JSInterpreter.Statment;

namespace JSInterpreter
{
    [TestFixture]
    public class InterpreterTests
    {
        [Test]
        public void NumericExpressions()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                ["2 + 3"] = 5.0,
                ["5 - 2"] = 3.0,
                ["10 / 2"] = 5.0,
                ["(2 + 3) * 4"] = 20.0,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"({expression.Key});"));
        }

        [Test]
        public void ComparaisonExpressions()
        {
            var testExpressions = new Dictionary<string, bool>()
            {
                ["5 > 3"]  = true,
                ["5 < 3"]  = !true,
                ["5 >= 5"] = true,
                ["5 <= 3"] = !true,
                ["5 == 5"] = true,
                ["5 != 5"] = !true,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"({expression.Key});"));
        }

        [Test]
        public void NumericExpressionsInVariables()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                ["var a = 10; var b = 20; a + b;"] = 30.0,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key}"));
        }

        [Test]
        public void LogicalOperators()
        {
            var testExpressions = new Dictionary<string, bool>()
            {
                ["false or true"] = false,
                ["true and true"] = true,
                ["true and false"] = false,
                ["true or false"] = true,
                ["true or true"] = true,
                ["false or false"] = false,
                ["!false"] = true,
                ["!true"] = false,
                ["!false and true"] = true,
                ["true and !true"] = false,
                ["true and true and true"] = true,
                ["true and true and false"] = false,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key};"));
        }

        [Test]
        public void IfExpression()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                [@"var a=10; var b=0; if(a >   5){ b = 1; } print(b);"] = 1.0,
                [@"var a=10; var b=0; if(a >= 10){ b = 1; } print(b);"] = 1.0,
                [@"var ok = true; var a=10; var b=0; if(a >= 10 and ok){ b = 1; } print(b);"] = 1.0,
                [@"var ok = false; var a=10; var b=0; if(a >= 10 and ok){ b = 1; } print(b);"] = 0.0,
                [@"var ok = true; var a=10; var b=0; if(a >= 10 and !ok){ b = 1; } print(b);"] = 0.0,
                [@"var ok = true; var a=10; var b=0; var c = 0; if(a >= 10 and ok){ b = 1; } print(c);"] = 0.0,
                [@"var ok = true; var a=10; var b=0; var c = 0; 
                    if(a >= 10 and ok) { 
                        b = 1; 
                    }
                    print(c);"
                ] = 0.0,

                [@"var ok = true; var a=10; var b=0; var c = 0; 
                    if(a >= 10 and ok) {  b = 1;  }
                    if(b > 0) { c = 1; }
                    print(c);"
                ] = 1.0,

                [@"var ok = true; var a=10; var b=0; var c = 0; 
                    if(a >= 10 and ok) b = 1;
                    if(b > 0) c = 1;
                    print(c);"
                ] = 1.0,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key}"));
        }

        [Test]
        public void WhileLoop()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                [@"var a = 1;
                   while (a < 5) {
                     a = a + 1;
                   }
                   print(a);"
                ] = 5.0,
                [@"var a = 1;
                   while (a <= 5) {
                     a = a + 1;
                   }
                   print(a);"
                ] = 6.0,
                [@"var a = 1;
                   while (a <= 5) a = a + 1;
                   print(a);"
                ] = 6.0,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key}"));
        }

        [Test]
        public void ForLoop()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                [@"for (var i = 1; i < 5; i = i + 1) {
                        print(i);
                   }
                   print(i);"
                ] = 5.0,
                [@"for (var i = 1; i <= 5; i = i + 1) {
                        print(i);
                   }
                   print(i);"
                ] = 6.0,

                [@"var cumul = 0;
                   for (var i = 1; i <= 5; i = i + 1) {
                        cumul = cumul + i;
                   }
                   print(cumul);"
                ] = 15.0,

            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key}"));
        }

        [Test]
        public void UserFunction()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                [@"function factorial(n) {
                     if (n <= 1) return 1;
                       return n * factorial(n - 1);
                   }
                   print(factorial(5));  // Should print 120
                "] = 120.0,

                [@"var c; 
                   function add(a, b) {
                       return a + b;
                   }
                   c = add(5, 3); // Should print 8
                   print(c);
                "] = 8.0,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key}"));
        }

        [Test]
        public void DeclareVar()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                [@"var c; c = 1; print(c);"] = 1.0,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key}"));
        }

        [Test]
        public void MissingDeclareVar()
        {
            var testExpressions = new Dictionary<string, double>()
            {
                [@"c = 1; print(c);"] = 1.0,
            };

            // This should throw an exception
            foreach (var expression in testExpressions)
                Assert.Throws<RuntimeException>(() => FredInterpreter.Run($"{expression.Key}"));
        }
    }
}