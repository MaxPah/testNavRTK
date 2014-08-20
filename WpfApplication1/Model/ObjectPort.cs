using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WpfApplication1.Model
{
    [Serializable]
    public class ObjectPort
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Baudrate { get; set; }
           
        public string Databits { get; set; }

        public string Stopbit {get; set;}

        public string Parity { get; set; }

        public string Handshake { get; set; }

        public string ToStringComplete()
        {
            return Id +
                " Name: " + Name +
                " / BaudRate: " + Baudrate +
                " / Databits: " + Databits +
                " / Stopbit " + Stopbit +
                " / Parity: " + Parity +
                " / Handshake: " + Handshake;
        }
    }
}
