using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using DCS_DAL;
using System.Configuration;
using System.Data.SqlClient;
using FOCAS_CLASS;

namespace Fanuc_test_04_24
{
    class DataDao
    {
        public Model model_out { get; set; }
        private FOCAS_class focas = new FOCAS_class();

        private int machine_id;
        private string machine_ip;
        public DataDao(string ip, ushort handle,int id, Model model_in)
        {
            this.machine_id = id;
            this.machine_ip = ip;

            #region 执行数据采集
            // 获取CNC状态
            focas.Get_autstat(handle, model_in);
            // 获取坐标信息
            focas.Get_position_abs(handle, model_in);
            // 运行时间
            focas.Get_time_run2(handle, model_in);
            focas.Get_time_cut2(handle, model_in);
            // 获取零件信息
            focas.Get_component_num(handle, model_in);
            // 获取主轴信息
            focas.Get_spindle_sspeed(handle, model_in);
            focas.Get_spindle_fspeed(handle, model_in);
            // 获取程序信息
            focas.Get_ncprog_reg(handle, model_in);
            focas.Get_ncprog_exe(handle, model_in);
            focas.Get_nncpro_seq(handle, model_in);
            #endregion

            this.model_out = model_in;
        }
       

        // 数据库写数据操作
        public int DBopreate(Model model)
        {
            #region test
            //string constr = ConfigurationManager.ConnectionStrings["strCon"].ToString();
            //SqlHelper sql = new SqlHelper(constr);
            //i++;
            //SqlParameter[] para = { new SqlParameter("@student_no", i) };
            //sql.ExecuteSql(@"INSERT INTO student(student_no,name,gender,birthday,province_code,address)
            //  VALUES(@student_no, '测试数据', 'M', '1998-09-5', '330000', '南京孝陵卫大道');", para);
            #endregion

            #region 将model的数据写入数据库
            string constr = ConfigurationManager.ConnectionStrings["strCon"].ToString();
            string operastr = @"INSERT INTO MachineData(machine_id,ip,cnc_autstat,cnc_tmmode,cnc_runstat,cnc_spmotion,cnc_alarm_rough,cnc_edit,abs_data1,abs_data2,abs_data3,
                                mach_data1,mach_data2,mach_data3,rel_data1,rel_data2,rel_data3,dist_data1,dist_data2,dist_data3,time_run_2,time_cut_2,cnc_component_num,cnc_sp_sspeed,
                                cnc_sp_fspeed,cncprog_reg_num,cncprog_available_num,used_memory,unused_memory,ncprog_exe_sub,ncprog_exe_main,ncprog_exe_seqnum)
                 VALUES(@machine_id,@machine_ip,@cnc_autstat,@cnc_tmmode,@cnc_runstat,@cnc_spmotion,@cnc_alarm_rough,@cnc_edit,@abs_data1,@abs_data2,@abs_data3,
                        @mach_data1,@mach_data2,@mach_data3,@rel_data1,@rel_data2,@rel_data3,@dist_data1,@dist_data2,@dist_data3,@time_run_2,@time_cut_2,@cnc_component_num,
                        @cnc_sp_sspeed,@cnc_sp_fspeed,@cncprog_reg_num,@cncprog_available_num,@used_memory,@unused_memory,@ncprog_exe_sub,@ncprog_exe_main,@ncprog_exe_seqnum);";
            SqlHelper sql = new SqlHelper(constr);
            SqlParameter[] para = { new SqlParameter("@machine_id", machine_id),
                                    new SqlParameter("@machine_ip", machine_ip),
                                    new SqlParameter("@cnc_autstat", model.Cnc_autstat),
                                    new SqlParameter("@cnc_tmmode", model.Cnc_tmmode),
                                    new SqlParameter("@cnc_runstat", model.Cnc_runstat),
                                    new SqlParameter("@cnc_spmotion", model.Cnc_spmotion),
                                    new SqlParameter("@cnc_alarm_rough", model.Cnc_alarm_rough),
                                    new SqlParameter("@cnc_edit", model.Cnc_edit),
                                    new SqlParameter("@abs_data1", model.Abs_data1),
                                    new SqlParameter("@abs_data2", model.Abs_data2),
                                    new SqlParameter("@abs_data3", model.Abs_data3),
                                    new SqlParameter("@mach_data1", model.Mach_data1),
                                    new SqlParameter("@mach_data2", model.Mach_data2),
                                    new SqlParameter("@mach_data3", model.Mach_data3),
                                    new SqlParameter("@rel_data1", model.Rel_data1),
                                    new SqlParameter("@rel_data2", model.Rel_data2),
                                    new SqlParameter("@rel_data3", model.Rel_data3),
                                    new SqlParameter("@dist_data1", model.Dist_data1),
                                    new SqlParameter("@dist_data2", model.Dist_data2),
                                    new SqlParameter("@dist_data3", model.Dist_data3),
                                    new SqlParameter("@time_run_2", model.Time_run_2),
                                    new SqlParameter("@time_cut_2", model.Time_cut_2),
                                    new SqlParameter("@cnc_component_num", model.Cnc_component_num),
                                    new SqlParameter("@cnc_sp_sspeed", model.Cnc_sp_sspeed),
                                    new SqlParameter("@cnc_sp_fspeed", model.Cnc_sp_fspeed),
                                    new SqlParameter("@cncprog_reg_num", model.Cncprog_reg_num),
                                    new SqlParameter("@cncprog_available_num", model.Cncprog_available_num),
                                    new SqlParameter("@used_memory", model.Used_memory),
                                    new SqlParameter("@unused_memory", model.Unused_memory),
                                    new SqlParameter("@ncprog_exe_sub", model.Ncprog_exe_sub),
                                    new SqlParameter("@ncprog_exe_main", model.Ncprog_exe_main),
                                    new SqlParameter("@ncprog_exe_seqnum", model.Ncprog_exe_seqnum)
             };
            if(sql.ExecuteSql(operastr, para) > 0)
                return machine_id;
            else
                throw new Exception("插入失败");
            #endregion
        }

    }
}
