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
using System.ComponentModel;
using WpfApplication1.Helper;

namespace WpfApplication1.ViewModel
{
    class SerialSettingsViewModel : INotifyPropertyChanged
    {
        #region variables
        //Richtextbox
        private Queue<string> gpsTrame;
        private string link;

        //Serial 
        SerialPort sp = new SerialPort();
        string recieved_data;
        ObjectsPorts objports = new ObjectsPorts();
        private ObjectPort selectedObjectPort;

        #endregion

        private RelayCommand goToView;
        public ICommand GoToView
        {
            get
            {
                if (goToView == null)
                {
                    goToView = new RelayCommand(ExecuteGoToView, CanGoToView);
                }
                return goToView;
            }
        }
        private RelayCommand stop;
        public ICommand Stop
        {
            get
            {
                if (stop == null)
                {
                    stop = new RelayCommand(ExecuteStop, CanStop);
                }
                return stop;
            }
        }

        public SerialSettingsViewModel()
        {
            link = "../../Ports.xml";
            string[] portName;
            portName = SerialPort.GetPortNames();
            gpsTrame = new Queue<string>();

            XMLtoSerialPort();
            sp.PortName = "COM1";
            sp.BaudRate = 115200;

            
            //GoToView = new CommandImpl(MyCommandExecute, MyCommandCanExecute);

            if (!sp.IsOpen)
                sp.Open();

            //Sets button State and Creates function call on data recieved
            sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);
        }

        private void ExecuteGoToView()
        {
            Console.WriteLine("ViewClicked");
            //(Frame)NavigationService.Navigate(new DataParsedView());
        }

        private bool CanGoToView()
        {
            return true;
        }
        private void ExecuteStop()
        {
            sp.Close();
        }

        private bool CanStop()
        {
            return true;
        }

        public void XMLtoSerialPort()
        {
            if (File.Exists(link))
            {
                objports = ObjectsPorts.Charger(link);
            }
            else
            {
                objports = new ObjectsPorts();
            }
            objports.Enregistrer(link);
        }

        public ObjectsPorts ObjPorts
        {
            get
            { return objports; }
            set
            {
                objports = value;
                OnPropertyChanged("ObjPorts");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            /*  if(listBox1.SelectedItem != null)
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
          */
            //Sets button State and Creates function call on data recieved
            sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);
        }
        #region Recieving

        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            recieved_data = sp.ReadLine();
            if (recieved_data !=null)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), recieved_data);
        }
        private void WriteData(string text)
        {
            if (gpsTrame != null)
                if (gpsTrame.Count() > 12)
                    gpsTrame.Clear();
            if (text != null)
                gpsTrame.Enqueue(text);
            OnPropertyChanged("GpsTrame");
        }

        public string GpsTrame
        {
            get
            {
                string alltrames = "";
                foreach (string o in gpsTrame)
                    alltrames += o.ToString();
                return alltrames;
            }
            set
            {
                gpsTrame.Enqueue(value);
                OnPropertyChanged("GpsTrame");

                Console.WriteLine(gpsTrame);
            }
        }

        #endregion
        /*
        private void portSettingsValid(object sender, RoutedEventArgs e)
        {
            ObjectsPorts objports;

            //PortSettings.IsOpen = false;

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
                //Name = popupName.Text,
                //Baudrate = popupBaud.Text,
                //Databits = popupDatabits.Text,
                //Stopbit =  popupStopbit.Text,
                //Parity = popupParity.Text,
                //Handshake = popupHandshake.Text
                };
                     
           objports.Add(obj);
           objports.Enregistrer(link);
           XMLtoSerialPort();

        }

        #region Navigation
        private void goToPopUp(object sender, RoutedEventArgs e)
        {   
            //PortSettings.IsOpen = true; 
        }

       /* private void goToViewData(object sender, MouseButtonEventArgs e)
        {
            if (sp != null)
                if (sp.IsOpen)
                    sp.Close();
            NavigationService.Navigate(new DataParsedView()); 
        }*/

        /* private void closePopUp(object sender, RoutedEventArgs e)
         {
             PortSettings.IsOpen = false; 
         }
         *
         #endregion
  */
        private void OnOffButton_MouseEnter(object sender, MouseEventArgs e)
        {
            //OnOffButton.Background = (SolidColorBrush)Application.Current.FindResource("Transparent");
        }

        private void listboxDeleteItem(object sender, MouseButtonEventArgs e)
        {
            if (true)//listBox1.SelectedItem != null)
                try
                {
                    int id = 0;

                    // var objdelete = (ObjectPort)listBox1.SelectedItem;

                    //  id = objdelete.Id;

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

        public ObjectPort SelectedPort
        {
            get { return selectedObjectPort; }
            set
            {
                if (selectedObjectPort != value)
                {
                    selectedObjectPort = value;
                    OnPropertyChanged("SelectedPort");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
