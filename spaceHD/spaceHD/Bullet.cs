using System;
using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace space
{
    public class Bullet
    {
        private Bitmap _bulletBitmap;
        private double _x, _y, _angle;
        private bool _active = false;
        private int _bulletSpeed;
        private Circle _collisionCircle;

        public List<Enemy> removeEnemy = new List<Enemy>();

        public Bullet(double x, double y, double angle, string bulletType)
        {
            if (bulletType == "laser")
            {
                _bulletBitmap = SplashKit.BitmapNamed("Bullet2");
                _bulletSpeed = 30;
                _collisionCircle = SplashKit.CircleAt(x, y, 200);
            }
            else if (bulletType == "fireball")
            {
                _bulletBitmap = SplashKit.BitmapNamed("Bullet");
                _bulletSpeed = 20;
                _collisionCircle = SplashKit.CircleAt(x , y, 200);
            }
            else
            {
                throw new ArgumentException("Invalid bullet type.");
            }

            _x = x - _bulletBitmap.Width / 2;
            _y = y - _bulletBitmap.Height / 2;
            _angle = angle;
            _active = true;
        }

        public Bullet()
        {
            _active = false;
        }

        public void Update()
        {
            Vector2D movement = new Vector2D();
            Matrix2D rotation = SplashKit.RotationMatrix(_angle);
            movement.X += _bulletSpeed;
            movement = SplashKit.MatrixMultiply(rotation, movement);
            _x += movement.X;
            _y += movement.Y;

            if ((_x > SplashKit.ScreenWidth() || _x < 0) || _y > SplashKit.ScreenHeight() || _y < 0)
            {
                _active = false;
            }
        }

        public void Draw()
        {
            if (_active)
            {
                DrawingOptions options = SplashKit.OptionRotateBmp(_angle);
                _bulletBitmap.Draw(_x, _y, options);
            }
        }

        public Circle CollisionCircle
        {
            get
            {
                return _collisionCircle;
            }
        }

        public bool CollidedWith(Player p, Bullet b)
        {
            b.Update();
            return _bulletBitmap.BitmapCollision(_x, _y, p._shipBitmap, p._x, p._y);
        }

        public bool CollidedWith(Enemy e, Bullet b)
        {
            b.Update();
            return _bulletBitmap.BitmapCollision(_x, _y, e._shipBitmap, e.X, e.Y);
        }

        public void SetBulletType(string bulletType)
        {
            if (bulletType == "laser")
            {
                _bulletBitmap = SplashKit.BitmapNamed("Bullet2");
                _bulletSpeed = 30;
                _collisionCircle = SplashKit.CircleAt(_x, _y, 200);
            }
            else if (bulletType == "fireball")
            {
                _bulletBitmap = SplashKit.BitmapNamed("Bullet");
                _bulletSpeed = 20;
                _collisionCircle = SplashKit.CircleAt(_x - 26, _y, 200);
            }
            else
            {
                throw new ArgumentException("Invalid bullet type.");
            }
        }
    }
}