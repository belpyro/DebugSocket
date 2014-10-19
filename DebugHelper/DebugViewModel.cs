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

        public void LoadKspTypes()
        {
            var result = DebugModel.Instance.GetValue(null, Commands.GetTypes);

            if (result == null || result.HasError) return;

            LoadedTypes = (List<MemberInfoWrapper>)result.Data;

            if (OnLoaded != null) OnLoaded(this, EventArgs.Empty);
        }

        public object GetKspValue(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.GetValue(wrapper, Commands.GetValue);

            return result.HasError ? null : result.Data;
        }

        public IEnumerable<MemberInfoWrapper> GetChildren(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.GetValue(wrapper, Commands.GetChildren);

            return (IEnumerable<MemberInfoWrapper>) (result.HasError ? null : result.Data);
        }

        public IEnumerable<MemberInfoWrapper> LoadedTypes { get; set; }

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
