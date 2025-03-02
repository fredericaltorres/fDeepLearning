using NUnit.Framework;
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
            var testExpression = new Dictionary<string, double>()
            {
                ["2 + 3"] = 5.0,
                ["5 - 2"] = 3.0,
                ["10 / 2"] = 5.0,
                ["(2 + 3) * 4"] = 20.0,
            };
            foreach (var expression in testExpression)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"({expression.Key});"));
        }

        [Test]
        public void ComparaisonExpressions()
        {
            var testExpression = new Dictionary<string, bool>()
            {
                ["5 > 3"]  = true,
                ["5 < 3"]  = !true,
                ["5 >= 5"] = true,
                ["5 <= 3"] = !true,
                ["5 == 5"] = true,
                ["5 != 5"] = !true,
            };
            foreach (var expression in testExpression)
                Assert.AreEqual(expression.Value, FredInterpreter.Run($"({expression.Key});"));
        }
    }
}