using System;
using System.Windows.Controls;
using WpfApplication1.Model;
using WpfApplication1.ViewModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace WpfApplication1
{
    public partial class DataParsedView : Window
    {
        public DataParsedView()
        {
            InitializeComponent();
            DataContext = new DataParsedViewModel();
        }
    } 
}