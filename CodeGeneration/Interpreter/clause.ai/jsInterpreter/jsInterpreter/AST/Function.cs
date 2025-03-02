using System.Collections.Generic;

namespace JSInterpreter
{
    public class Function : ICallable
    {
        private readonly Statment.Function _declaration;
        private readonly InterpreterEnvironment _closure;

        public Function(Statment.Function declaration, InterpreterEnvironment closure)
        {
            _declaration = declaration;
            _closure = closure;
        }

        public int Arity()
        {
            return _declaration.Params.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new InterpreterEnvironment(_closure);
            for (int i = 0; i < _declaration.Params.Count; i++)
            {
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (ReturnException returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}