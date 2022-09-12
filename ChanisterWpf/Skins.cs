namespace ChanisterWpf
{
    /*class Skins
    {
		void skinsInit()
		{
			foreach (MenuItem item in Blue_board.Items)
			{
				item.IsChecked = false;
				if (NameToSkin((string)item.Header) == Properties.Settings.Default.blueSkin)
				{
					item.IsChecked = true;
				}
			}
			foreach (MenuItem item in Red_board.Items)
			{
				item.IsChecked = false;
				if (NameToSkin((string)item.Header) == Properties.Settings.Default.redSkin)
				{
					item.IsChecked = true;
				}
			}
		}
		void ChangeTheme(object sender, RoutedEventArgs e)
		{
			MenuItem menu = sender as MenuItem;
			if (!menu.IsChecked)
			{
				menu.IsChecked = true;
			}
			MenuItem parentMenu = menu.Parent as MenuItem;
			foreach (MenuItem item in parentMenu.Items)
			{
				if (item != menu)
				{
					item.IsChecked = false;
				}
			}
			if (parentMenu.Name == "Blue_board")
			{
				Properties.Settings.Default.blueSkin = NameToSkin(menu.Header.ToString());
			}
			if (parentMenu.Name == "Red_board")
			{
				Properties.Settings.Default.redSkin = NameToSkin(menu.Header.ToString());
			}
		}
		void ApplyTheme(string themeName)
		{
			(App.Current as App).ChangeSkin(NameToSkin(themeName));
		}
		public Skin NameToSkin(string name)
		{
			switch (name)
			{
				case "Classic _blue":
					return Skin.Classic_blue;
				case "Classic _red":
					return Skin.Classic_red;
				case "Sakura _purple":
					return Skin.Sakura_purple;
				case "_White is tight":
					return Skin.White_is_tight;
				default:
					return Skin.White_is_tight;
			}
		}
	}*/
}
