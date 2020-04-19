using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace 作业调度C_sharp
{
    public enum Status { Wait, Run, Finish };
    [Serializable]
    public class JCB : BaseClone<JCB>
    {
        //计时器
        public static double Time { get; set; }
        //内存最大作业数
        public static int Number = 1;
        //作业名
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        //作业状态
        private Status _status;
        public Status Statu
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Statu");
            }
        }
        //作业优先级
        public int Super { get; set; }
        //需要时间
        public double Ntime { get; set; }
        //运行时间
        public double Rtime { get; set; }
        //到达时间
        public double Atime { get; set; }
        //开始时间
        public double Stime { get; set; }
        //完成时间
        public double Ftime { get; set; }
        //周转时间
        public double ZzTime { get; set; }
        //带权周转时间
        public double DqzzTime { get; set; }
        //平均周转时间
        public double AzzTime { get; set; }
        //带权平均周转时间
        public double AdqzzTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        //先来先服务算法
        public static void FCFS(List<JCB> JCBs)
        {
            //按到达时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Atime.CompareTo(y.Atime);
            });
            //就绪队列
            List<JCB> Ready = new List<JCB>();
            //运行队列
            List<JCB> Run = new List<JCB>();

            //进程个数
            int n = JCBs.Count();
            //进程处理个数
            int flag = 0;
            //计算开始、完成时间
            for (int i = 0; flag != n;)
            {
                //加入就绪队列
                EnReadyQueue(Ready, JCBs, n, ref i);

                //加入运行队列
                if (Ready.Count() != 0 || Run.Count() != JCB.Number)
                {
                    if (Ready.Count() == 0 && Run.Count() == 0) { Time++; continue; }
                    Enqueue(Ready, Run);
                }
                if (Run.Count() != 0) Count_number(Run, ref flag);              
            }
            //按开始时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Stime.CompareTo(y.Stime);
            });
            Time = 0;
        }

        //优先级调度算法
        public static void Static_priority(List<JCB> JCBs)
        {
            //按到达时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Atime.CompareTo(y.Atime);
            });
            //就绪队列
            List<JCB> Ready = new List<JCB>();
            //运行队列
            List<JCB> Run = new List<JCB>();

            //作业个数
            int n = JCBs.Count();
            //作业处理个数
            int flag = 0;
            //计算开始、完成时间
            for (int i = 0; flag != n;)
            {
                //加入就绪队列
                EnReadyQueue(Ready, JCBs, n, ref i);

                //加入运行队列
                if (Ready.Count() != 0 || Run.Count() == 0)
                {

                    //按照优先级进行排序
                    Ready.Sort(delegate (JCB X, JCB Y)
                    {
                        return X.Super.CompareTo(Y.Super);
                    });
                    if (Ready.Count() == 0 && Run.Count() == 0) { Time++; continue; }
                    Enqueue(Ready, Run);
                }
                if (Run.Count() != 0) Count_number(Run, ref flag);
            }
            //按开始时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Stime.CompareTo(y.Stime);
            });
            //重置
            Time = 0;
        }

        //短作业优先算法
        public static void Short_priority(List<JCB> JCBs)
        {
            //按到达时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Atime.CompareTo(y.Atime);
            });

            //就绪队列
            List<JCB> Ready = new List<JCB>();
            //运行队列
            List<JCB> Run = new List<JCB>();
            //作业个数
            int n = JCBs.Count();
            //作业处理个数
            int flag = 0;
            //计算开始、完成时间
            for (int i = 0; flag != n;)
            {
                //加入就绪队列
                EnReadyQueue(Ready, JCBs, n, ref i);
                //加入运行队列
                if (Ready.Count() != 0 || Run.Count() == 0)
                {

                    //按照运行时间进行排序
                    Ready.Sort(delegate (JCB X, JCB Y)
                    {
                        return X.Ntime.CompareTo(Y.Ntime);
                    });
                    if (Ready.Count() == 0 && Run.Count() == 0) { Time++; continue; }
                    Enqueue(Ready, Run);
                }
                if (Run.Count() != 0) Count_number(Run, ref flag);
            }
            //按开始时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Stime.CompareTo(y.Stime);
            });
            //重置
            Time = 0;
        }

        //高响应比算法
        public static void Hrrn(List<JCB> JCBs)
        {
            //按到达时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Atime.CompareTo(y.Atime);
            });

            //就绪队列
            List<JCB> Ready = new List<JCB>();
            List<JCB> Ready2 = new List<JCB>();
            //运行队列
            List<JCB> Run = new List<JCB>();
            //作业个数
            int n = JCBs.Count();
            //作业处理个数
            int flag = 0;
            //计算开始、完成时间
            for (int i = 0; flag != n;)
            {
                //加入就绪队列
                EnReadyQueue(Ready, JCBs, n, ref i);
                //加入运行队列
                if (Ready.Count() != 0 || Run.Count() == 0)
                {
                    Count_hrrn(Ready);
                    //按照响应时间进行排序
                    Ready.Sort(delegate (JCB X, JCB Y)
                    {
                        return -X.Super.CompareTo(Y.Super);
                    });
                    if (Ready.Count() == 0 && Run.Count() == 0) { Time++; continue; }
                    Enqueue(Ready, Run);
                }
                if (Run.Count() != 0) Count_number(Run, ref flag);

            }
            //按开始时间排序
            JCBs.Sort(delegate (JCB x, JCB y)
            {
                return x.Stime.CompareTo(y.Stime);
            });
            //重置
            Time = 0;
        }
        //计算优先权函数
        private static void Count_hrrn(List<JCB> ready)
        {
            int n = ready.Count();
            for(int i = 0; i < n; i++)
            {
                ready[i].Super = (int)(1 + (Time - ready[i].Atime) / ready[i].Ntime);
            }
        }

        //分析函数
        public static void Analyse(List<JCB> JCBs)
        {
            int n = JCBs.Count();
            //计算周转、带权周转时间
            for (int i = 0; i < n; i++)
            {
                //周转时间 = 完成时间 - 到达时间
                JCBs[i].ZzTime = JCBs[i].Ftime - JCBs[i].Atime;
                //带权周转时间 = 周转时间 / 运行时间
                JCBs[i].DqzzTime = Math.Round(JCBs[i].ZzTime / JCBs[i].Ntime, 2);
            }

            //计算平均周转时间,平均带权周转时间
            double SzzTime = 0;
            double SdqzzTime = 0;
            for (int i = 0; i < n; i++)
            {
                SzzTime = SzzTime + JCBs[i].ZzTime;
                SdqzzTime = SdqzzTime + JCBs[i].DqzzTime;
                //平均周转时间 = 周转时间和 / 个数
                JCBs[i].AzzTime = Math.Round(SzzTime / n, 2);
                //平均带权周转时间 = 带权周转时间和 / 个数
                JCBs[i].AdqzzTime = Math.Round(SdqzzTime / n, 2);
            }

        }

        public override JCB Clone()
        {
            return base.Clone();
        }
        //计数操作
        public static void Count_number(List<JCB> Run, ref int flag)
        {
            //将1s拆分
            for (int j = 0; j < Run.Count(); j++)
            {
                //计算运行时间
                Run[j].Rtime += 1.0 / Run.Count();
            }

            Time++;
            //判断是否出队列
            for (int j = 0; j < Run.Count(); j++)
            {
                if (Run[j].Rtime >= Run[j].Ntime)
                {
                    Run[j].Ftime = Time;
                    Run[j].Statu = Status.Finish;
                    //时钟校正
                    //Time--;
                    Run.RemoveAt(j);
                    j--;
                    flag++;
                }
            }
        }

        //进运行队列操作
        public static void Enqueue(List<JCB> Ready,List<JCB> Run)
        {
            //非抢占
            while (Run.Count() != JCB.Number && Ready.Count() != 0)
            {

                //进队
                Run.Add(Ready[0]);
                //出队
                Ready.RemoveAt(0);

            }
            //设置状态
            for (int j = 0; j < Run.Count(); j++)
            {
                Run[j].Statu = Status.Run;
                //开始时间
                if (Run[j].Rtime == 0)
                    Run[j].Stime = Time;
            }
        }

        //进就绪队列操作
        public static void EnReadyQueue(List<JCB> Ready, List<JCB> JCBs,int n,ref int i)
        {
            //加入就绪队列
            while (i < n && JCBs[i].Atime == Time)
            {
                Ready.Add(JCBs[i]);
                i++;
            }
        }
    }

    
    /// <summary>
    /// 通用的深复制方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BaseClone<T>
    {
        public virtual T Clone()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            memoryStream.Position = 0;
            return (T)formatter.Deserialize(memoryStream);

        }
    }
}


