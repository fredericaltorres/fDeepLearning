using System.Collections.Generic;

namespace JSInterpreter
{
    #region AST

    public abstract class ASTExpr
    {
        public interface IVisitor<T>
        {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitLogicalExpr(Logical expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Assign : ASTExpr
        {
            public readonly Token Name;
            public readonly ASTExpr Value;

            public Assign(Token name, ASTExpr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }

        public class Binary : ASTExpr
        {
            public readonly ASTExpr Left;
            public readonly Token Operator;
            public readonly ASTExpr Right;

            public Binary(ASTExpr left, Token @operator, ASTExpr right)
            {
                Left = left;
                Operator = @operator;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Call : ASTExpr
        {
            public readonly ASTExpr Callee;
            public readonly Token Paren;
            public readonly List<ASTExpr> Arguments;

            public Call(ASTExpr callee, Token paren, List<ASTExpr> arguments)
            {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        public class Grouping : ASTExpr
        {
            public readonly ASTExpr Expression;

            public Grouping(ASTExpr expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Literal : ASTExpr
        {
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Logical : ASTExpr
        {
            public readonly ASTExpr Left;
            public readonly Token Operator;
            public readonly ASTExpr Right;

            public Logical(ASTExpr left, Token @operator, ASTExpr right)
            {
                Left = left;
                Operator = @operator;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        public class Unary : ASTExpr
        {
            public readonly Token Operator;
            public readonly ASTExpr Right;

            public Unary(Token @operator, ASTExpr right)
            {
                Operator = @operator;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class Variable : ASTExpr
        {
            public readonly Token Name;

            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }
    }

    #endregion
}