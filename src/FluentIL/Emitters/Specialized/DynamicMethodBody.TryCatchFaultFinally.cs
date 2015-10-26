using System;
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
            var il = (ReflectionILEmitter)_methodInfo.GetILEmitter();
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
            var il = (ReflectionILEmitter)_methodInfo.GetILEmitter();

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
            Body = body;
        }

        public override Type ExceptionType => typeof (T);
        public override Action<DynamicMethodBody> Body { get; }
    }
}
