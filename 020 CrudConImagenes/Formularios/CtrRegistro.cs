using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _020_CrudConImagenes.Modelos;
using _020_CrudConImagenes.Logica;

namespace _020_CrudConImagenes.Formularios
{
    public partial class CtrRegistro : UserControl
    {
        private int id;
        private BdPersona oDbPersona;
        Persona auxPersona;
        private string rutaImagen;

        public CtrRegistro()
        {

            InitializeComponent();
            oDbPersona = new BdPersona();
            Listar();
            id = 0;
            rutaImagen = "";


            dgvDatosPersona.EstilosDgv1();
        }
        #region Helpers

        private void Listar()
        {
            List<Persona> lstPersona = this.oDbPersona.Obtener().ToList();
            dgvDatosPersona.DataSource = lstPersona;
            dgvDatosPersona.Columns["Id"].Visible = false;

            DataGridViewImageColumn d =(DataGridViewImageColumn)dgvDatosPersona.Columns["Foto"];
            d.ImageLayout = DataGridViewImageCellLayout.Zoom;
            
        }
        private bool CamposCorrectos()
        {
            txtNombre.QuitaEspacios();
            txtApematerno.QuitaEspacios();
            txtApePaterno.QuitaEspacios();
            if (!string.IsNullOrEmpty(txtNombre.Text) &&
                !string.IsNullOrEmpty(txtApematerno.Text)
                && !string.IsNullOrEmpty(txtApePaterno.Text))
            {
                return true;
            }

            return false;
        }
        private int CalculaEdad(DateTime tiempo)
        {
            int edad = DateTime.Now.Subtract(tiempo).Days / 365;
            return edad;
        }
        private int? GetId()
        {
            if (dgvDatosPersona.CurrentRow != null)
            {
                string idStr = dgvDatosPersona.Rows[dgvDatosPersona.CurrentRow.Index].Cells[0].Value.ToString();


                return int.Parse(idStr);

            }
            return null;
        }
        private void LimpiaCampos()
        {
            txtNombre.Clear();
            txtApePaterno.Clear();
            txtApematerno.Clear();
            picImgPersona.Image = null;
            this.id = 0;
            this.rutaImagen = "";

        }

        #endregion
        private byte[] SelecionaFoto()
        {
            byte[] imgBytes = null;


            if (rutaImagen != "")
            {
                imgBytes = ImageHelper.ImageABytes(picImgPersona.Image);
            }
            else
            {
                if (auxPersona == null)
                {
                    imgBytes = ImageHelper.bytesImageDefe();
                }
                else
                {
                    //imgBytes = auxPersona.Foto;
                    imgBytes = ImageHelper.ImageABytes(picImgPersona.Image);
                }
            }
            //if (this.auxPersona == null)
            //{

            //    if (rutaImagen == "")
            //    {
            //        imgBytes = ImageHelper.bytesImageDefe();
            //    }
            //    else
            //    {
            //        //imgBytes = ImageHelper.RutaABytes(rutaImagen);
            //    }

            //}
            //else
            //{
            //    if (rutaImagen == "")
            //    {
            //        imgBytes = auxPersona.Foto;
            //    }
            //    else
            //    {
            //        imgBytes = ImageHelper.RutaABytes(rutaImagen);
            //    }

            //}
            return imgBytes;
        }
        private void SelecionaImagen(string ruta = "")
        {

            Image defe = ImageHelper.ImageDefe();
            if (ruta == "")
            {
                picImgPersona.Image = defe;
            }
            try
            {

                //picImgPersona.Image = Image.FromFile(ruta);
                byte[] arrimg = File.ReadAllBytes(ruta);
                using (MemoryStream ms = new MemoryStream(arrimg))
                {
                    Image img = Image.FromStream(ms);
                    picImgPersona.Image = img;

                }
            }
            catch (Exception e)
            {

                picImgPersona.Image = defe;
            }
        }
        private void btnSubir_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            open.Filter = "Images(*.jpg) | *.jpg";


            if (open.ShowDialog() == DialogResult.OK)
            {
                SelecionaImagen(open.FileName);
                this.rutaImagen = open.FileName;

            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (CamposCorrectos())
            {

                Persona per = new Persona();
                per.Nombre = txtNombre.Text;
                per.ApellidoPaterno = txtApePaterno.Text;
                per.ApellidoMaterno = txtApematerno.Text;
                per.FechaNaci = dtpFechaNaci.Value;
                per.Edad = CalculaEdad(per.FechaNaci);
                per.Foto = SelecionaFoto();

                if (id == 0)
                {
                    this.oDbPersona.Agregar(per);
                }
                else
                {
                    per.Id = this.id;
                    this.oDbPersona.Modificar(per);

                }



                LimpiaCampos();
                Listar();

                pnlRegistro.Visible = false;
                dgvDatosPersona.Visible = true;

            }
        }

        private void CargaDatos(int id)
        {

            this.auxPersona = this.oDbPersona.ObtenerPorId(id);
            if (auxPersona == null)
            {
                MessageBox.Show("Este registro ya no existe");
                return;
            }

            this.id = id;
            txtNombre.Text = auxPersona.Nombre;
            txtApePaterno.Text = auxPersona.ApellidoPaterno;
            txtApematerno.Text = auxPersona.ApellidoMaterno;
            dtpFechaNaci.Value = auxPersona.FechaNaci;
            picImgPersona.Image = ImageHelper.ByteAImagen(auxPersona.Foto);

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            int? id = GetId();
            if (id != null)
            {
                CargaDatos((int)id);
                pnlRegistro.Visible = true;
                dgvDatosPersona.Visible = false;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            int? id = GetId();
            if (id != null)
            {
                this.oDbPersona.Eliminar((int)id);
                Listar();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (picImgPersona.Image == null) return;


            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Archivos de imagen(*.jpg)|*.jpg";
            save.Title = "Guarda imagen";


            if (save.ShowDialog() == DialogResult.OK)
            {


                try
                {


                    using (Bitmap bm = new Bitmap(picImgPersona.Image))
                    {
                        using (FileStream fs = new FileStream(save.FileName, FileMode.Create, FileAccess.ReadWrite))
                        {

                            bm.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //picImgPersona.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }

                    }
                    MessageBox.Show($"Se ha guardado correctamenta la imagen en {save.FileName}");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }



        }

        private void dgvDatosPersona_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            //MessageBox.Show($"Fila {e.RowIndex} Columna {e.ColumnIndex}");
            //MessageBox.Show(dgvDatosPersona.CurrentRow == null ? "es null" : "No es null");


        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiaCampos();
            pnlRegistro.Visible = false;
            dgvDatosPersona.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnlRegistro.Visible = true;
            dgvDatosPersona.Visible = false;
        }
    }
}
