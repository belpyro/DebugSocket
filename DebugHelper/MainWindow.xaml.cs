using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SocketCommon;
using SocketCommon.Wrappers.Tree;
using Xceed.Wpf.Toolkit;

namespace DebugHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DebugViewModel _model;

        private LogServer.LogServer _server = new LogServer.LogServer();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(ItemExpanded), true);
            AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(ItemClicked), true);
        }

        private void ItemClicked(object sender, RoutedEventArgs e)
        {
            cw.DataContext = (TypeTree.SelectedItem as TreeViewItem).Tag;
            cw.Show();
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _server.OnRecieved -= _server_OnRecieved;
            _server.Stop();
        }

        private void ItemExpanded(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;

            if (item == null) return;

            var info = item.Tag as MemberInfoWrapper;

            if (info == null) return;

            switch (info.ItemType)
            {
                case MemberType.Field:
                case MemberType.Property:
                    var data = _model.GetKspValue(info) as MemberInfoWrapper;
                    FillItemValue(item, data);
                    break;
                case MemberType.Type:
                    var children = _model.GetChildren(info);
                    FillItemByChildren(item, children);
                    break;
                case MemberType.Value:
                    break;
                case MemberType.Collection:
                    var items = _model.GetKspCollection(info);
                    FillItemByChildren(item, items);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void FillItemValue(TreeViewItem item, MemberInfoWrapper wrapper)
        {
            item.Items.Clear();

            var child = new TreeViewItem() { Header = wrapper == null ? "no value or no instantiate" : wrapper.Value, Tag = wrapper };

            item.Items.Add(child);
        }

        private void FillItemByChildren(TreeViewItem item, IEnumerable<MemberInfoWrapper> data)
        {
            item.Items.Clear();

            if (data == null || !data.Any())
            {
                item.Header = string.Format("Member {0} not exist", item.Header);
                return;
            }

            foreach (var wrapper in data)
            {
                wrapper.Parent = item.Tag as MemberInfoWrapper;

                var dataItem = new TreeViewItem() { Header = string.Format("{0} ({1})", wrapper.Name, wrapper.TypeName), Tag = wrapper };

                switch (wrapper.ItemType)
                {
                    case MemberType.Field:
                        dataItem.Foreground = new SolidColorBrush(Colors.CornflowerBlue);
                        break;
                    case MemberType.Property:
                        dataItem.Foreground = new SolidColorBrush(Colors.DarkGreen);
                        break;
                    case MemberType.Type:
                        dataItem.Foreground = new SolidColorBrush(Colors.Firebrick);
                        break;
                    case MemberType.Value:
                        break;
                    case MemberType.Collection:
                        dataItem.Foreground = new SolidColorBrush(Colors.DarkOrange);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (wrapper.IsStatic)
                {
                    dataItem.FontWeight = FontWeights.SemiBold;
                    dataItem.FontStyle = FontStyles.Italic;
                }

                dataItem.Items.Add(new TreeViewItem() { Header = "Loading..." });

                item.Items.Add(dataItem);

            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _model = new DebugViewModel();
            DataContext = _model;
            _server.OnRecieved += _server_OnRecieved;
            _server.Start();
        }

        void _server_OnRecieved(object sender, LogServer.RequestEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                LogBlock.Text += Environment.NewLine + e.Request.Data;
                MyScrollViewer.ScrollToBottom();
            });
        }

        void TypesLoaded()
        {
            if (_model.LoadedTypes == null || !_model.LoadedTypes.Any()) return;

            TypeTree.Items.Clear();

            foreach (var item in _model.LoadedTypes.Select(wrapper => new TreeViewItem() { Header = wrapper.Name, Tag = wrapper }))
            {
                item.Items.Add(new TreeViewItem() { Header = "Loading..." });

                TypeTree.Items.Add(item);
            }
        }

        private void CompleteBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            var text = CompleteBox.Text;

            foreach (TreeViewItem item in TypeTree.Items)
            {
                item.Visibility = item.Header.ToString().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //var cw = new ChildWindow
            //{
            //    WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center,
            //    IsModal = true,
            //    Content = new TextBlock() {Text = "asasasas"}
            //};

            //cw.SetValue(Grid.RowSpanProperty, 4);
            //cw.SetValue(Grid.ColumnSpanProperty, 3);

            //Root.Children.Add(cw);
            //Indicator.IsBusy = true;
            _model.LoadKspTypes();
            TypesLoaded();
            //Indicator.IsBusy = false;
        }

        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = TypeTree.SelectedItem as TreeViewItem;

            if (item == null) return;

            var info = item.Tag as MemberInfoWrapper;

            if (info == null) return;

            info.Value = _model.SelectedValue;

            _model.SetValue(info);
        }

        private void MethodsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = TypeTree.SelectedItem as TreeViewItem;

            if (item == null) return;

            var info = item.Tag as MemberInfoWrapper;

            if (info == null) return;

            MethodItem.DataContext = _model.GetMethods(info);
        }

        private void EventsButton_OnClick(object sender, RoutedEventArgs e)
        {
            EventItem.DataContext = _model.GetGameEvents();
        }

        private void AttachEvent_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn == null) return;

            if (btn.Tag == null)
            {
                return;
            }

            var tag = (Commands)btn.Tag;
            var item = btn.DataContext;
            switch (tag)
            {
                case Commands.EventAttach:
                    _model.AttachToEvent((MemberInfoWrapper)item);
                    btn.Content = "Dettach";
                    btn.Tag = Commands.EventDetach;
                    break;
                case Commands.EventDetach:
                    _model.DettachToEvent((MemberInfoWrapper)item);
                    btn.Content = "Attach";
                    btn.Tag = Commands.EventAttach;
                    break;
            }
        }

        private void ButtonSetValue_Click(object sender, RoutedEventArgs e)
        {
            var item = TypeTree.SelectedItem as TreeViewItem;

            if (item == null) return;

            var info = item.Tag as MemberInfoWrapper;

            if (info == null) return;

            _model.SetValue(info); 
        }
    }
}
