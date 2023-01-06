using System.Windows.Forms;
using System.IO;
using System;
using System.IO.Compression;
using System.Collections.Generic;
//C://전세준//03.참고//LogExport//01.raw//Mirae HMI//History
namespace LogExport
{
    public partial class frmLogExport : Form
    {
        string strTargetPath;
        string strSourcePath;
        List<string> strList = new List<string>();
        DateTime dtStartDate;
        DateTime dtEndDate;

        public frmLogExport()
        {
            InitializeComponent();
        }
        private void frmLogExport_Load(object sender, EventArgs e)
        {
            dtStartDatePicker.Value = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")));
            dtEndDatePicker.Value = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")),int.Parse(DateTime.Now.ToString("dd")));

            dtStartDate = dtStartDatePicker.Value;
            dtEndDate = dtEndDatePicker.Value;

            String strStartDate = dtStartDate.ToString("yyMMdd");
            String strEndDate = dtEndDate.ToString("yyMMdd");

            kinds.SelectedIndex = 0;
        }

        private void CopyFile()
        {
            int fileCount=0;
            strList.Clear();
            switch (kinds.SelectedItem.ToString())
            {
                case "test":
                    strSourcePath = "C://전세준//03.참고//LogExport//01.raw//Mirae HMI//History";
                    break;
                case "M500HT":
                    strSourcePath = "M500HT";
                    break;
                case "MH7":
                    strSourcePath = "MH7";
                    break;
                case "MH5":
                    strSourcePath = "MH5";
                    break;
                default :
                    MessageBox.Show("Choose Model");
                    break;
            }
            
            try
            {
                DirectoryInfo diTargetDirectory = new DirectoryInfo(strTargetPath);
                if (diTargetDirectory.Exists == false)
                {
                    diTargetDirectory.Create();
                }
            }
            catch
            {
                MessageBox.Show("Please sellect the path");
                return;
            }

            string[] directory = Directory.GetDirectories(strSourcePath, "*", SearchOption.AllDirectories);

            foreach (var getDir in directory)
            {
                string strDirectoryName = getDir;
                DirectoryInfo diDirectoryFiles = new DirectoryInfo(Path.Combine(strSourcePath, strDirectoryName));

                foreach (var file in diDirectoryFiles.GetFiles())
                {
                    string strFileName = Path.GetFileNameWithoutExtension(file.Name);
                    
                    for (DateTime dtTargetDate = dtStartDate; dtTargetDate <= dtEndDate;dtTargetDate = dtTargetDate.AddDays(1))
                    {
                        string strTargetDate = dtTargetDate.ToString("yyMMdd");
                        
                        if(strFileName.Contains(strTargetDate))
                        {
                            DirectoryInfo diCheckDirectory = new DirectoryInfo(Path.Combine(strTargetPath, strTargetDate));
                            if (diCheckDirectory.Exists == false)
                            {
                                diCheckDirectory.Create();
                            }
                            File.Copy(Path.Combine(strSourcePath, strDirectoryName, file.Name), Path.Combine(strTargetPath, strTargetDate, file.Name), true);
                            strList.Add(file.Name + " ");
                            fileCount++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            MessageBox.Show("Export Successed!\r\n" + "Export Files : " + fileCount);
        }
        
        private void btnTargetDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbDialog = new FolderBrowserDialog();
            fbDialog.ShowDialog();

            strTargetPath = fbDialog.SelectedPath;
            lblTargetPath.Text = strTargetPath;

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
            TimeSpan ts = dtEndDate.Subtract(dtStartDate);
            if (DateTime.Compare(dtStartDate, dtEndDate) == 1)
            {
                MessageBox.Show("You must set the date correctly.\r\n");
                return;
            }else if (ts.TotalDays > 365)
            {
                MessageBox.Show("The period must be less than one year.\r\n");
                return;
            }
            CopyFile();
        }

        private void btnCollectZIP_Click(object sender, EventArgs e)
        {
            if (DateTime.Compare(dtStartDatePicker.Value, dtEndDatePicker.Value) == 1)
            {
                MessageBox.Show("You must set the date correctly.\r\n" + dtStartDatePicker.Value + dtEndDatePicker.Value);
                return;
            }
            CopyFile();
            try
            {
                ZipFile.CreateFromDirectory(strTargetPath, strTargetPath + ".zip");
            }
            catch(IOException)
            {
                MessageBox.Show("Error\r\n.Zip File has Faild");
                return;
            }
            catch 
            {
                MessageBox.Show("Error");
                return;
            }
        }

        private void btn_ColList_Click(object sender, EventArgs e)
        {
            Form2 logList = new Form2(strList);
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
    }
}
