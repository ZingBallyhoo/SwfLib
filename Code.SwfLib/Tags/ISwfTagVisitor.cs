﻿namespace Code.SwfLib.Tags
{
    public interface ISwfTagVisitor
    {

        object Visit(CSMTextSettingsTag tag);

        object Visit(DefineFontNameTag tag);

        object Visit(DefineSpriteTag tag);

        object Visit(DefineTextTag tag);

        object Visit(EndTag tag);

        object Visit(ExportTag tag);

        object Visit(FileAttributesTag tag);

        object Visit(MetadataTag tag);

        object Visit(PlaceObject2Tag tag);

        object Visit(SetBackgroundColorTag tag);

        object Visit(ShowFrameTag tag);

        object Visit(SwfTagBase tag);

        object Visit(UnknownTag tag);

    }
}