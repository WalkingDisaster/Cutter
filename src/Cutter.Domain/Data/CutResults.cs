namespace Cutter.Domain.Data
{
    public struct CutResults
    {
        public CutGroupResult[] Groups { get; set; }
        public UncutItemResult[] UncutItems { get; set; }
    }
}