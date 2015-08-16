using System.Messaging;
using System.Threading.Tasks;
using System.Transactions;

namespace ImageMaker.MessageQueueing.MessageQueueing
{
    public class QueueUtilizer<TMessage>
    {
        private readonly string _queueName;

        public QueueUtilizer(string queueName)
        {
            _queueName = queueName;
        }

        public async Task SendMessage(TMessage message)
        {
            await Task.Factory.StartNew(() =>
            {
                if (!MessageQueue.Exists(_queueName))
                    MessageQueue.Create(_queueName, true);

                MessageQueue messageQueue = new MessageQueue(@"FormatName:Direct=OS:" + _queueName);
                Message msg = new Message();
                msg.Body = message;

                //Create a transaction scope.
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    messageQueue.Send(msg, MessageQueueTransactionType.Automatic);
                    // Complete the transaction.
                    scope.Complete();
                }

                messageQueue.Close();
            });
        }
    }
}
