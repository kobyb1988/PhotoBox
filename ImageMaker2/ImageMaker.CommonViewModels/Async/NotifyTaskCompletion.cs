using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ImageMaker.CommonViewModels.Async
{
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public event EventHandler ResultUpdated;

        #region Properties and Fields
        public Task<TResult> TaskRes { get; private set; }
        public Task Task { get; private set; }
        public Task TaskCompletion { get; private set; }
        public TResult Result
        {
            get
            {
                return TaskRes.With(x => x.Status == TaskStatus.RanToCompletion ? x.Result : default(TResult));
            }
        }
        public TaskStatus Status { get { return TaskRes == null ? Task.Status : TaskRes.Status; } }
        public bool IsCompleted { get { return TaskRes == null ? Task.IsCompleted : TaskRes.IsCompleted; } }
        public bool IsNotCompleted { get { return TaskRes == null ? !Task.IsCompleted : !TaskRes.IsCompleted; } }
        public bool IsSuccessfullyCompleted { get { return TaskRes == null ? Task.Status == TaskStatus.RanToCompletion : TaskRes.Status == TaskStatus.RanToCompletion; } }
        public bool IsCanceled { get { return TaskRes == null ? Task.IsCanceled : TaskRes.IsCanceled; } }
        public bool IsFaulted { get { return TaskRes == null ? Task.IsFaulted : TaskRes.IsFaulted; } }
        public AggregateException Exception { get { return TaskRes == null ? Task.Exception : TaskRes.Exception; } }
        public Exception InnerException { get { return (Exception == null) ? null : Exception.InnerException; } }
        public string ErrorMessage { get { return (InnerException == null) ? null : InnerException.Message; } }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors
        public NotifyTaskCompletion(Task<TResult> taskRes)
        {
            ApplyTask(taskRes);
        }

        public void ApplyTask(Task<TResult> taskRes)
        {
            Debug.Assert(taskRes != null, "taskRes in NotifyTaskCompletion is null");
            TaskRes = taskRes;
            if (!taskRes.IsCompleted)
                TaskCompletion = WatchTaskAsync(taskRes);
        }

        public NotifyTaskCompletion(Task task)
        {
            Debug.Assert(task != null, "taskRes in NotifyTaskCompletion is null");
            Task = task;
            if (!task.IsCompleted)
                TaskCompletion = WatchTaskAsync(task);
        }

        #endregion

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException ex)
            {
                _logger.Trace(ex.ToString);
            }
            catch (Exception ex)
            {
                _logger.Error($"Произошла ошибка при выполнении асинхронной операции {ErrorMessage}, {ex.ToString()}");
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this, new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
                //if (ResultUpdated != null) ResultUpdated(this, new EventArgs());
            }
        }
    }
}
