using System;
using space;
using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

using System.IO.Ports;
using System.IO.Pipelines;
using System.Threading;
using WindowsInput;
using System.Security.Cryptography;

namespace space
{
    public class SpaceGame
    {

        private Bitmap explosionBitmap;
        private List<Explosion> _explosionList = new List<Explosion>();

        public SerialPort serialPort;

        private string bulletType;
        private SoundEffect _bulletSound;
        private SoundEffect _gameSound;
        private SoundEffect _laserSound;

        private Player _player;
        private Window _gameWindow;

        private Bullet _bullet = new Bullet();
        
        private List<Enemy> _enemyList = new List<Enemy>();
        private List<Enemy> _bossList = new List<Enemy>();
        private Enemy _enemy;

        private Bitmap bg;
        private Bitmap bg2;
        private Bitmap bg3;

        public bool eFlag = false;
        public bool bFlag = false;

        public static bool runleft = false;
        public static bool runright = false;
        public static bool shoot = false;

        int posX = 0;
        int posY = 0;
        int pos = -740;

        static int counter;
        static int counter2;
        static int counterboss2;

        public SpaceGame()
        {
            _gameWindow = new Window("Star Wars", 700, 600);
            Load();
            _player = new Player { X = _gameWindow.Width-70, Y = _gameWindow.Height-90};
            
            for(int i =0; i < 1; i++)
            {
                _enemyList.Add(new TroopEnemy { X = 0 + 100 * i, Y = 50 });
                _bossList.Add(new Boss { X = 0 + 100 * i, Y = 50 });
            }
        }
        private void Load()
        {
            SplashKit.LoadBitmap("Bullet", "Fire.png");
            SplashKit.LoadBitmap("Bullet2", "Laser.png");
            SplashKit.LoadBitmap("player", "player.png");
            SplashKit.LoadBitmap("1", "1.png");
            SplashKit.LoadBitmap("Boss", "boss.png");
            bg = SplashKit.LoadBitmap("BG", "bg4.jpg");
            bg2 = SplashKit.LoadBitmap("BG2", "bg5.jpg");
            bg3 = SplashKit.LoadBitmap("BG3", "bg3.jpg");

            explosionBitmap = SplashKit.LoadBitmap("Explosion", "explosion.png");
            _gameSound = SplashKit.LoadSoundEffect("Game", "space.wav");
            _bulletSound = SplashKit.LoadSoundEffect("Fireball", "bullet.wav");
            _laserSound = SplashKit.LoadSoundEffect("Laser", "laser.wav");



            serialPort = new SerialPort("COM4", 9600); // Create an object as a port to the electronic board

            try
            {
                serialPort.Open();
                Console.WriteLine("Serial port COM4 opened. Press any key to exit.");
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static void runL(bool check)
        {
            runleft = check;
        }


        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var data = serialPort.ReadExisting();

            var inputSimulator = new InputSimulator();

            foreach (char c in data)
            {
                switch (c)
                {
                    case 'L':
                        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.LEFT);
                        runleft = true;
                        break;
                    case 'R':
                        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RIGHT);
                        runright = true;
                        break;
                    case 'F':
                        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
                        shoot = true;
                        break;
                    case ' ':
                        // Handle other characters, if needed
                        runleft = false;
                        runright = false;
                        shoot = false;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Update()
        {
            serialPort.DataReceived += DataReceivedHandler;
            SplashKit.PlaySoundEffect("Game",10,0.6f);

            while (!_gameWindow.CloseRequested)
            {
                
                EnemyStayOnWindow(_gameWindow);
                BossStayOnWindow(_gameWindow);

                SplashKit.ProcessEvents();
                StayOnWindow(_gameWindow);

                //Moving by gamepad
                if (runleft == true)
                {
                    _player.Move(0, -10);
                    Console.WriteLine("Trai");
                }
                if (runright == true)
                {
                    _player.Move(0, 10);
                    Console.WriteLine("Phai");
                }

                if (shoot == true)
                {
                    _player.Shoot(bulletType);
                    SplashKit.PlaySoundEffect("Fireball", 0.5f);
                }

                //Moving by keyboard
                if (SplashKit.KeyDown(KeyCode.RightKey))
                {
                    _player.Move(0, 10);
                    
                }
                if (SplashKit.KeyDown(KeyCode.LeftKey))
                {
                    _player.Move(0, -10);
                }
                if (SplashKit.KeyDown(KeyCode.SpaceKey))
                {
                    _player.Shoot(bulletType);
                    SplashKit.PlaySoundEffect("Fireball", 0.3f);

                }


                if (eFlag)
                {
                        foreach(var item in _enemyList)
                        {
                            item.Move(0, 24 * SplashKit.Rnd());
                        }
                       
                }else
                {
                        foreach(var item in _enemyList)
                        {
                            item.Move(0, -24 * SplashKit.Rnd());
                        }
         
                }

                if (bFlag)
                {
                    foreach (var item in _bossList)
                    {
                        item.Move(0, 12 * SplashKit.Rnd());
                    }

                }
                else
                {
                    foreach (var item in _bossList)
                    {
                        item.Move(0, -12 * SplashKit.Rnd());
                    }

                }


                _player.Update();   
                    
                    if(_enemyList.Count < 1)
                    {
                        _enemyList.Add(new TroopEnemy { X = 0 + 50, Y = 50 });
                    }
                    foreach(var item in _enemyList)
                    {
                        item.Update();
                    }

                    foreach(var i in _bossList)
                    {
                        i.Update();
                    }
                    
                    Draw();

                    checkHit(_player);

                    checkHit(_enemyList);

                    checkHitBoss(_bossList);

               

                //Change the bullet
                if (SplashKit.KeyTyped(KeyCode.LKey))
                {
                    _bullet.SetBulletType("laser");
                }
                else if (SplashKit.KeyTyped(KeyCode.FKey))
                {
                    _bullet.SetBulletType("fireball");
                }

            }
                _gameWindow.Close();
                _gameWindow = null;
        }

        
        private void Draw()
        {
            posY = pos+200;

            SplashKitSDK.Timer t = new SplashKitSDK.Timer("BG Timer");
            t.Start();
            SplashKit.Delay(100);
            
            _gameWindow.Clear(Color.Black);
            if(t.Ticks/1000<1)
            {
                 pos+=2;
                 if(posY == -10){
                    pos = -740;
                    _gameWindow.DrawBitmap(bg2, posX, posY);

                }
            }
            _gameWindow.DrawBitmap(bg3, posX,posY);

            _player.Draw();

            foreach(var item in _enemyList)
            {
                item.Draw();
            }


            if (_player.score >= 10 && _bossList.Count == 0)
            {
                // Add a new boss to the _bossList if the player's score is 15 and there are no bosses on the screen
                _bossList.Add(new Boss { X = 350, Y = 50 });
              
            }

            // Draw the boss(es) from the _bossList
            foreach (var boss in _bossList)
            {
                boss.Draw();
            }
        

            foreach (var explosion in _explosionList.ToList())
            {
                if (!explosion.AnimationEnded)
                {
                    explosion.Update();
                    _gameWindow.DrawBitmap(explosionBitmap, explosion.X, explosion.Y);
                }
                else
                {
                    // Remove the explosion from the list once the animation ends
                    _explosionList.Remove(explosion);
                }
            }

            _gameWindow.Refresh(60);
        }

        // Explosion with properties and methods for the animation
        public class Explosion
        {
            private int frameIndex;
            private int frameDelay = 1;
            private int frameCount = 5;
            private int frameWidth;
            private int frameHeight;
            private int ticks;

            public double X { get; private set; }
            public double Y { get; private set; }
            public bool AnimationEnded { get; private set; }

            public Explosion(double x, double y)
            {
                X = x;
                Y = y;
                frameIndex = 0;
                ticks = 0;

                // Update frameWidth and frameHeight with the actual size of the explosion bitmap
                frameWidth = 90;
                frameHeight = 100;

                AnimationEnded = false;
            }

            public void Update()
            {
                ticks++;
                if (ticks >= frameDelay)
                {
                    ticks = 0;
                    frameIndex++;

                    if (frameIndex >= frameCount)
                    {
                        frameIndex = 0;
                        AnimationEnded = true;
                    }
                }
            }
        }


        public void StayOnWindow(Window w)
        {
            const double gap = 10;//the distance between 2 sides of windows and the player
            if(_player.X < gap){
                _player.X = gap;
            }else if( _player.X > w.Width - 90){
                _player.X = w.Width - 90; 
            }
        }

        public void EnemyStayOnWindow(Window w)
        {
            const double gap = 10;
         
            foreach (var item in _enemyList)
            {
                if(item.X < gap)
                {
                    item.X = gap;
                    eFlag = false;
                    
                    for(int i =0; i< _enemyList.Count; i++)
                    _enemyList.ElementAt(i).Shoot(bulletType);
                    SplashKit.PlaySoundEffect("Laser", 0.6f);


                }
                else if( item.X > w.Width - 90)
                {
                    item.X = w.Width - 90;
                    eFlag = true;
                    
                    for(int i = 0; i< _enemyList.Count; i++)
                    _enemyList.ElementAt(i).Shoot(bulletType);
                    SplashKit.PlaySoundEffect("Laser", 0.6f);


                }
            }
            
        }


        public void BossStayOnWindow(Window w)
        {
            const double gap = 10;

            foreach (var item in _bossList)
            {
                if(item.X < gap)
                {
                    item.X = gap;
                    bFlag = false;
                    //_enemy.Shoot();
                    for(int i =0; i< _bossList.Count; i++)
                    _bossList.ElementAt(i).Shoot(bulletType);
                
                }else if( item.X > w.Width - 90)
                {
                    item.X = w.Width - 90;
                    bFlag = true;
                    //_enemy.Shoot();
                    for(int i = 0; i< _bossList.Count; i++)
                    _bossList.ElementAt(i).Shoot(bulletType);
                
                }
            }
            
        }


        private void checkHit(Player player)
        {
            bool playerHit = false;
            int lifeGone;
           
            for(int i = 0; i<_enemyList.Count; i++)
            {
                foreach (var item in _enemyList[i]._bulletList)
                {
                    if (item.CollidedWith(_player, item))
                    {
                        playerHit = true;
                        break;
                    }
                }

                foreach (var item in _bossList[i]._bulletList)
                {
                    if (item.CollidedWith(_player, item))
                    {
                        playerHit = true;
                        break;
                    }
                }
            }
            
            if(playerHit)
            {
                playerHit = false;
                counter ++;
                if(counter > 1)
                {
                    lifeGone = _player._lifeCount--;
                    counter = 0;
                if(lifeGone==1)
                {
                   SplashKit.CloseCurrentWindow();
                }else
                {
                    _player.RemoveLife(lifeGone);
                }
                }
            }
        }

        private void checkHit(List<Enemy> enemy)
        {
            List<Enemy> RemoveEnemy = new List<Enemy>();
            bool enemyHit = false;
           
            foreach (var item2 in _enemyList)
            {
            
                foreach (var item in _player._bulletList)
                {
                    if(item.CollidedWith(item2, item))
                    {
                        enemyHit = true;
                        _player.score++;
                        RemoveEnemy.Add(item2);

                        // Create an explosion animation when bullet hits an enemy
                        _explosionList.Add(new Explosion(item2.X, item2.Y));

                        break;
                    }
                }
            }

            if(enemyHit)
            {
                enemyHit = false;
                counter2 ++;
                if(counter2 > 3)
                {
                    _player.score++;
                    counter2 = 0;
                }
            }
            
            foreach(var item in RemoveEnemy)
            {
                enemy.Remove(item);
            }
        }

        private void AddBoss()
        {
            
            foreach(var item in _bossList)
            {
                item.Draw();
            }
        }



        private void checkHitBoss(List<Enemy> boss)
        {
            List<Enemy> RemoveBoss = new List<Enemy>();
            bool bossHit = false;
 
            foreach (var item2 in _bossList)
            {
                
                foreach (var item in _player._bulletList)
                {
                    if (item.CollidedWith(item2, item))
                    {
                        bossHit = true;
                        _player.score+=10;
                        RemoveBoss.Add(item2);

                        break;
                    }
                }
            }

            if(bossHit)
            {
                bossHit = false;
                counterboss2 ++;
                if(counterboss2 > 3)
                {
                    _player.score+=10;
                    counterboss2 = 0;
                
                }
            }
            
            foreach(var i in RemoveBoss)
            {
                _bossList.Remove(i);
            }
        }
    }
    
}