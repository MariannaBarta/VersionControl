using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using Webszolgaltatas_DCWC5L.Entities;
using Webszolgaltatas_DCWC5L.MnbServiceReference;

namespace Webszolgaltatas_DCWC5L
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currencies = new BindingList<string>();
        string result = null;
        string result_currency = null;

        public Form1()
        {
            InitializeComponent();

            GetCurrency();

            RefreshData();
        }

        public void RefreshData()
        {
            Rates.Clear();

            GetExchangeRate();

            LoadXmlDocument();

            LoadCurrencyList();

            AddDiagram();

            dataGridView1.DataSource = Rates;
            chartRateData.DataSource = Rates;
            comboBox1.DataSource = Currencies;
        }

        public string GetCurrency()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            var request_currency = new GetCurrenciesRequestBody()
            {
            };

            var response_currency = mnbService.GetCurrencies(request_currency);

            var result_currency = response_currency.GetCurrenciesResult;

            return result_currency;
        }


        public string GetExchangeRate()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = comboBox1.SelectedItem.ToString(),
                startDate = dateTimePicker1.Value.ToString(),
                endDate = dateTimePicker2.Value.ToString()
            };

            var response = mnbService.GetExchangeRates(request);

            var result = response.GetExchangeRatesResult;

            return result;

        }

        public void LoadCurrencyList()
        {
            result_currency = GetCurrency();
            var xml_curr = new XmlDocument();
            xml_curr.LoadXml(result_currency);

            foreach (XmlElement element in xml_curr.DocumentElement)
            {
                string currency = null; ;
                Currencies.Add(currency);

                currency = element.GetAttribute("curr");

            }
        }

        private void LoadXmlDocument()
        {
            result = GetExchangeRate();
            var xml = new XmlDocument();
            xml.LoadXml(result);

            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new RateData();
                Rates.Add(rate);

                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                if (childElement == null)
                    continue;
                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;

            }

        }

        public void AddDiagram()
        {
            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
