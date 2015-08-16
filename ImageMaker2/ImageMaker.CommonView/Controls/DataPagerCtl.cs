using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Annotations;

namespace ImageMaker.CommonView.Controls
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
    public class DataPagerCtl : Control
    {
        static DataPagerCtl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPagerCtl), new FrameworkPropertyMetadata(typeof(DataPagerCtl)));
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(DataPagerCtl), new PropertyMetadata(10));

        public ICollectionView ItemsView
        {
            get { return (ICollectionView)GetValue(ItemsViewProperty); }
            set { SetValue(ItemsViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsViewProperty =
            DependencyProperty.Register("ItemsView", typeof(ICollectionView), typeof(DataPagerCtl), new PropertyMetadata(ItemsViewChanged));

        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(DataPagerCtl), new PropertyMetadata(0));
        
        private ICommand _moveNextCommand;
        private ICommand _movePreviousCommand;

        public ICommand MoveNextCommand
        {
            get { return _moveNextCommand ?? (_moveNextCommand = new RelayCommand(MoveNext, () => CurrentPage < _pagesTotal)); }
        }

        public ICommand MovePreviousCommand
        {
            get { return _movePreviousCommand ?? (_movePreviousCommand = new RelayCommand(MovePrevious, () => CurrentPage > 1)); }
        }

        public ICommand MoveFirstCommand
        {
            get { return _moveFirstCommand ?? (_moveFirstCommand = new RelayCommand(MoveFirst, () => CurrentPage > 1)); }
        }

        public ICommand MoveLastCommand
        {
            get { return _moveLastCommand ?? (_moveLastCommand = new RelayCommand(MoveLast, () => CurrentPage < _pagesTotal)); }
        }

        private void MoveFirst()
        {
            MoveToPage(1);
        }

        private void MoveLast()
        {
            MoveToPage(_pagesTotal);
        }

        private void MoveToPage(int pageIndex)
        {
            CurrentPage = pageIndex;

            CommandManager.InvalidateRequerySuggested();
            ItemsView.Refresh();
        }

        private void MovePrevious()
        {
            MoveToPage(CurrentPage - 1);
        }

        private void MoveNext()
        {
            MoveToPage(CurrentPage + 1);
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
                    oldView.CollectionChanged -= pager.OnCollectionChanged;
                    if (oldView.Filter != null)
                        oldView.Filter -= pager.Filter;
                }
            }

            if (newView != null)
            {
                INotifyCollectionChanged collection = newView.SourceCollection as INotifyCollectionChanged;
                if (collection == null)
                    return;

                collection.CollectionChanged += pager.OnCollectionChanged;

                newView.Filter += pager.Filter;
                pager.CalculatePages();
            }
        }

        private bool Filter(object o)
        {
            if (ItemsView == null)
                return false;

            Delegate[] predicates = ItemsView.Filter.GetInvocationList();
            bool result = predicates.OfType<Predicate<object>>().Where(x => x != Filter).Aggregate(true, (comparison, method) => method(o));
            int index = ((IList)ItemsView.SourceCollection).IndexOf(o);
            return result && index >= PageSize * (CurrentPage - 1) && index < CurrentPage * PageSize;
        }

        private int _pagesTotal;
        private ICommand _moveFirstCommand;
        private ICommand _moveLastCommand;

        private void CalculatePages()
        {
            if (ItemsView == null)
                return;

            int count = ItemsView.SourceCollection.OfType<object>().Count();
            int remainder = count % PageSize;
            
            _pagesTotal = count / PageSize;
            _pagesTotal = remainder != 0 ? _pagesTotal + 1 : _pagesTotal;

            CurrentPage = 0;

            MoveNext();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if ((notifyCollectionChangedEventArgs.NewItems == null || notifyCollectionChangedEventArgs.NewItems.Count < 1))
                return;

            CalculatePages();
        }
    }
}
