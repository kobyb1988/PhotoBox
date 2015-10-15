using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ImageMaker.CommonViewModels.ViewModels;

namespace SandBox.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        private ObservableCollection<TestItem> _items;
        private ICollectionView _itemsView;

        public TestViewModel()
        {
            _items =
                new ObservableCollection<TestItem>(
                    Enumerable.Range(0, 50).Select(x => new TestItem() {Content = "../Resources/girls.jpg"}));

        }

        public ObservableCollection<TestItem> Items
        {
            get { return _items ?? (_items = new ObservableCollection<TestItem>(Enumerable.Range(0, 50).Select(x => new TestItem() { Content = "../Resources/girls.jpg" } ))); }
        }

        public ICollectionView ItemsView
        {
            get { return _itemsView ?? (_itemsView = new ListCollectionView(Items)); }
        }
    }

    public class TestItem
    {
        public string Content { get; set; }
    }
}
