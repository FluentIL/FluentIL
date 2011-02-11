using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Diagnostics;

namespace FluentIL
{
	public partial class DynamicMethodBody
	{
		readonly Stack<Action> PreEmitActions = new Stack<Action>();
		private void ExecutePreEmitActions()
		{
			while ( PreEmitActions.Count > 0 ) 
                PreEmitActions.Pop()();
		}

		#region Emit (basic)
        public DynamicMethodBody Emit(OpCode opcode)
        {
			ExecutePreEmitActions();
			Debug.WriteLine(opcode);
            _Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode);

            return this;
        }
        #endregion


		public DynamicMethodBody Emit(OpCode opcode, string arg)
        {
			ExecutePreEmitActions();
			Debug.WriteLine("{0} {1}", opcode, arg.ToString());
			_Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

		public DynamicMethodBody Emit(OpCode opcode, int arg)
        {
			ExecutePreEmitActions();
			Debug.WriteLine("{0} {1}", opcode, arg.ToString());
			_Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

		public DynamicMethodBody Emit(OpCode opcode, double arg)
        {
			ExecutePreEmitActions();
			Debug.WriteLine("{0} {1}", opcode, arg.ToString());
			_Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

		public DynamicMethodBody Emit(OpCode opcode, Label arg)
        {
			ExecutePreEmitActions();
			Debug.WriteLine("{0} {1}", opcode, arg.ToString());
			_Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

	}
}
