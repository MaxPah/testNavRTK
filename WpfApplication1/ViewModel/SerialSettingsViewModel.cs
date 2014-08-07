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
        #region Variables

            private Queue<string> gpsTrame;
            private string link;
            private string[] portName;
            private string onOffButton = "Off";
            private string[] enumBauds = { "115200", "4800", "9600"};
            private string[] enumDatabits = { "8", "7", "6", "5" };
            private string[] enumStopbit = { "One", "OntPointFive", "Two", "None" };
            private string[] enumParity ={ "None", "Even", "Mark", "Odd", "Space"};
            private string[] enumHandshake = { "None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff" };
            private bool _isOpen;
            private string selectedName;
            private string selectedBauds;
            private string selectedDatabits;
            private string selectedStopbits;
            private string selectedParity;
            private string selectedHandshake;
            private RelayCommand openPopUp;
            private RelayCommand closePopUp;
            private RelayCommand validationNewPort;
            private RelayCommand goToView;
            private RelayCommand stop;
            private RelayCommand listBoxDeleteItem;
            SerialPort sp = new SerialPort();
            string recieved_data;
            ObjectsPorts objports = new ObjectsPorts();
            private ObjectPort selectedObjectPort;
       
        #endregion Variables

        #region Accessors

            public bool IsOpen
            {
                get { return _isOpen; }
                set
                {
                    if (_isOpen == value) return;
                    _isOpen = value;
                    OnPropertyChanged("IsOpen");
                }
            }
            public string[] EnumBauds
            {
                get { return enumBauds; }
                set
                {
                    if (enumBauds == value) return;
                    enumBauds = value;
                    OnPropertyChanged("EnumBauds");
                }
            }
            public string[] EnumDatabits
            {
                get { return enumDatabits; }
                set
                {
                    if (enumDatabits == value) return;
                    enumDatabits = value;
                    OnPropertyChanged("EnumDatabits");
                }
            }
            public string[] EnumHandshake
            {
                get { return enumHandshake; }
                set
                {
                    if (enumHandshake == value) return;
                    enumHandshake = value;
                    OnPropertyChanged("EnumHandshake");
                }
            }
            public string[] EnumParity
            {
                get { return enumParity; }
                set
                {
                    if (enumParity == value) return;
                    enumParity = value;
                    OnPropertyChanged("EnumParity");
                }
            }
            public string[] EnumStopbit
            {
                get { return enumStopbit; }
                set
                {
                    if (enumStopbit == value) return;
                    enumStopbit = value;
                    OnPropertyChanged("EnumStopbit");
                }
            }
            public ICommand OpenPopUp
            {
                get
                {
                    if (openPopUp == null)
                    {
                        openPopUp = new RelayCommand(ExecuteOpenPopUp, CanOpenPopUp);
                    }
                    return openPopUp;
                }
            }
            public ICommand ClosePopUp
            {
                get
                {
                    if (closePopUp == null)
                    {
                        closePopUp = new RelayCommand(ExecuteClosePopUp, CanClosePopUp);
                    }
                    return closePopUp;
                }
            }
            public ICommand ValidationNewPort
            {
                get
                {
                    if (validationNewPort == null)
                    {
                        validationNewPort = new RelayCommand(ExecuteValidationNewPort, CanValidationNewPort);
                    }
                    return validationNewPort;
                }
            }
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
            public ICommand ListBoxDeleteItem
            {
                get
                {
                    if (listBoxDeleteItem == null)
                    {
                        listBoxDeleteItem = new RelayCommand(ExecuteListBoxDeleteItem, CanListBoxDeleteItem);
                    }
                    return listBoxDeleteItem;
                }
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
            public string OnOffButton
            {
                get
                { return onOffButton; }
                set
                {
                    onOffButton = value;
                    OnPropertyChanged("OnOffButton");
                }
            }
            public string[] PortName
            {
                get
                {
                    portName = SerialPort.GetPortNames();
                    return portName;
                }
                set
                {
                    portName = value;
                    OnPropertyChanged("PortName");
                }
            }
            public string SelectedName
            {
                get
                {
                    return selectedName;
                }
                set
                {
                    selectedName = value;
                    OnPropertyChanged("SelectedName");
                }
            }
            public string SelectedBauds
            {
                get
                {
                    return selectedBauds;
                }
                set
                {
                    selectedBauds = value;
                    OnPropertyChanged("SelectedBauds");
                }
            }
            public string SelectedDatabits
            {
                get
                {
                    return selectedDatabits;
                }
                set
                {
                    selectedDatabits = value;
                    OnPropertyChanged("SelectedDatabits");
                }
            }
            public string SelectedStopbits
            {
                get
                {
                    return selectedStopbits;
                }
                set
                {
                    selectedStopbits = value;
                    OnPropertyChanged("SelectedStopbits");
                }
            }
            public string SelectedParity
            {
                get
                {
                    return selectedParity;
                }
                set
                {
                    selectedParity = value;
                    OnPropertyChanged("SelectedParity");
                }
            }
            public string SelectedHandshake
            {
                get
                {
                    return selectedHandshake;
                }
                set
                {
                    selectedHandshake = value;
                    OnPropertyChanged("SelectedHandshake");
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
                }
            }

        #endregion Accessors

        #region Constructor
            public SerialSettingsViewModel()
            {
                link = "../../Ports.xml";
                portName = SerialPort.GetPortNames();
                gpsTrame = new Queue<string>();

                XMLtoSerialPort();

        }
        #endregion Constructor

        #region Methods

            private bool CanListBoxDeleteItem() { return true;}
            private void ExecuteListBoxDeleteItem()
            {
            try
                {
                    int id=0; 

                    if( selectedObjectPort != null)
                         id = selectedObjectPort.Id;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(link);
                    XmlNode node = doc.SelectSingleNode("//ObjectPort[@id=" + id + "]");
                if (node !=null)
                    node.ParentNode.RemoveChild(node);
                    
                    doc.Save("../../Ports.xml");
                    XMLtoSerialPort();
                    OnPropertyChanged("ObjPorts");
                    OnPropertyChanged("ListBoxDeleteItem");
                }
                catch (Exception listboxDeleteItemException)
                {
                    Console.WriteLine(listboxDeleteItemException.Message);
                }

            }
            private void ExecuteGoToView()
            {
                sp.Close();
                Console.WriteLine("ViewClicked");
                Frame ShellFrame = new Frame();
                ShellFrame.Navigate(new SerialSettingsView());

                
            }
            private bool CanGoToView()
            { return true; }
            private bool CanValidationNewPort()
            { return true; }
            private void ExecuteValidationNewPort()
            {
                _isOpen = false;
                OnPropertyChanged("IsOpen");

                if (File.Exists(link))
                {
                    objports = ObjectsPorts.Charger(link);
                }
                else
                {
                    File.WriteAllText(link, "<?xml version=\"1.0\" encoding=\"utf-8\"?><ArrayOfObjectPort/>");
                    objports = new ObjectsPorts();
                }

                 ObjectPort obj = new ObjectPort()
                 {
                     Id = objports.MaxId() + 1,
                     Name = selectedName,
                     Baudrate = selectedBauds,
                     Databits = selectedDatabits,
                     Stopbit = selectedStopbits,
                     Parity = selectedParity,
                     Handshake = selectedHandshake,
                 };
                 
                objports.Add(obj);
                objports.Enregistrer(link);
                OnPropertyChanged("ObjPorts");
            }
            private void ExecuteStop()
            {
                if (onOffButton == "On")
                {
                    sp.Close();
                    onOffButton = "Off";
                }
                else
                {
                    if (selectedObjectPort == null)
                    {
                        sp.PortName = "COM1";
                        sp.BaudRate = 115200;
                    }
                    else
                    {
                        sp.PortName = selectedObjectPort.Name;
                        sp.BaudRate = int.Parse(selectedObjectPort.Baudrate);
                    }


                    if (!sp.IsOpen)
                        sp.Open();

                    //Sets button State and Creates function call on data recieved
                    sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);

                    onOffButton = "On";
                    if (!sp.IsOpen)
                        sp.Open();
                }
                OnPropertyChanged("OnOffButton");
            }
            private bool CanStop()
            {
                return true;
            }
            private bool CanOpenPopUp()
            { return true; }
            private void ExecuteOpenPopUp()
            {
                _isOpen = true;
                OnPropertyChanged("IsOpen");
            }
            private bool CanClosePopUp()
            { return true; }
            private void ExecuteClosePopUp()
            {
                _isOpen = false;
                OnPropertyChanged("IsOpen");
            }

        #endregion Methods

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

        #region Recieving

        private delegate void UpdateUiTextDelegate(Queue<string> gpsTrame);
        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            

            // Collecting the characters received to our 'buffer' (string).
            recieved_data = sp.ReadLine();

            if (gpsTrame != null)
            {
                if (gpsTrame.Count() > 22)
                    gpsTrame.Dequeue();

                if (recieved_data != null)
                    gpsTrame.Enqueue(recieved_data);
                OnPropertyChanged("GpsTrame");
            }

           
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), gpsTrame);
        }

        private void WriteData(Queue<string> gpsTrame)
        {
            OnPropertyChanged("GpsTrame");
        }
        
        #endregion

    
        #region Navigation
         #endregion

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
        #endregion INotifyPropertyChanged Members
    }
}
