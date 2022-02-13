using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Threading;

namespace musica3._0
{
	/// <summary>
	/// Description of musica.
	/// </summary>
	public class musica
	{
		public string titolo { get; set; }
		public string autore { get; set; }
		public string album { get; set; }
		public string durata { get; set; }
		public string canzone { get; set; }
		public byte[] img { get; set; }
		public bool preferito { get; set; }

		public musica(musica m)
		{
			titolo = m.titolo;
			autore = m.autore;
			durata = m.durata;
			album = m.durata;
			canzone = m.canzone;
			img = m.img;
			preferito = false;
		}

		public musica(string t, string a, string al, string d, Uri m, TagLib.IPicture i)
		{
			this.titolo = t;
			this.autore = a;
			album = al;
			this.durata = d;
			canzone = m.ToString();
			if (i != null)
			{
				byte[] arraybyte = Convert.FromBase64String(Convert.ToBase64String(i.Data.Data));
				img = arraybyte;
			}
			else
			{
				i = null;
			}
			preferito = false;
		}

		public musica(string t, string a, string al, string d, string m, byte[] arraybyte)
		{
			this.titolo = t;
			this.autore = a;
			album = al;
			this.durata = d;
			canzone = m;
			img = arraybyte;
			preferito = false;
		}

		public musica ()
		{

		}

	}
}