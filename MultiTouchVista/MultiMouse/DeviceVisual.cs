using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace MultiMouse
{

class DeviceVisual : System.Windows.Window
{
    
    public DeviceVisual(string deviceId, int x, int y)
    {
        //Me.deviceId = deviceId
        base.Left = x;
        base.Top = y;
        this.Visible = true;
        this.InitializeMouse();
        //If DeviceVisual.IsHighContrast Then
        //    Dim color As Color = color.FromArgb(&HFF, &HFF, &HFF, 0)
        //    Me.CursorColor = color
        //End If
    }
    
    internal void ChangeCursorColor(System.Drawing.Color cursorColor)
    {
        System.Drawing.Bitmap _bitmap;
        
        _bitmap = new System.Drawing.Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_bitmap);
        System.Windows.Forms.Control c = new System.Windows.Forms.Control();
        c.Cursor.Draw(g, new System.Drawing.Rectangle(0, 0, 20, 20));
        
        //_bitmap.MakeTransparent(System.Drawing.Color.Magenta)
        //Dim color As System.Drawing.Color = color.FromArgb(cursorColor.A, cursorColor.R, cursorColor.G, cursorColor.B)
        //Dim i As Integer
        //For i = 0 To (_bitmap.Width - 1) - 1
        //    Dim j As Integer
        //    For j = 0 To (_bitmap.Height - 1) - 1
        //        Dim pixel As System.Drawing.Color = _bitmap.GetPixel(i, j)
        //        If (((pixel.A = color.Black.A) AndAlso (pixel.R = color.Black.R)) AndAlso ((pixel.B = color.Black.B) AndAlso (pixel.G = color.Black.G))) Then
        //            _bitmap.SetPixel(i, j, color)
        //        End If
        //    Next j
        //Next i
        
        //_bitmap = Colorize(_bitmap, cursorColor);
        //System.Drawing.Color.FromArgb(255, 0, 255, 0))
        
        MemoryStream stream = new MemoryStream();
        _bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        stream.Position = 0;
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = stream;
        image.EndInit();
        Image image2 = new Image();
        image2.Source = image;
        base.Content = image2;
    }
    
    //public static System.Drawing.Bitmap Colorize(System.Drawing.Image source, System.Drawing.Color color)
    //{
		//float fHue = color.GetHue();
		//float fSat = color.GetSaturation();
		//DIBitmap32 dibTmp = new DIBitmap32(source);
		////' The line above is the same as the line following...
		////Dim dibTmp As DIBitmap32 = CType(source, DIBitmap32)
		//{
		//    for (int iX = 0; iX <= dibTmp.Width - 1; iX++) {
		//        for (int iY = 0; iY <= dibTmp.Height - 1; iY++) {
		//            dibTmp.Pixel(iX, iY) = Pixel32.FromHSB(fHue, fSat, dibTmp.Pixel(iX, iY).GetBrightness(), dibTmp.Pixel(iX, iY).Alpha);
		//        }
		//    }
		//}
		//return dibTmp;
		//// <- widening conversion performed
    //}
    
    internal void ChangeCursorImage(BitmapImage cursorImage)
    {
        Image image = new Image();
        image.Source = cursorImage;
        base.Content = image;
        if (this.Visible) {
            base.Show();
        }
    }
    
    private void InitializeMouse()
    {
        _DefaultImageUsed = true;
        MemoryStream stream = new MemoryStream();
        System.Drawing.Bitmap _bitmap;
        
        _bitmap = new System.Drawing.Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_bitmap);
        System.Windows.Forms.Control c = new System.Windows.Forms.Control();
        c.Cursor.Draw(g, new System.Drawing.Rectangle(0, 0, 20, 20));
        
        //_bitmap.MakeTransparent(System.Drawing.Color.Magenta)
        _bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        stream.Position = 0;
        this.defaultImage = new BitmapImage();
        this.defaultImage.BeginInit();
        this.defaultImage.StreamSource = stream;
        this.defaultImage.EndInit();
        Image image = new Image();
        image.Source = this.defaultImage;
        base.Content = image;
        base.BorderBrush = Brushes.Transparent;
        base.Topmost = true;
        base.ShowInTaskbar = false;
        base.Visibility = Visibility.Visible;
        base.MaxHeight = this.defaultImage.Height;
        base.MaxWidth = this.defaultImage.Width;
        base.AllowsTransparency = true;
        base.Background = Brushes.Transparent;
        base.RenderSize = new Size(this.defaultImage.Width, this.defaultImage.Height);
        base.WindowStyle = WindowStyle.None;
        base.IsEnabled = true;
        base.Show();
    }
    
    internal BitmapImage defaultImage;
    
    //Private deviceId As String
    
    //Private _CursorColor As Color = Colors.Black
    //Friend Property CursorColor() As Color
    //    Get
    //        If (_CursorImage Is Nothing) Then
    //            Return _CursorColor
    //        End If
    //        Return Colors.Transparent
    //    End Get
    //    Set(ByVal value As Color)
    //        _CursorColor = value
    //        _DefaultImageUsed = True
    //        Me.ChangeCursorColor(_CursorColor)
    //    End Set
    //End Property
    
    private BitmapImage _CursorImage;
    internal BitmapImage CursorImage {
        get {
            if (((_CursorImage != null))) {
                return _CursorImage;
            }
            return this.defaultImage;
        }
        set {
            _CursorImage = value;
            _DefaultImageUsed = false;
            this.ChangeCursorImage(_CursorImage);
            //Me.CursorColor = Colors.Transparent
        }
    }
    
    private bool _DefaultImageUsed;
    internal bool DefaultImageUsed {
        get { return _DefaultImageUsed; }
    }
    
    //Friend ReadOnly Property DeviceID() As String
    //    Get
    //        Return Me.deviceId
    //    End Get
    //End Property
    
    private bool _Visible;
    internal bool Visible {
        get { return _Visible; }
        set {
            _Visible = value;
            if (_Visible)
                this.Visibility = Visibility.Visible;
            else
                this.Visibility = Visibility.Hidden;
        }
    }
    
}

