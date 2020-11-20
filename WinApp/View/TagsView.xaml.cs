using iFactoryApp.ViewModel;
using System.Windows;

namespace iFactoryApp.View
{
    /// <summary>
    /// SystemLogView.xaml 的交互逻辑
    /// </summary>
    public partial class TagsView : Window
    {
        public TagsView()
        {
            InitializeComponent();
            TagsViewModel viewModel = IoC.GetViewModel<TagsViewModel>(this);
            this.DataContext = viewModel;
        }
    }
}
