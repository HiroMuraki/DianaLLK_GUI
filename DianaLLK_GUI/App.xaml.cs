﻿using System;
using System.Windows;

namespace DianaLLK_GUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static readonly ResourceDictionary ImageDict = new ResourceDictionary() {
            Source = new Uri("Resources/Images.xaml", UriKind.Relative)
        };
        public static readonly ResourceDictionary ColorDict = new ResourceDictionary() {
            Source = new Uri("Resources/PresetColors.xaml", UriKind.Relative)
        };

        private void Application_Startup(object sender, StartupEventArgs e) {
            MainWindow window = new MainWindow();
            window.Show();
        }
    }
}
