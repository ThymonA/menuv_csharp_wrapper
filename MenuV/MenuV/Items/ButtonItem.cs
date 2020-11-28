namespace MenuV
{
    public class ButtonItem<T> : Item<T>, IButtonItem
    {
        public ButtonItem(
            T value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false)
        {
            this.__event = "OnSelect";
            this.__type = typeof(T) == typeof(Menu) ? "menu" : "button";
            this.Icon = icon;
            this.Label = label;
            this.Description = description;
            this.Disabled = disabled;
            this.Value = value;
            this.SaveOnUpdate = saveOnUpdate;

            this.AddEvents("enter", "leave", "update", "destroy", "select");
        }

        protected override T ValueParser(string key, object value)
        {
            return value.Ensure(default(T));
        }

        protected override T GetValue()
        {
            return this.Value is T v ? v : default;
        }
    }
}
