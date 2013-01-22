﻿namespace Code.SwfLib.Actions {
    public class ActionDefineFunction2 : ActionBase {
        public override ActionCode ActionCode {
            get { return ActionCode.DefineFunction2; }
        }

        public override TResult AcceptVisitor<TArg, TResult>(IActionVisitor<TArg, TResult> visitor, TArg arg) {
            return visitor.Visit(this, arg);
        }
    }
}
