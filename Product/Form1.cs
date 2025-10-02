using Newtonsoft.Json;
using System.ComponentModel;

namespace WinFormsTestv2
{
    public partial class Form1 : Form
    {
        private readonly string jsonFilePath = "Data\\products.json";
        private List<Product> _allProducts = new List<Product>();

        private string _currentSortColumn = "id"; // 預設以 id 排序
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;

        public Form1()
        {
            InitializeComponent();
            setupDataGridView();
            setupCmbFilter();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            Rootobject rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonData);
            _allProducts = rootObject.products.ToList();
            dataGridViewJson.DataSource = _allProducts;

            //setupDataGridView();
        }

        private void setupDataGridView()
        {
            dataGridViewJson.ReadOnly = true;

            dataGridViewJson.AutoGenerateColumns = false;
            dataGridViewJson.Columns.Clear();
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                HeaderText = "ID"
            });
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "title",
                HeaderText = "Title"
            });
            //dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            //{
            //    DataPropertyName = "description",
            //    HeaderText = "Description"
            //});
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "category",
                HeaderText = "Category"
            });
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "price",
                HeaderText = "Price"
            });
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "rating",
                HeaderText = "Rating"
            });
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "stock",
                HeaderText = "Stock",
                Name = "stock"
            });
            dataGridViewJson.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "brand",
                HeaderText = "Brand"
            });

            dataGridViewJson.Columns["stock"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridViewJson.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void dataGridViewJson_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dataGridViewJson.Columns[e.ColumnIndex];
            string column_name = column.DataPropertyName;
            List<Product> sortedProducts;

            if (column.SortMode == DataGridViewColumnSortMode.NotSortable)
            {
                return;
            }

            if (_currentSortColumn == column_name && _currentSortDirection == ListSortDirection.Ascending)
            {
                sortedProducts = _allProducts.OrderByDescending(_ => _.GetType().GetProperty(column_name).GetValue(_)).ToList();
                _currentSortDirection = ListSortDirection.Descending;
            }
            else
            {
                _currentSortColumn = column_name;
                _currentSortDirection = ListSortDirection.Ascending;
                sortedProducts = _allProducts.OrderBy(_ => _.GetType().GetProperty(column_name).GetValue(_)).ToList();
            }

            dataGridViewJson.DataSource = sortedProducts;

        }

        private void ApplySort()
        {
            if (_allProducts == null || _allProducts.Count == 0)
            {
                return;
            }

            IEnumerable<Product> productsToShow = _allProducts;
            string upperKeyword = txtFilter.Text.ToUpper();
            string selectedColumn = cmbFilter.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedColumn) && !string.IsNullOrEmpty(upperKeyword))
            {
                switch (selectedColumn)
                {
                    case "title":
                        productsToShow = _allProducts.Where(p =>
                            p.title != null && p.title.ToUpper().Contains(upperKeyword)
                        );
                        break;
                    case "category":
                        productsToShow = _allProducts.Where(p =>
                            p.category != null && p.category.ToUpper().Contains(upperKeyword)
                        );
                        break;
                    case "brand":
                        productsToShow = _allProducts.Where(p =>
                            p.brand != null && p.brand.ToUpper().Contains(upperKeyword)
                        );
                        break;
                    case "All Fields":
                    default:
                        productsToShow = _allProducts.Where(p =>
                            (p.title != null && p.title.ToUpper().Contains(upperKeyword)) ||
                            (p.category != null && p.category.ToUpper().Contains(upperKeyword)) ||
                            (p.brand != null && p.brand.ToUpper().Contains(upperKeyword))
                        );
                        break;
                }
            }

            List<Product> sortedProducts;

            switch (_currentSortColumn)
            {
                case "id":
                    sortedProducts = (_currentSortDirection == ListSortDirection.Ascending)
                        ? productsToShow.OrderBy(p => p.id).ToList()
                        : productsToShow.OrderByDescending(p => p.id).ToList();
                    break;
                case "title":
                    sortedProducts = (_currentSortDirection == ListSortDirection.Ascending)
                        ? productsToShow.OrderBy(p => p.title).ToList()
                        : productsToShow.OrderByDescending(p => p.title).ToList();
                    break;
                case "category":
                    sortedProducts = (_currentSortDirection == ListSortDirection.Ascending)
                        ? productsToShow.OrderBy(p => p.category).ToList()
                        : productsToShow.OrderByDescending(p => p.category).ToList();
                    break;
                case "price":
                    sortedProducts = (_currentSortDirection == ListSortDirection.Ascending)
                        ? productsToShow.OrderBy(p => p.price).ToList()
                        : productsToShow.OrderByDescending(p => p.price).ToList();
                    break;
                default:
                    sortedProducts = productsToShow.ToList();
                    break;
            }

            dataGridViewJson.DataSource = sortedProducts;

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            Product new_product = new Product { id = _allProducts.Count > 0 ? _allProducts.Max(p => p.id) + 1 : 1 };
            using (var editorForm = new ProductEditorForm(new_product))
            {
                if (editorForm.ShowDialog() == DialogResult.OK)
                {
                    _allProducts.Add(new_product);
                    ApplySort();
                }

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewJson.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select data.");
                return;
            }
            Product selectedProduct = dataGridViewJson.SelectedRows[0].DataBoundItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Fail to access data.");
                return;
            }
            using (var editForm = new ProductEditorForm(selectedProduct))
            {
                if (DialogResult.OK == editForm.ShowDialog())
                {
                    ApplySort();
                    MessageBox.Show("Edit successfully.");
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewJson.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select data to delete.");
                return;
            }

            Product product_delete = dataGridViewJson.SelectedRows[0].DataBoundItem as Product;
            var confirm = MessageBox.Show($"Are you sure to delete product '{product_delete.title}'", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                _allProducts.Remove(product_delete);
                ApplySort();
                MessageBox.Show("Delete Successful!");
            }
        }

        private void setupCmbFilter()
        {
            var options = new List<string>
            {
                "All Fields",
                "title",
                "category",
                "brand"
            };

            cmbFilter.DataSource = options;
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySort();
        }
    }
}
