using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        public static string ipstr;
        public static int portnum;
        public static string passwordstr;

        private Point mouseOff; //抓取窗体Form中的鼠标的坐标,需要设置一个参数
        private bool leftFlag;  //标签，用来标记鼠标的左键的状态
        private void FrmMain_MouseDown(object sender, MouseEventArgs e)  //鼠标左键按下后触发的MouseDown事件
        {
            if (e.Button == MouseButtons.Left)   //判断鼠标左键是否被按下
            {
                mouseOff = new Point(e.X, e.Y); //通过结构，将鼠标在窗体中的坐标（e.X,e.Y）赋值给mouseOff参数
                leftFlag = true;    //标记鼠标左键的状态
            }
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)  //鼠标移动触发的MouseMove事件
        {
            if (leftFlag)    //判断，鼠标左键是否被按下
            {
                Point mouseSet = Control.MousePosition; //抓取屏幕中鼠标光标所在的位置
                mouseSet.Offset(-mouseOff.X, -mouseOff.Y);  //两个坐标相减，得到窗体左上角相对于屏幕的坐标
                Location = mouseSet;    //将上面得到的坐标赋值给窗体Form的Location属性
            }
        }


        private void FrmMain_MouseUp(object sender, MouseEventArgs e)    //鼠标释放按键后触发的MouseUp事件
        {
            if (leftFlag)
            {
                leftFlag = false;
            }
        }









        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //connect



            Form2 form2 = new Form2();
            ipstr = textBox1.Text;
            int.TryParse(textBox2.Text, out portnum);
            passwordstr = textBox3.Text;
            form2.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Start();

        }



        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity >= 0.05)
                this.Opacity -= 0.05;
            else
            {
                timer.Stop();
                this.Close();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void FrmLogin_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
