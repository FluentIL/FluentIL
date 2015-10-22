using System;
using System.Drawing;
using System.Reflection.Emit;
using FluentIL;
using ImageProcessing.Util;

namespace ImageProcessing
{
    public class ILApplier : IFilterApplier
    {
        public void Apply(Filter filter, Bitmap target)
        {
            Action<byte[], byte[]> action = (bytesSource, bytesDest) =>
            {
                var stride = target.GetStride();
                var method = CreateFilterMethodIL(bytesSource, stride,
                    stride / target.Width,
                    filter.Matrix, filter.Size, filter.Bias);

                method.Invoke(null, new object[] { bytesSource, bytesDest });
            };

            target.ExecuteTransformAction(action);
        }

        static DynamicMethod CreateFilterMethodIL(byte[] src,
            int stride, int bytesPerPixel, double[] filter, int filterWidth, int bias)
        {
            #region filter analysis
            
            var allwaysFilterNeg = true;
            var neverFilterNeg = true;
            double negs = 0;
            for (var i = 0; i < filter.Length; i++)
            {
                if (i > 0) allwaysFilterNeg = false;
                if (i >= 0) continue;
                neverFilterNeg = false;
                negs += i;
            }

            #endregion

            if (filter[filter.Length / 2] >= Math.Abs(negs)) neverFilterNeg = true;

            var result = IL.NewMethod()
                .WithParameter(typeof(byte[]), "src")
                .WithParameter(typeof(byte[]), "dest")
                .WithVariable(typeof(double), "pixelsAccum")
                .WithVariable(typeof(double), "filterAccum")
                .Returns(typeof(void))

                .For("iDst", 0, src.Length - 1)
                    .Stloc(0.0, "pixelsAccum", "filterAccum")
                    
                    .Repeater(0, filter.Length - 1, 1,
                        (ind, body) => filter[ind] != 0,
                        (index, body) =>
                        {
                            body
                                .Ldarg("src")
                                .Ldloc("iDst")
                                .Add(ComputeOffset(index, filterWidth, stride, bytesPerPixel))
                                .Dup()
                                .Ifge(0).Andlt(src.Length)
                                    .LdelemU1()
                                    .ConvR8()
                                    .Mul(filter[index])
                                    .AddToVar("pixelsAccum")
                                    .AddToVar("filterAccum", filter[index])
                                .Else()
                                    .Pop().Pop()
                                .EndIf();
                        }
                    )

                    .Ldarg("dest")
                    .Ldloc("iDst", "pixelsAccum", "filterAccum")

                    .Dup()
                    .IfNoteq(0.0)
                        .EmitIf(!neverFilterNeg && allwaysFilterNeg, (r) => r.Neg())
                        .EmitIf(!neverFilterNeg && !allwaysFilterNeg, (r) => r.AbsR8())
                        .Div()
                    .Else()
                        .Pop()
                    .EndIf()

                    .Add((double)bias)

                    .EnsureLimits(0.0, 255.0)
                    .ConvU1()
                    .StelemI1()
                .Next()
                .Ret();

            return result;
        }

        static int ComputeOffset(int index, int filtersize, int stride, int bpp)
        {
            var yFilter = index / filtersize;
            var xFilter = index % filtersize;

            return stride * (yFilter - filtersize / 2) +
                      bpp * (xFilter - filtersize / 2);
        }


