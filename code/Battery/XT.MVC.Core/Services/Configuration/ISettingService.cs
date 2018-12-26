using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using XT.MVC.Core.Configuration;

namespace XT.MVC.Core.Services.Configuration
{

    /// <summary>
    /// Setting service interface
    /// </summary>
    public partial interface ISettingService
    {
        T GetSettingByKey<T>(string key, T defaultValue = default(T),
            int appId = 0, bool loadSharedValueIfNotFound = false);

        void SetSetting<T>(string key, T value, int appId = 0, bool clearCache = true);

        bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int appId = 0)
            where T : ISettings, new();

        T LoadSetting<T>(int appId = 0) where T : ISettings, new();

        void SaveSetting<T>(T settings, int appId = 0) where T : ISettings, new();

        void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int appId = 0, bool clearCache = true) where T : ISettings, new();

        void DeleteSetting<T>() where T : ISettings, new();

        void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int appId = 0) where T : ISettings, new();

        void ClearCache();
    }
}
