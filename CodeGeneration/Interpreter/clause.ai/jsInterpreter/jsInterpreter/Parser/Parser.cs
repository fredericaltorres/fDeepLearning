using System;
using System.Collections.Generic;

namespace JSInterpreter
{
    #region AST

    #endregion

    #region Parser

    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Statment> Parse()
        {
            List<Statment> statements = new List<Statment>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Statment Declaration()
        {
            try
            {
                if (Match(TokenType.FUNCTION)) return Function("function");
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (Exception)
            {
                Synchronize();
                return null;
            }
        }

        private Statment Function(string kind)
        {
            Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
            Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
            List<Token> parameters = new List<Token>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (Match(TokenType.COMMA));
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
            List<Statment> body = Block();
            return new Statment.Function(name, parameters, body);
        }

        private Statment VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            ASTExpr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Statment.Var(name, initializer);
        }

        private Statment Statement()
        {
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Statment.Block(Block());

            return ExpressionStatement();
        }

        private Statment ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            Statment initializer;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            ASTExpr condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            ASTExpr increment = null;
            if (!Check(TokenType.RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            Statment body = Statement();

            return new Statment.For(initializer, condition, increment, body);
        }

        private Statment IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            ASTExpr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Statment thenBranch = Statement();
            Statment elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new Statment.If(condition, thenBranch, elseBranch);
        }

        private Statment PrintStatement()
        {
            ASTExpr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Statment.Print(value);
        }

        private Statment ReturnStatement()
        {
            Token keyword = Previous();
            ASTExpr value = null;
            if (!Check(TokenType.SEMICOLON))
            {
                value = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Statment.Return(keyword, value);
        }

        private Statment WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            ASTExpr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Statment body = Statement();

            return new Statment.While(condition, body);
        }

        private List<Statment> Block()
        {
            List<Statment> statements = new List<Statment>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Statment ExpressionStatement()
        {
            ASTExpr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Statment.Expression(expr);
        }

        private ASTExpr Expression()
        {
            return Assignment();
        }

        private ASTExpr Assignment()
        {
            ASTExpr expr = Or();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                ASTExpr value = Assignment();

                if (expr is ASTExpr.Variable)
                {
                    Token name = ((ASTExpr.Variable)expr).Name;
                    return new ASTExpr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private ASTExpr Or()
        {
            ASTExpr expr = And();

            while (Match(TokenType.OR))
            {
                Token @operator = Previous();
                ASTExpr right = And();
                expr = new ASTExpr.Logical(expr, @operator, right);
            }

            return expr;
        }

        private ASTExpr And()
        {
            ASTExpr expr = Equality();

            while (Match(TokenType.AND))
            {
                Token @operator = Previous();
                ASTExpr right = Equality();
                expr = new ASTExpr.Logical(expr, @operator, right);
            }

            return expr;
        }

        private ASTExpr Equality()
        {
            ASTExpr expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token @operator = Previous();
                ASTExpr right = Comparison();
                expr = new ASTExpr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private ASTExpr Comparison()
        {
            ASTExpr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token @operator = Previous();
                ASTExpr right = Term();
                expr = new ASTExpr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private ASTExpr Term()
        {
            ASTExpr expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token @operator = Previous();
                ASTExpr right = Factor();
                expr = new ASTExpr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private ASTExpr Factor()
        {
            ASTExpr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token @operator = Previous();
                ASTExpr right = Unary();
                expr = new ASTExpr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private ASTExpr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token @operator = Previous();
                ASTExpr right = Unary();
                return new ASTExpr.Unary(@operator, right);
            }

            return Call();
        }

        private ASTExpr Call()
        {
            ASTExpr expr = Primary();

            while (true)
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private ASTExpr FinishCall(ASTExpr callee)
        {
            List<ASTExpr> arguments = new List<ASTExpr>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments.");
                    }
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }

            Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

            return new ASTExpr.Call(callee, paren, arguments);
        }

        private ASTExpr Primary()
        {
            if (Match(TokenType.FALSE)) return new ASTExpr.Literal(false);
            if (Match(TokenType.TRUE)) return new ASTExpr.Literal(true);
            if (Match(TokenType.NULL)) return new ASTExpr.Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new ASTExpr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new ASTExpr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                ASTExpr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new ASTExpr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Exception Error(Token token, string message)
        {
            // Report the error
            if (token.Type == TokenType.EOF)
            {
                Console.WriteLine($"Error at end: {message}");
            }
            else
            {
                Console.WriteLine($"Error at '{token.Lexeme}': {message}");
            }

            return new Exception("Parse error");
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.FUNCTION:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }

    #endregion
}