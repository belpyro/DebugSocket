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
    public class Rotate_Wrapper {
        
        private double _x;
        
        private double _y;
        
        private double _z;
        
        private ModuleBase_Wrapper _item;
        
        private int _sourcemodulecount;
        
        private bool _isdisposed;
        
        public double X {
            get {
                return _x;
            }
            set {
                _x = value;
            }
        }
        
        public double Y {
            get {
                return _y;
            }
            set {
                _y = value;
            }
        }
        
        public double Z {
            get {
                return _z;
            }
            set {
                _z = value;
            }
        }
        
        public ModuleBase_Wrapper Item {
            get {
                return this._item;
            }
            set {
                this._item = value;
            }
        }
        
        public int SourceModuleCount {
            get {
                return _sourcemodulecount;
            }
            set {
                _sourcemodulecount = value;
            }
        }
        
        public bool IsDisposed {
            get {
                return _isdisposed;
            }
            set {
                _isdisposed = value;
            }
        }
    }
}
