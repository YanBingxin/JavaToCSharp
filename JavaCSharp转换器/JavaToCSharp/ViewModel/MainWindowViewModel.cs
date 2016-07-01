using JavaToCSharp.IView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using JavaToCSharp.IViewModel;
using System.Windows.Input;
using JavaToCSharp.Common;
using JavaToCSharp.Model;
using System.Windows;
using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization;

namespace JavaToCSharp.ViewModel
{
    public class MainWindowViewModel : NotifyObject, IMainWindowViewModel
    {
        #region 命令
        /// <summary>
        /// 解密命令
        /// </summary>
        public ICommand DescryptCommand { get; set; }
        /// <summary>
        /// 最小化命令
        /// </summary>
        public ICommand MinCommand { get; set; }
        /// <summary>
        /// 最大化命令
        /// </summary>
        public ICommand MaxCommand { get; set; }
        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand { get; set; }
        /// <summary>
        /// 拖动命令
        /// </summary>
        public ICommand DragMoveCommand { get; set; }
        /// <summary>
        /// 获取模板命令
        /// </summary>
        public ICommand GetFormatCommand { get; set; }
        #endregion

        #region 属性

        private string _mformat;
        /// <summary>
        /// 按钮内容
        /// </summary>
        public string MFormat
        {
            get
            {
                return _mformat;
            }
            set
            {
                _mformat = value;
                this.RaisePropertyChanged(() => MFormat);
                DescryptExecute(string.Empty);
            }
        }

        private string _time;
        /// <summary>
        /// 花费时间
        /// </summary>
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                this.RaisePropertyChanged(() => Time);
            }
        }

        private string _btnContent;
        /// <summary>
        /// 按钮内容
        /// </summary>
        public string btnContent
        {
            get
            {
                return _btnContent;
            }
            set
            {
                _btnContent = value;
                this.RaisePropertyChanged(() => btnContent);
            }
        }

        private string _content = string.Empty;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    this.RaisePropertyChanged(() => Content);
                    DescryptExecute(string.Empty);
                }

            }
        }
        /// <summary>
        /// 转换结果
        /// </summary>
        private string _resContent = string.Empty;
        /// <summary>
        /// 获取或设置转换结果
        /// </summary>
        public string ResContent
        {
            get
            {
                return _resContent;
            }
            set
            {
                _resContent = value;
                this.RaisePropertyChanged(() => ResContent);
            }
        }

        private List<ProInfo> _proInfos;
        /// <summary>
        /// 属性实体
        /// </summary>
        public List<ProInfo> ProInfos
        {
            get
            {
                return _proInfos;
            }
            set
            {
                _proInfos = value;
                this.RaisePropertyChanged(() => ProInfos);
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 页面
        /// </summary>
        private IMainWindowView m_View;
        /// <summary>
        /// 运算线程
        /// </summary>
        private Thread _desThread;
        /// <summary>
        /// 计时线程
        /// </summary>
        private Thread _countThread;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="view"></param>
        public MainWindowViewModel(IMainWindowView view)
        {
            m_View = view;
            m_View.DataContext = this;
            InitData();
            InitCommand();
        }

        /// <summary>
        /// 初始命令
        /// </summary>
        private void InitCommand()
        {
            DescryptCommand = new RelayCommand(CopyExecute);
            MinCommand = new RelayCommand(MinWindowExecute);
            MaxCommand = new RelayCommand(MaxWindowExecute);
            CloseCommand = new RelayCommand(CloseExecute);
            DragMoveCommand = new RelayCommand(DragMoveExecute);
            GetFormatCommand = new RelayCommand(GetFormatExecute);
        }

        public void GetFormatExecute(object obj)
        {
            MFormat = @"
        /// <summary>
        /// {0}
        /// </summary>
        private {1} _{2} {4};
        /// <summary>
        /// 获取或设置{0}
        /// </summary>
        public {1} {3} 
        {{ 
            get {{ return _{2}; }}
            set {{ _{2} = value;}}
        }}
";
        }

        /// <summary>
        /// 赋值到剪贴板
        /// </summary>
        /// <param name="obj"></param>
        public void CopyExecute(object obj)
        {
            Clipboard.SetDataObject(ResContent);
            Time = "已复制到剪贴板";
        }

        /// <summary>
        /// 初始数据
        /// </summary>
        private void InitData()
        {
            btnContent = string.Format("复制");
        }
        #endregion

        #region 方法处理
        /// <summary>
        /// 解密处理
        /// </summary>
        /// <param name="prarmeter"></param>
        public void DescryptExecute(object prarmeter)
        {
            WorkDescrypt();
            CountTime();
        }

        private void CountTime()
        {
            if (_countThread != null && _desThread.IsAlive)
                _countThread.Abort();
            while (_countThread == null || _countThread.ThreadState == ThreadState.Stopped)
            {
                _countThread = new Thread(new ThreadStart(Count));
                _countThread.IsBackground = true;
                _countThread.Start();
            }
        }
        /// <summary>
        /// 计时
        /// </summary>
        private void Count()
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                TimeSpan span = DateTime.Now - start;
                Time = string.Format("本次耗时：{0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        private void WorkDescrypt()
        {
            if (_desThread != null && _desThread.IsAlive)
                _desThread.Abort();
            _desThread = new Thread(new ThreadStart(Descrypt));
            _desThread.IsBackground = true;
            _desThread.Start();
        }
        /// <summary>
        /// 解密线程方法
        /// </summary>
        private void Descrypt()
        {
            ResContent = JavaToCSharpHelper.Descrypt(Content, MFormat);
            ResContent = string.IsNullOrEmpty(ResContent) && !string.IsNullOrEmpty(Content) ? "提示：输入的代码格式有误。" : ResContent;
            //关闭计时进程
            if (_countThread != null && _desThread.IsAlive)
                _countThread.Abort();
        }

        #region 界面UI接口
        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="parameter"></param>
        private void MinWindowExecute(object parameter)
        {
            if ((m_View as Window) != null)
                (m_View as Window).WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="parameter"></param>
        private void MaxWindowExecute(object parameter)
        {
            Window win = (m_View as Window);
            if (win != null)
            {
                win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }
        /// <summary>
        ///关闭窗口 
        /// </summary>
        /// <param name="parameter"></param>
        private void CloseExecute(object parameter)
        {
            m_View.Close();
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="obj"></param>
        private void DragMoveExecute(object obj)
        {
            Window win = (m_View as Window);
            if (win != null)
            {
                win.DragMove();
            }
        }
        #endregion
        #endregion
    }
}
