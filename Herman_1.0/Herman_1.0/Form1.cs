using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using System.IO;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.Threading;

namespace Herman_1._0
{
    public partial class Form1 : Form
    {

        int call = 0;
        int exitSudos = 0;
        int system = 0;

        string cmdSys = "";

        static TextBox textBox;

        public Form1()
        {
            InitializeComponent();
        }

        
        public void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            if (Directory.Exists("settings/"))
            {
                if (File.Exists("settings/name.hs") == false)
                {
                    File.WriteAllText("settings/name.hs", "судо");
                }
            }
            else
            {
                Directory.CreateDirectory("settings/");
                
                File.WriteAllText("settings/name.hs", "судо");
            }


            if (e.Result.Confidence > 0.5)
            {


                if(e.Result.Text == File.ReadAllText("settings/name.hs"))
                {
                    speak("Я вас слушаю");
                    call = 1;
                }
                else
                {
                    textBox.Text = e.Result.Text + "\n" + e.Result.Confidence;
                    obr(e.Result.Text);
                    call = 0;
                }
                /*textBox.Text = e.Result.Text;

                if(e.Result.Text == "как тебя зовут")
                {
                    speak("Меня зовут Судо.");
                }
                if(e.Result.Text == "привет")
                {
                    speak("Привет.");
                }
                if (e.Result.Text == "как дела")
                {
                    speak("У меня всё отлично. А у вас?");
                }
                if(e.Result.Text == "завершить программу")
                {
                    speak("Программа завершена!");
                    Application.Exit();
                }*/
            }
        }

        private void obr(string name)
        {
            string[] comand = File.ReadAllLines("settings/commands.hs");
            using(StreamReader sr = new StreamReader("settings/commands.hs"))
            {
                String line;

                while((line = sr.ReadLine()) != null)
                {
                    string[] stroka = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if(stroka[0] == name)
                    {
                        if(stroka[1] == "open")
                        {
                            Process.Start(stroka[2]);
                            speak("открываю");
                            break;
                        }
                        if(stroka[1] == "hello")
                        {
                            speak("Здравствуйте");
                            break;
                        }
                        if(stroka[1] == "name")
                        {
                            speak("Я робот по имени" + File.ReadAllText("settings/name.hs"));
                        }
                        if(stroka[1] == "exitSudo")
                        {
                            speak("Вы действительно этого хотите?");
                            call = 1;
                            exitSudos = 1;
                            break;
                        }
                        if(stroka[1] == "exitSudoYes" && exitSudos == 1)
                        {
                            speak("Как жаль. Выключаюсь");
                            Application.Exit();
                        }
                        if(stroka[1] == "exitSudoNo" && exitSudos == 1)
                        {
                            speak("Я буду ждать пока вы не позовете меня");
                            exitSudos = 0;
                            call = 0;
                            break;
                        }
                        if(stroka[1] == "cmd")
                        {
                            speak("Вы действительно этого хотите?");
                            system = 1;
                            cmdSys = stroka[2];
                            break;
                        }
                        if (stroka[1] == "exitSudoYes" && system == 1 && cmdSys != "")
                        {
                            speak("Выполняю");
                            Process.Start("cmd", cmdSys);
                            break;
                        }
                        if (stroka[1] == "exitSudoNo" && system == 1)
                        {
                            speak("Отменяю действие");
                            cmdSys = "";
                            system = 0;
                            break;
                        }
                        if (stroka[1] == "aboutyou")
                        {
                            speak("Меня создал программист");
                            break;
                        }
                        /*speak("Я не знаю такой команды но попытюсь найти это в интернете");
                        Process.Start("https://www.google.com/search?q=" + name);*/
                    }
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox = textBox1;

            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-ru");

            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
            try
            {
                sre.SetInputToDefaultAudioDevice();
            }
            catch (Exception)
            {

                MessageBox.Show("Микрофон не подключен");
                Application.Exit();
            }
            

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);

            Choices numbers = new Choices();
            //numbers.Add(new string[] { "один", "два", "три", "четыре", "пять", "как тебя зовут", "блин", "привет", "как дела", "завершить программу" });
            string[] comand = File.ReadAllLines("settings/commands.hs");
            using (StreamReader sr = new StreamReader("settings/commands.hs"))
            {
                String line;
                while ((line = sr.ReadLine()) != null) //читаем по одной линии(строке) пока не вычитаем все из потока (пока не достигнем конца файла)
                {
                    string[] stroka = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    numbers.Add(new string[] { stroka[0] });
                }
            }


            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            numbers.Add(new string[] { File.ReadAllText("settings/name.hs") });
            gb.Append(numbers);

            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);

            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static void speak(string text)
        {
            SpeechSynthesizer speaker = new SpeechSynthesizer();
            speaker.SetOutputToDefaultAudioDevice();
            speaker.Rate = 1;
            speaker.Volume = 100;
            speaker.Speak(text);
        }
    }
}
