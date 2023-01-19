using System.Windows.Forms;
using System.IO;
using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Security.Cryptography;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;

namespace LogExport
{
    public partial class frmLogExport : Form
    {
        string strTargetPath;
        string strSourcePath;
        string strDataPath;
        string strMCSPath;
        string foldernum;
        List<string> strList = new List<string>();
        DateTime dtStartDate;
        DateTime dtEndDate;
        string[] mcsDirectory = {""};
        string[] directory;
        string[] exceptData = { ".exe", ".dll"};
        string[] changeData = { ".db", ".mdb" };
        int lang;
        //Stopwatch stopwatch = new Stopwatch();
        
        public frmLogExport()
        {
            InitializeComponent();
            //lang = 0;   //lang 0 : KOR, 1 : ENG, 2 : CHN

            switch (Properties.Settings.Default.LANGUAGE)
            {
                case "KOR": lang = 0; break;
                case "ENG": lang = 1; break;
                case "CHN": lang = 2; break;
            }
            //lblTargetPath.Text = Properties.Settings.Default.TESTTARGETPATH;


            Lang(lang);
        }
        private void Lang(int lang)
        {
            lblModel.Text = Properties.Resources.TARGET_MODEL.Split(',')[lang];
            lblPath.Text = Properties.Resources.lblPath.Split(',')[lang];
            btn_ColList.Text = Properties.Resources.btn_ColList.Split(',')[lang];
            btnCollect.Text = Properties.Resources.btn_Collect.Split(',')[lang];
            btnCollectZIP.Text = Properties.Resources.btnCollectZIP.Split(',')[lang];
            groupBox2.Text = Properties.Resources.groupBox2.Split(',')[lang];
            groupBox3.Text = Properties.Resources.groupBox3.Split(',')[lang];
        }
      
        private void frmLogExport_Load(object sender, EventArgs e)
        {
            dtStartDatePicker.Value = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")));
            dtEndDatePicker.Value = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")));

            dtStartDate = dtStartDatePicker.Value;
            dtEndDate = dtEndDatePicker.Value;

            kinds.SelectedIndex = 0;
        }

        private void CopyDataFile(string strTargetPath)
        {
            switch (kinds.SelectedItem.ToString())
            {
                case "M500HT":
                    strDataPath = @"C:\Mirae HMI\Data";
                        //@"D:\01.factory\internship\03.참고\DataExport\raw\Mirae HMI\Data";
                    break;

                case "Test":
                    strDataPath = @"D:\01.factory\internship\03.참고\DataExport\raw\Mirae HMI\Data";
                    break;

                default:
                    strDataPath = @"C:\MiraeData\" + kinds.SelectedItem + @"\Data";
                    break;
            }


            //strTargetPath = lblTargetPath.Text + "\\"+DateTime.Now.ToString("yyMMdd") +"LE";
            DirectoryInfo diTargetDirectory = new DirectoryInfo(strTargetPath + "\\Data");
            if (diTargetDirectory.Exists == false)
            {
                diTargetDirectory.Create();
            }

            CopyFolder(strDataPath, strTargetPath + "\\Data");
        }

