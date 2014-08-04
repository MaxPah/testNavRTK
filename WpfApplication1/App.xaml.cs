using System;
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
        private void InitializeViewService(Frame rootFrame)
        {
            ViewService.Service.NavigationService.Initialized(rootFrame);
            ViewService.Service.RegisterView("DataParsedView", typeof(DataParsedView));
            ViewService.Service.RegisterView("SerialSettingsView", typeof(SerialSettingsView));
        }
    }
}
