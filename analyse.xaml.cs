using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace 作业调度C_sharp
{
    /// <summary>
    /// analyse.xaml 的交互逻辑
    /// </summary>
    public partial class analyse : Window
    {
        public analyse()
        {
            InitializeComponent();
            DataGrid table = this.Table as DataGrid;
            table.ItemsSource = MainWindow.ITEM;
        }
    }
}
