using SqlSugar;
using System.ComponentModel;

namespace iFactory.DataService.Model
{
    /// <summary>
    /// 基类
    /// </summary>
    public class BaseNotifyModel : INotifyPropertyChanged
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
