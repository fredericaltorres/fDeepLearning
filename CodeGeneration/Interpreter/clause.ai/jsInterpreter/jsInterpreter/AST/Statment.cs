using System.Collections.Generic;

namespace JSInterpreter
{
    #region AST

    public abstract class Statment
    {
        public interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
            T VisitForStmt(For stmt);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Block : Statment
        {
            public readonly List<Statment> Statements;

            public Block(List<Statment> statements)
            {
                Statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

        public class Expression : Statment
        {
            public readonly ASTExpr Expr;

            public Expression(ASTExpr expr)
            {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Function : Statment
        {
            public readonly Token Name;
            public readonly List<Token> Params;
            public readonly List<Statment> Body;

            public Function(Token name, List<Token> @params, List<Statment> body)
            {
                Name = name;
                Params = @params;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
        }

        public class If : Statment
        {
            public readonly ASTExpr Condition;
            public readonly Statment ThenBranch;
            public readonly Statment ElseBranch;

            public If(ASTExpr condition, Statment thenBranch, Statment elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        public class Print : Statment
        {
            public readonly ASTExpr Expr;

            public Print(ASTExpr expr)
            {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        public class Return : Statment
        {
            public readonly Token Keyword;
            public readonly ASTExpr Value;

            public Return(Token keyword, ASTExpr value)
            {
                Keyword = keyword;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }

        public class Var : Statment
        {
            public readonly Token Name;
            public readonly ASTExpr Initializer;

            public Var(Token name, ASTExpr initializer)
            {
                Name = name;
                Initializer = initializer;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

        public class While : Statment
        {
            public readonly ASTExpr Condition;
            public readonly Statment Body;

            public While(ASTExpr condition, Statment body)
            {
                Condition = condition;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }

        public class For : Statment
        {
            public readonly Statment Initializer;
            public readonly ASTExpr Condition;
            public readonly ASTExpr Increment;
            public readonly Statment Body;

            public For(Statment initializer, ASTExpr condition, ASTExpr increment, Statment body)
            {
                Initializer = initializer;
                Condition = condition;
                Increment = increment;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitForStmt(this);
            }
        }
    }

    #endregion
}