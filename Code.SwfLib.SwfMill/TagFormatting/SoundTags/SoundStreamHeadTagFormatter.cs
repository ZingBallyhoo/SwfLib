﻿using System.Xml.Linq;
using Code.SwfLib.Tags.SoundTags;

namespace Code.SwfLib.SwfMill.TagFormatting.SoundTags {
    public class SoundStreamHeadTagFormatter : TagFormatterBase<SoundStreamHeadTag> {
        protected override XElement FormatTagElement(SoundStreamHeadTag tag) {
            throw new System.NotImplementedException();
        }

        protected override void AcceptTagAttribute(SoundStreamHeadTag tag, XAttribute attrib) {
            throw new System.NotImplementedException();
        }

        protected override void AcceptTagElement(SoundStreamHeadTag tag, XElement element) {
            throw new System.NotImplementedException();
        }
    }
}