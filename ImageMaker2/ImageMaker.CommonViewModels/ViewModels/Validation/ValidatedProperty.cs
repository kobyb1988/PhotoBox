using System;

namespace ImageMaker.CommonViewModels.ViewModels.Validation
{
    public class ValidatedProperty<TMember>
    {
        private readonly Predicate<TMember> _predicate;
        private bool _isValid;

        public ValidatedProperty(string error, Predicate<TMember> predicate, bool defState = true)
        {
            _isValid = defState;
            _predicate = predicate;
            Error = error;
        }

        public string GetError(TMember member)
        {
            _isValid = _predicate(member);
            return _isValid ? string.Empty : Error;
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public string Error { get; private set; }
    }
}