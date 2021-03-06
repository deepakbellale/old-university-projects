﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Service
{
    public partial class Form2 : Form
    {
        DataSet dsz;
        public Form2(ref DataSet ds)
        {
            InitializeComponent();
            this.dsz = ds;

            dataGridView.DataSource = dsz.Tables["КомпанииЗаказчики"];
        }

        //Add
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // генерация данных о заказе
                // создание нового заказа
                DataRow newrow = dsz.Tables["КомпанииЗаказчики"].NewRow();
                // заполнение атрибутов заказчика
                if (textBox1.Text.CompareTo("0") == 0)
                    newrow["IDКомпанииЗаказчика"] = dsz.Tables["КомпанииЗаказчики"].Rows.Count + 1;
                else
                    newrow["IDКомпанииЗаказчика"] = int.Parse(textBox1.Text);
                newrow["НазваниеКомпанииЗаказчика"] = (string)textBox2.Text;
                newrow["СпецификацияКомпании"] = (string)textBox3.Text;
                newrow["СредстваСвязи"] = (string)textBox4.Text;

                // записываем созданную запись в таблицу
                dsz.Tables["КомпанииЗаказчики"].Rows.Add(newrow);

                // работа с DataGridView, показываеющей таблицу заказчиков
                // отмена выделения всех выбранных строк в DataGridView
                foreach (DataGridViewRow dgvr in dataGridView.SelectedRows)
                    dgvr.Selected = false;

                // установка выбора вновь созданного заказчика
                // последняя строка DataGridView – это строка для ручного ввода новой 
                // записи, поэтому последняя значимая строка – предпоследняя
                dataGridView.Rows[dataGridView.Rows.Count - 2].Selected = true;
            }
            catch
            {
                MessageBox.Show("Данная компания-заказчик уже существует в базе данных!");
            }
        }

        //Delete
        private void button2_Click(object sender, EventArgs e)
        {   
            // если заказ не был выбран, удалять нечего
            if (dataGridView.SelectedRows.Count == 0)
                return;

            //получение номера текущего выбранного заказазчика - ID
            int nom = (int)dataGridView.SelectedRows[0].Cells["IDКомпанииЗаказчика"].Value;

            // выбираем все записи, соответствующие текущему заказу
            DataRow[] drs = dsz.Tables["КомпанииЗаказчики"].Select("IDКомпанииЗаказчика=" + nom);

            // удаляем все найденные заказы из таблиц набора данных
            //for (int i = drs.Length - 1; i >= 0; i--)
            //  drs[i].Delete();
            // поиск заказа по ключу
            dsz.Tables["КомпанииЗаказчики"].Rows.Find(new object[] { (object)nom }).Delete();
            // удаление заказа из набора данных  
            // DataGridView заказов обновится автоматически

        }

        //Save
        private void button3_Click(object sender, EventArgs e)
        {
            dsz.WriteXml("isps.xml", XmlWriteMode.WriteSchema);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            dsz.WriteXml("isps.xml", XmlWriteMode.WriteSchema);
        }

        

    }
}
