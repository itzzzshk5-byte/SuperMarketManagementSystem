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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // Initialize all form controls
        }

        // Runs when the form loads — no startup logic needed here
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Fires when the username textbox text changes — no action needed
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        // Fires when the label is clicked — no action needed
        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Handles the Login button click — validates input and checks credentials
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Check if username or password fields are empty or whitespace
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter all fields.", "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further execution if validation fails
            }
            ;

            // Hardcoded credentials for login verification
            string correctUsername = "admin";
            string correctPassword = "noor123";

            // Check if entered credentials match the correct ones
            if (textBox1.Text == correctUsername && textBox2.Text == correctPassword)
            {
                // Credentials are correct — open the Market form
                Market mt = new Market();
                mt.Show();
            }
            else
            {
                // Credentials are wrong — show error message
                MessageBox.Show("Invalid username or password.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}