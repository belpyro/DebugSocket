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
    public class Displace_Wrapper {
        
        private ModuleBase_Wrapper _x;
        
        private ModuleBase_Wrapper _y;
        
        private ModuleBase_Wrapper _z;
        
        private ModuleBase_Wrapper _item;
        
        private int _sourcemodulecount;
        
        private bool _isdisposed;
        
        public ModuleBase_Wrapper X {
            get {
                return this._x;
            }
            set {
                this._x = value;
            }
        }
        
        public ModuleBase_Wrapper Y {
            get {
                return this._y;
            }
            set {
                this._y = value;
            }
        }
        
        public ModuleBase_Wrapper Z {
            get {
                return this._z;
            }
            set {
                this._z = value;
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
