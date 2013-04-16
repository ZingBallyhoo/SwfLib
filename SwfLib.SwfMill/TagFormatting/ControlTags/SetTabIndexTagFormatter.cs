﻿using System.Xml.Linq;
using Code.SwfLib.Tags.ControlTags;

namespace SwfLib.SwfMill.TagFormatting.ControlTags {
    public class SetTabIndexTagFormatter : TagFormatterBase<SetTabIndexTag> {
        protected override void FormatTagElement(SetTabIndexTag tag, XElement xTag) {
        }

        public override string TagName {
            get { return "SetTabIndex"; }
        }
    }
}