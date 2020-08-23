using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    /// <summary>
    /// 树形菜单节点数据
    /// </summary>
    public class TreeNodeData: BaseNotifyModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        private string _Name;
        public string Name
        {
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
            get { return _Name; }
        }
        /// <summary>
        /// 显示名称
        /// </summary>
        private string _DisplayName;
        public string DisplayName
        {
            set
            {
                _DisplayName = value;
                NotifyPropertyChanged("DisplayName");
            }
            get { return _DisplayName; }
        }
        /// <summary>
        /// 节点
        /// </summary>
        private string _Root;
        public string Root
        {
            set
            {
                _DisplayName = value;
                NotifyPropertyChanged("Root");
            }
            get { return _Root; }
        }
       
        /// <summary>
        /// 子节点数据
        /// </summary>
        public List<TreeNodeData> ChildNodes { get; set; }

        public TreeNodeData()
        {
            ChildNodes = new List<TreeNodeData>();
        }
    }
}
