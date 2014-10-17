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
            AddHandler(TreeViewItem.ExpandedEvent,new RoutedEventHandler(ItemExpanded), true);
        }

        private void ItemExpanded(object sender, RoutedEventArgs e)
        {

            var item = e.OriginalSource as TreeViewItem;

            if (item == null) return;

            var info = item.DataContext as MemberInfoWrapper;

            if (info == null || info.Name.Equals(info.TypeName)) return;

            var data = model.GetKspValue(info);
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
            TypeTree.ItemsSource = model.LoadedTypes;
        }
    }
}
