using GalaSoft.MvvmLight;
using iFactory.DevComServer;
using System.Collections.ObjectModel;

namespace iFactoryApp.ViewModel
{

    public class TagsViewModel : ViewModelBase
    {
        /// <summary>
        /// 标签队列，需要用set;get
        /// </summary>
        public ObservableCollection<ITag> TagsCollection { set; get; } = new ObservableCollection<ITag>();

        public TagsViewModel()
        {
            LoadAllModels();
        }
        /// <summary>
        /// 加载所有标签
        /// </summary>
        public void LoadAllModels()
        {
            TagsCollection.Clear();
            if (TagList.PLCGroups !=null)
            {
                foreach(var item in TagList.PLCGroups)
                {
                    foreach(var group in item.PlcDevice.TagGroups)
                    {
                        switch(group.DataType)
                        {
                            case TagDataType.Bool:
                                TagGroup<bool> boolGroup = group as TagGroup<bool>;
                                if(boolGroup.Tags !=null)
                                {
                                    TagsCollection.AddRange(boolGroup.Tags);
                                }
                                break;
                            case TagDataType.Short:
                                TagGroup<short> shortGroup = group as TagGroup<short>;
                                if (shortGroup.Tags != null)
                                {
                                    TagsCollection.AddRange(shortGroup.Tags);
                                }
                                break;
                            case TagDataType.Int:
                                TagGroup<int> intGroup = group as TagGroup<int>;
                                if (intGroup.Tags != null)
                                {
                                    TagsCollection.AddRange(intGroup.Tags);
                                }
                                break;
                            case TagDataType.Float:
                                TagGroup<float> floatGroup = group as TagGroup<float>;
                                if (floatGroup.Tags != null)
                                {
                                    TagsCollection.AddRange(floatGroup.Tags);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
