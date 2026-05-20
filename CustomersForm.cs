using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;

namespace SuperMarket
{
    public partial class CustomersForm : Form
    {
        // Stores the ID of the currently selected customer from the grid
        int selectedId = 0;

        public CustomersForm()
        {
            InitializeComponent();
        }

        // Runs when the form loads
        private void CustomersForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();                  // Populate the grid with customer data
            dgvCustomers.ReadOnly = true;     // Prevent direct editing in the grid
            dgvCustomers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect; // Highlight the full row on click
        }

        // Fetches all customers from the database and binds them to the DataGridView
        private void LoadCustomers()
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM customers";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);                        // Fill DataTable with query results
                    dgvCustomers.DataSource = dt;       // Bind DataTable to the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Add button click — inserts a new customer into the database
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validate that the name field is not empty
            if (txtName.Text == "")
            {
                MessageBox.Show("Customer name is required!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    // Parameterized INSERT query to prevent SQL injection
                    string query = @"INSERT INTO customers 
                        (name, phone, email) 
                        VALUES (@name, @phone, @email)";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.ExecuteNonQuery(); // Execute the INSERT
                }
                MessageBox.Show("Customer Added Successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomers(); // Refresh the grid
                ClearFields();   // Reset input fields
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Update button click — updates the selected customer's details
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Ensure a customer is selected before attempting update
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a customer first!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    // Parameterized UPDATE query targeting the selected customer by ID
                    string query = @"UPDATE customers SET 
                        name=@name, phone=@phone, 
                        email=@email WHERE id=@id";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@id", selectedId);
                    cmd.ExecuteNonQuery(); // Execute the UPDATE
                }
                MessageBox.Show("Customer Updated Successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomers(); // Refresh the grid
                ClearFields();   // Reset input fields
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Delete button click — removes the selected customer from the database
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Ensure a customer is selected before attempting delete
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a customer first!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Ask the user to confirm the deletion
            if (MessageBox.Show("Are you sure you want to delete?",
                "Confirm", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = CDBHelper.GetConnection())
                    {
                        conn.Open();
                        // DELETE query targeting the selected customer by ID
                        SQLiteCommand cmd = new SQLiteCommand(
                            "DELETE FROM customers WHERE id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", selectedId);
                        cmd.ExecuteNonQuery(); // Execute the DELETE
                    }
                    MessageBox.Show("Customer Deleted!",
                        "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers(); // Refresh the grid
                    ClearFields();   // Reset input fields
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Fires when a cell in the grid is clicked — loads the selected row's data into input fields
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ignore header row clicks
            {
                DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["id"].Value);       // Store selected customer's ID
                txtName.Text = row.Cells["name"].Value.ToString();         // Populate name field
                txtPhone.Text = row.Cells["phone"].Value.ToString();       // Populate phone field
                txtEmail.Text = row.Cells["email"].Value.ToString();       // Populate email field
            }
        }

        // Handles the Clear button click — resets the form to its default state
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // Clears all input fields and resets the selected customer ID
        private void ClearFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            selectedId = 0; // Reset so no customer is considered selected
        }

    
            private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT * FROM customers
                WHERE name LIKE @search
                OR phone LIKE @search
                OR email LIKE @search";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search",
                        "%" + txtSearch.Text + "%");
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCustomers.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

      
            private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadCustomers();
        }
    }
    }
    
