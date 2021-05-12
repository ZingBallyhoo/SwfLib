namespace SwfLib.Avm2.Opcodes {
    public class FindDefinitionOpcode : BaseAvm2Opcode {
        public AbcMultiname Name { get; set; }
        
        public uint RawName { get; set; }
        
        public override TResult AcceptVisitor<TArg, TResult>(IAvm2OpcodeVisitor<TArg, TResult> visitor, TArg arg) {
            return visitor.Visit(this, arg);
        }

    }
}
