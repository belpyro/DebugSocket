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
    public class Compass_Wrapper {
        
        private float _magneticheading;
        
        private float _trueheading;
        
        private float _headingaccuracy;
        
        private Vector3_Wrapper _rawvector;
        
        private double _timestamp;
        
        private bool _enabled;
        
        public float magneticHeading {
            get {
                return _magneticheading;
            }
            set {
                _magneticheading = value;
            }
        }
        
        public float trueHeading {
            get {
                return _trueheading;
            }
            set {
                _trueheading = value;
            }
        }
        
        public float headingAccuracy {
            get {
                return _headingaccuracy;
            }
            set {
                _headingaccuracy = value;
            }
        }
        
        public Vector3_Wrapper rawVector {
            get {
                return this._rawvector;
            }
            set {
                this._rawvector = value;
            }
        }
        
        public double timestamp {
            get {
                return _timestamp;
            }
            set {
                _timestamp = value;
            }
        }
        
        public bool enabled {
            get {
                return _enabled;
            }
            set {
                _enabled = value;
            }
        }
    }
}
