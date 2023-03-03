using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Printing;
using System.Windows.Forms.VisualStyles;

namespace Блокнот
{
    public partial class Form1 : Form
    {
        public System.Drawing.FontStyle fs = FontStyle.Regular;
        public int fontSize = 0;
        public string filename;
        public bool isFileCnanged;

        public FontSettings fontss;
        public FontSettings fontSetts;
        public Form1()
        {
            InitializeComponent(); //Инициализация компонентов с конструктора

            Init();
        }
        public void Init()
        {
            filename = "";
            isFileCnanged = false;
            UpdateTextWithTitle();
            FontSettings fs = new FontSettings();
        }

        // sender - предоставляет ссылку на объект, вызвавший событие
        // EventArgs e передает объект, относящийся к обрабатываемому событию
        public void CreateNewDocument(object sender, EventArgs e) // Новое окно
        {
            SaveUnsavedFile();
            TextField.Text = "";
            filename = "";
            isFileCnanged = false;
            UpdateTextWithTitle();
        }

        public void OpenFile(object sender, EventArgs e) // Открыть файл
        {
            openFileDialog1.FileName = "";
            SaveUnsavedFile();
            // openfileDialog1 - экземпляр класса OpenFileDialog помогает отркыть диалоговое окно
            // DialogResult отоброжает что было нажато в диалоговом окне
            // было ли нажато в модальном окне 'ok'
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    {
                        // в наше поле для текста ввводим все из файла
                        TextField.Text = sr.ReadToEnd();
                    }
                    filename = openFileDialog1.FileName;
                    isFileCnanged = false;
                }
                catch
                {
                    // Выводит сообщение 
                    MessageBox.Show("Невозможно открыть файл");
                }
            }
            UpdateTextWithTitle();
        }

        public void SaveFile(string _filename) // Сохранение файла
        {   
            if (_filename == "") // запись в новый файл
            {
                // True если диалоговое окно будет запрашивать разрешение на сохранение файла
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    _filename = saveFileDialog1.FileName;
                }
            }
            // запись в существующий файл
            try
            {
                using (StreamWriter sw = new StreamWriter(_filename + ".txt"))
                {
                    sw.Write(TextField.Text);
                    filename = _filename;
                    isFileCnanged = false;
                }
            }
            catch 
            {
                MessageBox.Show("Невозможно сохранить файл!");
            }
            UpdateTextWithTitle();
        }

        public void Save(object sender, EventArgs e) // Сохранить
        {
            SaveFile(filename);
        }

        public void SaveAs(object sender, EventArgs e) // Сохранить как
        {
            SaveFile(filename);
        }

        public void NewWindow(object sender, EventArgs e) // Новое окно
        {
            // Создаем экземпляр класса Form1 для того, чтобы использовать Show() и открыть новое окно
            // !!! Не ShowDialog() тк этот метод создаст модальное окно(нужен ответ пользователя перед тем как работать с прошлыми окнами)
            var Form2 = new Form1();
            Form2.Show();
        }
        
        public void Exit(object sender, EventArgs e) // Выход
        {
            Close();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!isFileCnanged)
            {
                this.Text = this.Text.Replace('*', ' ');
                isFileCnanged= true;
                this.Text = '*' + this.Text;
            }
        }

        private void OnTextCganged(object sender, EventArgs e)
        {
            if (!isFileCnanged)
            {
                this.Text = this.Text.Replace('*', ' ');
                this.Text = '*' + this.Text;
                isFileCnanged= true;
            }
        }

        public void UpdateTextWithTitle()
        {
            if (filename != "")
            {
                this.Text = filename + " - Notepad";
            }
            else
            {
                this.Text = filename + "Безымянный - Notepad";
            }
        }

        public void SaveUnsavedFile()
        {
            if (isFileCnanged)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в файле?", "Сохранение файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes) 
                {
                    SaveFile(filename);
                }
            }
        }
        public void CopyText()
        {
            if (TextField.SelectedText != "")
            {
                Clipboard.SetText(TextField.SelectedText);
            }
        }
        public void CutText()
        {
            if (TextField.SelectedText != "")
            {
                Clipboard.SetText(TextField.SelectedText);
                TextField.Text = TextField.Text.Remove(TextField.SelectionStart, TextField.SelectionLength);
            }
        }
        public void PasteText()
        {
            
            TextField.Text = TextField.Text.Substring(0, TextField.SelectionStart) + Clipboard.GetText() + TextField.Text.Substring(TextField.SelectionStart, TextField.Text.Length - TextField.SelectionStart);
            
        }
        private void OnCopyClick(object sender, EventArgs e)
        {
            CopyText();
        }
        private void OnCutClick(object sender, EventArgs e)
        {
            CutText();
        }

        private void OnPasteClick(object sender, EventArgs e)
        {
            PasteText();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            SaveUnsavedFile();
        }

        private void OnFontClick(object sender, EventArgs e)
        {
            fontSetts = new FontSettings();
            fontSetts.Show();
        }

        private void OnFocus(object sender, EventArgs e)
        {
            if (fontSetts != null)
            {
                fontSize = fontSetts.fontSize;
                fs = fontSetts.fs;
                TextField.Font = new Font(TextField.Font.FontFamily, fontSize, fs);
                fontSetts.Close();
            }
        }

        private void ClickSeal(object sender, EventArgs e)
        {
            PrintDocument pDocument = new PrintDocument();
            pDocument.PrintPage += PrintPageH;
            PrintDialog pDialog = new PrintDialog();
            pDialog.Document = pDocument;
            if (pDialog.ShowDialog() == DialogResult.OK)
            {
                pDialog.Document.Print();
            }
        }

        public void PrintPageH(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(TextField.Text, TextField.Font, Brushes.Black, 0, 0);
        }

        private void infoProgramClick(object sender, EventArgs e)
        {
            DialogResult = MessageBox.Show("Блокнот");
        }
        
    }
}
