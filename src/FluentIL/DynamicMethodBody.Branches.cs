using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace FluentIL
{
	public partial class DynamicMethodBody
	{
		readonly Stack<IfEmitter> _IfEmitters = new Stack<IfEmitter>();
		public DynamicMethodBody Else()
		{
			_IfEmitters.Peek().EmitElse();
			return this;
		}

		public DynamicMethodBody EndIf()
		{
			_IfEmitters.Pop().EmitEndIf();
			return this;
		}

		#region Beq
		public DynamicMethodBody Beq(string label)
        {
            return Beq(GetLabel(label));
        }

        public DynamicMethodBody Beq(Label label)
        {
            return Emit(OpCodes.Beq, label);
        }

		public DynamicMethodBody Ceq()
		{
			return Emit(OpCodes.Ceq);
		}

		public DynamicMethodBody Ifeq()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIf(OpCodes.Ceq);
			return this;
		}

		public DynamicMethodBody IfNoteq()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIfNot(OpCodes.Ceq);
			return this;
		}

		#endregion

		#region Beq_S
		public DynamicMethodBody Beq_S(string label)
        {
            return Beq_S(GetLabel(label));
        }

        public DynamicMethodBody Beq_S(Label label)
        {
            return Emit(OpCodes.Beq_S, label);
        }

		#endregion

		#region Bne_Un
		public DynamicMethodBody Bne_Un(string label)
        {
            return Bne_Un(GetLabel(label));
        }

        public DynamicMethodBody Bne_Un(Label label)
        {
            return Emit(OpCodes.Bne_Un, label);
        }

		#endregion

		#region Bne_Un_S
		public DynamicMethodBody Bne_Un_S(string label)
        {
            return Bne_Un_S(GetLabel(label));
        }

        public DynamicMethodBody Bne_Un_S(Label label)
        {
            return Emit(OpCodes.Bne_Un_S, label);
        }

		#endregion

		#region Bge
		public DynamicMethodBody Bge(string label)
        {
            return Bge(GetLabel(label));
        }

        public DynamicMethodBody Bge(Label label)
        {
            return Emit(OpCodes.Bge, label);
        }

		#endregion

		#region Bge_S
		public DynamicMethodBody Bge_S(string label)
        {
            return Bge_S(GetLabel(label));
        }

        public DynamicMethodBody Bge_S(Label label)
        {
            return Emit(OpCodes.Bge_S, label);
        }

		#endregion

		#region Bge_Un
		public DynamicMethodBody Bge_Un(string label)
        {
            return Bge_Un(GetLabel(label));
        }

        public DynamicMethodBody Bge_Un(Label label)
        {
            return Emit(OpCodes.Bge_Un, label);
        }

		#endregion

		#region Bge_Un_S
		public DynamicMethodBody Bge_Un_S(string label)
        {
            return Bge_Un_S(GetLabel(label));
        }

        public DynamicMethodBody Bge_Un_S(Label label)
        {
            return Emit(OpCodes.Bge_Un_S, label);
        }

		#endregion

		#region Bgt
		public DynamicMethodBody Bgt(string label)
        {
            return Bgt(GetLabel(label));
        }

        public DynamicMethodBody Bgt(Label label)
        {
            return Emit(OpCodes.Bgt, label);
        }

		public DynamicMethodBody Cgt()
		{
			return Emit(OpCodes.Cgt);
		}

		public DynamicMethodBody Ifgt()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIf(OpCodes.Cgt);
			return this;
		}

		public DynamicMethodBody IfNotgt()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIfNot(OpCodes.Cgt);
			return this;
		}

		#endregion

		#region Bgt_S
		public DynamicMethodBody Bgt_S(string label)
        {
            return Bgt_S(GetLabel(label));
        }

        public DynamicMethodBody Bgt_S(Label label)
        {
            return Emit(OpCodes.Bgt_S, label);
        }

		#endregion

		#region Bgt_Un
		public DynamicMethodBody Bgt_Un(string label)
        {
            return Bgt_Un(GetLabel(label));
        }

        public DynamicMethodBody Bgt_Un(Label label)
        {
            return Emit(OpCodes.Bgt_Un, label);
        }

		public DynamicMethodBody Cgt_Un()
		{
			return Emit(OpCodes.Cgt_Un);
		}

		public DynamicMethodBody Ifgt_Un()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIf(OpCodes.Cgt_Un);
			return this;
		}

		public DynamicMethodBody IfNotgt_Un()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIfNot(OpCodes.Cgt_Un);
			return this;
		}

		#endregion

		#region Bgt_Un_S
		public DynamicMethodBody Bgt_Un_S(string label)
        {
            return Bgt_Un_S(GetLabel(label));
        }

        public DynamicMethodBody Bgt_Un_S(Label label)
        {
            return Emit(OpCodes.Bgt_Un_S, label);
        }

		#endregion

		#region Ble
		public DynamicMethodBody Ble(string label)
        {
            return Ble(GetLabel(label));
        }

        public DynamicMethodBody Ble(Label label)
        {
            return Emit(OpCodes.Ble, label);
        }

		#endregion

		#region Ble_S
		public DynamicMethodBody Ble_S(string label)
        {
            return Ble_S(GetLabel(label));
        }

        public DynamicMethodBody Ble_S(Label label)
        {
            return Emit(OpCodes.Ble_S, label);
        }

		#endregion

		#region Ble_Un
		public DynamicMethodBody Ble_Un(string label)
        {
            return Ble_Un(GetLabel(label));
        }

        public DynamicMethodBody Ble_Un(Label label)
        {
            return Emit(OpCodes.Ble_Un, label);
        }

		#endregion

		#region Ble_Un_S
		public DynamicMethodBody Ble_Un_S(string label)
        {
            return Ble_Un_S(GetLabel(label));
        }

        public DynamicMethodBody Ble_Un_S(Label label)
        {
            return Emit(OpCodes.Ble_Un_S, label);
        }

		#endregion

		#region Blt
		public DynamicMethodBody Blt(string label)
        {
            return Blt(GetLabel(label));
        }

        public DynamicMethodBody Blt(Label label)
        {
            return Emit(OpCodes.Blt, label);
        }

		public DynamicMethodBody Clt()
		{
			return Emit(OpCodes.Clt);
		}

		public DynamicMethodBody Iflt()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIf(OpCodes.Clt);
			return this;
		}

		public DynamicMethodBody IfNotlt()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIfNot(OpCodes.Clt);
			return this;
		}

		#endregion

		#region Blt_S
		public DynamicMethodBody Blt_S(string label)
        {
            return Blt_S(GetLabel(label));
        }

        public DynamicMethodBody Blt_S(Label label)
        {
            return Emit(OpCodes.Blt_S, label);
        }

		#endregion

		#region Blt_Un
		public DynamicMethodBody Blt_Un(string label)
        {
            return Blt_Un(GetLabel(label));
        }

        public DynamicMethodBody Blt_Un(Label label)
        {
            return Emit(OpCodes.Blt_Un, label);
        }

		public DynamicMethodBody Clt_Un()
		{
			return Emit(OpCodes.Clt_Un);
		}

		public DynamicMethodBody Iflt_Un()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIf(OpCodes.Clt_Un);
			return this;
		}

		public DynamicMethodBody IfNotlt_Un()
		{
			var emitter = new IfEmitter(this);
			_IfEmitters.Push(emitter);
			emitter.EmitIfNot(OpCodes.Clt_Un);
			return this;
		}

		#endregion

		#region Blt_Un_S
		public DynamicMethodBody Blt_Un_S(string label)
        {
            return Blt_Un_S(GetLabel(label));
        }

        public DynamicMethodBody Blt_Un_S(Label label)
        {
            return Emit(OpCodes.Blt_Un_S, label);
        }

		#endregion

		#region Brtrue
		public DynamicMethodBody Brtrue(string label)
        {
            return Brtrue(GetLabel(label));
        }

        public DynamicMethodBody Brtrue(Label label)
        {
            return Emit(OpCodes.Brtrue, label);
        }

		#endregion

		#region Brfalse
		public DynamicMethodBody Brfalse(string label)
        {
            return Brfalse(GetLabel(label));
        }

        public DynamicMethodBody Brfalse(Label label)
        {
            return Emit(OpCodes.Brfalse, label);
        }

		#endregion

		#region Br
		public DynamicMethodBody Br(string label)
        {
            return Br(GetLabel(label));
        }

        public DynamicMethodBody Br(Label label)
        {
            return Emit(OpCodes.Br, label);
        }

		#endregion

		#region Br_S
		public DynamicMethodBody Br_S(string label)
        {
            return Br_S(GetLabel(label));
        }

        public DynamicMethodBody Br_S(Label label)
        {
            return Emit(OpCodes.Br_S, label);
        }

		#endregion

	}
}
