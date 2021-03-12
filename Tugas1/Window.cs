using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using LearnOpenTK.Common;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Timers;

namespace Tugas1
{
    class Window : GameWindow
    {
        //jarum jam
        private float[] _vertices_jam =
        {
            0.0f, 0.0f, 0.0f,
            0.0f, 0.15f, 0.0f
        };
        private int _vertexBufferObject_jam;
        private int _vertexArrayObject_jam;
        private Shader _shader_jam;

        //jarum menit
        private float[] _vertices_menit =
        {
            0.0f, 0.0f, 0.0f,
            0.0f, 0.2f, 0.0f
        };
        private int _vertexBufferObject_menit;
        private int _vertexArrayObject_menit;
        private Shader _shader_menit;

        //jarum detik
        private float[] _vertices_detik =
        {
            0.0f, 0.0f, 0.0f,
            0.0f, 0.3f, 0.0f
        };
        private int _vertexBufferObject_detik;
        private int _vertexArrayObject_detik;
        private Shader _shader_detik;

        //lingkaran (clock)
        private float[] _vertices_clock = new float[1080];

        private int _vertexBufferObject_clock;
        private int _vertexArrayObject_clock;
        private Shader _shader_clock;

        private int count_menit = 0;
        private int count_detik = 0;

        ////square
        //private readonly float[] _vertices_square =
        //{
        //    0.05f,0.05f,0.0f, //indeks 0
        //    0.05f,-0.05f,0.0f, //indeks 1
        //    -0.05f,-0.05f,0.0f, //indeks 2
        //    -0.05f,0.05f,0.0f //indeks 3
        //};
        //private readonly uint[] _indeces =
        //{
        //    0,1,3,
        //    1,2,3
        //};

        //private int _vertexBufferObject_square;
        //private int _vertexArrayObject_square;
        //private Shader _shader_square;
        //private int _elementBufferObject_square;


        //mouse
        int mouse_click = 0;
        private float[] _vertices_line = new float[500];
        private int _vertexBufferObject_line;
        private int _vertexArrayObject_line;
        private Shader _shader_line;

        private int point_count = 0;

        //keyboard
        bool isInvisible = false;

        //bezier curve
        private float[] _vertices_bezier = new float[1080];
        private int _vertexBufferObject_bezier;
        private int _vertexArrayObject_bezier;
        private Shader _shader_bezier;
        private int _vertice_bezier_count = 0;
        private Vector2[] _vertices_bezier_temporary = new Vector2[1080];
        private int calculate = 0;
        private int n = 2;

        //timer
        private int interval = 1000;

        Vector2 setBezier2(Vector2 p1, Vector2 p2, float t)
        {
            Vector2 p;
            float a1 = 1 - t;
            float a2 = t;

            p.X = a1 * p1.X + a2 * p2.X;
            p.Y = a1 * p1.Y + a2 * p2.Y;
            return p;
        }

        Vector2 setBezier3(Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 p;
            float a1 = (float)Math.Pow((1 - t),2);
            float a2 = 2*(1-t)*t;
            float a3 = t * t;

            p.X = a1 * p1.X + a2 * p2.X + a3 * p3.X;
            p.Y = a1 * p1.Y + a2 * p2.Y + a3 * p3.Y;
            return p;
        }

        Vector2 setBezier4(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t)
        {
            Vector2 p;
            float a1 = (float)Math.Pow((1 - t), 3);
            float a2 = (float)Math.Pow((1 - t), 2) * 3 * t;
            float a3 = 3 * t * t * (1 - t);
            float a4 = t * t * t;
            p.X = a1 * p1.X + a2 * p2.X + a3 * p3.X + a4 * p4.X;
            p.Y = a1 * p1.Y + a2 * p2.Y + a3 * p3.Y + a4 * p4.Y;
            return p;
        }

        protected float[] rotate(float x, float y)
        {
            float deg = -0.105f;
            float[] p = {
            (float)((x * Math.Cos(deg)) - (y*(Math.Sin(deg)))),
            (float)(x * Math.Sin(deg) + (y * (Math.Cos(deg))))
            };
            return p;
        }

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            float x = 0.0f;
            float y = 0.0f;

