﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.WindowsMobile.Status;

namespace BarCodeScanner
{
    public partial class ServiceForm : Form
    {
        public ServiceForm()
        {
            InitializeComponent();
            MainForm.Log("[SF.Enter] Вход в настройки");
            listBox1.Items.Add("Сканер номер " + Config.scannerNumber);
            listBox1.Items.Add("MAC адрес " + Config.scannerMac);
            listBox1.Items.Add("IP адрес " + Config.scannerIp);
            listBox1.Items.Add("Дата и время " + System.DateTime.Now.ToString());
            try
            {
                if (MainForm.PingServer(Config.serverIp))
                    listBox1.Items.Add("Сервер " + Config.serverIp + " пингуется");
                else
                    listBox1.Items.Add("Сервер " + Config.serverIp + " не пингуется");
            }
            catch
            {
                listBox1.Items.Add("Ошибка пинга сервера " + Config.serverIp);
            }

            try
            {
                listBox1.Items.Add(Test1C());
            }
            catch
            {
                listBox1.Items.Add("1С не отвечает на запросы");
            }

            if ((SystemState.PowerBatteryState & BatteryState.Charging) != BatteryState.Charging)
            {
                listBox1.Items.Add("Заряд батареи " + Battery());

                if ((SystemState.PowerBatteryState & BatteryState.NotPresent) == BatteryState.NotPresent)
                    listBox1.Items.Add("Батарея отстутствует или неисправна");

                if ((SystemState.PowerBatteryState & BatteryState.Critical) == BatteryState.Critical)
                    listBox1.Items.Add("Критическое состояние батареи");
            }

            MainForm.doclist = Directory.GetFiles(MainForm.CurrentPath + "doc", "*_*_*.xml");

            listBox1.Items.Add("Количество документов " + MainForm.doclist.Length.ToString());
        }

        private string Battery()
        {
            switch (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength) {
                case BatteryLevel.VeryHigh:
                    return "81-100%";
                case BatteryLevel.High:
                    return "61-80%";
                case BatteryLevel.Medium:
                    return "41-60%";
                case BatteryLevel.Low:
                    return "21-40%";
                default: // VeryLow
                    return "0-20%";
            }
        }

        // (SystemState.PowerBatteryState == BatteryState.Critical)

        private void button1_Click(object sender, EventArgs e)
        {
            if (MainForm.DownloadSettings())
            {
                MainForm.Log("[SF.DownloadSettings] Настройки загружены");
                MessageBox.Show("Настройки успешно загружены");
            }
            else
            {
                MainForm.Log("[SF.NotDownloadSettings] Настройки загрузить не удалось");
                MessageBox.Show("Настройки загрузить не удалось");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileListForm flf = new FileListForm();
            flf.ShowDialog();
            flf.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
/*            listBox1.Items.Clear();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(MainForm.CurrentPath + "log.txt");
            while ((line = file.ReadLine()) != null)
            {
                listBox1.Items.Add(line);
            }
            listBox1.Focus();
            file.Close();*/

            MainForm.LogSave();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = @"\FlashDisk\Program Files\Notepad\Notepad.exe";
            processStartInfo.Arguments = @"\Program Files\barcodescanner\log.txt";
            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Doc;
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            File.Delete(MainForm.CurrentPath + "log.txt");
            MessageBox.Show("Лог-файл удалён");
        }

        private void ServiceForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                button2_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                button3_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Back))
            {
                button5_Click(this, e);
            }
            if (e.KeyCode.GetHashCode() == 190)
            {
                button6_Click(this, e);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D0)
            {
                button7_Click(this, e);
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm.SetTime(MainForm.GetTime());
                MessageBox.Show("Время с сервера получено: " + DateTime.Now.ToString());
            }
            catch
            {
                MessageBox.Show("Не удалось получить всемя с сервера");
            }
        }

        private string Test1C()
        {
            if (MainForm.TestConnect1C())
                return "1С отвечает на запросы";
            else return "1С не отвечает";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Test1C());
        }

    }
}