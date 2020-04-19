# WPF作业调度程序
使用WPF编写的作业调度

![MainWindow](https://github.com/lixiaobei/Job_Schedule/tree/master/example_photo/MainWindow.png)

主要实现功能为以下

![First_serve](https://github.com/lixiaobei/Job_Schedule/tree/master/example_photo/first_serve.png)

先来先服务算法——总是把当前处于就绪队列之首的那个进程调度到运行状态

![priority](https://github.com/lixiaobei/Job_Schedule/tree/master/example_photo/priority.png)

优先级调度算法——每个进程都有一个优先级与其关联,而具有最高优先级的进程会分配到 CPU。

![short](https://github.com/lixiaobei/Job_Schedule/tree/master/example_photo/short_job.png)

短作业优先算法——以作业的长短来计算优先级，作业越短，其优先级越高。

![dynamic](https://github.com/lixiaobei/Job_Schedule/tree/master/example_photo/dynamic.png)

高响应比算法——把CPU分配给就绪队列中响应比最高的进程。该算法中的响应比是指作业等待时间与运行比值，响应比 =（等待时间+要求服务时间）/ 要求服务时间
