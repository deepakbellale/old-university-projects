﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Security.Cryptography;

namespace ApplicationMain
{
    public partial class Form1 : Form
    {
        // RSA
        BigInteger pAlice;
        BigInteger qAlice;
        BigInteger nAlice;
        BigInteger E_alice;
        BigInteger d_alice;

        BigInteger pBob;
        BigInteger qBob;
        BigInteger nBob;         
        BigInteger E_bob;
        BigInteger d_bob;

        private int aliceOrBob;// Флаг - 1 значит, что отправила Алиса, 0 - что отправил Боб
        string _message = string.Empty;
        string _sign = string.Empty;
        
        public Form1()
        {
            InitializeComponent();
            
            generateButton.Enabled= false;
            generateButton2.Enabled = true;

            ourMessageTextBox.Enabled = true;

            newSingTextBox.Enabled = false;
        }
        
        // Расширенный алгоритм Евклида
        private void РасширенныйАлгоритмЕвклида(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y, out BigInteger d)
        {
            BigInteger q, r, x1, x2, y1, y2;

            if (b == 0)
            {
                d = a;
                x = 1;
                y = 0;
                return;
            }

            x2 = 1;
            x1 = 0;
            y2 = 0;
            y1 = 1;

            while (b > 0)
            {
                q = a / b;
                r = a - q * b;
                x = x2 - q * x1;
                y = y2 - q * y1;
                a = b;
                b = r;
                x2 = x1;
                x1 = x;
                y2 = y1;
                y1 = y;
            }

            d = a;
            x = x2;
            y = y2;
        }

        BigInteger НайтиОбратныйПоМодулю(BigInteger a, BigInteger n)
        {
            BigInteger x, y, d;
            РасширенныйАлгоритмЕвклида(a, n, out x, out y, out d);

            if (d == 1) return x;

            return 0;

        }

        // Тест Миллера-Рабина
        bool ТестМиллераРабина(BigInteger n,  BigInteger a)
        {
            // Отсеиваем тривиальные случаи(самые простые)
            if (n <= 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;
            
            BigInteger s = 0, d = n - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            // Реализация проверок по возведению в степень по модулю 'n'
            BigInteger r = 1;
            BigInteger x = (BigInteger)BigInteger.ModPow(a, d, n);
            if (x == 1 || x == n - 1)
                return true;

            x = (long)BigInteger.ModPow(BigInteger.Pow(a, (int)d), BigInteger.Pow(2, (int)r), n);

            if (x == 1)
                return false;

            if (x != n - 1)
                return false;

            return true;
        }


        // Функция проверки, использует тест Миллера-Рабина
        bool ПроверитьЧислоТестомМиллераРабина(BigInteger i)
        {
            // ПРОВЕРКА НЕ НА 2х ОСНОВАНИЯХ 'a', а на 10! Т.е. чем больше оснований - тем точнее проверка числа на простоту long[] massA = new long [11];
            // Разные основания 'а' от 2 до 10
            for (long a = 2; a < i && a < 11; a++)
            {
                // Запускаем тест
                bool b = ТестМиллераРабина((long)i, (long)a);// Передаем число и проверяем
                if (!b)
                {
                    return false;
                }
            }
            return true;
        }

        // Получить простое число
        public  long ПолучитьПростоеЧисло(ref int i)
        {
            Random r = new Random();
            long число = 0;
            bool isCorrect = false;

            for ( ; i < 10 * i && !isCorrect; i++)
            {
                    число = i; //(long)r.Next(3, 15);
                    if (ПроверитьЧислоТестомМиллераРабина(число))
                    {
                        break;
                    }
                    //else
                    //{
                    //    Console.WriteLine("Введенное число НЕ простое!");
                    //}
            }

            return число;
        }

        // Функция для умножения двух чисел x,y по модулю m
        BigInteger mulmod(BigInteger x,BigInteger y, BigInteger m)
        {
            return (x*y)%m;
        }

        // Функция возведения числа x в степен а по модулю n
        BigInteger powmod(BigInteger x,BigInteger a,BigInteger m)
        {
                BigInteger r = 1;
                while(a>0)
                {
                        if(a%2!=0) 
                           r=mulmod(r,x,m);
                        a=a>>1; 
                        x=mulmod(x,x,m);
                }
                return r;
        }
        
        // Генерация простых чисел
        private void generateSimpleButton_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int i = r.Next(1, 100);
            BigInteger fn;

           

            //1 Алиса
            i = i + 1;
            pAlice = ПолучитьПростоеЧисло(ref i);
            // Ищем второе простое число
            i = i + 1;
            qAlice = ПолучитьПростоеЧисло(ref i);

            nAlice = pAlice * qAlice;

            // Выведем сгенерированные простые числа
            pAliceTextBox.Text = pAlice.ToString();
            qAliceTextBox.Text = qAlice.ToString();
            nAliceTextBox.Text = (pAlice * qAlice).ToString();

            // Установим константу 'e'
            r = new Random();

            E_alice = (BigInteger)new long[] { 17, 257, 65537 }[r.Next(0,3)];// Одно из 3х простых чисел Ферма
            eAliceTextBox.Text = E_alice.ToString();

            // Найдем константу 'd_alice' - закрытый ключ Алисы
            fn = (pAlice - 1) * (qAlice - 1);
            d_alice = НайтиОбратныйПоМодулю(E_alice, fn);
            if (d_alice < 0)
                d_alice = fn + d_alice;

            dAliceTextBox.Text = d_alice.ToString();


            //2 Боб
            i = i + 1;
            pBob = ПолучитьПростоеЧисло(ref i);
            // Ищем второе простое число
            i = i + 1;
            qBob = ПолучитьПростоеЧисло(ref i);

            nBob = pBob * qBob;

            // Выведем сгенерированные простые числа
            pBobTextBox.Text = pBob.ToString();
            qBobTextBox.Text = qBob.ToString();
            nBobTextBox.Text = (pBob * qBob).ToString();

            // Установим константу 'e'
            r = new Random();

            E_bob = E_alice;// (BigInteger)new long[] { 65537, 257, 17 }[r.Next(0, 3)];// Одно из 3х простых чисел Ферма
            eBobTextBox.Text = E_bob.ToString();

            // Найдем константу 'd_bob' - закрытый ключ Боба
            var  fn2 = (pBob - 1) * (qBob - 1);
            d_bob = НайтиОбратныйПоМодулю(E_bob, fn2);
            if (d_bob < 0)
                d_bob = fn2 + d_bob;

            dBobTextBox.Text = d_bob.ToString();
            

            Clear();
        }

