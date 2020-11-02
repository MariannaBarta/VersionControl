using Mikroszimulacio_DCWC5L.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mikroszimulacio_DCWC5L
{
    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();

        int StartYear = 2005;
        int EndYear;
        List<string> Results_Male = new List<string>();
        List<string> Results_Female = new List<string>();

        Random rng = new Random();



        public Form1()
        {
            InitializeComponent();

            Population = GetPopulation(@"C:\Temp\nép-teszt.csv");
            BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");

        }

        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                }
            }

            return population;
        }


        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> birth_population = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birth_population.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        BirthProb = double.Parse(line[2])
                    });
                }
            }

            return birth_population;
        }


        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> death_population = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    death_population.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Age = int.Parse(line[1]),
                        DeathProb = double.Parse(line[2])
                    });
                }
            }

            return death_population;
        }

        public void Simulation(int StartYear, int EndYear)
        {
            for (int year = StartYear; year <= EndYear; year++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    Person person = Population[i];
                    SimStep(year, person);
                }
                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();
                
                Results_Male.Add(Environment.NewLine + string.Format("Szimulációs év: {0}" + Environment.NewLine + "\tFiúk: {1}" , year, nbrOfMales, nbrOfFemales));
                Results_Female.Add(Environment.NewLine + string.Format("Szimulációs év: {0}" + Environment.NewLine + "\tLányok: {2}", year, nbrOfMales, nbrOfFemales));
                richTextBox1.AppendText(Results_Male.Last());
                richTextBox1.AppendText(Results_Female.Last());
            }
        }

        private void SimStep(int year, Person person)
        {
            if (!person.IsAlive) return;

            byte age = (byte)(year - person.BirthYear);

            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Age == age
                             select x.DeathProb).FirstOrDefault();

            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            if (person.IsAlive && person.Gender == Gender.Female)
            {
                double pBirth = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.BirthProb).FirstOrDefault();

                if (rng.NextDouble() <= pBirth)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.NbrOfChildren = 0;
                    újszülött.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(újszülött);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            Results_Male.Clear();
            Results_Female.Clear();
            Population.Clear();
            BirthProbabilities.Clear();
            DeathProbabilities.Clear();
            EndYear = decimal.ToInt32(numericUpDown1.Value);
            Simulation(StartYear, EndYear);
            richTextBox1.AppendText(Environment.NewLine);
            //DisplayResults();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"C:\Temp";
            openFileDialog1.ShowDialog();
            textBox2.Text = openFileDialog1.FileName;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = Math.Min(Math.Max(numericUpDown1.Value, numericUpDown1.Minimum), numericUpDown1.Maximum);
        }


    }
}
