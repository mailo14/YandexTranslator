using RStyleTranslator;
using RStyleTranslator.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Ninject;
using System.Threading.Tasks;

namespace RStyleTranslator
{
    /// <summary>
    /// ViewModel переводчика
    /// </summary>
    public class TranslatorViewModel : ViewModelBase
    {
        ITranslateService _TranslateService;
        ILangDbQueries _LangDbQueries;
        /// <summary>
        /// Создает ViewModel переводчика
        /// </summary>
        /// <param name="langDbQueries">сервис запросов к бд</param>
        /// <param name="translateService">сервис перевода</param>
        public TranslatorViewModel(ILangDbQueries langDbQueries, ITranslateService translateService)
        {
            this._LangDbQueries = langDbQueries;
            this._TranslateService = translateService;
        }

        #region Свойства
        string _FromLangId;
        /// <summary>
        /// Шифр (краткое название) исходного языка для перевода
        /// </summary>
        public string FromLangId
        {
            get => _FromLangId;
            set
            {
                _FromLangId = value;
                IsTranslated = false;
                OnPropertyChanged();
                OnPropertyChanged("ToLangs");
            }
        }

        string _ToLangId;
        /// <summary>
        /// Шифр конечного языка перевода
        /// </summary>
        public string ToLangId
        {
            get => _ToLangId;
            set
            {
                _ToLangId = value;
                IsTranslated = false;
                TranslatedText = "";
                OnPropertyChanged();
            }
        }

        
        string _RawText = "Исходный текст";
        /// <summary>
        /// Исходный текст
        /// </summary>
        public string RawText
        {
            get => _RawText;
            set
            {
                _RawText = value;
                IsTranslated = false;
                TranslatedText = "";
                OnPropertyChanged();
            }
        }

        string _TranslatedText;
        /// <summary>
        /// Перевод исходного текста
        /// </summary>
        public string TranslatedText
        {
            get => _TranslatedText;
            set
            {
                _TranslatedText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Доступные языки исходного текста
        /// </summary>
        public ObservableCollection<Lang> FromLangs
        {
            get => new ObservableCollection<Lang>(_LangDbQueries.GetFromLangs());
        }

        /// <summary>
        /// Доступные конечные языки для перевода
        /// </summary>
        public ObservableCollection<Lang> ToLangs
        {
            get => new ObservableCollection<Lang>(_LangDbQueries.GetToLangs(FromLangId));
        }

        bool _IsTranslated = false;
        /// <summary>
        /// Был ли переведен текущий текст
        /// </summary>
        public bool IsTranslated
        {
            get => _IsTranslated;
            set
            {
                if (_IsTranslated != value)
                {
                    _IsTranslated = value;
                }
            }
        }
        #endregion

        #region Команды
        ICommand _TranslateCommand;
        public ICommand TranslateCommand
        {
            get
            {
                if (_TranslateCommand == null)
                {
                    _TranslateCommand = new AwaitableDelegateCommand(Translate, CanTranslate);
                }
                return _TranslateCommand;
            }
        }

        /// <summary>
        /// Выполняет перевод исходного текста
        /// </summary>        
        async Task Translate()
        {
            try
            {
                TranslatedText = await _TranslateService.Translate(FromLangId, ToLangId, RawText);
                IsTranslated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool CanTranslate()
        {
            return !IsTranslated
                && FromLangId != null
                && ToLangId != null
                && RawText.Trim().Length > 0;
        }

        ICommand _UpdateLangsCommand;
        public ICommand UpdateLangsCommand
        {
            get
            {
                if (_UpdateLangsCommand == null)
                {
                    _UpdateLangsCommand = new AwaitableDelegateCommand(UpdateLangs);
                }
                return _UpdateLangsCommand;
            }
        }
        /// <summary>
        /// Выполняет обновление базы языков
        /// </summary>
        async Task UpdateLangs()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var langInfo = await _TranslateService.GetLangsInfo();

                _LangDbQueries.UpdateLangs(langInfo);

                OnPropertyChanged("FromLangs");
                FromLangId = "ru";
                TranslatedText = "";
                Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show(ex.Message);
            }
        }

        DelegateCommand _SaveXmlCommand;
        public ICommand SaveXmlCommand
        {
            get
            {
                if (_SaveXmlCommand == null)
                {
                    _SaveXmlCommand = new DelegateCommand(SaveXml, CanSaveXml);
                }
                return _SaveXmlCommand;
            }
        }

        /// <summary>
        /// Сохраняет перевод в файл xml по выбранному пути
        /// </summary>
        void SaveXml()
        {
            var saveDialogService = new SaveDialogService();
            if (saveDialogService.OpenSaveFileDialog() == true)
            {
                XDocument xmldoc = MakeXml();
                try
                {
                    string path = saveDialogService.FilePath +
                        (!saveDialogService.FilePath.EndsWith(".xml") ? ".xml" : "");
                    xmldoc.Save(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
                MessageBox.Show("Файл сохранен");
            }
        }   

        bool CanSaveXml()
        {
            return IsTranslated;
        }
        #endregion

        /// <summary>
        /// Генерирует xml по переводу
        /// </summary>
        /// <returns></returns>
        public XDocument MakeXml()
        {
            return new XDocument(
                 new XElement("translate",
                     new XElement("rawText",
                         new XAttribute("code", FromLangId),
                         new XAttribute("value", RawText)
                     ),
                     new XElement("translateText",
                         new XAttribute("code", ToLangId),
                         new XAttribute("value", TranslatedText)
                     ),
                     new XElement("time",
                         new XAttribute("value", DateTime.Now.ToString("dd.MM.yyyy THH:mm"))
                     )
                 )
            );
        }
    }
}