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
            var result = DebugModel.Instance.ExecuteCommand(null, Commands.GetTypes);

            if (result == null || result.HasError) return;

            LoadedTypes = (List<MemberInfoWrapper>)result.Data;

            //if (OnLoaded != null) OnLoaded(this, EventArgs.Empty);
        }

        public object GetKspValue(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.ExecuteCommand(wrapper, Commands.GetValue);

            return result.HasError ? null : result.Data;
        }

        public IEnumerable<MemberInfoWrapper> GetKspCollection(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.ExecuteCommand(wrapper, Commands.GetCollection);

            return (IEnumerable<MemberInfoWrapper>) (result.HasError ? null : result.Data);
        }

        public IEnumerable<MemberInfoWrapper> GetChildren(MemberInfoWrapper wrapper)
        {
            DataResponce result = DebugModel.Instance.ExecuteCommand(wrapper, Commands.GetChildren);

            return (IEnumerable<MemberInfoWrapper>) (result.HasError ? null : result.Data);
        }
        public MemberInfoWrapper GetExternalWrapper()
        {
            DataResponce result = DebugModel.Instance.ExecuteCommand(null, Commands.GetExternal);

            return (MemberInfoWrapper) (result.HasError ? null : result.Data);
        }

        public IEnumerable<MethodInfoWrapper> GetMethods(MemberInfoWrapper wrapper)
        {
           DataResponce result = DebugModel.Instance.ExecuteCommand(wrapper, Commands.GetMethods);

           return (IEnumerable<MethodInfoWrapper>)(result.HasError ? null : result.Data);
        }

        public IEnumerable<MemberInfoWrapper> GetGameEvents()
        {
           DataResponce result = DebugModel.Instance.ExecuteCommand(null, Commands.GetGameEvents);

           return (IEnumerable<MemberInfoWrapper>)(result.HasError ? null : result.Data);
        }

        public void SetValue(MemberInfoWrapper wrapper)
        {
            DebugModel.Instance.ExecuteCommand(wrapper, Commands.SetValue);
        }

        public void AttachToEvent(MemberInfoWrapper wrapper)
        {
            DebugModel.Instance.ExecuteCommand(wrapper, Commands.EventAttach);
        }

        public void DettachToEvent(MemberInfoWrapper wrapper)
        {
            DebugModel.Instance.ExecuteCommand(wrapper, Commands.EventDetach);
        }
        
        public void CallMethod(MemberInfoWrapper wrapper)
        {
            DebugModel.Instance.ExecuteCommand(wrapper, Commands.CallMethod);
        }
        
        public void RefreshTypes()
        {
            DebugModel.Instance.ExecuteCommand(null, Commands.Refresh);
        }

        public IEnumerable<MemberInfoWrapper> LoadedTypes { get; private set; }

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

        public string SelectedValue { get; set; }
    }
}
