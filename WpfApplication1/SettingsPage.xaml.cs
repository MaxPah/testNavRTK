using System;
using System.IO;
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
using System.Threading;
using System.Windows.Threading;
using System.Configuration;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using WpfApplication1.Model;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        #region variables
        //Richtextbox
        FlowDocument mcFlowDoc = new FlowDocument();
        Paragraph para = new Paragraph();
        string link;
        
        //Serial 
        SerialPort sp = new SerialPort();
        string recieved_data;
        ObjectsPorts objports = new ObjectsPorts();
            
        #endregion

        public SettingsPage()
        {
            link = "../../Ports.xml";
            InitializeComponent();
            popupName.ItemsSource = SerialPort.GetPortNames();
            XMLtoSerialPort();
            this.DataContext = this;
        }

        public void XMLtoSerialPort()
        {
            if (File.Exists(link))
            {
                objports = ObjectsPorts.Charger(link);
            }
            else {
                objports = new ObjectsPorts();
            }

            listBox1.ItemsSource = objports;

           // objports.Enregistrer(link);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnOffButton.IsCancel == true)
            {
                if(listBox1.SelectedItem != null)
                {
                  ObjectPort objectselected = (ObjectPort)listBox1.SelectedItem;
                  sp.PortName = objectselected.Name;
                  sp.BaudRate = int.Parse(objectselected.Baudrate);

                }
                else
                {
                    sp.PortName = "COM1";
                    sp.BaudRate = 115200;
                }

               
                if (!sp.IsOpen)
                    sp.Open();

                //Sets button State and Creates function call on data recieved
                sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);
                
                OnOffButton.IsCancel = false;
                OnOffButton.Background = (SolidColorBrush)this.FindResource("secondColor");
                LabelOnOffButton.Content = "On";
                OffRect.Visibility = System.Windows.Visibility.Visible;
                OnRect.Visibility = System.Windows.Visibility.Hidden;

            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    sp.Close();
                    OnOffButton.IsCancel = true;
                    OnOffButton.Background = (SolidColorBrush)this.FindResource("Black");
                    LabelOnOffButton.Content = "Off";
                    OnRect.Visibility = System.Windows.Visibility.Visible;
                    OffRect.Visibility = System.Windows.Visibility.Hidden;
                   
                }
                catch
                {
                }
            }
        }

        #region Recieving

        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            recieved_data = sp.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), recieved_data);
        }
        private void WriteData(string text)
        {
            // Assign the value of the recieved_data to the RichTextBox.
            para.Inlines.Add(text);
            mcFlowDoc.Blocks.Add(para);
            viewDatas.Document = mcFlowDoc;
            viewDatas.ScrollToEnd();
        }

        #endregion

        private void portSettingsValid(object sender, RoutedEventArgs e)
        {
            ObjectsPorts objports;

            PortSettings.IsOpen = false;

            if (File.Exists(link))
            {
                objports = ObjectsPorts.Charger(link);
            }
            else
            {
                File.WriteAllText(link, "<?xml version=\"1.0\" encoding=\"utf-8\"?><ArrayOfObjectPort/>");
                objports = new ObjectsPorts();
            }

            ObjectPort obj = new ObjectPort() {
               Id = objports.MaxId() + 1,
                Name = popupName.Text,
                Baudrate = popupBaud.Text,
                Databits = popupDatabits.Text,
                Stopbit =  popupStopbit.Text,
                Parity = popupParity.Text,
                Handshake = popupHandshake.Text
                };
                     
           objports.Add(obj);
           objports.Enregistrer(link);
           XMLtoSerialPort();

        }

        #region Navigation
        private void goToPopUp(object sender, RoutedEventArgs e)
        {   
            PortSettings.IsOpen = true; 
        }

        private void goToViewData(object sender, MouseButtonEventArgs e)
        {
            if (sp != null)
                if (sp.IsOpen)
                    sp.Close();
            this.NavigationService.Navigate(new ViewDataPage()); 
        }

        private void closePopUp(object sender, RoutedEventArgs e)
        {
            PortSettings.IsOpen = false; 
        }
        #endregion

        private void OnOffButton_MouseEnter(object sender, MouseEventArgs e)
        {
            OnOffButton.Background = (SolidColorBrush)this.FindResource("Transparent");
        }

        private void listboxDeleteItem(object sender, MouseButtonEventArgs e)
        {
          if (listBox1.SelectedItem != null)
            try
            {
                int id = 0;

               var objdelete = (ObjectPort)listBox1.SelectedItem;
                
                    id = objdelete.Id;

                XmlDocument doc = new XmlDocument();
                doc.Load(link);
                XmlNode node = doc.SelectSingleNode("//ObjectPort[@id=" + id + "]");
                node.ParentNode.RemoveChild(node);

                doc.Save("../../Ports.xml");
                XMLtoSerialPort();

            }
            catch
            {
            }
        }
    }
}
