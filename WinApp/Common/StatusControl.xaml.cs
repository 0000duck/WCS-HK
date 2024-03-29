﻿using iFactory.DevComServer;
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
        public static readonly DependencyProperty PlcTagNameProperty = DependencyProperty.Register(
           "PlcTagName", typeof(String), typeof(StatusControl), new PropertyMetadata(default(string)));

        public string PlcTagName
        {
            get => (string)GetValue(PlcTagNameProperty);
            set => SetValue(PlcTagNameProperty, value);
        }

        //public static readonly DependencyProperty PlcNameProperty = DependencyProperty.Register(
        //  "PlcName", typeof(string), typeof(StatusControl), new PropertyMetadata(default(string)));

        //public string PlcName
        //{
        //    get => (string)GetValue(PlcNameProperty);
        //    set => SetValue(PlcNameProperty, value);
        //}

        private string _PlcName;
        /// <summary>
        /// 标签的设备名称,必须先赋值,否则使用会有问题
        /// </summary>
        public string PlcName
        {
            set => _PlcName = value; 
            get => _PlcName;
        }
        private string _TagName;
        /// <summary>
        /// 标签名称，必须后赋值
        /// </summary>
        public string TagName
        {
            set 
            { 
                _TagName = value;
                if (tag != null && tag.TagName== _TagName)
                {
                    tag.PropertyChanged -= Tag_PropertyChanged;
                }
                tag = null;
                TagList.GetTag(_TagName, out tag, _PlcName);
                if (tag != null)
                {
                    tag.PropertyChanged += Tag_PropertyChanged;
                }
            }
            get => _TagName;
        }
    }
}
