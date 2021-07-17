using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SwfLib.Avm2.Data;
using SwfLib.Avm2.Opcodes;

namespace SwfLib.Avm2 {
    public class AbcFileLoader {

        public readonly AbcFileInfo FileInfo;

        public readonly IList<AbcClass> Classes = new List<AbcClass>();

        public readonly IList<AbcScript> Scripts = new List<AbcScript>();

        public readonly IList<AbcMethod> Methods = new List<AbcMethod>();

        public readonly IList<AbcMetadata> Metadata = new List<AbcMetadata>();

        public readonly IList<AbcNamespace> Namespaces = new List<AbcNamespace>();

        public readonly IList<AbcNamespaceSet> NamespaceSets = new List<AbcNamespaceSet>();

        public readonly IList<AbcMultiname> Multinames = new List<AbcMultiname>();

        private readonly IList<AbcMetadata> _metadata = new List<AbcMetadata>();

        public AbcFileLoader(AbcFileInfo fileInfo) {
            FileInfo = fileInfo;

            ReadConstants();

            for (var i = 0; i < fileInfo.Classes.Count; i++) {
                Classes.Add(new AbcClass
                {
                    ID = i
                });
            }

            for (var i = 0; i < fileInfo.Scripts.Count; i++) {
                Scripts.Add(new AbcScript());
            }

            foreach (var metaInfo in fileInfo.Metadata) {
                _metadata.Add(ReadMetadata(metaInfo));
            }

            var j = 0u;
            foreach (var methodInfo in fileInfo.Methods) {
                var method = new AbcMethod {
                    ID = j++,
                    Name = fileInfo.ConstantPool.Strings[(int)methodInfo.Name],
                    ReturnType = GetMultiname(methodInfo.ReturnType, AbcMultiname.Void),
                    NeedArguments = methodInfo.NeedArguments,
                    NeedActivation = methodInfo.NeedActivation,
                    NeedRest = methodInfo.NeedRest,
                    SetDxns = methodInfo.SetDxns,
                    IgnoreRest = methodInfo.IgnoreRest,
                    Native = methodInfo.Native,
                };
                for (var paramIndex = 0; paramIndex < methodInfo.ParamTypes.Length; paramIndex++) {
                    var paramInfo = methodInfo.ParamTypes[paramIndex];
                    method.Params.Add(new AbcMethodParam {
                        Type = GetMultiname(paramInfo, AbcMultiname.Any),
                        Name = methodInfo.HasParamNames ? fileInfo.ConstantPool.Strings[(int)methodInfo.ParamNames[paramIndex].ParamName] : null
                    });
                }
                if (methodInfo.HasOptional) {
                    var paramShift = methodInfo.ParamTypes.Length - methodInfo.Options.Length;
                    for (var i = 0; i < methodInfo.ParamTypes.Length; i++) {
                        var defaultIndex = i - paramShift;
                        if (defaultIndex >= 0) {
                            var param = method.Params[i];
                            var option = methodInfo.Options[defaultIndex];
                            param.Default = GetConstantValue(option.Kind, option.Value);
                        }
                    }
                }
                Methods.Add(method);
            }

            foreach (var instance in fileInfo.Instances)
            {
                foreach (var trait in instance.Traits)
                {
                    if (trait.Kind != AsTraitKind.Method) continue;
                    if (trait.Method.Method == 18038)
                    {
                        
                    }
                }
            }
            
            foreach (var instance in fileInfo.Classes)
            {
                foreach (var trait in instance.Traits)
                {
                    if (trait.Kind != AsTraitKind.Method) continue;
                    if (trait.Method.Method == 18038)
                    {
                        
                    }
                }
            }
            
            foreach (var instance in fileInfo.Scripts)
            {
                foreach (var trait in instance.Traits)
                {
                    if (trait.Kind != AsTraitKind.Method) continue;
                    if (trait.Method.Method == 18038)
                    {
                        
                    }
                }
            }
            
            LoadClassInstances();
            LoadMethodBodies();
            LoadClassInitializers();
            LoadScriptInitializers();
            LoadTraits();
        }

        private void LoadClassInstances() {
            for (var i = 0; i < FileInfo.Instances.Count; i++) {
                var instanceInfo = FileInfo.Instances[i];
                var @class = Classes[i];
                @class.Instance = new AbcInstance {
                    ID = i,
                    Name = GetMultiname(instanceInfo.Name, null),
                    SuperName = GetMultiname(instanceInfo.SuperName, AbcMultiname.Void),
                };
                foreach (var index in instanceInfo.Interfaces) {
                    @class.Instance.Interfaces.Add(GetMultiname(index, null));
                }
            }
        }

