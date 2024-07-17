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
   
    public partial class FaxTrans : Form
    {
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MoashotSendFAX(string sUserID, string sPwd, string sFrom, string sTo, string sContents, string sTitle,
                                                string sIndexCode, string sTime);

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
        private string logPath = Application.StartupPath + "\\log\\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        private string CurTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

        public FaxTrans()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label9.Text = "";
            suserid = textBox1.Text.Replace(" ", "");
            spwd = textBox2.Text.Replace(" ", "");
            sfrom = textBox3.Text.Replace(" ", "").Replace("-","");
            sto = textBox4.Text.Replace(" ", "").Replace("-","");
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
            if (suserid==""||suserid.Length==0)
            {
                MessageBox.Show("아이디를 입력해주세요");
                return;
            }
            if (spwd==""||spwd.Length==0)
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
           /* if(Regex.IsMatch(scontents, @"[ㄱ-ㅎ가-힣]"))
            {
                MessageBox.Show("문서 경로에 한글이 있습니다. 영문 경로로 변경해주세요");
                return;
            }*/
                
            int result = MoashotSendFAX(suserid, spwd, sfrom, sto, scontents, stitle, sindexcode, stime);
            if(result == 0)
            {
                label9.Text = "성공";
                if(stime =="" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " FAX " + " 발송번호 " + sfrom + " " + " 수신번호 " + sto + " 발송 완료", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " FAX " + " 발송번호 " + sfrom + " " + " 수신번호 " + sto + " " + sRdate + " 예약접수 완료", Encoding.Default);
                }

            }
            else if(result == 1)
            {
                label9.Text = "실패";
                if(stime == "" || stime.Length == 0)
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 발송 실패 ", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(logPath, "\r\n" + CurTime + " " + " 발송 실패 ", Encoding.Default);
                }
                    
            }
            else if(result ==2 || result == 3 || result == 4 || result == 5)
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
            else if(result == 6)
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
            else if(result == 7)
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
            else if(result == 8)
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
            else if(result == 9)
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
            else if(result == 10)
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

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                string ofdDirectory = ofd.FileName;
                textBox5.Text = ofdDirectory;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(label9.Text != "" || label9.Text.Length != 0)
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
            if(textBox1.Text == "kevin719")
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
    }
}
