using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace FacialRecognition
{
    public partial class Form1 : Form
    {
        private Capture videoCapture;
        private Image<Bgr, Byte> currentFrame = null;
        Mat frame = new Mat();
        private bool eyeDetectionEnabled = false;
        CascadeClassifier facesCascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");
        CascadeClassifier eyeCascadeClassifier = new CascadeClassifier("haarcascade_righteye_2splits.xml");
        public Form1()
        {
            InitializeComponent();
        }
        public Stopwatch watch { get; set; }
        public override string Text { get; set; }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
        private void find_eyes_Click(object sender, EventArgs e)
        {
            videoCapture = new Capture(1);
            videoCapture.ImageGrabbed += ProcessFrame;
            videoCapture.Start();
            eyeDetectionEnabled = true;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            //video capture
            videoCapture.Retrieve(frame, 0);
            currentFrame = frame.ToImage<Bgr, Byte>().Resize(pictureBox1.Width, pictureBox1.Height,Inter.Cubic);

            //step 2 detect faces
            if (eyeDetectionEnabled)
            {
                //convert bgr image to gray image
                Mat grayImage = new Mat();
                CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);
                // equilize the gray image to enhance the gray image
                CvInvoke.EqualizeHist(grayImage, grayImage);
                Rectangle[] faces = facesCascadeClassifier.DetectMultiScale(grayImage, 1.3, 5, Size.Empty, Size.Empty);
                Rectangle[] eyes = eyeCascadeClassifier.DetectMultiScale(grayImage, 1.3, 5, Size.Empty, Size.Empty);
                //if eyes detected
                if(faces.Length > 0)
                {
                    foreach (var face in faces)
                    {

                        CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Green).MCvScalar, 2);
                        foreach (var eye in eyes)
                        {
                            CvInvoke.Rectangle(currentFrame, eye, new Bgr(Color.Red).MCvScalar, 2);
                            var cordX = eye.X + (eye.Width / 2);
                            var cordY = eye.Y + (eye.Height / 2);
                            writeToPort(new Point(cordX, cordY));
                        }
                    }
                }
            }

            //render the video into the Picture Box
            pictureBox1.Image = currentFrame.Bitmap;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            watch = Stopwatch.StartNew();
            port.Open();

        }

        public void writeToPort(Point coordinates)
        {
            
            if (watch.ElapsedMilliseconds > 15)
            {
                watch = Stopwatch.StartNew();

                port.Write(String.Format("X{0}Y{1}",coordinates.X, coordinates.Y));
            }
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
