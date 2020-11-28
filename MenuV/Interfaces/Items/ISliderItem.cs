namespace MenuV
{
    using System.Collections.Generic;

    public interface ISliderItem : IItem
    {
        IList<IItemValue> Values { get; }
    }
}
