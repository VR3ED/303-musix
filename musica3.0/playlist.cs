using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace musica3._0
{
    public class playlist
    {
        public string nomeP;
        public List<musica> brani = new List<musica>();
        public string percorso;
        public playlist(string path, string nome)
        {
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer l = new XmlSerializer(typeof(playlist));
                //apro stream reader
                //Uri u = new Uri(path, UriKind.Relative);
                StreamReader sr = new StreamReader(@"c:\playlists\" + path);
                //registro ciò che leggo
                brani = ((playlist)l.Deserialize(sr)).brani;
                //chiudo il deseializzatore perchè si
                sr.Close();
            }
            catch (Exception E)
            {
                //MessageBox.Show(E.ToString());
            }
            this.percorso = path;
            nomeP = nome;
        }

        public playlist(string nome, string percor, List<musica> b)
        {
            nomeP = nome;
            percorso = percor;
            brani = b;
        }

        public playlist()
        {

        }
    }
}
