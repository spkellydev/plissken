using System.Collections.Generic;
using System.Collections.Immutable;

namespace PlisskenLibrary.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Parent also has a scope
        /// </summary>
        public BoundScope Parent { get; }
        
        /// <summary>
        /// Only declares new things. We want to support nesting of scopes and shadowing.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        /// var x = 10
        /// {
        ///     var x = false               |> shadowing
        /// }
        public bool TryDeclare(VariableSymbol variable)
        {
            if (_variables.ContainsKey(variable.Name))
                return false;
            _variables.Add(variable.Name, variable);
            return true;
        }

        /// <summary>
        /// If the variable is in scope, we can return it. Otherwise we need to check if there is a parent and if
        /// that parent has the variable in scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool TryLookup(string name, out VariableSymbol variable)
        {
            if (_variables.TryGetValue(name, out variable))
            {
                return true;
            }
            if (Parent == null)
                return false;
            return Parent.TryLookup(name, out variable);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }
    }
}
