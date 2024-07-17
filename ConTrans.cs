using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransmissionApp
{
    public partial class ConTrans : Form
    {
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetContents(string sUserID, string sPwd, string sSvc, string sJobID);

        string suserid = "";
        string spwd = "";
        string ssvc = "";
        string sjobid = "";
       
        public ConTrans()
        {
            InitializeComponent();
        }

        private void ConTrans_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("FAX");
            comboBox1.Items.Add("SMS");
            comboBox1.Items.Add("LMS");
            comboBox1.Items.Add("MMS");
            //FAX 초기값 지정
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            suserid = textBox1.Text.Replace(" ", "");
            spwd = textBox2.Text.Replace(" ", "");
            sjobid = textBox3.Text.Replace(" ", "");
            ssvc = comboBox1.SelectedItem.ToString();

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
            if(sjobid == "" || sjobid.Length == 0)
            {
                MessageBox.Show("잡아이디를 입력해주세요");
                return;
            }

            int result = GetContents(suserid, spwd, ssvc, sjobid);

            if (result == 0)
            {
                label7.Text = "성공";
            }
            else if (result == 1)
            {
                label7.Text = "실패";
            }
            else if (result == 2 || result == 3 || result == 4 || result == 5)
            {
                label7.Text = "서버 접속 실패";
            }
            else if (result == 6)
            {
                label7.Text = "파일경로 에러";
            }
            else if (result == 7)
            {
                label7.Text = "파일쓰기 에러";
            }
            else if (result == 8)
            {
                label7.Text = "파일읽기 에러";
            }
            else if (result == 9)
            {
                label7.Text = "서비스 타입 에러";
            }
            else if (result == 10)
            {
                label7.Text = "결과 내용 없음";
            }
        }
    }
}
