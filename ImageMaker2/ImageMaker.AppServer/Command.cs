using System.Runtime.Serialization;

namespace ImageMaker.AppServer
{

    [KnownType(typeof(Command))]
    [KnownType(typeof(CloseCommand))]
    [DataContract]
    public abstract class BaseCommand
    {
        public abstract void Process(ICommandProcessor commandProcessor);
    }

    [DataContract]
    public class Command : BaseCommand
    {
        public override void Process(ICommandProcessor commandProcessor)
        {
            commandProcessor.ProcessCommand(this);
        }
    }

    [DataContract]
    public class CloseCommand : BaseCommand
    {
        public override void Process(ICommandProcessor commandProcessor)
        {
            commandProcessor.ProcessCloseCommand(this);
        }
    }

    public interface ICommandProcessor
    {
        void ProcessCommand(BaseCommand command);

        void ProcessCloseCommand(BaseCommand command);
    }
}