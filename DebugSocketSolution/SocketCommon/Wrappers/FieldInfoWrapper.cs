using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using SocketCommon.Annotations;

namespace SocketCommon.Wrappers
{
    [Serializable]
    public class FieldInfoWrapper:INotifyPropertyChanged
    {
        private object _value;
        private FieldInfo _data;

        public FieldInfo Data
        {
            get { return _data; }
            set
            {
                if (Equals(value, _data)) return;
                _data = value;
                OnPropertyChanged("Data");
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
