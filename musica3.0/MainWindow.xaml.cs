using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml.Serialization;
using VideoLibrary;
using YoutubeExtractor;

namespace musica3._0
{
    public partial class MainWindow : Window
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        playlist tutti;
        List<musica> elencoCorrente = new List<musica>();
        List<PlaylistRef> elencoPl = new List<PlaylistRef>();

        List<playlist> supporto = new List<playlist>();

        bool shuffle = false;
        bool pausa = false;
        bool mousepausa = false;
        int repeat = 0;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            //this.WindowState = WindowState.Maximized;
            volumeSlider.Value = 50;
            tutti = new playlist("TuttiBrani.xml", "Tutti i brani");
            
            aggiungiPlaylist(tutti);

            leggiPlaylistM();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            //timer.Start();
        }

        public void leggiPlaylistM()
        {
            StreamReader sr;
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer l = new XmlSerializer(typeof(List<PlaylistRef>));
                //apro stream reader
                //Uri u = new Uri(path, UriKind.Relative);
                sr = new StreamReader(@"c:\playlists\elenco.xml");
                //registro ciò che leggo
                elencoPl = ((List<PlaylistRef>)l.Deserialize(sr));
                //chiudo il deseializzatore perchè si
                sr.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
            int i = 0;
            foreach (PlaylistRef item in elencoPl)
            {
                supporto[i] = new playlist(item.percorso, item.nome);
                aggiungiPlaylist(supporto[i]);
                i++;
            }
            
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null && !pausa)
            {
                sliderTempo.Value = (mediaPlayer.Position.TotalSeconds / double.Parse(mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds.ToString())) * 100;
                txtDurata.Content = mediaPlayer.Position.Minutes + ":" + mediaPlayer.Position.Seconds + "/" + mediaPlayer.NaturalDuration.TimeSpan.Minutes + ":" + mediaPlayer.NaturalDuration.TimeSpan.Seconds;

                if (sliderTempo.Value >= 99)//(mediaPlayer.Position.TotalSeconds >= double.Parse(mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds.ToString()))
                { 
                    if (repeat == 0)
                    {
                        pausa = true;
                    }
                    if (repeat == 1)
                    {
                        btnNext_PreviewMouseLeftButtonDown(null, null);
                    }
                    if (repeat == 2)
                    {
                        caricaBrano((musica)lxLista.SelectedItem);
                        //mediaPlayer.Play();
                    }
                }
            }
        }
        public void aggiungiPlaylist(playlist pl)
        {
            StackPanel p = new StackPanel();
            p.Orientation = Orientation.Horizontal;
            p.HorizontalAlignment = HorizontalAlignment.Stretch;
            Rectangle r = new Rectangle();
            r.Width = 4;
            r.Height = 20;
            r.Fill = Brushes.Transparent;
            Label l = new Label();
            l.Content = pl.nomeP;
            l.Foreground = Brushes.White;
            p.Children.Add(r);
            p.Children.Add(l);
            p.MouseLeftButtonDown += selezionePlaylist;
            supporto.Add(pl);
            
            lxPlaylist.Items.Add(p);
        }
        public void selezionePlaylist(object sender, MouseButtonEventArgs e)
        {
            //qui semplicemente pulisco i rettangoli facendoli diventare trasparenti
            foreach (object item in lxPlaylist.Items)
            {
                if (item is StackPanel)
                {
                    foreach (object pannello in ((StackPanel)item).Children)
                    {
                        if (pannello is Rectangle)
                        {
                            ((Rectangle)pannello).Fill = Brushes.Transparent;
                        }
                    }
                }
            }
            //poi faccio le cose utili tipo pulire la lista e caricare i brani + creare una lista brani di supporto, elencoCorrente (utilissima)
            lxLista.Items.Clear();
            foreach (object item in ((StackPanel)sender).Children)
            {
                if (item is Label)
                {
                    if (((Label)item).Content.ToString() != "Tutti i brani")
                    {
                        for (int i = 0; i < supporto.Count; i++)
                        {
                            if (supporto[i].nomeP == ((Label)item).Content.ToString())
                            {
                                lxLista.Items.Clear();
                                elencoCorrente = supporto[i].brani;
                                foreach (musica m in supporto[i].brani)
                                {
                                    lxLista.Items.Add(m);
                                    lblPlaylist.Content = supporto[i].nomeP;
                                }
                            }
                        }
                    }
                    else
                    {
                        elencoCorrente = tutti.brani;
                        foreach (musica m in tutti.brani)
                        {
                            lxLista.Items.Add(m);
                            lblPlaylist.Content = "Tutti i brani";
                        }
                    }
                }
                else if(item is Rectangle)
                {
                    ((Rectangle)item).Fill = Brushes.MediumPurple;
                }
            }

        }
        public musica creaOggetto()
        {
            string artista = ((musica)lxLista.SelectedItem).autore;
            string album = ((musica)lxLista.SelectedItem).album;
            string durata = ((musica)lxLista.SelectedItem).durata;
            string titolo = ((musica)lxLista.SelectedItem).titolo;
            string canzone = ((musica)lxLista.SelectedItem).canzone;
            byte[] img = ((musica)lxLista.SelectedItem).img;
            musica st = new musica(titolo, artista, album, durata, canzone, img);
            return st;
        }
        public void filePlaylist(string path, playlist pl)
        {
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer s = new XmlSerializer(typeof(playlist));
                //creo lo stream di dati
                StreamWriter sw = new StreamWriter(@"c:\playlists\" + path, false);
                //serializz su un file (flusso,ogetto da serisalizzare)
                s.Serialize(sw, pl);
                //chiudo il seializzatore perchè si
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " su file" + " " + path);
            }
        }
        public void caricaBrano(musica st)
        {
            bool temp = true;
            if (mediaPlayer.Source == null || repeat==0)
            {
                pausa = true;
                temp = false;
            }
            mediaPlayer.Stop();
            sliderTempo.Value = 0;
            //pausa = false;
            timer.Stop();
            mediaPlayer.Open(new Uri(st.canzone));
            txtTitolo.Content = st.titolo;
            txtTitolo.Content = st.titolo;
            txtAutore.Content = st.autore;
            txtAlbum.Content = st.album;
            txtDurata.Content = st.durata;
            if (temp)
            {
                timer.Start();
                mediaPlayer.Play();
            }
            if (st.img != null)
            {
                BitmapImage i = new BitmapImage();
                //imgBrano.Source = new BitmapImage(new Uri(st.img.SyncRoot.ToString()));
                MemoryStream ms = new MemoryStream(st.img);
                ms.Seek(0, SeekOrigin.Begin);
                i.BeginInit();
                i.StreamSource = ms;
                i.EndInit();
                imgBrano.Source = i;
            }
            else
            {
                imgBrano.Source = new BitmapImage(new Uri(@"images/bg.jpg", UriKind.Relative));
            }
        }
        private void sliderControl_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //cambio posizione canzone
            timer.Stop();
            mediaPlayer.Pause();
            //mediaPlayer.Position = (mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds * sliderTempo.Value / 100) ;
        }
        private void sliderControl_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
        }
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //faccio la ricerca nella listbox
            List<musica> ricerca = new List<musica>();
            lxLista.Items.Clear();
            
                foreach (musica item in elencoCorrente)
                {
                    try
                    {
                        if (item.titolo.Contains(searchBox.Text))
                        {
                            lxLista.Items.Add(item);
                        }
                    }
                    catch(Exception)
                    { }
                }
            

        }
    
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //aggiungi un brano da memoria
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Stop();
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                TagLib.File tagFile = TagLib.File.Create(mediaPlayer.Source.LocalPath);
                string artist = tagFile.Tag.FirstPerformer;
                string album = tagFile.Tag.Album;
                string title = tagFile.Tag.Title;
                TagLib.IPicture img = null;
                try
                {
                    img = tagFile.Tag.Pictures[0];
                    MemoryStream ms = new MemoryStream(img.Data.Data);
                }
                catch (Exception)
                {
                }
                //
                mediaPlayer.Open(mediaPlayer.Source);
                mediaPlayer.Volume = 0;
                mediaPlayer.Play();
                Thread.Sleep(1000);
                string tempo = double.Parse(mediaPlayer.NaturalDuration.TimeSpan.Hours.ToString()) + ":" + double.Parse(mediaPlayer.NaturalDuration.TimeSpan.Minutes.ToString()) + ":" + double.Parse(mediaPlayer.NaturalDuration.TimeSpan.Seconds.ToString());
                mediaPlayer.Volume = volumeSlider.Value/700;
                mediaPlayer.Stop();
                //
                musica m = new musica(title, artist, album, tempo, mediaPlayer.Source, img);
                //
                //if(!tutti.brani.Contains(m))
                //{
                tutti.brani.Add(m);
                
                filePlaylist("TuttiBrani.xml",tutti);

                lxLista.Items.Clear();
                elencoCorrente = tutti.brani;
                foreach (musica M in tutti.brani)
                {
                    lxLista.Items.Add(M);
                    lblPlaylist.Content = "Tutti i brani";
                }
                lxLista.SelectedItem = m;
                if (repeat != 0)
                {
                    timer.Start();
                }
            }
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //rimuovi tutti i brani
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer l = new XmlSerializer(typeof(List<PlaylistRef>));
                //apro stream reader
                //Uri u = new Uri(path, UriKind.Relative);
                StreamReader sr = new StreamReader(@"c:\playlists\elenco.xml");
                //registro ciò che leggo
                elencoPl = ((List<PlaylistRef>)l.Deserialize(sr));
                //chiudo il deseializzatore perchè si
                sr.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
            int i = 0;
            
            File.Delete(@"c:\playlists\elenco.xml");
            foreach (PlaylistRef item in elencoPl)
            {
                File.Delete(@"C:\playlists\" + item.percorso);
            }
            lxPlaylist.Items.Clear();
            tutti = new playlist("TuttiBrani.xml", "Tutti i brani");

            aggiungiPlaylist(tutti);
        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            
            //avvio manuale
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Play();
                pausa = false;
            }
                

        }
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //pausa manuale
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Pause();
                pausa = true;
            }
        }
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            //stop manuale
            if (mediaPlayer.Source != null)
                mediaPlayer.Stop();
        }
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            //cancella tutti i brani
            lxPlaylist.Items.Clear();
            tutti.brani.Clear();
            File.Delete(@"C:\playlists\" + tutti.percorso);
            tutti = new playlist("TuttiBrani.xml", "Tutti i brani");
            aggiungiPlaylist(tutti);
        }
        private void btnBack_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnBack.IsMouseOver)
            {
                cambiaImmagine(@"images/back.png", sender);
            }
            else
            {
                cambiaImmagine(@"images/backV.png", sender);
            }
        }
        public void cambiaImmagine(string source, object o)
        {
            BitmapImage b = new BitmapImage(new Uri(source, UriKind.Relative));
            ((Image)o).Source = b;
        }
        private void btnPausa_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!pausa)
            {
                if (btnPausa.IsMouseOver)
                {
                    mousepausa = true;
                    cambiaImmagine(@"images/pausa.png", sender);
                }
                else
                {
                    mousepausa = false;
                    cambiaImmagine(@"images/pausaV.png", sender);
                }
            }
            else if(pausa)
            {
                if (btnPausa.IsMouseOver)
                {
                    mousepausa = true;
                    cambiaImmagine(@"images/play.png", sender);
                }
                else
                {
                    mousepausa = false;
                    cambiaImmagine(@"images/playV.png", sender);
                }
            }
        }
        private void btnNext_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnNext.IsMouseOver)
            {
                cambiaImmagine(@"images/next.png", sender);
            }
            else
            {
                cambiaImmagine(@"images/nextV.png", sender);
            }
        }
        private void btnShuffle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(shuffle)
            {
                btnShuffle.Source = new BitmapImage(new Uri(@"images/shuffleV.png", UriKind.Relative));
                shuffle = false;
            }
            else if(!shuffle)
            {
                btnShuffle.Source = new BitmapImage(new Uri(@"images/shuffle.png", UriKind.Relative));
                shuffle = true;
            }
        }
        private void btnBack_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //canzone indietro
            if (repeat == 0)
            {
                pausa = true;
            }
            if (lxLista.SelectedItem != null)
            {
                if ( (lxLista.Items.IndexOf(lxLista.SelectedItem) - 1) != -1)
                {
                    if (pausa == false)
                    {
                        lxLista.SelectedItem = elencoCorrente[lxLista.Items.IndexOf(lxLista.SelectedItem) - 1];
                        
                        pausa = false;
                    }
                    else
                    {
                        lxLista.SelectedItem = elencoCorrente[lxLista.Items.IndexOf(lxLista.SelectedItem) - 1];
                    }
                   
                }
                else
                {
                    if (pausa == false)
                    {
                        lxLista.SelectedItem = elencoCorrente[elencoCorrente.Count - 1];
                        
                        pausa = false;
                    }
                    else
                    {
                        lxLista.SelectedItem = elencoCorrente[elencoCorrente.Count - 1];
                    }
                    
                }
            }
        }
        private void btnPausa_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //cambio lo stato di pausa
            if (pausa)
            {
                pausa = false;
                if (!mousepausa)
                {
                    btnPausa.Source = new BitmapImage(new Uri(@"images/pausaV.png", UriKind.Relative));
                }
                else if(mousepausa)
                {
                    mediaPlayer.Play();
                    timer.Start();
                    btnPausa.Source = new BitmapImage(new Uri(@"images/pausa.png", UriKind.Relative));
                }
            }
            else if (!pausa)
            {
                if(!mousepausa)
                {
                    btnPausa.Source = new BitmapImage(new Uri(@"images/playV.png", UriKind.Relative));
                }
                else if(mousepausa)
                {
                    mediaPlayer.Pause();
                    timer.Stop();
                    btnPausa.Source = new BitmapImage(new Uri(@"images/play.png", UriKind.Relative));
                }
                pausa = true;
            }
        }
        private void btnNext_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //prossima canzone
            if (repeat == 0)
            {
                pausa = true;
            }
            if (lxLista.SelectedItem != null)
            {
                if (repeat == 0)
                {
                    pausa = false;
                }
                if (elencoCorrente.Count != lxLista.Items.IndexOf(lxLista.SelectedItem) + 1)
                {
                    if(shuffle == false)
                    {
                        if (pausa == false)
                        {
                            lxLista.SelectedItem = elencoCorrente[lxLista.Items.IndexOf(lxLista.SelectedItem) + 1];
                            
                            pausa = false;
                        }
                        else
                        {
                            lxLista.SelectedItem = elencoCorrente[lxLista.Items.IndexOf(lxLista.SelectedItem) + 1];
                        }
                        
                    }
                    else if (shuffle)
                    {
                        Random a = new Random();
                        int r = a.Next(0, elencoCorrente.Count);
                        //indiciRandom[lxLista.Items.IndexOf(lxLista.SelectedItem)] = r;
                        if (pausa == false)
                        {
                            lxLista.SelectedItem = elencoCorrente[r];
                            
                            pausa = false;
                        }
                        else
                        {
                            lxLista.SelectedItem = elencoCorrente[r]; ;
                        }
                         //indiciRandom[lxLista.Items.IndexOf(lxLista.SelectedItem)]
                    }
                }
                else
                {
                    if (pausa == false)
                    {
                        lxLista.SelectedItem = elencoCorrente[0];
                        
                        pausa = false;
                    }
                    else
                    {
                        lxLista.SelectedItem = elencoCorrente[0];
                    }
                        
                }
            }
            
        }
        private void btnRepeat_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(repeat == 0)//avvia riproduzione playlist
            {
                btnRepeat.Source = new BitmapImage(new Uri(@"images/repeat.png", UriKind.Relative));
                repeat++;
            }
            else if(repeat == 1)//avvia riproduzione canzone singola
            {
                btnRepeat.Source = new BitmapImage(new Uri(@"images/repeatT.png", UriKind.Relative));
                repeat++;
            }
            else if(repeat == 2)//togli riproduzione, una volta che finisce una canzone basta
            {
                btnRepeat.Source = new BitmapImage(new Uri(@"images/repeatV.png", UriKind.Relative));
                repeat=0;
            }
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //sono fortissimo l'ho gestito direttamente in un altra parte, alla creazione di una nuovo stackpanel playlist
        }
        private void Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(btnAddP.IsMouseOver)
            {
                cambiaImmagine(@"images/add.png", sender);
            }
            else
            {
                cambiaImmagine(@"images/addV.png", sender);
            }  
        }
        private void btnAddP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //aggiungi nuova playlist
            var window = new plAgg(tutti);
            playlist result; 
            if ((bool)window.ShowDialog() == true)
            {
                result = window.result;
                elencoPl.Add(new PlaylistRef(result.nomeP, result.percorso));
                //MessageBox.Show(result.percorso, "");
                try
                {
                    //Istanzio l'oggetto serializzatore
                    XmlSerializer s = new XmlSerializer(typeof(List<PlaylistRef>));
                    //creo lo stream di dati
                    StreamWriter sw = new StreamWriter(@"c:\playlists\elenco.xml", false);
                    //serializz su un file (flusso,ogetto da serisalizzare)
                    s.Serialize(sw, elencoPl);
                    //chiudo il seializzatore perchè si
                    sw.Close();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message + " su file" + " " + result.percorso);
                }

                filePlaylist(result.percorso, result);

                aggiungiPlaylist(result);
            }
            
        }
        private void lxLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((musica)lxLista.SelectedItem != null)
            {
                caricaBrano((musica)lxLista.SelectedItem);
            }
        }
        private void volumeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //cambio volume
            mediaPlayer.Volume = volumeSlider.Value / 500;
        }

        private void Image_IsMouseDirectlyOverChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnDownload.IsMouseOver)
            {
                cambiaImmagine(@"images/download.png", sender);
            }
            else
            {
                cambiaImmagine(@"images/downloadV.png", sender);
            }
        }

        private void btnDownload_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(txtUrl.Text != "")
            {
                try
                {

                    //IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
                    //VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == 360);
                    //if(video.RequiresDecryption)
                    //{
                    //    DownloadUrlResolver.DecryptDownloadUrl(video);
                    //}
                    //AudioDownloader downloader = new AudioDownloader(video, @"C:\Users\iT-Taste\Desktop\musica\" + video.Title + ".mp3");
                    //downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                    

                    YouTube youtube = YouTube.Default;

                    Video vid = youtube.GetVideo(txtUrl.Text);

                    System.IO.File.WriteAllBytes(@"C:\Users\iT-Taste\Desktop\musica\" + vid.FullName, vid.GetBytes());

                    var Convert = new NReco.VideoConverter.FFMpegConverter();

                    string SaveMP3File = @"C:\Users\iT-Taste\Desktop\musica\" + vid.FullName.Replace(".mp4", ".mp3");

                    Convert.ConvertMedia(@"C:\Users\iT-Taste\Desktop\musica\" + vid.FullName, SaveMP3File, "mp3");
                    //Delete the MP4 file after conversion

                    File.Delete(@"C:\Users\iT-Taste\Desktop\musica\" + vid.FullName);

                    timer.Stop();
                    pausa = true;
                    mediaPlayer.Stop();
                    mediaPlayer.Open(new Uri(SaveMP3File));
                    mediaPlayer.Volume = 0;
                    mediaPlayer.Play();
                    Thread.Sleep(2000);
                    string tempo = double.Parse(mediaPlayer.NaturalDuration.TimeSpan.Hours.ToString()) + ":" + double.Parse(mediaPlayer.NaturalDuration.TimeSpan.Minutes.ToString()) + ":" + double.Parse(mediaPlayer.NaturalDuration.TimeSpan.Seconds.ToString());
                    mediaPlayer.Volume = volumeSlider.Value / 200;
                    string titolo = vid.Title.Split('-')[1];
                    string autore = vid.Title.Split('-')[0];
                    if (titolo == "YouTube")
                    {
                        titolo = vid.Title.Split('-')[0];
                        autore = vid.Title.Split('-')[1];
                    }
                    musica m = new musica(titolo, autore, "", tempo, SaveMP3File, null);
                    tutti.brani.Add(m);
                    filePlaylist("TuttiBrani.xml", tutti);

                    lxLista.Items.Clear();
                    elencoCorrente = tutti.brani;
                    foreach (musica M in tutti.brani)
                    {
                        lxLista.Items.Add(M);
                        lblPlaylist.Content = "Tutti i brani";
                    }
                    lxLista.SelectedItem = m;
                    if(repeat != 0)
                    {
                        timer.Start();
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.ToString(), "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("inserire un Url per scaricare una canzone", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
