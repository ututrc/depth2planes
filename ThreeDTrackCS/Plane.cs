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

        public Plane( Vector3d position, Vector3d normal, int id )
        {
            this.position = position;
            this.normal = normal;
            this.id = id;
        }
    }
}