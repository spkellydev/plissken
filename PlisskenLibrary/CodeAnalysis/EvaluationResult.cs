﻿using System;
using System.Collections.Generic;

namespace PlisskenLibrary.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<string> diagnostics, object value)
        {
            Diagnostics = diagnostics;
            Value = value;
        }

        public IEnumerable<string> Diagnostics { get; }
        public object Value { get; }
    }
}
