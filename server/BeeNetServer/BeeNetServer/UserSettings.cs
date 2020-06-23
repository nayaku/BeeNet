using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer
{
    public class CUserSettings
    {
        public class CPictureSettings
        {
            private string _pictureStorePath = Path.Join("wwwroot", "pictures");
            /// <summary>
            /// 启动相似判断
            /// </summary>
            public bool UseSimilarJudge { get; set; } = true;
            /// <summary>
            /// 相似判断阀值
            /// </summary>
            public float SimilarThreshold { get; set; } = 0.95f;

            /// <summary>
            /// 图片库的位置
            /// </summary>
            public string PictureStorePath
            {
                get => _pictureStorePath;
                set
                {
                    CreatePictureStoreDirectory(value);
                    _pictureStorePath = value;
                }
            }

            private static void CreatePictureStoreDirectory(string value)
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
            }
            //public bool AutoAddFileNameAsLabel
            public CPictureSettings()
            {
                CreatePictureStoreDirectory(_pictureStorePath);
            }
        }
        public CPictureSettings PictureSettings { get; set; } = new CPictureSettings();
    }

    public static class UserSettingReader
    {
        private const string userSettingsFilePath = "UserSetting.json";
        public static CUserSettings UserSettings { get; set; }
        static UserSettingReader()
        {
            ReadSettings();
            SaveChange();
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        public static void ReadSettings()
        {
            if (File.Exists(userSettingsFilePath))
            {
                using var reader = File.OpenText(userSettingsFilePath);
                JsonSerializer jsonSerializer = JsonSerializer.Create();
                UserSettings = jsonSerializer.Deserialize(reader, typeof(CUserSettings)) as CUserSettings;

            }
            else
            {
                UserSettings = new CUserSettings();
            }
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        public static void SaveChange()
        {
            using var writer = File.CreateText(userSettingsFilePath);
            var serializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });
            serializer.Serialize(writer, UserSettings);
        }
    }
}
