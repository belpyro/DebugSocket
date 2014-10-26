using System;
using System.Collections.Generic;
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
using SocketCommon.Wrappers.Tree;

namespace DebugHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DebugViewModel model;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(ItemExpanded), true);
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
                    var data = model.GetKspValue(info) as MemberInfoWrapper;
                    FillItemValue(item, data);
                    break;
                case MemberType.Type:
                    var children = model.GetChildren(info);
                    FillItemByChildren(item, children);
                    break;
                case MemberType.Value:
                    break;
                case MemberType.Collection:
                    var items = model.GetKspCollection(info);
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
            model = new DebugViewModel();
            model.OnLoaded += model_OnLoaded;
            DataContext = model;
            model.LoadKspTypes();
        }

        void model_OnLoaded(object sender, EventArgs e)
        {
            if (model.LoadedTypes == null || !model.LoadedTypes.Any()) return;

            TypeTree.Items.Clear();

            foreach (var item in model.LoadedTypes.Select(wrapper => new TreeViewItem() { Header = wrapper.Name, Tag = wrapper }))
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
            TypeTree.Items.Clear();

            var result = model.GetExternalWrapper();
        }

        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = TypeTree.SelectedItem as TreeViewItem;

            if (item == null) return;

            var info = item.Tag as MemberInfoWrapper;

            if (info == null) return;

            info.Value = model.SelectedValue;

            model.SetValue(info);
        }
    }
}