            //radius lingkaran
            float radius = 0.5f;

            ////radius elips
            //float radius_x = 0.1f;
            //float radius_y = 0.2f;

            for (int i = 0; i < 360; i++)
            {
                //rumus derajat ke radian = derajat * 3.1416f / 180;
                float degInRad = i * 3.1416f / 180;

                //x
                //lingkaran
                _vertices_clock[i * 3] = x + (float)Math.Cos(degInRad) * radius;
                //elips
                //_vertices[i * 3] = x + (float)Math.Cos(degInRad) * radius_x;

                //y
                _vertices_clock[i * 3 + 1] = y + (float)Math.Sin(degInRad) * radius;
                //elips
                //_vertices[i * 3 + 1] = y + (float)Math.Sin(degInRad) * radius_y;

                //z
                _vertices_clock[i * 3 + 2] = 0;
            }

        }

        protected override void OnLoad()
        {
            Console.WriteLine("Titik kontrol: " + n);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            //clock
            _vertexBufferObject_clock = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_clock);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_clock.Length * sizeof(float), _vertices_clock, BufferUsageHint.StaticDraw);

            _vertexArrayObject_clock = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_clock);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader_clock = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock.frag");
            _shader_clock.Use();

            //jarum detik
            _vertexBufferObject_detik = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_detik);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_detik.Length * sizeof(float), _vertices_detik, BufferUsageHint.StaticDraw);

            _vertexArrayObject_detik = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_detik);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader_detik = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock_needle.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock_needle.frag");
            _shader_detik.Use();

            //jarum menit
            _vertexBufferObject_menit = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_menit);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_menit.Length * sizeof(float), _vertices_menit, BufferUsageHint.StaticDraw);

            _vertexArrayObject_menit = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_menit);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader_menit = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock_needle.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock_needle.frag");
            _shader_menit.Use();

            //jarum jam
            _vertexBufferObject_jam = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_jam);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_jam.Length * sizeof(float), _vertices_jam, BufferUsageHint.StaticDraw);

            _vertexArrayObject_jam = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_jam);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader_jam = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock_needle.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader_clock_needle.frag");
            _shader_jam.Use();

            ////square
            //_vertexBufferObject_square = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_square);
            //GL.BufferData(BufferTarget.ArrayBuffer, _vertices_square.Length * sizeof(float), _vertices_square, BufferUsageHint.StaticDraw);

            //_vertexArrayObject_square = GL.GenVertexArray();
            //GL.BindVertexArray(_vertexArrayObject_square);
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(0);

            ////inisialisasi element buffer object
            //_elementBufferObject_square = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject_square);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, _indeces.Length * sizeof(uint), _indeces, BufferUsageHint.StaticDraw);

            //_shader_square = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader.frag");
            //_shader_square.Use();


            //inisialisasi mouse line
            _vertexBufferObject_line = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_line);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_line.Length * sizeof(float), _vertices_line, BufferUsageHint.StaticDraw);

            _vertexArrayObject_line = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_line);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader_line = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader.frag");
            _shader_line.Use();

            //inisialisasi bezier
            _vertexBufferObject_bezier = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bezier);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_bezier.Length * sizeof(float), _vertices_bezier, BufferUsageHint.StaticDraw);

            _vertexArrayObject_bezier = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_bezier);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader_bezier = new Shader("E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader.vert", "E:/Kuliah/Grafkom/Tugas1/Tugas1/Tugas1/Shaders/shader.frag");
            _shader_bezier.Use();

            base.OnLoad();
        }

        protected void updateClock()
        {
            if(count_detik < 60)
            {
                float[] temp = rotate(_vertices_detik[3], _vertices_detik[4]);
                _vertices_detik[3] = temp[0];
                _vertices_detik[4] = temp[1];
                count_detik++;

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_detik);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices_detik.Length * sizeof(float), _vertices_detik, BufferUsageHint.StaticDraw);
                GL.BindVertexArray(_vertexArrayObject_detik);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
            }
            else
            {
                if (count_menit < 60)
                {
                    float[] temp = rotate(_vertices_menit[3], _vertices_menit[4]);
                    _vertices_menit[3] = temp[0];
                    _vertices_menit[4] = temp[1];
                    count_menit++;

                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_menit);
                    GL.BufferData(BufferTarget.ArrayBuffer, _vertices_menit.Length * sizeof(float), _vertices_menit, BufferUsageHint.StaticDraw);
                    GL.BindVertexArray(_vertexArrayObject_menit);
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                    GL.EnableVertexAttribArray(0);
                    count_detik = 0;
                }
                else
                {
                    float[] temp = rotate(_vertices_jam[3], _vertices_jam[4]);
                    _vertices_jam[3] = temp[0];
                    _vertices_jam[4] = temp[1];

                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_jam);
                    GL.BufferData(BufferTarget.ArrayBuffer, _vertices_jam.Length * sizeof(float), _vertices_jam, BufferUsageHint.StaticDraw);
                    GL.BindVertexArray(_vertexArrayObject_jam);
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                    GL.EnableVertexAttribArray(0);
                    count_menit = 0;
                }
            }
        }

        protected void MousePointUpdate()
        {
            //update titik kontrol
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_line);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_line.Length * sizeof(float), _vertices_line, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vertexArrayObject_line);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //update garis bezier
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bezier);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices_bezier.Length * sizeof(float), _vertices_bezier, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vertexArrayObject_bezier);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            updateClock();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            //tengah sini berikan apa yang ingin kita gambar

            ////square
            //_shader_square.Use();
            //GL.BindVertexArray(_vertexArrayObject_square);
            //GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);
            //GL.DrawElements(PrimitiveType.LineStrip, 3, DrawElementsType.UnsignedInt, 3 * sizeof(uint));

            ////garis
            //GL.DrawArrays(PrimitiveType.LineStrip, 0, 2);

            ////segitiga
            //GL.DrawArrays(PrimitiveType.TriangleFan, 0, 3);

            ////lingkaran penuh
            //_shader.Use();
            //GL.BindVertexArray(_vertexArrayObject);
            //GL.DrawArrays(PrimitiveType.TriangleFan, 0, 360);

            //lingkaran outline
            _shader_clock.Use();
            GL.BindVertexArray(_vertexArrayObject_clock);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, 360);

            //jarum detik
            _shader_detik.Use();
            GL.BindVertexArray(_vertexArrayObject_detik);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, 2);

            //jarum menit
            _shader_menit.Use();
            GL.BindVertexArray(_vertexArrayObject_menit);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, 2);

            //jarum jam
            _shader_jam.Use();
            GL.BindVertexArray(_vertexArrayObject_jam);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, 2);

            //keyboard
            if (isInvisible == false)
            {
                //mouse line
                if (mouse_click > 1)
                {
                    //MousePointUpdate();
                    //GL.BindVertexArray(_vertexArrayObject_line);
                    //GL.DrawArrays(PrimitiveType.LineStrip, 0, mouse_click);
                }
            }

            //if(point_count > 2) {
            //    int i = 0;
            //    for (int j = 0; j < point_count; j++)
            //    {
            //        _vertices_bezier[i] = _vertices_line[j];
            //        i++;
            //        _vertices_bezier[i] = _vertices_line[j+1];
            //        i++;
            //        _vertices_bezier[i] = 0;
            //        _vertice_bezier_count++;
            //        for (float t = 0.0f; t <= 1.0f; t += 0.01f)
            //        {
            //            Vector2 P = setBezier3(_vertices_bezier_temporary[j], _vertices_bezier_temporary[j+1], _vertices_bezier_temporary[j + 2], t);
            //            _vertices_bezier[i] = P.X;
            //            i++;
            //            _vertices_bezier[i] = P.Y;
            //            i++;
            //            _vertices_bezier[i] = 0;
            //            i++;
            //        }

            //    }
            if (point_count == n)
            {
                if (calculate == 0)
                {
                    int i = 0;

                    //vertex x
                    _vertices_bezier[i] = _vertices_line[0];
                    i++;
                    //vertex y
                    _vertices_bezier[i] = _vertices_line[1];
                    i++;
                    //vertex z
                    _vertices_bezier[i] = 0;
                    i++;
                    //t
                    _vertice_bezier_count++;
                    for (float t = 0.0f; t <= 1.0f; t += 0.01f)
                    {
                        if (n == 2)
                        {
                            Vector2 P = setBezier2(_vertices_bezier_temporary[0], _vertices_bezier_temporary[1], t);
                            _vertices_bezier[i] = P.X;
                            i++;
                            _vertices_bezier[i] = P.Y;
                            i++;
                            _vertices_bezier[i] = 0;
                            i++;
                        }
                        else if (n == 3)
                        {
                            Console.WriteLine("mouse click: " + mouse_click);
                            Vector2 P = setBezier3(_vertices_bezier_temporary[0], _vertices_bezier_temporary[1], _vertices_bezier_temporary[2], t);
                            _vertices_bezier[i] = P.X;
                            i++;
                            _vertices_bezier[i] = P.Y;
                            i++;
                            _vertices_bezier[i] = 0;
                            i++;
                        }
                        else if (n == 4)
                        {
                            Vector2 P = setBezier4(_vertices_bezier_temporary[0], _vertices_bezier_temporary[1], _vertices_bezier_temporary[2], _vertices_bezier_temporary[3], t);
                            _vertices_bezier[i] = P.X;
                            i++;
                            _vertices_bezier[i] = P.Y;
                            i++;
                            _vertices_bezier[i] = 0;
                            i++;
                        }
                        _vertice_bezier_count++;
                    }
                    calculate = 1;
                }

            }
            MousePointUpdate();
            _shader_bezier.Use();
            GL.BindVertexArray(_vertexArrayObject_bezier);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertice_bezier_count);

            SwapBuffers();
            base.OnRenderFrame(args);

            System.Threading.Thread.Sleep(interval);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyReleased(Keys.V))
            {
                isInvisible = !isInvisible;
                interval = 0;
            }
            if (KeyboardState.IsKeyReleased(Keys.R))
            {
                Console.WriteLine("Reset");
                point_count = 0;
                mouse_click = 0;
                _vertices_line = new float[500];
                _vertice_bezier_count = 0;
                calculate = 0;
                interval = 1000;
            }
            if (KeyboardState.IsKeyReleased(Keys.W))
            {
                if(n >= 2 && n < 4) 
                {
                    n++;
                    point_count = 0;
                    mouse_click = 0;
                    _vertices_line = new float[500];
                    _vertice_bezier_count = 0;
                    calculate = 0;
                    Console.WriteLine("Titik kontrol: " + n);
                }
            }
            if (KeyboardState.IsKeyReleased(Keys.S))
            {
                if (n <= 4 && n > 2) 
                {
                    n--;
                    point_count = 0;
                    mouse_click = 0;
                    _vertices_line = new float[500];
                    _vertice_bezier_count = 0;
                    calculate = 0;
                    Console.WriteLine("Titik kontrol: " + n);
                }
            }
            if (KeyboardState.IsKeyReleased(Keys.D))
            {
                interval += 10;
                Console.WriteLine("Interval: " + interval);
            }
            if (KeyboardState.IsKeyReleased(Keys.A))
            {
                interval -= 10;
                Console.WriteLine("Interval: " + interval);
            }
            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                //Console.WriteLine("Klik kiri");

                if(point_count %3 != 0 || point_count == 0)
                {
                    //position
                    float posX = (MousePosition.X - Size.X / 2f) / (Size.X / 2f);
                    float posY = -(MousePosition.Y - Size.Y / 2f) / (Size.Y / 2f);
                    Console.WriteLine("x: " + posX + " y: " + posY);
                    _vertices_bezier_temporary[mouse_click].X = posX;
                    _vertices_bezier_temporary[mouse_click].Y = posY;

                    _vertices_line[mouse_click * 3] = posX;
                    _vertices_line[mouse_click * 3 + 1] = posY;
                    _vertices_line[mouse_click * 3 + 2] = 0;
                    mouse_click++;
                    point_count++;
                }
            }
            //else
            //{
            //    Console.WriteLine("Klik kanan");
            //}
            
            base.OnMouseDown(e);
        }
    }
}
