﻿using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SwfLib.Avm2.Data {

    public class AbcDataReader {
        private readonly BinaryReader _reader;

        public AbcDataReader(MemoryStream stream) {
            _reader = new BinaryReader(stream);
        }

        public bool IsEOF {
            get { return _reader.BaseStream.Position == _reader.BaseStream.Length; }
        }

        public long Position { get { return _reader.BaseStream.Position; } }

        public AbcFileInfo ReadAbcFile() {
            try {
                var res = new AbcFileInfo {
                    MinorVersion = ReadU16(),
                    MajorVersion = ReadU16(),
                    ConstantPool = ReadConstantPool(),
                    Methods = ReadMultipleMethods().ToList(),
                    Metadata = ReadMutipleMetadata().ToList(),
                };
                var classCount = ReadU30();
                res.Instances = ReadMutipleInstances(classCount).ToList();
                res.Classes = ReadMultipleClasses(classCount).ToList();
                res.Scripts = ReadMultipleScripts().ToList();
                res.Bodies = ReadMultipleBodies().ToList();
                return res;
            } catch (Exception e) {
                throw new Exception(string.Format("Error at {0} ({0:x}):", Position), e);
            }
        }

        private AsMetadataInfo[] ReadMutipleMetadata() {
            var len = ReadU30();
            var res = new AsMetadataInfo[len];
            for (var i = 0; i < len; i++) {
                res[i] = ReadMetadata();
            }
            return res;
        }

        private AsInstanceInfo[] ReadMutipleInstances(uint count) {
            var res = new AsInstanceInfo[count];
            for (var i = 0; i < count; i++) {
                res[i] = ReadInstance();
            }
            return res;
        }

        private AsMethodInfo[] ReadMultipleMethods() {
            var len = ReadU30();
            var res = new AsMethodInfo[len];
            for (var i = 0; i < len; i++) {
                res[i] = ReadMethodInfo();
            }
            return res;
        }

        private AsClassInfo[] ReadMultipleClasses(uint count) {
            var res = new AsClassInfo[count];
            for (var i = 0; i < count; i++) {
                res[i] = ReadClass();
            }
            return res;
        }

        private AsScriptInfo[] ReadMultipleScripts() {
            var len = ReadU30();
            var res = new AsScriptInfo[len];
            for (var i = 0; i < len; i++) {
                res[i] = ReadScript();
            }
            return res;
        }

        private AsMethodBodyInfo[] ReadMultipleBodies() {
            var len = ReadU30();
            var res = new AsMethodBodyInfo[len];
            for (var i = 0; i < len; i++) {
                res[i] = ReadMethodBody();
            }
            return res;
        }

        public AsConstantPoolInfo ReadConstantPool() {
            var res = new AsConstantPoolInfo();

            var resIntegers = new int[Math.Max(ReadU30(), 1)];
            for (var i = 1; i < resIntegers.Length; i++) {
                resIntegers[i] = ReadS32();
            }

            var resUnsignedIntegers = new uint[Math.Max(ReadU30(), 1)];
            for (var i = 1; i < resUnsignedIntegers.Length; i++) {
                resUnsignedIntegers[i] = ReadU32();
            }

            var resDoubles = new double[Math.Max(ReadU30(), 1)];
            resDoubles[0] = double.NaN;
            for (var i = 1; i < resDoubles.Length; i++) {
                resDoubles[i] = ReadD64();
            }

            var resStrings = new string[Math.Max(ReadU30(), 1)];
            resStrings[0] = string.Empty;
            for (var i = 1; i < resStrings.Length; i++) {
                resStrings[i] = ReadString();
            }

            var resNamespaces = new AsNamespaceInfo[Math.Max(ReadU30(), 1)];
            for (var i = 1; i < resNamespaces.Length; i++) {
                resNamespaces[i] = ReadNamespace();
            }

            var resNamespaceSets = new AsNamespaceSetInfo[Math.Max(ReadU30(), 1)];
            for (var i = 1; i < resNamespaceSets.Length; i++) {
                resNamespaceSets[i] = ReadNamespaceSet();
            }

            var resMultinames = new AsMultinameInfo[Math.Max(ReadU30(), 1)];
            for (var i = 1; i < resMultinames.Length; i++) {
                resMultinames[i] = ReadMultiname();
            }

            res.Integers = resIntegers.ToList();
            res.UnsignedIntegers = resUnsignedIntegers.ToList();
            res.Doubles = resDoubles.ToList();
            res.Strings = resStrings.ToList();
            res.Namespaces = resNamespaces.ToList();
            res.NamespaceSets = resNamespaceSets.ToList();
            res.Multinames = resMultinames.ToList();

            return res;
        }

        private AsNamespaceInfo ReadNamespace() {
            return new AsNamespaceInfo {
                Kind = (AsType)ReadU8(),
                Name = ReadU30()
            };
        }

        private AsNamespaceSetInfo ReadNamespaceSet() {
            return new AsNamespaceSetInfo {
                Namespaces = ReadMultipleU30()
            };
        }

        private AsMultinameInfo ReadMultiname() {
            var r = new AsMultinameInfo { Kind = (AsMultinameKind)ReadU8() };
            switch (r.Kind) {
                case AsMultinameKind.QName:
                case AsMultinameKind.QNameA:
                    r.QName = new AsMultinameQName {
                        Namespace = ReadU30(),
                        Name = ReadU30()
                    };
                    break;
                case AsMultinameKind.RTQName:
                case AsMultinameKind.RTQNameA:
                    r.RtqName = new AsMultinameRTQName {
                        Name = ReadU30()
                    };
                    break;
                case AsMultinameKind.RTQNameL:
                case AsMultinameKind.RTQNameLA:
                    break;
                case AsMultinameKind.Multiname:
                case AsMultinameKind.MultinameA:
                    r.Multiname = new AsMultinameMultiname {
                        Name = ReadU30(),
                        NamespaceSet = ReadU30()
                    };
                    break;
                case AsMultinameKind.MultinameL:
                case AsMultinameKind.MultinameLA:
                    r.MultinameL = new AsMultinameMultinameL {
                        NamespaceSet = ReadU30()
                    };
                    break;
                case AsMultinameKind.Generic:
                    r.TypeName = new AsMultinameTypeName {
                        Name = ReadU30(),
                        Params = ReadMultipleU30()
                    };
                    break;
                default:
                    throw new Exception("Unknown Multiname kind " + r.Kind);
            }
            return r;
        }

        private AsMethodInfo ReadMethodInfo() {
            var paramCount = ReadU30();
            var r = new AsMethodInfo {
                ParamTypes = new uint[paramCount],
                ReturnType = ReadU30()
            };
            r.ParamTypes = ReadMultipleU30(paramCount);
            r.Name = ReadU30();
            r.Flags = (AsMethodFlags)ReadU8();
            if (r.HasOptional) {
                r.Options = new AsOptionDetailInfo[ReadU30()];
                for (var i = 0; i < r.Options.Length; i++) {
                    r.Options[i] = ReadOptionDetail();
                }
            }
            if (r.HasParamNames) {
                r.ParamNames = ReadMultipleParamInfo(paramCount);
            }
            return r;
        }

        private AsOptionDetailInfo ReadOptionDetail() {
            return new AsOptionDetailInfo {
                Value = ReadU30(),
                Kind = (AsConstantKind)ReadU8()
            };
        }

        private AsParamInfo[] ReadMultipleParamInfo(uint cnt) {
            var res = new AsParamInfo[cnt];
            for (var i = 0; i < cnt; i++) {
                res[i] = ReadParamInfo();
            }
            return res;
        }

        private AsParamInfo ReadParamInfo() {
            return new AsParamInfo {
                ParamName = ReadU30()
            };
        }

        private AsMetadataInfo ReadMetadata() {
            AsMetadataInfo r;
            r.Name = ReadU30();
            var cnt = ReadU30();
            r.Items = new AsMetadataItem[cnt];
            var raw = ReadMultipleU30(cnt * 2);
            for (var i = 0; i < cnt; i++) {
                r.Items[i] = new AsMetadataItem {
                    Key = raw[i],
                    Value = raw[i + cnt]
                };
            }
            return r;
        }

        private AsInstanceInfo ReadInstance() {
            var r = new AsInstanceInfo {
                Name = ReadU30(),
                SuperName = ReadU30(),
                Flags = (AsInstanceFlags)ReadU8()
            };
            if (r.HasProtectedNs)
                r.ProtectedNs = ReadU30();
            r.Interfaces = ReadMultipleU30();
            r.InstanceInitializer = ReadU30();
            r.Traits = ReadMutlipleTraits().ToList();

            return r;
        }

        private AsTraitsInfo[] ReadMutlipleTraits() {
            var res = new AsTraitsInfo[ReadU30()];
            for (var i = 0; i < res.Length; i++) {
                res[i] = ReadTrait();
            }
            return res;
        }

        private AsTraitsInfo ReadTrait() {
            var r = new AsTraitsInfo {
                Name = ReadU30(),
                Flags = ReadU8()
            };
            switch (r.Kind) {
                case AsTraitKind.Slot:
                case AsTraitKind.Const:
                    r.Slot.SlotId = ReadU30();
                    r.Slot.TypeName = ReadU30();
                    r.Slot.ValueIndex = ReadU30();
                    if (r.Slot.ValueIndex != 0)
                        r.Slot.ValueKind = (AsConstantKind)ReadU8();
                    else
                        r.Slot.ValueKind = AsConstantKind.Void;
                    break;
                case AsTraitKind.Class:
                    r.Class.SlotId = ReadU30();
                    r.Class.Class = ReadU30();
                    break;
                case AsTraitKind.Function:
                    r.Function.SlotId = ReadU30();
                    r.Function.Function = ReadU30();
                    break;
                case AsTraitKind.Method:
                case AsTraitKind.Getter:
                case AsTraitKind.Setter:
                    r.Method.DispId = ReadU30();
                    r.Method.Method = ReadU30();
                    break;
                default:
                    throw new Exception("Unknown trait kind");
            }
            if (r.HasMetadata) {
                r.Metadata = ReadMultipleU30();
            }
            return r;
        }

        private AsClassInfo ReadClass() {
            return new AsClassInfo {
                ClassInitializer = ReadU30(),
                Traits = ReadMutlipleTraits()
            };
        }

        private AsScriptInfo ReadScript() {
            return new AsScriptInfo {
                ScriptInitializer = ReadU30(),
                Traits = ReadMutlipleTraits()
            };
        }

        private AsMethodBodyInfo ReadMethodBody() {
            var r = new AsMethodBodyInfo {
                Method = ReadU30(),
                MaxStack = ReadU30(),
                LocalCount = ReadU30(),
                InitScopeDepth = ReadU30(),
                MaxScopeDepth = ReadU30(),
            };

            var len = ReadU30();
            r.Code = _reader.ReadBytes((int)len);

            r.Exceptions = ReadMultipleExceptions();
            r.Traits = ReadMutlipleTraits();

            return r;
        }

        private AsExceptionInfo[] ReadMultipleExceptions() {
            var res = new AsExceptionInfo[ReadU30()];
            for (var i = 0; i < res.Length; i++) {
                res[i] = ReadExceptionInfo();
            }
            return res;
        }

        private AsExceptionInfo ReadExceptionInfo() {
            return new AsExceptionInfo {
                From = ReadU30(),
                To = ReadU30(),
                Target = ReadU30(),
                ExceptionType = ReadU30(),
                VariableName = ReadU30()
            };
        }

        #region Primitives

        public byte ReadU8() {
            return _reader.ReadByte();
        }
        
        public byte PeekU8()
        {
            var u8 = _reader.ReadByte();
            _reader.BaseStream.Position--;
            return u8;
        }

        private ushort ReadU16() {
            return _reader.ReadUInt16();
        }

        /// <summary>
        /// Reads variable-length encoded unsigned 30-bit integer
        /// </summary>
        /// <returns></returns>
        public uint ReadU30() {
            return ReadU32() & 0x3fffffff;
        }

        private uint ReadU32() {
            uint val = 0;
            var bt = _reader.ReadByte();
            val |= bt & 0x7fu;
            if ((bt & 0x80) == 0) return val;

            bt = _reader.ReadByte();
            val |= (bt & 0x7fu) << 7;
            if ((bt & 0x80) == 0) return val;

            bt = _reader.ReadByte();
            val |= (bt & 0x7fu) << 14;
            if ((bt & 0x80) == 0) return val;

            bt = _reader.ReadByte();
            val |= (bt & 0x7fu) << 21;
            if ((bt & 0x80) == 0) return val;

            bt = _reader.ReadByte();
            val |= (bt & 0x7fu) << 28;
            return val;
        }

        /// <summary>
        /// Reads signed 24-bit integer
        /// </summary>
        /// <returns></returns>
        public int ReadS24() {
            var i0 = _reader.ReadByte();
            var i1 = _reader.ReadByte();
            var i2 = _reader.ReadByte();
            var res = (i2 << 16) | (i1 << 8) | i0;
            if ((res & 0x800000) != 0) res = (int)(res | 0xff000000);
            return res;
        }

        /// <summary>
        /// Reads variable-length encoded signed 32-bit integer
        /// </summary>
        /// <returns></returns>
        private int ReadS32() {
            return (int) ReadU32();
        }

        private uint[] ReadMultipleU30() {
            var len = ReadU30();
            return ReadMultipleU30(len);
        }

        private uint[] ReadMultipleU30(uint cnt) {
            var res = new uint[cnt];
            for (var i = 0; i < res.Length; i++) {
                res[i] = ReadU30();
            }
            return res;
        }

        public double ReadD64() {
            return BitConverter.Int64BitsToDouble((long)_reader.ReadUInt64());
        }

        private string ReadString() {
            var len = ReadU30();
            if (len == 0) return "";
            var dataStream = new MemoryStream();
            for (var i = 0; i < len; i++) {
                var bt = _reader.ReadByte();
                if (bt != 0) dataStream.WriteByte((byte) bt);
            }
            return Encoding.UTF8.GetString(dataStream.ToArray());
        }

        #endregion

    }

}