//#region " Colorize "

//// DIBitmap32 (Class): 32-Bit ARGB device-independant bitmap.
//// Pixel32 (Structure): 32-Bit ARGB pixel.
//// Modified: 2/12/2006
//// by M_i_c_h_a_e_l__A__S_e_e_l
//// m_s_e_e_l_1_9_7_6_[at]_g_m_a_i_l_[dot]_c_o_m
//// (Name and email obscured for anti search/scan purposes.)

///// <summary>
///// Represents a 32-bit ARGB device-independant bitmap.
///// </summary>
//public class DIBitmap32
//{
//    private Pixel32[,] _pxPixels;
//    private Size _szSize;
//    [DllImport("kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
//    private static extern void CopyMemoryPtr(IntPtr toAddress, ref Pixel32 fromPixel32, int bytesToCopy);
//    [DllImport("kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
//    private static extern void CopyMemoryPtr(ref Pixel32 toPixel32, IntPtr fromAddress, int bytesToCopy);
    
//    // Copies one or more Pixel32 structures to the specified memory address.
//    // Copies one or more Pixel32 structures from the specified memory address.
    
//    public DIBitmap32(int width, int height)
//    {
//         // ERROR: Not supported in C#: ReDimStatement

//        _szSize = new Size(width, height);
//    }
    
//    public DIBitmap32(Size size)
//    {
//         // ERROR: Not supported in C#: ReDimStatement

//        _szSize = size;
//    }
    
//    public DIBitmap32(DIBitmap32 original)
//    {
//        {
//            _pxPixels = original._pxPixels;
//            _szSize = original._szSize;
//        }
//    }
    
//    public DIBitmap32(System.Drawing.Image original) : this((DIBitmap32)original)
//    {
//    }
    
//    public DIBitmap32 Clone()
//    {
//        return new DIBitmap32(this);
//    }
    
//    public int Width {
//        get { return (int)_szSize.Width; }
//    }
    
//    public int Height {
//        get { return (int)_szSize.Height; }
//    }
    
//    public Size Size {
//        get { return _szSize; }
//    }
    
//    public Pixel32 Pixel {
//        get { return _pxPixels(y, x); }
//        set { _pxPixels(y, x) = value; }
//    }
    
