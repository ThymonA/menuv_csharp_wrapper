namespace MenuV
{
    using System;

    public class Event : IEvent
    {
        public Guid UUID { get; }

        public string Resource { get; }

        public Action<IMenu, object[]> Func { get; set; }

        public Event(Action<IMenu, object[]> func, string resource)
        {
            this.UUID = Guid.NewGuid();
            this.Resource = string.IsNullOrWhiteSpace(resource) ? MenuV.CurrentResourceName : resource;
            this.Func = func;
        }
    }
}
