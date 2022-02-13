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
using System.Windows.Shapes;

namespace musica3._0
{
    /// <summary>
    /// Logica di interazione per plAgg.xaml
    /// </summary>
    public partial class plAgg : Window
    {
        playlist n;

        public plAgg(playlist t)
        {
            InitializeComponent();
            foreach (musica item in t.brani)
            {
                lxLista.Items.Add(item);
            }
        }

        private void btnCrea_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(btnCrea.IsMouseOver)
            {
                btnCrea.Source = new BitmapImage( new Uri(@"images/crea.png", UriKind.Relative) );
            }
            else
            {
                btnCrea.Source = new BitmapImage(new Uri(@"images/creaV.png", UriKind.Relative));
            }
        }

        private void btnCrea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = txtNome.Text.Replace(" ", "_") + ".xml";
            List<musica> brani = new List<musica>();
            foreach (musica item in lxLista.SelectedItems)
            {
                brani.Add(item);
            }
            Window.GetWindow(this).DialogResult = true;

            n = new playlist(txtNome.Text, path, brani);
            
            this.Close();
        }

        public playlist result
        {
            get { return n; }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }
    }
}
