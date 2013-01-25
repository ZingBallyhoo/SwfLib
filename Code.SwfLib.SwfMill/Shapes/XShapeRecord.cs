﻿using System;
using System.Linq;
using System.Xml.Linq;
using Code.SwfLib.Shapes.Records;

namespace Code.SwfLib.SwfMill.Shapes {
    public static class XShapeRecord {

        private class Writer : IShapeRecordVisitor<object, XElement> {

            public XElement Visit(EndShapeRecord record, object arg) {
                return new XElement("ShapeSetup");
            }

            public XElement Visit(StyleChangeShapeRecordRGB record, object arg) {
                var res = FormatShapeSetup(record);
                if (record.StateNewStyles) {
                    var xFillStyles = res.Element("fillStyles");
                    var xLineStyles = res.Element("lineStyles");
                    foreach (var fillStyle in record.FillStyles) {
                        xFillStyles.Add(XFillStyleRGB.ToXml(fillStyle));
                    }
                    foreach (var lineStyle in record.LineStyles) {
                        xLineStyles.Add(XLineStyleRGB.ToXml(lineStyle));
                    }
                }
                return res;
            }

            public XElement Visit(StyleChangeShapeRecordRGBA record, object arg) {
                var res = FormatShapeSetup(record);
                if (record.StateNewStyles) {
                    var xFillStyles = res.Element("fillStyles");
                    var xLineStyles = res.Element("lineStyles");
                    foreach (var fillStyle in record.FillStyles) {
                        xFillStyles.Add(XFillStyleRGBA.ToXml(fillStyle));
                    }
                    foreach (var lineStyle in record.LineStyles) {
                        xLineStyles.Add(XLineStyleRGBA.ToXml(lineStyle));
                    }
                }
                return res;
            }

            public XElement Visit(StyleChangeShapeRecordEx record, object arg) {
                var res = FormatShapeSetup(record);
                if (record.StateNewStyles) {
                    var xFillStyles = res.Element("fillStyles");
                    var xLineStyles = res.Element("lineStyles");
                    foreach (var fillStyle in record.FillStyles) {
                        xFillStyles.Add(XFillStyleRGBA.ToXml(fillStyle));
                    }
                    foreach (var lineStyle in record.LineStyles) {
                        xLineStyles.Add(XLineStyleEx.ToXml(lineStyle));
                    }
                }
                return res;
            }

            public XElement Visit(StraightEdgeShapeRecord record, object arg) {
                return new XElement("LineTo",
                      new XAttribute("x", record.DeltaX),
                      new XAttribute("y", record.DeltaY)
                );
            }

            public XElement Visit(CurvedEdgeShapeRecord record, object arg) {
                return new XElement("CurveTo",
                    new XAttribute("x1", record.ControlDeltaX),
                    new XAttribute("y1", record.ControlDeltaY),
                    new XAttribute("x2", record.AnchorDeltaX),
                    new XAttribute("y2", record.AnchorDeltaY)
                );
            }

            private static XElement FormatShapeSetup(StyleChangeShapeRecord styleChange) {
                var setup = new XElement("ShapeSetup");
                if (styleChange.FillStyle0.HasValue) {
                    setup.Add(new XAttribute("fillStyle0", styleChange.FillStyle0.Value));
                }
                if (styleChange.FillStyle1.HasValue) {
                    setup.Add(new XAttribute("fillStyle1", styleChange.FillStyle1.Value));
                }
                if (styleChange.LineStyle.HasValue) {
                    setup.Add(new XAttribute("lineStyle", styleChange.LineStyle.Value));
                }
                setup.Add(new XAttribute("x", styleChange.MoveDeltaX));
                setup.Add(new XAttribute("y", styleChange.MoveDeltaY));

                if (styleChange.StateNewStyles) {
                    var xFillStyles = new XElement("fillStyles");
                    setup.Add(xFillStyles);
                    var xLineStyles = new XElement("lineStyles");
                    setup.Add(xLineStyles);
                }
                return setup;
            }
        }

        private class Reader : IShapeRecordVisitor<XElement, IShapeRecord> {
            public IShapeRecord Visit(EndShapeRecord record, XElement arg) {
                return record;
            }

            public IShapeRecord Visit(StyleChangeShapeRecordRGB record, XElement arg) {
                //TODO: read nested
                ReadStyleChange(record, arg);
                return record;
            }

            public IShapeRecord Visit(StyleChangeShapeRecordRGBA record, XElement arg) {
                //TODO: read nested
                ReadStyleChange(record, arg);
                return record;
            }

            public IShapeRecord Visit(StyleChangeShapeRecordEx record, XElement arg) {
                //TODO: read nested
                ReadStyleChange(record, arg);
                return record;
            }

