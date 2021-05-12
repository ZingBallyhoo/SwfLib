﻿namespace SwfLib.Avm2.Opcodes.Stack {
    public class PushNamespaceOpcode : BaseAvm2Opcode {
        public uint Index;
        
        public override TResult AcceptVisitor<TArg, TResult>(IAvm2OpcodeVisitor<TArg, TResult> visitor, TArg arg) {
            return visitor.Visit(this, arg);
        }

    }
}