//    // Provides a method for CType() conversion or implicit conversion of a DIBitmap32 to a System.Drawing.Bitmap.
//    public static implicit operator System.Drawing.Bitmap(DIBitmap32 dib)
//    {
//        if (dib == null) {
//            return null;
//        }
//        else {
//            {
//                // Create an empty bitmap to recieve the bit information.
//                System.Drawing.Bitmap bmpTmp = new System.Drawing.Bitmap(dib.Width, dib.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//                // Create a BitmapData instance for working with the bitmap's bits.
//                System.Drawing.Imaging.BitmapData datTmp = bmpTmp.LockBits(new System.Drawing.Rectangle(0, 0, dib.Width, dib.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//                // Copy the bit data to the memory address of the bitmap's bits.
//                CopyMemoryPtr(datTmp.Scan0, dib._pxPixels(0, 0), dib.Width * dib.Height * 4);
//                // Release the lock on the bits.
//                bmpTmp.UnlockBits(datTmp);
//                // Return the new bitmap.
//                return bmpTmp;
//            }
//        }
//    }
    
//    // Provides a method for CType() conversion of a System.Drawing.Image to a DIBitmap32. Narrows because all images are converted to 32-bit ARGB.
//    public static explicit operator DIBitmap32(System.Drawing.Image img)
//    {
//        if (img == null) {
//            return null;
//        }
//        else {
//            // Create a temporary bitmap clone of the provided image.
//            System.Drawing.Bitmap bmpTmp = new System.Drawing.Bitmap(img);
//            {
//                // Create a BitmapData instance for working with the bitmap bits.
//                System.Drawing.Imaging.BitmapData datTmp = bmpTmp.LockBits(new System.Drawing.Rectangle(0, 0, bmpTmp.Width, bmpTmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//                // Create an empty DIBitmap32 to receive the bit information.
//                DIBitmap32 dibTmp = new DIBitmap32(bmpTmp.Width, bmpTmp.Height);
//                // Copy the bit data from the memory address of the bitmap's bits.
//                CopyMemoryPtr(dibTmp._pxPixels(0, 0), datTmp.Scan0, bmpTmp.Width * bmpTmp.Height * 4);
//                // Release the lock on the bits.
//                bmpTmp.UnlockBits(datTmp);
//                // Dispose of the temporary bitmap.
//                bmpTmp.Dispose();
//                // Return the new DIBitmap32.
//                return dibTmp;
//            }
//        }
//    }
//}


///// <summary>
///// Represents a 32-bit ARGB pixel.
///// </summary>
//public struct Pixel32
//{
//    // The following 4 variables must be declared in the order Blue, Green, Red, Alpha.
//    public byte Blue;
//    public byte Green;
//    public byte Red;
//    public byte Alpha;
    
//    public Pixel32(byte red, byte green, byte blue,  // ERROR: Unsupported modifier : In, Optional
//byte alpha)
//    {
//        {
//            this.Alpha = alpha;
//            this.Red = red;
//            this.Green = green;
//            this.Blue = blue;
//        }
//    }
    
//    // Uses 0 to 360 (degree) hue, 0.0 to 1.0 saturation, 0.0 to 1.0 brightness.
//    public static Pixel32 FromHSB(float hue, float saturation, float brightness,  // ERROR: Unsupported modifier : In, Optional
//byte alpha)
//    {
//        if (saturation == 0) {
//            byte bTmp = (byte)brightness * 255;
//            return new Pixel32(bTmp, bTmp, bTmp);
//        }
//        else {
//            hue /= 360;
//            float fA;
//            if (brightness > 0.5) {
//                fA = brightness + saturation - (brightness * saturation);
//            }
//            else {
//                fA = brightness * (1 + saturation);
//            }
//            float fB = (2 * brightness) - fA;
//            float[] fC = {(float)hue + (1 / 3), hue, (float)hue - (1 / 3)};
//            float[] fClr = new float[3];
//            for (int i = 0; i <= 2; i++) {
//                if (fC(i) < 0)
//                    fC(i) += 1;
//                if (fC(i) > 1)
//                    fC(i) -= 1;
//                if (6 * fC(i) < 1) {
//                    fClr(i) = fB + ((fA - fB) * fC(i) * 6);
//                }
//                else if (2 * fC(i) < 1) {
//                    fClr(i) = fA;
//                }
//                else if (3 * fC(i) < 2) {
//                    fClr(i) = (float)fB + ((fA - fB) * ((2 / 3) - fC(i)) * 6);
//                }
//                else {
//                    fClr(i) = fB;
//                }
//            }
//            return new Pixel32((byte)fClr(0) * 255, (byte)fClr(1) * 255, (byte)fClr(2) * 255, alpha);
//        }
//    }
    
