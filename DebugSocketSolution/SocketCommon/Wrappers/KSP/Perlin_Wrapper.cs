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
    public class Perlin_Wrapper {
        
        private double _frequency;
        
        private double _lacunarity;
        
        private LibNoise.Unity.QualityMode _quality;
        
        private int _octavecount;
        
        private double _persistence;
        
        private int _seed;
        
        private ModuleBase_Wrapper _item;
        
        private int _sourcemodulecount;
        
        private bool _isdisposed;
        
        public double Frequency {
            get {
                return _frequency;
            }
            set {
                _frequency = value;
            }
        }
        
        public double Lacunarity {
            get {
                return _lacunarity;
            }
            set {
                _lacunarity = value;
            }
        }
        
        public LibNoise.Unity.QualityMode Quality {
            get {
                return _quality;
            }
            set {
                _quality = value;
            }
        }
        
        public int OctaveCount {
            get {
                return _octavecount;
            }
            set {
                _octavecount = value;
            }
        }
        
        public double Persistence {
            get {
                return _persistence;
            }
            set {
                _persistence = value;
            }
        }
        
        public int Seed {
            get {
                return _seed;
            }
            set {
                _seed = value;
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
