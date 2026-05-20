using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;

namespace SuperMarket
{
    public partial class ProductsForm : Form
    {
        // Stores the ID of the currently selected product from the grid
        int selectedId = 0;

        public ProductsForm()
        {
            InitializeComponent(); // Initialize all form controls
        }

        // Runs when the form loads — populates the grid and configures its behavior
        private void ProductsForm_Load(object sender, EventArgs e)
        {
            LoadProducts();                  // Fetch and display all products
            dgvProducts.ReadOnly = true;     // Prevent direct editing in the grid
            dgvProducts.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect; // Highlight full row on click
        }

        // Fetches all products from the database and binds them to the DataGridView
        private void LoadProducts()
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM products";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);                      // Fill DataTable with query results
                    dgvProducts.DataSource = dt;      // Bind DataTable to the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Add button click — inserts a new product into the database
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validate that name and price fields are not empty
            if (txtProductName.Text == "" || txtPrice.Text == "")
            {
                MessageBox.Show("Name and Price are required!");
                return;
            }
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    // Parameterized INSERT query to prevent SQL injection
                    string query = @"INSERT INTO products
                        (product_name, category, price, stock)
                        VALUES (@name, @cat, @price, @stock)";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@cat", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@stock", txtStock.Text);
                    cmd.ExecuteNonQuery(); // Execute the INSERT
                }
                MessageBox.Show("Product Added Successfully!");
                LoadProducts(); // Refresh the grid
                ClearFields();  // Reset input fields
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Update button click — updates the selected product's details
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Ensure a product is selected before attempting update
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a product first!");
                return;
            }
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    // Parameterized UPDATE query targeting the selected product by ID
                    string query = @"UPDATE products SET
                        product_name=@name, category=@cat,
                        price=@price, stock=@stock WHERE id=@id";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@cat", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@stock", txtStock.Text);
                    cmd.Parameters.AddWithValue("@id", selectedId);
                    cmd.ExecuteNonQuery(); // Execute the UPDATE
                }
                MessageBox.Show("Product Updated!");
                LoadProducts(); // Refresh the grid
                ClearFields();  // Reset input fields
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Delete button click — removes the selected product from the database
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Ensure a product is selected before attempting delete
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a product first!");
                return;
            }
            // Ask the user to confirm the deletion
            if (MessageBox.Show("Delete this product?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = CDBHelper.GetConnection())
                    {
                        conn.Open();
                        // DELETE query targeting the selected product by ID
                        SQLiteCommand cmd = new SQLiteCommand(
                            "DELETE FROM products WHERE id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", selectedId);
                        cmd.ExecuteNonQuery(); // Execute the DELETE
                    }
                    MessageBox.Show("Product Deleted!");
                    LoadProducts(); // Refresh the grid
                    ClearFields();  // Reset input fields
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Fires when a cell in the grid is clicked — loads the selected row's data into input fields
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ignore header row clicks
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["id"].Value);                  // Store selected product's ID
                txtProductName.Text = row.Cells["product_name"].Value.ToString();     // Populate product name field
                txtCategory.Text = row.Cells["category"].Value.ToString();            // Populate category field
                txtPrice.Text = row.Cells["price"].Value.ToString();                  // Populate price field
                txtStock.Text = row.Cells["stock"].Value.ToString();                  // Populate stock field
            }
        }

        // Handles the Clear button click — resets the form to its default state
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // Clears all input fields and resets the selected product ID
        private void ClearFields()
        {
            txtProductName.Clear();
            txtCategory.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            selectedId = 0; // Reset so no product is considered selected
        }
    }
}