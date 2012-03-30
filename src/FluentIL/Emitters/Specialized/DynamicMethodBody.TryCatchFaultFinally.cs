using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

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
            var il = (ReflectionILEmitter)methodInfoField.GetILEmitter();
            var @tryLabel = il.BeginExceptionBlock();

#if DEBUG
            Console.WriteLine("try {");
            Console.WriteLine("IL_{0}:", @tryLabel.GetHashCode());
#endif

            @body(this);
            Emit(OpCodes.Leave, @tryLabel);

            foreach (var catchBody in catches)
            {
                il.BeginCatchBlock(catchBody.ExceptionType);
                catchBody.Body(this);
                Emit(OpCodes.Leave, @tryLabel);
            }

            il.EndExceptionBlock();
#if DEBUG
            Console.WriteLine("} // end of try block");
#endif


            return this;
        }

        public DynamicMethodBody Try(
            Action<DynamicMethodBody> @body,
            Action<DynamicMethodBody> @finally,
            params CatchBody[] catches
            )
        {
            var il = (ReflectionILEmitter)methodInfoField.GetILEmitter();

            var tryFinally = il.BeginExceptionBlock();
#if DEBUG
            Console.WriteLine("{");
            Console.WriteLine("IL_{0}:", @tryFinally.GetHashCode());
#endif

            Try(@body, catches);
            Emit(OpCodes.Leave, tryFinally);

            if (@finally != null)
            {
                il.BeginFinallyBlock();
                @finally(this);
            }

            il.EndExceptionBlock();

#if DEBUG
            Console.WriteLine("} // end of finally block");
#endif
            return this;
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
