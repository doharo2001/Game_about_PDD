using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string fpath; // путь к файлу теста
        string fname; // файл теста

        // XmlReader обеспечивает чтение данных xml-файла
        System.Xml.XmlReader xmlReader;
       
        string qw;     // вопрос
        
        // варианты ответа
        string[] answ = new string[3];
 
        string pic;    // путь к файлу иллюстрации

        int right; // правильный ответ (номер)
        int otv;   // выбранный ответ (номер)
        int n;     // количество правильных ответов
        int nv;    // общее количество вопросов
        int mode;  // состояние программы:
                           // 0 - начало работы;
                           // 1 - тестирование;
                           // 2 - завершение работы

        // конструктор формы
        // (см. также Program.cs )
        public Form1(string[] args)
        {
            InitializeComponent();

            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;

            // имя файла теста должно быть указано
            // в качестве парамета команды запуска программы
            if (args.Length > 0)
            {
               // указано только имя файла теста
               if (args[0].IndexOf(":") == -1) {
               fpath = Application.StartupPath + "\\";
               fname = args[0];
            }
                
            else
            {
                // указан путь к файлу теста
                fpath = args[0].Substring(0,args[0].LastIndexOf("\\")+1);
                fname = args[0].Substring(args[0].LastIndexOf("\\")+1);
            }

        try
        {
            // получаем доступ к xml-документу
            xmlReader = new System.Xml.XmlTextReader(fpath + fname);
            xmlReader.Read();

            mode = 0;
            n    = 0;

            // загрузить заголовок теста
            this.showHead();

            // загрузить описание теста
            this.showDescription();
        }
        
        catch(Exception exc)
        {
            label1.Text = "Ошибка доступа к файлу  " +
                fpath + fname;

            MessageBox.Show("Ошибка доступа к файлу.\n" +
                fpath + fname + "\n",
                "Экзаменатор",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            mode = 2;
        }
    }
    else
    {
        label1.Text =
            "Файл теста необходимо указать " +
            "в команде запуска программы.\n" +
            "Например: 'exam economics.xml' " +
            "или 'exam c:\\spb.xml'.";
        mode = 2;
    }
  }

  // выводит название (заголовок) теста
  private void showHead()
  {
    // ищем узел <head>
    do xmlReader.Read();
    while(xmlReader.Name != "head");

    // считываем заголовок
    xmlReader.Read();

    // вывести название теста в заголовок окна
    this.Text = xmlReader.Value;

    // выходим из узла <head>
    xmlReader.Read();
  }

    // выводит описание теста
    private void showDescription()
    {
        // ищем узел <description>
        do
            xmlReader.Read();
        while(xmlReader.Name != "description");

        // считываем описание теста
        xmlReader.Read();

        // выводим описание теста
        label1.Text = xmlReader.Value;

        // выходим из узла <description>
        xmlReader.Read();

        // ищем узел вопросов <qw>
        do
            xmlReader.Read();
        while(xmlReader.Name != "qw");

        // входим внутрь узла
        xmlReader.Read();
    }

    // читает вопрос из файла теста
    private Boolean getQw() {
        // считываем тэг <q>
        xmlReader.Read();

        if (xmlReader.Name == "q")
        {
            // здесь прочитан тэг <q>,
            // атрибут text которого содержит вопрос, а
            // атрибут src - имя файла иллюстрации.

            // извлекаем значение атрибутов:
            qw  = xmlReader.GetAttribute("text");
            pic = xmlReader.GetAttribute("src");
            if (!pic.Equals(string.Empty)) pic = fpath + pic;

            // входим внутрь узла
            xmlReader.Read();
            int i = 0;

            // считываем данные узла вопроса <q>
            while (xmlReader.Name != "q")
            {
                xmlReader.Read();

                // варианты ответа
                if (xmlReader.Name == "a")
                {
                    // запоминаем правильный ответ
                    if (xmlReader.GetAttribute("right") == "yes")
                        right = i;

                    // считываем вариант ответа
                    xmlReader.Read();
                    if (i < 3) answ[i] = xmlReader.Value;

                    // выходим из узла <a>
                    xmlReader.Read();

                    i++;
                }
            }

            // выходим из узла вопроса <q>
            xmlReader.Read();

            return true;
        }
        // если считанный тэг не является
        // тэгом вопроса <q>
        else
            return false;
    }

    // выводит вопрос и варианты ответа
    private void showQw() {
        // выводим вопрос
        label1.Text = qw;

        // иллюстрация
        if (pic.Length != 0)
        {
            try
            {
                pictureBox1.Image =
                new Bitmap(pic);

                pictureBox1.Visible = true;

                radioButton1.Top = pictureBox1.Bottom + 16;
            }
            catch
            {
                if (pictureBox1.Visible)
                    pictureBox1.Visible = false;

                label1.Text +=
                    "\n\n\nОшибка доступа к файлу " + pic + ".";

                radioButton1.Top = label1.Bottom + 8;
            }
        }
        else
        {
            if (pictureBox1.Visible)
                pictureBox1.Visible = false;

            radioButton1.Top = label1.Bottom;
        }

        // показать варианты ответа
        radioButton1.Text = answ[0];
        radioButton2.Top  = radioButton1.Top + 24;;
        radioButton2.Text = answ[1];
        radioButton3.Top  = radioButton2.Top + 24;;
        radioButton3.Text = answ[2];

        radioButton4.Checked = true; 
        button1.Enabled = false;
    }

    // щелчок на кнопке выбора ответа
    // функция обрабатывает событие Click
    // компонентов radioButton1 - radioButton3
    private void radioButton1_Click(object sender, EventArgs e)
    {
        if ((RadioButton)sender == radioButton1) otv = 0;
        if ((RadioButton)sender == radioButton2) otv = 1;
        if ((RadioButton)sender == radioButton3) otv = 2;

        button1.Enabled = true;
    }

    // щелчок на кнопке Ok
    private void button1_Click_1(object sender, EventArgs e)
    {
        switch (mode)
        {
        case 0:        // начало работы программы
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton3.Visible = true;

            this.getQw();
            this.showQw();

            mode = 1;

            button1.Enabled = false;
            radioButton4.Checked = true;
            break;

        case 1:
            nv++;

            // правильный ли ответ выбран
            if (otv == right) n++;

            if (this.getQw()) this.showQw();
            else {
                // больше вопросов нет
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;

                pictureBox1.Visible  = false;

                // обработка и вывод результата
                this.showLevel();

                // следующий щелчок на кнопке Ok
                // закроет окно программы
                mode = 2;
            }
            break;
        case 2:   // завершение работы программы
            this.Close(); // закрыть окно
            break;
        }
    }

    // выводит оценку 
    private void showLevel()
    {
        // ищем узел <levels>
        do
            xmlReader.Read();
        while (xmlReader.Name != "levels");

        // входим внутрь узла
        xmlReader.Read();

        // читаем данные узла
        while (xmlReader.Name != "levels")
        {
            xmlReader.Read();

            if (xmlReader.Name == "level")
                // n - кол-во правильных ответов,
                // проверяем, попадаем ли в категорию
                if (n >= System.Convert.ToInt32(
                    xmlReader.GetAttribute("score")))
                break;
        }

        // выводим оценку
        label1.Text =
            "Тестирование завершено.\n" +
            "Всего вопросов: " + nv.ToString() + ". " +
            "Правильных ответов: " + n.ToString() + ".\n" +
            xmlReader.GetAttribute("text");
    }
  }
}
