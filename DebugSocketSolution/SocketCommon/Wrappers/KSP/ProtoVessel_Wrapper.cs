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
    public class ProtoVessel_Wrapper {
        
        public System.Collections.Generic.List<ProtoPartSnapshot> protoPartSnapshots;
        
        public System.Collections.Generic.Dictionary<string, KSPParseable> vesselStateValues;
        
        public OrbitSnapshot_Wrapper orbitSnapShot;
        
        public System.Guid vesselID;
        
        public bool landed;
        
        public string landedAt;
        
        public bool splashed;
        
        public string vesselName;
        
        public int rootIndex;
        
        public int stage;
        
        public uint refTransform;
        
        public Vector3d_Wrapper position;
        
        public double altitude;
        
        public double latitude;
        
        public double longitude;
        
        public float height;
        
        public Vector3_Wrapper normal;
        
        public Quaternion_Wrapper rotation;
        
        public Vector3_Wrapper CoM;
        
        public Vessel_Wrapper vesselRef;
        
        public Vessel.Situations situation;
        
        public VesselType vesselType;
        
        public double missionTime;
        
        public double launchTime;
        
        public bool persistent;
        
        public ConfigNode actionGroups;
        
        public ConfigNode discoveryInfo;
        
        public ConfigNode flightPlan;
        
        public ConfigNode ctrlState;
        
        public ProtoTargetInfo_Wrapper targetInfo;
        
        public bool wasControllable;
    }
}
