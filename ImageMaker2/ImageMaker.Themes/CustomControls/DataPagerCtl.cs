using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Annotations;

namespace ImageMaker.Themes.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageMaker.Themes.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageMaker.Themes.CustomControls;assembly=ImageMaker.Themes.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DataPagerCtl/>
    ///
    /// </summary>
    public class DataPagerCtl : Control, INotifyPropertyChanged
    {
        static DataPagerCtl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPagerCtl), new FrameworkPropertyMetadata(typeof(DataPagerCtl)));
        }

        public DataPagerCtl()
        {
            Loaded += (sender, args) => CalculateWidths();
        }

        //public IList ItemsSource
        //{
        //    get { return (IList)GetValue(ItemsSourceProperty); }
        //    set { SetValue(ItemsSourceProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register("ItemsSource", typeof(IList), typeof(DataPagerCtl), new PropertyMetadata(null, ItemsSourceChanged));

        //private static void ItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    DataPagerCtl pager = dependencyObject as DataPagerCtl;
        //    if (pager == null || dependencyPropertyChangedEventArgs.NewValue == null)
        //        return;

        //    IList itemsSource = (IList) dependencyPropertyChangedEventArgs.NewValue;
        //    pager.ItemsView = new ListCollectionView(itemsSource);
        //    pager.ItemsView.CollectionChanged += pager.ItemsViewOnCollectionChanged;
        //    pager.ItemsView.Filter += pager.Filter;
        //    pager.CalculatePages();
        //}

        
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }   

        // Using a DependencyProperty as the backing store for PageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(DataPagerCtl), new PropertyMetadata(0));



        public int PagesInRow
        {
            get { return (int)GetValue(PagesInRowProperty); }
            set { SetValue(PagesInRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PagesInRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagesInRowProperty =
            DependencyProperty.Register("PagesInRow", typeof(int), typeof(DataPagerCtl), new PropertyMetadata(0));


        public ListCollectionView PagesView
        {
            get { return (ListCollectionView)GetValue(PagesViewProperty); }
            set { SetValue(PagesViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PagesView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagesViewProperty =
            DependencyProperty.Register("PagesView", typeof(ListCollectionView), typeof(DataPagerCtl), new PropertyMetadata(null));

        
        public IList<PageItemWrapper> Pages
        {
            get { return (IList<PageItemWrapper>)GetValue(PagesProperty); }
            set { SetValue(PagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagesProperty =
            DependencyProperty.Register("Pages", typeof(IList<PageItemWrapper>), typeof(DataPagerCtl), new PropertyMetadata(null));

        
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(DataPagerCtl), new PropertyMetadata(0));


        public ICollectionView ItemsView
        {
            get { return (ICollectionView)GetValue(ItemsViewProperty); }
            set { SetValue(ItemsViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsViewProperty =
            DependencyProperty.Register("ItemsView", typeof(ICollectionView), typeof(DataPagerCtl), new PropertyMetadata(ItemsViewChanged));

        private RelayCommand _moveNextCommand;
        private RelayCommand _movePreviousCommand;
        private ICommand _moveToCommand;

        public RelayCommand MoveNextCommand
        {
            get
            {
                return _moveNextCommand ?? (_moveNextCommand = new RelayCommand(MoveNext, () => Pages != null && PageIndex < Pages.Count - 1)); 
            }
        }

        public RelayCommand MovePreviousCommand
        {
            get { return _movePreviousCommand ?? (_movePreviousCommand = new RelayCommand(MovePrevious, () => PageIndex > 0)); }
        }

        public ICommand MoveToCommand
        {
            get { return _moveToCommand ?? (_moveToCommand = new RelayCommand<int>(MoveToPage)); }
        }

        private static void ItemsViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pager = d as DataPagerCtl;
            if (pager == null)
                return;

            var oldView = e.OldValue as ICollectionView;
            var newView = e.NewValue as ICollectionView;
            if (oldView != null)
            {
                INotifyCollectionChanged collection = oldView.SourceCollection as INotifyCollectionChanged;
                if (collection != null)
                {
                    oldView.CollectionChanged -= pager.ItemsViewOnCollectionChanged;
                    if (oldView.Filter != null)
                        oldView.Filter -= pager.Filter;
                }
            }

            if (newView != null)
            {
                INotifyCollectionChanged collection = newView.SourceCollection as INotifyCollectionChanged;
                if (collection == null)
                    return;

                collection.CollectionChanged += pager.ItemsViewOnCollectionChanged;

                newView.Filter += pager.Filter;
                pager.CalculatePages();
            }
        }

        private void ItemsViewOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if ((notifyCollectionChangedEventArgs.NewItems == null || notifyCollectionChangedEventArgs.NewItems.Count < 1))
                return;

            CalculatePages();
        }

        private void CalculatePages()
        {
            if (ItemsView == null)
                return;

            int count = ItemsView.SourceCollection.OfType<object>().Count();
            int remainder = count % PageSize;

            int totalCount = count / PageSize;
            totalCount = remainder != 0 ? totalCount + 1 : totalCount;

            PageIndex = -1;

            Pages = Enumerable.Range(0, totalCount).Select(x => new PageItemWrapper(x)).ToList();
            PagesView = new ListCollectionView(new ObservableCollection<PageItemWrapper>(Pages));
            PagesView.Filter += page =>
                                {
                                    PageItemWrapper wrapper = page as PageItemWrapper;
                                    if (wrapper == null) return false;

                                    int index = Pages.IndexOf(wrapper);
                                    if (index == Pages.Count - 1
                                        || index == 0
                                        || (Pages.Count - 2)/2 < PagesInRow)
                                    {
                                        wrapper.Update(false);
                                        return true;
                                    }

                                    int rem = (Pages.Count - 2) % PagesInRow;
                                    int groupsCount = (Pages.Count - 2) / PagesInRow;
                                    groupsCount = rem != 0 ? groupsCount + 1 : groupsCount;

                                    int groupIdx = 0;
                                    if (index <= PagesInRow)
                                        groupIdx = 1;
                                    else
                                    {
                                        int idxRem = index % PagesInRow;
                                        groupIdx = index / PagesInRow;
                                        groupIdx = (idxRem != 0 ? groupIdx + 1 : groupIdx);
                                    }

                                    int pageIndexGroupIdx = 0;
                                    if (PageIndex <= PagesInRow)
                                        pageIndexGroupIdx = 1;
                                    else
                                    {
                                        int idxRem = PageIndex % PagesInRow;
                                        pageIndexGroupIdx = PageIndex / PagesInRow;
                                        pageIndexGroupIdx = (idxRem != 0 ? pageIndexGroupIdx + 1 : pageIndexGroupIdx);
                                    }

                                    if (pageIndexGroupIdx == groupIdx)
                                    {
                                        wrapper.Update(false);
                                        return true;
                                    }

                                    if ((pageIndexGroupIdx - 1) * PagesInRow == index)
                                    {
                                        wrapper.Update(true);
                                        return true;
                                    }

                                    if ((pageIndexGroupIdx + 1)*PagesInRow + 1 - PagesInRow == index)
                                    {
                                        wrapper.Update(true);
                                        return true;
                                    }

                                    return false;
                                };

            if (totalCount <= 0)
                return;

            MoveNext();
        }

        private bool Filter(object o)
        {
            if (ItemsView == null)
                return false;

            Delegate[] predicates = ItemsView.Filter.GetInvocationList();
            bool result = predicates.OfType<Predicate<object>>().Where(x => x != Filter).Aggregate(true, (comparison, method) => method(o));
            int index = ((IList)ItemsView.SourceCollection).IndexOf(o);
            return result && (index >= PageSize * PageIndex) && (index < (PageIndex + 1) * PageSize);
        }

        private void MoveToPage(int pageIndex)
        {
            if (Pages.Count <= 0)
                return;

            if (PageIndex >= 0)
                Pages.ElementAt(PageIndex).IsSelected = false;

            PageIndex = pageIndex;
            if (PageIndex >= 0)
                Pages.ElementAt(PageIndex).IsSelected = true;

            CommandManager.InvalidateRequerySuggested();
            ItemsView.Refresh();
            PagesView.Refresh();
            CalculateWidths();
        }

        private void CalculateWidths()
        {
            var visibleItems = PagesView.OfType<PageItemWrapper>().ToList();
            if (this.Template == null)
                return;

            ItemsControl itemsControl = (ItemsControl)this.Template.FindName("items", this);
            if (itemsControl == null)
                return;

            foreach (var page in visibleItems)
            {
                page.Width = itemsControl.ActualWidth / visibleItems.Count;
            }
        }

        private void MovePrevious()
        {
            MoveToPage(PageIndex - 1);
            MovePreviousCommand.RaiseCanExecuteChanged();
        }

        private void MoveNext()
        {
            MoveToPage(PageIndex + 1);
            MoveNextCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PageItemWrapper : INotifyPropertyChanged
    {
        private readonly int _index;
        private string _content;
        private bool _isSelected;
        private double _width;

        public PageItemWrapper(int index)
        {
            _index = index;
            _content = (_index + 1).ToString(CultureInfo.InvariantCulture);
        }

        public int Index
        {
            get { return _index; }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                    return;

                _width = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                if (_content == value)
                    return;
                
                _content = value;
                OnPropertyChanged();
            }
        }

        public void Update(bool isRear)
        {
            Content = isRear ? "..." : (_index + 1).ToString(CultureInfo.InvariantCulture);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
