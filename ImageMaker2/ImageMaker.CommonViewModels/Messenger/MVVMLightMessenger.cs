using System;

namespace ImageMaker.CommonViewModels.Messenger
{
    public class MvvmLightMessenger : IMessenger
    {
        private readonly MessageFactory _messageFactory;

        public MvvmLightMessenger(MessageFactory messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public TMessage CreateMessage<TMessage>()
        {
            return _messageFactory.CreateMessage<TMessage>();
        }

        public void Send<TMessage>(TMessage message)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(message);
        }

        public void Register<TMessage>(object recepient, Action<TMessage> callback)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register(recepient, callback);
        }

        public void Unregister<TMessage>(object recepient)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<TMessage>(recepient);
        }
    }
}
