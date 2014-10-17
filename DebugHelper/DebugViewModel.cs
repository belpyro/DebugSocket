using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketCommon.Wrappers;
using SocketCommon.Wrappers.Tree;

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
        }

        public void LoadKspTypes()
        {
            var result = DebugModel.Instance.GetAll();

            if (result != null && !result.HasError)
            {
                LoadedTypes = (IEnumerable<TypeInfoWrapper>)result.Data;
                if (OnLoaded != null) OnLoaded(this, EventArgs.Empty);
            }
        }

        public object GetKspValue(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.GetValue(wrapper.TypeName, wrapper.Name,
                wrapper.Type == MemberType.Field ? Commands.GetField : Commands.GetProperty);

            return result.HasError ? null : result.Data;
        }

        private void DoubleClickExecute(object obj)
        {
            if (obj == null) return;

            DataResponce result;

            if (obj is FieldInfoWrapper)
            {
                var item = obj as FieldInfoWrapper;
                result = DebugModel.Instance.GetValue(ReturnedType.Name, item.Data.Name, Commands.GetField);

                if (!result.HasError)
                {
                    item.Value = result.Data;
                }
            }
            else
            {
                var item = obj as PropertyInfoWrapper;
                result = DebugModel.Instance.GetValue(ReturnedType.Name, item.Data.Name, Commands.GetProperty);

                if (!result.HasError)
                {
                    item.Value = result.Data;
                }
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

            var result = DebugModel.Instance.Get("FlightGlobals");//obj.NewValue.ToString());
            if (result == null || result.HasError) { return; }
            this.ReturnedType = (TypeWrapper)result.Data;

            this.OnPropertyChanged("ReturnedType");
        }

        public IEnumerable<TypeInfoWrapper> LoadedTypes { get; set; }

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

        public event EventHandler OnLoaded;
    }
}
