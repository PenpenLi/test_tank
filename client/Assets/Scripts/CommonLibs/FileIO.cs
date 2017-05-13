using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TKBase
{
    class FileIO
    {
        /// <summary>
        /// 将对象序列化
        /// </summary>
        /// <param name="FilePath">文件(支持绝大多数数据类型)</param>
        /// <param name="obj">要序列化的对象(如哈希表,数组等等)</param>
        public static void FileSerialize(string FilePath, object obj)
        {
            if (System.IO.File.Exists(FilePath) == false)
            {
                FileInfo noteData = new FileInfo(FilePath);
            }
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            BinaryFormatter sl = new BinaryFormatter();
            sl.Serialize(fs, obj);
            fs.Close();


        }

        /// <summary>
        /// 将文件反序列化
        /// </summary>
        /// <param name="FilePath">文件路径(必须是经过当前序列化后的文件)</param>
        /// <returns>返回 null 表示序列反解失败或者目标文件不存在</returns>
        public static object FileDeSerialize(string FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFormatter sl = new BinaryFormatter();
                    object obg = sl.Deserialize(fs);
                    fs.Close();
                    return obg;
                }
                catch
                {
                    return null;
                }


            }
            else
            {
                return null;
            }

        }
    }

}