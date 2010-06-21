﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Code.SwfLib.Tags;

namespace Code.SwfLib.SwfMill.TagFormatting {
    public class UnknownTagFormatter : TagFormatterBase<UnknownTag> {

        private const string ID_ATTRIB = "id";

        public override void AcceptAttribute(UnknownTag tag, XAttribute attrib) {
            switch (attrib.Name.LocalName) {
                case ID_ATTRIB:
                  //TODO: parse id
                    break;
                default:
                    throw new FormatException("Invalid attribute " + attrib.Name.LocalName);
            }
        }

        public override void AcceptElement(UnknownTag tag, XElement element) {
            switch (element.Name.LocalName) {
                case DATA_TAG:
                    ProcessRawData(tag, element);
                    break;
                default:
                    throw new FormatException("Invalid element " + element.Name.LocalName);
            }
        }

        public override XElement FormatTag(UnknownTag tag) {
            return new XElement(XName.Get(SwfTagNameMapping.UNKNOWN_TAG),
                                 new XAttribute(XName.Get("id"), string.Format("0x{0:x}", (int)tag.RawData.Type)),
                                 new XElement(XName.Get("data"), Convert.ToBase64String(tag.RawData.Data)));
        }
    }
}