using BallShooter.Properties;
using System.Diagnostics.Metrics;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;

namespace BallShooter
{
    public partial class Form1 : Form
    {
        readonly System.Windows.Forms.Timer timer = new();
        readonly System.Windows.Forms.Timer timer2 = new();
        readonly System.Windows.Forms.Timer timer3 = new();
        readonly Random rand = new();
        static Size programsize = new();
        static Size playsize = new(1024, 1000);
        static readonly List<Image> pictures = new() { Resources._00yellow, Resources._01orange, Resources._02red, Resources._03magenta,
            Resources._04purple, Resources._05blue, Resources._06aqua, Resources._07green, Resources._08white, Resources._09grey};
        static readonly Image spaceshippic = Resources.spaceship;
        private readonly List<Ball> enemy = new();
        private readonly List<Ball> enemysplit = new();
        private readonly Ball ship;
        private readonly List<Ball> shot = new();
        private readonly List<Ball> missile = new();
        private readonly List<Ball> firedmissile = new();
        readonly Bitmap arenabmp, infobmp;
        int level = 0;
        bool domove = true;
        int countdowntime = 5;
        int maxlife = 25;
        int life = 25;
        long score = 0;
        long nextlevel = 1000;
        int autoshot = 0;
        int autoshotlevel = 1;
        int autoshotcounter = 2, autoshotcountermax = 2;
        bool newlevel = true;
        public Form1()
        {
            InitializeComponent();  //required windows stuff
            ship = new Ball(500, 950, spaceshippic); //make ship
            ship.SetBounds(0, 900, 1024, 1000); //set boundaries
            ship.SetActions(Bound.Wrap, Bound.Ignore, Bound.Wrap, Bound.Ignore); //set to wrap left and right and ignore up and down
            Store.Set(1024, 1000); //set Storage rectangle 
            arenabmp = new Bitmap(playsize.Width, playsize.Height); //make bitmap for game arena
            pictureBox1.Image = arenabmp; //set picturebox to use arena bmp bitmap
            infobmp = new Bitmap(760, 1040); //make bitmap for information area
            pictureBox2.Image = infobmp; //set picturebox to use info bmp bitmap
        }
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            Keys k = e.KeyData; //get keyboard data
            if (k == Keys.Left) //if left arrow key
                ship.AddMove(-1, 0); //move left
            if (k == Keys.Right) //if right arrow key
                ship.AddMove(1, 0); //move right
            if (k == Keys.Down) //if down arrow key
                ship.movex = 0; //stop movement
            if (k == Keys.Up) //if up arrow key
                MakeShot(); //shoot
            if (k == Keys.Back) //if backspace key
                FireMissile(); //fire a missile
            if ((k == Keys.Space) || (k == Keys.P)) //if space key or P key
                domove = !domove; //toggle movement flag (pause/unpause)
            if (k == Keys.L) //if L key
                if (level < 50) //if level < 50
                    score = nextlevel + 10; //next level
            if (k == Keys.H) //if H key
                life = maxlife; //fully heal
            if (k == Keys.D) //if D key
                life = -10; //die
            if (k == Keys.M) //if M key
                level = 51; //set max level
            if (k == Keys.Escape) //if Escape key
                if (AskUser("Exit? Are you sure?", "Exit?")) //if user agrees
                    Application.Exit(); //exit program
            e.Handled = true;  //tell windows we took care of it
            DrawIt(); //draw the arena
        }
        private void FireMissile()
        {
            if (missile.Count == 0) return; //if we don't have any missiles, return
            firedmissile.Add(missile[0]); //add first missile to fired missiles
            int i = firedmissile.Count - 1;
            missile.Remove(missile[0]); //delete that missile from inventory
            firedmissile[i].posx = ship.posx; //set horizontal position
            firedmissile[i].posy = ship.posy - firedmissile[i].sizey; //set vertical position
            firedmissile[i].AddMove(0, -5); //slow movement
            firedmissile[i].SetBounds(0, 0, 1024, 1000); //set boundaries
            firedmissile[i].SetActions(Bound.Stop, Bound.Stop, Bound.Stop, Bound.Stop); //set actions to stop at boundaries
        }
        private bool AskUser(string question, string title)
        {
            domove = false; //stop movement while waiting for user to answer
            bool r = (MessageBox.Show(question, title, MessageBoxButtons.YesNo) == DialogResult.Yes); //get yes/no answer
            domove = true; //resume movement
            return r; //return yes/no answer in form of true/false
        }
        private void AddShot() => AddShot(0);
        private void AddShot(int s)
        {
            int x = ship.posx + ship.sizex / 2; //position in middle of ship
            int y = ship.posy + ship.sizey / 4; //position in upper part of ship
            int r = 5 + autoshot / 10; //very small to smallish
            int halfs = (s + 1) / 2; //half of s + 1
            if (s % 2 == 0) halfs = -halfs; //if even, make half of s negative
            double move = halfs * 1.45; //compute move in horizontal direction
            Ball b = new(x, y, r); //make new ball for shot, no picture so will be solid white circle
            b.AddMove(move, -5); //move
            b.SetBounds(0, 0, 1024, 1000); //set boundaries
            b.SetActions(Bound.Bounce, Bound.Ignore, Bound.Bounce, Bound.Ignore); //set actions for boundaries
            shot.Add(b); //add to shot list
        }
        private void OnResize(object sender, EventArgs e) => this.ClientSize = programsize; //resize denied!
        private void OnLoad(object sender, EventArgs e)
        {
            programsize = this.ClientSize; //get program size (clientsize)
            StartGame(); //begin the game
            timer.Interval = 100; //set timer interval to 1/10th a second
            timer.Tick += DoMove; //set function to do each time timer reaches interval
            timer.Start(); //start timer
        }
        private void MakeShot()
        {
            if (autoshotcounter == 0) //if ready to shoot
            {
                AddShot(); //fire regular shot
                for (int i = 1; i < autoshot; i++) //for each autoshot
                    AddShot(i); //fire shot
                autoshotcounter = autoshotcountermax; //set autoshot counter back to maximum
            }
            else //otherwise
                autoshotcounter--; //get ready...
        }
        private void DoMove(object? sender, EventArgs e)
        {
            timer.Interval = 100 - level * 3 / 2; //speed up as we gain levels
            if (!domove) return; //if we are paused, just exit function
            if (autoshot < level / autoshotlevel) //maximum is by level
                autoshot++; //add to autoshot
            if (autoshot > 0) MakeShot(); //handle autoshot
            if (enemy.Count == 0) //if we have no enemies
                MakeLevel(level); //make a new level
            enemysplit.Clear(); //clear split enemies list
            int makeenemy = 0; //counter for creating new enemies
            ship.Move(); //move the ship
            for (int i = 0; i < enemy.Count; i++) //for each enemy
            {
                double dx = RandRange(-0.10, 0.10); //get thrust in x
                double dy = RandRange(-0.08, 0.16); //get thrust in y, weighted toward moving downward
                enemy[i].Move(dx * (level + 1), dy * (level + 1)); //do enemy movement
                if (enemy[i].posy > 990 - enemy[i].sizey) //if enemy is at bottom of play area
                {
                    enemy.RemoveAt(i);  //remove enemy
                    i--; //fix counter of for loop
                    score -= 10; //decrease score
                    life -= 5; //decrease life
                    makeenemy++; //set counter to make one new enemy
                }
            }
            for (int s = 0; s < shot.Count; s++) //for each shot
            {
                shot[s].Move(0, -.5); //move shot
                for (int i = 0; i < enemy.Count; i++) //loop through enemies
                {
                    if (shot[s].IsHit(enemy[i])) //if shot hits enemy
                    {
                        enemysplit.Add(enemy[i]); //make copy of split enemy
                        enemy.RemoveAt(i); //get rid of enemy
                        i = enemy.Count - 1; //get through for i loop
                        score += 100;  //add points
                    }
                }
                if (shot[s].posy < 10) //if shot at top of play area
                {
                    shot.RemoveAt(s); //get rid of shot
                    s--; //adjust for loop counter
                }
            }
            while (enemysplit.Count > 0) //while there are enemies to split
            {
                if ((enemysplit[0].sizex > 80) && (enemy.Count < 50 * level)) //only split if size > 80 and have less than 50 enemies / level
                {
                    int newsize = enemysplit[0].sizex / 2; //compute new size
                    enemy.Add(new(enemysplit[0].posx + newsize / 2, enemysplit[0].posy, newsize, enemysplit[0].pic));
                    enemy.Add(new(enemysplit[0].posx, enemysplit[0].posy + newsize / 2, newsize, enemysplit[0].pic));
                    enemy.Add(new(enemysplit[0].posx + newsize, enemysplit[0].posy + newsize / 2, newsize, enemysplit[0].pic));
                    enemy.Add(new(enemysplit[0].posx + newsize / 2, enemysplit[0].posy + newsize, newsize, enemysplit[0].pic));
                }
                else //otherwise (if not splitting)
                    makeenemy++; //just add one to make enemies counter
                enemysplit.RemoveAt(0); //remove that enemy to split
            }
            while (makeenemy > 0) //while we need to make enemies
            {
                AddEnemy(); //add a new enemy
                makeenemy--; //decrease make enemy flag
            }
            if (firedmissile.Count > 0) //if we have any fired missiles
            {
                for (int i = 0; i < firedmissile.Count; i++) //for every fired missile
                {
                    firedmissile[i].Move(); //move the missile
                    if (firedmissile[i].posy < 10) //if missile has hit boundary
                    {
                        for (int j = 0; j < enemy.Count; j++) //for each enemy
                        {
                            score += 1000; //add to score for that enemy
                            enemy.RemoveAt(j); //get rid of enemy
                            j--; //fix counter for for loop
                        }
                        firedmissile.RemoveAt(i); //get rid of fired missile
                        i--; //fix counter for for loop
                    }
                }
            }
            if (score > nextlevel) //if score is enough
            {
                level++; //bump up level
                nextlevel *= 2; //next level will require double score
                MakeLevel(level); //make new level
                maxlife = 50 * level; //compute maximum life
                life = maxlife; //set life to full
                newlevel = true; //we are in a new level
                if (level % 5 == 0) //every five levels
                    MakeMissile(); //make the missile
            }
            DrawIt(); //draw screen
        }

