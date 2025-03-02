using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenType
{
    Number, Identifier, Operator, LeftParen, RightParen, Semicolon, Equal,
    While, For, Function, Print, EOF, LessThan, LeftCurly, RightCurly
}

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
    public override string ToString()
    {
        return $"Token({Type}, {Value})";
    }
}

public class Lexer
{
    private string _input;
    private int _position;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        while (_position < _input.Length)
        {
            char currentChar = _input[_position];
            if (char.IsWhiteSpace(currentChar))
            {
                _position++;
            }
            else if (char.IsDigit(currentChar))
            {
                tokens.Add(new Token(TokenType.Number, ReadNumber()));
            }
            else if (char.IsLetter(currentChar))
            {
                string identifier = ReadIdentifier();
                switch (identifier)
                {
                    case "while":
                        tokens.Add(new Token(TokenType.While, identifier));
                        break;
                    case "for":
                        tokens.Add(new Token(TokenType.For, identifier));
                        break;
                    case "function":
                        tokens.Add(new Token(TokenType.Function, identifier));
                        break;
                    case "print":
                        tokens.Add(new Token(TokenType.Print, identifier));
                        break;
                    default:
                        tokens.Add(new Token(TokenType.Identifier, identifier));
                        break;
                }
            }
            else if (Regex.IsMatch(currentChar.ToString(), @"[+\-*/]"))
            {
                tokens.Add(new Token(TokenType.Operator, currentChar.ToString()));
                _position++;
            }
            else if (currentChar == '{')
            {
                tokens.Add(new Token(TokenType.LeftCurly, "{"));
                _position++;
            }
            else if (currentChar == '}')
            {
                tokens.Add(new Token(TokenType.RightCurly, "}"));
                _position++;
            }
            else if (currentChar == '<')
            {
                tokens.Add(new Token(TokenType.LessThan, "<"));
                _position++;
            }
            else if (currentChar == '(')
            {
                tokens.Add(new Token(TokenType.LeftParen, "("));
                _position++;
            }
            else if (currentChar == ')')
            {
                tokens.Add(new Token(TokenType.RightParen, ")"));
                _position++;
            }
            else if (currentChar == ';')
            {
                tokens.Add(new Token(TokenType.Semicolon, ";"));
                _position++;
            }
            else if (currentChar == '=')
            {
                tokens.Add(new Token(TokenType.Equal, "="));
                _position++;
            }
            else
            {
                throw new Exception($"Unexpected character: {currentChar}");
            }
        }
        tokens.Add(new Token(TokenType.EOF, ""));
        return tokens;
    }

    private string ReadNumber()
    {
        int start = _position;
        while (_position < _input.Length && char.IsDigit(_input[_position]))
        {
            _position++;
        }
        return _input.Substring(start, _position - start);
    }

    private string ReadIdentifier()
    {
        int start = _position;
        while (_position < _input.Length && char.IsLetterOrDigit(_input[_position]))
        {
            _position++;
        }
        return _input.Substring(start, _position - start);
    }
}

public abstract class ASTNode 
{ 
    public override string ToString()
    {
        return $"ASTNode {GetType().Name}";
    }
}

public class NumberNode : ASTNode
{
    public int Value { get; }
    public NumberNode(int value) { Value = value; }
}

public class VariableNode : ASTNode
{
    public string Name { get; }
    public VariableNode(string name) { Name = name; }
}

public class BinaryOpNode : ASTNode
{
    public ASTNode Left { get; }
    public string Operator { get; }
    public ASTNode Right { get; }
    public BinaryOpNode(ASTNode left, string op, ASTNode right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}

public class AssignmentNode : ASTNode
{
    public string Variable { get; }
    public ASTNode Value { get; }
    public AssignmentNode(string variable, ASTNode value)
    {
        Variable = variable;
        Value = value;
    }
}

public class SemicolonNode : ASTNode
{
    // This class might be empty or contain minimal information
    // as semicolons typically don't carry semantic meaning
}

public class WhileNode : ASTNode
{
    public ASTNode Condition { get; }
    public List<ASTNode> Body { get; }
    public WhileNode(ASTNode condition, List<ASTNode> body)
    {
        Condition = condition;
        Body = body;
    }
}

public class ForNode : ASTNode
{
    public AssignmentNode Initialization { get; }
    public ASTNode Condition { get; }
    public AssignmentNode Increment { get; }
    public List<ASTNode> Body { get; }
    public ForNode(AssignmentNode init, ASTNode condition, AssignmentNode increment, List<ASTNode> body)
    {
        Initialization = init;
        Condition = condition;
        Increment = increment;
        Body = body;
    }
}

public class FunctionNode : ASTNode
{
    public string Name { get; }
    public List<string> Parameters { get; }
    public List<ASTNode> Body { get; }
    public FunctionNode(string name, List<string> parameters, List<ASTNode> body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }
}

public class FunctionCallNode : ASTNode
{
    public string Name { get; }
    public List<ASTNode> Arguments { get; }
    public FunctionCallNode(string name, List<ASTNode> arguments)
    {
        Name = name;
        Arguments = arguments;
    }
}

public class PrintNode : ASTNode
{
    public ASTNode Expression { get; }
    public PrintNode(ASTNode expression) { Expression = expression; }
}

public class Parser
{
    private List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public List<ASTNode> Parse()
    {
        var program = new List<ASTNode>();
        while (Current().Type != TokenType.EOF)
        {
            program.Add(ParseStatement());
        }
        return program;
    }

