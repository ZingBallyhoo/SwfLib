﻿using SwfLib.Avm2.Opcodes;
using SwfLib.Avm2.Opcodes.Arithmetics;
using SwfLib.Avm2.Opcodes.Branching;
using SwfLib.Avm2.Opcodes.Debug;
using SwfLib.Avm2.Opcodes.Stack;
using SwfLib.Avm2.Opcodes.TypeCasting;
using SwfLib.Avm2.Opcodes.Xml;

namespace SwfLib.Avm2 {
    public class Avm2OpcodeFactory {

        public BaseAvm2Opcode CreateOpcode(Avm2Opcode opcode) {
            switch (opcode) {
                case Avm2Opcode.Add: return new AddOpcode();
                case Avm2Opcode.AddI: return new AddIOpcode();
                case Avm2Opcode.AsType: return new AsTypeOpcode();
                case Avm2Opcode.AsTypeLate: return new AsTypeLateOpcode();
                case Avm2Opcode.BitAnd: return new BitAndOpcode();
                case Avm2Opcode.BitNot: return new BitNotOpcode();
                case Avm2Opcode.BitOr: return new BitOrOpcode();
                case Avm2Opcode.BitXor: return new BitXorOpcode();
                case Avm2Opcode.Call: return new CallOpcode();
                case Avm2Opcode.CallMethod: return new CallMethodOpcode();
                case Avm2Opcode.CallProperty: return new CallPropertyOpcode();
                case Avm2Opcode.CallPropLex: return new CallPropLexOpcode();
                case Avm2Opcode.CallPropVoid: return new CallPropVoidOpcode();
                case Avm2Opcode.CallStatic: return new CallStaticOpcode();
                case Avm2Opcode.CallSuper: return new CallSuperOpcode();
                case Avm2Opcode.CallSuperVoid: return new CallSuperVoidOpcode();
                case Avm2Opcode.CheckFilter: return new CheckFilterOpcode();
                case Avm2Opcode.Coerce: return new CoerceOpcode();
                case Avm2Opcode.CoerceA: return new CoerceAOpcode();
                case Avm2Opcode.CoerceS: return new CoerceSOpcode();
                case Avm2Opcode.Construct: return new ConstructOpcode();
                case Avm2Opcode.ConstructProp: return new ConstructPropOpcode();
                case Avm2Opcode.ConstructSuper: return new ConstructSuperOpcode();
                case Avm2Opcode.ConvertB: return new ConvertBOpcode();
                case Avm2Opcode.ConvertD: return new ConvertDOpcode();
                case Avm2Opcode.ConvertI: return new ConvertIOpcode();
                case Avm2Opcode.ConvertO: return new ConvertOOpcode();
                case Avm2Opcode.ConvertS: return new ConvertSOpcode();
                case Avm2Opcode.ConvertU: return new ConvertUOpcode();
                case Avm2Opcode.Debug: return new DebugOpcode();
                case Avm2Opcode.DebugFile: return new DebugFileOpcode();
                case Avm2Opcode.DebugLine: return new DebugLineOpcode();
                case Avm2Opcode.DecLocal: return new DecLocalOpcode();
                case Avm2Opcode.DecLocalI: return new DecLocalIOpcode();
                case Avm2Opcode.Decrement: return new DecrementOpcode();
                case Avm2Opcode.DecrementI: return new DecrementIOpcode();
                case Avm2Opcode.DeleteProperty: return new DeletePropertyOpcode();
                case Avm2Opcode.Divide: return new DivideOpcode();
                case Avm2Opcode.Dup: return new DupOpcode();
                case Avm2Opcode.Dxns: return new DxnsOpcode();
                case Avm2Opcode.DxnsLate: return new DxnsLateOpcode();
                case Avm2Opcode.Equal: return new EqualsOpcode();
                case Avm2Opcode.EscXAttr: return new EscXAttrOpcode();
                case Avm2Opcode.EscXElem: return new EscXElemOpcode();
                case Avm2Opcode.FindProperty: return new FindPropertyOpcode();
                case Avm2Opcode.FindPropStrict: return new FindPropStrictOpcode();
                case Avm2Opcode.GetDescendants: return new GetDescendantsOpcode();
                case Avm2Opcode.GetGlobalScope: return new GetGlobalScopeOpcode();
                case Avm2Opcode.GetGlobalSlot: return new GetGlobalSlotOpcode();
                case Avm2Opcode.GetLex: return new GetLexOpcode();
                case Avm2Opcode.GetLocal: return new GetLocalOpcode();
                case Avm2Opcode.GetLocal0: return new GetLocal0Opcode();
                case Avm2Opcode.GteLocal1: return new GetLocal1Opcode();
                case Avm2Opcode.GetLocal2: return new GetLocal2Opcode();
                case Avm2Opcode.GetLocal3: return new GetLocal3Opcode();
                case Avm2Opcode.GetProperty: return new GetPropertyOpcode();
                case Avm2Opcode.GetScopeObject: return new GetScopeObjectOpcode();
                case Avm2Opcode.GetSlot: return new GetSlotOpcode();
                case Avm2Opcode.GetSuper: return new GetSuperOpcode();
                case Avm2Opcode.GreaterEquals: return new GreaterEqualsOpcode();
                case Avm2Opcode.GreaterThan: return new GreaterThanOpcode();
                case Avm2Opcode.HasNext: return new HasNextOpcode();
                case Avm2Opcode.HasNext2: return new HasNext2Opcode();
                case Avm2Opcode.IfEq: return new IfeqOpcode();
                case Avm2Opcode.IfFalse: return new IfFalseOpcode();
                case Avm2Opcode.IfGe: return new IfgeOpcode();
                case Avm2Opcode.IfGt: return new IfgtOpcode();
                case Avm2Opcode.IfLe: return new IfleOpcode();
                case Avm2Opcode.IfLt: return new IfltOpcode();
                case Avm2Opcode.IfNe: return new IfneOpcode();
                case Avm2Opcode.IfNge: return new IfngeOpcode();
                case Avm2Opcode.IfNgt: return new IfngtOpcode();
                case Avm2Opcode.IfNle: return new IfnleOpcode();
                case Avm2Opcode.IfNlt: return new IfnltOpcode();
                case Avm2Opcode.IfStrictEq: return new IfStrictEqOpcode();
                case Avm2Opcode.IfStrictNe: return new IfStrictNeOpcode();
                case Avm2Opcode.IfTrue: return new IfTrueOpcode();
                case Avm2Opcode.In: return new InOpcode();
                case Avm2Opcode.IncLocal: return new IncLocalOpcode();
                case Avm2Opcode.IncLocalI: return new IncLocalIOpcode();
                case Avm2Opcode.Increment: return new IncrementOpcode();
                case Avm2Opcode.IncrementI: return new IncrementIOpcode();
                case Avm2Opcode.InitProperty: return new InitPropertyOpcode();
                case Avm2Opcode.InstanceOf: return new InstanceOfOpcode();
                case Avm2Opcode.IsType: return new IsTypeOpcode();
                case Avm2Opcode.IsTypeLate: return new IsTypeLateOpcode();
                case Avm2Opcode.Jump: return new JumpOpcode();
                case Avm2Opcode.Kill: return new KillOpcode();
                case Avm2Opcode.Label: return new LabelOpcode();
                case Avm2Opcode.LessEquals: return new LessEqualsOpcode();
                case Avm2Opcode.LessThan: return new LessThanOpcode();
/*
lf32
lf64
li8
li16
li32
*/
                case Avm2Opcode.LookupSwitch: return new LookupSwitchOpcode();
                case Avm2Opcode.LShift: return new LShiftOpcode();
                case Avm2Opcode.Modulo: return new ModuloOpcode();
                case Avm2Opcode.Multiply: return new MultiplyOpcode();
                case Avm2Opcode.MultiplyI: return new MultiplyIOpcode();
                case Avm2Opcode.Negate: return new NegateOpcode();
                case Avm2Opcode.NegateI: return new NegateIOpcode();
                case Avm2Opcode.NewActivation: return new NewActivationOpcode();
                case Avm2Opcode.NewArray: return new NewArrayOpcode();
                case Avm2Opcode.NewCatch: return new NewCatchOpcode();
                case Avm2Opcode.NewClass: return new NewClassOpcode();
                case Avm2Opcode.NewFunction: return new NewFunctionOpcode();
                case Avm2Opcode.NewObject: return new NewObjectOpcode();
                case Avm2Opcode.NextName: return new NextNameOpcode();
                case Avm2Opcode.NextValue: return new NextValueOpcode();
                case Avm2Opcode.Nop: return new NopOpcode();
                case Avm2Opcode.Not: return new NotOpcode();
                case Avm2Opcode.Pop: return new PopOpcode();
                case Avm2Opcode.PopScope: return new PopScopeOpcode();
                case Avm2Opcode.PushByte: return new PushByteOpcode();
                case Avm2Opcode.PushDouble: return new PushDoubleOpcode();
                case Avm2Opcode.PushFalse: return new PushFalseOpcode();
                case Avm2Opcode.PushInt: return new PushIntOpcode();
                case Avm2Opcode.PushNamespace: return new PushNamespaceOpcode();
                case Avm2Opcode.PushNan: return new PushNanOpcode();
                case Avm2Opcode.PushNull: return new PushNullOpcode();
                case Avm2Opcode.PushScope: return new PushScopeOpcode();
                case Avm2Opcode.PushShort: return new PushShortOpcode();
                case Avm2Opcode.PushString: return new PushStringOpcode();
                case Avm2Opcode.PushTrue: return new PushTrueOpcode();
                case Avm2Opcode.PushUInt: return new PushUIntOpcode();
                case Avm2Opcode.PushUndefined: return new PushUndefinedOpcode();
                case Avm2Opcode.PushWith: return new PushWithOpcode();
                case Avm2Opcode.ReturnValue: return new ReturnValueOpcode();
                case Avm2Opcode.ReturnVoid: return new ReturnVoidOpcode();
                case Avm2Opcode.RShift: return new RShiftOpcode();
                case Avm2Opcode.SetGlobalSlot: return new SetGlobalSlotOpcode();
                case Avm2Opcode.SetLocal: return new SetLocalOpcode();
                case Avm2Opcode.SetLocal0: return new SetLocal0Opcode();
                case Avm2Opcode.SetLocal1: return new SetLocal1Opcode();
                case Avm2Opcode.SetLocal2: return new SetLocal2Opcode();
                case Avm2Opcode.SetLocal3: return new SetLocal3Opcode();
                case Avm2Opcode.SetProperty: return new SetPropertyOpcode();
                case Avm2Opcode.SetSlot: return new SetSlotOpcode();
                case Avm2Opcode.SetSuper: return new SetSuperOpcode();
                case Avm2Opcode.StrictEquals: return new StrictEqualsOpcode();
                case Avm2Opcode.Subtract: return new SubtractOpcode();
                case Avm2Opcode.SubtractI: return new SubtractIOpcode();
                case Avm2Opcode.Swap: return new SwapOpcode();
                case Avm2Opcode.Throw: return new ThrowOpcode();
                case Avm2Opcode.TypeOf: return new TypeOfOpcode();
                case Avm2Opcode.UrShift: return new UrshiftOpcode();
                /*
sf32
sf64
si8
si16
si32
                 */
                /*
sxi_1
sxi_8
sxi_16
                 */
                default: return new UnknownOpcode();
            }
        }
    }
}
