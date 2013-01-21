﻿using Code.SwfLib.Data.Actions;

namespace Code.SwfLib.Actions {
    public class ActionCharToAscii : ActionBase {

        public override ActionCode ActionCode {
            get { return ActionCode.CharToAscii; }
        }

        public override TResult AcceptVisitor<TArg, TResult>(IActionVisitor<TArg, TResult> visitor, TArg arg) {
            return visitor.Visit(this, arg);
        }
    }
}