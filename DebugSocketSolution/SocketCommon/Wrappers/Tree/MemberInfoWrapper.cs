using System;
using System.Collections.Generic;
using System.ComponentModel;
using SocketCommon.Annotations;

namespace SocketCommon.Wrappers.Tree
{
    [Serializable]
    public class MemberInfoWrapper: INotifyPropertyChanged
    {
        private string _name;
        private string _typeName;
        private object _value;
        private MemberType _itemType;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (value == _typeName) return;
                _typeName = value;
                OnPropertyChanged("TypeName");
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public MemberType ItemType
        {
            get { return _itemType; }
            set
            {
                if (value == _itemType) return;
                _itemType = value;
                OnPropertyChanged("ItemType");
            }
        }

        public bool IsStatic { get; set; }

        public MemberInfoWrapper Parent { get; set; }

        public int Index { get; set; }

        #region PropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
 
        #endregion
    }
}