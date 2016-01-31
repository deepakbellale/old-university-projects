﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Service
{
    public partial class Form1 : Form
    {
        DataSet ds = new DataSet();//Для таблицы Заказов и ЗаписейОплаты

        List<string> ids = new List<string>();
        List<string> idz = new List<string>();

        int nom;//Номер текущего заказа
        DateTime date;//Дата текущего заказа
        public Form1()
        {
            InitializeComponent();
            textBox2.Text = DateTime.Now.ToShortDateString();//Дата заказа
            textBox3.Text = "0";//Цена
            textBox4.Text = DateTime.Now.ToShortDateString();//Дедлайн заказа
            textBox13.Text = DateTime.Now.ToShortDateString();//Дата оплаты
            textBox7.Text = DateTime.Now.ToShortDateString();//Дата заказа

            //Проверяем наличие файлов
            if (!File.Exists("bd.xml"))
                CreateBD();
            else
                // считываем информацию из файлов с данными
                ds.ReadXml("bd.xml", XmlReadMode.ReadSchema);

            // установка источника данных для DataGridView
            dataGridView1.DataSource = ds.Tables["Заказы"];
            dataGridView4.DataSource = ds.Tables["ЗаписьОплаты"];

            dataGridView2.DataSource = ds.Tables["КомпанииЗаказчики"];
            dataGridView3.DataSource = ds.Tables["КомпанииИсполнители"];

            Refresh();
            comboBox1.DataSource = ids;
            comboBox2.DataSource = idz;
        }

        // Для загрузки IDшников в комбобоксы
        void Refresh()
        {
            ids.Clear();
            idz.Clear();
            // загрузим в комбобоксы реальные ID
            // выбор выбранного заказа и заполнение второго DataGridView
            DataRowCollection drs3 = ds.Tables["КомпанииИсполнители"].Rows;
            DataRowCollection drsZ = ds.Tables["КомпанииЗаказчики"].Rows;

            // последовательное заполнение информации о найденных записях 
            // в DataGridView для записей заказов
            foreach (DataRow dr in drs3)
            {
                ids.Add(dr["IDКомпанииИсполнителя"].ToString());
            }
            foreach (DataRow dr in drsZ)
            {
                idz.Add(dr["IDКомпанииЗаказчика"].ToString());
            }

        }
        //База данных заказов и оплаты заказов
        public void CreateBD()
        {
            // создаем таблицу заказов
            // добавляем таблицу в список таблиц набора данных
            ds.Tables.Add(new DataTable("Заказы"));
            // атрибут
            DataColumn dc = new DataColumn("НомерЗаказа", Type.GetType("System.Int32"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("IDКомпанииИсполнителя", Type.GetType("System.Int32"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("IDКомпанииЗаказчика", Type.GetType("System.Int32"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("Выполнено/НеВыполнено", Type.GetType("System.Boolean"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("ДатаСдачиЗаказа", Type.GetType("System.DateTime"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("ДатаПриёмаЗаказа", Type.GetType("System.DateTime"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("Название/Описание", Type.GetType("System.String"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // атрибут  
            dc = new DataColumn("Стоимость", Type.GetType("System.Int32"));
            ds.Tables["Заказы"].Columns.Add(dc);
            // описание первичного ключа - массива ссылок на столбцы таблицы
            // первичным ключом будет номер
            DataColumn[] keyZ = new DataColumn[2] {ds.Tables["Заказы"].Columns["НомерЗаказа"], ds.Tables["Заказы"].Columns["ДатаПриёмаЗаказа"] };//
            ds.Tables["Заказы"].PrimaryKey = keyZ;

            // создаем таблицу заказов
            // добавляем таблицу в список таблиц набора данных
            ds.Tables.Add(new DataTable("ЗаписьОплаты"));
            // атрибут
            dc = new DataColumn("НомерЗаказа", Type.GetType("System.Int32"));
            ds.Tables["ЗаписьОплаты"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("ДатаПриёмаЗаказа", Type.GetType("System.DateTime"));
            ds.Tables["ЗаписьОплаты"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("Оплачено/НеОплачено", Type.GetType("System.Boolean"));
            ds.Tables["ЗаписьОплаты"].Columns.Add(dc);
            // атрибут  
            dc = new DataColumn("Предоплата", Type.GetType("System.Int32"));
            ds.Tables["ЗаписьОплаты"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("ДатаОплаты", Type.GetType("System.DateTime"));
            ds.Tables["ЗаписьОплаты"].Columns.Add(dc);
            // описание первичного ключа - массива ссылок на столбцы таблицы
            // первичным ключом будет номер
            DataColumn[] keyZO = new DataColumn[2] { ds.Tables["ЗаписьОплаты"].Columns["НомерЗаказа"], ds.Tables["ЗаписьОплаты"].Columns["ДатаПриёмаЗаказа"] };//
            ds.Tables["ЗаписьОплаты"].PrimaryKey = keyZO;

            // создание связи между таблицами
            // указывается имя отношения и два массива связанных полей - для родительской и дочерних таблиц
            DataRelation rel = new DataRelation("СвязьЗаказаСОплатойЗаказа",
            new DataColumn[] { ds.Tables["Заказы"].Columns["НомерЗаказа"] },//, ds.Tables["Заказы"].Columns["ДатаПриёмаЗаказа"] },
            new DataColumn[] { ds.Tables["ЗаписьОплаты"].Columns["НомерЗаказа"] });//, ds.Tables["ЗаписьОплаты"].Columns["ДатаПриёмаЗаказа"] });
            //добавляем связь в список связей набора данных
            ds.Relations.Add(rel);

            // создаем таблицу заказов
            // добавляем таблицу в список таблиц набора данных
            ds.Tables.Add(new DataTable("КомпанииЗаказчики"));
            // формируем список столбцов таблицы заказчиков
            // для каждого столбца указывается имя столбца и тип данных
            dc = new DataColumn("IDКомпанииЗаказчика", Type.GetType("System.Int32"));
            ds.Tables["КомпанииЗаказчики"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("НазваниеКомпанииЗаказчика", Type.GetType("System.String"));
            ds.Tables["КомпанииЗаказчики"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("СпецификацияКомпании", Type.GetType("System.String"));
            ds.Tables["КомпанииЗаказчики"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("СредстваСвязи", Type.GetType("System.String"));
            ds.Tables["КомпанииЗаказчики"].Columns.Add(dc);
            // описание первичного ключа - массива ссылок на столбцы таблицы
            // первичным ключом будет комбинация номера и даты
            DataColumn[] keyZak = new DataColumn[1] { ds.Tables["КомпанииЗаказчики"].Columns["IDКомпанииЗаказчика"] };
            ds.Tables["КомпанииЗаказчики"].PrimaryKey = keyZak;


            // создаем таблицу исполнителей
            // добавляем таблицу в список таблиц набора данных
            ds.Tables.Add(new DataTable("КомпанииИсполнители"));
            // атрибут
            dc = new DataColumn("IDКомпанииИсполнителя", Type.GetType("System.Int32"));
            ds.Tables["КомпанииИсполнители"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("НазваниеКомпанииИсполнителя", Type.GetType("System.String"));
            ds.Tables["КомпанииИсполнители"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("СпецификацияКомпании", Type.GetType("System.String"));
            ds.Tables["КомпанииИсполнители"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("СредстваСвязи", Type.GetType("System.String"));
            ds.Tables["КомпанииИсполнители"].Columns.Add(dc);
            // атрибут
            dc = new DataColumn("СчетчикКомпанииИсполнителя", Type.GetType("System.Int32"));
            ds.Tables["КомпанииИсполнители"].Columns.Add(dc);
            // описание первичного ключа - массива ссылок на столбцы таблицы
            // первичным ключом будет номер 
            DataColumn[] keyIsp = new DataColumn[1] { ds.Tables["КомпанииИсполнители"].Columns["IDКомпанииИсполнителя"] };
            ds.Tables["КомпанииИсполнители"].PrimaryKey = keyIsp;

            ds.WriteXml("bd.xml", XmlWriteMode.WriteSchema);
        }



        //Выборка данных при клике на строчку в таблице "Заказы"
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Выводим информацию о заказах по заказчику
            // сохранение номера выбранного заказа из ячейки строки 
            // номер строки приходит в качестве параметра
            if (dataGridView1.RowCount > 1)
            {
                nom = (int)dataGridView1.Rows[e.RowIndex].Cells["IDКомпанииЗаказчика"].Value;
                ZagrZak(nom);
                nom = (int)dataGridView1.Rows[e.RowIndex].Cells["IDКомпанииИсполнителя"].Value;
                ZagrIsp(nom);
            }
        }


        //Загрузка таблицы исполнителей
        private void ZagrIsp(int nomer)
        {
            try
            {
                // очистка DataGridView, т.к. изменяется
                dataGridView3.Rows.Clear();

                // выбор выбранного заказа и заполнение второго DataGridView
                DataRow[] drs3 = ds.Tables["КомпанииИсполнители"].Select("IDКомпанииИсполнителя=" + nomer);

                // последовательное заполнение информации о найденных записях 
                // в DataGridView для записей заказов
                foreach (DataRow dr in drs3)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    dgvr.CreateCells(dataGridView3,dr["IDКомпанииИсполнителя"], dr["НазваниеКомпанииИсполнителя"], dr["СпецификацияКомпании"], dr["СредстваСвязи"], dr["СчетчикКомпанииИсполнителя"]);
                    dataGridView3.Rows.Add(dgvr);
                }
            }
            catch(Exception er)
            {
                MessageBox.Show("Вы не ввели данные об исполнителе заказа! Данные отсутствуют\n"+er.Message);
            }
        }

        //Загрузка таблицы заказчиков
        private void ZagrZak(int nomer)
        {
            try
            {
                //// очистка DataGridView, т.к. изменяется
                //dataGridView2.Rows.Clear();

                // выбор выбранного заказа и заполнение второго DataGridView
                DataRow[] drs2 = ds.Tables["КомпанииЗаказчики"].Select("IDКомпанииЗаказчика=" + nomer);

                // последовательное заполнение информации о найденных записях 
                // в DataGridView для записей заказов
                foreach (DataRow dr in drs2)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    dgvr.CreateCells(dataGridView2, dr["IDКомпанииЗаказчика"], dr["НазваниеКомпанииЗаказчика"], dr["СпецификацияКомпании"], dr["СредстваСвязи"]);
                    dataGridView2.Rows.Add(dgvr);
                }
            }
            catch
            {
                MessageBox.Show("Вы не ввели данные о заказчике! Данные отсутствуют");
            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ds.WriteXml("bd.xml", XmlWriteMode.WriteSchema);
        }

        private void DELETEOLD_Click(object sender, EventArgs e)
        {
            // если заказ не был выбран, удалять нечего
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            //получение номера текущего выбранного заказа
            nom = (int)dataGridView1.SelectedRows[0].Cells["НомерЗаказа"].Value;
            DateTime date = (DateTime)dataGridView1.SelectedRows[0].Cells["ДатаПриёмаЗаказа"].Value;

            // выбираем все записи, соответствующие текущему заказу
            DataRow[] drs = ds.Tables["Заказы"].Select("НомерЗаказа=" + nom + " and ДатаПриёмаЗаказа='"+date+"'");

            // удаляем все найденные заказы из таблиц набора данных
            for (int i = drs.Length - 1; i >= 0; i--)
                drs[i].Delete();
            // поиск заказа по ключу
            //ds.Tables["Заказы"].Rows.Find(new object[] {(object)nom,(object)idIsp,(object)idZ}).Delete();
            // удаление заказа из набора данных  
            // DataGridView заказов обновиться автоматически
  
            // очистка DataGridView2 и DataGridView3 с записями о заказе(КомпанииЗаказчики и КомпанииИсполнители) – т.к. выбранного заказа уже нет
            //dataGridView2.Rows.Clear();
            //dataGridView3.Rows.Clear();
        }

        private void ADDNEW_Click(object sender, EventArgs e)
        {
            try
            {
                // генерация данных о заказе
                // создание нового заказа
                DataRow newrow = ds.Tables["Заказы"].NewRow();
                // заполнение атрибутов заказа
                // для определения номера заказа можно узнать 
                // количество строк в таблице заказов

                newrow["НомерЗаказа"] = ds.Tables["Заказы"].Rows.Count + 1;
                nom = (int)newrow["НомерЗаказа"];
                newrow["ДатаПриёмаЗаказа"] = DateTime.Parse(textBox2.Text.ToString());


                if (comboBox1.Text.ToString().CompareTo("") == 0)
                    newrow["IDКомпанииИсполнителя"] = nom;
                else
                    newrow["IDКомпанииИсполнителя"] = int.Parse(comboBox1.Text.ToString());

                if (comboBox2.Text.ToString().CompareTo("") == 0)
                    newrow["IDКомпанииЗаказчика"] = nom;
                else
                    newrow["IDКомпанииЗаказчика"] = int.Parse(comboBox2.Text.ToString());

                newrow["Выполнено/НеВыполнено"] = Boolean.Parse(checkBox1.Checked.ToString());
                newrow["ДатаСдачиЗаказа"] = DateTime.Parse(textBox4.Text.ToString());
                newrow["Название/Описание"] = (string)textBox8.Text;
                newrow["Стоимость"] = int.Parse(textBox3.Text.ToString());

                // запоминаем дату и номер заказа
                nom = (int)newrow["НомерЗаказа"];

                // записываем созданную запись в таблицу
                ds.Tables["Заказы"].Rows.Add(newrow);

                // очищаем DataGridView для исполнителей и заказчиков – т.к. создается новый заказ, 
                // у которого еще нет записей
                //dataGridView2.Rows.Clear();
                //dataGridView3.Rows.Clear();

                // работа с DataGridView, показываеющей таблицу заказов
                // отмена выделения всех выбранных строк в DataGridView
                foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
                    dgvr.Selected = false;

                // установка выбора вновь созданного заказа
                // последняя строка DataGridView – это строка для ручного ввода новой 
                // записи, поэтому последняя значимая строка – предпоследняя
                dataGridView1.Rows[dataGridView1.Rows.Count - 2].Selected = true;    
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message);
            }
            
        }

        //Редактирование заказчиков
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 zaks=new Form2(ref this.ds);

            if (zaks.Visible == false)
            {
                zaks.Show();
            }
        }

        //Редактирование исполнителей
        private void button3_Click(object sender, EventArgs e)
        {
            Form3 isps = new Form3(ref this.ds);

            if (isps.Visible == false)
            {
                isps.Show();
            }
        }

        //Добавление Оплаты в Таблицу "ЗаписьОплаты"
        private void button1_Click(object sender, EventArgs e)
        {
            // генерация данных о заказе
            // создание нового заказа
            try
            {
                DataRow newrow = ds.Tables["ЗаписьОплаты"].NewRow();
                // заполнение атрибутов заказа
                // для определения номера заказа можно узнать 
                // количество строк в таблице заказов

                newrow["НомерЗаказа"] = int.Parse(textBox1.Text.ToString());
                newrow["ДатаПриёмаЗаказа"] = DateTime.Parse(textBox7.Text.ToString());
                newrow["Оплачено/НеОплачено"] = Boolean.Parse(checkBox2.Checked.ToString());
                newrow["Предоплата"] = int.Parse(textBox12.Text.ToString());
                newrow["ДатаОплаты"] = DateTime.Parse(textBox13.Text.ToString());

                // записываем созданную запись в таблицу
                ds.Tables["ЗаписьОплаты"].Rows.Add(newrow);

                // работа с DataGridView, показываеющей таблицу заказов
                // отмена выделения всех выбранных строк в DataGridView
                foreach (DataGridViewRow dgvr in dataGridView4.SelectedRows)
                    dgvr.Selected = false;

                // установка выбора вновь созданного заказа
                // последняя строка DataGridView – это строка для ручного ввода новой 
                // записи, поэтому последняя значимая строка – предпоследняя
                dataGridView4.Rows[dataGridView4.Rows.Count - 2].Selected = true;
            }
            catch
            {
                MessageBox.Show("Ошибка номера или даты заказа(заказ с данным номером должен уже существовать)!");
            }        
        }

        //Удаление выбранной оплаты(если ошибочно внесена)
        private void button4_Click(object sender, EventArgs e)
        {
            // если заказ не был выбран, удалять нечего
            if (dataGridView4.SelectedRows.Count == 0)
                return;

            //получение номера текущего выбранного заказа
            nom = (int)dataGridView4.SelectedRows[0].Cells["НомерЗаказа"].Value;
            DateTime date = (DateTime)dataGridView4.SelectedRows[0].Cells["ДатаПриёмаЗаказа"].Value;

            // выбираем все записи, соответствующие текущему заказу
            DataRow[] drs = ds.Tables["ЗаписьОплаты"].Select("НомерЗаказа=" + nom + " and ДатаПриёмаЗаказа='" + date + "'");

            // удаляем все найденные заказы из таблиц набора данных ЗаписОплаты
            for (int i = drs.Length - 1; i >= 0; i--)
                drs[i].Delete();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = ds.Tables["КомпанииЗаказчики"];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView3.DataSource = ds.Tables["КомпанииИсполнители"];
        }
    }
}

