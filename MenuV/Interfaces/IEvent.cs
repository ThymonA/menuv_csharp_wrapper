namespace MenuV
{
    using System;

    public interface IEvent
    {
        Guid UUID { get; }

        string Resource { get; }

        Action<IMenu, object[]> Func { get; set; }
    }
}
