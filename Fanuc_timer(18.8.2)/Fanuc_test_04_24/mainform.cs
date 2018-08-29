using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using FOCAS_CLASS;

namespace Fanuc_test_04_24
{
    public partial class mainform : Skin_Color
    {
        public mainform()
        {
            InitializeComponent();
        }

        FOCAS_class a = new FOCAS_class();
        Model model = new Model();


        //连接句柄
        private void skinButton1_Click(object sender, EventArgs e)
        {
            int ret;
            string ip = skinTextBox1.Text;
            string port = skinTextBox5.Text;
            string timeout = skinTextBox4.Text;
            ret = a.Connect_suc(ip, port, timeout, out FOCAS_CLASS.Handle.h1);
            if (ret == Focas1.EW_OK)
            {
                skinTextBox6.Text = "连接成功";
            }
            else
            {
                skinTextBox6.Text = "连接失败";
            }
        }

        //释放句柄
        private void skinButton2_Click(object sender, EventArgs e)
        {
            int ret = a.Free_handle(FOCAS_CLASS.Handle.h1);
            if (ret == Focas1.EW_OK)
            {
                skinTextBox6.Text = "释放成功";
            }
            else
            {
                skinTextBox6.Text = "释放失败";
            }
        }

        //刷新并储存数据
        private void skinButton5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DataDao dc = new DataDao(IP.mf_ip, FOCAS_CLASS.Handle.h1, 1, model);

            #region 数据显示
            #region CNC模式状态
            //CNC模式
            textBox32.Text = dc.model_out.Cnc_autstat.ToString();

            //TM模式
            textBox33.Text = dc.model_out.Cnc_tmmode.ToString();

            //运行状态
            textBox34.Text = dc.model_out.Cnc_runstat.ToString();

            //主轴运行状态
            textBox35.Text = dc.model_out.Cnc_spmotion.ToString();

            //编辑状态
            textBox36.Text = dc.model_out.Cnc_edit.ToString();
            #endregion

            #region 坐标信息
            textBox4.Text = dc.model_out.Abs_data1.ToString();
            textBox5.Text = dc.model_out.Abs_data2.ToString();
            textBox6.Text = dc.model_out.Abs_data3.ToString();

            //机器坐标
            textBox9.Text = dc.model_out.Mach_data1.ToString();
            textBox8.Text = dc.model_out.Mach_data2.ToString();
            textBox7.Text = dc.model_out.Mach_data3.ToString();

            //相对坐标
            textBox15.Text = dc.model_out.Rel_data1.ToString();
            textBox14.Text = dc.model_out.Rel_data2.ToString();
            textBox13.Text = dc.model_out.Rel_data3.ToString();

            //剩余余量
            textBox12.Text = dc.model_out.Dist_data1.ToString();
            textBox11.Text = dc.model_out.Dist_data1.ToString();
            textBox10.Text = dc.model_out.Dist_data1.ToString();
            #endregion

            #region 主轴负载信息
            //主轴转速
            textBox3.Text = dc.model_out.Cnc_sp_sspeed.ToString();



            //加工时间
            textBox24.Text = dc.model_out.Time_run_2.ToString();
            textBox25.Text = dc.model_out.Time_cut_2.ToString();
            #endregion

            #region  程序相关
            //注册程序数目、可用程序数目、已使用内存、未使用内存
            textBox38.Text = dc.model_out.Cncprog_reg_num.ToString();
            textBox39.Text = dc.model_out.Cncprog_available_num.ToString();
            textBox41.Text = dc.model_out.Used_memory.ToString();
            textBox40.Text = dc.model_out.Unused_memory.ToString();

            //读取执行程序编号:子程序编号、主程序编号、执行程序序列号
            textBox42.Text = dc.model_out.Ncprog_exe_sub.ToString();
            textBox43.Text = dc.model_out.Ncprog_exe_main.ToString();
            textBox44.Text = dc.model_out.Ncprog_exe_seqnum.ToString();
            #endregion
            #endregion

            # region 数据存储
            dc.DBopreate(model);
            #endregion

        }
    }
}