        public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                int flag = 0;
                foreach (var str in exceptData)
                {
                    if (name.Contains(str) == true)
                    {
                        flag += 1;
                    }
                    else
                    {
                        continue;
                    }
                }
                foreach (var str in changeData)
                {
                    if (name.Contains(str) == true)
                    {
                        dest = Path.Combine(destFolder, name+"__");
                    }
                    else
                    {
                        continue;
                    }
                }
                if (flag == 0)
                {
                    File.Copy(file, dest, true);
                }
            }
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }
        private void CopyLogFile()
        {
            DirectoryInfo mcsDirectoryFiles;
            int logfileCount = 0;
            switch (kinds.SelectedItem.ToString())
            {
                case "M500HT":
                    strSourcePath = @"C:\Mirae HMI\History";
                               //@"D:\01.factory\internship\03.참고\DataExport\raw\Mirae HMI\History";
                    strMCSPath = @"C:\";
                    break;
                case "Test":
                    strSourcePath = @"D:\01.factory\internship\03.참고\DataExport\raw\Mirae HMI\History";
                    strMCSPath = @"C:\Users\USER\Desktop";
                    break;
                default:
                    strSourcePath = @"C:\MiraeData\" + kinds.SelectedItem + @"\HMI\History";
                    strMCSPath = @"C:\MiraeData\";
                    break;
            }
            
            try
            {
                mcsDirectory = Directory.GetDirectories(strMCSPath, "LOG", SearchOption.TopDirectoryOnly);
                mcsDirectoryFiles = new DirectoryInfo(strSourcePath +@"\MCS");
                if (mcsDirectoryFiles.Exists == false)
                {
                    mcsDirectoryFiles.Create();
                }
                foreach (string di in mcsDirectory)
                {
                    CopyFolder(di, strSourcePath +@"\MCS");
                }
                
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ErrorMcs.Split(',')[lang]);

            }

            try
            {
                directory = Directory.GetDirectories(strSourcePath, "*", SearchOption.AllDirectories);
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ErrorModel.Split(',')[lang]);
                return;
            }
            
            foreach (var getDir in directory)
            {
                string strDirectoryName = getDir;
                DirectoryInfo diDirectoryFiles = new DirectoryInfo(Path.Combine(strSourcePath, strDirectoryName));

                foreach (var file in diDirectoryFiles.GetFiles())
                {
                    string strFileName = Path.GetFileNameWithoutExtension(file.Name);

                    for (DateTime dtTargetDate = dtStartDate; dtTargetDate <= dtEndDate; dtTargetDate = dtTargetDate.AddDays(1))
                    {
                        string strTargetDate = dtTargetDate.ToString("yyMMdd");

                        if (strFileName.Contains(strTargetDate) == true)
                        {
                            DirectoryInfo diCheckDirectory = new DirectoryInfo(Path.Combine(strTargetPath, strTargetDate));
                            if (diCheckDirectory.Exists == false)
                            {
                                diCheckDirectory.Create();
                            }
                            
                            File.Copy(Path.Combine(strSourcePath, strDirectoryName, file.Name), Path.Combine(strTargetPath, strTargetDate, file.Name), true);
                            if (strList.Contains(file.Name) == false)
                            {
                                strList.Add(file.Name);
                                logfileCount++;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            //try
            //{
            //    if (mcsDirectoryFiles.Exists == false)
            //    {
            //        mcsDirectoryFiles.Delete(true);
            //    }
            //}
            //catch
            //{
            //    MessageBox.Show("error");
            //}
            ////stopwatch.Stop();
            MessageBox.Show(Properties.Resources.Success.Split(',')[lang] + "\r\n" + Properties.Resources.exportLogFiles.Split(',')[lang] + logfileCount + "\r\n");
        }

        private void btnTargetDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbDialog = new FolderBrowserDialog();
            fbDialog.ShowDialog();

            lblTargetPath.Text = fbDialog.SelectedPath;

            fbDialog.Dispose();
        }

        private void dtStartDatePicker_ValueChanged(object sender, EventArgs e)
        {
            dtStartDate = dtStartDatePicker.Value;
        }

        private void dtEndDatePicker_ValueChanged(object sender, EventArgs e)
        {
            dtEndDate = dtEndDatePicker.Value;
        }

        private void btnCollect_Click(object sender, EventArgs e)
        {
            try
            {
                strTargetPath = lblTargetPath.Text;
                DirectoryInfo diTargetDirectoryCheck = new DirectoryInfo(strTargetPath);
                strTargetPath = lblTargetPath.Text + "\\" + DateTime.Now.ToString("yyMMdd") + "LE";

                for (int i = 0; ; i++)
                {

                    if (i == 0)
                    {
                        foldernum = "";
                    }
                    else
                    {
                        foldernum = "(" + i + ")";
                    }
                    if (Directory.Exists(strTargetPath + foldernum) == false)
                    {
                        break;
                    }

                }
                strTargetPath += foldernum;
                DirectoryInfo diTargetDirectory = new DirectoryInfo(strTargetPath);
                if (diTargetDirectory.Exists == false)
                {
                    diTargetDirectory.Create();
                }
                
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ErrorTargetPath.Split(',')[lang]);
                return;
            }

            strList.Clear();
            
            if (logCheck.Checked == true)
            {
                TimeSpan ts = dtEndDate.Subtract(dtStartDate);
                if (DateTime.Compare(dtStartDate, dtEndDate) == 1)
                {
                    MessageBox.Show(Properties.Resources.ErrorDate.Split(',')[lang] + "\r\n");
                    return;
                }
                else if (ts.TotalDays > 365)
                {
                    MessageBox.Show(Properties.Resources.Preiod.Split(',')[lang] + "\r\n");
                    return;
                }
                CopyLogFile();
            }

            if (dataCheck.Checked == true)
            {
                CopyDataFile(strTargetPath);
            }

            try
            {
                DirectoryInfo diTargetDirectory = new DirectoryInfo(strTargetPath);
                if (diTargetDirectory.Exists == true)
                {
                    System.Diagnostics.Process.Start("Explorer.exe", "/select, \"" + strTargetPath + "\"");
                }
            }
            catch { return; }

        }

        private void btnCollectZIP_Click(object sender, EventArgs e)
        {
            try
            {
                strTargetPath = lblTargetPath.Text;
                DirectoryInfo diTargetDirectoryCheck = new DirectoryInfo(strTargetPath);
                strTargetPath = lblTargetPath.Text + "\\" + DateTime.Now.ToString("yyMMdd") + "LE";
                DirectoryInfo diTargetDirectory = new DirectoryInfo(strTargetPath);
                if (diTargetDirectory.Exists == false)
                {
                    diTargetDirectory.Create();
                }
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ErrorTargetPath.Split(',')[lang]);
                return;
            }

            strList.Clear();

            if (logCheck.Checked == true)
            {
                TimeSpan ts = dtEndDate.Subtract(dtStartDate);
                if (DateTime.Compare(dtStartDate, dtEndDate) == 1)
                {
                    MessageBox.Show(Properties.Resources.ErrorDate.Split(',')[lang] + "\r\n");
                    return;
                }
                else if (ts.TotalDays > 365)
                {
                    MessageBox.Show(Properties.Resources.Preiod.Split(',')[lang] + "\r\n");
                    return;
                }
                CopyLogFile();
            }

            if (dataCheck.Checked == true)
            {
                CopyDataFile(strTargetPath);
            }
            string zipnum;
            for (int i = 0;; i++)
            {

                if (i == 0)
                {
                    zipnum = ".zip";
                }
                else
                {
                    zipnum = "(" + i + ").zip";
                }
                if (File.Exists(Path.Combine(strTargetPath, strTargetPath  + zipnum))==false)
                {
                    break;
                }
                
            }

            try
            {
                ZipFile.CreateFromDirectory(strTargetPath, strTargetPath + zipnum);
                Directory.Delete(strTargetPath, true);

            }
            catch (IOException)
            {
                MessageBox.Show(Properties.Resources.ErrorZip.Split(',')[lang] + "\r\n");
                return;
            }
            catch
            {
                MessageBox.Show("Error");
                return;
            }



            System.Diagnostics.Process.Start("Explorer.exe", "/select, \"" + strTargetPath + zipnum + "\"");
        }

        private void btn_ColList_Click(object sender, EventArgs e)
        {
            LogList logList = new LogList(strList);
            logList.ShowDialog();
            logList.Dispose();
        }

        private void btnEndDateTimeChange(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            switch (button.Text)
            {
                case "+1M":
                    dtEndDatePicker.Value = dtEndDate.AddMonths(1);
                    break;
                case "+1W":
                    dtEndDatePicker.Value = dtEndDate.AddDays(7);
                    break;
                case "+1D":
                    dtEndDatePicker.Value = dtEndDate.AddDays(1);
                    break;
                case "-1M":
                    dtEndDatePicker.Value = dtEndDate.AddMonths(-1);
                    break;
                case "-1W":
                    dtEndDatePicker.Value = dtEndDate.AddDays(-7);
                    break;
                case "-1D":
                    dtEndDatePicker.Value = dtEndDate.AddDays(-1);
                    break;
            }

            dtEndDate = dtEndDatePicker.Value;
        }
        private void btnStartDateTimeChange(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            switch (button.Text)
            {
                case "+1M":
                    dtStartDatePicker.Value = dtStartDate.AddMonths(1);
                    break;
                case "+1W":
                    dtStartDatePicker.Value = dtStartDate.AddDays(7);
                    break;
                case "+1D":
                    dtStartDatePicker.Value = dtStartDate.AddDays(1);
                    break;
                case "-1M":
                    dtStartDatePicker.Value = dtStartDate.AddMonths(-1);
                    break;
                case "-1W":
                    dtStartDatePicker.Value = dtStartDate.AddDays(-7);
                    break;
                case "-1D":
                    dtStartDatePicker.Value = dtStartDate.AddDays(-1);
                    break;
            }
            dtStartDate = dtStartDatePicker.Value;
        }
        private void logCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (logCheck.Checked == true)
            {
                groupBox2.Enabled = groupBox3.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = groupBox3.Enabled = false;
            }
        }

   
    }
}
