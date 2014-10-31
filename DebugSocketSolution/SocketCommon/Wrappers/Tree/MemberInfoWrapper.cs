using System;
using System.Collections.Generic;
using System.ComponentModel;
using SocketCommon.Annotations;

namespace SocketCommon.Wrappers.Tree
{
    [Serializable]
    public sealed class MemberInfoWrapper: INotifyPropertyChanged
    {
        private string _name;
        private string _typeName;
        private object _value;
        private MemberType _itemType;

        /// <summary>
        /// Name of member
        /// </summary>
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

        /// <summary>
        /// Name of member type
        /// </summary>
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

        /// <summary>
        /// Current value. Can be set for changing.
        /// </summary>
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

        /// <summary>
        /// Type for current member (property, field, collection, etc)
        /// </summary>
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

        /// <summary>
        /// Current member is static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Parent for current member. This is using for graph creation.
        /// </summary>
        public MemberInfoWrapper Parent { get; set; }

        /// <summary>
        /// Index for element from collection.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Name of method for calling on a server side.
        /// </summary>
        public string MethodName { get; set; }

        #region PropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
 
        #endregion
    }
}