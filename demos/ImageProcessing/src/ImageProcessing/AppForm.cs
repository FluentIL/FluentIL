using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ImageProcessing.Util;

namespace ImageProcessing
{
    public partial class AppForm : Form
    {
        protected string ProgramNameField;
        protected string FileNameField;
        protected Bitmap ImageField;


        public AppForm()
        {
            Text = ProgramNameField = "Image Processing with Emitting";
            ResizeRedraw = true;

            Menu = new MainMenu();
            Menu.MenuItems.Add("&File");
            Menu.MenuItems[0].MenuItems.Add(new MenuItem("&Open...",
                                     new EventHandler(MenuFileOpenOnClick),
                                     Shortcut.CtrlO));


            Menu.MenuItems.Add("Filter&CS");


            MenuItem m = new MenuItem(
                "&Negative",
                new EventHandler(MenuFilterCSOnClick)
                );
            m.Tag = Filters.Negative;
            Menu.MenuItems[1].MenuItems.Add(m);

            m = new MenuItem(
                "&Blur",
                new EventHandler(MenuFilterCSOnClick)
                );
            m.Tag = Filters.Blur;
            Menu.MenuItems[1].MenuItems.Add(m);

            m = new MenuItem(
                "&Edge",
                new EventHandler(MenuFilterCSOnClick)
                );
            m.Tag = Filters.Edge;
            Menu.MenuItems[1].MenuItems.Add(m);

            m = new MenuItem(
                "&Sharpen",
                new EventHandler(MenuFilterCSOnClick)
                );
            m.Tag = Filters.Sharpen;
            Menu.MenuItems[1].MenuItems.Add(m);


            Menu.MenuItems.Add("Filter&IL");
            m = new MenuItem(
                "&Negative",
                new EventHandler(MenuFilterILOnClick)
                );
            m.Tag = Filters.Negative;
            Menu.MenuItems[2].MenuItems.Add(m);

            m = new MenuItem(
                "&Blur",
                new EventHandler(MenuFilterILOnClick)
                );
            m.Tag = Filters.Blur;
            Menu.MenuItems[2].MenuItems.Add(m);

            m = new MenuItem(
                "&Edge",
                new EventHandler(MenuFilterILOnClick)
                );
            m.Tag = Filters.Edge;
            Menu.MenuItems[2].MenuItems.Add(m);

            m = new MenuItem(
                "&Sharpen",
                new EventHandler(MenuFilterILOnClick)
                );
            m.Tag = Filters.Sharpen;
            Menu.MenuItems[2].MenuItems.Add(m);
        }


        void MenuFilterCSOnClick(object sender, EventArgs ea)
        {
            Cursor cursorSave = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            var before = DateTime.Now;
            var item = (MenuItem) sender;
            new CsApplier().Apply((Filter)item.Tag, this.ImageField);
            MessageBox.Show("Total time = " + (DateTime.Now - before).TotalMilliseconds + " milliseconds");
            this.Invalidate();

            Cursor.Current = cursorSave;
        }

        void MenuFilterILOnClick(object sender, EventArgs ea)
        {
            Cursor cursorSave = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            var before = DateTime.Now;
            var item = (MenuItem)sender;
            new IlApplier().Apply((Filter)item.Tag, this.ImageField);
            MessageBox.Show("Total time = " + (DateTime.Now - before).TotalMilliseconds + " milliseconds");
            this.Invalidate();

            Cursor.Current = cursorSave;   
        }

        
        void MenuFileOpenOnClick(object sender, EventArgs ea)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;" +
                                "*.jfif;*.png;*.tif;*.tiff;*.wmf;*.emf";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ImageField = (Bitmap)Bitmap.FromFile(dlg.FileName);
                    if (!ImageField.IsCompatible())
                    {
                        ImageField.Dispose();
                        MessageBox.Show("Could not process. Please select a 24bit or 32bit image file", ProgramNameField);
                        return;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, ProgramNameField);
                    return;
                }
                FileNameField = dlg.FileName;
                Text = ProgramNameField + " - " + Path.GetFileName(FileNameField);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            if (ImageField != null)
                DrawImage(pea.Graphics, ImageField, ClientRectangle);
        }

        void DrawImage(Graphics grfx, Image image,
                                      RectangleF rectf)
        {
            SizeF sizef = new SizeF(image.Width / image.HorizontalResolution,
                                    image.Height / image.VerticalResolution);

            float fScale = Math.Min(rectf.Width / sizef.Width,
                                    rectf.Height / sizef.Height);

            sizef.Width *= fScale;
            sizef.Height *= fScale;

            grfx.DrawImage(image, rectf.X + (rectf.Width - sizef.Width) / 2,
                                  rectf.Y + (rectf.Height - sizef.Height) / 2,
                                  sizef.Width, sizef.Height);
        }
    }
}