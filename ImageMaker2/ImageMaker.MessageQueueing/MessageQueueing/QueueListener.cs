using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading.Tasks;

namespace ImageMaker.MessageQueueing.MessageQueueing
{
    public class QueueListener<TMessage> where TMessage : class
    {
        private readonly MessageQueue _queue;

        public QueueListener(string queueName)
        {
            if (!MessageQueue.Exists(queueName))
                MessageQueue.Create(queueName, true);

            //Connect to the queue
            _queue = new MessageQueue(queueName);
        }

        public Task<TMessage> StartListening()
        {
            TaskCompletionSource<TMessage> source = new TaskCompletionSource<TMessage>();
            Task.Factory.FromAsync(_queue.BeginReceive(TimeSpan.FromSeconds(10)), result =>
            {
                try
                {
                    if (!result.IsCompleted)
                    {
                        source.TrySetResult(null);
                        return;
                    }

                    Message message = _queue.EndReceive(result);
                    message.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(TMessage) });
                    TMessage content = message.Body as TMessage;

                    if (content != null)
                        Console.WriteLine("Processing {0} ", content);
                    else
                    {
                        Console.WriteLine("Processing empty messsage ");
                    }

                    source.TrySetResult(content);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    source.TrySetResult(null);
                }
            });

            return source.Task;
        }
    }

    public class MessageEventArgs<TMessage> : EventArgs
    {
        public MessageEventArgs(object sender, TMessage content)
        {
            Sender = sender;
            Content = content;
        }

        public object Sender { get; private set; }

        public TMessage Content { get; private set; }
    }
}
