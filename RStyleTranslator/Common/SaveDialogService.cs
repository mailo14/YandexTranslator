using Microsoft.Win32;

namespace RStyleTranslator
{
    /// <summary>
    /// Диалог сохранения файла
    /// </summary>
    public class SaveDialogService 
    {
        public string FilePath { get; set; }

        public bool OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

    }
}
