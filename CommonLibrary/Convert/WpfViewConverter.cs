using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// 适用于状态装换。=true为绿色，False为红色
    /// </summary>
    public class StatusColorConverter : IValueConverter
    {
        /// <summary>
        /// 当为false时，转换为红色，true为绿色  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool keyValue = System.Convert.ToBoolean(value);

            if (keyValue == false)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else
            {
                return new SolidColorBrush(Colors.LimeGreen);
            }
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 获取用户类别转换器
    /// </summary>
    public class OrderStatusConverter : IValueConverter
    {
        private static List<EnumStruct<int>> OrderStatusList = EnumHelper.ConvertEnumToList<int>(typeof(OrderStatusEnum));
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keyValue = System.Convert.ToInt32(value);
            var item = OrderStatusList.FirstOrDefault(x => x.Value == keyValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();
            var item = OrderStatusList.FirstOrDefault(x => x.Description == des);
            if (item != null)
            {
                return item.Value;
            }
            return 0;
        }
    }
    /// <summary>
    /// 时间转换器。默认时间显示无
    /// </summary>
    public class DatetTimeConverter : IValueConverter
    {
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = DateTime.MinValue;
            DateTime.TryParse(value.ToString(), out dt);

            if (dt <= DateTime.Parse("2001/01/01"))//以2001年为判断条件
            {
                return "无";
            }
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();

            if (des == "无")
            {
                return DateTime.MinValue;
            }
            return value;
        }
    }

    /// <summary>
    /// 开箱设备转换器
    /// </summary>
    public class OpenMachineModeConverter : IValueConverter
    {
        private static List<EnumStruct<int>> DeviceTypeList = EnumHelper.ConvertEnumToList<int>(typeof(OpenMachineMode));
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keyValue = System.Convert.ToInt32(value);
            var item = DeviceTypeList.FirstOrDefault(x => x.Value == keyValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();
            var item = DeviceTypeList.FirstOrDefault(x => x.Description == des);
            if (item != null)
            {
                return item.Value;
            }
            return 0;
        }
    }
    /// <summary>
    /// 转换器
    /// </summary>
    public class BarcodeMachineModeConverter : IValueConverter
    {
        private static List<EnumStruct<int>> typeList = EnumHelper.ConvertEnumToList<int>(typeof(BarcodeMachineMode));
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keyValue = System.Convert.ToInt32(value);
            var item = typeList.FirstOrDefault(x => x.Value == keyValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();
            var item = typeList.FirstOrDefault(x => x.Description == des);
            if (item != null)
            {
                return item.Value;
            }
            return 0;
        }
    }
    
    /// <summary>
    /// 贴标设备模式转换器
    /// </summary>
    public class PackModeConverter : IValueConverter
    {
        private static List<EnumStruct<int>> typeList = EnumHelper.ConvertEnumToList<int>(typeof(PackMode));
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keyValue = System.Convert.ToInt32(value);
            var item = typeList.FirstOrDefault(x => x.Value == keyValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();
            var item = typeList.FirstOrDefault(x => x.Description == des);
            if (item != null)
            {
                return item.Value;
            }
            return 0;
        }
    }
    /// <summary>
    /// 启用模式转换器
    /// </summary>
    public class EnableModeConverter : IValueConverter
    {
        private static List<EnumStruct<int>> typeList = EnumHelper.ConvertEnumToList<int>(typeof(EnableMode));
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool keyValue = System.Convert.ToBoolean(value);
            int intValue = keyValue==true ? 1:0;
            var item = typeList.FirstOrDefault(x => x.Value == intValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
