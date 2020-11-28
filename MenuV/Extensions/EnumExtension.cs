namespace MenuV
{
    public static class EnumExtension
    {
        public static string ToText(this MenuPosition position)
        {
            if (position.IsNullOrDefault())
            {
                return "topleft";
            }

            switch (position)
            {
                case MenuPosition.TopLeft:
                    return "topleft";
                case MenuPosition.TopCenter:
                    return "topcenter";
                case MenuPosition.TopRight:
                    return "topright";
                case MenuPosition.CenterLeft:
                    return "centerleft";
                case MenuPosition.Center:
                    return "center";
                case MenuPosition.CenterRight:
                    return "centerright";
                case MenuPosition.BottomLeft:
                    return "bottomleft";
                case MenuPosition.BottomCenter:
                    return "bottomcenter";
                case MenuPosition.BottomRight:
                    return "bottomright";
            }

            return "topleft";
        }

        public static string ToText(this MenuSize size)
        {
            if (size.IsNullOrDefault())
            {
                return "size-110";
            }

            switch (size)
            {
                case MenuSize.Size100:
                    return "size-100";
                case MenuSize.Size110:
                    return "size-110";
                case MenuSize.Size125:
                    return "size-125";
                case MenuSize.Size150:
                    return "size-150";
                case MenuSize.Size175:
                    return "size-175";
                case MenuSize.Size200:
                    return "size-200";
            }

            return "size-110";
        }
    }
}
