using Admin.Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Assignment1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Sale : Window
    {
        public static NpgsqlConnection con;
        public static NpgsqlCommand cmd;
        HttpClient client = new HttpClient();
        public Sale()
        {
            client.BaseAddress = new Uri("https://localhost:7244/Produce/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            InitializeComponent();
            confirmSale.IsEnabled = false;
        }
        

        private void establishConnect()
        {
            // need to install postgresql adapter/library from packageManager
            // NpgSQL
            // create instances of connector and command adapter

            try
            {
                con = new NpgsqlConnection(get_ConnectionString());
            }
            catch (NpgsqlException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private string get_ConnectionString()
        {
            // pass 5 values: host, port, dbName, userName, password

            string host = "Host=localhost;";
            string port = "Port=5432;";
            string dbName = "Database=producePro;";
            string userName = "Username=postgres;";
            string password = "Password=irahudgin;";

            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }

        private async void calculateTotal_Click(object sender, RoutedEventArgs e)
        {
                int prodId = int.Parse(productId.Text);
                
                var server_response = await client.GetStringAsync("GetProducebyProductId/" + productId.Text);
                Response response_JSON = JsonConvert.DeserializeObject<Response>(server_response);
                
                // string query = "select amount from produce where product_id" +
                   // " = @prodId";
                double amountProduce = 0;
                amountProduce = response_JSON.produce.amount;

                double amountRequested = double.Parse(amountKg.Text);

                if ((amountProduce - amountRequested) < 0)
                {
                    MessageBox.Show("Not enough produce in stock. Check stock and try again");
                    Sale newSale = new Sale();
                    newSale.Show();
                    this.Close();
                    return;
                }

            server_response = await client.GetStringAsync("GetProducebyProductId/" + productId.Text);
            response_JSON = JsonConvert.DeserializeObject<Response>(server_response);
            bool noData = true;
            string priceStr = "";
                 
            noData = false;
            priceStr = response_JSON.produce.price.ToString();

                double priceMulti = double.Parse(priceStr);
                double priceTotal = priceMulti * double.Parse(amountKg.Text);
                total.Text = priceTotal.ToString();
                /// MessageBox.Show(priceTotal.ToString());
                confirmSale.IsEnabled = true;

        }

        private async void confirmSale_Click(object sender, RoutedEventArgs e)
        {
            Sales sale = new Sales();

                double totalDoub = double.Parse(total.Text);

                sale.amountBought = double.Parse(amountKg.Text);
                sale.productId = int.Parse(productId.Text);
                sale.customerName = custName.Text;

            var server_response = await client.PostAsJsonAsync("AddSale", sale);

            var server_response2 = await client.GetStringAsync("GetProducebyProductId/" + productId.Text);
            var response_JSON = JsonConvert.DeserializeObject<Response>(server_response2);

            Produce produce = new Produce();
            produce.productId = response_JSON.produce.productId;
            produce.produceId = response_JSON.produce.produceId;
            produce.price = response_JSON.produce.price;
            produce.amount = response_JSON.produce.amount;

            var server_response3 = await client.PutAsJsonAsync("ProduceSaleUpdate", produce);
                MessageBox.Show("Sale Confirmed!");
                Sale newSale = new Sale();
                newSale.Show();
                this.Close();
                Admin adminWindow = new Admin();
                adminWindow.Show();
            
        }
    }
}
