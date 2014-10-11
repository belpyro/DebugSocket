using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketCommon.Wrappers;

namespace DebugHelper
{
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using DebugHelper.Annotations;
    using GalaSoft.MvvmLight.Command;

    using SocketCommon;

    public class DebugViewModel : INotifyPropertyChanged
    {
        public DebugViewModel()
        {
            SelectionChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(SelectionChangedExecute);
            DoubleClickCommand = new RelayCommand<object>(DoubleClickExecute);

            var result = DebugModel.Instance.GetAll();

            if (result != null && !result.HasError)
            {
                LoadedTypes = (IEnumerable<AssemblyWrapper>)result.Data;
            }
        }

        private void DoubleClickExecute(object obj)
        {
            if (obj == null) return;

            var item = obj as FieldInfoWrapper;

            var result = DebugModel.Instance.GetValue(ReturnedType.Name, item.Data.Name, Commands.GetField);

            if (!result.HasError)
            {
                item.Value = result.Data;
                this.OnPropertyChanged("ReturnedType");
            }
        }

        private void SelectionChangedExecute(RoutedPropertyChangedEventArgs<object> obj)
        {
            if (obj.NewValue == null) return;

            if (obj.NewValue is AssemblyWrapper)
            {
                SelectedType = obj.NewValue;
                return;
            }

            var result = DebugModel.Instance.Get(obj.NewValue.ToString());
            if (result == null || result.HasError) { return; }
            this.ReturnedType = (TypeWrapper)result.Data;

            this.OnPropertyChanged("ReturnedType");
        }

        public IEnumerable<AssemblyWrapper> LoadedTypes { get; set; }

        public object SelectedType { get; set; }

        public TypeWrapper ReturnedType { get; set; }

        public ICommand DoubleClickCommand { get; private set; }
        
        public ICommand SelectionChangedCommand { get; private set; }

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
