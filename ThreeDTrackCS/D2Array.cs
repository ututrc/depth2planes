using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDTrackCS
{
    /// <summary>
    /// A generic two dimensional "fast" array
    /// Uses one dimensional array with scope to work
    /// </summary>
    /// <typeparam name="T">The type of array</typeparam>
    public class D2Array<T>
    {

        T[] data;
        int width, height;

        public D2Array( int width, int height )
        {
            this.width = width;
            this.height = height;
            data = new T[width * height];
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public T this[int col, int row]
        {
            get
            {
                return data[col + row * width];
            }
            set
            {
                data[col + row * width] = value;
            }
        }

    }
}
