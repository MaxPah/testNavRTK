﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApplication1.Model;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //var mainView = new DataParsedView();
                var mainView = new ShellView();
                mainView.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
