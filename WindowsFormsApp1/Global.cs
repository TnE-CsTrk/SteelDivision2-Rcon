using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public enum Language
    {
        zh_CN,
        en_US
    }
    public class Global
    {
        public static Language language = Language.zh_CN;

    }
}
