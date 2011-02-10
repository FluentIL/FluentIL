using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
	public partial class DynamicMethodBody
	{
		public DynamicMethodBody Emit(OpCode opcode, string arg)
        {
            _Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

		public DynamicMethodBody Emit(OpCode opcode, int arg)
        {
            _Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

		public DynamicMethodBody Emit(OpCode opcode, double arg)
        {
            _Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

		public DynamicMethodBody Emit(OpCode opcode, Label arg)
        {
            _Info.AsDynamicMethod.GetILGenerator()
                .Emit(opcode, arg);

            return this;
        }

	}
}