        private void MakeMissile()
        {
            missile.Add(new(-1, -1, Resources.missile)); //add new missile
        }

        private static Point DrawThisString(Graphics g, string text, Point p, bool newline)
        {
            SolidBrush brush = new(Color.White); //set brush to white
            Font font = new("Comic Sans MS", 25); //make a font to use
            g.DrawString(text, font, brush, p); //draw the string
            SizeF txtsize = g.MeasureString(text + "  ", font); //get size of string (two extra spaces at end)
            if (newline) //if new line
                p.Y += (int)txtsize.Height; //add height to Y component
            else //otherwise
                p.X += (int)txtsize.Width; //add width to Y component
            return p; //return point
        }

        private static void DrawStringCentered(Graphics g, string text, int x0, int y0, int wide, int high, bool clear) => DrawStringCentered(g, text, x0, y0, wide, high, clear, 35);
        private static void DrawStringCentered(Graphics g, string text, int x0, int y0, int wide, int high, bool clear, int fontsize)
        {
            Font font = new("Comic Sans MS", fontsize); //use Arial Black for font
            DrawStringCentered(g, text, x0, y0, wide, high, clear, font);
        }
        private static void DrawStringCentered(Graphics g, string text, int x0, int y0, int wide, int high, bool clear, Font font)
        {
            DrawStringCentered(g, text, x0, y0, wide, high, clear, font, Color.White);
        }
        private static void DrawStringCentered(Graphics g, string text, int x0, int y0, int wide, int high, bool clear, Font font, Color color)
        {
            SolidBrush brush = new(color); //set brush to color
            //Font font = new("Arial Black", fontsize); 
            SolidBrush backbrush = new(Color.Black); //set brush for background
            SizeF size = g.MeasureString(text, font); //get size of text
            float px = (float)(x0 + ((wide - size.Width) / 2)); //calculate starting x position
            float py = (float)(y0 + ((high - size.Height) / 2)); //calculate starting y position
            if (clear) //if clear
                g.FillRectangle(backbrush, px, py, size.Width, size.Height); //then erase background
            g.DrawString(text, font, brush, px, py); //draw the text
        }
        private static Point DrawBarGraph(Graphics g, string text, Point p, Size s, int var1, int var2)
        {
            if (var2 > 0) //avoid divide by zero error for autoshot when not available
            {
                while (var2 > 100000) { var1 /= 1000; var2 /= 1000; } //reduce size of variables as needed
                SolidBrush backbrush = new(Color.DarkRed); //make background brush dark red
                SolidBrush frontbrush = new(Color.DarkGreen); //make foreground brush dark green
                g.FillRectangle(backbrush, p.X, p.Y, s.Width - 1, s.Height); //apply background
                int newwidth = s.Width * var1 / var2; //compute portion for foreground
                g.FillRectangle(frontbrush, p.X, p.Y, newwidth, s.Height); //apply foreground
                g.DrawRectangle(new Pen(Color.White), p.X, p.Y, s.Width - 1, s.Height); //outline in white
                DrawStringCentered(g, text, p.X, p.Y, s.Width, s.Height, false, 20); //draw text centered
                p.Y += s.Height; //add height to point
            }
            return p; //return point
        }
        private static Point DrawBarGraph(Graphics g, string text, Point p, Size s, long var1, long var2)
        {
            if (var2 > 0) //avoid divide by zero error for autoshot when not available
            {
                while (var2 > 100000) { var1 /= 1000; var2 /= 1000; } //reduce size of variables as needed
                SolidBrush backbrush = new(Color.DarkRed); //make background brush dark red
                SolidBrush frontbrush = new(Color.DarkGreen); //make foreground brush dark green
                g.FillRectangle(backbrush, p.X, p.Y, s.Width - 1, s.Height); //apply background
                int newwidth = (int)((long)s.Width * var1 / var2); //compute portion for foreground
                g.FillRectangle(frontbrush, p.X, p.Y, newwidth, s.Height); //apply foreground
                g.DrawRectangle(new Pen(Color.White), p.X, p.Y, s.Width - 1, s.Height); //outline in white
                DrawStringCentered(g, text, p.X, p.Y, s.Width, s.Height, false, 20); //draw text centered
                p.Y += s.Height; //add height to point
            }
            return p; //return point
        }
        private void DrawIt()
        {
            Graphics arena = Graphics.FromImage(arenabmp); //get graphics object to do painting with
            bool ispaused = !domove; //set flag for if paused
            domove = false; //set to not move during painting
            arena.Clear(Color.Black); //clear arena to black
            arena.DrawImage(Resources.space, new Rectangle(0, 0, 1024, 1000), new Rectangle(0, 0, 1024, 1000), GraphicsUnit.Pixel); //draw background
            if (enemy.Count == 0) //if no enemies
            {
                arena.DrawImage(Resources.explodingspace, new Rectangle(0, 0, 1024, 1000), new Rectangle(0, 0, 1024, 1000), GraphicsUnit.Pixel); //draw explosion
                timer2.Interval = 500; //set timer interval to a half second
                timer2.Tick += UnPauseByTime; //set function to use
                timer2.Start(); //start timer
                domove = false; //pause
                ispaused = true; //set to paused
            }
            else //otherwise
            {
                foreach (Ball b in enemy) //for each enemy
                    b.Paint(arena); //paint it in arena
                foreach (Ball s in shot) //for each shot
                    s.Paint(arena); //paint it in arena
            }
            foreach (Ball m in firedmissile) //for each fired missile
                m.Paint(arena); //paint it in arena
            ship.Paint(arena); //paint ship in arena
            if (life < 0) //if we have died
            {
                Color FadeRed = Color.FromArgb(128, Color.Red); //faded red color (translucent)
                DrawOutlinedString(arena, "You DIED!", "a dripping marker", 100, 0, 0, 1024, 1000, FadeRed, Color.Yellow, 15); //inform user
                timer3.Interval = 5000; //set timer interval to 5 seconds
                timer3.Tick += DeathTimer; //set function to use
                timer3.Start(); //start timer
                domove = false; //pause
                ispaused = true; //set to paused
            }
            if (newlevel) //if this is a new level
            {
                if (level != 0)
                {
                    DrawStringCentered(arena, "Welcome to level " + (level + 1) + "!", 0, 0, 1024, 900, true); //show welcome message
                    newlevel = false; //and we are no longer in a new level
                    timer2.Interval = 1000; //set timer interval to 1 second
                    timer2.Tick += UnPauseByTime; //set function to use
                    timer2.Start(); //start timer
                    domove = false; //pause
                    ispaused = true; //set to paused
                }
            }
            if (ispaused) //if we are paused
                if (life >= 0) //and we aren't dead
                    DrawStringCentered(arena, "PAUSED!  Press P to continue...", 0, 0, 1024, 1100, true); //show reminder that we are paused
            arena.Dispose(); //get rid of graphics object
            pictureBox1.Image = arenabmp; //set picturebox to use arena bmp
            pictureBox1.Invalidate(); //tell picturebox to repaint
            statusline.Text = "Ship @(" + ship.posx + "," + ship.posy + ") Move: (" + ship.movex + "," + ship.movey + ")";
            Graphics info = Graphics.FromImage(infobmp); //get graphics object to do painting with
            info.Clear(Color.FromArgb(0, 0, 15)); //clear information area to very dark blue
            Point p = new(10, 10); //start at 10,10
            p = DrawThisString(info, "Health: " + life, p, true); //write health line
            Point sz = p; //copy info for size
            sz.Y -= 10; //subtract starting point for vertical info
            sz.X = infobmp.Width / 2; //set starting point for horizontal info
            p = DrawThisString(info, "Score: " + RepresentLong(score), p, true); //write score line
            p = DrawThisString(info, "Next: " + RepresentLong(nextlevel), p, true); //write next level
            p = DrawThisString(info, "Level: " + (level + 1), p, true); //write level line
            p = DrawThisString(info, "Enemies: " + enemy.Count, p, true); //write enemy line
            p = DrawThisString(info, "Shots: " + shot.Count, p, true); //write shots line
            Size size = new(infobmp.Width / 2, sz.Y); //set size for graphics displays
            sz.Y = 10; //start at top
            sz = DrawBarGraph(info, "Life: " + life, sz, size, life, maxlife); //make bar graph for life
            sz = DrawBarGraph(info, "Score: " + RepresentLong(score), sz, size, score, nextlevel); //make bar graph for score
            sz.Y += size.Height * 3; //skip down some so autoshots lines up with shots
            sz = DrawBarGraph(info, "AutoShots: " + autoshot, sz, size, autoshot, level / autoshotlevel); //make bar graph for autoshots
            sz.Y += size.Height; //move down a "line"
            sz.X = 10; //left border
            Rectangle src = new(0, 0, Resources.missile.Width, Resources.missile.Height); //source rectangle
            Rectangle dst = new(sz.X, sz.Y, Resources.missile.Width, Resources.missile.Height); //destination rectangle
            foreach (Ball m in missile) //for each missile owned
            {
                info.DrawImage(Resources.missile, dst, src, GraphicsUnit.Pixel); //draw the missile
                dst.X += Resources.missile.Width; //add the width to the position we will draw the next one (make them side-by-side)
            }
            info.Dispose(); //get rid of graphics object
            pictureBox2.Image = infobmp; //set picturebox to use info bmp
            pictureBox2.Invalidate(); //tell picturebox to repaint
            domove = !ispaused; //set to move again or stay paused
        }

