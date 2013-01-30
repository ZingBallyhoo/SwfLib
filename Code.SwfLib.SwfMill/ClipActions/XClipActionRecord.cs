﻿using System.Xml.Linq;
using Code.SwfLib.ClipActions;
using Code.SwfLib.SwfMill.Actions;

namespace Code.SwfLib.SwfMill.ClipActions {
    public static class XClipActionRecord {

        public static XElement ToXml(ClipActionRecord clipAction) {
            var res = new XElement("Event");

            if (clipAction.Actions.Count > 0) {
                var xActions = new XElement("actions");
                foreach (var action in clipAction.Actions) {
                    xActions.Add(XAction.ToXml(action));
                }
                res.Add(xActions);
            }

            res.Add(new XAttribute("flags1", XClipEventFlags.GetFlags1(clipAction.Flags)));
            res.Add(new XAttribute("flags2", XClipEventFlags.GetFlags2(clipAction.Flags)));

            return res;
            //TODO: key code
        }

        public static ClipActionRecord FromXml(XElement xClipAction) {
            var res = new ClipActionRecord();
            var xActions = xClipAction.Element("actions");
            if (xActions != null) {
                foreach (var xAction in xActions.Elements()) {
                    res.Actions.Add(XAction.FromXml(xAction));
                }
            }
            XClipEventFlags.SetFlags1(ref res.Flags, int.Parse(xClipAction.Attribute("flags1").Value));
            XClipEventFlags.SetFlags2(ref res.Flags, int.Parse(xClipAction.Attribute("flags2").Value));
            return res;
            //TODO: read keycode
        }
    }
}
