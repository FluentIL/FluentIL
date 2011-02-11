using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
	public partial class DynamicMethodBody
	{

		#region Common Opcodes
        
		public DynamicMethodBody Ret()
        {
            return Emit(OpCodes.Ret);
        }

		public DynamicMethodBody Dup()
        {
            return Emit(OpCodes.Dup);
        }

		public DynamicMethodBody Pop()
        {
            return Emit(OpCodes.Pop);
        }

		public DynamicMethodBody LdelemU1()
        {
            return Emit(OpCodes.Ldelem_U1);
        }

		public DynamicMethodBody StelemI1()
        {
            return Emit(OpCodes.Stelem_I1);
        }

		public DynamicMethodBody LdelemI1()
        {
            return Emit(OpCodes.Ldelem_I1);
        }

		public DynamicMethodBody ConvR8()
        {
            return Emit(OpCodes.Conv_R8);
        }

		public DynamicMethodBody ConvU1()
        {
            return Emit(OpCodes.Conv_U1);
        }

		public DynamicMethodBody Neg()
        {
            return Emit(OpCodes.Neg);
        }

		#endregion
	}
}

