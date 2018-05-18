using System;
using System.Drawing;

namespace arcball
{
    /*
     * This code is a part of NeHe tutorials and was converted from C++ into C#. 
     * Matrix/Vector manipulations have been updated.
     */
    public class arcball
    {
        private const float Epsilon = 1.0e-5f;

        private Vector3f StVec; //Saved click vector
        private Vector3f EnVec; //Saved drag vector
        private float adjustWidth; //Mouse bounds width
        private float adjustHeight; //Mouse bounds height

        public arcball(float NewWidth, float NewHeight)
        {
            StVec = new Vector3f();
            EnVec = new Vector3f();
            setBounds(NewWidth, NewHeight);
        }

        private void mapToSphere(Point point, Vector3f vector)
        {
            Point2f tempPoint = new Point2f(point.X, point.Y);

            //Adjust point coords and scale down to range of [-1 ... 1]
            tempPoint.x = (tempPoint.x * this.adjustWidth) - 1.0f;
            tempPoint.y = 1.0f - (tempPoint.y * this.adjustHeight);

            //Compute square of the length of the vector from this point to the center
            float length = (tempPoint.x * tempPoint.x) + (tempPoint.y * tempPoint.y);

            //If the point is mapped outside the sphere... (length > radius squared)
            if (length > 1.0f)
            {
                //Compute a normalizing factor (radius / sqrt(length))
                float norm = (float)(1.0 / Math.Sqrt(length));

                //Return the "normalized" vector, a point on the sphere
                vector.x = tempPoint.x * norm;
                vector.y = tempPoint.y * norm;
                vector.z = 0.0f;
            }
            //Else it's inside
            else
            {
                //Return a vector to a point mapped inside the sphere sqrt(radius squared - length)
                vector.x = tempPoint.x;
                vector.y = tempPoint.y;
                vector.z = (float)System.Math.Sqrt(1.0f - length);
            }
        }

        public void setBounds(float NewWidth, float NewHeight)
        {
            //Set adjustment factor for width/height
            adjustWidth = 1.0f / ((NewWidth - 1.0f) * 0.5f);
            adjustHeight = 1.0f / ((NewHeight - 1.0f) * 0.5f);
        }

        //Mouse down
        public virtual void click(Point NewPt)
        {
            mapToSphere(NewPt, this.StVec);
        }

        //Mouse drag, calculate rotation
        public void drag(Point NewPt, Quat4f NewRot)
        {
            //Map the point to the sphere
            this.mapToSphere(NewPt, EnVec);

            //Return the quaternion equivalent to the rotation
            if (NewRot != null)
            {
                Vector3f Perp = new Vector3f();

                //Compute the vector perpendicular to the begin and end vectors
                Vector3f.cross(Perp, StVec, EnVec);

                //Compute the length of the perpendicular vector
                if (Perp.length() > Epsilon)
                //if its non-zero
                {
                    //We're ok, so return the perpendicular vector as the transform after all
                    NewRot.x = Perp.x;
                    NewRot.y = Perp.y;
                    NewRot.z = Perp.z;
                    //In the quaternion values, w is cosine (theta / 2), where theta is the rotation angle
                    NewRot.w = Vector3f.dot(StVec, EnVec);
                }
                //if it is zero
                else
                {
                    //The begin and end vectors coincide, so return an identity transform
                    NewRot.x = NewRot.y = NewRot.z = NewRot.w = 0.0f;
                }
            }
        }
    }

    public class Matrix4f
    {
        private float[,] M;

        public Quat4f Rotation
        {
            set
            {
                float n, s;
                float xs, ys, zs;
                float wx, wy, wz;
                float xx, xy, xz;
                float yy, yz, zz;

                M = new float[4, 4];

                n = (value.x * value.x) + (value.y * value.y) + (value.z * value.z) + (value.w * value.w);
                s = (n > 0.0f) ? 2.0f / n : 0.0f;

                xs = value.x * s;
                ys = value.y * s;
                zs = value.z * s;
                wx = value.w * xs;
                wy = value.w * ys;
                wz = value.w * zs;
                xx = value.x * xs;
                xy = value.x * ys;
                xz = value.x * zs;
                yy = value.y * ys;
                yz = value.y * zs;
                zz = value.z * zs;

                M[0, 0] = 1.0f - (yy + zz);
                M[0, 1] = xy - wz;
                M[0, 2] = xz + wy;

                M[1, 0] = xy + wz;
                M[1, 1] = 1.0f - (xx + zz);
                M[1, 2] = yz - wx;

                M[2, 0] = xz - wy;
                M[2, 1] = yz + wx;
                M[2, 2] = 1.0f - (xx + yy);

                M[3, 3] = 1.0f;

            }
        }

        public Matrix4f()
        {
            setIdentity();
        }

        public void get_Renamed(float[] dest)
        {
            int k = 0;
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    dest[k] = this.M[j, i];
                    k++;
                }
        }

        public void setZero()
        {
            this.M = new float[4, 4]; // set to zero
        }

        public void setIdentity()
        {
            this.M = new float[4, 4]; // set to zero
            for (int i = 0; i <= 3; i++) this.M[i, i] = 1.0f;
        }

        public void set_Renamed(Matrix4f m1)
        {
            this.M = m1.M;
        }

        public void mul(Matrix4f m1, Matrix4f m2)
        {
            float[] MulMat = new float[16];
            float elMat = 0.0f;
            int k = 0;

            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    for (int l = 0; l <= 3; l++) elMat += m1.M[i, l] * m2.M[l, j];
                    MulMat[k] = elMat;
                    elMat = 0.0f;
                    k++;
                }

            k = 0;
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    m1.M[i, j] = MulMat[k];
                    k++;
                }
        }

    }

    public class Point2f
    {
        public float x, y;

        public Point2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Quat4f
    {
        public float x, y, z, w;
    }

    public class Vector3f
    {
        public float x, y, z;

        public static void cross(Vector3f Result, Vector3f v1, Vector3f v2)
        {
            Result.x = (v1.y * v2.z) - (v1.z * v2.y);
            Result.y = (v1.z * v2.x) - (v1.x * v2.z);
            Result.z = (v1.x * v2.y) - (v1.y * v2.x);
        }

        public static float dot(Vector3f v1, Vector3f v2)
        {
            return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z + v2.z);
        }

        public virtual float length()
        {
            return (float)System.Math.Sqrt(x * x + y * y + z * z);
        }
    }

}