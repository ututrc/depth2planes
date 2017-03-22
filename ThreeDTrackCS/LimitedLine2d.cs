namespace ThreeDTrackCS
{
    /// <summary>
    /// Limited line is a ray between two points
    /// </summary>
    public class LimitedLine2d
    {

        public Vector2d Start, Stop;

        public LimitedLine2d( Vector2d start, Vector2d stop )
        {
            Start = start;
            Stop = stop;
        }

    }
}