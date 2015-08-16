using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ImageMaker.CommonViewModels.ViewModels.Validation
{
    public class ValidationRule<TMember> : ICustomValidationRule
    {
        private readonly TMember _member;
        private readonly Dictionary<string, ValidatedProperty<TMember>> _rules = new Dictionary<string, ValidatedProperty<TMember>>();
        private bool _isValidObject;

        private ValidationRule(TMember member)
        {
            _member = member;
        }

        public static ValidationRule<TMember> Create(TMember member)
        {
            return new ValidationRule<TMember>(member);
        }

        public ValidationRule<TMember> For<TProperty>(
            Expression<Func<TMember, TProperty>> property, 
            Predicate<TMember> rule, 
            string error,
            bool defValue = true)
        {
            string memberName = ((MemberExpression) property.Body).Member.Name;
            _rules.Add(memberName, new ValidatedProperty<TMember>(error, rule));
            return this;
        }

        public ValidationRule<TMember> Complete()
        {
            _isValidObject = _rules.All(x => x.Value.IsValid);
            return this;
        }

        public string GetError(string propertyName)
        {
            if (!_rules.ContainsKey(propertyName))
                return string.Empty;

            string error = _rules[propertyName].GetError(_member);
            _isValidObject = _rules.All(x => x.Value.IsValid);
            return error;
        }

        public bool IsValidObject
        {
            get { return _isValidObject; }
        }
    }
}