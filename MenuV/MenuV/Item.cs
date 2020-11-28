namespace MenuV
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    public abstract class Item<T> : IItem
    {
        public string __event { get; set; } = "unknown";

        public string __type { get; set; } = "item";

        public Type Type { get; } = typeof(T);

        public Guid UUID { get; } = Guid.NewGuid();

        public string Icon { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public object Value { get; set; } = null;

        public bool Disabled { get; set; } = false;

        public IDictionary<string, IList<CallbackDelegate>> Events { get; } = new Dictionary<string, IList<CallbackDelegate>>();

        public bool SaveOnUpdate { get; set; } = false;

        public virtual void Trigger(string name, params object[] parameters)
        {
            name = (string.IsNullOrWhiteSpace(name) ? "unknown" : name).ToLower();

            if (name == "unknown") { return; }

            name = name.StartsWith("on") ? $"On{name.Substring(2).FirstCharUpper()}" : $"On{name.FirstCharUpper()}";

            if (!Events.ContainsKey(name)) { return; }

            foreach (var callback in Events[name])
            {
                Task.Run(() => callback(parameters));
            }
        }

        public virtual void On(string name, CallbackDelegate callback)
        {
            name = (string.IsNullOrWhiteSpace(name) ? "unknown" : name).ToLower();

            if (name == "unknown") { return; }

            name = name.StartsWith("on") ? $"On{name.Substring(2).FirstCharUpper()}" : $"On{name.FirstCharUpper()}";

            if (!Events.ContainsKey(name)) { return; }

            Events[name].Add(callback);
        }

        public virtual void Call(params string[] parameters) => Trigger(__event, this, parameters);

        protected virtual bool Validate<TAny>(string key, TAny value) => true;

        protected abstract T ValueParser(string key, object value);

        protected abstract T GetValue();

        protected void AddEvents(params string[] events)
        {
            foreach (var evt in events)
            {
                var name = (string.IsNullOrWhiteSpace(evt) ? "unknown" : evt).ToLower();

                if (name == "unknown") { continue; }

                name = name.StartsWith("on") ? $"On{name.Substring(2).FirstCharUpper()}" : $"On{name.FirstCharUpper()}";

                if (Events.ContainsKey(name)) { continue; }

                Events.Add(name, new List<CallbackDelegate>());
            }
        }
    }
}
