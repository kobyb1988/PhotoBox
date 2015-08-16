using System.Configuration;

namespace ImageMaker.MessageQueueing.MessageQueueing
{
    public class QueueUtilizerFactory
    {
        public QueueUtilizerFactory()
        {
        }

        public QueueUtilizer<TMessage> CreateQueue<TMessage>() where TMessage : class
        {
            string queueName = ConfigurationManager.AppSettings["instagramPrinterQueue"];
            return new QueueUtilizer<TMessage>(queueName);
        }
    }
}
