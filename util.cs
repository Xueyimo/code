using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace arcball
{
    class util
    {
        public string infopath = @".\info.txt";//通常是bin\Debug\下
        public string wherepath = @".\where.txt";
        public string statepath = @".\state.txt";
        public util()
        {

        }
        public PO GetUnitVector(PO p0, PO p1)
        {
            //给定指定两点p0,p1的单位向量
            PO vec = cal_vec(p0, p1);
            float x1=vec.length();
            vec.x = vec.x / x1;
            vec.y = vec.y / x1;
            vec.z = vec.z / x1;
            return vec;
        }
        public void cal_vec(PO p0, PO p1, PO vec)
        {
            vec.x = p1.x - p0.x;
            vec.y = p1.y - p0.y;
            vec.z = p1.z - p0.z;
        }
        public PO cal_vec(PO p0, PO p1)
        {
            PO vec=new PO();
            vec.x = p1.x - p0.x;
            vec.y = p1.y - p0.y;
            vec.z = p1.z - p0.z;
            return vec;
        }
        public void cal_vecdot(PO vec1, PO vec2, PO vec)
        {
            vec.x = vec1.y * vec2.z - vec1.z * vec2.y;
            vec.y=-(vec1.x*vec2.z-vec1.z*vec2.x);
            vec.z =vec1.x * vec2.y - vec1.y * vec2.x;
        }
        public PO cal_vecdot(PO vec1, PO vec2)
        {
            //vec=vec1*vec2;
            PO vec = new PO();
            vec.x = vec1.y * vec2.z - vec1.z * vec2.y;
            vec.y = -(vec1.x * vec2.z - vec1.z * vec2.x);
            vec.z = vec1.x * vec2.y - vec1.y * vec2.x;
            return vec;
        }
        public virtual float Dot(PO vec1, PO vec2)
        {
            return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
        }
        public void Po_vec1_f(PO p1, PO p0, PO vec1, PO vec2, PO vec3, float x1, float x2, float x3)
        {
            //P1=PO+f*vec1
            p1.x = p0.x + vec1.x * x1 + vec2.x * x2 + vec3.x * x3;
            p1.y = p0.y + vec1.y * x1 + vec2.y * x2 + vec3.y * x3;
            p1.z = p0.z + vec1.z * x1 + vec2.z * x2 + vec3.z * x3;
        }
        public PO Po_vec_f(PO p0,PO vec,float x)
        {
            PO p1 = new PO();
            p1.x = p0.x + vec.x * x;
            p1.y = p0.y + vec.y * x;
            p1.z = p0.z + vec.z * x;
            return p1;
        }
        public PO vec_f(PO vec, float x)
        {
            PO p1 = new PO();
            p1.x = vec.x * x;
            p1.y = vec.y * x;
            p1.z = vec.z * x;
            return p1;
        }
        public void copy_p(PO p1,PO p2)
        {
            p1.x = p2.x;
            p1.y = p2.y;
            p1.z = p2.z;
        }
        public float d3_Cal_Angle(PO p1, PO p2, PO center)
        {
            PO vec1, vec2;
            vec1 = new PO();
            vec2 = new PO();
            vec1.x = p1.x - center.x;
            vec1.y = p1.y - center.y;
            vec1.z = p1.z - center.z;
            vec2.x = p2.x - center.x;
            vec2.y = p2.y - center.y;
            vec2.z = p2.z - center.z;
            return (float)System.Math.Acos(Dot(vec1, vec2) / (vec1.length() * vec2.length()));
        }
        public float d3_Cal_Solid_Angle(PO p0, PO p1, PO p2, PO p3)
        {
            float s, a, b, y;
            float s1, s2, s3, s4, sita;
            a = d3_Cal_Angle(p1, p2, p0);
            b = d3_Cal_Angle(p1, p3, p0);
            y = d3_Cal_Angle(p2, p3, p0);
            s = (a + b + y) / 2;
            s1 = (float)System.Math.Tan(s / 2);
            s2 = (float)System.Math.Tan((s - a) / 2);
            s3 = (float)System.Math.Tan((s - b) / 2);
            s4 = (float)System.Math.Tan((s - y) / 2);
            sita = (float)System.Math.Atan(System.Math.Sqrt(s1 * s2 * s3 * s4)) * 4;
            return sita;
        }
        public float d2_Cal_Op_Angle(LINE l0,PO p0)
        {
            //计算夹角的度数
            PO pa, pb;
            float a, b, c;
            PO vecab, veca0, vecb0;
            pa = l0.p1;
            pb = l0.p2;
            vecab = cal_vec(pa,pb);
            veca0 = cal_vec(pa,p0);
            vecb0 = cal_vec(pb,p0);
            a=vecab.length();
            b=veca0.length();
            c=vecb0.length();
            return (float)System.Math.Acos((b*b+c*c-a*a)/(2*b*c));
        }
        public float Direct(SURFACE s0,PO p0)
        {
            util u1 = new util();
            PO p1, p2, p3,vec1,vec2,vec,vecx;
            p1 = s0.p1;
            p2 = s0.p2;
            p3 = s0.p3;
            vec1=new PO();
            vec2=new PO();
            vec = new PO();
            vecx = new PO();
            cal_vec(p1,p2,vec1);
            cal_vec(p1,p3,vec2);
            //u1.InFile("e:/info.txt","---start Direct---");
            //vec1.ToPrint();
            //vec2.ToPrint();
            cal_vecdot(vec1,vec2,vec);//计算向量积
            //vec.ToPrint();
            cal_vec(p1,p0,vecx);
            //vecx.ToPrint();
            //u1.InFile("e:/info.txt", "---end Direct---");
            if (System.Math.Abs(Dot(vec, vecx)) > 0.0001)
                return Dot(vec, vecx);
            else
                return 0;
        }
        public float Direct_2d(LINE l0, PO p0)
        {
            util u1 = new util();
            PO pa, pb;
            pa = l0.p1;
            pb = l0.p2;
            float a, b, c, flag;
            if (pa.x == pb.x)
            {
                a = 1;
                b = 0;
                c = -pa.x;
            }
            else
            {
                a = (float)((pa.y - pb.y) * 1.0 / (pa.x - pb.x));
                b = -1;
                c =(float)(pa.y - a * pa.x);
            }
            flag = a * p0.x + b * p0.y + c;
            return flag;
        }
        public bool noSearched(SURFACE s, PO p0)
        {
            /*
             * 判断某个面的某个方向是否被搜索过,
             * 搜索过返回true，未搜索过返回false
             */
            util u1 = new util();
            float f;
            f = u1.Direct(s, p0);
            if (f > 0 && s.positive)
                return true;
            else if (f < 0 && s.negtive)
                return true;
            return false;
        }
        public bool noSearched(LINE l0, PO p0)
        {
            /*
             * 判断某条线的某个方向是否被搜索过,
             * 搜索过返回true，未搜索过返回false
             */
            util u1 = new util();
            float f;
            f = u1.Direct_2d(l0, p0);
            if (f > 0 && l0.positive)
                return true;
            else if (f < 0 && l0.negtive)
                return true;
            return false;
        }
        public PO d3_Cal_Vec_Po_Line(PO p1,PO p2,PO p3)
        {
            /*
             * p2,p3某条线的端点，p1直线外的点
             * 返回垂足
             */
            float u;
            PO p0,vec;
            p0 = new PO();
            vec = new PO();
            u = ((p2.x - p3.x) * (p2.x - p1.x) + (p2.y - p3.y) * (p2.y - p1.y) + (p2.z - p3.z) * (p2.z - p1.z)) / ((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y) + (p2.z - p1.z) * (p2.z - p1.z));
            p0.x = u * p1.x + (1 - u) * p2.x;
            p0.y = u * p1.y + (1 - u) * p2.y;
            p0.z = u * p1.z + (1 - u) * p2.z;
            return p0;
        }
        public float d3_Cal_Bi_Angle(PO pi,PO pj,PO p1,PO p2)
        {
            /*
             * pi,pj是二面角的交线，p1,p2是其余的两个端点
             * 计算二面角的符号
             */
            PO px, py,vec1,vec2;
            px = new PO();
            py = new PO();
            px = d3_Cal_Vec_Po_Line(pi,pj,p1);
            py = d3_Cal_Vec_Po_Line(pi,pj,p2);
            vec1 = new PO();
            vec2 = new PO();
            cal_vec(p1,px,vec1);
            cal_vec(p2,py,vec2);
            return Dot(vec1,vec2);
        }
        public void InFile(String path,String str)
        {
            StreamWriter sw = new StreamWriter(path,true);
            sw.WriteLine(str);
            sw.Close();
        }
        public void InFile(String path, PO p)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine("({0},{1},{2})",p.x,p.y,p.z);
            sw.Close();
        }
        public void InFile(String path, int i)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine("{0}", i);
            sw.Close();
        }
        public void InFile(String path, float i)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine("{0}", i);
            sw.Close();
        }
        public PO AddVec(PO p0,PO p1)
        {
            //计算两个向量的和
            PO pr = new PO();
            pr.x = p0.x + p1.x;
            pr.y = p0.y + p1.y;
            pr.z = p0.z + p1.z;
            return pr;
        }
        private float[,] Inverse(float[,] a)
        {
            //求矩阵a的逆矩阵
            util u1=new util();
            float[,] b = new float[3,3];
            float deter;
            deter = a[0, 0] * a[1, 1] * a[2, 2] + a[0, 1] * a[1, 2] * a[2, 0] + a[1, 0] * a[2, 1] * a[0, 2] -
                a[0, 2] * a[1, 1] * a[2, 0] - a[0, 0] * a[2, 1] * a[1, 2] - a[0, 1] * a[1, 0] * a[2, 2];
            if (deter != 0)
            {
                b[0, 0] = (a[1, 1] * a[2, 2] - a[1, 2] * a[2, 1]) / deter;
                b[0, 1] = -(a[1, 0] * a[2, 2] - a[1, 2] * a[2, 0]) / deter;
                b[0, 2] = (a[1, 0] * a[2, 1] - a[1, 1] * a[2, 0]) / deter;
                b[1, 0] = -(a[0, 1] * a[2, 2] - a[0, 2] * a[2, 1]) / deter;
                b[1, 1] = (a[0, 0] * a[2, 2] - a[0, 2] * a[2, 0]) / deter;
                b[1, 2] = -(a[0, 0] * a[2, 1] - a[0, 1] * a[2, 0] )/ deter;
                b[2, 0] = (a[0, 1] * a[1, 2] - a[1, 1] * a[0, 2]) / deter;
                b[2, 1] = -(a[0, 0] * a[1, 2] - a[1, 0] * a[0, 2]) / deter;
                b[2, 2] = (a[0, 0] * a[1, 1] - a[1, 0] * a[0, 1]) / deter;
                return b;
            }
            else{
                u1.InFile(u1.infopath ,"det equals zero!!!");
                return null;
            }

        }
        private float[,] Array_Multi(float[,] a,float[,] b)
        {
            //计算矩阵乘法a*b;
            util u1 = new util();
            int ro1, ro2, co1, co2;
            ro1 = 3;
            co1 = 3;
            ro2 = 3;
            co2 =b.Length;
            u1.InFile(u1.infopath,co2);
            float[,] re=new float[co1,ro2];
            float temp;
            for(int i=0;i<co1;i++)
                for (int j = 0; j < co2; j++)
                {
                    temp = 0;
                    for (int k = 0; k < co1; k++)
                        temp = temp + a[i, k] * b[k, j];
                    re[i, j] = temp;
                }
            return re;
        }
        private float[,] Vec_to_Array(PO[] po)
        {
            //将点数组转换到矩阵中去
            util u1 = new util();
            int dimes;
            float[,] a;
            dimes = po.GetLength(0);
            //u1.InFile(u1.infopath,dimes);
            a=new float[3,dimes];
            for (int i = 0; i < dimes; i++)
            {
                a[0, i] = po[i].x;
                a[1, i] = po[i].y;
                a[2, i] = po[i].z;
            }
            return a;
        }
        private float CalAgl(PO p)
        {
            //计算以原点为起点的某个向量的终点的与x轴正方向沿逆时针的夹角
            float m1;
            PO pi = new PO();
            m1 = (float)System.Math.Pow(System.Math.Pow(p.x, 2) + System.Math.Pow(p.y, 2), 0.5);
            pi.x = p.x / m1;
            pi.y = p.y / m1;
            if (pi.y >= 0)
                return (float)System.Math.Acos(pi.x);
            else
                return (float)(3.1415926 * 2 - System.Math.Acos(pi.x));
        }
        public float Cal_surface_angle(PO p0,PO p1,PO p2,PO p4)
        {
            /*
             * p0p1构成旋转轴，p2辅助p0p1轴构成一个
             * 按右手规则的直角坐标系si             
             * 计算p4点对应的夹角
             */
            util u1 = new util();
            PO p3, p5;//p2,p4对应的两个垂足
            PO pi1, pj1, pz1;//si坐标系
            PO pi, pj, pz;//s坐标系
            pi = new PO(0,0,1);
            pj = new PO(0,1,0);
            pz = new PO(0,0,1);
            p3 = d3_Cal_Vec_Po_Line(p0,p1,p2);
            p5 = d3_Cal_Vec_Po_Line(p0,p1,p4);
            u1.InFile(u1.infopath, "ga1");
            pj1 = Po_vec_f(p1, GetUnitVector(p3, p2), 1);
            p4 = Po_vec_f(p1,GetUnitVector(p5, p4), 1);//计算p2,p4相对于旋转轴的位置
            pz1 = GetUnitVector(p1,p0);
            u1.InFile(u1.infopath, "ga2");
            pi1 = cal_vecdot(pz1,pj1);
            u1.InFile(u1.infopath, "ga3");
            PO[] px1=new PO[3];
            PO[] px2=new PO[3];
            PO[] px3= new PO[1];
            px1[0] = pi1;
            px1[1] = pj1;
            px1[2] = pz1;
            px2[0] = pi;
            px2[1] = pj;
            px2[2] = pz;
            px3[0] = p4;
            float[,] v,u;
            float[,] x1,x2;
            v=Vec_to_Array(px1);
            u=Vec_to_Array (px2);
            x2=Vec_to_Array(px3);
            u1.InFile(u1.infopath ,"ga5");
            x1=Array_Multi(Array_Multi (Inverse(v),u),x2);
            return CalAgl(new PO(x1[0,0],x1[0,1]));
        }
        public bool IsXl(PO s1, PO d1, PO s2, PO d2)
        {
            /*
             * 判断边s1-d1与s2-d2是否相交，如果相交返回true,否则返回false
             * 其中相交不能包括任意线段的端点
             */
            float D, dl1, dl2, c1, c2;
            D = (s1.x - d1.x) * (d2.y - s2.y) - (d2.x - s2.x) * (s1.y - d1.y);
            dl1 = (d2.x - d1.x) * (d2.y - s2.y) - (d2.x - s2.x) * (d2.y - d1.y);
            dl2 = (s1.x - d1.x) * (d2.y - d1.y) - (d2.x - d1.x) * (s1.y - d1.y);
            if (D == 0) return false;
            c1 = dl1 / D;
            c2 = dl2 / D;
            if (0 < c1 && c1 < 1 && 0 < c2 && c2 < 1)
                return true;
            else
                return false;

        }
        public PO[] Get_po_from_string(string str)
        {
            string sn="";
            float[] fn=new float[10];
            int j=0;
            foreach(char ch in str){
                if((int)ch>=48&&(int)ch<=57|| ch.Equals('.')||ch.Equals('-')||ch.Equals('E'))
                    sn=sn+ch;
                else
                {
                    if (!sn.Equals(""))
                        fn[j++] = float.Parse(sn);
                    sn = "";
                }
            }
            //1,2,3,4,5,6
            PO[] pc=new PO[3];
            pc[0]=new PO();
            pc[0].x = fn[1];
            pc[0].y = fn[2];
            pc[0].z = fn[3];
            pc[1] = new PO();
            pc[1].x = fn[4];
            pc[1].y = fn[5];
            pc[1].z = fn[6];
            pc[2] = new PO();
            pc[2].x = fn[7];
            pc[2].y = fn[8];
            pc[2].z = fn[9];
            return pc;
        }
        public PO SearchOutHeart(PO pa,PO pb,PO p0){
            //返回三角形pa,pb,pc的外心
		    float x1, x2, x3, y1, y2, y3;
		    PO pr;
		    pr = new PO();
		    x1 = pa.x;
		    x2 = pb.x;
		    x3 = p0.x;
		    y1 = pa.y;
		    y2 = pb.y;
		    y3 = p0.y;
		    pr.x = (float)((System.Math.Pow(x2, 2)*y1 - System.Math.Pow(x3, 2)*y1 - System.Math.Pow(x1, 2)*y2 +System.Math.Pow(x3, 2)*y2 - System.Math.Pow(y1, 2)*y2 + y1*System.Math.Pow(y2, 2) + System.Math.Pow(x1, 2)*y3- System.Math.Pow(x2, 2)*y3 + System.Math.Pow(y1, 2)*y3 - System.Math.Pow(y2, 2)*y3 - y1*System.Math.Pow(y3, 2) + y2*System.Math.Pow(y3, 2))/ (2 * (x2*y1 - x3*y1 - x1*y2 + x3*y2 + x1*y3 - x2*y3)));
		    pr.y =(float)( -(-System.Math.Pow(x1, 2)*x2 + x1*System.Math.Pow(x2, 2) + System.Math.Pow(x1, 2)*x3 - System.Math.Pow(x2, 2)*x3 - x1*System.Math.Pow(x3, 2) + x2*System.Math.Pow(x3, 2) - x2*System.Math.Pow(y1, 2) + x3*System.Math.Pow(y1, 2) + x1*System.Math.Pow(y2, 2) - x3*System.Math.Pow(y2, 2) - x1*System.Math.Pow(y3, 2) + x2*System.Math.Pow(y3, 2)) / (2 * (x2*y1 - x3*y1 - x1*y2 + x3*y2 + x1* y3 - x2*y3)));
		    return pr;
	    }
    }
}
