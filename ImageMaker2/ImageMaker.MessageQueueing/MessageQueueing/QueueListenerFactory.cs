using System.Configuration;

namespace ImageMaker.MessageQueueing.MessageQueueing
{
    public class QueueListenerFactory
    {
        public QueueListenerFactory()
        {
        }

        public QueueListener<TMessage> Create<TMessage>() where TMessage : class
        {
            string queueName = ConfigurationManager.AppSettings["instagramPrinterQueue"];
            return new QueueListener<TMessage>(queueName);
        }
    }
}
