using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using System.Drawing;

namespace SuperMarket
{
    public partial class InventoryForm : Form
    {
        public InventoryForm()
        {
            InitializeComponent(); // Initialize all form controls
        }

        // Runs when the form loads — immediately fetches and displays inventory
        private void InventoryForm_Load(object sender, EventArgs e)
        {
            LoadInventory();
        }

        // Fetches all products from the database and displays them with color-coded stock status
        private void LoadInventory()
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();

                    // Query selects product details and assigns a stock status label
                    // using a CASE statement based on the stock quantity
                    // Results are ordered by stock ascending (lowest stock appears first)
                    string query = @"SELECT 
                        product_name AS 'Product Name',
                        category AS 'Category',
                        price AS 'Price',
                        stock AS 'Stock',
                        CASE
                            WHEN stock = 0 THEN 'OUT OF STOCK'
                            WHEN stock <= 10 THEN 'LOW STOCK'
                            ELSE 'In Stock'
                        END AS 'Status'
                        FROM products
                        ORDER BY stock ASC";

                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);                       // Fill DataTable with query results
                    dgvInventory.DataSource = dt;      // Bind DataTable to the grid

                    // Loop through each row and apply background color based on stock status
                    foreach (DataGridViewRow row in dgvInventory.Rows)
                    {
                        if (row.Cells["Status"].Value != null)
                        {
                            string status = row.Cells["Status"].Value.ToString();

                            if (status == "OUT OF STOCK")
                                row.DefaultCellStyle.BackColor = Color.LightCoral;   // Red for out of stock
                            else if (status == "LOW STOCK")
                                row.DefaultCellStyle.BackColor = Color.LightYellow;  // Yellow for low stock
                            else
                                row.DefaultCellStyle.BackColor = Color.LightGreen;   // Green for in stock
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Refresh button click — reloads inventory data from the database
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInventory(); // Re-fetch and rebind inventory data
            MessageBox.Show("Inventory Refreshed!",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}