using RStyleTranslator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RStyleTranslator
{
    public class LangDbQueries: ILangDbQueries
    {
        LangDbFactory _DbFactory;
        public LangDbQueries(LangDbFactory dbFactory)
        {
            this._DbFactory = dbFactory;
        }

        /// <summary>
        /// Удаляет все строки таблицы DbSet-а (TRUNCATE)
        /// </summary>
        /// <param name="dbset">целевой DbSet</param>
        void ClearTable<TEntity>(IQueryable<TEntity> dbset) where TEntity : class
        {
            var query = dbset.ToString();
            var start = query.IndexOf(" FROM ") + 6;
            var end = query.IndexOf(" AS ", start);
            var table = query.Substring(start, end - start);

            _DbFactory.Create().Database.ExecuteSqlCommand($"TRUNCATE TABLE {table}");
        }

        /// <summary>
        /// Получает список доступных языков исходного текста
        /// </summary>
        public IEnumerable<Lang> GetFromLangs()
        {
            var uow = _DbFactory.Create();
            return (from l in uow.Langs join r in uow.Routes on l.Id equals r.FromLangId select l)
                    .Distinct().OrderBy(pp => pp.Name);
        }

        /// <summary>
        /// Получает список доступных конечных языков для перевода
        /// </summary>
        public IEnumerable<Lang> GetToLangs(string fromLangId)
        {
            var uow = _DbFactory.Create();
            if (fromLangId != null)
                return from l in uow.Langs
                       join r in uow.Routes on l.Id equals r.ToLangId
                       where r.FromLangId == fromLangId
                       orderby l.Name
                       select l;
            return Enumerable.Empty<Lang>();
        }

        /// <summary>
        /// Обновляет информацию о языках и направлениях перевода в базе 
        /// </summary>
        public void UpdateLangs(LangInfo langInfo)
        {
            var uow = _DbFactory.Create();
            uow.Configuration.AutoDetectChangesEnabled = false;
            ClearTable(uow.Langs);
            foreach (var l in langInfo.langs)
                uow.Langs.Add(new Lang() { Id = l.Key, Name = l.Value });

            ClearTable(uow.Routes);

            foreach (var r in langInfo.dirs)
            {
                if (r == "sl-en" || r == "en-sl") continue;//требование заказчика – функционал перевода с английского на словенский и наоборот должен быть недоступен
                var pos = r.IndexOf('-');
                uow.Routes.Add(new Route() { FromLangId = r.Substring(0, pos), ToLangId = r.Substring(pos + 1) });
            }
            uow.ChangeTracker.DetectChanges();
            uow.SaveChanges();
        }
    }
}
