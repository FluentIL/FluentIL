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
            return Try(@body, null, catches);
        }

        public DynamicMethodBody Try(
            Action<DynamicMethodBody> @body,
            Action<DynamicMethodBody> @finally,
            params CatchBody[] catches
            )
        {
            return Try(@body, @finally, null, catches);
        }

        public DynamicMethodBody Try(
            Action<DynamicMethodBody> @body,
            Action<DynamicMethodBody> @finally,
            Action<DynamicMethodBody> @fault,
            params CatchBody[] catches
            )
        {
            
            var il = (ReflectionILEmitter)methodInfoField.GetILEmitter();

            var @tryLabel = il.BeginExceptionBlock();

            @body(this);
            il.Emit(OpCodes.Leave, @tryLabel);

            foreach (var catchBody in catches)
            {
                il.BeginCatchBlock(catchBody.ExceptionType);
                catchBody.Body(this);
                il.Emit(OpCodes.Leave, @tryLabel);
            }


            if (@finally != null)
            {
                il.BeginFinallyBlock();
                @finally(this);
            }
            il.EndExceptionBlock();



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
