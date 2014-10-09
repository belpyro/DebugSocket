using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketCommon.Wrappers;

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
                LoadedTypes = (IEnumerable<AssemblyWrapper>) result.Data;
            }
        }

        private void GetTypeCommandExecute()
        {
            var result = DebugModel.Instance.Get(SelectedType);
            if (result != null && !result.HasError)
            {
                ReturnedType = new TypeWrapper
                                   {
                                       Fields = (result.Data as Type).GetFields(),
                                       Properties = (result.Data as Type).GetProperties(),
                                       Name = (result.Data as Type).FullName
                                   };

                this.OnPropertyChanged("ReturnedType");
            }
        }

        public IEnumerable<AssemblyWrapper> LoadedTypes { get; set; }

        public string SelectedType { get; set; }
        
        public TypeWrapper ReturnedType { get; set; }

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
