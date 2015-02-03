using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Microsoft.Win32;

using AForge.Video.DirectShow; //Bu Bilgisayarýmýza Baðlý Video Aygýtlarýný programýmýza dahil etmemiz için gerekli kütüphane
using AForge.Vision.Motion; //hareket algýlamasýný gerçekleþtiren Kütüphane
using AForge.Video;
using AForge.Imaging.Filters;
using AForge.Imaging; //Ekrana Görüntüyü veren Kütüphane


namespace MotionDetection
{
    public partial class Anaform : Form
    {
        
       
      
        
        private IVideoSource ivideo = null;
        private IMotionDetector mdetector=null, md = null;
        private int dtip = 0;
        private FilterInfoCollection webcamler = null;
        private string webcam;
        private VideoCaptureDevice Secilenwebcam = null;
        public bool noise = false;

        public Anaform()
        {
            InitializeComponent();
            
            try
            {
                webcamler = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (webcamler.Count == 0)
                    throw new ApplicationException();

                foreach (FilterInfo webcam in webcamler)
                {
                    comboBox1.Items.Add(webcam.Name);
                }

            }
            catch (ApplicationException)
            {
                comboBox1.Items.Add("Video Aygýtý Bulunamadý.");
                comboBox1.Enabled = false;
                button1.Enabled = false;

            }
            comboBox1.SelectedIndex = 0;
        }


   


        private void Anaform_Load(object sender, EventArgs e)
        {
            
            
        }

        private void DetectorSec(int deger) 
        {
            
            Kamera camera = kameraKutusu1.Camera;
         

            if (camera != null)
            {
                camera.Lock();
                switch (deger)
                {
                    case 0: 
                        md = null;
                        camera.MDetector = md;
                        break;
                    case 1:
                        //md = new TwoFramesDifferenceMotionDetector(true);
                        TwoFramesDifferenceMotionDetector TwoFrameTH = new TwoFramesDifferenceMotionDetector(true, noise);
                        TwoFrameTH.DifferenceThreshold = trackBar1.Value;
                        
                        //label4.Text = TwoFrameTH.MotionLevel.ToString();
                        camera.MDetector = TwoFrameTH;

                        break;
                    case 2:
                        //md = new BackgroundModelingHighPrecisionMotionDetector(true); 
                        BackgroundModelingHighPrecisionMotionDetector BgHighTH = new BackgroundModelingHighPrecisionMotionDetector(true, noise);
                        BgHighTH.DifferenceThreshold = trackBar1.Value;
                        //label4.Text = BgHighTH.MotionLevel.ToString();
                        camera.MDetector = BgHighTH;
                        break;
                    case 3:
                        //md = new BackgroundModelingLowPrecisionMotionDetector(true);
                        BackgroundModelingLowPrecisionMotionDetector BgLowTH = new BackgroundModelingLowPrecisionMotionDetector(true);
                        BgLowTH.DifferenceThreshold = trackBar1.Value;
                        //label4.Text = BgLowTH.MotionLevel.ToString();
                        camera.MDetector = BgLowTH;
                        break;
                    case 4:
                        //md = new CountingMotionDetector(true); 
                        CountingMotionDetector CountTH = new CountingMotionDetector(true);
                        CountTH.DifferenceThreshold = trackBar1.Value;
                        //CountTH.FramesPerBackgroundUpdate = 50;
                        

                        
                        camera.MDetector = CountTH;
                        
                        

                        //camera.IsRunning
                        break;
                    
                        
                        
                }
                //camera.MDetector = md;
                camera.UnLock();
                

            }

        }   
  
        private void OpenVideo(IVideoSource iv)
        {
            // this.Cursor = Cursors.WaitCursor;

            try
            {
                CloseVideo();

                Kamera camera = new Kamera(iv, md);
                Kamera camera2 = new Kamera(iv, mdetector);
                camera.YeniFrame += new EventHandler(Kamera_YeniFrame);
                camera.Start();

                kameraKutusu1.Camera = camera;


                ivideo = iv;

            }
            catch { }
        }

        private void Kamera_YeniFrame(object sender, System.EventArgs e) 
        {

            //Kamera cam = kameraKutusu1.Camera;
            
            //    if (md is ICountingMotionDetector)
            //    {
            //        ICountingMotionDetector cDetector = (ICountingMotionDetector)cam.MDetector;
            //        //
            //        if (cDetector.MotionLevel > 100)
            //        {
            //            if (cDetector.ObjectsCount >= 1)
            //            {
            //                cDetector.Reset();
            //                dtip = 0;
            //                DetectorSec(dtip);
                            
            //            }
                        
            //        }

            //    }                  
        }

        private void Anaform_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseVideo();
            Application.Exit();
        }

        private void CloseVideo()
        {
            try
            {
                Kamera camera = kameraKutusu1.Camera;
                if (camera != null)
                {

                    kameraKutusu1.Camera = null;
                    Application.DoEvents();

                    camera.Stop();
                    camera = null;

                    if (mdetector != null)
                    {
                        mdetector.Reset();
                    }

                    ivideo = null;
                }
            }
            catch { }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            CloseVideo();
            webcam = webcamler[comboBox1.SelectedIndex].MonikerString;
            Secilenwebcam = new VideoCaptureDevice(webcam);
            OpenVideo(Secilenwebcam);
            comboBox1.Enabled = false;// Webcam seçimi pasif et
          
            
           
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CloseVideo();
            Application.Exit();
            Dispose();
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseVideo();
            Application.Exit();
            Dispose();
        }
        
        public  void setCount(int count)
        {
            label5.Text = count.ToString() ;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) DetectorSec(0);
            if (comboBox2.SelectedIndex == 1) DetectorSec(1);
            if (comboBox2.SelectedIndex == 2) DetectorSec(2);
            if (comboBox2.SelectedIndex == 3) DetectorSec(3);
            if (comboBox2.SelectedIndex == 4) DetectorSec(4);
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            DetectorSec(0);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) DetectorSec(0);
            if (comboBox2.SelectedIndex == 1) DetectorSec(1);
            if (comboBox2.SelectedIndex == 2) DetectorSec(2);
            if (comboBox2.SelectedIndex == 3) DetectorSec(3);
            if (comboBox2.SelectedIndex == 4) DetectorSec(4);

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int deger = trackBar1.Value;
            label3.Text = deger.ToString();
        }

        private void kameraKutusu1_Click(object sender, EventArgs e)
        {

        }

        private void Anaform_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            
            Application.Exit();
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { noise = true; }
            else noise = false;
        }

        

       

       
    }
}