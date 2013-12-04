using System;

namespace Tharga.Reporter.Engine
{
    static class ExteptionExtension
    {
        public static Exception AddData(this Exception exp, object key, object value)
        {
            exp.Data.Add(key, value);
            return exp;
        }
    }
}