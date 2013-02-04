﻿using System;
using Code.SwfLib.Data;

namespace Code.SwfLib {
    public static class SwfStreamWriterExt {

        public static void WriteSwfFileInfo(this SwfStreamWriter writer, SwfFileInfo fileInfo) {
            string format = fileInfo.Format;
            if (format == null || format.Length != 3)
                throw new InvalidOperationException("Format should be of length 3");
            writer.WriteByte((byte)format[0]);
            writer.WriteByte((byte)format[1]);
            writer.WriteByte((byte)format[2]);

            writer.WriteByte(fileInfo.Version);

            var len = fileInfo.FileLength;
            writer.WriteByte((byte)((len >> 0) & 0xff));
            writer.WriteByte((byte)((len >> 8) & 0xff));
            writer.WriteByte((byte)((len >> 16) & 0xff));
            writer.WriteByte((byte)((len >> 24) & 0xff));
        }

        public static void WriteSwfHeader(this SwfStreamWriter writer, SwfHeader header) {
            writer.WriteRect(ref header.FrameSize);
            writer.WriteFixedPoint8(header.FrameRate);
            writer.WriteUInt16(header.FrameCount);
        }

        public static void WriteRGB(this SwfStreamWriter writer, ref SwfRGB val) {
            writer.WriteByte(val.Red);
            writer.WriteByte(val.Green);
            writer.WriteByte(val.Blue);
        }

        public static void WriteRGBA(this SwfStreamWriter writer, SwfRGBA val) {
            writer.WriteRGBA(ref val);
        }

        public static void WriteRGBA(this SwfStreamWriter writer, ref SwfRGBA val) {
            writer.WriteByte(val.Red);
            writer.WriteByte(val.Green);
            writer.WriteByte(val.Blue);
            writer.WriteByte(val.Alpha);
        }

        public static void WriteARGB(this SwfStreamWriter writer, SwfRGBA val) {
            writer.WriteByte(val.Alpha);
            writer.WriteByte(val.Red);
            writer.WriteByte(val.Green);
            writer.WriteByte(val.Blue);
        }

        public static void WriteRect(this SwfStreamWriter writer, ref SwfRect rect) {
            var btCount = new BitsCount(rect.XMin, rect.XMax, rect.YMin, rect.YMax);
            var bits = btCount.GetSignedBits();
            if (bits < 1) bits = 1;
            writer.WriteUnsignedBits(bits, 5);
            writer.WriteSignedBits(rect.XMin, bits);
            writer.WriteSignedBits(rect.XMax, bits);
            writer.WriteSignedBits(rect.YMin, bits);
            writer.WriteSignedBits(rect.YMax, bits);
            writer.FlushBits();
        }

        public static void WriteMatrix(this SwfStreamWriter writer, SwfMatrix matrix) {
            writer.WriteMatrix(ref matrix);
        }

        public static void WriteMatrix(this SwfStreamWriter writer, ref SwfMatrix matrix) {
            bool hasScale = matrix.HasScale;
            writer.WriteBit(hasScale);
            if (hasScale) {
                var sx = (int)(matrix.ScaleX * 65536.0);
                var sy = (int)(matrix.ScaleY * 65536.0);
                var scaleBits = new BitsCount(sx, sy).GetSignedBits();
                if (scaleBits < 1) scaleBits = 1;
                writer.WriteUnsignedBits(scaleBits, 5);
                writer.WriteFixedPoint16(matrix.ScaleX, scaleBits);
                writer.WriteFixedPoint16(matrix.ScaleY, scaleBits);
            }
            bool hasRotate = matrix.HasRotate;
            writer.WriteBit(hasRotate);
            if (hasRotate) {
                var rx = (int)(matrix.RotateSkew0 * 65536.0);
                var ry = (int)(matrix.RotateSkew1 * 65536.0);
                var rotateBits = new BitsCount(rx, ry).GetSignedBits();
                if (rotateBits < 1) rotateBits = 1;
                writer.WriteUnsignedBits(rotateBits, 5);
                writer.WriteFixedPoint16(matrix.RotateSkew0, rotateBits);
                writer.WriteFixedPoint16(matrix.RotateSkew1, rotateBits);
            }
            var translateBits = new BitsCount(matrix.TranslateX, matrix.TranslateY).GetSignedBits();
            writer.WriteUnsignedBits(translateBits, 5);
            writer.WriteSignedBits(matrix.TranslateX, translateBits);
            writer.WriteSignedBits(matrix.TranslateY, translateBits);
            writer.FlushBits();
        }

    }
}
