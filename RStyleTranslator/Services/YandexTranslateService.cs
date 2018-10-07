using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace RStyleTranslator
{
    public class YandexTranslateService : ITranslateService
    {
        string key;

        /// <summary>
        /// Создает объект сервиса яндекс
        /// </summary>
        /// <param name="key">API-ключ</param>
        public YandexTranslateService(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Выполняет перевод сервисом яндекса
        /// </summary>
        /// <param name="fromLangId">шифр исходного языка</param>
        /// <param name="toLangId">шифр языка перевода</param>
        /// <param name="rawText">текст для перевода</param>
        /// <returns>переведенный текст</returns>
        public async Task<string> Translate(string fromLangId, string toLangId, string rawText)
        {
            string ret = null;
            Task<WebResponse> getResponseTask = null;
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                WebRequest request = WebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/translate?"
                    + "key=" + key
                    + "&text=" + HttpUtility.UrlEncode(rawText)
                    + "&lang=" + HttpUtility.UrlEncode(fromLangId + "-" + toLangId));
                getResponseTask = request.GetResponseAsync();
                await getResponseTask;
            }
            catch (NotSupportedException respEx)
            {
                throw new Exception("Ошибка запроса: " + respEx.Message, respEx);
            }
            catch (WebException respEx)
            {
                string error = "";
                if (respEx.Status == WebExceptionStatus.ProtocolError)                  
                {
                    if (respEx.Message.Length > 36 + 3 && int.TryParse(respEx.Message.Substring(36, 3), out int code))
                    {

                        switch (code)
                        {
                            case 401: case 402: error = "Ошибка yandex: Проблемы с API-ключом"; break;
                            case 404: error = "Ошибка yandex: Превышено суточное ограничение на объем переведенного текста"; break;
                            case 413: case 414: error = "Ошибка yandex: Превышен максимально допустимый размер текста"; break;
                            case 422: error = "Ошибка yandex: Текст не может быть переведен"; break;
                            case 501: error = "Ошибка yandex: Заданное направление перевода не поддерживается"; break;
                            default: error = $"Неизвестная ошибка yandex: код {code}, текст {respEx.Message}"; break;
                        }
                    }
                    else
                        error = "Неизвестная ошибка yandex: " + respEx.Message;
                }
                else
                    error = "Ошибка подключения к yandex";                
                throw new Exception(error, respEx);
            }
            catch (Exception respEx)
            {
                throw new Exception("Неизвестная ошибка " + respEx.Message, respEx);
            }

            if (getResponseTask.Status != TaskStatus.Faulted)
            {
                WebResponse response = getResponseTask.Result;
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    string line;

                    if ((line = stream.ReadLine()) != null)
                    {
                        Translation translation = JsonConvert.DeserializeObject<Translation>(line);
                        ret = "";
                        foreach (var el in translation.text)
                            ret += translation.text[0];
                    }
                }
            }
            Mouse.OverrideCursor = null;
            return ret;
        }
        class Translation
        {
            public string code { get; set; }
            public string lang { get; set; }
            public string[] text { get; set; }
        }

        /// <summary>
        /// Запрашивает базу языков и направлений перевода с яндекса
        /// </summary>
        /// <returns>Структура LangInfo</returns>
        public async Task<LangInfo> GetLangsInfo()
        {
            LangInfo langInfo=null;
            Task<WebResponse> getResponseTask = null;
            
            try
            {
                WebRequest request = WebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/getLangs?"
                    + "key=" + key
                    + "&ui=ru"
                );
                getResponseTask = request.GetResponseAsync();
                await getResponseTask;
            }
            catch (NotSupportedException respEx)
            {
                throw new Exception("Ошибка запроса: " + respEx.Message, respEx);
            }
            catch (WebException respEx)
            {
                throw new Exception("Ошибка подключения к yandex: " + respEx.Message, respEx);
            }
            if (getResponseTask.Status != TaskStatus.Faulted)
            {
                WebResponse response = getResponseTask.Result;
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    string line;
                    if ((line = stream.ReadLine()) != null)
                         langInfo = JsonConvert.DeserializeObject<LangInfo>(line);                        
                }
            }
            return langInfo;
        }
      
    }
}