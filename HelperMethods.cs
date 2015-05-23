using System;
using System.Collections.Generic;
using System.IO;

namespace Storage.CompoundDocument
{
    /// <summary>
    /// 
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Closes all.
        /// </summary>
        /// <param name="set">The set.</param>
        public static void CloseAll(this IEnumerable<ResultSet> set)
        {
            foreach (var item in set)
            {
                item.CloseStream();
            }
        }

        /// <summary>
        /// Reads the stream part.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        internal static byte[] ReadStreamPart(this Stream str, int size)
        {
            var readbytes = new byte[size];
            str.Read(readbytes, 0, size);
            return readbytes;
        }

        /// <summary>
        /// Converts to int.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        internal static int ConvertToInt(this byte[] array)
        {
            if (array == null || array.Length == 0)
                return 0;

/*
            var tempArray = new byte[array.Length];
            Array.Copy(array, tempArray, array.Length);

            bool isLittleEndian = false;
            if (isLittleEndian)
                Array.Reverse(tempArray);
*/

            int result = 0;
            switch (array.Length)
            {
                case 2:
                    result = BitConverter.ToInt16(array, 0);
                    break;
                case 4:
                    result = BitConverter.ToInt32(array, 0);
                    break;
            }
            return result;
        }
    }
}
