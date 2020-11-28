namespace MenuV
{
    public class CheckboxItem : Item<bool>, ICheckboxItem
    {
        public CheckboxItem(
            bool value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false)
        {
            this.__event = "OnCheck";
            this.__type = "checkbox";
            this.Icon = icon;
            this.Label = label;
            this.Description = description;
            this.Disabled = disabled;
            this.Value = value;
            this.SaveOnUpdate = saveOnUpdate;

            this.AddEvents("enter", "leave", "update", "destroy", "change", "check", "uncheck");
        }

        protected override bool ValueParser(string key, object value)
        {
            return value.Ensure(false);
        }

        protected override bool GetValue()
        {
            return Value.Ensure(false);
        }
    }
}
