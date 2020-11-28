namespace MenuV
{
    public class ConfirmItem : Item<bool>, IConfirmItem
    {
        public ConfirmItem(
            bool value,
            string icon = "",
            string label = "",
            string description = "",
            bool disabled = false,
            bool saveOnUpdate = false)
        {
            this.__event = "OnConfirm";
            this.__type = "confirm";
            this.Icon = icon;
            this.Label = label;
            this.Description = description;
            this.Disabled = disabled;
            this.Value = value;
            this.SaveOnUpdate = saveOnUpdate;

            this.AddEvents("enter", "leave", "update", "destroy", "confirm", "deny", "change");
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
