//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18444
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wrappers {
    
    
    [System.SerializableAttribute()]
    public class ControlSurface_Wrapper {
        
        public Vector3_Wrapper velocity;
        
        public float AoA;
        
        public float deflectionLiftCoeff;
        
        public float dragCoeff;
        
        public Vector3_Wrapper inputVector;
        
        public Vector3_Wrapper pivotAxis;
        
        public float ctrlSurfaceRange;
        
        public float ctrlSurfaceArea;
        
        public Vessel_Wrapper vessel;
        
        public System.Collections.Generic.List<Part> editorLinks;
        
        public System.Collections.Generic.List<Part> symmetryCounterparts;
        
        public bool isClone;
        
        public int stackSymmetry;
        
        public int symmetryMode;
        
        public Part_Wrapper potentialParent;
        
        public AttachModes attachMode;
        
        public ProtoPart_Wrapper protoPartRef;
        
        public ProtoPartSnapshot_Wrapper protoPartSnapshot;
        
        public AvailablePart partInfo;
        
        public uint uid;
        
        public uint flightID;
        
        public uint missionID;
        
        public uint launchID;
        
        public Part_Wrapper parent;
        
        public System.Collections.Generic.List<Part> fuelLookupTargets;
        
        public System.Collections.Generic.List<Part> children;
        
        public Transform_Wrapper partTransform;
        
        public PartStates ResumeState;
        
        public int stageOffset;
        
        public int childStageOffset;
        
        public int manualStageOffset;
        
        public int defaultInverseStage;
        
        public int inverseStage;
        
        public int inStageIndex;
        
        public int originalStage;
        
        public string stagingIcon;
        
        public VStackIcon_Wrapper stackIcon;
        
        public StackIconGrouping stackIconGrouping;
        
        public bool frozen;
        
        public bool isControlSource;
        
        public int PhysicsSignificance;
        
        public Part.PhysicalSignificance physicalSignificance;
        
        public bool isPersistent;
        
        public bool started;
        
        public string InternalModelName;
        
        public int CrewCapacity;
        
        public System.Collections.Generic.List<ProtoCrewMember> protoModuleCrew;
        
        public InternalModel_Wrapper internalModel;
        
        public Transform_Wrapper airlock;
        
        public FlightIntegrator_Wrapper flightIntegrator;
        
        public CollisionEnhancer_Wrapper collisionEnhancer;
        
        public PartBuoyancy_Wrapper partBuoyancy;
        
        public Vector3_Wrapper orgPos;
        
        public Quaternion_Wrapper orgRot;
        
        public Quaternion_Wrapper attRotation;
        
        public PartJoint_Wrapper attachJoint;
        
        public float breakingForce;
        
        public float breakingTorque;
        
        public float crashTolerance;
        
        public float mass;
        
        public Vector3_Wrapper CoMOffset;
        
        public Vector3_Wrapper CenterOfBuoyancy;
        
        public float buoyancy;
        
        public Rigidbody_Wrapper rb;
        
        public bool packed;
        
        public Part.DragModel dragModel;
        
        public float maximum_drag;
        
        public float minimum_drag;
        
        public Vector3_Wrapper dragReferenceVector;
        
        public Vector3_Wrapper surfaceAreas;
        
        public string dragModelType;
        
        public float angularDrag;
        
        public double staticPressureAtm;
        
        public double dynamicPressureAtm;
        
        public string customPartData;
        
        public System.Collections.Generic.List<FXGroup> fxGroups;
        
        public float temperature;
        
        public float maxTemp;
        
        public float heatDissipation;
        
        public float heatConductivity;
        
        public float explosionPotential;
        
        public bool ActivatesEvenIfDisconnected;
        
        public bool fuelCrossFeed;
        
        public Part_Wrapper editorCollision;
        
        public float scaleFactor;
        
        public float rescaleFactor;
        
        public Callback OnJustAboutToBeDestroyed;
        
        public Callback OnEditorAttach;
        
        public Callback OnEditorDetach;
        
        public Callback OnEditorDestroy;
        
        public bool stageAfter;
        
        public bool stageBefore;
        
        public bool hasHeiarchyModel;
        
        public string initialVesselName;
        
        public VesselType vesselType;
        
        public string flagURL;
        
        public System.Collections.Generic.List<UnityEngine.Collider> currentCollisions;
        
        public bool GroundContact;
        
        public bool WaterContact;
        
        public Vector3_Wrapper vel;
        
        public uint lastFuelRequestId;
        
        public string NoCrossFeedNodeKey;
        
        public AttachRules attachRules;
        
        public System.Collections.Generic.List<AttachNode> attachNodes;
        
        public AttachNode srfAttachNode;
        
        public AttachNodeMethod attachMethod;
        
        public AttachNode topNode;
        
        public string partName;
        
        public Vector3_Wrapper mirrorAxis;
        
        public Vector3_Wrapper mirrorRefAxis;
        
        public Vector3_Wrapper mirrorVector;
        
        public bool isMirrored;
        
        public Part.HighlightType highlightType;
        
        public Color_Wrapper highlightColor;
        
        public bool highlightRecurse;
        
        public string ConstructID;
        
        public Part_Wrapper localRoot;
        
        public PartStates State;
        
        public bool hasStagingIcon;
        
        public bool isConnected;
        
        public bool isAttached;
        
        public bool isControllable;
        
        public Rigidbody_Wrapper Rigidbody;
        
        public Orbit orbit;
        
        public bool Landed;
        
        public bool Splashed;
        
        public string ClassName;
        
        public int ClassID;
        
        public BaseEventList_Wrapper Events;
        
        public BaseFieldList Fields;
        
        public BaseActionList Actions;
        
        public PartModuleList_Wrapper Modules;
        
        public EffectList_Wrapper Effects;
        
        public PartResourceList Resources;
        
        public bool useGUILayout;
        
        public bool enabled;
        
        public Transform_Wrapper transform;
        
        public Rigidbody_Wrapper rigidbody;
        
        public Rigidbody2D_Wrapper rigidbody2D;
        
        public Camera_Wrapper camera;
        
        public Light_Wrapper light;
        
        public Animation_Wrapper animation;
        
        public ConstantForce_Wrapper constantForce;
        
        public Renderer_Wrapper renderer;
        
        public AudioSource_Wrapper audio;
        
        public GUIText_Wrapper guiText;
        
        public NetworkView_Wrapper networkView;
        
        public GUIElement_Wrapper guiElement;
        
        public GUITexture_Wrapper guiTexture;
        
        public Collider_Wrapper collider;
        
        public Collider2D_Wrapper collider2D;
        
        public HingeJoint_Wrapper hingeJoint;
        
        public ParticleEmitter_Wrapper particleEmitter;
        
        public ParticleSystem_Wrapper particleSystem;
        
        public GameObject_Wrapper gameObject;
        
        public bool active;
        
        public string tag;
        
        public string name;
        
        public UnityEngine.HideFlags hideFlags;
    }
}
