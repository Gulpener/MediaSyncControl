using MediaSyncControl.EF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

namespace MediaSyncControl.UI
{
    /// <summary>
    /// Interaction logic for MediaPlayerControl.xaml
    /// </summary>
    public partial class MediaPlayerControl : UserControl
    {
        bool IsPlaying = false;
        int episodeID;
        Thread thread;
        
        ImageSource SoundOff;
        ImageBrush brushSoundOff;
        ImageSource SoundOn;
        ImageBrush brushSoundOn;        
        ImageSource PlayIcon;
        ImageBrush brushPlayIcon;
        ImageSource PauseIcon;
        ImageBrush brushPauseIcon;

        public MediaPlayerControl()
        {
            InitializeComponent();

            SoundOff = loadImageSource(Properties.Resources.SoundOffIcon);
            SoundOn = loadImageSource(Properties.Resources.SoundOnIcon);
            PlayIcon = loadImageSource(Properties.Resources.PlayIcon);
            PauseIcon = loadImageSource(Properties.Resources.PauseIcon);
            brushSoundOff = new ImageBrush();
            brushSoundOff.ImageSource = SoundOff;//loadImageSource(Properties.Resources.SoundOffIcon);  
            brushSoundOn = new ImageBrush();
            brushSoundOn.ImageSource = SoundOn;//loadImageSource(Properties.Resources.SoundOnIcon);
            lblMuteSound.Background = brushSoundOn;

            brushPlayIcon = new ImageBrush();
            brushPlayIcon.ImageSource = PlayIcon;
            lblPlay.Background = brushPlayIcon;

            brushPauseIcon = new ImageBrush();
            brushPauseIcon.ImageSource = PauseIcon;

            EnableControls(false);
            
            soundSlider.Value = soundSlider.Maximum;
            SetControlFlow(0, 0, 0, 0);                      
        }

        private void EnableControls(bool enabled)
        {
            CompleteControlGrid.IsEnabled = enabled;
        }

        private void SetControlFlow(int sliderheight, int controlheight, int timeheight, int controlgridheight)
        {
            GridLength sliderHeight = new GridLength(sliderheight, GridUnitType.Pixel);
            GridLength controlHeight = new GridLength(controlheight, GridUnitType.Pixel);
            GridLength timeHeight = new GridLength(timeheight, GridUnitType.Pixel);

            ControlGrid.Height = controlgridheight;
            SliderBar.Height = sliderHeight;
            ControlBar.Height = controlHeight;
            TimeBar.Height = timeHeight;
        }

        internal void setSource(int episodeid)
        {
            this.episodeID = episodeid;
            string episodepath = DatabaseAdapter.GetEpisodePath(episodeID);
            string episodename = DatabaseAdapter.GetEpisodeName(episodeID);
            TimeSpan lastwatchedtime = DatabaseAdapter.GetLastWatchedTime(episodeID);
            lblEpisodeName.Content = episodename;
            mMediaPlayer.Source = new Uri(episodepath);
            mMediaPlayer.Position = lastwatchedtime;
            Play();
            Pause();
        }

        internal void Play()
        {
            mMediaPlayer.Play();
            IsPlaying = true;
            lblPlay.Content = "";
            lblPlay.Background = brushPauseIcon;
        }

        internal void Pause()
        {
            mMediaPlayer.Pause();
            IsPlaying = false;
            lblPlay.Content = "";
            lblPlay.Background = brushPlayIcon;
        }

        private void Stop()
        {
            mMediaPlayer.Stop();
            lblPlay.Content = "";
            lblPlay.Background = brushPlayIcon;
        }

        private void Seek(TimeSpan ts)
        {
            mMediaPlayer.Position = ts;            
        }

        private void seekBackward()
        {
            mMediaPlayer.Position -= TimeSpan.FromSeconds(10);
        }

        private void seekForward()
        {
            mMediaPlayer.Position += TimeSpan.FromSeconds(10);
        } 

