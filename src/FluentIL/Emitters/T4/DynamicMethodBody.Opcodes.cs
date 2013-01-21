﻿using System.Reflection.Emit;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
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

        public DynamicMethodBody Ldelem_Ref()
        {
            return Emit(OpCodes.Ldelem_Ref);
        }

        public DynamicMethodBody Stelem_Ref()
        {
            return Emit(OpCodes.Stelem_Ref);
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
		
		

		public DynamicMethodBody Throw()
        {
            return Emit(OpCodes.Throw);
        }
		
		

		public DynamicMethodBody Div()
        {
            return Emit(OpCodes.Div);
        }
		
		

		public DynamicMethodBody Mul()
        {
            return Emit(OpCodes.Mul);
        }
		
		

		public DynamicMethodBody Add()
        {
            return Emit(OpCodes.Add);
        }
		
		

		public DynamicMethodBody Sub()
        {
            return Emit(OpCodes.Sub);
        }
		
		

		public DynamicMethodBody Rem()
        {
            return Emit(OpCodes.Rem);
        }
		
		

		public DynamicMethodBody Not()
        {
            return Emit(OpCodes.Not);
        }				

		public DynamicMethodBody Nop()
        {
            return Emit(OpCodes.Nop);
        }
			
		public DynamicMethodBody Ldnull()
        {
            return Emit(OpCodes.Ldnull);
        }
		
		

		#endregion
	}
}

