using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SwfLib.Avm2.Data {

    public class AbcDataWriter {
        private readonly BinaryWriter _writer;

        public AbcDataWriter(Stream stream) {
            _writer = new BinaryWriter(stream);
        }

        public void WriteAbcFile(AbcFileInfo abc) {
            WriteU16(abc.MinorVersion);
            WriteU16(abc.MajorVersion);

            WriteConstantPool(abc.ConstantPool);

            WriteMultipleMethods(abc.Methods);
            WriteMultipleMetadata(abc.Metadata);

            if (abc.Classes.Count != abc.Instances.Count) {
                throw new Exception("Number of Classes and Instances differs");
            }
            var classCount = abc.Classes.Count;
            WriteU30((uint)classCount);
            WriteMultipleInstances(abc.Instances);
            WriteMultipleClasses(abc.Classes);

            WriteMultipleScripts(abc.Scripts);
            WriteMultipleBodies(abc.Bodies);
        }

        private void WriteMultipleMethods(IReadOnlyCollection<AsMethodInfo> methods) {
            WriteU30((uint)methods.Count);
            foreach (var value in methods) {
                WriteMethodInfo(value);
            }
        }

        private void WriteMultipleMetadata(IReadOnlyCollection<AsMetadataInfo> vals) {
            WriteU30((uint)vals.Count);
            foreach (var value in vals) {
                WriteMetadata(value);
            }
        }

        private void WriteMultipleInstances(IReadOnlyCollection<AsInstanceInfo> vals) {
            foreach (var value in vals) {
                WriteInstance(value);
            }
        }

        private void WriteMultipleClasses(IReadOnlyCollection<AsClassInfo> vals) {
            foreach (var value in vals) {
                WriteClass(value);
            }
        }

        private void WriteMultipleScripts(IReadOnlyCollection<AsScriptInfo> scripts) {
            WriteU30((uint)scripts.Count);
            foreach (var value in scripts) {
                WriteScript(value);
            }
        }

        private void WriteMultipleBodies(IReadOnlyCollection<AsMethodBodyInfo> bodies) {
            WriteU30((uint)bodies.Count);
            foreach (var value in bodies) {
                WriteMethodBody(value);
            }
        }

        public void WriteConstantPool(AsConstantPoolInfo constantPool) {
            WriteU30((uint)(constantPool.Integers.Count <= 1 ? 0 : constantPool.Integers.Count));
            for (var i = 1; i < constantPool.Integers.Count; i++) {
                WriteS32(constantPool.Integers[i]);
            }

            WriteU30((uint)(constantPool.UnsignedIntegers.Count <= 1 ? 0 : constantPool.UnsignedIntegers.Count));
            for (var i = 1; i < constantPool.UnsignedIntegers.Count; i++) {
                WriteU32(constantPool.UnsignedIntegers[i]);
            }

            WriteU30((uint)(constantPool.Doubles.Count <= 1 ? 0 : constantPool.Doubles.Count));
            for (var i = 1; i < constantPool.Doubles.Count; i++) {
                WriteD64(constantPool.Doubles[i]);
            }

            WriteU30((uint)(constantPool.Strings.Count <= 1 ? 0 : constantPool.Strings.Count));
            for (var i = 1; i < constantPool.Strings.Count; i++) {
                WriteString(constantPool.Strings[i]);
            }

            WriteU30((uint)(constantPool.Namespaces.Count <= 1 ? 0 : constantPool.Namespaces.Count));
            for (var i = 1; i < constantPool.Namespaces.Count; i++) {
                WriteNamespace(constantPool.Namespaces[i]);
            }

            WriteU30((uint)(constantPool.NamespaceSets.Count <= 1 ? 0 : constantPool.NamespaceSets.Count));
            for (var i = 1; i < constantPool.NamespaceSets.Count; i++)
                WriteNamespaceSet(constantPool.NamespaceSets[i]);

            WriteU30((uint)(constantPool.Multinames.Count <= 1 ? 0 : constantPool.Multinames.Count));
            for (var i = 1; i < constantPool.Multinames.Count; i++)
                WriteMultiname(constantPool.Multinames[i]);

        }

        private void WriteNamespace(in AsNamespaceInfo ns) {
            WriteU8((byte)ns.Kind);
            WriteU30(ns.Name);
        }

        private void WriteNamespaceSet(in AsNamespaceSetInfo nss) {
            WriteMutipleU30(nss.Namespaces);
        }

        private void WriteMultiname(in AsMultinameInfo multiname) {
            WriteU8((byte)multiname.Kind);
            switch (multiname.Kind) {
                case AsMultinameKind.QName:
                case AsMultinameKind.QNameA:
                    WriteU30(multiname.QName.Namespace);
                    WriteU30(multiname.QName.Name);
                    break;
                case AsMultinameKind.RTQName:
                case AsMultinameKind.RTQNameA:
                    WriteU30(multiname.RtqName.Name);
                    break;
                case AsMultinameKind.RTQNameL:
                case AsMultinameKind.RTQNameLA:
                    break;
                case AsMultinameKind.Multiname:
                case AsMultinameKind.MultinameA:
                    WriteU30(multiname.Multiname.Name);
                    WriteU30(multiname.Multiname.NamespaceSet);
                    break;
                case AsMultinameKind.MultinameL:
                case AsMultinameKind.MultinameLA:
                    WriteU30(multiname.MultinameL.NamespaceSet);
                    break;
                case AsMultinameKind.Generic:
                    WriteU30(multiname.TypeName.Name);
                    WriteU30((uint)multiname.TypeName.Params.Length);
                    foreach (var value in multiname.TypeName.Params)
                        WriteU30(value);
                    break;
                default:
                    throw new Exception("Unknown Multiname kind");
            }
        }

        private void WriteMethodInfo(AsMethodInfo methodInfo) {
            var paramCount = methodInfo.ParamTypes.Length;
            WriteU30((uint)paramCount);
            WriteU30(methodInfo.ReturnType);
            WriteMutipleU30(methodInfo.ParamTypes, true);
            WriteU30(methodInfo.Name);
            WriteU8((byte)methodInfo.Flags);
            if (methodInfo.HasOptional) {
                WriteU30((uint)methodInfo.Options.Length);
                foreach (var option in methodInfo.Options) {
                    WriteOptionDetail(option);
                }
            }
            if (methodInfo.HasParamNames) {
                if (methodInfo.ParamNames.Length != methodInfo.ParamTypes.Length) {
                    throw new Exception("Mismatching number of parameter names and types");
                }
                foreach (var paramInfo in methodInfo.ParamNames) {
                    WriteParamInfo(paramInfo);
                }
            }
        }

        private void WriteOptionDetail(AsOptionDetailInfo optionDetail) {
            WriteU30(optionDetail.Value);
            WriteU8((byte)optionDetail.Kind);
        }

        private void WriteParamInfo(AsParamInfo paramInfo) {
            WriteU30(paramInfo.ParamName);
        }

        private void WriteMetadata(AsMetadataInfo metadata) {
            WriteU30(metadata.Name);
            WriteU30((uint)metadata.Items.Length);
            foreach (var item in metadata.Items) {
                WriteU30(item.Key);
            }
            foreach (var item in metadata.Items) {
                WriteU30(item.Value);
            }
        }

        private void WriteInstance(AsInstanceInfo instance) {
            WriteU30(instance.Name);
            WriteU30(instance.SuperName);
            WriteU8((byte)instance.Flags);
            if (instance.HasProtectedNs)
                WriteU30(instance.ProtectedNs);
            WriteMutipleU30(instance.Interfaces);
            WriteU30(instance.InstanceInitializer);
            WriteMultipleTraits(instance.Traits);
        }

        private void WriteMultipleTraits(IReadOnlyCollection<AsTraitsInfo> traits) {
            WriteU30((uint)traits.Count);
            foreach (var trait in traits) {
                WriteTrait(trait);
            }
        }

        private void WriteTrait(AsTraitsInfo trait) {
            WriteU30(trait.Name);
            WriteU8(trait.Flags);
            switch (trait.Kind) {
                case AsTraitKind.Slot:
                case AsTraitKind.Const:
                    WriteU30(trait.Slot.SlotId);
                    WriteU30(trait.Slot.TypeName);
                    WriteU30(trait.Slot.ValueIndex);
                    if (trait.Slot.ValueIndex != 0) {
                        WriteU8((byte)trait.Slot.ValueKind);
                    }
                    break;
                case AsTraitKind.Class:
                    WriteU30(trait.Class.SlotId);
                    WriteU30(trait.Class.Class);
                    break;
                case AsTraitKind.Function:
                    WriteU30(trait.Function.SlotId);
                    WriteU30(trait.Function.Function);
                    break;
                case AsTraitKind.Method:
                case AsTraitKind.Getter:
                case AsTraitKind.Setter:
                    WriteU30(trait.Method.DispId);
                    WriteU30(trait.Method.Method);
                    break;
                default:
                    throw new Exception("Unknown trait kind");
            }
            if (trait.HasMetadata) {
                WriteMutipleU30(trait.Metadata);
            }
        }

        private void WriteClass(AsClassInfo @class) {
            WriteU30(@class.ClassInitializer);
            WriteMultipleTraits(@class.Traits);
        }

        private void WriteScript(AsScriptInfo script) {
            WriteU30(script.ScriptInitializer);
            WriteMultipleTraits(script.Traits);
        }

        private void WriteMethodBody(AsMethodBodyInfo methodBody) {
            WriteU30(methodBody.Method);
            WriteU30(methodBody.MaxStack);
            WriteU30(methodBody.LocalCount);
            WriteU30(methodBody.InitScopeDepth);
            WriteU30(methodBody.MaxScopeDepth);

            WriteU30((uint)methodBody.Code.Length);
            _writer.Write(methodBody.Code);

            WriteMultipleExceptions(methodBody.Exceptions);
            WriteMultipleTraits(methodBody.Traits);
        }

        private void WriteMultipleExceptions(AsExceptionInfo[] exceptions) {
            WriteU30((uint)exceptions.Length);
            foreach (var exc in exceptions) {
                WriteExceptionInfo(exc);
            }
        }

        private void WriteExceptionInfo(AsExceptionInfo exc) {
            WriteU30(exc.From);
            WriteU30(exc.To);
            WriteU30(exc.Target);
            WriteU30(exc.ExceptionType);
            WriteU30(exc.VariableName);
        }

        #region Primitives

        public void WriteMutipleU30(uint[] vals, bool skipLength = false) {
            if (!skipLength) {
                WriteU30((uint)vals.Length);
            }
            foreach (var val in vals) {
                WriteU30(val);
            }
        }

        public void WriteU8(byte v) {
            _writer.Write(v);
        }

        public void WriteU16(ushort v) {
            _writer.Write(v);
        }

        public void WriteU30(uint val) {
            if (val >= (1 << 30)) {
                throw new ArgumentOutOfRangeException("val");
            }
            WriteU32(val);
        }

        public void WriteU32(ulong val) {
            if (val < 0x80) {
                _writer.Write((byte)val);
            } else {
                _writer.Write((byte)((val & 0x7f) | 0x80));
                val = val >> 7;

                if (val < 0x80) {
                    _writer.Write((byte)val);
                } else {
                    _writer.Write((byte)((val & 0x7f) | 0x80));
                    val = val >> 7;

                    if (val < 0x80) {
                        _writer.Write((byte)val);
                    } else {
                        _writer.Write((byte)((val & 0x7f) | 0x80));
                        val = val >> 7;

                        if (val < 0x80) {
                            _writer.Write((byte)val);
                        } else {
                            _writer.Write((byte)((val & 0x7f) | 0x80));
                            val = val >> 7;

                            _writer.Write((byte)(val & 0x7f));
                        }
                    }
                }
            }
        }

        public void WriteS32(int val) {
            WriteU32((uint)val);
        }

        public void WriteD64(double val) {
            var l = BitConverter.DoubleToInt64Bits(val);
            _writer.Write(l);
        }

        public void WriteString(string val) {
            var bytes = Encoding.UTF8.GetBytes(val);
            WriteU30((uint)bytes.Length);
            _writer.Write(bytes);
        }

        #endregion

    }
}
