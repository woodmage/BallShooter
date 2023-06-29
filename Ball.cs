using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallShooter
{
    public enum Bound
    {
        Stop = 0,
        Bounce = 1,
        Wrap = 2,
        Ignore = 3
    };

    internal class Store
    {
        static public Rectangle bounds = new();
        static public void Set(int x, int y, int w, int h) => bounds = new(x, y, w, h);
        static public void Set(Point p, Size s) => bounds = new(p.X, p.Y, s.Width, s.Height);
        static public void Set(Rectangle r) => bounds = new(r.Left, r.Top, r.Width, r.Height);
        static public void Set(int x, int y) => bounds = new(0, 0, x, y);
        static public void Set(Point p) => bounds = new(0, 0, p.X, p.Y);
        static public void Set(Size s) => bounds = new(0, 0, s.Width, s.Height);
        static public int Left() => bounds.Left;
        static public int Right() => bounds.Right;
        static public int Top() => bounds.Top;
        static public int Bottom() => bounds.Bottom;
        static public int Width() => bounds.Width;
        public static int Height() => bounds.Height;
    }

    internal class Ball
    {
        public int posx, posy, sizex, sizey;
        public Image? pic;
        public double movex, movey;
        public int boundminx, boundminy, boundmaxx, boundmaxy, actionminx, actionminy, actionmaxx, actionmaxy;

        private static Bitmap MakePic(Image? pic, int width, int height)
        {
            Bitmap bmp = new(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            Rectangle r = new(0, 0, width, height);
            Pen p = new(Color.White);
            SolidBrush b = new(Color.White);
            if (pic == null)
            {
                g.DrawEllipse(p, r);
                g.FillEllipse(b, r);
            }
            else
            {
                Rectangle s = new(0, 0, pic.Width, pic.Height);
                g.DrawImage(pic, r, s, GraphicsUnit.Pixel);
            }
            g.Dispose();
            return bmp;
        }
        private void InitBall(int x, int y, int width, int height, Image? pic)
        {
            this.pic = MakePic(pic, width, height); //get pic or make one if needed
            posx = x; 
            posy = y;
            sizex = width;
            sizey = height;
            boundminx = Store.Left();
            boundminy = Store.Top();
            boundmaxx = Store.Right();
            boundmaxy = Store.Bottom();
            actionminx = actionminy = actionmaxx = actionmaxy = 0; //set all boundary actions to stop
            movex = movey = 0;  //start with no movement
        }
        public Ball(int x, int y, int width, int height, Image? pic) => InitBall(x, y, width, height, pic);
        public Ball(int x, int y, int width, int height) => InitBall(x, y, width, height, null);
        public Ball(int x, int y, Image pic) => InitBall(x, y, pic.Width, pic.Height, pic);
        public Ball(int x, int y, int radius, Image? pic) => InitBall(x, y, radius, radius, pic);
        public Ball(int x, int y, int radius) => InitBall(x, y, radius, radius, null);
        public void SetBounds(int left, int top, int right, int bottom)
        {
            boundminx = left;
            boundminy = top;
            boundmaxx = right;
            boundmaxy = bottom;
        }
        public void SetActions(int minx, int miny, int maxx, int maxy)
        {
            actionminx = minx;
            actionminy = miny;
            actionmaxx = maxx;
            actionmaxy = maxy;
        }
        public void SetActions(Bound minx, Bound miny, Bound maxx, Bound maxy) => SetActions((int)minx, (int)miny, (int)maxx, (int)maxy);
        public void AddMove(double dx, double dy)
        {
            movex += dx;
            movey += dy;
        }
        public void Move()
        {
            posx += (int)movex;
            posy += (int)movey;
            Bounds();
        }
        public void Move(int x, int y)
        {
            AddMove(x, y);
            Move();
        }
        public void Move(double x, double y)
        {
            AddMove(x, y);
            Move();
        }
        public void MoveTo(int x, int y)
        {
            posx = x;
            posy = y;
        }
        public void Paint(Graphics g)
        {
            if (pic != null)
            {
                Rectangle src = new(0, 0, pic.Width, pic.Height);
                Rectangle dst = new(posx, posy, sizex, sizey);
                g.DrawImage(pic, dst, src, GraphicsUnit.Pixel);
            }
        }
        public bool IsHit(Ball b)
        {
            if ((posx + sizex < b.posx) || (posx > b.posx + b.sizex)) return false;
            if ((posy + sizey < b.posy) || (posy > b.posy + b.sizey)) return false;
            return true;
        }
        private void Bounds()
        {
            //int left = posx;
            //int top = posy;
            int right = posx + sizex - 1;
            int bottom = posy + sizey - 1;
            if (posx < boundminx)
            {
                if (actionminx == 0) { posx = boundminx; movex = 0.0; }
                if (actionminx == 1) { posx = boundminx - (posx - boundminx); movex = -movex; }
                if (actionminx == 2) { posx = boundmaxx - (right - boundminx); }
            }
            if (posy < boundminy)
            {
                if (actionminy == 0) { posy = boundminy; movey = 0.0; }
                if (actionminy == 1) { posy = boundminy - (posy - boundminy); movey = -movey; }
                if (actionminy == 2) { posy = boundmaxy - (bottom - boundmaxy); }
            }
            if (right > boundmaxx)
            {
                if (actionmaxx == 0) { posx = boundmaxx - sizex; movex = 0.0; }
                if (actionmaxx == 1) { posx = boundmaxx - (right - boundmaxx); movex = -movex; }
                if (actionmaxx == 2) { posx = boundminx + (right - boundmaxx);  }
            }
            if (bottom > boundmaxy)
            {
                if (actionmaxy == 0) { posy = boundmaxy - sizex; movey = 0.0; }
                if (actionmaxy == 1) { posy = boundmaxy - (bottom - boundmaxy); movey = -movey; }
                if (actionmaxy == 2) { posy = boundminy + (bottom - boundmaxy); }
            }
        }
    }
    internal class BallOld
    {
        public int posx, posy, sizex, sizey;
        public Image? pic;
        public double movex, movey;
        public int boundminx, boundminy, boundmaxx, boundmaxy, actionminx, actionminy, actionmaxx, actionmaxy;
        //public int boundx, boundy;

        public static Bitmap Makepic(Image? pic, int sizex, int sizey)
        {
            Bitmap bmp = new(sizex, sizey);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            Rectangle r = new(0, 0, sizex, sizey);
            if (pic == null)
            {
                Pen p = new(Color.White);
                SolidBrush b =new(Color.White);
                g.DrawEllipse(p, r);
                g.FillEllipse(b, r);
            }
            else
            {
                g.DrawImage(bmp, r, r, GraphicsUnit.Pixel);
            }
            g.Dispose();
            return bmp;
        }
        public void InitBall(int x, int y, Image? pic, int sizex, int sizey)
        {
            posx = x;
            posy = y;
            this.pic = Makepic(pic, sizex, sizey);
            movex = movey = 0.0;
            boundminx = Store.Left();
            boundminy = Store.Top();
            boundmaxx = Store.Right();
            boundmaxy = Store.Bottom();
            //boundx = boundy = 0;
            this.sizex = sizex;
            this.sizey = sizey;
        }
        /*public Ball(int x, int y, Image pic) => InitBall(x, y, pic, pic.Width, pic.Height);
        public Ball(int x, int y, int width, int height) => InitBall(x, y, null, width, height);
        public Ball(int x, int y, int r) => InitBall(x, y, null, r, r);
        public Ball(int x, int y, Image pic, int width, int height) => InitBall(x, y, pic, width, height);*/
        public void SetMove(double x, double y)
        {
            movex = x;
            movey = y;
        }
        public double GetMoveX() => movex;
        public double GetMoveY() => movey;
        public void AddMove(double dx, double dy)
        {
            movex += dx;
            movey += dy;
        }
        public void SetBounds(int left, int top, int right, int bottom)
        {
            boundminx = left;
            boundminy = top;
            boundmaxx = right;
            boundmaxy = bottom;
        }
        public void SetBounds(Rectangle r) => SetBounds(r.Left, r.Top, r.Right, r.Bottom);
        public void SetActions(int left, int top, int right, int bottom)
        {
            actionminx = left;
            actionminy = top;
            actionmaxx = right;
            actionmaxy = bottom;
        }
        public void SetActions(Bound left, Bound top, Bound right, Bound bottom) => SetActions((int)left, (int)top, (int)right, (int)bottom);
        public bool IsHit(int x, int y, int width, int height)
        {
            if ((posx + sizex < x) || (posx > x + width)) return false;
            if ((posy + sizey < y) || (posy > y + height)) return false;
            return true;
        }
        public bool IsHit(Ball b) => IsHit(b.posx, b.posy, b.sizex, b.sizey);
        private static void DoAction(ref int pos, ref double move, int bound1, int bound2, int action, int over)
        {
            if (action == 0)
            {
                pos = bound1;
                move = 0.0;
            }
            else if (action == 1)
            {
                pos = bound1 + over;
                move = -move;
            }
            else if (action == 2)
            {
                pos = bound2 - over;
            }
        }
        private void Bounds()
        {
            if (posx < boundminx) DoAction(ref posx, ref movex, boundminx, boundmaxx, actionminx, boundminx - posx);
            if (posx > boundmaxx) DoAction(ref posx, ref movex, boundmaxx, boundminx, actionmaxx, boundmaxx - posx);
            if (posy < boundminy) DoAction(ref posy, ref movey, boundminy, boundmaxy, actionminy, boundminy - posy);
            if (posy > boundmaxy) DoAction(ref posy, ref movey, boundmaxy, boundminy, actionmaxy, boundmaxy - posy);
        }
        public void Move()
        {
            posx += (int)movex;
            posy += (int)movey;
            Bounds();
        }
        public void Move(int x, int y)
        {
            posx += x;
            posy += y;
            Bounds();
        }
        public void Move(double thrustx, double thrusty)
        {
            AddMove(thrustx, thrusty);
            Move();
        }
        public void Paint(Graphics g)
        {
            if (pic != null)
            {
                Rectangle src = new(0, 0, pic.Width, pic.Height);
                Rectangle dst = new(posx, posy, sizex, sizey);
                g.DrawImage(pic, dst, src, GraphicsUnit.Pixel);
            }
        }
    }
}
