﻿using System.Windows;
using System.Windows.Input;
using Assembly.Helpers;
using Assembly.Helpers.Native;

namespace Assembly.Metro.Dialogs.ControlDialogs
{
	/// <summary>
	///     Interaction logic for About.xaml
	/// </summary>
	public partial class About
	{
		public About()
		{
			InitializeComponent();
			DwmDropShadow.DropShadowToWindow(this);

			string version = VariousFunctions.GetApplicationVersion();
			lblTitle.Text = lblTitle.Text.Replace("{version}", version);
		}

		private void btnActionClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}