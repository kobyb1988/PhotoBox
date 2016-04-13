using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ImageMaker.CommonViewModels.Commands
{
    public static class DelegateCommandExtensions
    {
        /// <summary>
        /// Makes DelegateCommnand listen on PropertyChanged events of some object,
        /// so that DelegateCommnand can update its IsEnabled property.
        /// </summary>
        public static DelegateCommand ListenOn<ObservedType, PropertyType>
            (this DelegateCommand delegateCommand,
            ObservedType observedObject,
            Expression<Func<ObservedType, PropertyType>> propertyExpression,
            Dispatcher dispatcher)
            where ObservedType : INotifyPropertyChanged
        {
            //string propertyName = observedObject.GetPropertyName(propertyExpression);
            string propertyName = NotifyPropertyChangedBaseExtensions.GetPropertyName(propertyExpression);

            observedObject.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    if (dispatcher != null)
                    {
                        ThreadTools.RunInDispatcher(dispatcher, delegateCommand.RaiseCanExecuteChanged);
                    }
                    else
                    {
                        delegateCommand.RaiseCanExecuteChanged();
                    }
                }
            };

            return delegateCommand; //chain calling
        }

        /// <summary>
        /// Makes DelegateCommnand listen on PropertyChanged events of some object,
        /// so that DelegateCommnand can update its IsEnabled property.
        /// </summary>
        public static DelegateCommand<T> ListenOn<T, ObservedType, PropertyType>
            (this DelegateCommand<T> delegateCommand,
            ObservedType observedObject,
            Expression<Func<ObservedType, PropertyType>> propertyExpression,
            Dispatcher dispatcher)
            where ObservedType : INotifyPropertyChanged
        {
            //string propertyName = observedObject.GetPropertyName(propertyExpression);
            string propertyName = NotifyPropertyChangedBaseExtensions.GetPropertyName(propertyExpression);

            observedObject.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    if (dispatcher != null)
                    {
                        ThreadTools.RunInDispatcher(dispatcher, delegateCommand.RaiseCanExecuteChanged);
                    }
                    else
                    {
                        delegateCommand.RaiseCanExecuteChanged();
                    }
                }
            };

            return delegateCommand; //chain calling
        }
    }
}
