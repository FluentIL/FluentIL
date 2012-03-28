using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
    partial class DynamicMethodBody
    {
        public DynamicMethodBody Try(
            Action<DynamicMethodBody> @body,
            params CatchBody[] catches
            )
        {
            throw new NotImplementedException();
        }

        public DynamicMethodBody Try(
            Action<DynamicMethodBody> @body,
            Action<DynamicMethodBody> @finally,
            params CatchBody[] catches
            )
        {
            throw new NotImplementedException();
        }

        public DynamicMethodBody Try(
            Action<DynamicMethodBody> @body,
            Action<DynamicMethodBody> @finally,
            Action<DynamicMethodBody> @fault,
            params CatchBody[] catches
            )
        {
            throw new NotImplementedException();
        }
    }

    public abstract class CatchBody
    {
        public abstract Type ExceptionType { get;  }
        public abstract Action<DynamicMethodBody> Body { get;  }
    }

    public class CatchBody<T> : CatchBody 
        where T : Exception
    {
        public CatchBody(Action<DynamicMethodBody> body )
        {
            bodyField = body;
        }

        public override Type ExceptionType
        {
            get { return typeof (T); }
        }

        private readonly Action<DynamicMethodBody> bodyField;
        public override Action<DynamicMethodBody> Body {
            get { return bodyField; }
        }
    }
}
