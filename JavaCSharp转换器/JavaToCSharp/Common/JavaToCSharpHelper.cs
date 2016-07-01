using JavaToCSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaToCSharp.Common
{

    /// <summary>
    /// ID:no
    /// Describe:转换javatoc#代码
    /// Author:ybx
    /// Date:2016-5-13 09:25:53
    /// </summary>
    public class JavaToCSharpHelper
    {
        public static string Descrypt(string Content, string format = null)
        {
            if (string.IsNullOrEmpty(Content))
            {
                return string.Empty;
            }
            string ResContent = string.Empty;
            string className = string.Empty;
            string[] temp = Content.Split("\r\n".ToCharArray());
            List<string> realtemp = new List<string>();
            List<ProInfo> pros = new List<ProInfo>();

            //过滤非类名，非属性代码
            foreach (string item in temp)
            {
                if (item.Contains("private") || item.Contains(@"//") || item.Contains("class"))
                {
                    realtemp.Add(item.Trim());
                }
            }
            if (realtemp.Count == 0)
            {
                return string.Empty;
            }

            //修正在代码行上面注释的代码
            for (int i = 0; i < realtemp.Count; i++)
            {
                if (!realtemp[i].Contains("class") && !(realtemp[i].Contains("private") && realtemp[i].Contains(@"//")))
                {
                    if (realtemp[i].Contains("private") && i - 1 >= 0 && realtemp[i - 1].Contains(@"//"))
                    {
                        realtemp[i] = realtemp[i].Trim() + realtemp[i - 1].Trim();
                    }
                }
            }

            //类名加工
            foreach (var item in realtemp)
            {
                Regex reg = new Regex(@"\s?public\s?class\s(\w+)\s.*");
                GroupCollection result = reg.Match(item).Groups;
                className = result[1].ToString();
                if (!string.IsNullOrEmpty(className))
                {
                    break;
                }
            }
            //属性加工
            foreach (var item in realtemp)
            {
                if (item.Contains("private") && item.Contains(@"//"))
                {
                    Regex reg = new Regex(@"\s?(private)\s?(\w+<?\w+>?)\s(\w+)\s?(=?.*);\s?//\s?(.*)");
                    GroupCollection result = reg.Match(item).Groups;
                    ProInfo info = new ProInfo()
                    {
                        Name = result[3].ToString(),
                        Opening = result[1].ToString(),
                        Typer = result[2].ToString(),
                        Notes = result[5].ToString(),
                        DefaultValue = result[4].ToString()
                    };
                    pros.Add(info);
                }
            }

            //属性代码生成
            foreach (var item in pros)
            {
                string res = string.Empty;
                try
                {
                    if (string.IsNullOrEmpty(format))
                    {
                        res = string.Format(@"
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
", item.Notes, item.Typer, item.Name, UpperFirst(item.Name), item.DefaultValue);
                    }
                    else
                    {
                        res = string.Format(format, item.Notes, item.Typer, item.Name, UpperFirst(item.Name), item.DefaultValue);
                    }

                    //替换ArrayList为List
                    res = res.Replace("ArrayList", "List");
                }
                catch (Exception)
                { }

                ResContent += res;
            }

            //生成类名
            if (!string.IsNullOrEmpty(className))
            {
                ResContent = string.Format(@"public class {0} 
{{
{1}
}}", UpperFirst(className), ResContent);
            }

            return ResContent;
        }

        /// <summary>
        /// 首字母变大写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UpperFirst(string value)
        {
            var temp = value.ToCharArray();
            temp[0] = temp[0].ToString().ToUpper()[0];
            return string.Concat(temp);
        }
    }
}
