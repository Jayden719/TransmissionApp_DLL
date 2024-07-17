using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransmissionApp
{
    public partial class MsgTrans : Form
    {
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

        string suserid = "";
        string spwd = "";
        string sfrom = "";
        string sto = "";
        string scontents = "";
        string stitle = "";
        string sindexcode = "";
        string stime = "";
        string sText = "";
        bool mmsContents = false;
        bool resChk = false;
        int MsgType = 0;

        string sTime = "";
        string sDate = "";
        string sRdate = "";
        string sRtime = "";
        private string logPath = Application.StartupPath + "\\log\\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        private string CurTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

        public MsgTrans()
        {
            InitializeComponent();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

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
                textBox6.Text = ofdDirectory;
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
                else if(bCnt > 2000)
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

        private void button1_Click(object sender, EventArgs e)
        {
            suserid = textBox1.Text.Replace(" ", "");
            spwd = textBox2.Text.Replace(" ", "");
            sfrom = textBox3.Text.Replace(" ", "").Replace("-", "");
            sto = textBox4.Text.Replace(" ", "").Replace("-", "");
            sText = richTextBox1.Text;
            stitle = textBox5.Text;
            stime = dateTimePicker1.Value.ToString("yyyyMMddHHmmss").Replace(" ", "");
            sindexcode = textBox7.Text.Replace(" ", "");
            scontents = textBox6.Text;

            sDate = dateTimePicker1.Value.ToString("yyyyMMdd").Replace(" ", "");
            sTime = dateTimePicker2.Value.ToString("HHmmss").Replace(" ", "");
            sRdate = dateTimePicker1.Value.ToString("yyyy/MM/dd").Replace(" ", "");
            sRtime = dateTimePicker2.Value.ToString("hh:mm:ss").Replace(" ", "");

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
            if(mmsContents == true)
            {
                Console.WriteLine("MMS 전송" + scontents);
                //MMS 전송
                result = MoashotSendMMS(suserid, spwd, sfrom, sto, scontents, sText, stitle, sindexcode, stime);
                MsgType = 3;
            }else
            {
                if (textBox6.Text != "" && textBox6.Text.Length > 0)
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
                    }else if(Int32.Parse(label10.Text) > 90 && Int32.Parse(label10.Text) <= 2000)
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


            /* if (Regex.IsMatch(scontents, @"[ㄱ-ㅎ가-힣]"))
             {
                 MessageBox.Show("문서 경로에 한글이 있습니다. 영문 경로로 변경해주세요");
                 return;
             }*/

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
                label9.Text = "성공";
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
                label9.Text = "실패";
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
                label9.Text = "서버 접속 실패";
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
                label9.Text = "파일경로 에러";
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
                label9.Text = "파일쓰기 에러";
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
                label9.Text = "파일읽기 에러";
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
                label9.Text = "서비스 타입 에러";
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
                label9.Text = "결과 내용 없음";
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
                richTextBox1.Text = "";
            }
            if (textBox1.Text == "kevin719")
            {
                textBox2.Text = "k4152030!!";
                textBox3.Text = "010-8306-6137";
                textBox4.Text = "010-8306-6137";
                textBox5.Text = "테스트";
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
            richTextBox1.Text = "";
            label9.Text = "";
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
                foreach(char c in tempChr)
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
                }else if(cnt > 0 && cnt <= 90)
                {
                    label13.Text = "SMS전송";
                }else if(cnt > 90 && cnt <= 2000)
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

        private void res_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (res_chk.Checked == true)
            {
                resChk = true;
              
            }
            else
            {
                resChk = false;
              
            }
        }
    }
}
