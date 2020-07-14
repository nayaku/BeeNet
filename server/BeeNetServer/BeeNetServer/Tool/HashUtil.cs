using Shipwreck.Phash;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Shipwreck.Phash.PresentationCore;
using System.Windows.Media.Imaging;
using BeeNetServer.Models;
using FreeImageAPI;

namespace BeeNetServer.Tool
{
    public class HashUtil
    {

        ///// <summary>
        ///// 获取md5码
        ///// </summary>
        ///// <param name="path">路径。注意必须是实际的路径。</param>
        ///// <returns>md5码</returns>
        ///// <exception cref="ArgumentException"></exception>
        //public static string GetMD5ByHashAlgorithm(string path)
        //{
        //    //if (!File.Exists(path))
        //    //    throw new ArgumentException(string.Format("<{0}>, 不存在", path));
        //    int bufferSize = 1024 * 1024 * 4;//自定义缓冲区大小4M
        //    byte[] buffer = new byte[bufferSize];
        //    Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
        //    int readLength;//每次读取长度 
        //    var output = new byte[bufferSize];
        //    while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //    {
        //        //计算MD5 
        //        hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
        //    }
        //    //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0) 
        //    hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
        //    string md5 = BitConverter.ToString(hashAlgorithm.Hash);
        //    hashAlgorithm.Clear();
        //    inputStream.Close();
        //    md5 = md5.Replace("-", "");
        //    return md5;
        //}

        ///// <summary>
        ///// 获取图片特征值
        ///// </summary>
        ///// <param name="path">图片路径</param>
        ///// <returns></returns>
        //public static byte[] PriHash(string path)
        //{
        //    using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    var bitmapSource = BitmapFrame.Create(stream);
        //    var hash = ImagePhash.ComputeDigest(bitmapSource.ToLuminanceImage()).Coefficients;
        //    return hash;
        //}

        public static (int Width,int Height) GetSize(Stream stream)
        {
            var bitmap=FreeImageBitmap.FromStream(stream);
            return (bitmap.Width, bitmap.Height);
        }

        public static string GetMD5(Stream stream)
        {
            var hashAlgorithm = new MD5CryptoServiceProvider();
            var md5Bytes=hashAlgorithm.ComputeHash(stream);
            hashAlgorithm.Clear();
            string md5 = BitConverter.ToString(md5Bytes);
            md5 = md5.Replace("-", "");
            return md5;
        }

        /// <summary>
        /// 判断两个特征值相似度是否大于阀值
        /// </summary>
        /// <param name="hash1"></param>
        /// <param name="hash2"></param>
        /// <returns></returns>
        public static bool IsSimilar(byte[] hash1,byte[] hash2)
        {
            return ImagePhash.GetCrossCorrelation(hash1, hash2)> UserSettingReader.UserSettings.PictureSettings.SimilarThreshold;
        }

        //public static Size GetPictureSize(string path)
        //{
        //    using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    using var image = Image.FromStream(stream);
        //    return image.Size;

        //}

        /// <summary>
        /// 完善图片信息（包括尺寸、MD5、特征值）
        /// </summary>
        /// <param name="picture"></param>
        public static void ComplementPicture(Picture picture,Stream stream)
        {
            var bitmapSource = BitmapFrame.Create(stream);
            picture.Width = bitmapSource.PixelWidth;
            picture.Height = bitmapSource.PixelHeight;
            using var md5Hash = MD5.Create();
            var imageBitArray = bitmapSource.ToByteImage().Array;
            var md5Data = md5Hash.ComputeHash(imageBitArray);
            var md5String = BitConverter.ToString(md5Data).Replace("-", ""); ;
            picture.MD5 = md5String;
            if (UserSettingReader.UserSettings.PictureSettings.UseSimilarJudge)
            {
                var priHash = ImagePhash.ComputeDigest(bitmapSource.ToLuminanceImage()).Coefficients;
                picture.PriHash = priHash;
            }
            else
            {
                picture.PriHash = null;
            }
        }
        //public static void Main(string[] args)
        //{
        //    System.Diagnostics.Debug.WriteLine("hello");
        //    var sh = MD5Util.PriHash(@"C:\Users\ZMK\Desktop\新建文件夹\source.jpg");
        //    var fileStrings = Directory.GetFileSystemEntries(@"C:\Users\ZMK\Desktop\新建文件夹");
        //    var list = new List<Tuple<float, string>>();
        //    foreach (var file in fileStrings)
        //    {
        //        var res = MD5Util.PriHash(file);
        //        list.Add(new Tuple<float, string>(ImagePhash.GetCrossCorrelation(sh, res), file));
        //        //System.Diagnostics.Debug.WriteLine($"{ImagePhash.GetCrossCorrelation(sh, res)} {file} = {res}");

        //    }
        //    list.Sort((e1, e2) => e1.Item1 > e2.Item1 ? 1 : e1.Item1==e2.Item1 ? 0 :-1);
        //    foreach(var t in list)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"{t.Item1}\t {t.Item2}");
        //    }
        //}
    }
}
