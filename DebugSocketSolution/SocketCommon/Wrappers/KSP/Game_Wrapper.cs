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
    public class Game_Wrapper {
        
        public string Title;
        
        public string Description;
        
        public string linkURL;
        
        public string linkCaption;
        
        public Game.Modes Mode;
        
        public Game.GameStatus Status;
        
        public GameScenes startScene;
        
        public FlightState_Wrapper flightState;
        
        public KerbalRoster_Wrapper CrewRoster;
        
        public System.Collections.Generic.List<ProtoScenarioModule> scenarios;
        
        public ConfigNode additionalSystems;
        
        public ConfigNode config;
        
        public bool compatible;
        
        public int file_version_major;
        
        public int file_version_minor;
        
        public int file_version_revision;
        
        public string flagURL;
        
        public uint launchID;
        
        public int lastCompatibleMajor;
        
        public int lastCompatibleMinor;
        
        public int lastCompatibleRev;
        
        public double UniversalTime;
    }
}