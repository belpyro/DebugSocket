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
    public class NavMeshAgent_Wrapper {
        
        private Vector3_Wrapper _destination;
        
        private float _stoppingdistance;
        
        private Vector3_Wrapper _velocity;
        
        private Vector3_Wrapper _nextposition;
        
        private Vector3_Wrapper _steeringtarget;
        
        private Vector3_Wrapper _desiredvelocity;
        
        private float _remainingdistance;
        
        private float _baseoffset;
        
        private bool _isonoffmeshlink;
        
        private OffMeshLinkData_Wrapper _currentoffmeshlinkdata;
        
        private OffMeshLinkData_Wrapper _nextoffmeshlinkdata;
        
        private bool _autotraverseoffmeshlink;
        
        private bool _autobraking;
        
        private bool _autorepath;
        
        private bool _haspath;
        
        private bool _pathpending;
        
        private bool _ispathstale;
        
        private UnityEngine.NavMeshPathStatus _pathstatus;
        
        private Vector3_Wrapper _pathendposition;
        
        private NavMeshPath_Wrapper _path;
        
        private int _walkablemask;
        
        private float _speed;
        
        private float _angularspeed;
        
        private float _acceleration;
        
        private bool _updateposition;
        
        private bool _updaterotation;
        
        private float _radius;
        
        private float _height;
        
        private UnityEngine.ObstacleAvoidanceType _obstacleavoidancetype;
        
        private int _avoidancepriority;
        
        private bool _enabled;
        
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
        
        public Vector3_Wrapper destination {
            get {
                return this._destination;
            }
            set {
                this._destination = value;
            }
        }
        
        public float stoppingDistance {
            get {
                return _stoppingdistance;
            }
            set {
                _stoppingdistance = value;
            }
        }
        
        public Vector3_Wrapper velocity {
            get {
                return this._velocity;
            }
            set {
                this._velocity = value;
            }
        }
        
        public Vector3_Wrapper nextPosition {
            get {
                return this._nextposition;
            }
            set {
                this._nextposition = value;
            }
        }
        
        public Vector3_Wrapper steeringTarget {
            get {
                return this._steeringtarget;
            }
            set {
                this._steeringtarget = value;
            }
        }
        
        public Vector3_Wrapper desiredVelocity {
            get {
                return this._desiredvelocity;
            }
            set {
                this._desiredvelocity = value;
            }
        }
        
        public float remainingDistance {
            get {
                return _remainingdistance;
            }
            set {
                _remainingdistance = value;
            }
        }
        
        public float baseOffset {
            get {
                return _baseoffset;
            }
            set {
                _baseoffset = value;
            }
        }
        
        public bool isOnOffMeshLink {
            get {
                return _isonoffmeshlink;
            }
            set {
                _isonoffmeshlink = value;
            }
        }
        
        public OffMeshLinkData_Wrapper currentOffMeshLinkData {
            get {
                return this._currentoffmeshlinkdata;
            }
            set {
                this._currentoffmeshlinkdata = value;
            }
        }
        
        public OffMeshLinkData_Wrapper nextOffMeshLinkData {
            get {
                return this._nextoffmeshlinkdata;
            }
            set {
                this._nextoffmeshlinkdata = value;
            }
        }
        
        public bool autoTraverseOffMeshLink {
            get {
                return _autotraverseoffmeshlink;
            }
            set {
                _autotraverseoffmeshlink = value;
            }
        }
        
        public bool autoBraking {
            get {
                return _autobraking;
            }
            set {
                _autobraking = value;
            }
        }
        
        public bool autoRepath {
            get {
                return _autorepath;
            }
            set {
                _autorepath = value;
            }
        }
        
        public bool hasPath {
            get {
                return _haspath;
            }
            set {
                _haspath = value;
            }
        }
        
        public bool pathPending {
            get {
                return _pathpending;
            }
            set {
                _pathpending = value;
            }
        }
        
        public bool isPathStale {
            get {
                return _ispathstale;
            }
            set {
                _ispathstale = value;
            }
        }
        
        public UnityEngine.NavMeshPathStatus pathStatus {
            get {
                return _pathstatus;
            }
            set {
                _pathstatus = value;
            }
        }
        
        public Vector3_Wrapper pathEndPosition {
            get {
                return this._pathendposition;
            }
            set {
                this._pathendposition = value;
            }
        }
        
        public NavMeshPath_Wrapper path {
            get {
                return this._path;
            }
            set {
                this._path = value;
            }
        }
        
        public int walkableMask {
            get {
                return _walkablemask;
            }
            set {
                _walkablemask = value;
            }
        }
        
        public float speed {
            get {
                return _speed;
            }
            set {
                _speed = value;
            }
        }
        
        public float angularSpeed {
            get {
                return _angularspeed;
            }
            set {
                _angularspeed = value;
            }
        }
        
        public float acceleration {
            get {
                return _acceleration;
            }
            set {
                _acceleration = value;
            }
        }
        
        public bool updatePosition {
            get {
                return _updateposition;
            }
            set {
                _updateposition = value;
            }
        }
        
        public bool updateRotation {
            get {
                return _updaterotation;
            }
            set {
                _updaterotation = value;
            }
        }
        
        public float radius {
            get {
                return _radius;
            }
            set {
                _radius = value;
            }
        }
        
        public float height {
            get {
                return _height;
            }
            set {
                _height = value;
            }
        }
        
        public UnityEngine.ObstacleAvoidanceType obstacleAvoidanceType {
            get {
                return _obstacleavoidancetype;
            }
            set {
                _obstacleavoidancetype = value;
            }
        }
        
        public int avoidancePriority {
            get {
                return _avoidancepriority;
            }
            set {
                _avoidancepriority = value;
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
