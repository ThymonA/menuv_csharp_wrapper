namespace MenuV
{
    using System.Collections.Generic;

    public static class ItemExtension
    {
        public static object[] ToTable(this IList<IItem> items)
        {
            var objects = new object[] { };

            for (var i = 0; i < items.Count; i++)
            {
                var current = items[i];
                var obj = new
                {
                    index = i,
                    type = current.__type,
                    uuid = current.UUID.ToString(),
                    icon = current.Icon,
                    label = current.Label,
                    description = current.Description,
                    value = current.GetCurrentValue(),
                    values = new object[] { },
                    min = 0,
                    max = 0,
                    disabled = current.Disabled
                };

                if (current.__type == "slider")
                {
                    var sliderObject = current is ISliderItem v ? v : null;

                    if (sliderObject.IsNullOrDefault()) { continue; }

                    for (var i2 = 0; i2 < sliderObject.Values.Count; i2++)
                    {
                        var option = sliderObject.Values[i2];

                        if (option.IsNullOrDefault()) { continue; }

                        var optionObject = new { label = option.Label, description = option.Description, value = i2 };

                        obj.values.SetValue(optionObject, obj.values.Length);
                    }
                }

                objects.SetValue(obj, objects.Length);
            }

            return objects;
        }

        public static object GetCurrentValue(this IItem item)
        {
            switch (item.__type)
            {
                case "confirm":
                case "checkbox":
                    return item.Value.Ensure(false);
                case "range":
                    var rangeItem = item is IRangeItem v2 ? v2 : null;

                    return item.Value.Ensure(rangeItem.IsNullOrDefault() ? 0 : rangeItem.Min);
                case "slider":
                    return item.Value.Ensure(0);
                case "button":
                case "menu":
                case "unknown":
                    return "none";
            }

            return null;
        }
    }
}
