//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.34014
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wrappers {
    
    
    [System.SerializableAttribute()]
    public class Capability_Wrapper {
        
        private string _category;
        
        private string _capabilityname;
        
        private System.Collections.Generic.List<Capability.Value> _values;
        
        public string category {
            get {
                return _category;
            }
            set {
                _category = value;
            }
        }
        
        public string capabilityName {
            get {
                return _capabilityname;
            }
            set {
                _capabilityname = value;
            }
        }
        
        public System.Collections.Generic.List<Capability.Value> values {
            get {
                return _values;
            }
            set {
                _values = value;
            }
        }
    }
}
