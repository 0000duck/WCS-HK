using iFactory.DevComServer;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iFactoryApp
{
    /// <summary>
    /// StatusControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatusControl : UserControl
    {
        private Tag<short> tag;

        public StatusControl()
        {
            InitializeComponent();
            TagList.GetTag(TagName, out tag, PlcName);
            if(tag !=null)
            {
                tag.PropertyChanged += Tag_PropertyChanged;
            }
        }

        private void Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (tag.TagValue == 0)
                {
                    this.status.Fill = new SolidColorBrush(Colors.LightGray);
                }
                else if (tag.TagValue == 1)
                {
                    this.status.Fill = new SolidColorBrush(Colors.LimeGreen);
                }
                else if (tag.TagValue == 2)
                {
                    this.status.Fill = new SolidColorBrush(Colors.Red);
                }
            }));
        }
        public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register(
           "TagName", typeof(String), typeof(StatusControl), new PropertyMetadata(default(string)));

        public string TagName
        {
            get => (string)GetValue(TagNameProperty);
            set => SetValue(TagNameProperty, value);
        }

        public static readonly DependencyProperty PlcNameProperty = DependencyProperty.Register(
          "PlcName", typeof(string), typeof(StatusControl), new PropertyMetadata(default(string)));

        public string PlcName
        {
            get => (string)GetValue(PlcNameProperty);
            set => SetValue(PlcNameProperty, value);
        }

        //private string _TagName;
        //public string TagName 
        //{ set=> _TagName=value; get=> _TagName; 
        //}
        //private string _PlcName;
        //public string PlcName 
        //{ set => _PlcName = value; get=> _PlcName; 
        //}
    }
}
