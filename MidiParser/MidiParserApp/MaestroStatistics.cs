namespace MidiParserApp
{
    public class MaestroStatistics
    {
        public int MinTimeDiff { get; set; }
        public int MaxTimeDiff { get; set; }
        public double AvgTimeDiff { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public double AvgLength { get; set; }
        public int MinNoteNumber { get; set; }
        public int MaxNoteNumber { get; set; }
        public double AvgNoteNumber { get; set; }
        public int MinVelocity { get; set; }
        public int MaxVelocity { get; set; }
        public double AvgVelocity { get; set; }
    }
}
