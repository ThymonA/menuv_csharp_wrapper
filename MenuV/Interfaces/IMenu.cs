namespace MenuV
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// MenuV Menu
    /// </summary>
    public interface IMenu
    {
        bool IsOpen { get; set; }

        Guid UUID { get; set; }

        string Title { get; set; }

        string Subtitle { get; set; }

        MenuPosition Position { get; set; }

        Color Color { get; set; }

        MenuSize Size { get; set; }

        string Dictionary { get; set; }

        string Texture { get; set; }

        IList<IItem> Items { get; }

        void Trigger(string name, IMenu menu, params object[] parameters);

        Guid On(string name, Action<IMenu, object[]> callback);

        void RemoveOnEvent(string name, string uuid);

        void RemoveOnEvent(string name, Guid uuid);

        IButtonItem AddButton<T>(
            T value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false);

        ICheckboxItem AddCheckbox(
            bool value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false);

        IConfirmItem AddConfirm(
            bool value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false);

        object ToTable();
    }
}
