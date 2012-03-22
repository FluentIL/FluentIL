using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using FluentIL.Numbers;

// ReSharper disable CheckNamespace
namespace FluentIL.Emitters
// ReSharper restore CheckNamespace
{
	public partial class DynamicMethodBody
	{
		readonly Stack<IfEmitter> ifEmittersField = new Stack<IfEmitter>();
		public DynamicMethodBody Else()
		{
			ifEmittersField.Peek().EmitElse();
			return this;
		}

		public DynamicMethodBody EndIf()
		{
			ifEmittersField.Pop().EmitEndIf();
			return this;
		}
		
		private void SaveLeftSideToVariable(Type t)
		{
			var emitter = ifEmittersField.Peek();
			var variable = emitter.LeftSideVarName;

			if (!emitter.MultipleConditions)
			{
				if (GetVariableIndex(variable) == -1)
				{
					methodInfoField.WithVariable(typeof(int), variable);
					methodInfoField.GetILEmitter().DeclareLocal(t);
				}
				preEmitActionsField.Push( () =>
				{
					Stloc(variable);
					Ldloc(variable);
				} );

				emitter.MultipleConditions = true;
			}
		}

		private DynamicMethodBody And(Number right, OpCode opcode, bool not = false)
		{
			var emitter = ifEmittersField.Peek();
			var variable = emitter.LeftSideVarName;
			var a = preEmitActionsField.Pop();

			SaveLeftSideToVariable(typeof(int));

			preEmitActionsField.Push( () =>
				{
					a();
					Ldloc(variable);
					Emit(right);
					emitter.EmitIf(opcode, not);
				}
				);
			return this;
		}

		#region Beq
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Beq(string label)
		// ReSharper restore InconsistentNaming
        {
            return Beq(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Beq(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Beq, label);
        }

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ceq()
		// ReSharper restore InconsistentNaming
		{
			return Emit(OpCodes.Ceq);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifeq()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Ceq);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifeq(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return Ifeq();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifeq(int right)
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			preEmitActionsField.Push( () => 
				{
					LdcI4(right);
					emitter.EmitIf(OpCodes.Ceq);
				}
				);
			return this; 
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Andeq(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Ceq);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody AndNoteq(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Ceq, true);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNoteq()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Ceq, true);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNoteq(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return IfNoteq();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNoteq(int right)
		// ReSharper restore InconsistentNaming
		{
			LdcI4(right);
			return IfNoteq();
		}

		#endregion

		#region Beq_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Beq_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Beq_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Beq_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Beq_S, label);
        }

		#endregion

		#region Bne_Un
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bne_Un(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bne_Un(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bne_Un(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bne_Un, label);
        }

		#endregion

		#region Bne_Un_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bne_Un_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bne_Un_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bne_Un_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bne_Un_S, label);
        }

		#endregion

		#region Bge
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bge(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bge, label);
        }

		#endregion

		#region Bge_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bge_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bge_S, label);
        }

		#endregion

		#region Bge_Un
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge_Un(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bge_Un(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge_Un(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bge_Un, label);
        }

		#endregion

		#region Bge_Un_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge_Un_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bge_Un_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bge_Un_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bge_Un_S, label);
        }

		#endregion

		#region Bgt
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bgt(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bgt, label);
        }

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Cgt()
		// ReSharper restore InconsistentNaming
		{
			return Emit(OpCodes.Cgt);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifgt()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Cgt);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifgt(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return Ifgt();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifgt(int right)
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			preEmitActionsField.Push( () => 
				{
					LdcI4(right);
					emitter.EmitIf(OpCodes.Cgt);
				}
				);
			return this; 
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Andgt(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Cgt);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody AndNotgt(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Cgt, true);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotgt()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Cgt, true);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotgt(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return IfNotgt();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotgt(int right)
		// ReSharper restore InconsistentNaming
		{
			LdcI4(right);
			return IfNotgt();
		}

		#endregion

		#region Bgt_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bgt_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bgt_S, label);
        }

		#endregion

		#region Bgt_Un
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt_Un(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bgt_Un(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt_Un(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bgt_Un, label);
        }

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Cgt_Un()
		// ReSharper restore InconsistentNaming
		{
			return Emit(OpCodes.Cgt_Un);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifgt_Un()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Cgt_Un);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifgt_Un(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return Ifgt_Un();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ifgt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			preEmitActionsField.Push( () => 
				{
					LdcI4(right);
					emitter.EmitIf(OpCodes.Cgt_Un);
				}
				);
			return this; 
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Andgt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Cgt_Un);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody AndNotgt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Cgt_Un, true);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotgt_Un()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Cgt_Un, true);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotgt_Un(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return IfNotgt_Un();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotgt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			LdcI4(right);
			return IfNotgt_Un();
		}

		#endregion

		#region Bgt_Un_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt_Un_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Bgt_Un_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Bgt_Un_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Bgt_Un_S, label);
        }

		#endregion

		#region Ble
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble(string label)
		// ReSharper restore InconsistentNaming
        {
            return Ble(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Ble, label);
        }

		#endregion

		#region Ble_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Ble_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Ble_S, label);
        }

		#endregion

		#region Ble_Un
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble_Un(string label)
		// ReSharper restore InconsistentNaming
        {
            return Ble_Un(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble_Un(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Ble_Un, label);
        }

		#endregion

		#region Ble_Un_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble_Un_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Ble_Un_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Ble_Un_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Ble_Un_S, label);
        }

		#endregion

		#region Blt
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt(string label)
		// ReSharper restore InconsistentNaming
        {
            return Blt(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Blt, label);
        }

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Clt()
		// ReSharper restore InconsistentNaming
		{
			return Emit(OpCodes.Clt);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Iflt()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Clt);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Iflt(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return Iflt();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Iflt(int right)
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			preEmitActionsField.Push( () => 
				{
					LdcI4(right);
					emitter.EmitIf(OpCodes.Clt);
				}
				);
			return this; 
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Andlt(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Clt);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody AndNotlt(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Clt, true);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotlt()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Clt, true);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotlt(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return IfNotlt();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotlt(int right)
		// ReSharper restore InconsistentNaming
		{
			LdcI4(right);
			return IfNotlt();
		}

		#endregion

		#region Blt_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Blt_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Blt_S, label);
        }

		#endregion

		#region Blt_Un
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt_Un(string label)
		// ReSharper restore InconsistentNaming
        {
            return Blt_Un(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt_Un(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Blt_Un, label);
        }

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Clt_Un()
		// ReSharper restore InconsistentNaming
		{
			return Emit(OpCodes.Clt_Un);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Iflt_Un()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Clt_Un);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Iflt_Un(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return Iflt_Un();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Iflt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			preEmitActionsField.Push( () => 
				{
					LdcI4(right);
					emitter.EmitIf(OpCodes.Clt_Un);
				}
				);
			return this; 
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Andlt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Clt_Un);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody AndNotlt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			return And(right, OpCodes.Clt_Un, true);
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotlt_Un()
		// ReSharper restore InconsistentNaming
		{
			var emitter = new IfEmitter(this);
			ifEmittersField.Push(emitter);
			emitter.EmitIf(OpCodes.Clt_Un, true);
			return this;
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotlt_Un(double right)
		// ReSharper restore InconsistentNaming
		{
			LdcR8(right);
			return IfNotlt_Un();
		}

		// ReSharper disable InconsistentNaming
		public DynamicMethodBody IfNotlt_Un(int right)
		// ReSharper restore InconsistentNaming
		{
			LdcI4(right);
			return IfNotlt_Un();
		}

		#endregion

		#region Blt_Un_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt_Un_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Blt_Un_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Blt_Un_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Blt_Un_S, label);
        }

		#endregion

		#region Brtrue
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Brtrue(string label)
		// ReSharper restore InconsistentNaming
        {
            return Brtrue(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Brtrue(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Brtrue, label);
        }

		#endregion

		#region Brfalse
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Brfalse(string label)
		// ReSharper restore InconsistentNaming
        {
            return Brfalse(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Brfalse(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Brfalse, label);
        }

		#endregion

		#region Brtrue_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Brtrue_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Brtrue_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Brtrue_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Brtrue_S, label);
        }

		#endregion

		#region Brfalse_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Brfalse_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Brfalse_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Brfalse_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Brfalse_S, label);
        }

		#endregion

		#region Br
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Br(string label)
		// ReSharper restore InconsistentNaming
        {
            return Br(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Br(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Br, label);
        }

		#endregion

		#region Br_S
		// ReSharper disable InconsistentNaming
		public DynamicMethodBody Br_S(string label)
		// ReSharper restore InconsistentNaming
        {
            return Br_S(GetLabel(label));
        }

        // ReSharper disable InconsistentNaming
		public DynamicMethodBody Br_S(Label label)
		// ReSharper restore InconsistentNaming
        {
            return Emit(OpCodes.Br_S, label);
        }

		#endregion


		public DynamicMethodBody Cge()
		{
			return Clt()
				.Ldc(0)
				.Ceq();
		}
		
		
		public DynamicMethodBody Cle()
        {
            return Cgt()
                .Ldc(0)
                .Ceq();
        }
		
		public DynamicMethodBody Ifge(int right)
		{
			return Ifgt(right - 1);
		}

		public DynamicMethodBody Ifle(int right)
		{
			return Iflt(right + 1);
		}

		public DynamicMethodBody Andle(int right)
		{
			return Andlt(right + 1);
		}

		public DynamicMethodBody AndNotle(int right)
		{
			return AndNotlt(right + 1);
		}

	}
}
