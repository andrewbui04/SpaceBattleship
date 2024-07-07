using System;
using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace space
{
    public class Player
    {
        

        public double _x, _y;
        private double _angle;
        public Bitmap _shipBitmap;
        private Bullet _bullet = new Bullet();
        public List<Bullet> _bulletList = new List<Bullet>();
        public Bitmap [] _lifeBitmap = new Bitmap [5];

        private double _lifeX{get;set;}
        private double _lifeY{get;set;}

        public int _lifeCount{get; set;}

        public int score{get; set;}
        public Player()
        { 
            Angle = 270; 
            _shipBitmap = SplashKit.BitmapNamed("player");

            _lifeCount = 5; 
            _lifeX = 10;
            _lifeY = 10;
        }

        public double X {get { return _x; } set { _x = value; }}
        public double Y{get { return _y; } set { _y = value; }}
        public double Angle{ get { return _angle; } set { _angle = value; }}
      
        public void Draw()
        {
            _shipBitmap.Draw(_x, _y, SplashKit.OptionRotateBmp(_angle));
            _bullet.Draw();
            DrawBullets();

            for(int i = 0; i< _lifeCount; i++ )
            {
                _lifeBitmap[i] = new Bitmap("Energy","fuel.png");
                _lifeBitmap[i].Draw(_lifeX + (40*i),_lifeY);
                
            }
            SplashKit.DrawText("Score : " + score, Color.Red, "Montserrat-Bold",  200, 600,10);
        }
        public void Shoot(string bulletType)
        {
            Matrix2D anchorMatrix = SplashKit.TranslationMatrix(SplashKit.PointAt(_shipBitmap.Width / 2, _shipBitmap.Height / 2));

            // Move centre point of picture to origin
            Matrix2D result = SplashKit.MatrixMultiply(SplashKit.IdentityMatrix(), SplashKit.MatrixInverse(anchorMatrix));
            // Rotate around origin
            result = SplashKit.MatrixMultiply(result, SplashKit.RotationMatrix(_angle));
            // Move it back...
            result = SplashKit.MatrixMultiply(result, anchorMatrix);

            // Now move to location on screen...
            result = SplashKit.MatrixMultiply(result, SplashKit.TranslationMatrix(X, Y));

            // Result can now transform a point to the ship's location
            // Get right/centre
            Vector2D vector = new Vector2D();
            vector.X = _shipBitmap.Width;
            vector.Y = _shipBitmap.Height / 2;
            // Transform it...
            vector = SplashKit.MatrixMultiply(result, vector);
            _bullet = new Bullet(vector.X, vector.Y, Angle, "fireball");
            _bulletList.Add(_bullet);
        }

        public void Update()
        {
            _bullet.Update();
        }

        public void Move(double amountForward, double amountStrafe)
        {
            Vector2D movement = new Vector2D();Matrix2D rotation = SplashKit.RotationMatrix(_angle);
            movement.X += amountForward;movement.Y += amountStrafe;
            movement = SplashKit.MatrixMultiply(rotation, movement);
            _x += movement.X;_y += movement.Y;
        }

        public Circle CollisionCircle
        {
            get { return SplashKit.CircleAt(_x, _y, 20); }
          
        }

        public void RemoveLife(int val)
        {
            for(int i = 0; i< val; i++ )
            {
                // _lifeBitmap.Add(new Bitmap("Life","heart.png"));
                _lifeBitmap[i] = new Bitmap("Energy","fuel.png");
                _lifeBitmap[i].Draw(_lifeX + (40*i),_lifeY);
                
            }
        }

        private void DrawBullets()
        {
            foreach (Bullet _bullet in _bulletList)
            {
                _bullet.Draw();
            }
        }
    }
}