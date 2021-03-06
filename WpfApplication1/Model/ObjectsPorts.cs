﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace WpfApplication1.Model
{
    [Serializable()]
    public class ObjectsPorts : List<ObjectPort>
    {
        /// <summary>
        /// Save the current state of the class to XML format.
        /// </summary>
        /// <param name="chemin"></param>
        public void Enregistrer(string link)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObjectsPorts));
            StreamWriter ecrivain = new StreamWriter(link);
            serializer.Serialize(ecrivain, this);
            ecrivain.Close();
        }

        public static ObjectsPorts Charger(string link)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ObjectsPorts));
            StreamReader lecteur = new StreamReader(link);
            ObjectsPorts p = (ObjectsPorts)deserializer.Deserialize(lecteur);
            lecteur.Close();
            return p;
        }

        public int MaxId()
         {
             int max=0;
             foreach (ObjectPort o in this)
             {
                 if (max < o.Id)
                     max = o.Id;
             }
             return max;
         }
        public void DefaultSwap(int i)
        {
                foreach (ObjectPort o in this)
                {
                    if (o.Id == 0)
                        o.Id = i;
                    else if (o.Id == i)
                        o.Id = 0;

                }


                this.OrderBy(x => x.Id);
        }

    }
}