        private void LoadScriptInitializers() {
            for (var i = 0; i < FileInfo.Scripts.Count; i++) {
                var scriptInfo = FileInfo.Scripts[i];
                var script = Scripts[i];
                script.ScriptInitializer = GetMethod(scriptInfo.ScriptInitializer);
            }
        }

        private void LoadTraits() {
            for (var i = 0; i < FileInfo.Scripts.Count; i++) {
                var scriptInfo = FileInfo.Scripts[i];
                var script = Scripts[i];
                AddTraits(script.Traits, scriptInfo.Traits);
            }
            foreach (var bodyInfo in FileInfo.Bodies) {
                var method = GetMethod(bodyInfo.Method);
                var methodBody = method.Body;
                if (methodBody == null) continue; // todo; unloadble bodies
                AddTraits(methodBody.Traits, bodyInfo.Traits);
            }
            for (var i = 0; i < FileInfo.Instances.Count; i++) {
                var instanceInfo = FileInfo.Instances[i];
                var instance = Classes[i].Instance;
                AddTraits(instance.Traits, instanceInfo.Traits);
            }
            for (var i = 0; i < FileInfo.Classes.Count; i++) {
                var classInfo = FileInfo.Classes[i];
                var @class = Classes[i];
                AddTraits(@class.Traits, classInfo.Traits);
            }
        }

        private AbcMetadata ReadMetadata(AsMetadataInfo metaInfo) {
            var res = new AbcMetadata {
                Name = FileInfo.ConstantPool.Strings[(int)metaInfo.Name]
            };
            foreach (var item in metaInfo.Items) {
                res.Items.Add(new AbcMetadataItem {
                    Key = FileInfo.ConstantPool.Strings[(int)item.Key],
                    Value = FileInfo.ConstantPool.Strings[(int)item.Value]
                });
            }
            return res;
        }


        private void ReadConstants() {
            foreach (var ns in FileInfo.ConstantPool.Namespaces) {
                Namespaces.Add(new AbcNamespace {
                    Kind = ns.Kind,
                    Name = FileInfo.ConstantPool.Strings[(int)ns.Name]
                });
            }

            foreach (var nssInfo in FileInfo.ConstantPool.NamespaceSets) {
                var ns = new AbcNamespaceSet();
                if (nssInfo.Namespaces != null) {
                    foreach (var nsInfo in nssInfo.Namespaces) {
                        ns.Namespaces.Add(GetNamespace(nsInfo, AbcNamespace.Any));
                    }
                }
                NamespaceSets.Add(ns);
            }

            foreach (var multiname in FileInfo.ConstantPool.Multinames) {
                switch (multiname.Kind) {
                    case AsMultinameKind.QName:
                        Multinames.Add(new AbcMultinameQName {
                            Name = FileInfo.ConstantPool.Strings[(int)multiname.QName.Name],
                            Namespace = GetNamespace(multiname.QName.Namespace, AbcNamespace.Any)
                        });
                        break;
                    case AsMultinameKind.QNameA:
                        Multinames.Add(new AbcMultinameQNameA {
                            Name = FileInfo.ConstantPool.Strings[(int)multiname.QName.Name],
                            Namespace = GetNamespace(multiname.QName.Namespace, AbcNamespace.Any)
                        });
                        break;
                    case AsMultinameKind.Void:
                        Multinames.Add(AbcMultiname.Void);
                        break;
                    case AsMultinameKind.Multiname:
                        Multinames.Add(new AbcMultinameMultiname {
                            Name = FileInfo.ConstantPool.Strings[(int)multiname.Multiname.Name],
                            NamespaceSet = GetNamespaceSet(multiname.Multiname.NamespaceSet, null)
                        });
                        break;
                    case AsMultinameKind.MultinameA:
                    case AsMultinameKind.MultinameL:
                    case AsMultinameKind.MultinameLA:
                    case AsMultinameKind.RTQName:
                    case AsMultinameKind.RTQNameA:
                        //throw new NotImplementedException();
                        Multinames.Add(AbcMultiname.Void); //todo:
                        break;
                    case AsMultinameKind.RTQNameL:
                    case AsMultinameKind.RTQNameLA:
                        //throw new NotImplementedException();
                        Multinames.Add(AbcMultiname.Void); //todo:
                        break;
                    case AsMultinameKind.Generic:
                        Multinames.Add(new AbcMultinameGeneric());
                        break;
                    default:
                        throw new Exception("Unsupported multiname kind " + multiname.Kind);
                }
            }
            for (var i = 0; i < FileInfo.ConstantPool.Multinames.Count; i++) {
                var multiname = FileInfo.ConstantPool.Multinames[i];

                Multinames[i].Index = (uint)i;
                
                if (multiname.Kind == AsMultinameKind.Generic) {
                    var vector = (AbcMultinameGeneric)Multinames[i];
                    vector.Name = GetMultiname(multiname.TypeName.Name, null);
                    foreach (var arg in multiname.TypeName.Params) {
                        vector.Params.Add(GetMultiname(arg, AbcMultiname.Any));
                    }
                }
            }
        }

