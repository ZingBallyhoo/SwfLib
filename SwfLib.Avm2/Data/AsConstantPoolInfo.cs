using System.Collections.Generic;

namespace SwfLib.Avm2.Data {
    public class AsConstantPoolInfo {

        public List<int> Integers;

        public List<uint> UnsignedIntegers;

        public List<double> Doubles;

        public List<string> Strings;

        public List<AsNamespaceInfo> Namespaces;

        public List<AsNamespaceSetInfo> NamespaceSets;

        public List<AsMultinameInfo> Multinames;

    }
}
