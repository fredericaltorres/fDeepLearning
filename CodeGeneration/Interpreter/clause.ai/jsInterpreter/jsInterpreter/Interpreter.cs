using System;
using System.Collections.Generic;

namespace JSInterpreter
{
    #region Interpreter

    public class Interpreter : ASTExpr.IVisitor<object>, Statment.IVisitor<object>
    {
        public readonly InterpreterEnvironment Globals = new InterpreterEnvironment();
        private InterpreterEnvironment _environment;

        public Interpreter()
        {
            _environment = Globals;

            // Define native functions
            // Globals.Define("clock", new ClockFunction());
        }

        public void Interpret(List<Statment> statements)
        {
            try
            {
                foreach (var statement in statements)
                    Execute(statement);
            }
            catch (RuntimeException error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private void Execute(Statment stmt)
        {
            stmt.Accept(this);
        }

        public void ExecuteBlock(List<Statment> statements, InterpreterEnvironment environment)
        {
            InterpreterEnvironment previous = _environment;
            try
            {
                _environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previous;
            }
        }

        public object VisitBlockStmt(Statment.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new InterpreterEnvironment(_environment));
            return null;
        }

        public object VisitExpressionStmt(Statment.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        public object VisitFunctionStmt(Statment.Function stmt)
        {
            Function function = new Function(stmt, _environment);
            _environment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        public object VisitIfStmt(Statment.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            return null;
        }

        public object VisitPrintStmt(Statment.Print stmt)
        {
            object value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitReturnStmt(Statment.Return stmt)
        {
            object value = null;
            if (stmt.Value != null) value = Evaluate(stmt.Value);

            throw new ReturnException(value);
        }

        public object VisitVarStmt(Statment.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            _environment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public object VisitWhileStmt(Statment.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }
            return null;
        }

        public object VisitForStmt(Statment.For stmt)
        {
            if (stmt.Initializer != null)
            {
                Execute(stmt.Initializer);
            }

            while (stmt.Condition == null || IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);

                if (stmt.Increment != null)
                {
                    Evaluate(stmt.Increment);
                }
            }

            return null;
        }

        public object VisitAssignExpr(ASTExpr.Assign expr)
        {
            object value = Evaluate(expr.Value);
            _environment.Assign(expr.Name, value);
            return value;
        }

        public object VisitBinaryExpr(ASTExpr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    if (left is string && right is double)
                    {
                        return (string)left + Stringify(right);
                    }

                    if (left is double && right is string)
                    {
                        return Stringify(left) + (string)right;
                    }

                    throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            return null;
        }

        public object VisitCallExpr(ASTExpr.Call expr)
        {
            object callee = Evaluate(expr.Callee);

            List<object> arguments = new List<object>();
            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeException(expr.Paren, "Can only call functions and classes.");
            }

            ICallable function = (ICallable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new RuntimeException(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object VisitGroupingExpr(ASTExpr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(ASTExpr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(ASTExpr.Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Operator.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public object VisitUnaryExpr(ASTExpr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            return null;
        }

        public object VisitVariableExpr(ASTExpr.Variable expr)
        {
            return _environment.Get(expr.Name);
        }

        public object LastEvaluated; 

        private object Evaluate(ASTExpr expr)
        {
            var r = expr.Accept(this);
            if (r != null)
                LastEvaluated = r;
            else
                LastEvaluated = LastEvaluated;

            return LastEvaluated;
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is double) return;
            throw new RuntimeException(@operator, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeException(@operator, "Operands must be numbers.");
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "null";

            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }
    }

    #endregion
}