        private void LoadClassInitializers() {
            for (var i = 0; i < Classes.Count; i++) {
                var cls = Classes[i];
                var clsInfo = FileInfo.Classes[i];
                var iInfo = FileInfo.Instances[i];
                cls.ClassInitializer = GetMethod(clsInfo.ClassInitializer);
                cls.Instance.InstanceInitializer = GetMethod(iInfo.InstanceInitializer);
            }
        }

        private void LoadMethodBodies() {
            var bodies = FileInfo.Bodies.ToDictionary(item => item.Method);
            for (var i = 0; i < Methods.Count; i++) {
                var method = Methods[i];
                AsMethodBodyInfo body;
                bodies.TryGetValue((uint)i, out body);
                try
                {
                    method.Body = body != null ? GetMethodBody(body) : null;
                } catch (Exception e)
                {
                    //Console.Out.WriteLine(e);
                }
            }
        }

        private AbcMethodBody GetMethodBody(AsMethodBodyInfo info) {
            var opcodeReader = new Avm2OpcodeReader(this);

            var res = new AbcMethodBody {
                ID = info.Method,
                MaxStack = info.MaxStack,
                LocalCount = info.LocalCount,
                InitScopeDepth = info.InitScopeDepth,
                MaxScopeDepth = info.MaxScopeDepth,
            };

            if (res.ID == 2929)
            {
                
            }

            object prevCode = null;
            
            var reader = new AbcDataReader(new MemoryStream(info.Code, false));
            var factory = new Avm2OpcodeFactory();
            while (!reader.IsEOF) {
                var offset = reader.Position;
                var code = (Avm2Opcode)reader.ReadU8();
                var opcode = factory.CreateOpcode(code);
                opcode.Code = code;
                opcode.AcceptVisitor(opcodeReader, reader);
                res.Code.Add(new AbcMethodBodyInstruction {
                    Offset = (uint)offset,
                    EndOffset = (uint)reader.Position,
                    Opcode = opcode
                });
                prevCode = opcode;
            }
            foreach (var exc in info.Exceptions) {
                res.Exceptions.Add(GetExceptionBlock(exc));
            }
            return res;
        }

        private AbcExceptionBlock GetExceptionBlock(AsExceptionInfo info) {
            return new AbcExceptionBlock {
                From = info.From,
                To = info.To,
                Target = info.Target,
                ExceptionType = GetMultiname(info.ExceptionType, AbcMultiname.Any),
                VariableName = GetMultiname(info.VariableName, AbcMultiname.Void)
            };
        }

        private void AddTraits(ICollection<AbcTrait> target, IEnumerable<AsTraitsInfo> infos) {
            foreach (var info in infos) {
                target.Add(GetTrait(info));
            }
        }

