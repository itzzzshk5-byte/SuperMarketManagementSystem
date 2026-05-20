using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;

namespace SuperMarket
{
    public partial class OrdersForm : Form
    {
        // In-memory table to hold order items before placing the order
        DataTable orderItemsTable = new DataTable();

        public OrdersForm()
        {
            InitializeComponent(); // Initialize all form controls
        }

        // Runs when the form loads — sets up the grid and loads all required data
        private void OrdersForm_Load(object sender, EventArgs e)
        {
            SetupOrderItemsTable(); // Define columns for the order items grid
            LoadCustomers();        // Populate the customer dropdown
            LoadProducts();         // Populate the product dropdown
            LoadAllOrders();        // Display all existing orders
        }

        // Defines the columns for the order items DataTable and binds it to the grid
        private void SetupOrderItemsTable()
        {
            orderItemsTable.Columns.Add("ProductID", typeof(int));
            orderItemsTable.Columns.Add("Product Name", typeof(string));
            orderItemsTable.Columns.Add("Quantity", typeof(int));
            orderItemsTable.Columns.Add("Price", typeof(double));
            orderItemsTable.Columns.Add("Subtotal", typeof(double));
            dgvOrderItems.DataSource = orderItemsTable; // Bind table to the grid
        }

        // Fetches all customers from the database and populates the customer dropdown
        private void LoadCustomers()
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(
                        "SELECT id, name FROM customers", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cmbCustomer.DataSource = dt;
                    cmbCustomer.DisplayMember = "name"; // Show customer name in dropdown
                    cmbCustomer.ValueMember = "id";     // Use customer ID as the value
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Fetches all products from the database and populates the product dropdown
        private void LoadProducts()
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(
                        "SELECT id, product_name, price, stock FROM products", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cmbProduct.DataSource = dt;
                    cmbProduct.DisplayMember = "product_name"; // Show product name in dropdown
                    cmbProduct.ValueMember = "id";             // Use product ID as the value
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Add Item button click — validates input and adds product to the order items table
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            // Ensure a product is selected
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Please select a product!");
                return;
            }
            // Ensure quantity is entered
            if (txtQuantity.Text == "")
            {
                MessageBox.Show("Please enter quantity!");
                return;
            }

            try
            {
                // Get the selected product's details from the dropdown
                DataRowView selectedProduct =
                    (DataRowView)cmbProduct.SelectedItem;

                int productId = Convert.ToInt32(selectedProduct["id"]);
                string productName = selectedProduct["product_name"].ToString();
                double price = Convert.ToDouble(selectedProduct["price"]);
                int stock = Convert.ToInt32(selectedProduct["stock"]);
                int qty = Convert.ToInt32(txtQuantity.Text);

                // Check if requested quantity is available in stock
                if (qty > stock)
                {
                    MessageBox.Show("Not enough stock! Available: " + stock);
                    return;
                }

                // Calculate subtotal for this item and add it to the order items table
                double subtotal = price * qty;
                orderItemsTable.Rows.Add(productId, productName, qty, price, subtotal);
                txtQuantity.Clear(); // Clear quantity field after adding item
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Place Order button click — saves the order and its items to the database
        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            // Ensure a customer is selected
            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer!");
                return;
            }
            // Ensure at least one item has been added
            if (orderItemsTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add items first!");
                return;
            }

            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();

                    // Calculate the total order amount by summing all item subtotals
                    double total = 0;
                    foreach (DataRow row in orderItemsTable.Rows)
                    {
                        total += Convert.ToDouble(row["Subtotal"]);
                    }

                    int customerId = Convert.ToInt32(cmbCustomer.SelectedValue);

                    // Insert the order record into the orders table
                    string orderQuery = @"INSERT INTO orders 
                        (customer_id, order_date, total) 
                        VALUES (@cid, @date, @total)";
                    SQLiteCommand orderCmd = new SQLiteCommand(orderQuery, conn);
                    orderCmd.Parameters.AddWithValue("@cid", customerId);
                    orderCmd.Parameters.AddWithValue("@date", DateTime.Now.ToString());
                    orderCmd.Parameters.AddWithValue("@total", total);
                    orderCmd.ExecuteNonQuery();

                    // Retrieve the auto-generated ID of the newly inserted order
                    SQLiteCommand lastId = new SQLiteCommand(
                        "SELECT last_insert_rowid()", conn);
                    long orderId = (long)lastId.ExecuteScalar();

                    // Loop through each item in the order and insert into order_items table
                    foreach (DataRow item in orderItemsTable.Rows)
                    {
                        // Insert each order item linked to the order ID
                        string itemQuery = @"INSERT INTO order_items 
                            (order_id, product_id, quantity, price)
                            VALUES (@oid, @pid, @qty, @price)";
                        SQLiteCommand itemCmd = new SQLiteCommand(itemQuery, conn);
                        itemCmd.Parameters.AddWithValue("@oid", orderId);
                        itemCmd.Parameters.AddWithValue("@pid", item["ProductID"]);
                        itemCmd.Parameters.AddWithValue("@qty", item["Quantity"]);
                        itemCmd.Parameters.AddWithValue("@price", item["Price"]);
                        itemCmd.ExecuteNonQuery();

                        // Reduce the stock of each ordered product by the purchased quantity
                        string stockQuery = @"UPDATE products 
                            SET stock = stock - @qty 
                            WHERE id = @pid";
                        SQLiteCommand stockCmd = new SQLiteCommand(stockQuery, conn);
                        stockCmd.Parameters.AddWithValue("@qty", item["Quantity"]);
                        stockCmd.Parameters.AddWithValue("@pid", item["ProductID"]);
                        stockCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Order Placed Successfully!\nTotal: Rs. " + total,
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearOrder();     // Reset the order items grid
                    LoadAllOrders();  // Refresh the orders list
                    LoadProducts();   // Refresh products to reflect updated stock
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Fetches all placed orders from the database and displays them in the orders grid
        private void LoadAllOrders()
        {
            try
            {
                using (var conn = CDBHelper.GetConnection())
                {
                    conn.Open();
                    // JOIN orders with customers to display customer name instead of ID
                    // Results ordered by latest order first
                    string query = @"SELECT o.id, c.name, o.order_date, o.total
                                    FROM orders o
                                    JOIN customers c 
                                    ON o.customer_id = c.id
                                    ORDER BY o.id DESC";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvOrders.DataSource = dt; // Bind results to the orders grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Handles the Clear button click — resets the current order
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearOrder();
        }

        // Clears all items from the order and resets the quantity field
        private void ClearOrder()
        {
            orderItemsTable.Rows.Clear(); // Remove all items from the order grid
            txtQuantity.Clear();          // Reset the quantity input field
        }
    }
}