using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperMarket
{
    public partial class Market : Form
    {
        public Market()
        {
            InitializeComponent(); // Initialize all form controls
        }

        // Handles button1 click — opens the Products management form
        private void button1_Click(object sender, EventArgs e)
        {
            ProductsForm pf = new ProductsForm();
            pf.Show(); // Display the Products form
        }

        // Handles button2 click — opens the Customers management form
        private void button2_Click(object sender, EventArgs e)
        {
            CustomersForm cf = new CustomersForm();
            cf.Show(); // Display the Customers form
        }

        // Handles button3 click — opens the Orders management form
        private void button3_Click(object sender, EventArgs e)
        {
            OrdersForm of = new OrdersForm();
            of.Show(); // Display the Orders form
        }

        // Handles button4 click — opens the Inventory management form
        private void button4_Click(object sender, EventArgs e)
        {
            InventoryForm invf = new InventoryForm();
            invf.Show(); // Display the Inventory form
        }
    }
}