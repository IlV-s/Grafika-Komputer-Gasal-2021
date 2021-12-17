using LearnOpenTK.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.Text;

namespace ConsoleApp1
{
    class assest3d
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<Vector3> _textureVertices = new List<Vector3>();
        List<Vector3> _normals = new List<Vector3>();
        List<uint> _indices = new List<uint>();


        int _vertexBufferObject;
        //VAO
        int _vertexArrayObject;
        //EBO
        int _elementBufferObject;
        Shader _shader;

        int derajat = 0;
        bool tangan = false;
        bool kaki = false;
        Matrix4 transform = Matrix4.Identity;
        //1 0 0 0
        //0 1 0 0
        //0 0 1 0
        //0 0 0 1

        public Vector3 _centerPosition = new Vector3(0, 0, 0);
        public List<Vector3> _euler = new List<Vector3>();
        Matrix4 _view;
        Matrix4 _projection;
        public List<assest3d> Child = new List<assest3d>();


        public assest3d()
        {
            //sumbu x
            _euler.Add(new Vector3(1, 0, 0));

            //sumbu y
            _euler.Add(new Vector3(0, 1, 0));

            //sumbu z
            _euler.Add(new Vector3(0, 0, 1));
        }


        public void load(int size_x, int size_y)
        {
            //create buffer
            _vertexBufferObject = GL.GenBuffer();

            //setting buffer targetnya apa
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            //ngirim array vertex melalui buffer ke grapich card
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);

            
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);


            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0 * sizeof(float));

            //menyalakan variable index ke 0 yang ada pada shader
            GL.EnableVertexAttribArray(0);


            if (_indices.Count != 0)
            {
                //setting element buffer
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint),
                              _indices.ToArray(), BufferUsageHint.StaticDraw);
            }


            //setting shader
            _shader = new Shader("C:/Users/Adithya/source/repos/ConsoleApp1/ConsoleApp1/Shaders/shader.vert",
                                 "C:/Users/Adithya/source/repos/ConsoleApp1/ConsoleApp1/Shaders/shader.frag");
            _shader.Use();
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), size_x / (float)size_y, 0.1f, 100.0f);

            foreach (var item in Child)
            {
                item.load(size_x, size_y);
            }
        }


        public void render(int _lines)
        {
           
            transform = transform * Matrix4.CreateTranslation(0.00001f, 0.0f, 0.0f);

            //scaling
            //transform = transform * Matrix4.CreateScale(1.001f);

            //rotasi
            //transform = transform * Matrix4.CreateRotationX(0.01f);
            //transform = transform * Matrix4.CreateRotationY(0.01f);
            //transform = transform * Matrix4.CreateRotationZ(0.01f);

            
            _shader.Use();
            _shader.SetMatrix4("transform", transform);
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);
            
            //int vertexColorLocation /*namanya bisa sebarang*/ = GL.GetUniformLocation(_shader.Handle, "ourColor");
            //GL.Uniform4(vertexColorLocation, 0.0f, 0.0f, 1.0f, 1.0f);

            
            GL.BindVertexArray(_vertexArrayObject);

            
            //triangle
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
                    //GL.DrawArrays(PrimitiveType.LineStrip, 0, 3);
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
                else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertices.Count);
                }
                else if (_lines == 3)
                {
                    
                    //GL.DrawArrays(PrimitiveType.LineLoop, 0, ((_vertices.Length + 1) / 3));

                    
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
            }

            foreach (var item in Child)
            {
                item.render(_lines);
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


        public void createEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngel, stackAngel, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngel = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(stackAngel);
                y = radiusY * (float)Math.Cos(stackAngel);
                z = radiusZ * (float)Math.Sin(stackAngel);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngel = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngel);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngel);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;

            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 * sectorCount + 1);

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                }
            }
        }
        public void createCircle(float center_x, float center_y, float radius)
        {
            float pi = (float)Math.PI;
            for (int i = 0;i<360;i++)
            {
                float degInRad = i * pi / 180;
                Vector3 temp_vector;
                temp_vector.X = (float)Math.Cos(degInRad) * radius + center_x;
                temp_vector.Y = (float)Math.Cos(degInRad) * radius + center_y;
                temp_vector.Z = 0;
                _vertices.Add(temp_vector);
            }
        }

        public void createBoxVertices(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //  Titik 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //  Titik 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //  Segitiga depan 1
                0,1,2,
                //  Segitiga depan 2
                1,2,3,
                //  Segitiga atas 1
                0,4,5,
                //  Segitiga atas 2
                0,1,5,
                //  Segitiga kanan 1
                1, 3, 5,
                //  Segitiga kanan 2
                3,5,7,
                //  Segitiga kiri 1
                0,2,4,
                //  Segitiga kiri 2
                2,4,6,
                //  Segitiga belakang 1
                4,5,6,
                //  Segitiga belakang 2
                5,6,7,
                //  Segitiga bawah 1
                2,3,6,
                //  Segitiga bawah 2
                3,6,7
            };
        }
        public void createEllipticParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = -pi; u <= pi; u += pi / 30)
            {
                for (float v = pi; v >= 0; v -= pi / 30)
                {
                    temp_vector.X = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Z = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.Y = _z + (float)Math.Pow(v, 2);
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createEllipticParaboloid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 150)
            {
                for (float v = 0; v <= 100; ++v)    //////////////////////////////////////////////////////////////////////////
                {
                    temp_vector.Y = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Z = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.X = _z + v * v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createHyperboloidSatuSisi(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = -pi; u <= pi; u += pi / 30)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 30)
                {
                    temp_vector.X = _x + (1 / (float)Math.Cos(v) * (float)Math.Cos(u)) * radiusX;
                    temp_vector.Y = _y + (1 / (float)Math.Cos(v) * (float)Math.Sin(u)) * radiusY;
                    temp_vector.Z = _z + (float)Math.Tan(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }



        //public void rotate1(Vector3 pivot, Vector3 vector, float angle)
        //{

        //    var real_angle = angle;
        //    angle = MathHelper.DegreesToRadians(angle);


        //    //mulai rotasi
        //    for (int i = 0; i < _vertices.Count; i++)
        //    {
        //        _vertices[i] = getRotationResult1(pivot, vector, angle/10, _vertices[i]);
        //    }

        //    //rotate euler direction
        //    for (int i = 0; i < 3; i++)
        //    {
        //        //_euler[i] = getRotationResult(new Vector3(0, 0, 0), vector, angle, _euler[i]);
        //        _euler[i] = getRotationResult1(pivot, vector, angle, _euler[i]);



        //        //length = akar(x^2 + y^2 + z^2)
        //        float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
        //        Vector3 temporary = new Vector3(0, 0, 0);
        //        temporary.X = _euler[i].X / length;
        //        temporary.Y = _euler[i].Y / length;
        //        temporary.Z = _euler[i].Z / length;
        //        _euler[i] = temporary;
        //    }

        //    _centerPosition = getRotationResult1(pivot, vector, angle, _centerPosition);

        //    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        //    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);


        //    foreach (var item in Child)
        //    {
        //        item.rotate1(pivot, vector, real_angle);
        //    }

        //}
        public void RotateTangan(float n, bool tng)
        {
            //rotate parentnya

            //sumbu x
            if(tng == true)
            {
                
                if (tangan == false)
                {
                    if (derajat <= 30 && tangan == false)
                    {
                        transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(n));
                        derajat++;
                    }
                    else
                    {
                        tangan = true;
                    }
                }
                else if (tangan == true)
                {
                    if (derajat >= 0)
                    {
                        n = n * -1;
                        transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(n));
                        derajat--;
                    }
                    else
                    {
                        tangan = false;
                    }
                }
            }
            else
            {
                transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(n));
            }
            
        }

        public void RotateKaki(float n, bool kk)
        {
            //rotate parentnya

            //sumbu x
            if (kk == true)
            {

                if (kaki == false)
                {
                    if (derajat <= 30 && kaki == false)
                    {
                        transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(n));
                        derajat++;
                    }
                    else
                    {
                        kaki = true;
                    }
                }
                else if (kaki == true)
                {
                    if (derajat >= 0)
                    {
                        n = n * -1;
                        transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(n));
                        derajat--;
                    }
                    else
                    {
                        kaki = false;
                    }
                }
            }
            else
            {
                transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(n));
            }

        }

        Vector3 getRotationResult1(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
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
        //reset euler
        public void resetEuler()
        {
            _euler[0] = new Vector3(1, 0, 0);
            _euler[1] = new Vector3(0, 1, 0);
            _euler[2] = new Vector3(0, 0, 1);
            
        }


        public void addChildEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            assest3d newChild = new assest3d();
            newChild.createEllipsoid2(radiusX, radiusY, radiusZ, _x, _y, _z, sectorCount, stackCount);
            Child.Add(newChild);
        }

        public void addHyperBoloidSatuSisi(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            assest3d newChild = new assest3d();
            newChild.createHyperboloidSatuSisi(radiusX, radiusY, radiusZ, _x, _y, _z);
            Child.Add(newChild);
        }

        public void scale(float _scale)
        {
            transform = transform * Matrix4.CreateScale(_scale);
        }

        public void translate(float x, float y, float z)
        {
            transform = transform * Matrix4.CreateTranslation(x, y, z);
        }

        public void shader_eye()
        {
            _shader = new Shader("C:/Users/Adithya/source/repos/ConsoleApp1/ConsoleApp1/Shaders/shader_eye.vert", "C:/Users/Adithya/source/repos/ConsoleApp1/ConsoleApp1/Shaders/shader_eye.frag");
            _shader.Use();
        }

        public void shader_sclera()
        {
            _shader = new Shader("C:/Users/Adithya/source/repos/ConsoleApp1/ConsoleApp1/Shaders/Shader_sclera.vert", "C:/Users/Adithya/source/repos/ConsoleApp1/ConsoleApp1/Shaders/shader_sclera.frag");
            _shader.Use();
        }

        public void addChildbox(float x, float y, float z, float length)
        {
            assest3d newChild = new assest3d();
            newChild.createBoxVertices(x, y, z, length);
            Child.Add(newChild);
        }
        public void addChildbox2(float x, float y, float z, float length)
        {
            assest3d newChild = new assest3d();
            newChild.createBoxVertices(x, y, z, length);
            Child.Add(newChild);
        }
        public void addChildEllipticParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            assest3d newChild = new assest3d();
            newChild.createEllipticParaboloid(radiusX, radiusY, radiusZ, _x,_y,_z);
            Child.Add(newChild);
        }
    }
}
