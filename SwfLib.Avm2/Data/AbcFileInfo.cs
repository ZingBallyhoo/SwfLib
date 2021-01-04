using System.Collections.Generic;

namespace SwfLib.Avm2.Data {
    /// <summary>
    /// Represents abc file relational database.
    /// </summary>
    public class AbcFileInfo {

        public ushort MinorVersion;
        
        public ushort MajorVersion;

        public AsConstantPoolInfo ConstantPool;

        public List<AsMethodInfo> Methods;
        
        public List<AsMetadataInfo> Metadata;
        
        public List<AsInstanceInfo> Instances;
        
        public List<AsClassInfo> Classes;
        
        public List<AsScriptInfo> Scripts;
        
        public List<AsMethodBodyInfo> Bodies;

    }
}