//    public float GetHue()
//    {
//        return System.Drawing.Color.FromArgb(this.Alpha, this.Red, this.Green, this.Blue).GetHue();
//    }
    
//    public float GetSaturation()
//    {
//        return System.Drawing.Color.FromArgb(this.Alpha, this.Red, this.Green, this.Blue).GetSaturation();
//    }
    
//    public float GetBrightness()
//    {
//        return System.Drawing.Color.FromArgb(this.Alpha, this.Red, this.Green, this.Blue).GetBrightness();
//    }
    
//    public byte GetGrayValue( // ERROR: Unsupported modifier : In, Optional
//float redContent,  // ERROR: Unsupported modifier : In, Optional
//float greenContent,  // ERROR: Unsupported modifier : In, Optional
//float blueContent)
//    {
//        float fTotal = redContent + greenContent + blueContent;
//        return (byte)this.Red * redContent / fTotal + this.Green * greenContent / fTotal + this.Blue * blueContent / fTotal;
//    }
    
//    public Pixel32 BlendTo(Pixel32 destination,  // ERROR: Unsupported modifier : In, Optional
//byte additionalAlpha)
//    {
//        Pixel32 pxRet = destination;
//        float fSrc = (float)(this.Alpha / 255) * (additionalAlpha / 255);
//        float fDst = 1 - fSrc;
//        {
//            pxRet.Red = (byte)Conversion.Int((this.Red * fSrc) + (pxRet.Red * fDst));
//            pxRet.Green = (byte)Conversion.Int((this.Green * fSrc) + (pxRet.Green * fDst));
//            pxRet.Blue = (byte)Conversion.Int((this.Blue * fSrc) + (pxRet.Blue * fDst));
//            pxRet.Alpha = (byte)pxRet.Alpha + ((255 - pxRet.Alpha) * fSrc);
//        }
//        return pxRet;
//    }
    
//    public static implicit operator Color(Pixel32 px)
//    {
//        {
//            return Color.FromArgb(px.Alpha, px.Red, px.Green, px.Blue);
//        }
//    }
    
//    public static implicit operator int(Pixel32 px)
//    {
//        {
//            return System.Drawing.Color.FromArgb(px.Alpha, px.Red, px.Green, px.Blue).ToArgb();
//        }
//    }
    
//    public static implicit operator Pixel32(Color cl)
//    {
//        {
//            return new Pixel32(cl.R, cl.G, cl.B, cl.A);
//        }
//    }
    
//    public static implicit operator Pixel32(int i)
//    {
//        Pixel32 vRet = new Pixel32(System.Drawing.Color.FromArgb(i).R, System.Drawing.Color.FromArgb(i).G, System.Drawing.Color.FromArgb(i).B, System.Drawing.Color.FromArgb(i).A);
//    }
    
//    public static bool operator ==(Pixel32 px1, Pixel32 px2)
//    {
//        return (px1.Red == px2.Red) && (px1.Green == px2.Green) && (px1.Blue == px2.Blue) && (px1.Alpha == px2.Alpha);
//    }
    
//    public static bool operator !=(Pixel32 px1, Pixel32 px2)
//    {
//        return (px1.Red != px2.Red) || (px1.Green != px2.Green) || (px1.Blue != px2.Blue) || (px1.Alpha != px2.Alpha);
//        // <- OrElse instead of Or increases speed.
//    }
    
//    // Similar to the = operator, but ignores the alpha channel.
//    //public static bool operator Like(Pixel32 px1, Pixel32 px2)
//    //{
//    //    return (px1.Red == px2.Red) && (px1.Green == px2.Green) && (px1.Blue == px2.Blue);
//    //    // <- AndAlso instead of And increases speed.
//    //}
//}

//#endregion

}