        static DynamicMethod CreateFilterMethodIL_Reference(byte[] src, byte[] dst,
            int stride, int bytesPerPixel, double[] filter, int filterWidth, int bias)
        {
            var filterHeight = filter.Length / filterWidth;

            var result = new DynamicMethod(
                "DoFilter",
                typeof(void),
                new Type[] { typeof(byte[]), typeof(byte[]) },
                typeof(ILApplier));

            var allwaysFilterNeg = true;
            var neverFilterNeg = true;
            double negs = 0;
            for (int i = 0; i < filter.Length; i++)
            {
                if (filter[i] > 0) allwaysFilterNeg = false;
                if (filter[i] < 0)
                {
                    neverFilterNeg = false;
                    negs += filter[i];
                }
            }
            if (filter[filter.Length / 2] >= Math.Abs(negs)) neverFilterNeg = true;


            var ilgen = result.GetILGenerator();

            // criando as três variáveis usadas no corpo do método
            ilgen.DeclareLocal(typeof(int));        // iDst
            ilgen.DeclareLocal(typeof(double));     // pixelsAccum 
            ilgen.DeclareLocal(typeof(double));     // filterAccum

            ilgen.Emit(OpCodes.Ldc_I4_0);           // iDst = 0;
            ilgen.Emit(OpCodes.Stloc_0);

            // define um ponto de retorno .. goto .. 
            var top = ilgen.DefineLabel();
            ilgen.MarkLabel(top);

            ilgen.Emit(OpCodes.Ldc_R8, 0.0);
            ilgen.Emit(OpCodes.Dup);
            ilgen.Emit(OpCodes.Stloc_1);            // pixelsAccum = 0
            ilgen.Emit(OpCodes.Stloc_2);            // filterAccum = 0

            for (int i = 0; i < filter.Length; i++)
            {
                if (filter[i] == 0) continue;

                int yFilter = i / filterHeight;
                int xFilter = i % filterWidth;

                int offset = stride * (yFilter - filterHeight / 2) +
                        bytesPerPixel * (xFilter - filterWidth / 2);

                var lessThanZero = ilgen.DefineLabel();
                var greaterThan = ilgen.DefineLabel();
                var loopBottom = ilgen.DefineLabel();

                // primeiro vetor na pilha
                ilgen.Emit(OpCodes.Ldarg_0);

                ilgen.Emit(OpCodes.Ldloc_0);
                if (offset > 0)
                {
                    ilgen.Emit(OpCodes.Ldc_I4, offset);
                    ilgen.Emit(OpCodes.Add);            // iDst + offset
                }

                ilgen.Emit(OpCodes.Dup);
                ilgen.Emit(OpCodes.Dup);

                ilgen.Emit(OpCodes.Ldc_I4_0);
                ilgen.Emit(OpCodes.Blt_S, lessThanZero); // if (iDst < 0)

                ilgen.Emit(OpCodes.Ldc_I4, src.Length);
                ilgen.Emit(OpCodes.Bge_S, greaterThan); // if (iDst > src.Length)

                ilgen.Emit(OpCodes.Ldelem_U1);
                ilgen.Emit(OpCodes.Conv_R8); // obtém o elemento de cor ne vetor

                if (filter[i] != 1) // se filtro for 1, não altera valor de referência
                {
                    if (filter[i] == -1)
                    {
                        ilgen.Emit(OpCodes.Neg); // inverte o valor
                    }
                    else
                    {
                        // produto
                        ilgen.Emit(OpCodes.Ldc_R8, filter[i]);
                        ilgen.Emit(OpCodes.Mul);
                    }
                }

                ilgen.Emit(OpCodes.Ldloc_1);
                ilgen.Emit(OpCodes.Add);
                ilgen.Emit(OpCodes.Stloc_1); // atualizando pixelsAccum

                ilgen.Emit(OpCodes.Ldc_R8, filter[i]);
                ilgen.Emit(OpCodes.Ldloc_2);
                ilgen.Emit(OpCodes.Add);
                ilgen.Emit(OpCodes.Stloc_2); // filterAccum

                ilgen.Emit(OpCodes.Br, loopBottom);

                // organizando a pilha para o próximo elemento do filtro
                ilgen.MarkLabel(lessThanZero);
                ilgen.Emit(OpCodes.Pop);
                ilgen.MarkLabel(greaterThan);
                ilgen.Emit(OpCodes.Pop);
                ilgen.Emit(OpCodes.Pop);

                ilgen.MarkLabel(loopBottom);
            }

            ilgen.Emit(OpCodes.Ldarg_1);        // dst
            ilgen.Emit(OpCodes.Ldloc_0);        // iDst

            var shouldSkipDivide = ilgen.DefineLabel();
            var copyQuotient = ilgen.DefineLabel();
            var pixelIsBlack = ilgen.DefineLabel();
            var pixelIsWhite = ilgen.DefineLabel();
            var done = ilgen.DefineLabel();

            ilgen.Emit(OpCodes.Ldloc_1);
            ilgen.Emit(OpCodes.Ldloc_2);

            ilgen.Emit(OpCodes.Dup);
            ilgen.Emit(OpCodes.Ldc_R8, 0.0);
            ilgen.Emit(OpCodes.Beq_S, shouldSkipDivide); // if (filterAccum != 0)

            if (!neverFilterNeg)
            {
                if (allwaysFilterNeg)
                {
                    ilgen.Emit(OpCodes.Neg);
                }
                else
                {
                    var absFilterAccum = ilgen.DefineLabel();
                    ilgen.Emit(OpCodes.Dup);
                    ilgen.Emit(OpCodes.Ldc_R8, 0.0);
                    ilgen.Emit(OpCodes.Bge_S, absFilterAccum);
                    ilgen.Emit(OpCodes.Neg);
                    ilgen.MarkLabel(absFilterAccum);
                }
            }

            ilgen.Emit(OpCodes.Div); // pixelAccum / filterAccum
            ilgen.Emit(OpCodes.Br_S, copyQuotient);

            ilgen.MarkLabel(shouldSkipDivide);
            ilgen.Emit(OpCodes.Pop);

            ilgen.MarkLabel(copyQuotient);
            if (bias > 0)
            {
                ilgen.Emit(OpCodes.Ldc_R8, (double)bias);
                ilgen.Emit(OpCodes.Add);
            }

            ilgen.Emit(OpCodes.Dup);
            ilgen.Emit(OpCodes.Dup);

            ilgen.Emit(OpCodes.Ldc_R8, 0.0);
            ilgen.Emit(OpCodes.Blt_S, pixelIsBlack); // if (pixelsAccum < 0)

            ilgen.Emit(OpCodes.Ldc_R8, 255.0);
            ilgen.Emit(OpCodes.Bgt_S, pixelIsWhite); // if (pixelsAccum > 255)

            ilgen.Emit(OpCodes.Conv_U1); // cast para byte
            ilgen.Emit(OpCodes.Br_S, done);

            ilgen.MarkLabel(pixelIsBlack);
            ilgen.Emit(OpCodes.Pop);
            ilgen.Emit(OpCodes.Pop);
            ilgen.Emit(OpCodes.Ldc_I4_S, 0);
            ilgen.Emit(OpCodes.Br_S, done); // setando para 0

            ilgen.MarkLabel(pixelIsWhite);
            ilgen.Emit(OpCodes.Pop);
            ilgen.Emit(OpCodes.Ldc_I4_S, 255); // setando 255

            ilgen.MarkLabel(done);
            ilgen.Emit(OpCodes.Stelem_I1); // dst[iDst] = value;

            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldc_I4_1);
            ilgen.Emit(OpCodes.Add);
            ilgen.Emit(OpCodes.Dup);
            ilgen.Emit(OpCodes.Stloc_0);    // iDst ++

            ilgen.Emit(OpCodes.Ldc_I4, src.Length);
            ilgen.Emit(OpCodes.Blt, top);

            ilgen.Emit(OpCodes.Ret);
            return result;
        }

    }
}