    private ASTNode ParseStatement()
    {
        switch (Current().Type)
        {
            case TokenType.While:
                return ParseWhileLoop();
            case TokenType.For:
                return ParseForLoop();
            case TokenType.Function:
                return ParseFunctionDefinition();
            case TokenType.Print:
                return ParsePrintStatement();
            default:
                return ParseExpression();
        }
    }

    private ASTNode ParseExpression()
    {
        var left = ParseTerm();
        while (Current().Type == TokenType.Operator && (Current().Value == "+" || Current().Value == "-"))
        {
            var op = Consume().Value;
            var right = ParseTerm();
            left = new BinaryOpNode(left, op, right);
        }
        return left;
    }

    private ASTNode ParseTerm()
    {
        var left = ParseFactor();
        while (Current().Type == TokenType.Operator && (Current().Value == "*" || Current().Value == "/"))
        {
            var op = Consume().Value;
            var right = ParseFactor();
            left = new BinaryOpNode(left, op, right);
        }
        return left;
    }

    private ASTNode ParseFactor()
    {
        if (Current().Type == TokenType.Semicolon)
        {
            var z = Consume().Value;
            return new SemicolonNode();
        }
        else if (Current().Type == TokenType.Number)
        {
            return new NumberNode(int.Parse(Consume().Value));
        }
        else if (Current().Type == TokenType.Identifier)
        {
            string name = Consume().Value;
            if (Current().Type == TokenType.LeftParen)
            {
                return ParseFunctionCall(name);
            }
            else if (Current().Type == TokenType.Equal)
            {
                Consume(); // Consume '='
                var value = ParseExpression();
                return new AssignmentNode(name, value);
            }
            return new VariableNode(name);
        }
        else if (Current().Type == TokenType.LeftParen)
        {
            Consume(); // Consume '('
            var expr = ParseExpression();
            Consume(); // Consume ')'
            return expr;
        }
        throw new Exception("Unexpected token");
    }

    private WhileNode ParseWhileLoop()
    {
        Consume(); // Consume 'while'
        Consume(); // Consume '('
        var condition = ParseExpression();
        Consume(); // Consume ')'
        var body = new List<ASTNode>();
        Consume(); // Consume '{'
        while (Current().Type != TokenType.RightParen)
        {
            body.Add(ParseStatement());
        }
        Consume(); // Consume '}'
        return new WhileNode(condition, body);
    }

    private ForNode ParseForLoop()
    {
        Consume(); // Consume 'for'
        Consume(); // Consume '('
        var init = (AssignmentNode)ParseExpression();
        Consume(); // Consume ';'
        var condition = ParseExpression();
        Consume(); // Consume ';'
        var increment = (AssignmentNode)ParseExpression();
        Consume(); // Consume ')'
        var body = new List<ASTNode>();
        Consume(); // Consume '{'
        while (Current().Type != TokenType.RightParen)
        {
            body.Add(ParseStatement());
        }
        Consume(); // Consume '}'
        return new ForNode(init, condition, increment, body);
    }

    private FunctionNode ParseFunctionDefinition()
    {
        Consume(); // Consume 'function'
        string name = Consume().Value;
        Consume(); // Consume '('
        var parameters = new List<string>();
        while (Current().Type != TokenType.RightParen)
        {
            parameters.Add(Consume().Value);
            if (Current().Type == TokenType.Operator && Current().Value == ",")
            {
                Consume(); // Consume ','
            }
        }
        Consume(); // Consume ')'
        Consume(); // Consume '{'
        var body = new List<ASTNode>();
        while (Current().Type != TokenType.RightParen)
        {
            body.Add(ParseStatement());
        }
        Consume(); // Consume '}'
        return new FunctionNode(name, parameters, body);
    }

