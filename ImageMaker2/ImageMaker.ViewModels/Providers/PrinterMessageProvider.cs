using System;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.MessageQueueing.Dto;
using ImageMaker.MessageQueueing.MessageQueueing;
using ImageMaker.ViewModels.ViewModels.Images;

namespace ImageMaker.ViewModels.Providers
{
    public class PrinterMessageProvider
    {
        private readonly QueueListenerFactory _listenerFactory;
        private readonly IMappingEngine _mappingEngine;
        public event EventHandler<MessageEventArgs<InstagramImageViewModel>> MessageDelivered;

        protected virtual void OnMessageDelivered(InstagramMessageDto message)
        {
            InstagramImageViewModel image = _mappingEngine.Map<InstagramImageViewModel>(message);
            EventHandler<MessageEventArgs<InstagramImageViewModel>> handler = MessageDelivered;
            if (handler != null) handler(this, new MessageEventArgs<InstagramImageViewModel>(this, image));
        }

        public PrinterMessageProvider(
            QueueListenerFactory listenerFactory,
            IMappingEngine mappingEngine)
        {
            _listenerFactory = listenerFactory;
            _mappingEngine = mappingEngine;
        }

        public void StartListening()
        {
            _queueListener = _listenerFactory.Create<InstagramMessageDto>();
            Listen();
        }

        private QueueListener<InstagramMessageDto> _queueListener;
        private CancellationTokenSource _tokenSource;

        private bool _startAgain;

        private void Listen()
        {
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _startAgain = true;
                InterruptListening();
                return;
            }

             _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Run(async () =>
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    var message = await _queueListener.StartListening();
                    if (message != null)
                        OnMessageDelivered(message);
                }
            }, token).ContinueWith(t =>
            {
                if (!_startAgain)
                    return;

                _startAgain = false;
                StartListening();
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void InterruptListening()
        {
            _tokenSource.Do(x => x.Cancel());
        }
    }
}
