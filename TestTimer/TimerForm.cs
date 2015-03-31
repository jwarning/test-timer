using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTimer
{
    public partial class TimerForm : Form
    {
        private TimeSpan nextTestInterval;
        private DateTime nextTestTime;
        private int progressBarHeight;

        public TimerForm()
        {
            nextTestInterval = new TimeSpan(1, 0, 0); // 1 hour

            var nextTest = DateTime.Now.Add(nextTestInterval);

            // force seconds to zero
            nextTestTime = new DateTime(nextTest.Year, nextTest.Month, nextTest.Day, nextTest.Hour, nextTest.Minute, 0);
            progressBarHeight = 300;

            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer1.Interval = 1000; // 1 second
            timer1.Tick += new EventHandler(Timer1_Tick);
            timer1.Enabled = true;

            currentTimeLabel.Text = DateTime.Now.ToString("HH:mm");
            nextTestTimeTextBox.Text = nextTestTime.ToString("HH:mm");
        }

        private void Timer1_Tick(object Sender, EventArgs e)
        {
            currentTimeLabel.Text = DateTime.Now.ToString("HH:mm");

            // auto increment next test time
            if (DateTime.Now >= nextTestTime) nextTestTime = nextTestTime.Add(nextTestInterval);

            this.Refresh();
        }

        private void offButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            // update labels
            currentTimeLabel.Text = "Off";
            nextTestTimeTextBox.Text = "Off";
            nextTestTimeTextBox.ReadOnly = true;

            this.Refresh();
        }

        private void onButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;

            var now = DateTime.Now.Add(nextTestInterval);
            // force seconds to zero
            nextTestTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            // update labels
            currentTimeLabel.Text = DateTime.Now.ToString("HH:mm");
            nextTestTimeTextBox.Text = nextTestTime.ToString("HH:mm");
            nextTestTimeTextBox.ReadOnly = false;
        }

        private void pictureBoxRight_Paint(object sender, PaintEventArgs e)
        {
            double timeDifference = DateTime.Now.Subtract(nextTestTime).TotalMinutes / nextTestInterval.TotalMinutes;

            // hide progress bar when off
            if (!timer1.Enabled) timeDifference = -progressBarHeight;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // draw progress bar
            Rectangle rect = new Rectangle(0, 0, 100, progressBarHeight + Convert.ToInt32(timeDifference * progressBarHeight));
            e.Graphics.FillRectangle(Brushes.Green, rect);
        }

        private void pictureBoxLeft_Paint(object sender, PaintEventArgs e)
        {
            TimeSpan timeDifference = nextTestTime.Subtract(DateTime.Now);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // draw lights
            Rectangle rect = new Rectangle(0, 0, 100, 300);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 80, 130, 190)), rect);

            // red (5 minutes to go)

            if (timeDifference.TotalMinutes <= 5)
            {
                rect = new Rectangle(10, 10, 80, 80);

                // change opacity every two seconds to get a flashing effect
                if (DateTime.Now.Second % 2 != 0) e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0, 0)), rect);
                else e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(200, 255, 0, 0)), rect);
            }

            // yellow (10 minutes to go)
            if (timeDifference.TotalMinutes <= 10 && timeDifference.TotalMinutes > 5)
            {
                rect = new Rectangle(10, 110, 80, 80);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 195, 0)), rect);
            }

            // green (15 minutes to go)
            if (timeDifference.TotalMinutes <= 15 && timeDifference.TotalMinutes > 10)
            {
                rect = new Rectangle(10, 210, 80, 80);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 145, 210, 80)), rect);
            }
        }

        private void nextTestTimeTextBox_Validating(object sender, CancelEventArgs e)
        {
            DateTime value;
            if (DateTime.TryParse(nextTestTimeTextBox.Text, out value))
            {
                // make sure the date is no more than an hour in the future
                if (value.Subtract(DateTime.Now).TotalMinutes <= 60 && value.Subtract(DateTime.Now).TotalMinutes > 0)
                {
                    // clean up date time input
                    nextTestTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
                    nextTestTimeTextBox.Text = nextTestTime.ToString("HH:mm");

                    return;
                }
            }

            nextTestTimeTextBox.Text = nextTestTime.ToString("HH:mm");
        }

        private void nextTestTimeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // force the focus to change to trigger a validating event on the text box
                this.ActiveControl = currentTimeLabel;
            }
        }
    }
}
