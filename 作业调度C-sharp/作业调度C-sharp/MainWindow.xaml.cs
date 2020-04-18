using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace 作业调度C_sharp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //转换器
        public class SimpleConverter : IMultiValueConverter
        {
            object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                bool first = (bool)values[0];
                //bool second = (bool)values[1];
                if (first) return true;
                else return false;
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        //初始数据显示容器
        readonly ObservableCollection<JCB> items = new ObservableCollection<JCB>() {
            new JCB{ Name = "A", Atime = 1, Super = 1, Ntime = 4},
            new JCB{ Name = "B", Atime = 3, Super = 2, Ntime = 3},
            new JCB{ Name = "C", Atime = 2, Super = 3, Ntime = 3},
            new JCB{ Name = "D", Atime = 4, Super = 3, Ntime = 2},
            //new JCB{ Name = "E", Atime = 4, Super = 3, Ntime = 4},
        };
        //真正处理容器
        public static List<JCB> ITEM = new List<JCB>();
        //真正初始数据
        public static List<JCB> real = new List<JCB>();
        public MainWindow()
        {
            //窗体居中
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SetBinding();
            DataGrid table = this.Table as DataGrid;
            table.ItemsSource = items;
        }
        //绑定优先权框
        public void SetBinding()
        {
            //准备基础绑定
            Binding b1 = new Binding("IsChecked") { Source = this.psa };
            //Binding b2 = new Binding("IsChecked") { Source = this.dymatic_super };

            //准备MultiBinding
            MultiBinding mb = new MultiBinding() { Mode = BindingMode.OneWay };
            mb.Bindings.Add(b1);
            //mb.Bindings.Add(b2);
            mb.Converter = new SimpleConverter();
            //绑定
            this.super.SetBinding(TextBox.IsEnabledProperty, mb);
        }
        //重置按钮
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            items.Clear();
            ITEM.Clear();
            DataGrid table = this.Table as DataGrid;
            table.ItemsSource = items;
            this.super.Text = "";
            this.ntime.Text = "";
            this.atime.Text = "";
            this.pname.Text = "";
            JCB.Time = 0;
        }
        //添加按钮
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string name = pname.Text;
            string super = this.super.Text;
            string ntime = this.ntime.Text;
            string atime = this.atime.Text;
            Regex reg = new Regex(@"^\d+\.\d+$");
            if (reg.IsMatch(ntime) || reg.IsMatch(atime))
            {
                MessageBox.Show("输入的是小数");
                return;
            }
            //FCFS算法
            if (this.fcfs.IsChecked == true)
            {
                if (name == "" || ntime == "" || atime == "")
                    MessageBox.Show("请不要留空");
                else Get_JCB(name, ntime, atime);
            }


            //高响应比
            if (this.hrrn.IsChecked == true)
            {
                if (name == "" || ntime == "" || atime == "")
                    MessageBox.Show("请不要留空");
                else
                    Get_JCB(name, ntime, atime);
            }


            //静态优先级
            if (this.psa.IsChecked == true)
            {
                if (name == "" || ntime == "" || atime == "" || super == "")
                    MessageBox.Show("请不要留空");
                else Get_JCB(name, super, ntime, atime);
            }

            //短进程
            if (this.short_first.IsChecked == true)
            {
                if (name == "" || ntime == "" || atime == "" || super == "")
                    MessageBox.Show("请不要留空");
                else Get_JCB(name, super, ntime, atime);
            }

            DataGrid table = this.Table as DataGrid;
            table.ItemsSource = items;
            this.super.Text = "";
            this.ntime.Text = "";
            this.atime.Text = "";
            this.pname.Text = "";
        }
        //退出按钮
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        //开始按钮
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            String number = this.number.Text;
            JCB.Number = int.Parse(number);
            if(JCB.Number <= 0)
            {
                MessageBox.Show("个数不准为0");
                JCB.Number = 1;
                return;
            }
            real.Clear();
            for (int i = 0; i < items.Count(); i++)
            {
                JCB new_jcb = items[i].Clone();
                real.Add(new_jcb);
            }
            int nn = real.Count();
            ITEM.Clear();
            for (int i = 0; i < nn; i++) ITEM.Add(real[i]);
            int n = ITEM.Count();
            for (int i = 0; i < n; i++)
            {
                ITEM[i].Statu = Status.Wait;
            }
            if (fcfs.IsChecked == true) JCB.FCFS(ITEM);
            else if (short_first.IsChecked == true) JCB.Short_priority(ITEM);
            else if (psa.IsChecked == true) JCB.Static_priority(ITEM);
            else if (hrrn.IsChecked == true) JCB.Hrrn(ITEM);
            DataGrid table = this.Table as DataGrid;
            table.ItemsSource = null;
            table.ItemsSource = ITEM;
        }
        //分析按钮
        private void Analysis_Click(object sender, RoutedEventArgs e)
        {
            JCB.Analyse(ITEM);
            analyse Awin = new analyse();
            String T = "";
            //Awin.InitializeComponent();
            //FCFS算法
            if (this.fcfs.IsChecked == true) T = "FCFS";

            //高响应比优先调度算法
            if (this.hrrn.IsChecked == true) T = "高响应比优先调度算法";

            //静态优先级
            if (this.psa.IsChecked == true) T = "静态优先级";

            //短进程
            if (this.short_first.IsChecked == true) T = "短作业";

            Awin.Title = T + Awin.Title;
            Awin.Show();
        }
        //初始数据体按钮
        private void Input_Click(object sender, RoutedEventArgs e)
        {
            DataGrid table = this.Table as DataGrid;
            table.ItemsSource = items;
        }
        //无优先级
        private void Get_JCB(string name, string ntime, string atime)
        {
            JCB new_JCB = new JCB
            {
                Name = name,
                Ntime = Convert.ToDouble(ntime),
                Atime = Convert.ToDouble(atime)
            };
            items.Add(new_JCB);
            //real.Add(new_JCB);
            return;
        }
        //有优先级
        private void Get_JCB(string name, string super, string ntime, string atime)
        {
            JCB new_JCB = new JCB
            {
                Name = name,
                Super = Convert.ToInt16(super),
                Ntime = Convert.ToDouble(ntime),
                Atime = Convert.ToDouble(atime)
            };
            items.Add(new_JCB);
            //real.Add(new_JCB);
            return;
        }
    }
}
