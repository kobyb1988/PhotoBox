using System;

namespace ImageMaker.CommonViewModels.Messenger
{
    public class MessageFactory
    {
        public TMessage CreateMessage<TMessage>()
        {
            return Activator.CreateInstance<TMessage>();
        }
    }
}
