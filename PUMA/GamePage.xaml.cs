using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PUMA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
		readonly Game1 _game;

		public GamePage()
        {
            this.InitializeComponent();

			// Create the game.
			var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel1);
            this.DataContext = _game;
        }

        private void StartPauseAnimationButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = (AppBarToggleButton)sender;
            button.Content = new SymbolIcon(Symbol.Pause);
            _game.IsAnimated = true;
        }

        private void StartPauseAnimationButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = (AppBarToggleButton)sender;
            button.Content = new SymbolIcon(Symbol.Play);
            _game.IsAnimated = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _game.ResetScene();
        }

    }
}
