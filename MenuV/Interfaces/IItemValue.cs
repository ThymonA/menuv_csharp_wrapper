namespace MenuV
{
    using System;

    public interface IItemValue
    {
        Type Type { get; }

        string Label { get; set; }

        string Description { get; set; }

        object Value { get; set; }
    }
}
