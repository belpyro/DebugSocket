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
    public class ReachSpace_Wrapper {
        
        private string _id;
        
        private bool _isreached;
        
        private bool _iscomplete;
        
        private ProgressTree _subtree;
        
        public string Id {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }
        
        public bool IsReached {
            get {
                return _isreached;
            }
            set {
                _isreached = value;
            }
        }
        
        public bool IsComplete {
            get {
                return _iscomplete;
            }
            set {
                _iscomplete = value;
            }
        }
        
        public ProgressTree Subtree {
            get {
                return _subtree;
            }
            set {
                _subtree = value;
            }
        }
    }
}
