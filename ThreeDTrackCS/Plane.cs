using System;

namespace ThreeDTrackCS
{
    public struct Plane
    {

        private Vector3d position;
        private Vector3d normal;
        private int id;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public Vector3d Position
        {
            get
            {
                return position;
            }
        }

        public Vector3d Normal
        {
            get
            {
                return normal;
            }
        }

        public Plane( ref Vector3d position, ref Vector3d normal, ref int id )
        {
            this.position = position;
            this.normal = normal;
            this.id = id;
        }

        internal bool ContainsPoint( Vector3d position, double pointEpsilon )
        {
            return Math.Abs( normal.X * position.X + normal.Y * position.Y + normal.Z * position.Z - ( normal.X * this.position.X + normal.Y * this.position.Y + normal.Z * this.position.Z ) ) <= pointEpsilon;
        }

        public override string ToString()
        {
            return "{ \"id\": " + id + " \"position\": " + position + " \"normal\": " + normal + " }";
        }

        public string ToString(string format)
        {
            return "{ \"id\": " + id + " \"position\": " + position.ToString( format ) + " \"normal\": " + normal.ToString( format ) + " }";
        }
    }
}