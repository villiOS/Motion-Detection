using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;

namespace MotionDetection
{
    public partial class KameraKutusu : System.Windows.Forms.Control
    {
        private Kamera camera = null;
        private bool needSizeUpdate = false;
        private bool firstFrame = true;
        private int count=0;
        // timer used for flashing
        private int flash = 0;
        public Label lbl=new Label();
        // rectangle's color
        private Color rectColor = Color.Black;

        public KameraKutusu()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
           
        }

        public KameraKutusu(IContainer container,Label _lbl)
        {
            container.Add(this);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            InitializeComponent();
         
            lbl = _lbl;
            
        }
        [Browsable(false)]
        public Kamera Camera
        {
            get { return camera; }
            set
            {
                Monitor.Enter(this);

                if (camera != null)
                {
                    camera.YeniFrame -= new EventHandler(camera_YeniFrame);
                    camera.VideoError -= new EventHandler(camera_VideoError);
                }

                camera = value;
                needSizeUpdate = true;
                firstFrame = true;
                flash = 0;

                if (camera != null)
                {
                    camera.YeniFrame += new EventHandler(camera_YeniFrame);
                    camera.VideoError += new EventHandler(camera_VideoError);
                }

                Monitor.Exit(this);

                Invalidate();
            }
        }

        private void camera_YeniFrame(object sender, System.EventArgs e)
        {
            Invalidate();
            count = 0;
        }

        private void camera_VideoError(object sender, System.EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            
          

            if ((needSizeUpdate) || (firstFrame))
            {
                needSizeUpdate = false;
            }
            Monitor.Enter(this);

            Graphics ge = e.Graphics;
            Rectangle rc = this.ClientRectangle;
            Pen p = new Pen(Color.Black, 1);

            ge.DrawRectangle(p, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                count++;
             //   lbl.Text = count.ToString();

                Label lbl = new Label();
            
            if (camera != null)
            {
                try
                {
                    camera.Lock();

                    if ((camera.SonFrame != null) && (camera.SonFrameHata == null))
                    {
                        ge.DrawImage(camera.SonFrame, rc.X + 1, rc.Y + 1, rc.Width - 2, rc.Height - 2);
                    }

                    else
                    {
                        Font dr = new Font("Arial", 12);
                        SolidBrush drbush = new SolidBrush(Color.White);
                        ge.DrawString((camera.SonFrameHata == null) ? "Baðlantý Saðlanýyor" : camera.SonFrameHata, dr, drbush, new Point(5, 5));

                        dr.Dispose();
                        drbush.Dispose();
                    }
                }
                //catch (Exception)
                //{

                //}
                catch (ThreadAbortException) { Thread.ResetAbort(); }
                finally
                {
                    camera.UnLock();
                    
                }
            }
         
            
            p.Dispose();
            Monitor.Exit(this);
            base.OnPaint(e);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (flash > 0)
            {
                // calculate color
                if (--flash == 0)
                {
                    rectColor = Color.Black;
                }
                else
                {
                    rectColor = (rectColor == Color.Red) ? Color.Black : Color.Red;
                }

                // draw rectangle
                if (!needSizeUpdate)
                {
                    Graphics g = this.CreateGraphics();
                    Rectangle rc = this.ClientRectangle;
                    Pen pen = new Pen(rectColor, 1);

                    // draw rectangle
                    g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);

                    g.Dispose();
                    pen.Dispose();
                }
            }
        }
    }
}
