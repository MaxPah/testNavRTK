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
        #region FIELDS
            private Queue<string> gpsTrame; // Used to stock ALL trames GPS Message
            string recieved_data; // Used to stock one trame of the GPS Message
            private string link; // Used to stock the path for Ports.xml
            ObjectsPorts objports = new ObjectsPorts();//Used to be the list of ObjectPort from Ports.xml
            SerialPort sp = new SerialPort(); // Used to be THE SerialPort of NavRTK
            private string[] portName;//Used to stock all ports names availables
            private string onOffButton = "Read";// Used to stock the content of the switch button
            private string[] enumBauds = { "115200", "4800", "9600" };// Used to stock all bauds availables
            private string[] enumDatabits = { "8", "7", "6", "5" };// Used to stock all bits availables
            private string[] enumStopbit = { "One", "OntPointFive", "Two", "None" };// Used to stock all the stopbits availables
            private string[] enumParity = { "None", "Even", "Mark", "Odd", "Space" };// Used to stock all parity availables
            private string[] enumHandshake = { "None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff" };// Used to stock all handshake available
            private string selectedName; // Used to stock the name selected in the popup window
            private string selectedBauds;// Used to stock the baud selected in the popup window
            private string selectedDatabits;// Used to stock the databits selected in the popup window
            private string selectedStopbits;// Used to stock the stopbit selected in the popup window 
            private string selectedParity;// Used to stock the parity selected in the popup window
            private string selectedHandshake;// Used to stock the handshake selected in the popup window
            private ObjectPort selectedObjectPort;// Used to be the ObjectPort selected for recieved data
            private bool isOpen; // Used to determind if the popup "new port" is hidden or not
            private SolidColorBrush defaultItemColor = new SolidColorBrush(Colors.DeepSkyBlue);
        #endregion FIELDS

        #region PROPERTIES
            /// <summary>
            /// Determine if the PopUp is open or not
            /// </summary>
            public bool IsOpen
            {
                get { return isOpen; }
                set
                {
                    if (isOpen == value) return;
                    isOpen = value;
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
            public SolidColorBrush DefaultItemColor
            {
                get { return defaultItemColor; }
                set
                {
                    defaultItemColor = value;
                    OnPropertyChanged("DefaultItemColor");
                }
            }
        #endregion PROPERTIES

        #region COMMANDS
            /// <summary>
            /// Open the popup "new port settings"
            /// </summary>
            private RelayCommand openPopUp;
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

            /// <summary>
            /// Close the popup "new port settings"
            /// </summary>
            private RelayCommand closePopUp;
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

            /// <summary>
            /// Save the new port to Ports.xml
            /// </summary>
            private RelayCommand validationNewPort;
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

            /// <summary>
            /// Go to the other page
            /// </summary>
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

            /// <summary>
            /// Stop receiving data
            /// </summary>
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

            /// <summary>
            ///  Delete an ObjectPort from the list and Ports.xml
            /// </summary>
            private RelayCommand listBoxDeleteItem;
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

            /// <summary>
            ///  Delete an ObjectPort from the list and Ports.xml
            /// </summary>
            private RelayCommand listBoxDefaultItem;
            public ICommand ListBoxDefaultItem
            {
                get
                {
                    if (listBoxDefaultItem == null)
                    {
                        listBoxDefaultItem = new RelayCommand(ExecuteListBoxDefaultItem, CanListBoxDefaultItem);
                    }
                    return listBoxDefaultItem;
                }
            }
            #endregion COMMANDS

        #region CONSTRUCTOR
            public SerialSettingsViewModel()
            {
                link = "../../Ports.xml";
                portName = SerialPort.GetPortNames();
                gpsTrame = new Queue<string>();
                XMLtoSerialPort();
             }
        #endregion CONSTRUCTOR

        #region COMMANDS IMPLEMENTATION
            private bool CanListBoxDeleteItem() { return true; }
            private void ExecuteListBoxDeleteItem()
            {
                try
                {
                    int id = 0;

                    if (selectedObjectPort != null)
                        id = selectedObjectPort.Id;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(link);
                    XmlNode node = doc.SelectSingleNode("//ObjectPort[@id=" + id + "]");
                    if (node != null)
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
            private bool CanListBoxDefaultItem() { return true; }
            private void ExecuteListBoxDefaultItem()
            {
                try
                {
                    int id=-1 ;

                    if (selectedObjectPort != null)
                        id = selectedObjectPort.Id;
                    if (File.Exists(link))
                    {
                        objports = ObjectsPorts.Charger(link);
                    }
                    objports.DefaultSwap(id);
                    objports.Enregistrer(link);
                    XMLtoSerialPort();

                    OnPropertyChanged("ObjPorts");
                    OnPropertyChanged("ListBoxDeleteItem");
                }
                catch (Exception listboxDeleteItemException)
                {
                    Console.WriteLine(listboxDeleteItemException.Message);
                }

            }
            private bool CanValidationNewPort()
            { return true; }
            private void ExecuteValidationNewPort()
            {
                isOpen = false;
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
                if (onOffButton == "Break")
                {
                    sp.Close();
                    onOffButton = "Read";
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

                    if (sp != null)
                        if (!sp.IsOpen)
                            sp.Open();

                    //Sets button State and Creates function call on data recieved
                    sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);

                    onOffButton = "Break";
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
                isOpen = true;
                OnPropertyChanged("IsOpen");
            }
            private bool CanClosePopUp()
            { return true; }
            private void ExecuteClosePopUp()
            {
                isOpen = false;
                OnPropertyChanged("IsOpen");
            }
        #endregion COMMANDS IMPLEMENTATION           

        #region RECIEVING
            private void Recieve(object sender, SerialDataReceivedEventArgs e)
            {
                // Collecting the characters received to our 'buffer' (string).
                try
                {
                    if (sp.IsOpen == true)
                        recieved_data = sp.ReadLine();
                }
                catch (Exception exp)
                {
                    Console.WriteLine(" recieve" + exp.Message);
                   
                } 
               
                if (recieved_data != null && recieved_data.Substring(0,3) != "$GP")
                    recieved_data = "Data error\n";

                if (gpsTrame != null)
                {
                    if (gpsTrame.Count() > 22)
                        gpsTrame.Dequeue();

                    if (recieved_data != null)
                        gpsTrame.Enqueue(recieved_data);
                    OnPropertyChanged("GpsTrame");
                }
            }
        #endregion RECIEVING

        #region METHODS
            public void XMLtoSerialPort()
            {
                if (File.Exists(link))
                {
                    objports = ObjectsPorts.Charger(link);
                    foreach (ObjectPort o  in objports)
                    {
                        if (o.Id == 0)
                            defaultItemColor = new SolidColorBrush(Colors.Red);
                        else defaultItemColor = new SolidColorBrush(Colors.DeepSkyBlue);
                        OnPropertyChanged("DefaultItemColor");
                    }

                }
                else
                {
                    objports = new ObjectsPorts();
                }
                objports.Enregistrer(link);
            }
        #endregion METHODS

        #region NAVIGATION
            private void ExecuteGoToView()
            {

                Console.WriteLine("ViewClicked");
                if (sp.IsOpen)
                    sp.Close();

                App.Current.MainWindow.Visibility = Visibility.Hidden;
                App.Current.MainWindow = new DataParsedView();
                App.Current.MainWindow.Visibility = Visibility.Visible;
                //App.Current.MainWindow.Visibility = Visibility.Visible;
                //var newwindow = new DataParsedView();
                //App.Current.MainWindow.Close();
               // newwindow.Show();

               
            }
            private bool CanGoToView()
            { return true; }
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