using System;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing;

namespace WebCam2
{
    public partial class Form1 : Form
    {
        FilterInfoCollection FilterInfo;
        private VideoCaptureDevice videoCapture;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startCamera();
        }

        private void startCamera()
        {
            try
            {
                FilterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (FilterInfo.Count == 0)
                {
                    MessageBox.Show("No webcam found!");
                    return;
                }

                videoCapture = new VideoCaptureDevice(FilterInfo[0].MonikerString);
                videoCapture.NewFrame += new NewFrameEventHandler(Camera_On);
                videoCapture.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting camera: " + ex.Message);
            }
        }

        private void Camera_On(object sender, NewFrameEventArgs e)
        {
            // Always dispose old image to prevent memory leak
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();

            pictureBox1.Image = (Bitmap)e.Frame.Clone();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image to capture. Start the camera first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter a filename in the textbox.");
                return;
            }

            // Remove any invalid filename characters
            string safeFileName = string.Concat(textBox1.Text.Split(System.IO.Path.GetInvalidFileNameChars()));

            // Clone live image to captured image box
            pictureBox2.Image = (Bitmap)pictureBox1.Image.Clone();

            // Use textbox name in filename
            string filename = $@"C:\Users\Ayush\source\repos\WebCam2\photo\{safeFileName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg";

            pictureBox2.Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);

            MessageBox.Show($"Image saved to: {filename}");
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null && videoCapture.IsRunning)
            {
                videoCapture.SignalToStop();
                videoCapture.WaitForStop();
            }
        }

        
    }
}
