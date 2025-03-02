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
                ["true and true"] = true,
                ["true and false"] = true,
                ["true or true"] = true,
                ["true or false"] = true,
                ["false or false"] = false,
                ["!false"] = true,
                ["!true"] = false,
                ["!false and true"] = true,
                ["true and !true"] = false,
            };
            foreach (var expression in testExpressions)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"{expression.Key};"));
        }
    }
}