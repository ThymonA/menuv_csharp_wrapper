namespace MenuV
{
    using System;

    using CitizenFX.Core;

    /// <summary>
    /// MenuV Item
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Item Type
        /// </summary>
        string __type { get; }

        /// <summary>
        /// Item value type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// UUID of Item
        /// </summary>
        Guid UUID { get; }

        /// <summary>
        /// Set any emoji as Item icon
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// This text will be displayed in NUI
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// This will be used as description when this item is active
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Item's value
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Disable this item for selection in NUI
        /// </summary>
        bool Disabled { get; set; }

        /// <summary>
        /// Save when value changes
        /// </summary>
        bool SaveOnUpdate { get; set; }

        /// <summary>
        /// Trigger all events based on <see cref="name"/>
        /// </summary>
        /// <param name="name">Name of event</param>
        /// <param name="parameters">Parameters of event</param>
        void Trigger(string name, params object[] parameters);

        /// <summary>
        /// Register a <see cref="CallbackDelegate"/> as "on" event
        /// </summary>
        /// <param name="name">Name of event</param>
        /// <param name="callback">Callback to execute</param>
        void On(string name, CallbackDelegate callback);

        /// <summary>
        /// Trigger item's default event
        /// </summary>
        /// <param name="parameters">Parameters of event</param>
        void Call(params string[] parameters);
    }
}
