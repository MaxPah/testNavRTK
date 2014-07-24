using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using WpfApplication1.Model;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class ViewDataPage : Page
    {

         #region variables
  
        //Richtextbox
        FlowDocument mcFlowDoc = new FlowDocument();
        Paragraph para = new Paragraph();
       

        //Serial 
        string recieved_data;
            SerialPort sp;
            #endregion


        public ViewDataPage()
        {
           
            InitializeComponent();
            string[] listPort = SerialPort.GetPortNames();
            
                
        }

        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            string msg;
            // Collecting the characters received to our 'buffer' (string).
            recieved_data = sp.ReadLine();
            List<Object> list = new List<Object>();
           
            msg = GPSParsor.splitMessage(recieved_data, list);

            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), msg);
        }
        private void WriteData(string text)
        {
            // Assign the value of the recieved_data to the RichTextBox.
            para.Inlines.Add(text);
            mcFlowDoc.Blocks.Add(para);
            scrollData.Document = mcFlowDoc;
            scrollData.ScrollToEnd();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           if ( OnOffButton.IsCancel == true)
           {
               sp = new SerialPort("COM1", 115200);

               if (!sp.IsOpen)
                   sp.Open();



               sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);

               OnOffButton.IsCancel = false;
               OnOffButton.Background = (SolidColorBrush)this.FindResource("Transparent");
               LabelOnOffButton.Content = "On";
                OffRect.Visibility = System.Windows.Visibility.Visible;
                OnRect.Visibility = System.Windows.Visibility.Hidden;
           }
           else
           {
               try
               {
                   sp.Close();
                   OnOffButton.IsCancel = true;
                    OnOffButton.Background = (SolidColorBrush)this.FindResource("secondColor");
                    LabelOnOffButton.Content = "Off";
                    OnRect.Visibility = System.Windows.Visibility.Visible;
                    OffRect.Visibility = System.Windows.Visibility.Hidden;
                   

               }
               catch
               {
               }
           }
        }

        private void goToConnexion(object sender, MouseButtonEventArgs e)
        {
            if (sp != null)
                if (sp.IsOpen)
                    sp.Close();

            this.NavigationService.Navigate(new SettingsPage());
        }

    }
}