        private void DeathTimer(object? sender, EventArgs e)
        {
            timer3.Stop(); //stop the timer
            domove = false; //stay paused
            if (AskUser("Would you like to play again?", "You DIED!")) //if user wants to play again
                StartGame(); //restart game
            else //otherwise
                Application.Exit(); //exit game
        }

        private void UnPauseByTime(object? sender, EventArgs e)
        {
            //timer2.Interval = 1000; //set timer interval to 1 second
            if (domove) //if we unpaused
            {
                timer2.Stop(); //stop timer
                countdowntime = 5; //and reset counter
            }
            else //otherwise
            {
                Graphics g = Graphics.FromImage(arenabmp); //get graphics object so we can tell user how long he has
                DrawStringCentered(g, countdowntime.ToString(), 0, 0, 1024, 1200, true); //display countdown timer
                g.Dispose(); //get rid of graphics object
                pictureBox1.Image = arenabmp; //set picturebox to use arena bmp for image
                pictureBox1.Invalidate(); //tell picturebox to redraw
                countdowntime--; //decrease count down time
                if (countdowntime < 0) //if it is less than zero
                    domove = true; //unpause
            }
        }
        private void AddEnemy()
        {
            Ball b = new(0, 0, pictures[0]); //make empty ball for enemy
            bool goon = false; //make flag to tell whether we are done with creation
            while (!goon) //while not yet done
            {
                int r = RandRange(1, 20) * 4 + 20; //gives 24 to 100 for size of enemy
                int x = RandRange(0, Store.Width() - r - 1); //set horizontal position
                int y = RandRange(0, (Store.Height() - r - 1) / 4); //set vertical position
                int n = RandRange(0, 9); //get random for enemy color
                b = new(x, y, r, pictures[n]); //make ball for enemy
                goon = !HitAny(b); //if it hits any other enemy, we are not yet done
                if (enemy.Count > 100) goon = true; //bypass testing if over 100 enemies already
            }
            enemy.Add(b); //add ball to enemies
        }
        private void StartGame()
        {
            level = 0; //reset variables
            nextlevel = 1000;
            domove = true;
            countdowntime = 5;
            maxlife = 25;
            life = 25;
            score = 0;
            autoshot = 0;
            autoshotlevel = 1;
            autoshotcounter = 2;
            autoshotcountermax = 2;
            newlevel = true;
            shot.Clear();
            enemysplit.Clear();
            missile.Clear();
            firedmissile.Clear();
            MakeLevel(0); //make new level
            domove = false;
            Graphics arena = Graphics.FromImage(arenabmp); //get graphics object
            DrawOutlinedString(arena, "Bubble Shooter v. 1.0", "Comic Sans MS", 65, 0, 0, 1024, 900, Color.Yellow, Color.Red, 10); //show logo
            arena.Dispose(); //get rid of graphics object
            pictureBox1.Invalidate(); //tell picturebox to redraw
            timer2.Interval = 1000; //set timer interval to 1 second
            timer2.Tick += UnPauseByTime; //set function to use
            timer2.Start(); //start timer
        }
        private void MakeLevel(int level)
        {
            bool domovecopy = domove; //make copy of domove flag
            domove = false; //pause for now
            enemy.Clear(); //clear enemies
            int nballs = 5 + level; //set number balls to 5 + level
            for (int i = 0; i < nballs; i++) //for each ball
                AddEnemy(); //add enemy
            DrawIt(); //draw arena
            domove = domovecopy; //get status of domove flag back
        }
        private int RandRange(int min, int max) => rand.Next() % (max - min + 1) + min; //get random integer in min/max range
        private double RandRange(double min, double max) => RandThrust(max - min) + min; //get random double in min/max range
        private double RandThrust(double max) => rand.NextDouble() * max; //get random double in 0/max range
        private bool HitAny(Ball ball)
        {
            if (enemy.Count > 0) //if we have enemies to test against
                foreach (Ball b in enemy) //for each enemy
                    if (ball != b) //if it is not the same as our ball we are testing against
                        if (b.IsHit(ball)) return true; //if it hits, return true
            return false; //otherwise return false
        }
        private void DoMouse(object sender, MouseEventArgs e)
        {
            ship.posx = e.X; //copy x position to ship
            if (e.Button == MouseButtons.Left) //if left mouse button
                MakeShot(); //do shot
            if (e.Button == MouseButtons.Right) //if right mouse button
                FireMissile(); //fire missile
        }
        private static void DrawOutlinedString(Graphics g, string text, string fontfamily, int fontsize, int posx, int posy, int sizex, int sizey, Color back, Color fore, int s)
        {
            Font font = new(fontfamily, fontsize, FontStyle.Bold); //make font
            SizeF size = g.MeasureString(text, font); //measure text
            int px = posx + (sizex - (int)size.Width) / 2; //compute starting horizontal position
            int py = posy + (sizey - (int)font.Height) / 2; //compute starting vertical position
            FontFamily family = new(fontfamily); //make font family
            Pen p = new(back, s); //make pen for outline
            SolidBrush b = new(fore); //make brush for insides
            GraphicsPath gp = new(); //make new graphics path
            gp.AddString(text, family, (int)FontStyle.Bold, g.DpiY * fontsize / 72, new Point(px, py), new StringFormat()); //add string
            g.DrawPath(p, gp); //draw outline
            g.FillPath(b, gp); //draw insides
        }
        private static string RepresentLong(long value)
        {
            string retstr;
            long ten16 = 10000000000000000;
            long ten15 = 1000000000000000; //p
            long ten14 = 100000000000000;
            long ten13 = 10000000000000;
            long ten12 = 1000000000000; //t
            long ten11 = 100000000000;
            long ten10 = 100000000000;
            long ten9 = 1000000000; //g
            long ten8 = 100000000;
            long ten7 = 10000000;
            long ten6 = 1000000; //m
            long ten5 = 100000;
            long ten4 = 10000;
            long ten3 = 1000; //k
            long ten2 = 100;
            if (value > ten16)
            {
                long ip = (value / ten15);
                long dp = (value - ip * ten15) / ten14;
                retstr = ip + "." + dp + "P";
            }
            else if (value > ten13)
            {
                long ip = (value / ten12);
                long dp = (value - ip * ten12) / ten11;
                retstr = ip + "." + dp + "T";
            }
            else if (value > ten10)
            {
                long ip = (value / ten9);
                long dp = (value - ip * ten9) / ten8;
                retstr = ip + "." + dp + "G";
            }
            else if (value > ten7)
            {
                long ip = (value / ten6);
                long dp = (value - ip * ten6) / ten5;
                retstr = ip + "." + dp + "M";
            }
            else if (value > ten4)
            {
                long ip = (value / ten3);
                long dp = (value - ip * ten3) / ten2;
                retstr = ip + "." + dp + "K";
            }
            else
                retstr = value.ToString();
            return retstr;
        }
    }
}
