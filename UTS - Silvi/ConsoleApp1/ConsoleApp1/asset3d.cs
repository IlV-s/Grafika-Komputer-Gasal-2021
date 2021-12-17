using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ConsoleApp1
{
    class asset3d
    {

        List<Vector3> _vertices = new List<Vector3>();
        List<Vector3> _textureVertices = new List<Vector3>();
        List<Vector3> _normal = new List<Vector3>();

        float pi = 3.14159f;
        List<uint> _indices = new List<uint>();
        int _elementBufferObject;

        List<Vector3> _vertices_bezier_control = new List<Vector3>();
        int _vertexBufferObject;//menghandle vertices untuk ke graphic card
        int _vertexArrayObject; //bagian openGL yang ngurus array vertex yang dikirim
        Shader _shader;

        Matrix4 transform = Matrix4.Identity;
        /// <summary>
        /// 1 0 0 0
        /// 0 1 0 0
        /// 0 0 1 0
        /// 0 0 0 1
        /// </summary>

        public Vector3 _centerPosition = new Vector3(0, 0, 0);
        public List<Vector3> _euler = new List<Vector3>();
        Matrix4 _view;
        Matrix4 _projection;
        int[] _pascal = { };
        int index = 0;
        public List<asset3d> Child = new List<asset3d>();
        public asset3d()
        {
            //Xumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //Sumbu Y
            _euler.Add(new Vector3(0, 1, 0));
            //Sumbu Z
            _euler.Add(new Vector3(0, 0, 1));
        }

        //Untuk ngeload vertex" e
        public void load()
        {
            //create buffer (buat ngirim)
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            //ngirim array vertex melalui buffer ke graphic card
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

            //setelah ngirim, ini buat ngatur dia masuk mau dimana
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            ////parameter 1 -> lokasi index input yang ada di shader
            ////parameter 2 -? jumlah elemen yang dikirimkan, kalau contoh project 3 float untuk tiap vertex
            ////paramater 3 -> tipe data yang kita kirimkan berjenis apa
            ////parameter 4 -> apakah perlu dinormalisasi
            ////parameter 5 -> berapa banyak jumlah vertex yang kita kirim dari variabel _vertices. Dalam kasus project ini ada 3 vertex * sizeof(float)
            ////parameter 6 -> vertex mau mulai dari index ke berapa
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //tetep float soalnya vertice.toarray nya tetep range e 0-1
            //Untuk menyalakan/mengaktifkan
            GL.EnableVertexAttribArray(0);



            if (_indices.Count != 0)
            {
                //settingan EAO
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader("D:/- Folder Kuliah/7/Grafika Komputer/Project/ConsoleApp1/ConsoleApp1/Shader/shader.vert", "D:/- Folder Kuliah/7/Grafika Komputer/Project/ConsoleApp1/ConsoleApp1/Shader/shader.frag");
            _shader.Use();
        }


        public void loadKelasPlusProjection(int size_x, int size_y, string shaderColor)//size x size y iku size windows e kita
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            if (_indices.Count != 0)
            {
                //settingan EAO
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader("D:/- Folder Kuliah/7/Grafika Komputer/Project/ConsoleApp1/ConsoleApp1/Shader/shader.vert", "D:/- Folder Kuliah/7/Grafika Komputer/Project/ConsoleApp1/ConsoleApp1/Shader/" + shaderColor);
            _shader.Use();
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), size_x / (float)size_y, 0.1f, 100.0f);

            foreach(var item in Child)
            {
                item.loadKelasPlusProjection(size_x, size_y, shaderColor);
            }
        }
        public void render(int _lines)
        {
            //harus di atas biar walaupun frame 1 dia sudah jalan
            //transform = transform * Matrix4.CreateTranslation(0.02f, 0f, 0f);
            //transform = transform * Matrix4.CreateScale(1.1f);
            //transform = transform * Matrix4.CreateRotationX(0.01f);
            //transform = transform * Matrix4.CreateRotationY(0.01f);
            //step menggambar sebuah object
            //1. enable shader
            _shader.Use();
            //_shader.SetMatrix4("transform", transform);

            //main" shader tambahan buat dikirim ke shader dari windows.cs (uniform)
            //buat nyari nama variabel yang mau dipake
            /*int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uniformColor");
            GL.Uniform4(vertexColorLocation, 0.0f,0.0f,1.0f,1.0f);*/

            //2. panggil bind Vertex Array Object
            GL.BindVertexArray(_vertexArrayObject);
            //3. panggil fungsi untuk menggambar

            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (_lines == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count); //0 untuk index, angka 3 itu jumlah vertex
                }
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
                else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertices.Count);
                }
                else if (_lines == 3)
                {
                    //lingkaran tanpa isi
                    //GL.DrawArrays(PrimitiveType.LineLoop, 0, (_vertices.Length + 1) / 3);
                    //lingkaran dengan isi
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
            }
        }
        public void renderKelas(int _lines)
        {
            //transform = transform * Matrix4.CreateTranslation(0.2f, 0.1f, 0f);
            //transform = transform * Matrix4.CreateScale(1.1f);
            //transform = transform * Matrix4.CreateRotationX(0.1f);
            //transform = transform * Matrix4.CreateRotationY(0.01f);
            //1. enable shader
            _shader.Use();
            _shader.SetMatrix4("transform", transform);
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);
            /*int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uniformColor");
            GL.Uniform4(vertexColorLocation, 0.0f,0.0f,1.0f,1.0f);*/
            GL.BindVertexArray(_vertexArrayObject);

            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (_lines == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
                else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertices.Count);
                }
                else if (_lines == 3)
                {
                    //lingkaran tanpa isi
                    //GL.DrawArrays(PrimitiveType.LineLoop, 0, (_vertices.Length + 1) / 3);
                    //lingkaran dengan isi
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
            }
            foreach(var item in Child)
            {
                item.renderKelas(_lines);
            }
        }
        public void render(Camera cam, int Render_Type = 0)
        {
            _shader.Use();
            _shader.SetMatrix4("transform", transform);

            _shader.SetMatrix4("view", cam.GetViewMatrix());
            _shader.SetMatrix4("projection", cam.GetProjectionMatrix());

            GL.BindVertexArray(_vertexArrayObject);
            if (Render_Type == 0)
            {
                GL.DrawElements(PrimitiveType.Triangles,
                _vertices.Count,
                DrawElementsType.UnsignedInt, 0);
            }
            else if (Render_Type == 1)//point
            {
                GL.DrawElements(PrimitiveType.Points,
                _vertices.Count,
                DrawElementsType.UnsignedInt, 0);
            }
            else if (Render_Type == 2)//wireframe
            {
                GL.DrawElements(PrimitiveType.Lines,
                _vertices.Count,
                DrawElementsType.UnsignedInt, 0);
            }
            else if (Render_Type == 3)// true wireframe
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.DrawElements(PrimitiveType.Triangles,
                _vertices.Count,
                DrawElementsType.UnsignedInt, 0);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else if (Render_Type == 4)//testing
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.DrawElements(PrimitiveType.TriangleFan,
                _vertices.Count,
                DrawElementsType.UnsignedInt, 0);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else if(Render_Type == 5)//testing
            {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
            }
            else if(Render_Type == 6)
            {
                GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
            }
        }

        public void setVertices(List<Vector3> vertices)
        {
            _vertices = vertices;
        }
        public bool getVerticesLength()
        {
            if (_vertices.Count == 0)
            {
                return false;
            }
            if (_vertices.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void createTriangle()
        {
            Vector3 v1, v2, v3;

            v1.X = -0.5f;
            v1.Y = -0.5f;
            v1.Z = 0.0f;

            v2.X = 0.5f;
            v2.Y = -0.5f;
            v2.Z = 0.0f;

            v3.X = 0.0f;
            v3.Y = 0.5f;
            v3.Z = 0.0f;

            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);

            _indices = new List<uint>
            {
                0, 1, 2, 0
            };
        }
        public void createRectangle(float x, float y, float z, float length)
        {

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            //dari pada nulis new vector di add tlis dulu disini
            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //segitiga depan pertama
                0,1,2,
                //segitiga depan 2
                1,2,3
            };
        }

        public void createCircle(float center_x, float center_y, float radius)
        {

            //looping 360 titik buat lingkaran
            for (int i = 0; i < 360; i++)
            {
                //buat nyari alpha yang akan dikaliakn pada rumus lingkaran buat mencari titk x,y
                //(x,y) = (r cos alpha, r sin alpha)
                float degInRad = i * pi / 180;
                Vector3 temp_vector;
                //nyimpen x
                temp_vector.X = (float)Math.Cos(degInRad) * radius + center_x;
                //nyimpen y
                temp_vector.Y = (float)Math.Sin(degInRad) * radius + center_y;
                //nyimpen x
                temp_vector.Z = 0;
                _vertices.Add(temp_vector);
            }
        }
        public void createCircleForEyes(float center_x, float center_y, float radius)
        {

            //looping 360 titik buat lingkaran
            for (int i = 0; i < 360; i++)
            {
                //buat nyari alpha yang akan dikaliakn pada rumus lingkaran buat mencari titk x,y
                //(x,y) = (r cos alpha, r sin alpha)
                float degInRad = i * pi / 180;
                Vector3 temp_vector;
                //nyimpen x
                temp_vector.X = (float)Math.Cos(degInRad) * radius * (radius*(float)6.5f) + center_x;
                //nyimpen y
                temp_vector.Y = (float)Math.Sin(degInRad) * radius + center_y;
                //nyimpen x
                temp_vector.Z = 0;
                _vertices.Add(temp_vector);
            }
        }
        public void createCircleForEyesDalem(float center_x, float center_y, float radius)
        {

            //looping 360 titik buat lingkaran
            for (int i = 0; i < 360; i++)
            {
                //buat nyari alpha yang akan dikaliakn pada rumus lingkaran buat mencari titk x,y
                //(x,y) = (r cos alpha, r sin alpha)
                float degInRad = i * pi / 180;
                Vector3 temp_vector;
                //nyimpen x
                temp_vector.X = (float)Math.Cos(degInRad) * radius * (radius * (float)8f) + center_x;
                //nyimpen y
                temp_vector.Y = (float)Math.Sin(degInRad) * radius + center_y;
                //nyimpen x
                temp_vector.Z = 0;
                _vertices.Add(temp_vector);
            }
        }
        //public void createBezierVertices(List<Vector2> point, float radiusX = 0, float radiusY = 0, float radiusZ = 0,float _x, float _y, float _z)
        //{
        //    Vector3 temp_vector;
        //    for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        //    {
        //        Vector2 P = setBezier(point, i);
        //        temp_vector.X = _x + P.X;
        //        temp_vector.Y = _y + P.Y;
        //        temp_vector.Z = _z;
        //        if (radiusX != 0 && radiusY != 0)
        //            temp_vector.Z += ((float)Math.Sin(-pi * (temp_vector.X + radiusX) / (radiusX * 2)) * radiusZ + (float)Math.Sin(-pi * (temp_vector.Y + radiusY) / (radiusY * 2)) * radiusZ) / 2.0f;

        //        _vertices.Add(temp_vector);
        //    }
        //}
        public void createBoxVertices(float x, float y, float z, float length)
        {

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            //dari pada nulis new vector di add tlis dulu disini
            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //titik belakang
            //Titik 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //segitiga depan pertama
                0,1,2,
                //segitiga depan 2
                1,2,3,
                //segitiga atas 1
                0,4,5,
                //segitiga atas 2
                0,1,5,
                //segitiga kanan 1
                1,3,5,
                //segitiga kanan 2
                3,5,7,
                //segitiga kiri 1
                0,2,4,
                //segitiga kiri 2
                2,4,6,
                //segitiga belakang 1
                4,5,6,
                //segitiga belakanag 2
                5,6,7,
                //segitiga bawah 1
                2,3,6,
                //segitiga bawah 2
                3,6,7
            };
        }
        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = -pi / 2; v < pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;

                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createHalfEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            Vector3 temp_vector;

            for (float u = -pi; u <= 0; u += pi / 300)
            {
                for (float v = -pi / 2; v < pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;

                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createEllipsoidWithSurface(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = pi / stackCount;
            float sectorAngle, stackAngle, x, y, z;
            for(int i = 0; i<= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(stackAngle);
                y = radiusY * (float)Math.Cos(stackAngle);
                z = radiusZ * (float)Math.Sin(stackAngle);
                for(int j = 0; j<= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }
            uint k1, k2;
            for (int i =0; i< stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2){
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if(i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }

                }
            }
        }

        public void createHyperboloidSatuSisi(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;

            Vector3 temp_vector;
            for (float u = -pi; u<= pi; u += pi / 30){
                for(float v = -pi / 2; v <= pi /2; v += pi / 30)
                {
                    temp_vector.Z = _x + (1 / (float)Math.Cos(v)) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (1 / (float)Math.Cos(v)) * (float)Math.Sin(u) * radiusY;
                    temp_vector.X = _z + (float)Math.Tan(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createHyperboloidDuaSisi(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;

            Vector3 temp_vector;
            for (float u = -pi / 2; u <= pi / 2; u += pi / 30)
            {
                for(float v = -pi; v <= pi; v += pi / 30)
                {
                    temp_vector.Z = _x + (float)Math.Tan(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Tan(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.X = _z + (1.0f / (float)Math.Cos(v)) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
            for (float u = -pi / 2; u <= 3 * pi / 2; u += pi / 30)
            {
                for (float v = -pi; v <= pi; v += pi / 30)
                {
                    temp_vector.Z = _x + (float)Math.Tan(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Tan(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.X = _z + (1.0f / (float)Math.Cos(v)) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createEllipticCone(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for(float u = -pi; u <= pi; u += pi / 30)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 30)
                {
                    temp_vector.Z = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.X = _z + v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createEllipticParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 720)
            {
                for (float v = -pi; v <= pi; v += pi / 720)
                {
                    temp_vector.Z = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.X = _z + v * v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createHyperboloidParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 30)
            {
                for (float v = 0; v <= 100; v++)
                {
                    temp_vector.Z = _x + v * (float)Math.Tan(u) * radiusX;
                    temp_vector.Y = _y + v * (1.0f /(float)Math.Cos(u)) * radiusY;
                    temp_vector.X = _z + v * v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createCylinder(float radX, float radZ, float _x, float _y, float _z, float height)
        {
            Vector3 temp_vector;
            for (float i = 0; i <= 360; i += 0.01f)
            {
                float degInRad = i * pi / 180;

                temp_vector.X = _x + (float)Math.Cos(degInRad) * radX;
                temp_vector.Y = _y + height / 2.0f;
                temp_vector.Z = _z + (float)Math.Sin(degInRad) * radZ;
                _vertices.Add(temp_vector);

                temp_vector.X = _x + (float)Math.Cos(degInRad) * radX;
                temp_vector.Y = _y - height / 2.0f;
                temp_vector.Z = _z + (float)Math.Sin(degInRad) * radZ;
                _vertices.Add(temp_vector);
            }
        }
        public void loadObjFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Unable to open");
            }

            using(StreamReader streamReader = new StreamReader(path))
            {
                while (!streamReader.EndOfStream)
                {
                    List<string> words = new List<string>(streamReader.ReadLine().ToLower().Split(" "));


                    words.RemoveAll(s => s == string.Empty);
                    if(words.Count == 0)
                    {
                        continue;
                    }
                    string type = words[0];
                    words.RemoveAt(0);

                    switch (type)
                    {
                        case "v":
                            _vertices.Add(new Vector3(float.Parse(words[0])/10, float.Parse(words[1])/10, float.Parse(words[2])/10));
                            break;

                        case "vt":
                            _textureVertices.Add(new Vector3(float.Parse(words[0]), float.Parse(words[1]), words.Count < 3 ? 0 : float.Parse(words[2])));
                            break;
                        case "vn":
                            _normal.Add(new Vector3(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2])));
                            break;

                        case "f":
                            foreach(string w in words)
                            {
                                //Console.WriteLine(w);
                                //int count = 0;
                                //Console.WriteLine("Comps 0("+ count +")= " + comps[0]);
                                //Console.WriteLine("Comps 1(" + count + ")= " + comps[1]);
                                //Console.WriteLine("Comps 2(" + count + ")= " + comps[2]);
                                if (w.Length == 0)
                                {
                                    continue;
                                }
                                string[] comps = w.Split("/");
                                //count++;
                                _indices.Add(uint.Parse(comps[0]) - 1);
                                //_indices.Add(uint.Parse(comps[1]) - 1);
                                //_indices.Add(uint.Parse(comps[2]) - 1);

                            };
                            break;
                    }

                }
            }
        }


      //  public void rotate(Vector3 pivot, Vector3 vector, float angle)
      //  {
      //      //pivot buat rotate di titik mana
      //      //vector rotate di sumbu apa (choose x,y,z)
      //      //angle-> rotatenya berapa derajat

      //      angle = MathHelper.DegreesToRadians(angle);


      //      //mulai rotasi
      //      //jadi sebenarnya cara kerjanya titiknya dirotasi 1 1

      //      for (int i = 0; i < _vertices.Count; i++)
      //      {
      //          _vertices[i] = getRotationResult(pivot, vector, angle, _vertices[i]);
      //      }

      //      //rotate the eulerdirection
      //      for (int i = 0; i < 3; i++)
      //      {
      //          _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

      //          /// Normalize
      //          /// Langkah-Langkah
      //          //length = akar dari (x^2 + y^2 + z^2)
      //          float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
      //          Vector3 temporary = new Vector3(0, 0, 0);
      //          temporary.X = _euler[i].X / length;
      //          temporary.X = _euler[i].Y / length;
      //          temporary.X = _euler[i].Z / length;

      //          _euler[i] = temporary;
      //      }

      //      _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);

      //      //stiap ganti, harus di bind lagi
      //      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      //      //ngirim array vertex melalui buffer ke graphic card
      //      GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
      //  }

      //  Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false/*yang dirotasi gak cuma verticesnya tapi juga sumbunya makanya butuh bool isEuler*/)
      //  {
      //      Vector3 temp, newPosition;
      //      if (isEuler)
      //      {
      //          temp = point;
      //      }
      //      else
      //      {
      //          //jika bukan euler
      //          temp = point - pivot;
      //      }
      //      newPosition.X =
      //(float)temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
      //      (float)temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
      //      (float)temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));
      //      newPosition.Y =
      //             (float)temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
      //             (float)temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
      //             (float)temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));
      //      newPosition.Z =
      //             (float)temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
      //             (float)temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
      //             (float)temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

      //      if (isEuler)
      //      {
      //          temp = newPosition;
      //      }
      //      else
      //      {
      //          temp = newPosition + pivot;
      //      }
      //      return temp;
      //  }

      //  public void resetEuler()
      //  {
      //      _euler[0] = new Vector3(1, 0, 0);
      //      _euler[1] = new Vector3(0, 1, 0);
      //      _euler[2] = new Vector3(0, 0, 1);
      //  }

        public float rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            //pivot -> mau rotate di titik mana
            //vector -> mau rotate di sumbu apa? (x,y,z)
            //angle -> rotatenya berapa derajat?
            var real_angle = angle;
            angle = MathHelper.DegreesToRadians(angle);

            //mulai ngerotasi
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = getRotationResult(pivot, vector, angle, _vertices[i]);
            }
            //rotate the euler direction
            for (int i = 0; i < 3; i++)
            {
                _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

                //NORMALIZE
                //LANGKAH - LANGKAH
                //length = akar(x^2+y^2+z^2)
                float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
                Vector3 temporary = new Vector3(0, 0, 0);
                temporary.X = _euler[i].X / length;
                temporary.Y = _euler[i].Y / length;
                temporary.Z = _euler[i].Z / length;
                _euler[i] = temporary;
            }
            _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);

            foreach (var item in Child)
            {
                item.rotate(pivot, vector, real_angle);
            }
            return angle;
        }

        Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;
            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                (float)temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                (float)temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                (float)temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));
            newPosition.Y =
                (float)temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                (float)temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                (float)temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));
            newPosition.Z =
                (float)temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                (float)temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                (float)temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void resetEuler()
        {
            _euler[0] = new Vector3(1, 0, 0);
            _euler[1] = new Vector3(0, 1, 0);
            _euler[2] = new Vector3(0, 0, 1);
        }

        public void addChildBox(float x, float y, float z, float length)
        {
            asset3d newChild = new asset3d();
            newChild.createBoxVertices(x, y, z, length);
            Child.Add(newChild);
        }
        public void addChildEllipsoidWithSurface(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            asset3d newChild = new asset3d();
            newChild.createEllipsoidWithSurface(radiusX,radiusY,radiusZ,_x,_y,_z,24,72);
            Child.Add(newChild);
        }
        public void CreateBezier(List<Vector2> vect,float _x, float _y, float _z, float radX = 0, float radY = 0, float radZ = 0)
        {
            Vector3 temp_vector;
            //karena gk ad koma
            for (float t = 0; t <= 1.0f; t += 0.01f)
            {
                Vector2 p = getP(vect, t);
                temp_vector.X = _x + p.X;
                temp_vector.Y = _y + p.Y;
                temp_vector.Z = _z;
                if (radX != 0 && radY != 0)
                    temp_vector.Z += ((float)Math.Sin(-pi * (temp_vector.X + radX) / (radX * 2)) * radZ + (float)Math.Sin(-pi * (temp_vector.Y + radY) / (radY * 2)) * radZ) / 2.0f;

                _vertices.Add(temp_vector);
            }
        }
        public Vector2 getP(List<Vector2> vect, float t)
        {
            Vector2 p = new Vector2(0, 0);
            float[] k = new float[vect.Count];

            //looping untuk masukin buat curva dari rumusnya
            //looping P =(1-t)P1 + tP2
            for (int i = 0; i < vect.Count - 1; i++)
            {
                k[i] = (float)Math.Pow((1 - t), vect.Count - 1 - i) * (float)Math.Pow(t, i);
            }
            for (int i = 0; i < vect.Count - 1; i++)
            {
                p.X += k[i] * vect[i].X;
                p.Y += k[i] * vect[i].Y;
            }
            return p;
        }
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();
            //Element 1 dari pascal
            currow.Add(1);
            if (rowIndex == 0)
            {
                return currow;
            }
            List<int> prev = getRow(rowIndex - 1);
            //element pascal yang ditengah
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            //nambah element yang terakhir
            currow.Add(1);
            return currow;
        }

        public void addChildRectangle(float x, float y, float z, float length)
        {
            asset3d newChild = new asset3d();
            newChild.createRectangle(x, y, z, length);
            Child.Add(newChild);
        }
        public void scale(float _scale)
        {
            transform = transform * Matrix4.CreateScale(_scale);
        }
        public void scaleX(float scaleX)
        {
            transform = transform * Matrix4.CreateScale(scaleX, 1f, 1f);
        }
        public void scaleY(float scaleY)
        {
            transform = transform * Matrix4.CreateScale(1f, scaleY, 1f);
        }
        public void scaleZ(float scaleZ)
        {
            transform = transform * Matrix4.CreateScale(1f, 1f, scaleZ);
        }
        public void translate(float x, float y, float z)
        {
            transform = transform * Matrix4.CreateTranslation(x, y, z);
        }

        //public void createEllipsoidVertices(float _positionX = 0.4f, float _positionY = 0.4f, float _positionZ = 0.4f, float _radius = 0.3f, float _height = 0.2f, float _extended = 0.5f)
        //{
        //    Vector3 temp_vector;
        //    float _pi = (float)Math.PI;


        //    for (float v = -_height / 2; v <= (_height / 2); v += 0.0001f)
        //    {
        //        Vector3 p = setBeizer((v + (_height / 2)) / _height);
        //        for (float u = -_pi; u <= _pi; u += (_pi / 30))
        //        {

        //            temp_vector.X = p.X + _radius * (float)Math.Cos(u);
        //            temp_vector.Y = p.Y + _radius * (float)Math.Sin(u);
        //            temp_vector.Z = _positionZ + v;

        //            _vertices.Add(temp_vector);

        //        }
        //    }



        //}
        private float pascal(int n, int k)
        {
            float res = 1;

            if (k > n - k)
                k = n - k;

            for (int i = 0; i < k; ++i)
            {
                res *= (n - i);
                res /= (i + 1);
            }
            return res;
        }
        Vector3 setBezier(List<Vector3> p, int count, float t)
        {
            Vector3 result;
            float[] coef = new float[count];
            for (int i = 0; i < count; i++)
            {
                coef[i] = pascal(count - 1, i) * (float)Math.Pow((1 - t), count - i - 1) * (float)Math.Pow(t, i);
            }
            result.X = 0.0f;
            result.Y = 0.0f;
            result.Z = 0.0f;
            for (int i = 0; i < count; i++)
            {
                result.X += coef[i] * p[i].X;
                result.Y += coef[i] * p[i].Y;
                result.Y += coef[i] * p[i].Z;
            }
            return result;
        }
        public void BezierLine(List<Vector3> Points)
        {
            _vertices_bezier_control = Points;
            _vertices.Add(Points[0]);
            for (float t = 0.0f; t <= 1.0f; t += 0.01f)
            {
                Vector3 P = setBezier(_vertices_bezier_control, _vertices_bezier_control.Count, t);
                _vertices.Add(P);
            }
        }
    }
}
