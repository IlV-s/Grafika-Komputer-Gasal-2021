using System;
using System.Collections.Generic;
using System.Text;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ConsoleApp1
{
    class assets
    {
        float[] _vertices =
        {
            -0.5f,-0.5f,0.0f,//kaki kiri segitiga
            0.5f,-0.5f,0.0f,//kaki kanan segitiga
            0.0f,0.5f,0.0f //atas segitiga
        };


        uint[] _indices =
        {

        };
        int _elementBufferObject;


        int _vertexBufferObject;//menghandle vertices untuk ke graphic card
        int _vertexArrayObject; //bagian openGL yang ngurus array vertex yang dikirim
        Shader _shader;
        int[] _pascal = { };
        int index = 0;
        public assets(float[] vertices, uint[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }


        //Untuk ngeload vertex" e
        public void load()
        {
            //create buffer (buat ngirim)
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            //ngirim array vertex melalui buffer ke graphic card
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float)/*dikaliin sizeof(float) soalnya datanya binary*/, _vertices, BufferUsageHint.StaticDraw/*static dia gak nge update setelah di open window e (biasae dari 1000 frame baru ke update), stream lebih sering keganti dari dynamic (misal stream brarti dia per frame lgsg diupdate*/);

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

            //Untuk menyalakan/mengaktifkan
            GL.EnableVertexAttribArray(0);



            if(_indices.Length != 0)
            {
                //settingan EAO
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            }

            _shader = new Shader("D:/- Folder Kuliah/7/Grafika Komputer/Project/ConsoleApp1/ConsoleApp1/Shader/shader.vert", "D:/- Folder Kuliah/7/Grafika Komputer/Project/ConsoleApp1/ConsoleApp1/Shader/shader.frag");
            _shader.Use();
        }

        public void render(int _lines)
        {
            //step menggambar sebuah object
            //1. enable shader
            _shader.Use();

            //main" shader tambahan buat dikirim ke shader dari windows.cs (uniform)
            //buat nyari nama variabel yang mau dipake
            /*int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "uniformColor");
            GL.Uniform4(vertexColorLocation, 0.0f,0.0f,1.0f,1.0f);*/

            //2. panggil bind Vertex Array Object
            GL.BindVertexArray(_vertexArrayObject);
            //3. panggil fungsi untuk menggambar

            if(_indices.Length != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (_lines == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3); //0 untuk index, angka 3 itu jumlah vertex
                }
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Length + 1) / 3);
                }
                else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineLoop, 0, 3);
                }
                else if (_lines == 3)
                {
                    //lingkaran tanpa isi
                    //GL.DrawArrays(PrimitiveType.LineLoop, 0, (_vertices.Length + 1) / 3);
                    //lingkaran dengan isi
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Length + 1) / 3);
                }
                else if(_lines == 4)
                {
                    //karena inisialisasinya 1080, vertices.length e jadie 1080 jadi gk mungkin nggambar e kalo gitu, oleh karena itu perlu simpen variabel (buat gambar lingkaran)
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, index);

                }
            }
        }

        public void createCircle(float center_x, float center_y, float radius)
        {
            _vertices = new float[1080];//360* 3 karena ada 360 derajat dan 3 titik xyz

            //looping 360 titik buat lingkaran
            for(int i = 0; i < 360; i++)
            {
                //buat nyari alpha yang akan dikaliakn pada rumus lingkaran buat mencari titk x,y
                //(x,y) = (r cos alpha, r sin alpha)
                double degInRad = i * Math.PI / 180;

                //nyimpen x
                _vertices[i * 3] = (float)Math.Cos(degInRad) * radius + center_x;
                //nyimpen y
                _vertices[i * 3 + 1] = (float)Math.Sin(degInRad) * radius + center_y;
                //nyimpen x
                _vertices[i * 3 + 2] = 0;
            }
        }
        public void createEllips(float center_x, float center_y, float radius_x, float radius_y)
        {
            _vertices = new float[1080];//360* 3 karena ada 360 derajat dan 3 titik xyz

            //looping 360 titik buat lingkaran
            for (int i = 0; i < 360; i++)
            {
                //buat nyari alpha yang akan dikaliakn pada rumus lingkaran buat mencari titk x,y
                //(x,y) = (r cos alpha, r sin alpha)
                double degInRad = i * Math.PI / 180;

                //nyimpen x
                _vertices[i * 3] = (float)Math.Cos(degInRad) * radius_x + center_x;
                //nyimpen y
                _vertices[i * 3 + 1] = (float)Math.Sin(degInRad) * radius_y + center_y;
                //nyimpen x
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void updateMousePosition(float _x, float _y, float _z)
        {
            _vertices[index * 3] = _x;
            _vertices[index * 3 + 1] = _y;
            _vertices[index * 3 + 2] = _z;
            index++;

            //kalo mau buat yang line secara stiap klik, jadinya butuh di update terus"an
            GL.BufferData(BufferTarget.ArrayBuffer, index * 3 * sizeof(float)/*dikaliin sizeof(float) soalnya datanya binary*/, _vertices, BufferUsageHint.StaticDraw/*static dia gak nge update setelah di open window e (biasae dari 1000 frame baru ke update), stream lebih sering keganti dari dynamic (misal stream brarti dia per frame lgsg diupdate*/);

            //setelah ngirim, ini buat ngatur dia masuk mau dimana
            //_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            ////parameter 1 -> lokasi index input yang ada di shader
            ////parameter 2 -? jumlah elemen yang dikirimkan, kalau contoh project 3 float untuk tiap vertex
            ////paramater 3 -> tipe data yang kita kirimkan berjenis apa
            ////parameter 4 -> apakah perlu dinormalisasi
            ////parameter 5 -> berapa banyak jumlah vertex yang kita kirim dari variabel _vertices. Dalam kasus project ini ada 3 vertex * sizeof(float)
            ////parameter 6 -> vertex mau mulai dari index ke berapa
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            //Untuk menyalakan/mengaktifkan
            GL.EnableVertexAttribArray(0);

        }
        public List<float> CreateCurveBezier()
        {
            List<float> _vertices_bezier = new List<float>();
            //karena gk ad koma
            List<int> pascal = getRow(index - 1);
            _pascal = pascal.ToArray();
            for(float t= 0; t <= 1.0f; t += 0.01f)
            {
                Vector2 p = getP(index, t);
                _vertices_bezier.Add(p.X);
                _vertices_bezier.Add(p.Y);
                _vertices_bezier.Add(0);
            }
            return _vertices_bezier;
        }
        public Vector2 getP(int n, float t)
        {
            Vector2 p = new Vector2(0, 0);
            float[] k = new float[n];

            //looping untuk masukin buat curva dari rumusnya
            //looping P =(1-t)P1 + tP2
            for(int i=0; i < n; i++)
            {
                k[i] = (float)Math.Pow((1 - t), n - 1 - i) * (float)Math.Pow(t, i) * _pascal[i];
            }
            for(int i = 0; i < n; i++)
            {
                p.X += k[i] * _vertices[i * 3];
                p.Y += k[i] * _vertices[i * 3 + 1];
            }
            return p;
        }
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();
            //Element 1 dari pascal
            currow.Add(1);
            if(rowIndex == 0)
            {
                return currow;
            }
            List<int> prev = getRow(rowIndex - 1);
            //element pascal yang ditengah
            for (int i=1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            //nambah element yang terakhir
            currow.Add(1);
            return currow;
        }
        public void setVertices(float[] vertices)
        {
            _vertices = vertices;
        }
        public bool getVerticesLength()
        {
            if(_vertices[0] == 0)
            {
                return false;
            }
            if ((_vertices.Length + 1) / 3 > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
