using WpfApplication1.ViewModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class SerialSettingsView : Window
    {
         public SerialSettingsView()
	    {
            InitializeComponent();
            DataContext = new SerialSettingsViewModel();
	    }
    }

}
