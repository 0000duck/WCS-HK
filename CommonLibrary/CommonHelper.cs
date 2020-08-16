using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iFactory.CommonLibrary
{
    public class CommonHelper
    {
        /// <summary>
        /// 显示保存文件对话框，并返回路径
        /// </summary>
        /// <returns></returns>
        public static string ShowSaveFileDailog(string FileFormat="")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //可能要获取的路径名
            string localFilePath = "", fileNameExt = "", FilePath = "";

            //设置文件类型
            //书写规则例如：txt files(*.txt)|*.txt
            if(FileFormat.Length>0)
            {
                saveFileDialog.Filter = string.Format("{0} files(*.{0})|*.{0}", FileFormat);
            }
            else
            {
                saveFileDialog.Filter = "xls files(*.xlsx)|*.xlsx";
            }
            
            //设置默认文件名（可以不设置）
            saveFileDialog.FileName = "散装发货记录"+DateTime.Now.ToString("yyyyMMdd");
            //主设置默认文件extension（可以不设置）
            saveFileDialog.DefaultExt = "xls";
            //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
            saveFileDialog.AddExtension = true;

            //保存对话框是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;

            // Show save file dialog box
            DialogResult result = saveFileDialog.ShowDialog();
            //点了保存按钮进入
            if (result == DialogResult.OK)
            {
                //获得文件路径
                localFilePath = saveFileDialog.FileName.ToString();

                //获取文件名，不带路径
                fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

                //获取文件路径，不带文件名
                FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));

            }
            return localFilePath;
        }
        /// <summary>
        /// 通过时间生成订单号码
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumberByTime()
        {
            string orderNumber = "";
            DateTime dtNow = DateTime.Now;
            orderNumber = $"{dtNow.ToString("yyMMddhhmmss")}";
            return orderNumber;
        }


    }
}
