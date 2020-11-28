namespace MenuV
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public class Menu : IMenu
    {
        public bool IsOpen { get; set; }

        public Guid UUID { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public MenuPosition Position { get; set; }

        public Color Color { get; set; }

        public MenuSize Size { get; set; }

        public string Dictionary { get; set; }

        public string Texture { get; set; }

        public IList<IItem> Items { get; } = new List<IItem>();

        private IDictionary<string, IList<IEvent>> Events { get; } = new Dictionary<string, IList<IEvent>>();

        public Menu(
            string title = "MenuV",
            string subtitle = "",
            MenuPosition position = MenuPosition.TopLeft,
            int red = 0,
            int green = 0,
            int blue = 255,
            MenuSize size = MenuSize.Size110,
            string texture = "default",
            string dictionary = "menuv")
            : this(title, subtitle, position, Color.FromArgb(red, green, blue), size, texture, dictionary)
        {
        }

        public Menu(
            string title = "MenuV",
            string subtitle = "",
            MenuPosition position = MenuPosition.TopLeft,
            Color color = new Color(),
            MenuSize size = MenuSize.Size110,
            string texture = "default",
            string dictionary = "menuv")
        {
            this.IsOpen = false;
            this.UUID = Guid.NewGuid();
            this.Title = title;
            this.Subtitle = subtitle;
            this.Position = position;
            this.Color = color.IsNullOrDefault() ? Color.FromArgb(0, 0, 255) : color;
            this.Size = size;
            this.Texture = string.IsNullOrWhiteSpace(texture) ? "default" : texture;
            this.Dictionary = string.IsNullOrWhiteSpace(dictionary) ? "menuv" : dictionary;
        }

        public void Trigger(string name, IMenu menu, params object[] parameters)
        {
            name = (string.IsNullOrWhiteSpace(name) ? "unknown" : name).ToLower();

            if (name == "unknown") { return; }

            name = name.StartsWith("on") ? $"On{name.Substring(2).FirstCharUpper()}" : $"On{name.FirstCharUpper()}";

            if (!Events.ContainsKey(name)) { return; }

            foreach (var callback in Events[name])
            {
                Task.Run(() => callback.Func(menu, parameters));
            }
        }

        public Guid On(string name, Action<IMenu, object[]> callback)
        {
            name = (string.IsNullOrWhiteSpace(name) ? "unknown" : name).ToLower();

            if (name == "unknown") { return Guid.Empty; }

            name = name.StartsWith("on") ? $"On{name.Substring(2).FirstCharUpper()}" : $"On{name.FirstCharUpper()}";

            if (!Events.ContainsKey(name)) { return Guid.Empty; }

            var eventItem = new Event(callback, MenuV.CurrentResourceName);

            Events[name].Add(eventItem);

            return eventItem.UUID;
        }

        public void RemoveOnEvent(string name, string uuid) => RemoveOnEvent(name, new Guid(uuid));

        public void RemoveOnEvent(string name, Guid uuid)
        {
            name = (string.IsNullOrWhiteSpace(name) ? "unknown" : name).ToLower();

            if (name == "unknown") { return; }

            name = name.StartsWith("on") ? $"On{name.Substring(2).FirstCharUpper()}" : $"On{name.FirstCharUpper()}";

            if (!Events.ContainsKey(name)) { return; }

            for (var i = 0; i < Events[name].Count; i++)
            {
                if (Events[name][i].UUID == uuid)
                {
                    Events[name].RemoveAt(i);
                }
            }
        }

        public IButtonItem AddButton<T>(
            T value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false)
        {
            var buttonItem = new ButtonItem<T>(value, icon, label, description, disabled, saveOnUpdate);

            Items.Add(buttonItem);

            return buttonItem;
        }

        public ICheckboxItem AddCheckbox(
            bool value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false)
        {
            var checkboxItem = new CheckboxItem(value, icon, label, description, disabled, saveOnUpdate);

            Items.Add(checkboxItem);

            return checkboxItem;
        }

        public IConfirmItem AddConfirm(
            bool value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false)
        {
            var confirmItem = new ConfirmItem(value, icon, label, description, disabled, saveOnUpdate);

            Items.Add(confirmItem);

            return confirmItem;
        }

        public object ToTable()
        {
            return new
            {
                uuid = UUID,
                title = Title,
                subtitle = Subtitle,
                position = Position.ToText(),
                size = Size.ToText(),
                dictionary = Dictionary,
                texture = Texture,
                color = new
                {
                    r = (int)Color.R,
                    g = (int)Color.G,
                    b = (int)Color.B
                },
                items = Items.ToTable()
            };
        }

        public void SetTitle(string title)
        {
            this.Title = title;
        }

        public void SetSubtitle(string subtitle)
        {
            this.Subtitle = subtitle;
        }

        public void SetPosition(MenuPosition position)
        {
            this.Position = position;
        }

        public void Open() => MenuV.OpenMenu(this);

        public void Close() => MenuV.CloseMenu(this);

        public void ClearItems()
        {
            Items.Clear();
        }
    }
}