    private FunctionCallNode ParseFunctionCall(string name)
    {
        Consume(); // Consume '('
        var arguments = new List<ASTNode>();
        while (Current().Type != TokenType.RightParen)
        {
            arguments.Add(ParseExpression());
            if (Current().Type == TokenType.Operator && Current().Value == ",")
            {
                Consume(); // Consume ','
            }
        }
        Consume(); // Consume ')'
        return new FunctionCallNode(name, arguments);
    }

    private PrintNode ParsePrintStatement()
    {
        Consume(); // Consume 'print'
        Consume(); // Consume '('
        var expression = ParseExpression();
        Consume(); // Consume ')'
        return new PrintNode(expression);
    }

    private Token Current()
    {
        return _tokens[_position];
    }

    private Token Consume()
    {
        return _tokens[_position++];
    }
}

public class Interpreter
{
    private Dictionary<string, int> _variables = new Dictionary<string, int>();
    private Dictionary<string, FunctionNode> _functions = new Dictionary<string, FunctionNode>();

    public void Interpret(List<ASTNode> ast)
    {
        foreach (var node in ast)
        {
            Execute(node);
        }
    }

    public int LastEvaluated { get; private set; }

    public int SetLE(int value)
    {
        LastEvaluated = value;
        return value;
    }

    private int Execute(ASTNode node)
    {
        if (node is SemicolonNode)
        {
            return -1;
        }
        else if (node is NumberNode numberNode)
        {
            return SetLE(numberNode.Value);
        }
        else if (node is VariableNode variableNode)
        {
            return SetLE(_variables[variableNode.Name]);
        }
        else if (node is BinaryOpNode binaryOpNode)
        {
            int left = Execute(binaryOpNode.Left);
            int right = Execute(binaryOpNode.Right);
            switch (binaryOpNode.Operator)
            {
                case "+": return SetLE(left + right);
                case "-": return SetLE(left - right);
                case "*": return SetLE(left * right);
                case "/": return SetLE(left / right);
                default: throw new Exception("Unknown operator");
            }
        }
        else if (node is AssignmentNode assignmentNode)
        {
            int value = Execute(assignmentNode.Value);
            _variables[assignmentNode.Variable] = value;
            return SetLE(value);
        }
        else if (node is WhileNode whileNode)
        {
            while (Execute(whileNode.Condition) != 0)
            {
                foreach (var bodyNode in whileNode.Body)
                {
                    SetLE(Execute(bodyNode));
                }
            }
        }
        else if (node is ForNode forNode)
        {
            Execute(forNode.Initialization);
            while (Execute(forNode.Condition) != 0)
            {
                foreach (var bodyNode in forNode.Body)
                {
                    SetLE(Execute(bodyNode));
                }
                SetLE(Execute(forNode.Increment));
            }
        }
        else if (node is FunctionNode functionNode)
        {
            _functions[functionNode.Name] = functionNode;
        }
        else if (node is FunctionCallNode functionCallNode)
        {
            var function = _functions[functionCallNode.Name];
            var localVariables = new Dictionary<string, int>(_variables);
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                localVariables[function.Parameters[i]] = Execute(functionCallNode.Arguments[i]);
            }
            var oldVariables = _variables;
            _variables = localVariables;
            int result = 0;
            foreach (var bodyNode in function.Body)
            {
                result = Execute(bodyNode);
            }
            _variables = oldVariables;
            return SetLE(result);
        }
        else if (node is PrintNode printNode)
        {
            int value = Execute(printNode.Expression);
            Console.WriteLine(value);
            return SetLE(value);
        }
        return 0;
    }
}

public class Program
{
    static Lexer _lexer;
    static Parser _parser;
    static Interpreter _interpreter;

    private static List<ASTNode> ParseCode(string code)
    {
        _lexer = new Lexer(code);
        var tokens = _lexer.Tokenize();
        _parser = new Parser(tokens);
        return _parser.Parse();
    }

    public static void Main()
    {
        var ast = ParseCode("x = 5 + 3 * 2; print(x);");
        _interpreter = new Interpreter();
        _interpreter.Interpret(ast);
    }
}