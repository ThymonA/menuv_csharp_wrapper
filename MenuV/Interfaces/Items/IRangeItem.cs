namespace MenuV
{
    public interface IRangeItem : IItem
    {
        int Min { get; set; }

        int Max { get; set; }
    }
}
