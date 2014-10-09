using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugHelper
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using DebugHelper.Annotations;

    using GalaSoft.MvvmLight.Command;

    using SocketCommon;

    public class DebugViewModel :INotifyPropertyChanged
    {
        public DebugViewModel()
        {
            GetTypeCommand = new RelayCommand(GetTypeCommandExecute);

            DataResponce result = DebugModel.Instance.GetAll();
            if (result != null)
            {
                LoadedTypes = ((IEnumerable<string>)result.Data).OrderBy(x => x);
            }
        }

        private void GetTypeCommandExecute()
        {
            var result = DebugModel.Instance.Get(SelectedType);
            if (result != null && !result.HasError)
            {
                ReturnedType = result.Data;
                this.OnPropertyChanged("ReturnedType");
            }
        }

        public IEnumerable<object> LoadedTypes { get; set; }

        public string SelectedType { get; set; }
        
        public object ReturnedType { get; set; }

        public ICommand GetTypeCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