        public string Decrypt(char[] signMass, BigInteger prK, BigInteger prKN, BigInteger puK, BigInteger puKN)
        {
            string newMessage = "";
            string newMessage2 = "";

            for (int i = 0; i < signMass.Length; i++)
                {
                var a = powmod((BigInteger)signMass[i], prK, prKN);
                newMessage += (char)(((int)a));
            }

            var signMass2 = newMessage.ToCharArray(0, newMessage.Length);

            for (int i = 0; i < signMass2.Length; i++)
            {
                var a = powmod((BigInteger)signMass2[i], puK, puKN);
                newMessage2 += (char)(((int)a));
            }

            return newMessage2;
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            ourMessageTextBox.Text = _message;

            // Переводим сообщение полученное -  в массив символов
            char[] signMass = _sign.ToCharArray(0, _sign.Length);

            // Получаем расшифрованное из подписи сообщение(2 расшифровки)
            string newMessage2 = "";
            if (aliceOrBob == 1)
            {
                newMessage2 = Decrypt(signMass, d_bob, nBob, E_alice, nAlice);
            }
            else
            {
                newMessage2 = Decrypt(signMass, d_alice, nAlice, E_bob, nBob);
            }
           
            // Выводим полученную подпись
            newSingTextBox.Text = newMessage2;
            ourMessageTextBox.Text = sendedMessageTextBox.Text;

            // и выводим результат: зависит от того, совпали подписи или нет
            if (newMessage2 != sendedMessageTextBox.Text)
            {
                checkTextBox.Text = "ИЗМЕНИЛА";
                checkTextBox.BackColor = Color.Red;
            }            
            else
            {
                checkTextBox.Text = "НЕ ИЗМЕНИЛА";
                checkTextBox.BackColor = Color.Green;   
            }    
        }


        private void Clear()
        {
            ourMessageTextBox.Text = string.Empty;
            newSingTextBox.Text = string.Empty;
            checkTextBox.Text = string.Empty;

            checkTextBox.BackColor = Color.White;
        }

        private void ШифровкаОтБоба(object sender, EventArgs e)
        {
            // Попросим ввести сообщение 'm' от Боба
            _message = bobMessageTextBox.Text;
            
            // Шифруем сообщение - сначала приватным ключом Боба, а потом публичным ключом Алисы
            char[] mass = _message.ToCharArray(0, _message.Length);

            // Выведем результат:
            Crypt(mass, d_bob, nBob, E_alice, nAlice);

            // Морозим кнопку Алисы и размораживаем кнопку Боба
            generateButton.Enabled = true;
            generateButton2.Enabled = false;
            aliceOrBob = 0;

            // Чистим промежуточные значения
            Clear();
        }

        private void ШифровкаОтАлисы(object sender, EventArgs e)
        {
            // Попросим ввести сообщение 'm' от Алисы
            _message = aliceMessageTextBox.Text;

            // Шифруем сообщение - сначала приватным ключом Алисы, а потом публичным ключом 
            char[] mass = _message.ToCharArray(0, _message.Length);

            // Выведем результат:
            Crypt(mass, d_alice, nAlice, E_bob, nBob);

            // Морозим кнопку Алисы и размораживаем кнопку Боба
            generateButton.Enabled = false;
            generateButton2.Enabled = true;
            aliceOrBob = 1;

            // Чистим промежуточные значения
            Clear();
        }

        public void Crypt(char[] mass, BigInteger prK, BigInteger prKN, BigInteger puK, BigInteger puKN)
        {
            var sign = "";
            for (int i = 0; i < mass.Length; i++)
            {
                var a = powmod((BigInteger)mass[i], prK, prKN);
                sign += (char)((int)a);
            }

            mass = sign.ToCharArray(0, sign.Length);
            var sign2 = "";

            for (int i = 0; i < mass.Length; i++)
            {
                var a = powmod((BigInteger)mass[i], puK, puKN);
                sign2 += (char)((int)a);
            }
            
            // Выведем результат:
            _sign = sign2;
            signTextBox.Text = _sign;
            sendedMessageTextBox.Text = _message;
        }
        private void ourMessageTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void newSingTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void checkTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void nTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pBobTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
