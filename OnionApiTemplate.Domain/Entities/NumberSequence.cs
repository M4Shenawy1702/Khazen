namespace Khazen.Domain.Entities
{
    public class NumberSequence : BaseEntity<int>
    {
        public string Prefix { get; set; } = string.Empty;
        public int Year { get; set; }
        public int CurrentNumber { get; set; }
    }
}
