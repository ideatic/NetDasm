namespace System.Windows.FutureStyle.Office2007
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;

    /// <summary>
    /// A class that can act as a container for the main controls on the main window of an application.
    /// </summary>
    public class OfficeBackPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the ClientPanel class.
        /// </summary>
        public OfficeBackPanel()
        {
            this.dibujarSombras = false;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Transparent;
            dibujarGlossy = true;
            this.Style = OfficeBackPanelStyles.Luna;
        }

        /// <summary>
        /// Overridden. See the documentation on the base class for help on this member.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (this.DrawShadows)
            {
                base.Invalidate();
            }
        }

        /// <summary>
        /// Overridden. See the documentation on the base class for help on this member.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.DrawShadows)
            {
                foreach (Control control in base.Controls)
                {

                    if (control.Visible)
                    {
                        Rectangle rectangle1 = control.Bounds;
                        rectangle1.Inflate(-1, -1);
                        rectangle1.Offset(1, 1);
                        DrawDropShadow(e.Graphics, rectangle1);
                    }
                }
            }
        }



        /// <summary>
        /// Overridden. See the documentation on the base class for help on this member.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                if ((base.ClientRectangle.Width > 0) && (base.ClientRectangle.Height > 0))
                {
                    if (this.renderer != null)
                    {
                        this.renderer.DrawClientPanelBackground(e.Graphics, base.ClientRectangle);

                        if (dibujarGlossy)
                            e.Graphics.DrawImage(global::NetDasm.Properties.Resources.BackgroundGlossy, 0, 0);
                    }
                    else
                    {
                        base.OnPaintBackground(e);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Overridden. See the documentation on the base class for help on this member.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == 0x115) || (m.Msg == 0x114))
            {
                base.Invalidate();
            }
            base.WndProc(ref m);
        }


        /// <summary>
        /// Indicates whether shadows are drawn around child controls.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether shadows are drawn around child controls."), Category("Appearance")]
        public bool DrawShadows
        {
            get
            {
                return this.dibujarSombras;
            }
            set
            {
                this.dibujarSombras = value;
                base.Invalidate();
            }
        }

        /// <summary>
        /// Indicates whether the glossy effect is draw
        /// </summary>
        [DefaultValue(true), Description("Indicates whether the glossy effect is draw."), Category("Appearance")]
        public bool DrawGlossy
        {
            get
            {
                return this.dibujarGlossy;
            }
            set
            {
                this.dibujarGlossy = value;
                base.Invalidate();
            }
        }
        private bool dibujarGlossy;

        private bool dibujarSombras;


        [DefaultValue(10), Description("Indicates the shadow size."), Category("Appearance")]
        public int ShadowSize
        {
            get { return tamañoSombra; }
            set { tamañoSombra = value; this.Refresh(); }
        }
        private int tamañoSombra;

        [DefaultValue(25), Description("Indicates the shadow opacity."), Category("Appearance")]
        public byte ShadowOpacity
        {
            get { return opacidadSombra; }
            set { opacidadSombra = value; this.Refresh(); }
        }
        private byte opacidadSombra;

        private Style renderer;
        private OfficeBackPanelStyles currentStyle;
        public OfficeBackPanelStyles Style
        {
            get { return currentStyle; }
            set
            {
                currentStyle = value;
                switch (currentStyle)
                {
                    case OfficeBackPanelStyles.Vista:
                        renderer = new Vista();
                        break;
                    case OfficeBackPanelStyles.Transparent:
                        renderer = new Transparent();
                        break;
                    default:
                        renderer = new Luna();
                        break;
                }

                this.Refresh();
            }
        }

        #region Dibujar sombras
        public void DrawDropShadow(Graphics graphics, Rectangle bounds)
        {
            Color shadowColor = Color.FromArgb(this.ShadowOpacity, Color.Black);
            int size = this.ShadowSize;

            if ((bounds.Width > 0) && (bounds.Height > 0))
            {
                using (Bitmap bitmap1 = FuncionBitmap(size, shadowColor))
                {
                    graphics.DrawImage((Image)bitmap1, bounds.X - size, bounds.Y - size);
                    bitmap1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    graphics.DrawImage(bitmap1, bounds.Right, bounds.Y - size);
                    bitmap1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    graphics.DrawImage(bitmap1, bounds.Right, bounds.Bottom);
                    bitmap1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    graphics.DrawImage(bitmap1, bounds.X - size, bounds.Bottom);
                }
                Rectangle rectangle1 = new Rectangle(bounds.X, bounds.Y - size, bounds.Width, size);
                using (LinearGradientBrush brush1 = new LinearGradientBrush(rectangle1, Color.Transparent, shadowColor, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush1, rectangle1);
                }
                rectangle1.Offset(0, bounds.Height + size);
                using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle1, shadowColor, Color.Transparent, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush2, rectangle1);
                }
                Rectangle rectangle2 = new Rectangle(bounds.X - size, bounds.Y, size, bounds.Height);
                using (LinearGradientBrush brush3 = new LinearGradientBrush(rectangle2, Color.Transparent, shadowColor, LinearGradientMode.Horizontal))
                {
                    graphics.FillRectangle(brush3, rectangle2);
                }
                rectangle2.Offset(bounds.Width + size, 0);
                using (LinearGradientBrush brush4 = new LinearGradientBrush(rectangle2, shadowColor, Color.Transparent, LinearGradientMode.Horizontal))
                {
                    graphics.FillRectangle(brush4, rectangle2);
                }

                graphics.FillRectangle(new SolidBrush(shadowColor), bounds);
            }
        }

        private Bitmap FuncionBitmap(int tamaño, Color color)
        {
            Bitmap bitmap1 = new Bitmap(tamaño, tamaño);
            using (Graphics graphics1 = Graphics.FromImage(bitmap1))
            {
                using (GraphicsPath path1 = new GraphicsPath())
                {
                    path1.AddEllipse(new Rectangle(0, 0, tamaño * 2, tamaño * 2));
                    using (PathGradientBrush brush1 = new PathGradientBrush(path1))
                    {
                        brush1.CenterColor = color;
                        brush1.SurroundColors = new Color[] { Color.Transparent };
                        graphics1.FillRectangle(brush1, new Rectangle(0, 0, tamaño, tamaño));
                    }
                    return bitmap1;
                }
            }
        }


        #endregion
    }

    public enum OfficeBackPanelStyles
    {
        Luna, Vista, Transparent
    }

    public abstract class Style
    {
        public abstract void DrawClientPanelBackground(Graphics graphics, Rectangle bounds);
    }

    class Luna : Style
    {
        public override void DrawClientPanelBackground(Graphics graphics, Rectangle bounds)
        {
            using (LinearGradientBrush brush1 = new LinearGradientBrush(bounds, Color.Black, Color.White, LinearGradientMode.Vertical))
            {
                ColorBlend blend1 = new ColorBlend(3);
                blend1.Positions[0] = 0f;
                blend1.Colors[0] = Color.FromArgb(0xa3, 0xc2, 0xea);
                blend1.Positions[1] = 0.75f;
                blend1.Colors[1] = Color.FromArgb(0x56, 0x7d, 0xb0);
                blend1.Positions[2] = 1f;
                blend1.Colors[2] = Color.FromArgb(0x65, 0x91, 0xcd);
                brush1.InterpolationColors = blend1;
                graphics.FillRectangle(brush1, bounds);
            }
        }
    }

    class Transparent : Style
    {
        public override void DrawClientPanelBackground(Graphics graphics, Rectangle bounds)
        {
            graphics.FillRectangle(new LinearGradientBrush(bounds, Color.FromArgb(252, 252, 254), Color.FromArgb(244, 243, 238), LinearGradientMode.Vertical), bounds);

        }
    }

    class Vista : Style
    {
        public override void DrawClientPanelBackground(Graphics graphics, Rectangle bounds)
        {
            using (LinearGradientBrush brush1 = new LinearGradientBrush(bounds, Color.Black, Color.White, LinearGradientMode.Vertical))
            {
                ColorBlend blend1 = new ColorBlend(3);
                blend1.Positions[0] = 0f;
                blend1.Colors[0] = Color.FromArgb(0x4f, 0x4f, 0x4f);
                blend1.Positions[1] = 0.75f;
                blend1.Colors[1] = Color.FromArgb(0x37, 0x37, 0x37);
                blend1.Positions[2] = 1f;
                blend1.Colors[2] = Color.FromArgb(10, 10, 10);
                brush1.InterpolationColors = blend1;
                graphics.FillRectangle(brush1, bounds);
            }
        }
    }

}
