using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace twitch_aio
{
    public partial class MainWindow : Form
    {
        private int channelid;
        private int follows_sent;
        private string[] bots;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = API.getUserByName(textBox1.Text);
            if (id == 0) return;
            channelid = id;
            panel1.Enabled= true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bots == null || bots.Length < 1)
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    bots = File.ReadAllLines(ofd.FileName);
                    RunFollows();
                }
            }
            else RunFollows();
        }
        private void RunFollows()
        {
            if (backgroundWorker_0.IsBusy)
            {
                MessageBox.Show("This function is already running!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else backgroundWorker_0.RunWorkerAsync();
        }

        private void backgroundWorker_0_DoWork(object sender, DoWorkEventArgs e)
        {
            label2.Invoke(new Action(delegate
            {
                follows_sent = 0;
                label2.Text = "0/0";
            }));
            Parallel.For(0, bots.Length, i =>
            {
                if(API.follow(channelid, bots[i]))
                {
                    BeginInvoke(new Action(delegate
                    {
                        follows_sent++;
                        label2.Text = $"{follows_sent}/{bots.Length}";
                    }));
                }
            });
        }
    }
}
