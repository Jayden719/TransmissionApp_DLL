using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransmissionApp
{
    public partial class ResTrans : Form
    {
        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetResult(string sUserID, string sPwd, int ntype, string sFrom, string sTo);

        [DllImport("BizMoashot.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetContents(string sUserID, string sPwd, string sSvc, string sJobID);

        string suserid = "";
        string spwd = "";
        string sfrom = "";
        string sto = "";
        string sfromTime = "";
        string stoTime = "";
        string combo = "";
        string sjobid = "";
        int ntype;
        string reportPath = Application.StartupPath + "\\Result\\Report.txt";
        string report = "";
        bool conts = false;
        int result = 0;

        public ResTrans()
        {
            InitializeComponent();
        }

        private void ResTrans_Load(object sender, EventArgs e)
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


        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            label7.Text = "";
            suserid = textBox1.Text.Replace(" ", "");
            spwd = textBox2.Text.Replace(" ", "");
            sjobid = textBox3.Text.Replace(" ", "");
            sfromTime = dateTimePicker3.Value.ToString("HHmmss").Replace(" ", "");
            sfrom = dateTimePicker1.Value.ToString("yyyyMMdd").Replace(" ", "") + sfromTime;
            stoTime = dateTimePicker4.Value.ToString("HHmmss").Replace(" ", "");
            sto = dateTimePicker2.Value.ToString("yyyyMMdd").Replace(" ", "") + stoTime;
            combo = comboBox1.SelectedItem.ToString();


            if(conts == true)
            {
                //전송문서 확인 
                if(sjobid == "" && sjobid.Length == 0)
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

                result = GetContents(suserid, spwd, combo, sjobid);

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
                if(sjobid != "" || sjobid.Length != 0)
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text == "kevin719")
            {
                textBox2.Text = "k4152030!!";
                comboBox1.Text = "SMS";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
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
            textBox3.Text = clickJobID;
            checkBox1.Checked = true;
            conts = true;
        }
    }
}
