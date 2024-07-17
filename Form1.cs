using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransmissionApp
{
    public partial class Form1 : Form
    {
        //FAX 전송 마샬링
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MoashotSendFAX(string sUserID, string sPwd, string sFrom, string sTo, string sContents, string sTitle,
                                               string sIndexCode, string sTime);

        //SMS 전송 마샬링
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MoashotSendSMS(string sUserID, string sPwd, string sFrom, string sTo, string sContents, string sTitle,
                                                string sIndexCode, string sTime);

        //LMS 전송 마샬링
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MoashotSendLMS(string sUserID, string sPwd, string sFrom, string sTo, string sContents, string sTitle,
                                               string sIndexCode, string sTime);

        //MMS 전송 마샬링
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MoashotSendMMS(string sUserID, string sPwd, string sFrom, string sTo, string sContents, string sText, string sTitle,
                                               string sIndexCode, string sTime);

        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetResult(string sUserID, string sPwd, int ntype, string sFrom, string sTo);

        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetContents(string sUserID, string sPwd, string sSvc, string sJobID);

        string suserid = "";
        string spwd = "";
        string sfrom = "";
        string sto = "";
        string scontents = "";
        string stitle = "";
        string sindexcode = "";
        string stime = "";
        string sTime = "";
        string sDate = "";
        string sRdate = "";
        string sRtime = "";

        string sText = "";
        bool mmsContents = false;
        bool resChk = false;
        int MsgType = 0;
        private string CurTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

        string sfromTime = "";
        string stoTime = "";
        string combo = "";
        string sjobid = "";
        int ntype;
        string reportPath = Application.StartupPath + "\\Result\\Report.txt";
        string report = "";
        bool conts = false;
        int result = 0;

        private static string logPath = Application.StartupPath + "\\log\\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        private Point _imageLocation = new Point(20, 4);
        private Point _imgHitArea = new Point(20, 4);
        Image closeImage;

        public Form1()
        {
            InitializeComponent();                  
        }

            

        private void 전송결과ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 팩스전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fAXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FaxTrans ft = new FaxTrans();     
            //ft.Show();
            if (!tabControl1.Controls.Contains(tabPage1))
            {
                tabControl1.Controls.Add(tabPage1);
                tabControl1.SelectedTab = tabPage1;
            }
            else
            {
                tabControl1.SelectedTab = tabPage1;
            }
            textBox1.Focus();
        }

        private void sMS전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SmsTrans st = new SmsTrans();
            st.Show();
        }

        private void lMS전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LmsTrans lt = new LmsTrans();
            lt.Show();
        }

        private void mMS전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MmsTrans mt = new MmsTrans();
            mt.Show();
        }

        private void 전송결과ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //ResTrans rt = new ResTrans();
            //rt.Show();
            if (!tabControl1.Controls.Contains(tabPage3))
            {
                tabControl1.Controls.Add(tabPage3);
                tabControl1.SelectedTab = tabPage3;
            }
            else
            {
                tabControl1.SelectedTab = tabPage3;
            }
            textBox1.Focus();
        }

        private void 전송문서확인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConTrans ct = new ConTrans();
            ct.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("FAX");
            comboBox1.Items.Add("SMS");
            comboBox1.Items.Add("LMS");
            comboBox1.Items.Add("MMS");
            //FAX 초기값 지정
            comboBox1.SelectedIndex = 0;

            listView1.View = View.Details;
            listView1.Columns.Add("서비스", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("제목", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("장수", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("접수번호", 140, HorizontalAlignment.Center);
            listView1.Columns.Add("접수시간", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("완료시간", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("성공건", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("실패건", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("결과", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("상태", 60, HorizontalAlignment.Center);



            closeImage = Properties.Resources.red_cancel;
            tabControl1.Padding = new Point(15, 4);
            
            this.tabControl1.TabPages.Remove(this.tabPage1);
            this.tabControl1.TabPages.Remove(this.tabPage2);
            this.tabControl1.TabPages.Remove(this.tabPage3);

            this.Text = string.Format("전송프로그램_DLL연동 ver_{0}", Application.ProductVersion);
            string folderPath = Application.StartupPath + "\\log";
            DirectoryInfo di = new DirectoryInfo(folderPath);

            if (di.Exists == false)
            {
                di.Create();
            }

            System.IO.FileInfo fi = new System.IO.FileInfo(logPath);
            if (!fi.Exists)
            {
                string curTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                System.IO.File.WriteAllText(logPath, curTime + " Log 파일 최초 생성 ", Encoding.Default);
            }
        }

        private void 문자전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MsgTrans mt = new MsgTrans();
            //mt.Show();
            if (!tabControl1.Controls.Contains(tabPage2))
            {
                tabControl1.Controls.Add(tabPage2);
                tabControl1.SelectedTab = tabPage2;
            }
            else
            {
                tabControl1.SelectedTab = tabPage2;
            }
            textBox1.Focus();
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            Point p = e.Location;
            int _tabWidth = 0;
            _tabWidth = this.tabControl1.GetTabRect(tabControl.SelectedIndex).Width - (_imgHitArea.X);
            Rectangle r = this.tabControl1.GetTabRect(tabControl.SelectedIndex);
            r.Offset(_tabWidth, _imgHitArea.Y);
            r.Width = 16;
            r.Height = 16;
            if (tabControl1.SelectedIndex >= 0)
            {
                if (r.Contains(p))
                {
                    TabPage tabPage = (TabPage)tabControl.TabPages[tabControl.SelectedIndex];
                    tabControl.TabPages.Remove(tabPage);
                }
            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Image img = new Bitmap(closeImage);
            Rectangle r = e.Bounds;
            r = this.tabControl1.GetTabRect(e.Index);
            r.Offset(2, 2);
            Brush TitleBrush = new SolidBrush(Color.Black);
            Font f = this.Font;
            string title = this.tabControl1.TabPages[e.Index].Text;
            e.Graphics.DrawString(title, f, TitleBrush, new PointF(r.X, r.Y));
            e.Graphics.DrawImage(img, new Point(r.X + (this.tabControl1.GetTabRect(e.Index).Width - _imageLocation.X), _imageLocation.Y));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label9.Text = "";
            suserid = textBox1.Text.Replace(" ", "");
            spwd = textBox2.Text.Replace(" ", "");
            sfrom = textBox3.Text.Replace(" ", "").Replace("-", "");
            sto = textBox4.Text.Replace(" ", "").Replace("-", "");
            scontents = textBox5.Text.Trim();
            stitle = textBox6.Text;
            sDate = dateTimePicker1.Value.ToString("yyyyMMdd").Replace(" ", "");
            sTime = dateTimePicker2.Value.ToString("HHmmss").Replace(" ", "");
            sindexcode = textBox7.Text.Replace(" ", "");
            sRdate = dateTimePicker1.Value.ToString("yyyy/MM/dd").Replace(" ", "");
            sRtime = dateTimePicker2.Value.ToString("hh:mm:ss").Replace(" ", "");

            stime = sDate + sTime;
            sRdate = sRdate + " " + sRtime;

            // client 정보 유효성 검사
            if (suserid == "" || suserid.Length == 0)
            {
                MessageBox.Show("아이디를 입력해주세요");
                return;
            }
            if (spwd == "" || spwd.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력해주세요");
                return;
            }
            if (sfrom == "" || sfrom.Length == 0)
            {
                MessageBox.Show("발신번호를 입력해주세요");
                return;
            }
            if (sto == "" || sto.Length == 0)
            {
                MessageBox.Show("수신번호를 입력해주세요");
                return;
            }
            if (scontents == "" || scontents.Length == 0)
            {
                MessageBox.Show("발송할 팩스문서 절대경로를 입력해주세요");
                return;
            }
            if (stitle == "" || stitle.Length == 0)
            {
                MessageBox.Show("팩스 제목을 입력해주세요");
                return;
            }
            if (!res_chk.Checked)
            {
                stime = "";
            }

            int result = MoashotSendFAX(suserid, spwd, sfrom, sto, scontents, stitle, sindexcode, stime);
            if (result == 0)
            {
                label9.Text = "성공";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " FAX " + " 발송번호 " + sfrom + " " + " 수신번호 " + sto + " 발송 완료", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " FAX " + " 발송번호 " + sfrom + " " + " 수신번호 " + sto + " " + sRdate + " 예약접수 완료", Encoding.Default);
                }

            }
            else if (result == 1)
            {
                label9.Text = "실패";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 발송 실패 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 발송 실패 ", Encoding.Default);
                }

            }
            else if (result == 2 || result == 3 || result == 4 || result == 5)
            {
                label9.Text = "서버 접속 실패";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 서버 접속 실패 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 서버 접속 실패 ", Encoding.Default);
                }
            }
            else if (result == 6)
            {
                label9.Text = "파일경로 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 파일경로 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 파일경로 에러 ", Encoding.Default);
                }
            }
            else if (result == 7)
            {
                label9.Text = "파일쓰기 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 파일쓰기 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 파일쓰기 에러 ", Encoding.Default);
                }
            }
            else if (result == 8)
            {
                label9.Text = "파일읽기 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 파일읽기 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 파일읽기 에러 ", Encoding.Default);
                }
            }
            else if (result == 9)
            {
                label9.Text = "서비스 타입 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 서비스 타입 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 서비스 타입 에러 ", Encoding.Default);
                }
            }
            else if (result == 10)
            {
                label9.Text = "결과 내용 없음";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 결과 내용 없음 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 결과 내용 없음 ", Encoding.Default);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "모든 파일 | *.*";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ofdDirectory = ofd.FileName;
                textBox5.Text = ofdDirectory;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (label9.Text != "" || label9.Text.Length != 0)
            {
                label9.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
            }
            if (textBox1.Text == "kevin719")
            {
                textBox2.Text = "k4152030!!";
                textBox3.Text = "02-2178-9655";
                textBox4.Text = "02-2178-9655";
                textBox6.Text = "테스트";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (label9.Text != "" || label9.Text.Length != 0)
            {
                label9.Text = "";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (label9.Text != "" || label9.Text.Length != 0)
            {
                label9.Text = "";
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (label9.Text != "" || label9.Text.Length != 0)
            {
                label9.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            label9.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "모든 파일 | *.*";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ofdDirectory = ofd.FileName;
                textBox8.Text = ofdDirectory;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string text = richTextBox1.Text;
            char[] tempChr = text.ToCharArray();
            BchkMethod(text, tempChr);
        }

        private void BchkMethod(string text, char[] tempChr)
        {
            int bCnt = 0;

            foreach (char c in tempChr)
            {
                int chr = (int)c;
                if (chr > 122)
                {
                    bCnt += 2;
                }
                else
                {
                    bCnt += 1;
                }
            }
            label10.Text = bCnt.ToString();

            if (mmsContents == true)
            {
                label13.Text = "MMS전송";
            }
            else
            {
                if (0 < bCnt && bCnt <= 90)
                {
                    label13.Text = "SMS전송";

                }
                else if (bCnt > 90 && bCnt <= 2000)
                {
                    label13.Text = "LMS전송";
                }
                else if (bCnt > 2000)
                {
                    MessageBox.Show("글자수 초과되었습니다");
                    text = text.Substring(0, text.Length - 1);
                    richTextBox1.Text = text;
                    richTextBox1.Select(richTextBox1.Text.Length, 0);
                    char[] cChr = text.ToCharArray();
                    BchkMethod(text, cChr);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            suserid = textBox14.Text.Replace(" ", "");
            spwd = textBox13.Text.Replace(" ", "");
            sfrom = textBox12.Text.Replace(" ", "").Replace("-", "");
            sto = textBox11.Text.Replace(" ", "").Replace("-", "");
            sText = richTextBox1.Text;
            stitle = textBox9.Text;
            stime = dateTimePicker4.Value.ToString("yyyyMMddHHmmss").Replace(" ", "");
            sindexcode = textBox10.Text.Replace(" ", "");
            scontents = textBox8.Text;

            sDate = dateTimePicker4.Value.ToString("yyyyMMdd").Replace(" ", "");
            sTime = dateTimePicker3.Value.ToString("HHmmss").Replace(" ", "");
            sRdate = dateTimePicker4.Value.ToString("yyyy/MM/dd").Replace(" ", "");
            sRtime = dateTimePicker3.Value.ToString("hh:mm:ss").Replace(" ", "");

            stime = sDate + sTime;
            sRdate = sRdate + " " + sRtime;
            int result = 0;


            // client 정보 유효성 검사
            if (suserid == "" || suserid.Length == 0)
            {
                MessageBox.Show("아이디를 입력해주세요");
                return;
            }
            if (spwd == "" || spwd.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력해주세요");
                return;
            }
            if (sfrom == "" || sfrom.Length == 0)
            {
                MessageBox.Show("발신번호를 입력해주세요");
                return;
            }
            if (sto == "" || sto.Length == 0)
            {
                MessageBox.Show("수신번호를 입력해주세요");
                return;
            }
            if (sText == "" || sText.Length == 0)
            {
                MessageBox.Show("문자 내용을 입력해주세요");
                return;
            }
            if (stitle == "" || stitle.Length == 0)
            {
                MessageBox.Show("문자 제목을 입력해주세요");
                return;
            }
            if (resChk != true)
            {
                stime = "";
            }

            // MsgType 1:SMS 2:LMS 3:MMS
            if (mmsContents == true)
            {
                Console.WriteLine("MMS 전송" + scontents);
                //MMS 전송
                result = MoashotSendMMS(suserid, spwd, sfrom, sto, scontents, sText, stitle, sindexcode, stime);
                MsgType = 3;
            }
            else
            {
                if (textBox8.Text != "" && textBox8.Text.Length > 0)
                {
                    MessageBox.Show("MMS 체크박스를 눌러주세요");
                    return;
                }
                else
                {
                    if (Int32.Parse(label10.Text) <= 90)
                    {
                        //SMS 전송
                        result = MoashotSendSMS(suserid, spwd, sfrom, sto, sText, stitle, sindexcode, stime);
                        MsgType = 1;
                    }
                    else if (Int32.Parse(label10.Text) > 90 && Int32.Parse(label10.Text) <= 2000)
                    {
                        //LMS 전송
                        result = MoashotSendLMS(suserid, spwd, sfrom, sto, sText, stitle, sindexcode, stime);
                        MsgType = 2;
                    }
                    else
                    {
                        MessageBox.Show("바이트 수 초과되었습니다");
                    }
                }
            }

            string MsgTypeName;

            if (MsgType == 1)
            {
                MsgTypeName = "SMS";
            }
            else if (MsgType == 2)
            {
                MsgTypeName = "LMS";
            }
            else
            {
                MsgTypeName = "MMS";
            }

            if (result == 0)
            {
                label16.Text = "성공";
                if (stime == "" || stime.Length == 0)
                {


                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + MsgTypeName + " " + " 발송번호 " + sfrom + " " + " 수신번호 " + sto + " 발송 완료", Encoding.Default);
                }
                else
                {

                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + MsgTypeName + " " + " 발송번호 " + sfrom + " " + " 수신번호 " + sto + " " + sRdate + " 예약접수 완료", Encoding.Default);
                }
            }
            else if (result == 1)
            {
                label16.Text = "실패";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "발송 실패 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "발송 실패 ", Encoding.Default);
                }
            }
            else if (result == 2 || result == 3 || result == 4 || result == 5)
            {
                label16.Text = "서버 접속 실패";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "서버 접속 실패 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "서버 접속 실패 ", Encoding.Default);
                }
            }
            else if (result == 6)
            {
                label16.Text = "파일경로 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "파일경로 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "파일경로 에러 ", Encoding.Default);
                }
            }
            else if (result == 7)
            {
                label16.Text = "파일쓰기 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "파일쓰기 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "파일쓰기 에러 ", Encoding.Default);
                }
            }
            else if (result == 8)
            {
                label16.Text = "파일읽기 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "파일읽기 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "파일읽기 에러 ", Encoding.Default);
                }
            }
            else if (result == 9)
            {
                label16.Text = "서비스 타입 에러";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "서비스 타입 에러 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "서비스 타입 에러 ", Encoding.Default);
                }
            }
            else if (result == 10)
            {
                label16.Text = "결과 내용 없음";
                if (stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "결과 내용 없음 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + "결과 내용 없음 ", Encoding.Default);
                }
            }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            if (label16.Text != "" || label16.Text.Length != 0)
            {
                label16.Text = "";
                textBox14.Text = "";
                textBox13.Text = "";
                textBox10.Text = "";
                textBox11.Text = "";
                textBox12.Text = "";
                textBox9.Text = "";
                textBox8.Text = "";
                richTextBox1.Text = "";
            }
            if (textBox14.Text == "kevin719")
            {
                textBox13.Text = "k4152030!!";
                textBox11.Text = "010-8306-6137";
                textBox12.Text = "010-8306-6137";
                textBox9.Text = "테스트";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
            richTextBox1.Text = "";
            label16.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                mmsContents = true;
                string text = richTextBox1.Text;
                char[] tempChr = text.ToCharArray();
                BchkMethod(text, tempChr);
            }
            else
            {
                int cnt = 0;
                mmsContents = false;
                string text = richTextBox1.Text;
                char[] tempChr = text.ToCharArray();
                foreach (char c in tempChr)
                {
                    int chr = (int)c;
                    if (chr > 122)
                    {
                        cnt += 2;
                    }
                    else
                    {
                        cnt += 1;
                    }
                }
                if (cnt == 0)
                {
                    label13.Text = "";
                }
                else if (cnt > 0 && cnt <= 90)
                {
                    label13.Text = "SMS전송";
                }
                else if (cnt > 90 && cnt <= 2000)
                {
                    label13.Text = "LMS전송";
                }
                else
                {
                    MessageBox.Show("바이트 수 초가되었습니다");
                }

                BchkMethod(text, tempChr);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                resChk = true;

            }
            else
            {
                resChk = false;

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            label24.Text = "";
            suserid = textBox17.Text.Replace(" ", "");
            spwd = textBox16.Text.Replace(" ", "");
            sjobid = textBox15.Text.Replace(" ", "");
            sfromTime = dateTimePicker6.Value.ToString("HHmmss").Replace(" ", "");
            sfrom = dateTimePicker8.Value.ToString("yyyyMMdd").Replace(" ", "") + sfromTime;
            stoTime = dateTimePicker5.Value.ToString("HHmmss").Replace(" ", "");
            sto = dateTimePicker7.Value.ToString("yyyyMMdd").Replace(" ", "") + stoTime;
            combo = comboBox1.SelectedItem.ToString();


            if (conts == true)
            {
                //전송문서 확인 
                if (sjobid == "" && sjobid.Length == 0)
                {
                    MessageBox.Show("잡아이디를 입력해주세요");
                    return;
                }
                if (suserid == "" || suserid.Length == 0)
                {
                    MessageBox.Show("아이디를 입력해주세요");
                    return;
                }
                if (spwd == "" || spwd.Length == 0)
                {
                    MessageBox.Show("비밀번호를 입력해주세요");
                    return;
                }
                Console.WriteLine(combo);
                result = GetContents(suserid, spwd, combo, sjobid);
                Console.Write(result);
            }
            else
            {
                //전송결과 확인
                if (combo == "FAX")
                {
                    ntype = 1;

                }
                else if (combo == "SMS")
                {
                    ntype = 3;

                }
                else if (combo == "LMS")
                {
                    ntype = 5;
                }
                else
                {
                    ntype = 6;
                }

                if (suserid == "" || suserid.Length == 0)
                {
                    MessageBox.Show("아이디를 입력해주세요");
                    return;
                }
                if (spwd == "" || spwd.Length == 0)
                {
                    MessageBox.Show("비밀번호를 입력해주세요");
                    return;
                }
                if (sfrom == "" || sfrom.Length == 0)
                {
                    MessageBox.Show("시작시간을 입력해주세요");
                    return;
                }
                if (sto == "" || sto.Length == 0)
                {
                    MessageBox.Show("종료시간을 입력해주세요");
                    return;
                }
                if (sjobid != "" || sjobid.Length != 0)
                {
                    textBox3.Text = "";
                }

                result = GetResult(suserid, spwd, ntype, sfrom, sto);

                StreamReader sr = new StreamReader(reportPath, Encoding.Default);
                while (sr.Peek() > -1)
                {
                    report = sr.ReadLine().ToString();
                    string[] reportArray = report.Split('\t');
                    string rType = reportArray[0];
                    string rResult = reportArray[9];
                    string rStatus = reportArray[10];

                    //전송결과 네이밍
                    if (rType == "FAX")
                    {
                        if (rResult == "0")
                        {
                            rResult = "전송대기";
                        }
                        else if (rResult == "1")
                        {
                            rResult = "성공";
                        }
                        else if (rResult == "3")
                        {
                            rResult = "차단번호(블럭)";
                        }
                        else if (rResult == "4")
                        {
                            rResult = "전화번호오류";
                        }
                        else if (rResult == "5")
                        {
                            rResult = "전송취소";
                        }
                        else if (rResult.Substring(0, 1) == "5" & rResult.Length == 3)
                        {
                            rResult = "연결끊김";
                        }
                        else if (rResult == "6")
                        {
                            rResult = "중복번호";
                        }
                        else if (rResult == "769")
                        {
                            rResult = "연결끊김";
                        }
                        else if (rResult == "770")
                        {
                            rResult = "응답없음";
                        }
                        else if (rResult == "773")
                        {
                            rResult = "통화중";
                        }
                        else if (rResult == "777")
                        {
                            rResult = "전화번호오류";
                        }
                        else if (rResult == "778")
                        {
                            rResult = "전화번호오류";
                        }
                        else if (rResult == "83")
                        {
                            rResult = "문서만들기 실패";
                        }
                        else if (rResult == "92")
                        {
                            rResult = "중복번호";
                        }
                        else if (rResult == "93")
                        {
                            rResult = "전화번호오류";
                        }
                        else if (rResult == "99")
                        {
                            rResult = "전송차단";
                        }
                        else if (rResult == "774")
                        {
                            rResult = "라인혼잡";
                        }
                        else if (rResult == "91")
                        {
                            rResult = "전송취소";
                        }

                    }
                    else
                    {
                        if (rResult == "0")
                        {
                            rResult = "전송대기";
                        }
                        else if (rResult == "1")
                        {
                            rResult = "성공";
                        }
                        else if (rResult == "05")
                        {
                            rResult = "번호오류";
                        }
                        else if (rResult == "07")
                        {
                            rResult = "결번 및 정지";
                        }
                        else if (rResult == "08")
                        {
                            rResult = "단말기전원꺼짐";
                        }
                        else if (rResult == "09")
                        {
                            rResult = "음영지역";
                        }
                        else if (rResult == "10")
                        {
                            rResult = "단말기메시지꽉참";
                        }
                        else if (rResult == "11")
                        {
                            rResult = "기타에러";
                        }
                        else if (rResult == "13")
                        {
                            rResult = "번호이동";
                        }
                        else if (rResult == "14")
                        {
                            rResult = "무선망에러";
                        }
                        else if (rResult == "18")
                        {
                            rResult = "메시지중복오류";
                        }
                        else if (rResult == "20")
                        {
                            rResult = "기타에러";
                        }
                        else if (rResult == "45")
                        {
                            rResult = "미등록발신번호";
                        }
                        else if (rResult == "2001")
                        {
                            rResult = "번호오류";
                        }
                        else if (rResult == "3000")
                        {
                            rResult = "MMS 미지원 단말";
                        }
                        else if (rResult == "3002")
                        {
                            rResult = "요청시간오류";
                        }
                        else if (rResult == "3006")
                        {
                            rResult = "기타실패";
                        }
                        else if (rResult == "4001")
                        {
                            rResult = "단말기 일시 서비스 정지";
                        }
                        else if (rResult == "4002")
                        {
                            rResult = "네트워크 에러발생";
                        }
                        else if (rResult == "9002")
                        {
                            rResult = "폰넘버 에러";
                        }
                        else if (rResult == "92")
                        {
                            rResult = "중복번호";
                        }
                        else if (rResult == "93")
                        {
                            rResult = "시간초과";
                        }
                        else if (rResult == "94")
                        {
                            rResult = "수신번호형식오류";
                        }
                        else if (rResult == "91")
                        {
                            rResult = "전송취소";
                        }
                    }

                    //전송상태 네이밍
                    if (rStatus == "0")
                    {
                        rStatus = "전송대기";
                    }
                    else if (rStatus == "1")
                    {
                        rStatus = "전송중";
                    }
                    else if (rStatus == "10")
                    {
                        rStatus = "문서변환";
                    }
                    else if (rStatus == "100")
                    {
                        rStatus = "전송완료";
                    }
                    else if (rStatus == "11")
                    {
                        rStatus = "전송대기";
                    }
                    else if (rStatus == "12")
                    {
                        rStatus = "전송중";
                    }
                    else if (rStatus == "2")
                    {
                        rStatus = "전송완료";
                    }
                    else if (rStatus == "20" || rStatus == "200" || rStatus == "21")
                    {
                        rStatus = "전송중";
                    }
                    else if (rStatus == "250")
                    {
                        rStatus = "문서변환중";
                    }
                    else if (rStatus == "3")
                    {
                        rStatus = "전송대기";
                    }
                    else if (rStatus == "4")
                    {
                        rStatus = "대기중";
                    }
                    ListViewItem item = new ListViewItem(reportArray[0]);
                    item.SubItems.Add(reportArray[1]);
                    item.SubItems.Add(reportArray[2]);
                    item.SubItems.Add(reportArray[3]);
                    item.SubItems.Add(reportArray[5]);
                    item.SubItems.Add(reportArray[6]);
                    item.SubItems.Add(reportArray[7]);
                    item.SubItems.Add(reportArray[8]);
                    item.SubItems.Add(rResult);
                    item.SubItems.Add(rStatus);
                    listView1.Items.Add(item);
                }
                sr.Close();

                File.WriteAllText(reportPath, string.Empty);
            }

            if (result == 0)
            {
                label24.Text = "성공";
            }
            else if (result == 1)
            {
                label24.Text = "실패";
            }
            else if (result == 2 || result == 3 || result == 4 || result == 5)
            {
                label24.Text = "서버 접속 실패";
            }
            else if (result == 6)
            {
                label24.Text = "파일경로 에러";
            }
            else if (result == 7)
            {
                label24.Text = "파일쓰기 에러";
            }
            else if (result == 8)
            {
                label24.Text = "파일읽기 에러";
            }
            else if (result == 9)
            {
                label24.Text = "서비스 타입 에러";
            }
            else if (result == 10)
            {
                label24.Text = "결과 내용 없음";
            }
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            if (textBox17.Text == "kevin719")
            {
                textBox16.Text = "k4152030!!";
                comboBox1.Text = "SMS";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                conts = true;
            }
            else
            {
                conts = false;
            }
        }

        private void listview1_doubleClick(object sender, MouseEventArgs e)
        {
            string clickJobID = listView1.FocusedItem.SubItems[3].Text;
            textBox15.Text = clickJobID;
            checkBox3.Checked = true;
            conts = true;
        }
    }
}

