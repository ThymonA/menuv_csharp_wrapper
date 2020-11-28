namespace MenuV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    public class MenuV : BaseScript
    {
        public static string CurrentResourceName { get; set; } = "Menuv";

        private IMenu CurrentMenu { get; set; }

        private Guid CurrentUpdateUUID { get; set; } = Guid.Empty;

        private Guid CurrentItemUpdateUUID { get; set; } = Guid.Empty;

        private bool Loaded { get; set; }

        private IDictionary<Guid, IMenu> Menus { get; } = new Dictionary<Guid, IMenu>();

        private IList<IMenu> ParentMenus { get; } = new List<IMenu>();

        private IDictionary<string, Action<object, Delegate>> NuiCallbacks { get; } = new Dictionary<string, Action<object, Delegate>>();

        private static readonly MenuV Main = new MenuV();

        public static IMenu CreateMenu(
            string title = "MenuV",
            string subtitle = "",
            MenuPosition position = MenuPosition.TopLeft,
            int red = 0,
            int green = 0,
            int blue = 255,
            MenuSize size = MenuSize.Size110,
            string texture = "default",
            string dictionary = "menuv")
        {
            var menu = new Menu(title, subtitle, position, red, green, blue, size, texture, dictionary);

            if (Main.Menus.ContainsKey(menu.UUID))
            {
                Main.Menus[menu.UUID] = menu;
            }
            else
            {
                Main.Menus.Add(menu.UUID, menu);
            }

            return menu;
        }

        public static IMenu GetMenu(string uuid) => GetMenu(new Guid(uuid));

        public static IMenu GetMenu(Guid uuid) => Main.Menus.ContainsKey(uuid) ? Main.Menus[uuid] : null;

        public static void OpenMenu(string uuid)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.OpenMenu(menu, null);
        }

        public static void OpenMenu(string uuid, Delegate callback)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.OpenMenu(menu, callback);
        }

        public static void OpenMenu(Guid uuid)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.OpenMenu(menu, null);
        }

        public static void OpenMenu(Guid uuid, Delegate callback)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.OpenMenu(menu, callback);
        }

        public static void CloseMenu(string uuid)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.CloseMenu(menu, null);
        }

        public static void CloseMenu(string uuid, Delegate callback)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.CloseMenu(menu, callback);
        }

        public static void CloseMenu(Guid uuid)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.CloseMenu(menu, null);
        }

        public static void CloseMenu(Guid uuid, Delegate callback)
        {
            var menu = GetMenu(uuid);

            if (menu.IsNullOrDefault()) { return; }

            Main.CloseMenu(menu, callback);
        }

        public static void OpenMenu(IMenu menu) => Main.OpenMenu(menu, null);

        public static void CloseMenu(IMenu menu) => Main.CloseMenu(menu, null);

        public void OpenMenu(IMenu menu, Delegate callback)
        {
            if (menu.IsNullOrDefault()) { return; }

            var dictionaryLoaded = API.HasStreamedTextureDictLoaded(menu.Dictionary);

            if (!Loaded || !dictionaryLoaded)
            {
                Task.Run(() => MenuVLoadTick(dictionaryLoaded, menu, callback));
                return;
            }

            if (CurrentMenu != null)
            {
                ParentMenus.Add(CurrentMenu);

                CurrentMenu.RemoveOnEvent("update", CurrentUpdateUUID);
                CurrentMenu.RemoveOnEvent("ichange", CurrentItemUpdateUUID);
            }

            CurrentMenu = menu;

            CurrentUpdateUUID = menu.On(
                "update",
                (m, parameters) =>
                {
                    var key = (parameters.Length > 0 ? parameters[0] : "unknown") is string v ? v : string.Empty;

                    if (string.IsNullOrWhiteSpace(key)) { return; }

                    var value = parameters.Length > 1 ? parameters[1] : null;

                    switch (key)
                    {
                        case "title":
                        case "Title":
                            SendNuiMessage(new { action = "UPDATE_TITLE", title = value is string t ? t : string.Empty });
                            break;
                        case "subtitle":
                        case "Subtitle":
                            SendNuiMessage(new { action = "UPDATE_SUBTITLE", subtitle = value is string s ? s : string.Empty });
                            break;
                        case "items":
                        case "Items":
                            SendNuiMessage(new { action = "UPDATE_ITEMS", items = m.Items.ToTable() });
                            break;
                    }
                });

            CurrentItemUpdateUUID = menu.On(
                "ichange",
                (m, parameters) =>
                {
                    SendNuiMessage(new { action = "UPDATE_ITEMS", items = m.Items.ToTable() });
                });

            SendNuiMessage(new { action = "OPEN_MENU", menu = menu.ToTable() });

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
        }

        public void CloseMenu(IMenu menu, Delegate callback)
        {
            if (menu.IsNullOrDefault()) { return; }

            if (!Loaded)
            {
                Task.Run(() => MenuVCloseTick(menu, callback));
                return;
            }

            if (CurrentMenu.IsNullOrDefault() || CurrentMenu.UUID != menu.UUID)
            {
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
                return;
            }

            var uuid = CurrentMenu.UUID.ToString();

            CurrentMenu.RemoveOnEvent("update", CurrentUpdateUUID);
            CurrentMenu.RemoveOnEvent("ichange", CurrentItemUpdateUUID);
            CurrentMenu.Trigger("close", GetMenu(uuid));
            CurrentMenu = null;

            if (ParentMenus.Count <= 0)
            {
                SendNuiMessage(new { action = "CLOSE_MENU", uuid = uuid });
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
                return;
            }

            var prev_menu = ParentMenus.Last();

            if (prev_menu.IsNullOrDefault())
            {
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
                return;
            }

            ParentMenus.RemoveAt(ParentMenus.Count - 1);

            OpenMenu(prev_menu, callback);
        }

        public void CloseAll(Delegate callback)
        {
            if (!Loaded)
            {
                Task.Run(() => MenuVCloseAllTick(callback));
                return;
            }

            if (CurrentMenu.IsNullOrDefault())
            {
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
                return;
            }

            var uuid = CurrentMenu.UUID.ToString();

            CurrentMenu.RemoveOnEvent("update", CurrentUpdateUUID);
            CurrentMenu.RemoveOnEvent("ichange", CurrentItemUpdateUUID);
            CurrentMenu.Trigger("close", GetMenu(uuid));

            SendNuiMessage(new { action = "CLOSE_MENU", uuid = uuid });

            CurrentMenu = null;
            ParentMenus.Clear();

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
        }

        private async Task MenuVLoadTick(bool dictionaryLoaded, IMenu menu, Delegate callback)
        {
            do { await Delay(0); } while (!Loaded);

            if (!dictionaryLoaded)
            {
                do { await Delay(10); } while (!API.HasStreamedTextureDictLoaded(menu.Dictionary));
            }

            await Task.Run(() => OpenMenu(menu, callback));
        }

        private async Task MenuVCloseTick(IMenu menu, Delegate callback)
        {
            do { await Delay(0); } while (!Loaded);

            await Task.Run(() => CloseMenu(menu, callback));
        }

        private async Task MenuVCloseAllTick(Delegate callback)
        {
            do { await Delay(0); } while (!Loaded);

            await Task.Run(() => CloseAll(callback));
        }

        private void SendNuiMessage(object input)
        {
            Exports["menuv"].SendNUIMessage(input);
        }

        private void RegisterNuiCallback(string name, Action<object, Delegate> callback)
        {
            name = string.IsNullOrWhiteSpace(name) ? "unknown" : name;

            if (NuiCallbacks.IsNullOrDefault() || NuiCallbacks.ContainsKey(name)) { return; }

            NuiCallbacks.Add(name, callback);
        }

        private void NuiCallback(string name, dynamic info, Delegate callback)
        {
            name = string.IsNullOrWhiteSpace(name) ? "unknown" : name;

            if (NuiCallbacks.IsNullOrDefault() || !NuiCallbacks.ContainsKey(name)) { return; }

            Task.Run(() => NuiCallbacks[name](info, callback));
        }

        private void NuiCallbackOpen(dynamic info, Delegate callback)
        {
            var rawUUID = (info?.uuid || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.uuid;
            var uuid = new Guid(rawUUID is string v ? v : "00000000-0000-0000-0000-000000000000");

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke("ok"); }
            if (CurrentMenu.IsNullOrDefault() || CurrentMenu.UUID == uuid) { return; }
            if (ParentMenus.Any(parentMenu => parentMenu.UUID == uuid)) { return; }

            var current_uuid = CurrentMenu.UUID.ToString();

            CurrentMenu.RemoveOnEvent("update", CurrentUpdateUUID);
            CurrentMenu.RemoveOnEvent("ichange", CurrentItemUpdateUUID);
            CurrentMenu.Trigger("close", GetMenu(current_uuid));
            CurrentMenu = null;
            ParentMenus.Clear();
        }

        private void NuiCallbackSubmit(dynamic info, Delegate callback)
        {
            var rawUUID = (info?.uuid || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.uuid;
            var uuid = new Guid(rawUUID is string v ? v : "00000000-0000-0000-0000-000000000000");

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke("ok"); }
            if (CurrentMenu.IsNullOrDefault()) { return; }

            var selectedItem = CurrentMenu.Items
                .FirstOrDefault(i => i.UUID == uuid);

            if (selectedItem.IsNullOrDefault()) { return; }

            switch (selectedItem.__type)
            {
                case "confirm":
                case "checkbox":
                    selectedItem.Value = (info?.value || null).Ensure(false);
                    break;
                case "range":
                    var rangeItem = selectedItem is IRangeItem v2 ? v2 : null;

                    selectedItem.Value = (info?.value || null).Ensure(rangeItem.IsNullOrDefault() ? 0 : rangeItem.Min);
                    break;
                case "slider":
                    selectedItem.Value = (info?.value || null).Ensure(0);
                    break;
            }

            CurrentMenu.Trigger("select", CurrentMenu, selectedItem);

            switch (selectedItem.__type)
            {
                case "button":
                case "menu":
                    selectedItem.Trigger("select", selectedItem);
                    break;
                case "range":
                    selectedItem.Trigger("select", selectedItem, selectedItem.Value);
                    break;
                case "slider":
                    var sliderItem = selectedItem is ISliderItem v2 ? v2 : null;

                    if (sliderItem.IsNullOrDefault()) { return; }

                    var selectedValue = sliderItem.Value.Ensure(0);

                    if (sliderItem.Values.Count <= 0 || sliderItem.Values.Count - 1 < selectedValue) { return; }

                    var option = sliderItem.Values[selectedValue];

                    if (option.IsNullOrDefault()) { return; }

                    selectedItem.Trigger("select", selectedItem, option.Value);
                    break;
            }
        }

        private void NuiCallbackOpened(dynamic info, Delegate callback)
        {
            var rawUUID = (info?.uuid || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.uuid;
            var uuid = new Guid(rawUUID is string v ? v : "00000000-0000-0000-0000-000000000000");

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke("ok"); }
            if (CurrentMenu.IsNullOrDefault() || CurrentMenu.UUID != uuid) { return; }

            CurrentMenu.Trigger("open", CurrentMenu);
        }

        private void NuiCallbackClose(dynamic info, Delegate callback)
        {
            var rawUUID = (info?.uuid || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.uuid;
            var uuid = new Guid(rawUUID is string v ? v : "00000000-0000-0000-0000-000000000000");

            if (CurrentMenu.IsNullOrDefault() || CurrentMenu.UUID != uuid)
            {
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke("ok"); }
                return;
            }

            var current_uuid = CurrentMenu.UUID.ToString();

            CurrentMenu.RemoveOnEvent("update", CurrentUpdateUUID);
            CurrentMenu.RemoveOnEvent("ichange", CurrentItemUpdateUUID);
            CurrentMenu.Trigger("close", GetMenu(current_uuid));
            CurrentMenu = null;

            if (ParentMenus.Count <= 0)
            {
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke("ok"); }
                return;
            }

            var prev_menu = ParentMenus.Last();

            if (prev_menu.IsNullOrDefault())
            {
                if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
                return;
            }

            ParentMenus.RemoveAt(ParentMenus.Count - 1);

            OpenMenu(
                prev_menu,
                (Action)(() =>
                {
                    callback.DynamicInvoke("ok");
                }));
        }

        private void NuiCallbackSwitch(dynamic info, Delegate callback)
        {
            var rawPrevUUID = (info?.prev || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.prev;
            var rawNextUUID = (info?.next || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.next;
            var prev_uuid = new Guid(rawPrevUUID is string v ? v : "00000000-0000-0000-0000-000000000000");
            var next_uuid = new Guid(rawNextUUID is string v2 ? v2 : "00000000-0000-0000-0000-000000000000");

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke(); }
            if (CurrentMenu.IsNullOrDefault()) { return; }

            var prev_item = CurrentMenu.Items.FirstOrDefault(i => i.UUID == prev_uuid);
            var next_item = CurrentMenu.Items.FirstOrDefault(i => i.UUID == next_uuid);

            if (!prev_item.IsNullOrDefault()) { prev_item.Trigger("leave", prev_item); }
            if (!next_item.IsNullOrDefault()) { next_item.Trigger("enter", next_item); }

            if (!prev_item.IsNullOrDefault() && !next_item.IsNullOrDefault())
            {
                CurrentMenu.Trigger("switch", CurrentMenu, next_item, prev_item);
            }
        }

        private void NuiCallbackUpdate(dynamic info, Delegate callback)
        {
            var rawUUID = (info?.uuid || null).IsNullOrDefault() ? "00000000-0000-0000-0000-000000000000" : info?.uuid;
            var uuid = new Guid(rawUUID is string v ? v : "00000000-0000-0000-0000-000000000000");

            if (!callback.IsNullOrDefault()) { callback.DynamicInvoke("ok"); }
            if (CurrentMenu.IsNullOrDefault()) { return; }

            var selectedItem = CurrentMenu.Items.FirstOrDefault(i => i.UUID == uuid);

            if (selectedItem.IsNullOrDefault()) { return; }

            var newValue = (object)null;
            var oldValue = (object)null;

            switch (selectedItem.__type)
            {
                case "confirm":
                case "checkbox":
                    newValue = (info?.now || null).Ensure(false);
                    oldValue = (info?.prev || null).Ensure(false);
                    break;
                case "range":
                    var rangeItem = selectedItem is IRangeItem v2 ? v2 : null;

                    newValue = (info?.now || null).Ensure(rangeItem.IsNullOrDefault() ? 0 : rangeItem.Min);
                    oldValue = (info?.prev || null).Ensure(rangeItem.IsNullOrDefault() ? 0 : rangeItem.Min);
                    break;
                case "slider":
                    newValue = (info?.now || null).Ensure(0);
                    oldValue = (info?.prev || null).Ensure(0);
                    break;
            }

            if (selectedItem.__type == "button" || selectedItem.__type == "menu" || selectedItem.__type == "label") { return; }

            CurrentMenu.Trigger("update", CurrentMenu, selectedItem, newValue, oldValue);
            selectedItem.Trigger("change", selectedItem, newValue, oldValue);

            if (selectedItem.SaveOnUpdate)
            {
                selectedItem.Value = newValue;

                switch (selectedItem.__type)
                {
                    case "range":
                        selectedItem.Trigger("select", selectedItem, selectedItem.Value);
                        break;
                    case "slider":
                        var sliderItem = selectedItem is ISliderItem v2 ? v2 : null;

                        if (sliderItem.IsNullOrDefault()) { return; }

                        var selectedValue = sliderItem.Value.Ensure(0);

                        if (sliderItem.Values.Count <= 0 || sliderItem.Values.Count - 1 < selectedValue) { return; }

                        var option = sliderItem.Values[selectedValue];

                        if (option.IsNullOrDefault()) { return; }

                        selectedItem.Trigger("select", selectedItem, option.Value);
                        break;
                }
            }
        }

        private void MenuVLoaded()
        {
            Loaded = true;
        }

        public MenuV()
        {
            Exports.Add("NUICallback", new Action<string, object, Delegate>(NuiCallback));

            Exports["menuv"].IsLoaded(new Action(MenuVLoaded));

            RegisterNuiCallback("open", NuiCallbackOpen);
            RegisterNuiCallback("opened", NuiCallbackOpened);
            RegisterNuiCallback("submit", NuiCallbackSubmit);
            RegisterNuiCallback("close", NuiCallbackClose);
            RegisterNuiCallback("switch", NuiCallbackSwitch);
            RegisterNuiCallback("update", NuiCallbackUpdate);

            CurrentResourceName = API.GetCurrentResourceName();
        }
    }
}
