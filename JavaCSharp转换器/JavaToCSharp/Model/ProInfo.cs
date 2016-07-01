using JavaToCSharp.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace JavaToCSharp.Model
{
    public class ProInfo : NotifyObject
    {
        private string _opening;
        /// <summary>
        /// 修饰符
        /// </summary>
        public string Opening
        {
            get
            {
                return _opening;
            }
            set
            {
                _opening = value;
                this.RaisePropertyChanged("Opening");
            }
        }

        private string _typer;
        /// <summary>
        /// 类型
        /// </summary>
        public string Typer
        {
            get
            {
                return _typer;
            }
            set
            {
                _typer = value;
                this.RaisePropertyChanged("Typer");
            }
        }

        private string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        private string _notes;
        /// <summary>
        /// 注释
        /// </summary>
        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
                this.RaisePropertyChanged("Notes");
            }
        }

        private string _defaultValue;
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
                this.RaisePropertyChanged("DefaultValue");
            }
        }
    }
}
