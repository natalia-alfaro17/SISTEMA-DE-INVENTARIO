
using System;
using System.Windows.Forms;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaInventario.Datos;

namespace SistemaInventario1
{
    public partial class FrmInventario : Form
    {
        private ConexionDB conexion = new ConexionDB();

        public FrmInventario()
        {
            InitializeComponent();

            btnGuardar.Click += btnGuardar_Click;
            btnActualizar.Click += btnActualizar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            dgvProductos.CellClick += dgvProductos_CellClick;

            CargarProductos();
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text == "" || txtPrecio.Text == "" || txtStock.Text == "")
            {
                MessageBox.Show("Completa los campos obligatorios.");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) ||
                !int.TryParse(txtStock.Text, out int stock))
            {
                MessageBox.Show("Verifica que el precio y el stock sean números válidos.");
                return;
            }

            try
            {
                using (SqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"INSERT INTO Productos
                    (Nombre, Descripcion, Categoria, Precio, Stock)
                    VALUES
                    (@Nombre, @Descripcion, @Categoria, @Precio, @Stock)";

                    using (SqlCommand cmd = new SqlCommand(consulta, cn))
                    {
                        cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = txtNombre.Text;
                        cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 200).Value = txtDescripcion.Text;
                        cmd.Parameters.Add("@Categoria", SqlDbType.VarChar, 50).Value = txtCategoria.Text;
                        cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = precio;
                        cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = stock;

                        cmd.Parameters["@Precio"].Precision = 10;
                        cmd.Parameters["@Precio"].Scale = 2;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto guardado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el producto: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto de la tabla.");
                return;
            }

            if (txtNombre.Text == "" || txtPrecio.Text == "" || txtStock.Text == "")
            {
                MessageBox.Show("Completa los campos obligatorios.");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) ||
                !int.TryParse(txtStock.Text, out int stock))
            {
                MessageBox.Show("Verifica el precio y el stock.");
                return;
            }

            try
            {
                int id = Convert.ToInt32(dgvProductos.CurrentRow.Cells["Id"].Value);

                using (SqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"UPDATE Productos
                    SET Nombre = @Nombre,
                        Descripcion = @Descripcion,
                        Categoria = @Categoria,
                        Precio = @Precio,
                        Stock = @Stock
                    WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(consulta, cn))
                    {
                        cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = txtNombre.Text;
                        cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 200).Value = txtDescripcion.Text;
                        cmd.Parameters.Add("@Categoria", SqlDbType.VarChar, 50).Value = txtCategoria.Text;
                        cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = precio;
                        cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = stock;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                        cmd.Parameters["@Precio"].Precision = 10;
                        cmd.Parameters["@Precio"].Scale = 2;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto actualizado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Estás segura de que deseas eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.No)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(dgvProductos.CurrentRow.Cells["Id"].Value);

                using (SqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = "DELETE FROM Productos WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(consulta, cn))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto eliminado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            dgvProductos.ClearSelection();
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

                txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
                txtDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString();
                txtCategoria.Text = fila.Cells["Categoria"].Value?.ToString();
                txtPrecio.Text = fila.Cells["Precio"].Value?.ToString();
                txtStock.Text = fila.Cells["Stock"].Value?.ToString();
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
        }

        private void CargarProductos()
        {
            try
            {
                using (SqlConnection cn = conexion.ObtenerConexion())
                {
                    cn.Open();

                    string consulta = "SELECT * FROM Productos";

                    SqlDataAdapter adaptador = new SqlDataAdapter(consulta, cn);
                    DataTable tabla = new DataTable();

                    adaptador.Fill(tabla);

                    dgvProductos.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void FrmInventario_Load(object sender, EventArgs e)
        {

        }

        private void btnLimpiar_Click_1(object sender, EventArgs e)
        {

        }

        private void pnlEncabezado_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

