using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsFormsApp5
{

    public partial class Form1 : Form
    {
        public List<Student> students = new List<Student>();
        public List<Prepod> prepods = new List<Prepod>();
        public static string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=baza_laboratorka.mdb;";
        private OleDbConnection myConnection;

        public Form1()
        {
            InitializeComponent();
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            this.students.Clear();
            this.prepods.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            string query1 = "SELECT ФИО, Группа from Студенты";
            OleDbCommand command = new OleDbCommand(query1, myConnection);
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Student newstudent = new Student(reader[0].ToString(), reader[1].ToString());
                students.Add(newstudent);
            }
            reader.Close();
            string query2 = "SELECT ФИО from Преподы";
            OleDbCommand command1 = new OleDbCommand(query2, myConnection);
            reader = command1.ExecuteReader();
            while (reader.Read())
            {
                Prepod newprepod = new Prepod(reader[0].ToString());
                prepods.Add(newprepod);
            }
            reader.Close();
            FindPrepodGroups();
            foreach (var stud in students)
            {
                listBox1.Items.Add(stud.name + " " + stud.group);
            }
            foreach (var prep in prepods)
            {
                listBox2.Items.Add(prep.name);
                foreach (var group in prep.groups)
                {
                    listBox2.Items.Add(" " + group);
                }
            }
            string query3 = "SELECT Группа, Преподаватель FROM Кафедра";
            OleDbCommand command2 = new OleDbCommand(query3, myConnection);
            reader = command2.ExecuteReader();
            while (reader.Read())
            {
                listBox3.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
            }
            reader.Close();
        }

        public string ReturnSurname(string input)
        {
            string transformed = "";
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i] == ' ')
                {
                    break;
                }
                else
                {
                    transformed += input[i];
                }
                
            }
            return transformed;
        }

        public void AddStudent(string name, string group)
        {
            string query = $"INSERT INTO Студенты (ФИО, Группа) VALUES ('{name}', '{group}')";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void AddPrepod(string name)
        {
            string query = $"INSERT INTO Преподы (ФИО) VALUES ('{name}') ";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void AddGroup(string group_num, string prepod_name)
        {
            string query = $"INSERT INTO Кафедра (Группа, Преподаватель) VALUES ('{group_num}', '{prepod_name}')";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void ChangeStudent(string num, string name, string group)
        {
            string query = $"UPDATE Студенты SET ФИО = '{name}', Группа = '{group}' WHERE ФИО = '{num}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void ChangePrepod(string num, string name)
        {
            string query = $"UPDATE Преподы SET ФИО = '{name}' WHERE ФИО = '{num}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void ChangeGroup(string num, string group, string prep)
        {
            string query = $"UPDATE Кафедра SET Группа = '{group}', Преподаватель = '{prep}' WHERE Группа = '{num}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void DeleteStudent(string id)
        {
            string query = $"DELETE FROM Студенты WHERE ФИО = '{id}'  ";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void DeletePrepod(string id)
        {
            string query = $"DELETE FROM Преподы WHERE ФИО = '{id}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void DeleteGroup(string id)
        {
            string query = $"DELETE FROM Кафедра WHERE Группа = '{id}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            UpdateInfo();
        }

        public void FindPrepodGroups()
        {
            foreach (var prepod in prepods)
            {
                string query1 = $"SELECT Группа, Преподаватель FROM Кафедра WHERE Преподаватель = '{prepod.name}'";
                OleDbCommand command = new OleDbCommand(query1, myConnection);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    prepod.groups.Add(reader[0].ToString());
                }
                reader.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        public void Search(string name)
        {
            name = ReturnSurname(name);
            foreach (var stud in students)
            {
                if (name == ReturnSurname(stud.name))
                {
                    MessageBox.Show($"Был найден студент {stud.name} из группы {stud.group}");
                    return;
                }
            }
            foreach (var prep in prepods)
            {
                if (name == ReturnSurname(prep.name))
                {
                    string message = $"Был найден преподаватель {prep.name}, он преподает в группах";
                    foreach (var gr in prep.groups)
                    {
                        message += $" {gr},";
                    }
                    MessageBox.Show(message);
                    return;
                }
            }
            MessageBox.Show("Не найдено!");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Search(textBox1.Text);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                AddStudent(textBox1.Text, textBox2.Text);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Код записи не был введен!");
            }
            else
            {
                ChangeStudent(textBox3.Text, textBox1.Text, textBox2.Text);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Код записи не был введен!");
            }
            else
            {
                DeleteStudent(textBox3.Text);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                AddPrepod(textBox4.Text);
            }
        }

        public class Person
        {
            public string name = "";

            public Person(string name)
            {
                this.name = name;
            }
        }

        public class Student : Person
        {
            public string group = "";

            public Student(string name, string group) : base(name)
            {
                this.group = group;
            }
        }

        public class Prepod : Person
        {
            public List<string> groups = new List<string>();

            public Prepod(string name) : base(name) { }

            public void Add_Group(string group_name)
            {
                this.groups.Add(group_name);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                MessageBox.Show("Код записи не был введен!");
            }
            else
            {
                ChangePrepod(textBox7.Text, textBox4.Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                MessageBox.Show("Код записи не был введен!");
            }
            else
            {
                DeletePrepod(textBox7.Text);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox6.Text != "" && textBox5.Text != "")
            {
                AddGroup(textBox6.Text, textBox5.Text);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox8.Text == "")
            {
                MessageBox.Show("Код записи не был введен!");
            }
            else
            {
                ChangeGroup(textBox8.Text, textBox6.Text, textBox5.Text);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox8.Text == "")
            {
                MessageBox.Show("Код записи не был введен!");
            }
            else
            {
                DeleteGroup(textBox8.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myConnection.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Search(textBox9.Text);
        }
    }
}
