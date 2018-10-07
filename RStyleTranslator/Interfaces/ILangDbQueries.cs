using RStyleTranslator.Models;
using System.Collections.Generic;

namespace RStyleTranslator
{
    public interface ILangDbQueries
    {
        void UpdateLangs(LangInfo langInfo);
        IEnumerable<Lang> GetFromLangs();
        IEnumerable<Lang> GetToLangs(string fromLangId);
    }
}