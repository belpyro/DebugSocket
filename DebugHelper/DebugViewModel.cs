using System;
using System.Collections.Generic;
using SocketCommon.Wrappers.Tree;

namespace DebugHelper
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using DebugHelper.Annotations;
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

        public IEnumerable<MemberInfoWrapper> GetKspCollection(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.GetValue(wrapper, Commands.GetCollection);

            return (IEnumerable<MemberInfoWrapper>) (result.HasError ? null : result.Data);
        }

        public IEnumerable<MemberInfoWrapper> GetChildren(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.GetValue(wrapper, Commands.GetChildren);

            return (IEnumerable<MemberInfoWrapper>) (result.HasError ? null : result.Data);
        }
        public MemberInfoWrapper GetExternalWrapper()
        {
            DataResponce result = DebugModel.Instance.GetValue(null, Commands.GetExternal);

            return (MemberInfoWrapper) (result.HasError ? null : result.Data);
        }

        public void SetValue(MemberInfoWrapper wrapper)
        {
            DebugModel.Instance.GetValue(wrapper, Commands.SetValue);
        }

        public IEnumerable<MemberInfoWrapper> LoadedTypes { get; set; }

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

        public string SelectedValue { get; set; }
    }
}
