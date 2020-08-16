using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// combox下拉控件绑定数据类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CmbBindingProperty<T>
    {
        public string DisplayMember { set; get; }
        public T SelectedValue { set; get; }
    }
}
