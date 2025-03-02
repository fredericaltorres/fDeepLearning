using System.Collections.Generic;

namespace JSInterpreter
{
    #region Interpreter

    public interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }

    #endregion
}