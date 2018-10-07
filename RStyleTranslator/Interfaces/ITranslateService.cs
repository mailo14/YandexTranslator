using System.Collections.Generic;
using System.Threading.Tasks;

namespace RStyleTranslator
{
    public interface ITranslateService
    {
        Task<string> Translate(string fromLangId, string toLangId, string rawText);
        Task<LangInfo> GetLangsInfo();
    }
   
}