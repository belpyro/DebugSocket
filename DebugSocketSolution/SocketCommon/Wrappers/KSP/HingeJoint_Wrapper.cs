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
    public class HingeJoint_Wrapper {
        
        private JointMotor_Wrapper _motor;
        
        private JointLimits_Wrapper _limits;
        
        private JointSpring_Wrapper _spring;
        
        private bool _usemotor;
        
        private bool _uselimits;
        
        private bool _usespring;
        
        private float _velocity;
        
        private float _angle;
        
        private Rigidbody_Wrapper _connectedbody;
        
        private Vector3_Wrapper _axis;
        
        private Vector3_Wrapper _anchor;
        
        private Vector3_Wrapper _connectedanchor;
        
        private bool _autoconfigureconnectedanchor;
        
        private float _breakforce;
        
        private float _breaktorque;
        
        private bool _enablecollision;
        
        private Transform_Wrapper _transform;
        
        private Rigidbody_Wrapper _rigidbody;
        
        private Rigidbody2D_Wrapper _rigidbody2d;
        
        private Camera_Wrapper _camera;
        
        private Light_Wrapper _light;
        
        private Animation_Wrapper _animation;
        
        private ConstantForce_Wrapper _constantforce;
        
        private Renderer_Wrapper _renderer;
        
        private AudioSource_Wrapper _audio;
        
        private GUIText_Wrapper _guitext;
        
        private NetworkView_Wrapper _networkview;
        
        private GUIElement_Wrapper _guielement;
        
        private GUITexture_Wrapper _guitexture;
        
        private Collider_Wrapper _collider;
        
        private Collider2D_Wrapper _collider2d;
        
        private HingeJoint_Wrapper _hingejoint;
        
        private ParticleEmitter_Wrapper _particleemitter;
        
        private ParticleSystem_Wrapper _particlesystem;
        
        private GameObject_Wrapper _gameobject;
        
        private bool _active;
        
        private string _tag;
        
        private string _name;
        
        private UnityEngine.HideFlags _hideflags;
        
        public JointMotor_Wrapper motor {
            get {
                return this._motor;
            }
            set {
                this._motor = value;
            }
        }
        
        public JointLimits_Wrapper limits {
            get {
                return this._limits;
            }
            set {
                this._limits = value;
            }
        }
        
        public JointSpring_Wrapper spring {
            get {
                return this._spring;
            }
            set {
                this._spring = value;
            }
        }
        
        public bool useMotor {
            get {
                return _usemotor;
            }
            set {
                _usemotor = value;
            }
        }
        
        public bool useLimits {
            get {
                return _uselimits;
            }
            set {
                _uselimits = value;
            }
        }
        
        public bool useSpring {
            get {
                return _usespring;
            }
            set {
                _usespring = value;
            }
        }
        
        public float velocity {
            get {
                return _velocity;
            }
            set {
                _velocity = value;
            }
        }
        
        public float angle {
            get {
                return _angle;
            }
            set {
                _angle = value;
            }
        }
        
        public Rigidbody_Wrapper connectedBody {
            get {
                return this._connectedbody;
            }
            set {
                this._connectedbody = value;
            }
        }
        
        public Vector3_Wrapper axis {
            get {
                return this._axis;
            }
            set {
                this._axis = value;
            }
        }
        
        public Vector3_Wrapper anchor {
            get {
                return this._anchor;
            }
            set {
                this._anchor = value;
            }
        }
        
        public Vector3_Wrapper connectedAnchor {
            get {
                return this._connectedanchor;
            }
            set {
                this._connectedanchor = value;
            }
        }
        
        public bool autoConfigureConnectedAnchor {
            get {
                return _autoconfigureconnectedanchor;
            }
            set {
                _autoconfigureconnectedanchor = value;
            }
        }
        
        public float breakForce {
            get {
                return _breakforce;
            }
            set {
                _breakforce = value;
            }
        }
        
        public float breakTorque {
            get {
                return _breaktorque;
            }
            set {
                _breaktorque = value;
            }
        }
        
        public bool enableCollision {
            get {
                return _enablecollision;
            }
            set {
                _enablecollision = value;
            }
        }
        
        public Transform_Wrapper transform {
            get {
                return this._transform;
            }
            set {
                this._transform = value;
            }
        }
        
        public Rigidbody_Wrapper rigidbody {
            get {
                return this._rigidbody;
            }
            set {
                this._rigidbody = value;
            }
        }
        
        public Rigidbody2D_Wrapper rigidbody2D {
            get {
                return this._rigidbody2d;
            }
            set {
                this._rigidbody2d = value;
            }
        }
        
        public Camera_Wrapper camera {
            get {
                return this._camera;
            }
            set {
                this._camera = value;
            }
        }
        
        public Light_Wrapper light {
            get {
                return this._light;
            }
            set {
                this._light = value;
            }
        }
        
        public Animation_Wrapper animation {
            get {
                return this._animation;
            }
            set {
                this._animation = value;
            }
        }
        
        public ConstantForce_Wrapper constantForce {
            get {
                return this._constantforce;
            }
            set {
                this._constantforce = value;
            }
        }
        
        public Renderer_Wrapper renderer {
            get {
                return this._renderer;
            }
            set {
                this._renderer = value;
            }
        }
        
        public AudioSource_Wrapper audio {
            get {
                return this._audio;
            }
            set {
                this._audio = value;
            }
        }
        
        public GUIText_Wrapper guiText {
            get {
                return this._guitext;
            }
            set {
                this._guitext = value;
            }
        }
        
        public NetworkView_Wrapper networkView {
            get {
                return this._networkview;
            }
            set {
                this._networkview = value;
            }
        }
        
        public GUIElement_Wrapper guiElement {
            get {
                return this._guielement;
            }
            set {
                this._guielement = value;
            }
        }
        
        public GUITexture_Wrapper guiTexture {
            get {
                return this._guitexture;
            }
            set {
                this._guitexture = value;
            }
        }
        
        public Collider_Wrapper collider {
            get {
                return this._collider;
            }
            set {
                this._collider = value;
            }
        }
        
        public Collider2D_Wrapper collider2D {
            get {
                return this._collider2d;
            }
            set {
                this._collider2d = value;
            }
        }
        
        public HingeJoint_Wrapper hingeJoint {
            get {
                return this._hingejoint;
            }
            set {
                this._hingejoint = value;
            }
        }
        
        public ParticleEmitter_Wrapper particleEmitter {
            get {
                return this._particleemitter;
            }
            set {
                this._particleemitter = value;
            }
        }
        
        public ParticleSystem_Wrapper particleSystem {
            get {
                return this._particlesystem;
            }
            set {
                this._particlesystem = value;
            }
        }
        
        public GameObject_Wrapper gameObject {
            get {
                return this._gameobject;
            }
            set {
                this._gameobject = value;
            }
        }
        
        public bool active {
            get {
                return _active;
            }
            set {
                _active = value;
            }
        }
        
        public string tag {
            get {
                return _tag;
            }
            set {
                _tag = value;
            }
        }
        
        public string name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
        
        public UnityEngine.HideFlags hideFlags {
            get {
                return _hideflags;
            }
            set {
                _hideflags = value;
            }
        }
    }
}
