using System.Collections.Generic;

namespace Cutter.Domain.Model
{
    public struct OptimizationResult
    {
        public IEnumerable<CutItem> Items { get; set; }
        public int Repeat { get; set; }
    }
}