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
//using System.Windows.Threading; 

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class ViewDataPage : Page
    {

         #region variables

        //Richtextbox
        FlowDocument mcFlowDocGGA = new FlowDocument();
        FlowDocument mcFlowDocRMC = new FlowDocument();
        Paragraph paraGGA = new Paragraph();
        Paragraph paraRMC = new Paragraph();

      
        //Serial 
        string recieved_data;
            SerialPort sp;
            #endregion


        public ViewDataPage()
        {
           
            InitializeComponent();
            string[] listPort = SerialPort.GetPortNames();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
                     
                
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           
        }

        private delegate void UpdateUiTextDelegate1(MessageGPGGA objGGA);
        private delegate void UpdateUiTextDelegate2(MessageGPRMC objRMC);

        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
                recieved_data = sp.ReadLine();
                string msg;
                List<Object> list = new List<Object>();
                msg = GPSParsor.splitMessage(recieved_data, list);

            if(list[list.Count-1] != null)
               if (list[list.Count-1].GetType() == typeof(MessageGPGGA))
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate1(WriteDataGGA), list[list.Count-1]);
               else if (list[list.Count - 1].GetType() == typeof(MessageGPRMC))
                   Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate2(WriteDataRMC), list[list.Count - 1]);
           
          
        }
        //private void WriteData(string text)
        //{
        //    // Assign the value of the recieved_data to the RichTextBox.
        //    para.Inlines.Add(text);
        //    mcFlowDoc.Blocks.Add(para);
        //    scrollData.Document = mcFlowDoc;
        //    scrollData.ScrollToEnd();
        //}

        private void WriteDataGGA(MessageGPGGA objGGA)
        {
            GridGGA.DataContext = objGGA;
            GridGGAType.Text = objGGA.type;
            GridGGATime.Text = objGGA.timeUTC.ToString();
            GridGGALat.Text = objGGA.latitude.ToString();
            GridGGALon.Text = objGGA.longitude.ToString();
            GridGGAQual.Text = objGGA.gpsQuality.ToString();
            GridGGANSat.Text = objGGA.nSat.ToString();
            GridGGADil.Text = objGGA.dilution.ToString();
            GridGGAAlt.Text = objGGA.altitude + objGGA.altUnit.ToString();
            GridGGAGeo.Text =  objGGA.geoidal + objGGA.geoUnit.ToString();
            GridGGADGPSUTC.Text = objGGA.dGPSTime.ToString();
        }
        private void WriteDataRMC(MessageGPRMC objRMC)
        {
            GridRMC.DataContext = objRMC;
            GridGGAType_Copy.Text = objRMC.type;
            GridGGATime_Copy.Text = objRMC.timeUTC.ToString();
            GridGGALat_Copy.Text = objRMC.latitude.ToString();
            GridGGALon_Copy.Text = objRMC.longitude.ToString();
            GridGGAQual_Copy.Text = objRMC.status.ToString();
            GridGGANSat_Copy.Text = objRMC.speed.ToString();
            GridGGADil_Copy.Text = objRMC.cap.ToString();
            GridGGAAlt_Copy.Text = objRMC.date.ToString();
            GridGGAGeo_Copy.Text = objRMC.magnetic;
            GridGGADGPSUTC_Copy.Text = objRMC.integrity.ToString();
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
               OnOffButton.Background = (SolidColorBrush)this.FindResource("secondColor");
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
                    OnOffButton.Background = (SolidColorBrush)this.FindResource("Transparent");
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