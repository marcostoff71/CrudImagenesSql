using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace _020_CrudConImagenes.Logica
{
    public static class ImageHelper
    {
        public static byte[] RutaABytes(string ruta)
        {
            byte[] arrImg = null;
            try
            {
                byte[] bytes=File.ReadAllBytes(ruta);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    arrImg = ms.ToArray();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Imagen no compatible se seleciona una imagen por defecto");
                MessageBox.Show(e.Message);
                using (MemoryStream ms = new MemoryStream())
                {

                    global::_020_CrudConImagenes.Properties.Resources.usuarioDefe.Save(ms, global::_020_CrudConImagenes.Properties.Resources.usuarioDefe.RawFormat);
                    arrImg = ms.ToArray();
                }
                
            }

            return arrImg;
        }
        public static byte[] ImageABytes(Image img)
        {
            byte[] arrImg = null;
            try
            {
                
                //MemoryStream ms = new MemoryStream();
                
                //img.Save(ms, img.RawFormat);
                //arrImg = ms.ToArray();
                //ms.Close();




                //sirve pero todo se queda en formato jpg

                using(Bitmap bmp= new Bitmap(img))
                {
                    using(MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Jpeg);

                        arrImg=ms.ToArray();
                    }
                }


                
            }
            catch (Exception e)
            {
                MessageBox.Show("Imagen no compatible se seleciona una imagen por defecto");
                MessageBox.Show(e.Message);
                using (MemoryStream ms = new MemoryStream())
                {

                    global::_020_CrudConImagenes.Properties.Resources.usuarioDefe.Save(ms, global::_020_CrudConImagenes.Properties.Resources.usuarioDefe.RawFormat);
                    arrImg = ms.ToArray();
                }

            }

            return arrImg;
        }
        public static Image ByteAImagen(byte[] arr)
        {
            Image img = null;
            using(MemoryStream ms = new MemoryStream(arr))
            {
                img=Image.FromStream(ms);
            }

            return img;
        }


        /// <summary>
        /// Imagen por defecto
        /// </summary>
        /// <returns>Image </returns>
        
        public static Image ImageDefe()
        {
            Bitmap bmp = new Bitmap(500, 500);

            using(Graphics g = Graphics.FromImage(bmp))
            {

                Rectangle rec= new Rectangle(0,0,bmp.Width,bmp.Height);


                Font fon = new Font("Arial", 33, FontStyle.Regular, GraphicsUnit.Point);

                Brush brus = new SolidBrush(Color.Black);

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment= StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                g.FillRectangle(Brushes.White, rec);
                g.DrawString("Sin imagen", fon, brus, rec,stringFormat);

            }

            return bmp;
        }

        public static byte[] bytesImageDefe()
        {
            using (MemoryStream ms = new MemoryStream())
            {

                ImageDefe().Save(ms, ImageFormat.Jpeg);


                return ms.ToArray();
            }
        }
        

    }
}
