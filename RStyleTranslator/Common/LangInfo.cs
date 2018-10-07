using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RStyleTranslator
{
    public class LangInfo
    {
        public string[] dirs { get; set; } //направления перевода вида ru-en
        public Dictionary<string, string> langs { get; set; } //шифры и названия языков
    }
}