        private void mMediaPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (IsPlaying == true)
                {
                    Pause();
                }
                else
                {
                    Play();
                }
            }
        }

        private void lblPlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsPlaying == false)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        private void lblStop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Stop();
        }
                
        private void btnMoveBack_Click(object sender, RoutedEventArgs e)
        {            
            seekBackward();            
        }

        private void btnMoveForward_Click(object sender, RoutedEventArgs e)
        {
            seekForward();
        }

        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int SliderValue = (int)timelineSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            Seek(ts);
        }

        private void SeekToAudio(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double SliderValue = (double)soundSlider.Value;
            mMediaPlayer.Volume = SliderValue;
        }

        private void lblMuteSound_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (soundSlider.Value >= 0.001)
            {
                soundSlider.Value = 0;


                lblMuteSound.Background = brushSoundOff;

            }
            else if (soundSlider.Value == 0)
            {
                soundSlider.Value = soundSlider.Maximum;
                lblMuteSound.Background = brushSoundOn;

            }
            double SliderValue = (double)soundSlider.Value;
            mMediaPlayer.Volume = SliderValue;
        }

        private ImageSource loadImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                memory.Seek(0, SeekOrigin.Begin);
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                
                return bitmapImage;
            }
        }

        private void mMediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = mMediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            soundSlider.Maximum = 1;
            soundSlider.Minimum = 0;
            soundSlider.Value = 1;
            lblTotalTime.Content = getRightTime(mMediaPlayer.NaturalDuration.TimeSpan);
            DatabaseAdapter.setTotalTime(episodeID, mMediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds);
            EnableControls(true);
            thread = new Thread(new ThreadStart(updateWatchedTimeThread));
            thread.Start();
        }

        private void updateWatchedTimeThread()
        {
            while (true)
            {
                updateWatchedTime2();
                Thread.Sleep(1000);
            }
        }

        public void updateWatchedTime2()
        {
            if (mMediaPlayer != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.lblWatchedTime.Content = getRightTime(mMediaPlayer.Position)));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.timelineSlider.Value = mMediaPlayer.Position.TotalMilliseconds));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => DatabaseAdapter.setWatchedTime(episodeID, mMediaPlayer.Position)));
            }
        }

        private string getRightTime(TimeSpan timeSpan)
        {
            string righttime;
            string shours, sminutes, sseconds;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (hours == 0)
            {
                shours = "";
            }
            else if (hours <= 9)
            {
                shours = "0" + hours + ":";
            }
            else
            {
                shours = hours + ":";
            }
            if (minutes <= 9)
            {
                sminutes = "0" + minutes + ":";
            }
            else
            {
                sminutes = minutes + ":";
            }
            if (seconds <= 9)
            {
                sseconds = "0" + seconds;
            }
            else
            {
                sseconds = seconds.ToString();
            }

            righttime = shours+sminutes+sseconds;
            return righttime;
        }
                

        private void mMediaPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (thread != null)
            {
                thread.Abort();
            }
            EnableControls(false);
            //Dit kan niet worden gebruikt voor het versturen van het laatste gekeken moment
        }

        internal void Closing()
        {
            if (thread != null)
            {
                thread.Abort();
            }
            Stop();            
        }

        private void UserControl_MouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                soundSlider.Value = soundSlider.Value - 0.1;
                if (soundSlider.Value == 0)
                {
                    lblMuteSound.Background = brushSoundOff;
                }
            }
            else
            {
                soundSlider.Value = soundSlider.Value + 0.1;
                if (soundSlider.Value >= 0.001)
                {
                    lblMuteSound.Background = brushSoundOn;
                }
            }
        }

        private void Grid_MouseEnter_1(object sender, MouseEventArgs e)
        {
            SetControlFlow(20,35,29,84);
        }

        private void Grid_MouseLeave_1(object sender, MouseEventArgs e)
        {
            SetControlFlow(0, 0, 0,0);
        }

    }
}