        private AbcTrait GetTrait(AsTraitsInfo traitInfo) {
            AbcTrait trait;
            switch (traitInfo.Kind) {
                case AsTraitKind.Const:
                    trait = new AbcConstTrait {
                        SlotId = traitInfo.Slot.SlotId,
                        TypeName = GetMultiname(traitInfo.Slot.TypeName, AbcMultiname.Any),
                        Value = GetConstantValue(traitInfo.Slot.ValueKind, traitInfo.Slot.ValueIndex)
                    };
                    break;
                case AsTraitKind.Slot:
                    trait = new AbcSlotTrait {
                        SlotId = traitInfo.Slot.SlotId,
                        TypeName = GetMultiname(traitInfo.Slot.TypeName, AbcMultiname.Any),
                        Value = GetConstantValue(traitInfo.Slot.ValueKind, traitInfo.Slot.ValueIndex)
                    };
                    break;
                case AsTraitKind.Class:
                    trait = new AbcClassTrait {
                        SlotId = traitInfo.Class.SlotId,
                        Class = GetClass(traitInfo.Class.Class)
                    };
                    break;
                case AsTraitKind.Function:
                    trait = new AbcFunctionTrait {
                        SlotId = traitInfo.Function.SlotId,
                        Method = GetMethod(traitInfo.Function.Function)
                    };
                    break;
                case AsTraitKind.Method:
                    trait = new AbcMethodTrait {
                        DispId = traitInfo.Method.DispId,
                        Method = GetMethod(traitInfo.Method.Method)
                    };
                    break;
                case AsTraitKind.Getter:
                    trait = new AbcGetterTrait {
                        DispId = traitInfo.Method.DispId,
                        Method = GetMethod(traitInfo.Method.Method)
                    };
                    break;
                case AsTraitKind.Setter:
                    trait = new AbcSetterTrait {
                        DispId = traitInfo.Method.DispId,
                        Method = GetMethod(traitInfo.Method.Method)
                    };
                    break;
                default:
                    throw new Exception("unsupported trait kind " + traitInfo.Kind);
            }

            trait.Name = GetMultiname(traitInfo.Name, null);
            trait.Final = traitInfo.Final;
            trait.Override = traitInfo.Override;

            if (traitInfo.Metadata != null) {
                foreach (var metaIndex in traitInfo.Metadata) {
                    var meta = GetMetadata(metaIndex);
                    trait.Metadata.Add(meta);
                }
            }
            return trait;
        }

        private AbcConstant GetConstantValue(AsConstantKind kind, uint value) {
            switch (kind) {
                case AsConstantKind.Integer:
                    return FileInfo.ConstantPool.Integers[(int)value];
                case AsConstantKind.UInteger:
                    return FileInfo.ConstantPool.UnsignedIntegers[(int)value];
                case AsConstantKind.Double:
                    return FileInfo.ConstantPool.Doubles[(int)value];
                case AsConstantKind.String:
                    return FileInfo.ConstantPool.Strings[(int)value];
                case AsConstantKind.True:
                    return true;
                case AsConstantKind.False:
                    return false;
                case AsConstantKind.Null:
                    return AbcConstant.Null;
                case AsConstantKind.Undefined:
                    return AbcConstant.Undefined;
                case AsConstantKind.Namespace:
                    return GetNamespace(value, AbcNamespace.Any);
                //todo: other types
                default:
                    throw new Exception("unknown constant");
            }
        }

        public void SaveTo(AbcFile file) {
            foreach (var method in Methods) {
                file.Methods.Add(method);
            }
            foreach (var @class in Classes) {
                file.Classes.Add(@class);
            }
            foreach (var script in Scripts) {
                file.Scripts.Add(script);
            }
        }

        public AbcMultiname GetMultiname(uint index, AbcMultiname zeroMeaning) {
            if (index != 0) return Multinames[(int)index];
            if (zeroMeaning == null) throw new Exception("zero multiname is not allowed in current context");
            return zeroMeaning;
        }

        public AbcNamespace GetNamespace(uint index, AbcNamespace zeroMeaning) {
            if (index != 0) return Namespaces[(int)index];
            if (zeroMeaning == null) throw new Exception("zero namespace is not allowed in current context");
            return zeroMeaning;
        }

        public AbcNamespaceSet GetNamespaceSet(uint index, AbcNamespaceSet zeroMeaning) {
            if (index != 0) return NamespaceSets[(int)index];
            if (zeroMeaning == null) throw new Exception("zero namespace space is not allowed in current context");
            return zeroMeaning;
        }

        public AbcMethod GetMethod(uint index) {
            return Methods[(int)index];
        }

        public AbcClass GetClass(uint index) {
            return Classes[(int)index];
        }

        private AbcMetadata GetMetadata(uint index) {
            return _metadata[(int)index];
        }

        public string GetString(uint index) {
            return FileInfo.ConstantPool.Strings[(int)index];
        }

        public double GetDouble(uint index) {
            return FileInfo.ConstantPool.Doubles[(int)index];
        }

        public int GetInteger(uint index) {
            return FileInfo.ConstantPool.Integers[(int)index];
        }

        public uint GetUInteger(uint index) {
            return FileInfo.ConstantPool.UnsignedIntegers[(int)index];
        }
    }
}
