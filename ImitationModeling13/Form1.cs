using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImitationModeling9
{
    public partial class Form1 : Form
    {
        Random r = new Random(42);
        public Form1()
        {
            InitializeComponent();
        } 
        private int SimulateExperiment(double[] probs)
        {
            int k = 0;
            double A = r.NextDouble();
            while (A > 0)
                A -= probs[k++];
            return k - 1;
        }
        private int generatePoisson(double lambda)
        {
            
            int m = 0;
            double S = 0;
            while (true)
            {
                double alpha = r.NextDouble();
                S += Math.Log(alpha);
                if (S < -lambda)
                    break;
                else
                    ++m;
            }
            return m;

        }
        private int factorial(int n)
        {
            int res = 1;
            if (n == 0)
                return 1;
            while (n != 1)
            {
                res = res * n;
                n = n - 1;
            }
            return res;
        }
        private double poissonProb(double lambda, int k)
        {
            return (Math.Pow(lambda, k) / factorial(k)) * Math.Exp(-lambda);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double lambda;
            if (!Double.TryParse(lambdaTextBox.Text, out lambda))
                return;            
            chart1.Series[0].Points.Clear();
            

            double E = lambda;            
            double D = lambda;

            int maxRange = Convert.ToInt32(2.3 * lambda);
            int[] stat = new int[maxRange];
            int numExperiments = Convert.ToInt32(numTextBox.Text);
            double Ex = 0;
            double Dx = 0;
            List<int> results = new List<int>();
            for (int i = 0; i < numExperiments; ++i)
            {
                int result = generatePoisson(lambda);
                Ex += result;
                results.Add(result);
                if (result >= maxRange)
                    result = maxRange-1;
                stat[result]++;                
            }
            Ex /= numExperiments;            
            for (int i = 0; i < numExperiments;++i)
                Dx += (results[i] - Ex) * (results[i] - Ex);
            for (int i = 0; i < maxRange; ++i)
            {                
                double prob = stat[i] * 1.0 / numExperiments;                
                chart1.Series[0].Points.Add(prob);
               
            }
            Dx /= numExperiments;
            

            double Er = Math.Abs(E - Ex) / E;
            double Dr = Math.Abs(D - Dx) / D;
            eLabel.Text = String.Format("Average: {0} (error = {1}%)",
                         Ex, Convert.ToInt32(Er*100));
            dLabel.Text = String.Format("Variance: {0} (error = {1}%)",
                         Dx, Convert.ToInt32(Dr * 100));

            double chi = 0;
            for (int i =0; i < maxRange; ++i)
            {
                chi += (stat[i] * stat[i] * 1.0) / (poissonProb(lambda, i) * numExperiments);
            }
            chi -= numExperiments;
            chiLabel.Text = String.Format("{0} > 11.07 is {1}",
                         chi, (chi > 11.07) ? "true" : "false");            
        }
    }
}
