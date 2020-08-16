using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iFactoryApp.View
{
    /// <summary>
    /// 具有右键操的窗体
    /// </summary>
    interface IContextMenuView
    {
        /// <summary>
        /// 右键菜单操作，传入的click_name分为New Edit Delete
        /// </summary>
        void ContextMenu_Click(string click_name);
    }
}