            public IShapeRecord Visit(StraightEdgeShapeRecord record, XElement xShape) {
                var result = new StraightEdgeShapeRecord();
                foreach (var attribute in xShape.Attributes()) {
                    switch (attribute.Name.LocalName) {
                        case "x":
                            result.DeltaX = int.Parse(attribute.Value);
                            break;
                        case "y":
                            result.DeltaY = int.Parse(attribute.Value);
                            break;
                        default:
                            OnUnknownAttributeFound(attribute);
                            break;

                    }
                }
                foreach (var elem in xShape.Elements()) {
                    switch (elem.Name.LocalName) {
                        default:
                            OnUnknownElementFound(elem);
                            break;
                    }
                }
                return result;
            }

            public IShapeRecord Visit(CurvedEdgeShapeRecord record, XElement xShape) {
                var result = new CurvedEdgeShapeRecord();
                foreach (var attribute in xShape.Attributes()) {
                    switch (attribute.Name.LocalName) {
                        case "x1":
                            result.ControlDeltaX = int.Parse(attribute.Value);
                            break;
                        case "y1":
                            result.ControlDeltaY = int.Parse(attribute.Value);
                            break;
                        case "x2":
                            result.AnchorDeltaX = int.Parse(attribute.Value);
                            break;
                        case "y2":
                            result.AnchorDeltaY = int.Parse(attribute.Value);
                            break;
                        default:
                            OnUnknownAttributeFound(attribute);
                            break;

                    }
                }
                foreach (var elem in xShape.Elements()) {
                    switch (elem.Name.LocalName) {
                        default:
                            OnUnknownElementFound(elem);
                            break;
                    }
                }
                return result;
            }

            private static void ReadStyleChange(StyleChangeShapeRecord record, XElement xShape) {
                foreach (var attribute in xShape.Attributes()) {
                    switch (attribute.Name.LocalName) {
                        case "fillStyle0":
                            record.FillStyle0 = uint.Parse(attribute.Value);
                            break;
                        case "fillStyle1":
                            record.FillStyle1 = uint.Parse(attribute.Value);
                            break;
                        case "lineStyle":
                            record.LineStyle = uint.Parse(attribute.Value);
                            break;
                        case "x":
                            record.MoveDeltaX = int.Parse(attribute.Value);
                            break;
                        case "y":
                            record.MoveDeltaY = int.Parse(attribute.Value);
                            break;
                        default:
                            OnUnknownAttributeFound(attribute);
                            break;

                    }
                }
                foreach (var elem in xShape.Elements()) {
                    switch (elem.Name.LocalName) {
                        default:
                            OnUnknownElementFound(elem);
                            break;
                    }
                }
            }

            private static void OnUnknownElementFound(XElement elem) {
                throw new FormatException("Unknown element " + elem.Name.LocalName);
            }

            private static void OnUnknownAttributeFound(XAttribute elem) {
                throw new FormatException("Unknown attribute " + elem.Name.LocalName);
            }
        }

        private static readonly Writer _writer = new Writer();
        private static readonly Reader _reader = new Reader();
        private static readonly ShapeRecordFactory _factory = new ShapeRecordFactory();

        public static IShapeRecordRGB RGBFromXml(XElement xShape) {
            var type = GetShapeRecordType(xShape);
            var record = _factory.CreateRGB(type);
            record.AcceptVisitor(_reader, null);
            return record;
        }

        public static IShapeRecordRGBA RGBAFromXml(XElement xShape) {
            var type = GetShapeRecordType(xShape);
            var record = _factory.CreateRGBA(type);
            record.AcceptVisitor(_reader, null);
            return record;
        }

        public static IShapeRecordEx ExFromXml(XElement xShape) {
            var type = GetShapeRecordType(xShape);
            var record = _factory.CreateEx(type);
            record.AcceptVisitor(_reader, null);
            return record;
        }

        private static ShapeRecordType GetShapeRecordType(XElement xShape) {
            switch (xShape.Name.LocalName) {
                case "ShapeSetup":
                    if (xShape.Attributes().Any()) {
                        return ShapeRecordType.StyleChangeRecord;
                    }
                    return ShapeRecordType.EndRecord;
                case "LineTo":
                    return ShapeRecordType.StraightEdge;
                case "CurveTo":
                    return ShapeRecordType.CurvedEdgeRecord;
                default:
                    throw new FormatException("Unknown shape type " + xShape.Name.LocalName);
            }
        }

        public static XElement ToXml(IShapeRecord shapeRecord) {
            return shapeRecord.AcceptVisitor(_writer, null);
        }

    }
}
