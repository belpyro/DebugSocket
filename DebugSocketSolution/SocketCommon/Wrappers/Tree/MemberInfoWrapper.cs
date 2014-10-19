﻿using System;
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
        private MemberType _type;

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

        public MemberType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public string ParentType { get; set; }

        public bool IsStatic { get; set; }

        [field :NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}