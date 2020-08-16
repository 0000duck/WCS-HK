using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iFactory.CommonLibrary
{
    public static class WpfViewExtension
    {
        /// <summary>
        /// 导航到页面
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Frame NavigateToPage(this Frame frame,Page page,bool RemoveBackEntry=true)
        {
            try
            {
                if(RemoveBackEntry)
                {
                    frame.RemoveBackEntry();
                }
            }
            catch (Exception ex)
            { }
            frame.Navigate(page);
            return frame;
        }
    }
}
