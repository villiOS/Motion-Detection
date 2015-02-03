
namespace MotionDetection
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing;
    using System.Threading;

    using AForge.Video;
    using AForge.Vision.Motion;
    using System.Windows.Forms;

    public class Kamera
    {
        private IVideoSource ivideo = null;
        private IMotionDetector mdetector = null;
        private Bitmap sonframe = null;
        private string sonframeHata = null;
       
        private int width = -1;
        private int height = -1;

        public event EventHandler YeniFrame;
        public event EventHandler VideoError;

        public Bitmap SonFrame
        {
            get { return sonframe; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public string SonFrameHata
        {
            get { return sonframeHata; }
        }

        public int FrameKabul
        {
            get { return (ivideo == null) ? 0 : ivideo.FramesReceived; }
        }

        public bool IsRunning
        {
            get { return (ivideo == null) ? false : ivideo.IsRunning; }

        }

        public void WaitForStop()
        {
            Monitor.Enter(this);

            if (ivideo != null)
            {
                ivideo.WaitForStop();
            }
            Monitor.Exit(this);
        }

        public IMotionDetector MDetector
        {
            get { return mdetector; }
            set { mdetector = value; }
        }

        public Kamera(IVideoSource ivideo) : this(ivideo, null) { }

        public Kamera(IVideoSource ivideo, IMotionDetector mdetector)
        {
            this.ivideo = ivideo;
            this.mdetector = mdetector;

            ivideo.NewFrame += new NewFrameEventHandler(Kamera_YeniFrame);
            ivideo.VideoSourceError += new VideoSourceErrorEventHandler(Kamera_VideoError);
        }

        public void Start()
        {
            if (ivideo != null)
            {
                ivideo.Start();
            }
        }

        public void Stop()
        {
            Monitor.Enter(this);
            if (ivideo == null)
            {
                ivideo.Stop();
            }
            Monitor.Exit(this);
        }

        public void Lock()
        {
            Monitor.Enter(this);
        }

        public void UnLock()
        {
            Monitor.Exit(this);
        }

        private void Kamera_YeniFrame(object sender, NewFrameEventArgs e)
        {
            try
            {
                Monitor.Enter(this);

                if (sonframe != null)
                {
                    sonframe.Dispose();
                }

                sonframeHata = null;
                sonframe = (Bitmap)e.Frame.Clone();

                if (mdetector != null)
                {
                    mdetector.ProcessFrame(sonframe);
                }

                width = sonframe.Width;
                height = sonframe.Height;
                
            }
            catch
            { Thread.ResetAbort(); }

            finally
            {
                Monitor.Exit(this);

            }
            if (YeniFrame != null)
            {
                YeniFrame(this, new EventArgs());
            }

        }

        public void Kamera_VideoError(object sender, VideoSourceErrorEventArgs e)
        {
            if (mdetector != null)
            {
                mdetector.Reset();
            }
            sonframeHata = e.Description;

            if (VideoError != null)
            {
                VideoError(this, new EventArgs());
            }
        }
    }
}
