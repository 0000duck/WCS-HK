using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iFactory.CommonLibrary
{
    public static class MyFormExtensionMethods
    {
        /// <summary>
        /// Messagebox封装函数。根据关键字，自动判断窗体类型，并直前
        /// </summary>
        /// <param name="Content">显示内容</param>
        /// <param name="Caption">标题</param>
        /// <returns>返回选择按钮结果</returns>
        public static DialogResult MessageBoxShow(string Content, string Caption = "")
        {
            DialogResult ret = DialogResult.OK;

            if (Content.EndsWith("?") || Content.EndsWith("？") || Content.EndsWith("吗？"))
            {
                ret = MessageBox.Show(Content, Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            }
            else if (Content.Contains("?") || Content.Contains("？") || Content.Contains("吗？"))
            {
                ret = MessageBox.Show(Content, Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            }
            else if (Content.Contains("成功"))
            {
                ret = MessageBox.Show(Content, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else if (Caption.Contains("错误") || Caption.Contains("失败") || Content.Contains("失败") || Content.Contains("错误") || Content.Contains("不能"))
            {
                ret = MessageBox.Show(Content, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                ret = MessageBox.Show(Content, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            return ret;
        }

        /// <summary>
        /// 查找指定控件中的textbox控件
        /// </summary>
        /// <param name="txtboxName">控件名称</param>
        public static TextBox GetTxtControlsByName(System.Windows.Forms.Control CtlObjec, string txtboxName)
        {
            foreach (System.Windows.Forms.Control ControlItem in CtlObjec.Controls)
            {
                if (ControlItem is TextBox)
                {
                    if (ControlItem.Name.Equals(txtboxName))
                        return ControlItem as TextBox;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找指定控件中的Label控件
        /// </summary>
        /// <param name="LabelName">控件名称</param>
        public static Label GetLblControlsByName(System.Windows.Forms.Control CtlObjec, string LabelName)
        {
            foreach (System.Windows.Forms.Control ControlItem in CtlObjec.Controls)
            {
                if (ControlItem is Label)
                {
                    if (ControlItem.Name.Equals(LabelName))
                        return ControlItem as Label;
                }
            }
            return null;
        }
        /// <summary>
        /// 显示虚拟键盘
        /// </summary>
        public static void ShowVirtualKeyBoard()
        {
            try
            {
                string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";
                string keyboardPath = Path.Combine(progFiles, "TabTip.exe");

                Process.Start(keyboardPath);
            }
            catch (Exception)
            { }
        }
        /// <summary>
        /// 移除tableLayoutpannel的行
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="row_index_to_remove"></param>
        public static void remove_row(TableLayoutPanel panel, int row_index_to_remove)
        {
            try
            {
                if (row_index_to_remove >= panel.RowCount)
                {
                    return;
                }

                // delete all controls of row that we want to delete
                for (int i = 0; i < panel.ColumnCount; i++)
                {
                    var control = panel.GetControlFromPosition(i, row_index_to_remove);
                    panel.Controls.Remove(control);
                }

                // move up row controls that comes after row we want to remove
                for (int i = row_index_to_remove + 1; i < panel.RowCount; i++)
                {
                    for (int j = 0; j < panel.ColumnCount; j++)
                    {
                        var control = panel.GetControlFromPosition(j, i);
                        if (control != null)
                        {
                            panel.SetRow(control, i - 1);
                        }
                    }
                }

                // remove last row
                panel.RowStyles.RemoveAt(panel.RowCount - 1);
                panel.RowCount--;
                //Invalidate();
            }
            catch (Exception ex) { }
        }
    }
}
