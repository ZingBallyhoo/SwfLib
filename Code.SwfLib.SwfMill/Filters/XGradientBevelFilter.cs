﻿using System.Xml.Linq;
using Code.SwfLib.Filters;
using Code.SwfLib.SwfMill.Data;
using Code.SwfLib.SwfMill.Gradients;
using Code.SwfLib.SwfMill.Utils;

namespace Code.SwfLib.SwfMill.Filters {
    public static class XGradientBevelFilter {

        public const string TAG_NAME = "GradientBevel";

        public static XElement ToXml(GradientBevelFilter filter) {
            var res = new XElement(TAG_NAME,
                new XAttribute("blurX", CommonFormatter.Format(filter.BlurX)),
                new XAttribute("blurY", CommonFormatter.Format(filter.BlurY)),
                new XAttribute("angle", CommonFormatter.Format(filter.Angle)),
                new XAttribute("distance", CommonFormatter.Format(filter.Distance)),
                new XAttribute("strength", CommonFormatter.Format(filter.Strength)),
                new XAttribute("innerGlow", CommonFormatter.Format(filter.InnerGlow)),
                new XAttribute("knockout", CommonFormatter.Format(filter.Knockout)),
                new XAttribute("compositeSource", CommonFormatter.Format(filter.CompositeSource)),
                new XAttribute("onTop", CommonFormatter.Format(filter.OnTop)),
                new XAttribute("passes", filter.Passes)
            );
            res.Add(XGradientRecords.ToXml(filter.GradientColors));
            return res;
        }

        public static GradientBevelFilter FromXml(XElement xFilter) {
            const string node = "GradientBevel";
            var xInnerGlow = xFilter.Attribute("innerGlow");
            var xKnockout = xFilter.Attribute("knockout");
            var xCompositeSource = xFilter.Attribute("compositeSource");
            var xOnTop = xFilter.Attribute("onTop");
            var xPasses = xFilter.Attribute("passes");

            var filter = new GradientBevelFilter {
                BlurX = xFilter.RequiredDoubleAttribute("blurX", node),
                BlurY = xFilter.RequiredDoubleAttribute("blurY", node),
                Angle = xFilter.RequiredDoubleAttribute("angle", node),
                Distance = xFilter.RequiredDoubleAttribute("distance", node),
                Strength = xFilter.RequiredDoubleAttribute("strength", node),
                InnerGlow = CommonFormatter.ParseBool(xInnerGlow.Value),
                Knockout = CommonFormatter.ParseBool(xKnockout.Value),
                CompositeSource = xCompositeSource == null || CommonFormatter.ParseBool(xCompositeSource.Value),
                OnTop = CommonFormatter.ParseBool(xOnTop.Value),
                Passes = uint.Parse(xPasses.Value),
            };

            XGradientRecords.FromXml(xFilter.Element("gradientColors"), filter.GradientColors);
            return filter;
        }

    }
}
