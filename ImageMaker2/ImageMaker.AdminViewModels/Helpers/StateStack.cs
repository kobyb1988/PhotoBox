using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageMaker.AdminViewModels.Helpers
{
    public interface ICopyable<TObject>
    {
        TObject Copy();

        void CopyTo(TObject to);
    }

    public interface IStack
    {
        void Save();

        void Restore();
    }

    public class ItemState<TItem> : IStack where TItem : class, ICopyable<TItem>
    {
        private readonly TItem _item;
        //private readonly Action<TItem> _action;
        private TItem _savedState;

        public ItemState(TItem item)
        {
            _item = item;
            //_action = action;
        }

        public void Save()
        {
            if (_savedState == null)
            {
                _savedState = _item.Copy();
                return;
            }

            //_savedState = _item.Copy();
            var temp = _item.Copy();
            _savedState.CopyTo(_item);
            _savedState = temp;
        }

        public void Restore()
        {
            var temp = _item.Copy();
            _savedState.CopyTo(_item);
            _savedState = temp;
        }

        //public void Execute()
        //{
        //    _savedState = _item.Copy();
        //    _action(_item);
        //}
    }

    public class StackStorage
    {
        private readonly Lazy<Stack<IStack>> _undoStack = new Lazy<Stack<IStack>>();
        private readonly Lazy<Stack<IStack>> _redoStack = new Lazy<Stack<IStack>>();

        public void Do<TItem>(TItem item) where TItem : class, ICopyable<TItem>
        {
            var state = new ItemState<TItem>(item);
            state.Save();
            //state.Execute();
            _undoStack.Value.Push(state);
        }

        public Chain Chain<TItem>(TItem item) where TItem : class, ICopyable<TItem>
        {
            var chain = new Chain();
            _undoStack.Value.Push(chain);
            return chain.Add(item);
        }

        public bool CanUndo
        {
            get { return _undoStack.IsValueCreated && _undoStack.Value.Count > 0; }
        }

        public bool CanRedo
        {
            get { return _redoStack.IsValueCreated && _redoStack.Value.Count > 0; }
        }

        public void Undo()
        {
            var copy = _undoStack.Value.Pop();

            _redoStack.Value.Push(copy);

            copy.Restore();
        }

        public void Redo()
        {
            var copy = _redoStack.Value.Pop();
            copy.Save();
            _undoStack.Value.Push(copy);
        }

        public void Clear()
        {
            _undoStack.Value.Clear();
            _redoStack.Value.Clear();
        }

        public void Reset()
        {
            while(CanUndo)
                Undo();
        }
    }

    public class  Chain : IStack
    {
        private readonly List<IStack> _items = new List<IStack>();
 
        public Chain Add<TItem>(TItem item) where TItem : class, ICopyable<TItem>
        {
            var state = new ItemState<TItem>(item);
            state.Save();
            _items.Add(state);
            return this;
        }

        public void Save()
        {
            _items.ForEach(x => x.Save());
        }

        public void Restore()
        {
            Enumerable.Reverse(_items).ToList().ForEach(x => x.Restore());
        }
    }
}
