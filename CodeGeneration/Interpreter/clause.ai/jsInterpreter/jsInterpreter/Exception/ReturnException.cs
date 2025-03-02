using System;

namespace JSInterpreter
{
    #region Interpreter

    public class ReturnException : Exception
    {
        public readonly object Value;

        public ReturnException(object value)
        {
            Value = value;
        }
    }

    #endregion
}