using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonClass;
using System;
using System.IO;
using System.Data;
using UnityEngine.InputSystem;
using static GameSetting;
using PlayMode = CommonClass.PlayMode;
using ICSharpCode.SharpZipLib.GZip;
using System.Text;
using FMOD;
using Debug = UnityEngine.Debug;

public class GameSetting
{
    public static Config Config { get; private set; }
    public static PcmData Temp;
    public static PcmData Selected;
    public static SongInfo SelectedInfo;
    public static DanCourse SelectedCourse;
    public static bool FavorSelected;
    public static Difficulty Difficulty = Difficulty.Edit;
    public static Dictionary<SortColor, List<SongInfo>> Songs = new Dictionary<SortColor, List<SongInfo>>();
    public static PlayerRecord Record;
    public static DataSet Skin;
    public static DataSet SkinTranslation;

    //player2
    public static Difficulty Difficulty2P;
    public static Speed Speed2P;
    public static RandomType Random2P;
    public static bool Steal2P;
    public static bool Revers2P;
    public static Special Special2P;

    //game play options
    public static PlayMode Mode = PlayMode.Normal;
    public static Replay Replay;
    public static GameStyle Style = GameStyle.TraditionalStyle;

    //game play temp
    public static PlayMode TempMode = PlayMode.Normal;
    public static Special TempOption = Special.None;
    public static int TempScoreMode = 0;

    //keys
    public static InputAction DonLeft;
    public static InputAction DonRight;
    public static InputAction KaLeft;
    public static InputAction KaRight;

    public static InputAction DonLeft2;
    public static InputAction DonRight2;
    public static InputAction KaLeft2;
    public static InputAction KaRight2;

    public static bool Player2;

    public static InputAction DonLeft2P;
    public static InputAction DonRight2P;
    public static InputAction KaLeft2P;
    public static InputAction KaRight2P;
    public static AndroidConfig AndroidConfig;

    private static DataTable translations;

    public enum SkinPosition
    {
        TypeUnkown = 0x00000000,
        TypeEmotion = 0x00000001,           //脸部表情
        TypeHead = 0x00000010,           //头部
        TypeBody = 0x00000100,           //身体部分
        TypeDrumSkin = 0x00001000,           //鼓皮颜色
        TypeCos = 0x00010000,             //全身替换
        TypeSub = 0x00100000,             //挂载的小物件
        TypeFrontBack = 0x01000000,      //脸部和底部
        TypeFace = 0x10000000,             //面部颜色
        TypeSkin = 0x20000000,             //身体手脚等白色部分的颜色
        TypeTatoo = 0x30000000,             //鼓皮图案

        //遮罩
        MaskEmotion = 0x000000F,           //脸部表情
        MaskHead = 0x00000F0,
        MaskBody = 0x0000F00,               //身体
        MaskBodySkin = 0x000F000,           //鼓皮
        MaskFrontBack = 0xF000000,          //脸部和底部
    }

    public enum KeyType
    {
        KeyNone,
        Up,
        Down,
        Left,
        Right,
        LeftDon,
        RightDon,
        LeftKa,
        RightKa,
        Option,
        Confirm,
        Cancel,
        Escape,
        Config,
        Random,
        Direction,
    }
    public readonly static List<int> DrumTypes = new List<int>
    {
        0, 1, 2,  4,5,6,7,
        11, 12,   17,18,  20,
           22,  24,  26,  28,29, 30,
        31, 32,33,34,35,  37,38,39, 40,
        41, 43,  45,  47,48,49, 50,
    };

    public readonly static List<KeyCode> ReservedKeys = new List<KeyCode>
    {
        KeyCode.Escape, KeyCode.KeypadEnter, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Return
    };

    public GameSetting()
    {
        LoadConfig();
#if UNITY_ANDROID
        LoadQuality();
#endif
    }

    private void LoadConfig()
    {
        string path;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        path = Environment.CurrentDirectory + "/config.json";
#else
        path = Application.persistentDataPath + "/config.json";
#endif
        if (File.Exists(path))
        {
            try
            {
                Config = JsonUntity.Json2Object<Config>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Config = new Config
                {
                    PlayerName = "新来的咚酱",
                    MusicVolume = 1,
                    EffectVolume = 1,
                    Special = Special.None,
                    VSync = true
                };

                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        Config.Locale = Locale.English;
                        break;
                    case SystemLanguage.Spanish:
                        Config.Locale = Locale.Spanish;
                        break;
                    case SystemLanguage.Japanese:
                        Config.Locale = Locale.Japanese;
                        break;
                    case SystemLanguage.Korean:
                        Config.Locale = Locale.Korean;
                        break;
                }
            }
        }
        else
        {
            Debug.Log("设置文件不存在");
            Config = new Config
            {
                PlayerName = "新来的咚酱",
                MusicVolume = 1,
                EffectVolume = 1,
                SFXVolume = 1,
                Special = 0,
                InputCooldown = 0,
                VSync = true
            };

            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    Config.Locale = Locale.English;
                    break;
                case SystemLanguage.Spanish:
                    Config.Locale = Locale.Spanish;
                    break;
                case SystemLanguage.Japanese:
                    Config.Locale = Locale.Japanese;
                    break;
                case SystemLanguage.Korean:
                    Config.Locale = Locale.Korean;
                    break;
            }
        }

#if KOREAN_TEST
        Config.Locale = Locale.Korean;
#endif

        Style = (GameStyle)Config.GameStyle;
        if (Config.PlayList == null) Config.PlayList = new List<string>();
        if (Config.JudgeRange == null || Config.JudgeRange.Count < 3)
        {
            Config.JudgeRange = new Dictionary<HitNoteResult, int>
            {
                { HitNoteResult.Good, 75 },
                { HitNoteResult.Bad, 108 },
                { HitNoteResult.Perfect, 25 },
            };
        }
        if (Config.DonLeft == null || string.IsNullOrEmpty(Config.DonLeft.Path))
        {
            Config.DonLeft = new Key("f", "/Keyboard/f");
        }
        if (Config.DonRight == null || string.IsNullOrEmpty(Config.DonRight.Path))
        {
            Config.DonRight = new Key("j", "/Keyboard/j");
        }
        if (Config.KaLeft == null || string.IsNullOrEmpty(Config.KaLeft.Path))
        {
            Config.KaLeft = new Key("d", "/Keyboard/d");
        }
        if (Config.KaRight == null || string.IsNullOrEmpty(Config.KaRight.Path))
        {
            Config.KaRight = new Key("k", "/Keyboard/k");
        }
        if (Config.Option == null || string.IsNullOrEmpty(Config.Option.Path))
        {
            Config.Option = new Key("space", "/Keyboard/space");
        }

        if (Config.GamePadKeys == null)
        {
            Config.GamePadKeys = new Dictionary<KeyType, List<string>>();
            /*
            {

                { KeyType.Direction, new List<string>{ "<Gamepad>/dpad" } },

                { KeyType.LeftDon, new List<string>{ "<Gamepad>/dpad/right", "<Gamepad>/dpad/down" } },
                { KeyType.RightDon, new List<string>{ "<Gamepad>/buttonWest","<Gamepad>/buttonSouth" } },
                { KeyType.LeftKa, new List<string>{ "<Gamepad>/dpad/left","<Gamepad>/dpad/up" } },
                { KeyType.RightKa, new List<string>{ "<Gamepad>/buttonEast","<Gamepad>/buttonNorth" } },

                { KeyType.Confirm, new List<string>{ "<Gamepad>/buttonEast" } },
                { KeyType.Cancel, new List<string>{ "<Gamepad>/buttonSouth" } },
                { KeyType.Option, new List<string>{ "<Gamepad>/Start" } },
                { KeyType.Escape, new List<string>{ "<Gamepad>/Select" } },
                { KeyType.Config, new List<string>{ "<Gamepad>/buttonNorth" } },
            };
            */
        }

        ParseKey(0);
        ParseKey(1);
        ParseKey(2);
        ParseKey(3);

        if (Config.GamePadKeys2P == null)
        {
            Config.GamePadKeys2P = new Dictionary<KeyType, string>
            {
                { KeyType.LeftDon, string.Empty },
                { KeyType.RightDon, string.Empty },
                { KeyType.LeftKa, string.Empty },
                { KeyType.RightKa, string.Empty },
            };
        }
            
        ParseKey2P(KeyType.LeftDon);
        ParseKey2P(KeyType.RightDon);
        ParseKey2P(KeyType.LeftKa);
        ParseKey2P(KeyType.RightKa);

        Config.MusicVolume = Mathf.Min(1, Config.MusicVolume);
        Config.MusicVolume = Mathf.Max(0, Config.MusicVolume);
        if (Config.ScoreMode > 3 || Config.ScoreMode < 0)
            Config.ScoreMode = 0;
        if (Config.SortColor == null || Config.SortColor.Count == 0)
        {
            Config.SortColor = new Dictionary<int, SortColor>
            {
                { 0, new SortColor("POPs", "流行音乐", "#1d9fbd") },
                { 1, new SortColor("アニメ", "卡通動畫音樂", "#fe9800") },
                { 2, new SortColor("ボーカロイド曲", "VOCALOID乐曲", "#e4f2f3") },
                { 3, new SortColor("キッズ", "兒童歌曲", "#ff4a83") },
                { 4, new SortColor("バラエティ", "综合音乐", "#8ed41e") },
                { 5, new SortColor("クラシック", "古典音樂", "#d1a213") },
                { 6, new SortColor("ゲームミュージック", "遊戲音樂", "#9c75be") },
                { 7, new SortColor("ナムコオリジナル", "NAMCO原创音乐", "#ff5a13") },
                { 8, new SortColor("華語音樂", string.Empty, "#52c803") },
            };
        }

        if (Config.FavorColor == null) Config.FavorColor = new SortColor("收藏夾", string.Empty, "#FF9797");
        if (Config.Favorites == null)
        {
            Config.Favorites = new List<string>();
        }
        else if (Config.Favorites.Count > 30)
        {
            for (int i = Config.Favorites.Count - 1; i > 29; i--)
                Config.Favorites.RemoveAt(i);
        }
        if (Config.DrumSoundType < 0 || Config.DrumSoundType >= DrumTypes.Count)
            Config.DrumSoundType = 0;

        if (Config.ResolutionHeight == 0 || Config.ResolutionWidth == 0)
        {
            float x = (float)Screen.currentResolution.width / 16;
            float y = (float)Screen.currentResolution.height / 9;
            if (x > y)
            {
                Config.ResolutionHeight = Screen.currentResolution.height;
                Config.ResolutionWidth = Mathf.CeilToInt(y * 16);
            }
            else if (y > x)
            {
                Config.ResolutionHeight = Mathf.CeilToInt(x * 9);
                Config.ResolutionWidth = Screen.currentResolution.width;
            }
            else
            {
                Config.ResolutionWidth = Screen.currentResolution.width;
                Config.ResolutionHeight = Screen.currentResolution.height;
            }
            Config.FullScreen = true;
        }
        if (Config.Presets == null) Config.Presets = new Dictionary<int, List<string>>();
        if (Config.InputMode != 1 && Config.InputMode != 2)
            Config.InputMode = 1;
        //InputSystem.settings.updateMode = (InputSettings.UpdateMode)Config.InputMode;
        //InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        Time.fixedDeltaTime = 0.002f;

        if (Config.Controller < 0 || Config.Controller > 2) Config.Controller = 0;
        if (Config.DrumScale < 0.5 || Config.DrumScale > 1) Config.DrumScale = 1;

        SaveConfig();

        //加载记录
        path = string.Empty;
#if UNITY_STANDALONE_WIN
        path = Environment.CurrentDirectory + "/record.json";
#else
        path = Application.persistentDataPath + "/record.json";
#endif
        if (File.Exists(path))
        {
            Record Record = new Record();
            bool old_type = false;
            try
            {
                Record = JsonUntity.Json2Object<Record>(File.ReadAllText(path));
                old_type = true;
            }
            catch (Exception)
            {
            }

            if (old_type)
            {
                GameSetting.Record = new PlayerRecord();
                GameSetting.Record.Record = Record;
            }
            else
            {
                try
                {
                    GZipInputStream zipFile = new GZipInputStream(File.OpenRead(path));
                    using (MemoryStream re = new MemoryStream(50000))
                    {
                        int count;
                        byte[] data = new byte[50000];
                        while ((count = zipFile.Read(data, 0, data.Length)) != 0)
                        {
                            re.Write(data, 0, count);
                        }
                        byte[] overarr = re.ToArray();
                        re.Close();

                        //将byte数组转为string
                        string result = Encoding.UTF8.GetString(overarr);
                        GameSetting.Record = JsonUntity.Json2Object<PlayerRecord>(result);

                        if (GameSetting.Record.DaniRecord.Columns.Contains("good"))
                            GameSetting.Record.DaniRecord.Columns.Remove("good");
                        if (GameSetting.Record.Record.Columns.Contains("full_combo"))
                            GameSetting.Record.Record.Columns.Remove("full_combo");
                    }
                    zipFile.Close();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
        else
        {
            Debug.Log("设置文件不存在");
            Record = new PlayerRecord();
        }

        SaveRecord();
        //加载皮肤信息
        InitSkinData();

        //加载自定义场景信息

        //加载翻译表
        LoadTranslation();
    }

    private static void LoadQuality()
    {
        string path = Application.persistentDataPath + "/android_config.json";
        bool success = true;
        if (File.Exists(path))
        {
            try
            {
                AndroidConfig = JsonUntity.Json2Object<AndroidConfig>(File.ReadAllText(path));
            }
            catch (Exception)
            {
                success = false;
            }
        }
        else
            success = false;

        if (!success)
        {
            AndroidConfig = new AndroidConfig
            {
                Level = GraphicLevel.Lowest,
                SaveHiScore = false
            };
            SaveAndroidConfig();
        }
    }

    public static void SaveAndroidConfig()
    {
        string path = Application.persistentDataPath + "/android_config.json";
        File.Delete(path);
        File.AppendAllText(path, JsonUntity.Object2Json(AndroidConfig));
    }

    private static void LoadTranslation()
    {
        string path = string.Empty;
#if UNITY_STANDALONE_WIN
        path = Environment.CurrentDirectory + "/translation";
#else
        path = Application.persistentDataPath + "/translation";
#endif

        string suffix = "_cn.json";
        switch (Config.Locale)
        {
            case Locale.English:
                suffix = "_en.json";
                break;
            case Locale.Japanese:
                suffix = "_jp.json";
                break;
            case Locale.Korean:
                suffix = "_kr.json";
                break;
            case Locale.Spanish:
                suffix = "_es.json";
                break;
        }
        path += suffix;
        if (File.Exists(path))
        {
            string json_str = File.ReadAllText(path);
            translations = JsonUntity.Json2Object<DataSet>(json_str).Tables[0];
        }
        else
            translations = null;
    }

    private static readonly Dictionary<SkinPosition, SkinPosition> masks = new Dictionary<SkinPosition, SkinPosition>
    {
        { SkinPosition.TypeEmotion, SkinPosition.MaskEmotion },
        { SkinPosition.TypeHead, SkinPosition.MaskHead },
        { SkinPosition.TypeBody, SkinPosition.MaskBody },
        { SkinPosition.TypeDrumSkin, SkinPosition.MaskBodySkin },
        { SkinPosition.TypeFrontBack, SkinPosition.MaskFrontBack },
    };

    public static void InitSkinData()
    {
        Skin = new DataSet();

        DataTable Head = new DataTable("Head");
        Head.Columns.Add("Name", typeof(string));                    //皮肤名
        Head.Columns.Add("Pack", typeof(string));                    //包名
        Head.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Head.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Head);

        DataTable Body = new DataTable("Body");
        Body.Columns.Add("Name", typeof(string));                    //皮肤名
        Body.Columns.Add("Mask", typeof(int));                       //该皮肤遮挡的部位
        Body.Columns.Add("Pack", typeof(string));                    //包名
        Body.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Body.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Body);

        DataTable Emotion = new DataTable("Emotion");
        Emotion.Columns.Add("Name", typeof(string));                    //皮肤名
        Emotion.Columns.Add("Pack", typeof(string));                    //包名
        Emotion.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Emotion.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Emotion);

        DataTable Sub = new DataTable("Sub");
        Sub.Columns.Add("Name", typeof(string));                    //皮肤名
        Sub.Columns.Add("Pack", typeof(string));                    //包名
        Sub.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Sub.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Sub);

        DataTable Cos = new DataTable("Cos");
        Cos.Columns.Add("Name", typeof(string));                    //皮肤名
        Cos.Columns.Add("Mask", typeof(int));                       //该皮肤替换的部位(仅在启用TypeCos的挂件时生效)
        Cos.Columns.Add("Keep", typeof(int));                       //使用该皮肤后其他需要恢复默认值的部位(仅在启用TypeCos的挂件时生效)
        Cos.Columns.Add("Pack", typeof(string));                    //包名
        Cos.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Cos.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Cos);

        /*
        DataTable Face = new DataTable("Face");
        Face.Columns.Add("Name", typeof(string));                    //皮肤名
        Face.Columns.Add("Pack", typeof(string));                    //包名
        Face.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Face.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Face);
        */
        DataTable Tatoo = new DataTable("Tatoo");
        Tatoo.Columns.Add("Name", typeof(string));                    //皮肤名
        Tatoo.Columns.Add("Pack", typeof(string));                    //包名
        Tatoo.Columns.Add("Condition", typeof(int));                  //启用条件的种类
        Tatoo.Columns.Add("Value", typeof(string));                   //启用条件的值
        Skin.Tables.Add(Tatoo);

        //翻译
        SkinTranslation = new DataSet();
        DataTable Head1 = new DataTable("Head");
        Head1.Columns.Add("Name", typeof(string));  
        Head1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Head1);

        DataTable Body1 = new DataTable("Body");
        Body1.Columns.Add("Name", typeof(string));
        Body1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Body1);

        DataTable Emotion1 = new DataTable("Emotion");
        Emotion1.Columns.Add("Name", typeof(string));
        Emotion1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Emotion1);

        DataTable Sub1 = new DataTable("Sub");
        Sub1.Columns.Add("Name", typeof(string));
        Sub1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Sub1);

        DataTable Cos1 = new DataTable("Cos");
        Cos1.Columns.Add("Name", typeof(string));
        Cos1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Cos1);

        /*
        DataTable Face1 = new DataTable("Face");
        Face1.Columns.Add("Name", typeof(string));                    //皮肤名
        Face1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Face1);
        */
        DataTable Tatoo1 = new DataTable("Tatoo");
        Tatoo1.Columns.Add("Name", typeof(string));                    //皮肤名
        Tatoo1.Columns.Add("Translation", typeof(string));
        SkinTranslation.Tables.Add(Tatoo1);
    }

    public static string GetSkinPackName(SkinPosition postion, string skin_name)
    {
        string pack = string.Empty;
        switch (postion)
        {
            case SkinPosition.TypeHead:
                {
                    DataRow[] rows = Skin.Tables["Head"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
            case SkinPosition.TypeBody:
                {
                    DataRow[] rows = Skin.Tables["Body"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
            case SkinPosition.TypeEmotion:
                {
                    DataRow[] rows = Skin.Tables["Emotion"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
            case SkinPosition.TypeCos:
                {
                    DataRow[] rows = Skin.Tables["Cos"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
            case SkinPosition.TypeSub:
                {
                    DataRow[] rows = Skin.Tables["Sub"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
                /*
            case SkinPosition.TypeFace:
                {
                    DataRow[] rows = Skin.Tables["Face"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
                */
            case SkinPosition.TypeTatoo:
                {
                    DataRow[] rows = Skin.Tables["Tatoo"].Select(string.Format("Name = '{0}'", skin_name));
                    if (rows.Length > 0)
                        pack = rows[0]["Pack"].ToString();
                }
                break;
        }

        return pack;
    }

    private static Dictionary<DanLevel, int> level_id = new Dictionary<DanLevel, int>
    {
        //easy
        { DanLevel.Junior, 36000 },

        //normal
        { DanLevel.Class10, 36002 },
        { DanLevel.Class9, 36003 },
        { DanLevel.Class8 , 36004 },
        { DanLevel.Class7, 36005 },
        { DanLevel.Class6, 36006 },

        //hard
        { DanLevel.Class5, 36007 },
        { DanLevel.Class4, 36008 },
        { DanLevel.Class3, 36009 },
        { DanLevel.Class2 , 36010 },
        { DanLevel.Class1, 36011 },

        //oni
        { DanLevel.Dan1, 36012 },
        { DanLevel.Dan2, 36013},
        { DanLevel.Dan3, 36014},
        { DanLevel.Dan4, 36015 },
        { DanLevel.Dan5, 36016 },
        { DanLevel.Dan6, 36017 },
        { DanLevel.Dan7, 36018 },
        { DanLevel.Dan8, 36019 },
        { DanLevel.Dan9, 36020 },
        { DanLevel.Dan10 , 36021 },

        { DanLevel.Kurotou, 36022 },
        { DanLevel.Meijin, 36023 },
        { DanLevel.Choujin, 36024 },
        { DanLevel.Tatsujin, 36025},
        { DanLevel.Extension, 36026 },
    };

    public static void SaveConfig()
    {
        TempMode = Mode;
        TempOption = Config.Special;
        TempScoreMode = Config.ScoreMode;

        string path;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        path = Environment.CurrentDirectory + "/config.json";
#else
        path = Application.persistentDataPath + "/config.json";
#endif
        File.Delete(path);
        File.AppendAllText(path, JsonUntity.Object2Json(Config.Copy()));
    }
    public static void SaveRecord()
    {
        string path;
#if UNITY_STANDALONE_WIN
        path = Environment.CurrentDirectory + "/record.json";
#else
        path = Application.persistentDataPath + "/record.json";
#endif
        File.Delete(path);

        string str = JsonUntity.Object2Json(Record, false);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(str);
        using (MemoryStream ms = new MemoryStream())
        {
            GZipOutputStream compressedzipStream = new GZipOutputStream(ms);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            FileStream fs = new FileStream(path, FileMode.Create);
            fs.Write(ms.ToArray(), 0, ms.ToArray().Length);
            fs.Close();
            ms.Close();
        }
    }

    public static void SetPlayerName(string name)
    {
        Config.PlayerName = name;
        SaveConfig();
    }

    public static void SetJudgeTime(HitNoteResult type, int time)
    {
        if (!Config.JudgeRange.ContainsKey(type)) return;
        Config.JudgeRange[type] = time;

        SaveConfig();
    }

    public static void SetDrumSoundType(int index)
    {
        Config.DrumSoundType = index;
        SaveConfig();
    }

    public static void SetJudgeTimeAdjust(int time)
    {
        Config.JudgeTimeAdjust = time;
        SaveConfig();
    }
    public static void SetNoteAdjust(int time)
    {
        Config.NoteAdjust = time;
        SaveConfig();
    }
    public static void SetButton(KeyType type, Key key)
    {
        if (key != null)
        {
            switch (type)
            {
                case KeyType.LeftDon:
                    Config.DonLeft = key;
                    ParseKey(0);
                    break;
                case KeyType.RightDon:
                    Config.DonRight = key;
                    ParseKey(1);
                    break;
                case KeyType.LeftKa:
                    Config.KaLeft = key;
                    ParseKey(2);
                    break;
                case  KeyType.RightKa:
                    Config.KaRight = key;
                    ParseKey(3);
                    break;
                case KeyType.Option:
                    Config.Option = key;
                    ParseKey(4);
                    break;
            }
            SaveConfig();
        }
    }
    public static void SetPadButton(KeyType type)
    {
        switch (type)
        {
            case KeyType.LeftDon:
                ParseKey(0);
                break;
            case KeyType.RightDon:
                ParseKey(1);
                break;
            case KeyType.LeftKa:
                ParseKey(2);
                break;
            case KeyType.RightKa:
                ParseKey(3);
                break;
            case KeyType.Option:
                ParseKey(4);
                break;
        }
        SaveConfig();
    }

    private static void ParseKey(int type)
    {
        InputActionType input_type = InputActionType.Button;
        switch (type)
        {
            case 0:
                {
                    if (DonLeft != null)
                    {
                        DonLeft.Disable();
                        DonLeft = null;
                    }
                    if (DonLeft2 != null)
                    {
                        DonLeft2.Disable();
                        DonLeft2 = null;
                    }
                    if (!Config.DirectInput)
                        DonLeft = new InputAction(name: "DonLeft", input_type, binding: Config.DonLeft.Path, interactions: "Press(pressPoint=0.1)");
                    if (Config.GamePadKeys.TryGetValue(KeyType.LeftDon, out List<string> pathes))
                    {
                        if (pathes.Count > 0 && !string.IsNullOrEmpty(pathes[0]))
                        {
                            if (DonLeft != null)
                                DonLeft.AddBinding(pathes[0]);
                            else
                                DonLeft = new InputAction(name: "DonLeft", input_type, binding: pathes[0], interactions: "Press(pressPoint=0.1)");
                        }

                        if (pathes.Count > 1 && !string.IsNullOrEmpty(pathes[1]))
                        {
                            DonLeft2 = new InputAction(name: "DonLeft2", input_type, binding: pathes[1], interactions: "Press(pressPoint=0.1)");
                            DonLeft2.Enable();
                        }
                    }
                    if (DonLeft != null) DonLeft.Enable();
                }
                break;
            case 1:
                {
                    if (DonRight != null)
                    {
                        DonRight.Disable();
                        DonRight = null;
                    }
                    if (DonRight2 != null)
                    {
                        DonRight2.Disable();
                        DonRight2 = null;
                    }
                    if (!Config.DirectInput)
                        DonRight = new InputAction(name: "DonRight", input_type, binding: Config.DonRight.Path, interactions: "Press(pressPoint=0.1)");
                    if (Config.GamePadKeys.TryGetValue(KeyType.RightDon, out List<string> pathes))
                    {
                        if (pathes.Count > 0 && !string.IsNullOrEmpty(pathes[0]))
                        {
                            if (DonRight != null)
                                DonRight.AddBinding(pathes[0]);
                            else
                                DonRight = new InputAction(name: "DonRight", input_type, binding: pathes[0], interactions: "Press(pressPoint=0.1)");
                        }

                        if (pathes.Count > 1 && !string.IsNullOrEmpty(pathes[1]))
                        {
                            DonRight2 = new InputAction(name: "DonRight2", input_type, binding: pathes[1], interactions: "Press(pressPoint=0.1)");
                            DonRight2.Enable();
                        }
                    }
                    if (DonRight != null) DonRight.Enable();
                }
                break;
            case 2:
                {
                    if (KaLeft != null)
                    {
                        KaLeft.Disable();
                        KaLeft = null;
                    }
                    if (KaLeft2 != null)
                    {
                        KaLeft2.Disable();
                        KaLeft2 = null;
                    }
                    if (!Config.DirectInput)
                        KaLeft = new InputAction(name: "KaLeft", input_type, binding: Config.KaLeft.Path, interactions: "Press(pressPoint=0.1)");
                    if (Config.GamePadKeys.TryGetValue(KeyType.LeftKa, out List<string> pathes))
                    {
                        if (pathes.Count > 0 && !string.IsNullOrEmpty(pathes[0]))
                        {
                            if (KaLeft != null)
                                KaLeft.AddBinding(pathes[0]);
                            else
                                KaLeft = new InputAction(name: "KaLeft", input_type, binding: pathes[0], interactions: "Press(pressPoint=0.1)");
                        }

                        if (pathes.Count > 1 && !string.IsNullOrEmpty(pathes[1]))
                        {
                            KaLeft2 = new InputAction(name: "KaLeft2", input_type, binding: pathes[1], interactions: "Press(pressPoint=0.1)");
                            KaLeft2.Enable();
                        }
                    }
                    if (KaLeft != null) KaLeft.Enable();
                }
                break;
            case 3:
                {
                    if (KaRight != null)
                    {
                        KaRight.Disable();
                        KaRight = null;
                    }
                    if (KaRight2 != null)
                    {
                        KaRight2.Disable();
                        KaRight2 = null;
                    }
                    if (!Config.DirectInput)
                        KaRight = new InputAction(name: "KaRight", input_type, binding: Config.KaRight.Path, interactions: "Press(pressPoint=0.1)");
                    if (Config.GamePadKeys.TryGetValue(KeyType.RightKa, out List<string> pathes))
                    {
                        if (pathes.Count > 0 && !string.IsNullOrEmpty(pathes[0]))
                        {
                            if (KaRight != null)
                                KaRight.AddBinding(pathes[0]);
                            else
                                KaRight = new InputAction(name: "KaRight", input_type, binding: pathes[0], interactions: "Press(pressPoint=0.1)");
                        }

                        if (pathes.Count > 1 && !string.IsNullOrEmpty(pathes[1]))
                        {
                            KaRight2 = new InputAction(name: "KaRight2", input_type, binding: pathes[1], interactions: "Press(pressPoint=0.1)");
                            KaRight2.Enable();
                        }
                    }
                    if (KaRight != null) KaRight.Enable();
                }
                break;
        }
    }

    public static void ParseKey2P(KeyType type)
    {
        InputActionType input_type = InputActionType.Button;
        switch (type)
        {
            case KeyType.LeftDon:
                {
                    if (DonLeft2P != null)
                    {
                        DonLeft2P.Disable();
                        DonLeft2P = null;
                    }
                    string path = Config.GamePadKeys2P[KeyType.LeftDon];
                    if (!string.IsNullOrEmpty(path) && (!Config.DirectInput || !path.Contains("Keyboard")))
                        DonLeft2P = new InputAction(name: "DonLeft2P", input_type, binding: path, interactions: "Press(pressPoint=0.1)");
                    if (DonLeft2P != null) DonLeft2P.Enable();
                }
                break;
            case KeyType.RightDon:
                {
                    if (DonRight2P != null)
                    {
                        DonRight2P.Disable();
                        DonRight2P = null;
                    }
                    string path = Config.GamePadKeys2P[KeyType.RightDon];
                    if (!string.IsNullOrEmpty(path) && (!Config.DirectInput || !path.Contains("Keyboard")))
                        DonRight2P = new InputAction(name: "DonRight2P", input_type, binding: path, interactions: "Press(pressPoint=0.1)");
                    if (DonRight2P != null) DonRight2P.Enable();
                }
                break;
            case KeyType.LeftKa:
                {
                    if (KaLeft2P != null)
                    {
                        KaLeft2P.Disable();
                        KaLeft2P = null;
                    }
                    string path = Config.GamePadKeys2P[KeyType.LeftKa];
                    if (!string.IsNullOrEmpty(path) && (!Config.DirectInput || !path.Contains("Keyboard")))
                        KaLeft2P = new InputAction(name: "KaLeft2P", input_type, binding: path, interactions: "Press(pressPoint=0.1)");
                    if (KaLeft2P != null) KaLeft2P.Enable();
                }
                break;
            case KeyType.RightKa:
                {
                    if (KaRight2P != null)
                    {
                        KaRight2P.Disable();
                        KaRight2P = null;
                    }
                    string path = Config.GamePadKeys2P[KeyType.RightKa];
                    if (!string.IsNullOrEmpty(path) && (!Config.DirectInput || !path.Contains("Keyboard")))
                        KaRight2P = new InputAction(name: "KaRight2P", input_type, binding: path, interactions: "Press(pressPoint=0.1)");
                    if (KaRight2P != null) KaRight2P.Enable();
                }
                break;
        }
    }

    public static void SetAutoPlay(bool auto)
    {
        Config.Special = auto ? Special.AutoPlay : Special.None;
        SaveConfig();
    }

    public static void SetSpecialOption(Special option)
    {
        Config.Special = option;
        SaveConfig();
    }

    public static void SetMusicVolume(float volume)
    {
        Config.MusicVolume = volume;
        SaveConfig();
    }
    public static void SetDrumVolume(float volume)
    {
        Config.EffectVolume = volume;
        SaveConfig();
    }
    public static void SetSFXVolume(float volume)
    {
        Config.SFXVolume = volume;
        SaveConfig();
    }

    public static void AddPlayTimes(string song, Difficulty difficulty)
    {
        Record.Record.AddPlayCount(song, difficulty);
        SaveRecord();
    }

    public static void SetScore(string song, string player, Difficulty difficulty, bool clear, int score,
        int perfect, int good, int bad, int max_combo, int max_hits, int rank, bool is_win, List<int> sections)
    {
        Record.Record.UpdateScore(song, player, difficulty, clear, score, perfect, good, bad, max_combo, max_hits, rank, is_win, sections);
        SaveRecord();
    }

    public static void SetLastPlay(int diff)
    {
        Config.LastPlay = SelectedInfo.Title;
        if (!Config.PlayList.Contains(SelectedInfo.Title))
        {
            Config.PlayList.Add(SelectedInfo.Title);
            if (Config.PlayList.Count > 10) Config.PlayList.RemoveAt(0);
        }

        Config.LastDiff = diff;
        SaveConfig();
    }

    public static void SetFavorite(string song, bool add)
    {
        if (add)
        {
            Config.Favorites.Add(song);
        }
        else
        {
            Config.Favorites.RemoveAll(t=> t == song);
        }
        SaveConfig();
    }

    public static void SetResolution(int width, int height)
    {
        Config.ResolutionWidth = width;
        Config.ResolutionHeight = height;
        SaveConfig();
    }

    public static void SetFullScreen(bool full)
    {
        Config.FullScreen = full;
        SaveConfig();
    }

    public static void SetVSync(bool on)
    {
        Config.VSync = on;
        SaveConfig();
    }
    public static void SetStyle(int style)
    {
        Config.GameStyle = style;
        Style = (GameStyle)style;
        SaveConfig();
    }

    public static void SetAutoRectify(bool rectify)
    {
        Config.AutoRectify = rectify;
        SaveConfig();
    }

    public static string Translate(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;
        if (translations == null) return key;

        string[] sArray = key.Split(new char[2] { '\\', '\\' });
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        foreach (string i in sArray)
        {
            string new_key = i.Replace("'", "''");
            DataRow[] rows = translations.Select(string.Format("key = '{0}'", new_key));
            if (rows.Length == 0)
                result.Append(i);
            else
                result.Append(rows[0]["translation"].ToString().Replace("\\n", "\n"));
        }
        return result.ToString();
    }

    public static void ChangeLanguage(Locale locale)
    {
        Config.Locale = locale;
        SaveConfig();
        LoadTranslation();
    }

    public static KeyValuePair<ClearState, string> GetTitle()
    {
        List<AcVersion> versions = new List<AcVersion>();
        Dictionary<AcVersion, DanLevel> max = new Dictionary<AcVersion, DanLevel>();
        Dictionary<AcVersion, ClearState> max_state = new Dictionary<AcVersion, ClearState>();

        Array all = Enum.GetValues(typeof(AcVersion));
        foreach (AcVersion version in all)
        {
            DanLevel this_max = Record.GetMaxLeval(version);
            if (this_max != DanLevel.None)
            {
                max.Add(version, this_max);
                max_state.Add(version, Record.GetClear(version, this_max));
                versions.Add(version);
            }
        }

        if (versions.Count > 0)
        {
            if (versions.Count > 1)
            {
                versions.Sort((x, y) => {
                    return max[x] > max[y] ? -1 : max[x] < max[y] ? 1 :
                    (max_state[x] > max_state[y] ? -1 : max_state[x] < max_state[y] ? 1 : (x > y ? -1 : 1));
                });
            }

            StringBuilder title = new StringBuilder();
            for (int i = Mathf.Min(2, versions.Count); i > 0; i--)
            {
                if (versions.Count > 1 && i == 1) title.Append("♦");
                title.Append(Translate(versions[i - 1].ToString()));
                title.Append(Translate(max[versions[i - 1]].ToString()));
            }

            return new KeyValuePair<ClearState, string>(max_state[versions[0]], title.ToString());
        }
        else
            return new KeyValuePair<ClearState, string>(ClearState.NoClear, string.Empty);
    }
}

public class ReplayConfig
{
    public Dictionary<HitNoteResult, int> JudgeRange { set; get; }
    public int JudgeTimeAdjust { set; get; }
    public int NoteAdjust { set; get; }
    public string PlayerName { set; get; }
    public Special Special { set; get; }
    public int ScoreMode { set; get; }
    public string Emotion { set; get; }
    public string Body { set; get; }
    public string Head { set; get; }
    public string BodySkin { set; get; }
    public string Tatoo { set; get; }
    public string SkinCos { set; get; }
    public string SkinSub { set; get; }
    public string Face { set; get; }
    public string Skin { set; get; }
    public int DrumSoundType { set; get; }

    public Speed Speed = Speed.Normal;
    public RandomType Random = RandomType.None;
    public bool Steal = false;
    public bool Reverse = false;
}

public class Config
{
    public bool FullScreen { set; get; }
    public int ResolutionWidth{ set; get; }
    public int ResolutionHeight { set; get; }
    public bool VSync { set; get; }
    public Dictionary<HitNoteResult, int> JudgeRange { set; get; }
    public int JudgeTimeAdjust { set; get; }
    public int NoteAdjust { set; get; }
    public string PlayerName { set; get; }
    public float MusicVolume { set; get; }
    public float EffectVolume { set; get; }
    public Key DonRight { set; get; }
    public Key DonLeft { set; get; }
    public Key KaRight { set; get; }
    public Key KaLeft { set; get; }
    public Key Option { set; get; }
    public Dictionary<KeyType, List<string>> GamePadKeys { set; get; }
    public Dictionary<KeyType, string> GamePadKeys2P { set; get; }
    public Special Special { set; get; }
    public int ScoreMode { set; get; }
    public string LastPlay { set; get; }
    public int LastDiff { set; get; }
    public Dictionary<int, SortColor> SortColor { set; get; }
    public SortColor FavorColor { set; get; }
    public List<string> Favorites { set; get; }
    public string Emotion { set; get; }
    public string Body { set; get; }
    public string Head { set; get; }
    public string BodySkin { set; get; }
    public string Tatoo { set; get; }
    public string SkinCos { set; get; }
    public string SkinSub { set; get; }
    public string Face { set; get; }
    public string Skin { set; get; }
    public int DrumSoundType { set; get; }
    public int GameStyle { set; get; }
    public Locale Locale { set;get; }
    public bool AutoRectify { set; get; }
    public bool PlayRight { set; get; }
    public float SFXVolume { set; get; }
    public float InputCooldown { set; get; }
    public Dictionary<int, List<string>> Presets { set; get; }
    public List<string> PlayList { set; get; }

    public Speed Speed = Speed.Normal;
    public RandomType Random = RandomType.None;
    public bool Steal = false;
    public bool Reverse = false;
    public bool DirectInput = false;
    public int InputMode = 1;
    public int Controller = 0;
    public float DrumScale = 1;

    public Config Copy(bool save = true)
    {
        Config config = new Config();
        config.JudgeRange = JudgeRange;
        config.JudgeTimeAdjust = JudgeTimeAdjust;
        config.NoteAdjust = NoteAdjust;
        config.PlayerName = PlayerName;
        config.MusicVolume = MusicVolume;
        config.EffectVolume = EffectVolume;
        config.DonRight = DonRight;
        config.DonLeft = DonLeft;
        config.KaRight = KaRight;
        config.KaLeft = KaLeft;
        config.Option = Option;
        config.GamePadKeys = GamePadKeys;
        config.GamePadKeys2P = GamePadKeys2P;
        config.Special = Special;
        config.ScoreMode = ScoreMode;
        config.LastPlay = LastPlay;
        config.LastDiff = LastDiff;
        config.SortColor = SortColor;
        config.FavorColor = FavorColor;
        config.Favorites = Favorites;
        config.Emotion = Emotion;
        config.Body = Body;
        config.Head = Head;
        config.BodySkin = BodySkin;
        config.SkinCos = SkinCos;
        config.SkinSub = SkinSub;
        config.Face = Face;
        config.Skin = Skin;
        config.Tatoo = Tatoo;
        config.DrumSoundType = DrumSoundType;
        config.FullScreen = FullScreen;
        config.ResolutionWidth = ResolutionWidth;
        config.ResolutionHeight = ResolutionHeight;
        config.VSync = VSync;
        config.GameStyle = GameStyle;
        config.Locale = Locale;
        config.AutoRectify = AutoRectify;
        config.Presets = Presets;
        config.PlayRight = PlayRight;
        config.DirectInput = DirectInput;
        config.PlayList = PlayList;
        config.InputMode = InputMode;
        config.SFXVolume = SFXVolume;
        config.InputCooldown = InputCooldown;
        config.Controller = Controller;
        config.DrumScale = DrumScale;
        if (!save)
        {
            config.Reverse = Reverse;
            config.Speed = Speed;
            config.Random = Random;
            config.Steal = Steal;
        }
        return config;
    }

    public ReplayConfig Copy2Replay()
    {
        ReplayConfig config = new ReplayConfig();
        config.JudgeRange = JudgeRange;
        config.JudgeTimeAdjust = JudgeTimeAdjust;
        config.NoteAdjust = NoteAdjust;
        config.PlayerName = PlayerName;
        config.Special = Special;
        config.ScoreMode = ScoreMode;
        config.Emotion = Emotion;
        config.Body = Body;
        config.Head = Head;
        config.BodySkin = BodySkin;
        config.SkinCos = SkinCos;
        config.SkinSub = SkinSub;
        config.Face = Face;
        config.Skin = Skin;
        config.Tatoo = Tatoo;
        config.DrumSoundType = DrumSoundType;
        config.Reverse = Reverse;
        config.Speed = Speed;
        config.Random = Random;
        config.Steal = Steal;
        return config;
    }

    public void CopyReplayConfig(ReplayConfig replay)
    {
        JudgeRange = replay.JudgeRange;
        JudgeTimeAdjust = replay.JudgeTimeAdjust;
        NoteAdjust = replay.NoteAdjust;
        PlayerName = replay.PlayerName;
        Special = replay.Special;
        Reverse = replay.Reverse;
        Speed = replay.Speed;
        Random = replay.Random;
        Steal= replay.Steal;
        ScoreMode = replay.ScoreMode;
        Emotion = replay.Emotion;
        Body = replay.Body;
        Head = replay.Head;
        BodySkin = replay.BodySkin;
        SkinCos = replay.SkinCos;
        SkinSub = replay.SkinSub;
        Face = replay.Face;
        Skin = replay.Skin;
        Tatoo = replay.Tatoo;
        DrumSoundType = replay.DrumSoundType;
    }
}

public class Key
{
    public string Name { private set; get; }
    public string Path { private set; get; }
    public Key(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public override bool Equals(object another)
    {
        if (another is Key key)
            return Name == key.Name && Path == key.Path;

        return false;
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode() + Path.GetHashCode() * 2;
    }
}

public class SortColor
{
    public SortColor(string title, string sub, string color)
    {
        Title = title;
        SubTitle = sub;
        Color = color;
    }

    public string Title { set; get; }
    public string SubTitle { set; get; }
    public string Color { set; get; }

    public SortColor Copy()
    {
        SortColor new_color = new SortColor(Title, SubTitle, Color);
        return new_color;
    }
}

public class Record : DataTable
{
    public Record() : base("record")
    {
        Columns.Add("title", typeof(string));
        Columns.Add("mode", typeof(int));
        Columns.Add("difficulty", typeof(int));
        Columns.Add("play_count", typeof(int));
        Columns.Add("player", typeof(string));
        Columns.Add("clear", typeof(bool));
        Columns.Add("score", typeof(int));
        Columns.Add("perfect", typeof(int));
        Columns.Add("good", typeof(int));
        Columns.Add("bad", typeof(int));
        Columns.Add("max_combo", typeof(int));
        Columns.Add("max_hits", typeof(int));
        Columns.Add("rank", typeof(int));
        Columns.Add("ai_win", typeof(bool));
        Columns.Add("ai_sections", typeof(string));
        Columns.Add("crown", typeof(int));
    }
    public bool IsClear(string song, Difficulty difficulty)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 0", song, (int)difficulty));
        if (rows.Length > 0 && bool.Parse(rows[0]["clear"].ToString())) return true;

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 1", song, (int)difficulty));
        if (rows.Length > 0 && bool.Parse(rows[0]["clear"].ToString())) return true;

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2", song, (int)difficulty));
        if (rows.Length > 0 && bool.Parse(rows[0]["clear"].ToString())) return true;

        return false;
    }

    public int GetAiWinCount()
    {
        DataRow[] rows = Select("(difficulty = 3 or difficulty = 4) and mode = 2 and ai_win = true");
        return rows.Length;
    }

    public bool IsWin(string song, Difficulty difficulty)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2", song, (int)difficulty));
        if (rows.Length > 0 && bool.TryParse(rows[0]["ai_win"].ToString(), out bool old_win) && old_win)
            return true;

        return false;
    }
    
    public List<int> GetSections(string song, Difficulty difficulty)
    {
        List<int> result = new List<int>();
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2", song, (int)difficulty));
        if (rows.Length > 0)
        {
            string str = rows[0]["ai_sections"].ToString();
            for (int i = 0; i < str.Length; i++)
            {
                if (int.TryParse(str[i].ToString(), out int section_rank))
                    result.Add(section_rank);
                else
                {
                    result.Clear();
                    break;
                }
            }
        }

        return result;
    }

    public int GetCrown(string song, Difficulty difficulty)
    {
        song = song.Replace("'", "''");
        int crown = 0;
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 0 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            crown = 1;
            if (int.TryParse(rows[0]["crown"].ToString(), out int _crown))
            {
                if (_crown > crown) crown = _crown;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0)
            {
                crown = 2;
                if (int.Parse(rows[0]["good"].ToString()) == 0)
                    return 3;
            }
        }

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 1 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (crown < 1) crown = 1;
            if (int.TryParse(rows[0]["crown"].ToString(), out int _crown))
            {
                if (_crown > crown) crown = _crown;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0)
            {
                crown = 2;
                if (int.Parse(rows[0]["good"].ToString()) == 0)
                    return 3;
            }
        }

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (crown < 1) crown = 1;
            if (int.TryParse(rows[0]["crown"].ToString(), out int _crown))
            {
                if (_crown > crown) crown = _crown;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0)
            {
                crown = 2;
                if (int.Parse(rows[0]["good"].ToString()) == 0)
                    return 3;
            }
        }

        return crown;
    }

    public int GetCrownByScoreMode(string song, Difficulty difficulty, int score_mode)
    {
        int crown = 0;
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2 and clear = true", song, (int)difficulty, score_mode));
        if (rows.Length > 0)
        {
            crown = 1;
            if (int.TryParse(rows[0]["crown"].ToString(), out int _crown))
            {
                if (_crown > crown) return crown = _crown;
            }
            else if (int.TryParse(rows[0]["bad"].ToString(), out int bad) && bad == 0)
            {
                crown = 2;
                if (int.TryParse(rows[0]["good"].ToString(), out int good) && good == 0)
                    return 3;
            }
        }

        return crown;
    }

    public bool IsFullCombo(string song, Difficulty difficulty)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 0 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (int.TryParse(rows[0]["crown"].ToString(), out int crown))
            {
                if (crown > 1) return true;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0)
            {
                return true;
            }
        }
        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 1 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (int.TryParse(rows[0]["crown"].ToString(), out int crown))
            {
                if (crown > 1) return true;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0)
            {
                return true;
            }
        }

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (int.TryParse(rows[0]["crown"].ToString(), out int crown))
            {
                if (crown > 1) return true;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsDFullCombo(string song, Difficulty difficulty)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 0 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (int.TryParse(rows[0]["crown"].ToString(), out int crown))
            {
                if (crown == 3) return true;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0 && int.Parse(rows[0]["good"].ToString()) == 0)
            {
                return true;
            }
        }

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 1 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (int.TryParse(rows[0]["crown"].ToString(), out int crown))
            {
                if (crown == 3) return true;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0 && int.Parse(rows[0]["good"].ToString()) == 0)
            {
                return true;
            }
        }

        rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2 and clear = true", song, (int)difficulty));
        if (rows.Length > 0)
        {
            if (int.TryParse(rows[0]["crown"].ToString(), out int crown))
            {
                if (crown == 3) return true;
            }
            else if (int.Parse(rows[0]["bad"].ToString()) == 0 && int.Parse(rows[0]["good"].ToString()) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public int GetScore(string song, Difficulty difficulty, int score_mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, score_mode));
        if (rows.Length > 0)
            return int.Parse(rows[0]["score"].ToString());

        return 0;
    }
    public int GetPlayTimes(string song)
    {
        song = song.Replace("'", "''");
        int count = 0;
        DataRow[] rows = Select(string.Format("title = '{0}'", song));
        foreach (DataRow row in rows)
            count += int.Parse(rows[0]["play_count"].ToString());

        return 0;
    }
    public int GetRank(string song, Difficulty difficulty)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = 2", song, (int)difficulty));
        if (rows.Length > 0)
            return int.Parse(rows[0]["rank"].ToString());

        return 0;
    }
    public int GetPerfect(string song, Difficulty difficulty, Score mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)mode));
        if (rows.Length > 0)
            return int.Parse(rows[0]["perfect"].ToString());

        return 0;
    }
    public int GetGood(string song, Difficulty difficulty, Score mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)mode));
        if (rows.Length > 0)
            return int.Parse(rows[0]["good"].ToString());

        return 0;
    }
    public int GetBad(string song, Difficulty difficulty, Score mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)mode));
        if (rows.Length > 0)
            return int.Parse(rows[0]["bad"].ToString());

        return 0;
    }
    public int GetMaxCombo(string song, Difficulty difficulty, Score mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)mode));
        if (rows.Length > 0)
            return int.Parse(rows[0]["max_combo"].ToString());

        return 0;
    }
    public int GetMaxHits(string song, Difficulty difficulty, Score mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)mode));
        if (rows.Length > 0)
            return int.Parse(rows[0]["max_hits"].ToString());

        return 0;
    }

    public string GetBestPlayer(Difficulty difficulty, string song, Score mode)
    {
        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)mode));
        if (rows.Length > 0)
            return rows[0]["player"].ToString();

        return string.Empty;
    }

    public void AddPlayCount(string song, Difficulty difficulty)
    {
        if (Mode == PlayMode.Practice) return;

        song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", song, (int)difficulty, (int)Mode));
        if (rows.Length > 0)
        {
            rows[0]["play_count"] = int.Parse(rows[0]["play_count"].ToString()) + 1;
        }
        else
        {
            DataRow new_row = NewRow();
            new_row["title"] = song;
            new_row["mode"] = (int)Mode;
            new_row["difficulty"] = (int)difficulty;
            new_row["play_count"] = 1;

            Rows.Add(new_row);
        }
    }

    public void UpdateScore(string song, string player, Difficulty difficulty, bool clear, int score, int perfect,
        int good, int bad, int max_combo, int max_hits, int rank, bool is_win, List<int> sections)
    {
        string _song = song.Replace("'", "''");
        DataRow[] rows = Select(string.Format("title = '{0}' and difficulty = {1} and mode = {2}", _song, (int)difficulty, GameSetting.Config.ScoreMode));
        bool add = false;
        int old_crown = 0;
        int crown = !clear ? 0 : (bad > 0 ? 1 : (good > 0 ? 2 : 3));
        //Debug.Log(string.Format("new crown {0}", crown));
        if (rows.Length > 0)
        {
            bool old_clear = bool.Parse(rows[0]["clear"].ToString());
            int old_rank = int.Parse(rows[0]["rank"].ToString());
            int old_score = int.Parse(rows[0]["score"].ToString());
            int old_bad = int.Parse(rows[0]["bad"].ToString());
            int old_perfect = int.Parse(rows[0]["perfect"].ToString());
            int old_good = int.Parse(rows[0]["good"].ToString());
            int old_max = int.Parse(rows[0]["max_combo"].ToString());
            int old_hits = int.Parse(rows[0]["max_hits"].ToString());
            if (old_clear)
            {
                if (int.TryParse(rows[0]["crown"].ToString(), out int _crown))
                    old_crown = _crown;
                else
                {
                    old_crown = old_bad > 0 ? 1 : (old_good == 0 ? 3 : 2);
                    //Debug.Log(string.Format("old crown {0}", old_crown));
                    add = true;
                }
            }
            bool old_win = false;
            if (bool.TryParse(rows[0]["ai_win"].ToString(), out bool result)) old_win = result;
            List<int> old_sections = new List<int>();
            string str = rows[0]["ai_sections"].ToString();
            for (int i = 0; i < str.Length; i++)
            {
                if (int.TryParse(str[i].ToString(), out int section_rank))
                    old_sections.Add(section_rank);
                else
                {
                    old_sections.Clear();
                    break;
                }
            }
            
            if (old_score < score || (!old_clear && clear) || rank > old_rank || max_combo > old_max || max_hits > old_hits || old_sections.Count < sections.Count || crown > old_crown)
                add = true;

            if ((!is_win && old_win) || (old_sections.Count > sections.Count)) sections = old_sections;

            if (add)
            {
                max_combo = Mathf.Max(max_combo, old_max);
                max_hits = Mathf.Max(max_hits, old_hits);
                crown = Mathf.Max(crown, old_crown);

                if (score < old_score)
                {
                    score = old_score;
                    perfect = old_perfect;
                    good = old_good;
                    bad = old_bad;
                }

                clear = clear || old_clear;
                rank = Mathf.Max(rank, old_rank);

                is_win = is_win || old_win;

                foreach (DataRow row in rows)
                    Rows.Remove(row);
            }
        }
        else
        {
            add = true;
        }

        if (add)
        {
            DataRow new_row = NewRow();
            new_row["title"] = song;
            new_row["mode"] = GameSetting.Config.ScoreMode;
            new_row["player"] = player;
            new_row["difficulty"] = (int)difficulty;
            new_row["clear"] = clear;
            new_row["score"] = score;
            new_row["perfect"] = perfect;
            new_row["good"] = good;
            new_row["bad"] = bad;
            new_row["max_combo"] = max_combo;
            new_row["max_hits"] = max_hits;
            new_row["rank"] = rank;
            new_row["ai_win"] = is_win;
            new_row["crown"] = crown;

            string section_str = string.Empty;
            foreach (int i in sections)
                section_str += i.ToString();
            new_row["ai_sections"] = section_str;

            Rows.Add(new_row);
        }
    }
}

public class PlayerRecord
{
    public Record Record;
    public DaniRecord DaniRecord;
    public string Title = string.Empty;
    public DataTable Titles;
    public List<int> Rewards;

    public PlayerRecord()
    {
        Record = new Record();
        DaniRecord = new DaniRecord();
        Titles = new DataTable("titles");
        Titles.Columns.Add("version", typeof(int));
        Titles.Columns.Add("course", typeof(int));
        Titles.Columns.Add("clear", typeof(int));

        Rewards = new List<int>();
    }

    public void AddTitle(ClearState clear, AcVersion version, DanLevel level)
    {
        DataRow[] rows = Titles.Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            foreach (DataRow row in rows)
                Titles.Rows.Remove(row);
        }

        DataRow new_row = Titles.NewRow();
        new_row["version"] = (int)version;
        new_row["course"] = (int)level;
        new_row["clear"] = (int)clear;
        Titles.Rows.Add(new_row);
    }

    public ClearState GetClear(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Titles.Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
            return (ClearState)int.Parse(rows[0]["clear"].ToString());

        return ClearState.NoClear;
    }

    public DanLevel GetMaxLeval(AcVersion version)
    {
        DataRow[] rows = Titles.Select(string.Format("version = {0}", (int)version));
        if (rows.Length > 0)
        {
            List<DataRow> list = new List<DataRow>(rows);
            list.Sort((x, y) => { return int.Parse(x["course"].ToString()) > int.Parse(y["course"].ToString()) ? -1 : 1; });
            return (DanLevel)int.Parse(list[0]["course"].ToString());
        }

        return DanLevel.None;
    }
}

public class AndroidConfig
{
    public GraphicLevel Level { set; get; }
    public bool SaveHiScore { set; get; }
    public AndroidConfig() { }
}

public class DaniRecord : DataTable
{
    public DaniRecord() : base("dani_record")
    {
        Columns.Add("version", typeof(int));
        Columns.Add("course", typeof(int));
        Columns.Add("clear", typeof(int));
        Columns.Add("gauge", typeof(int));
        Columns.Add("score", typeof(int));
        Columns.Add("max_combo", typeof(int));
        Columns.Add("perfect", typeof(int));
        Columns.Add("good1", typeof(int));
        Columns.Add("bad", typeof(int));
        Columns.Add("drumrolls1", typeof(int));
        Columns.Add("drumrolls2", typeof(int));
        Columns.Add("drumrolls3", typeof(int));
        Columns.Add("hits", typeof(int));
        Columns.Add("good2", typeof(int));
        Columns.Add("good3", typeof(int));
    }

    public ClearState GetClear(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
            return (ClearState)int.Parse(rows[0]["clear"].ToString());

        return ClearState.NoClear;
    }

    public int GetGauge(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
            return int.Parse(rows[0]["gauge"].ToString());

        return 0;
    }

    public int GetScore(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
            return int.Parse(rows[0]["score"].ToString());

        return 0;
    }
    public int GetMaxCombo(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
            return int.Parse(rows[0]["max_combo"].ToString());

        return 0;
    }

    public int GetPerfect(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            return int.Parse(rows[0]["perfect"].ToString());
        }

        return 0;
    }

    public int GetBad(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            return int.Parse(rows[0]["bad"].ToString());
        }

        return 0;
    }
    public int GetTotalRolls(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
            return int.Parse(rows[0]["drumrolls1"].ToString()) + int.Parse(rows[0]["drumrolls2"].ToString()) + int.Parse(rows[0]["drumrolls3"].ToString());

        return 0;
    }

    public int GetRolls(AcVersion version, DanLevel level, int index)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            switch (index)
            {
                case 0:
                    return int.Parse(rows[0]["drumrolls1"].ToString());
                case 1:
                    return int.Parse(rows[0]["drumrolls2"].ToString());
                case 2:
                    return int.Parse(rows[0]["drumrolls3"].ToString());
            }
        }

        return 0;
    }


    public int GetTotalGood(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            return int.Parse(rows[0]["good1"].ToString()) + int.Parse(rows[0]["good2"].ToString()) + int.Parse(rows[0]["good3"].ToString());
        }

        return 0;
    }

    public int GetGoods(AcVersion version, DanLevel level, int index)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            switch (index)
            {
                case 0:
                    return int.Parse(rows[0]["good1"].ToString());
                case 1:
                    return int.Parse(rows[0]["good2"].ToString());
                case 2:
                    return int.Parse(rows[0]["good3"].ToString());
            }
        }

        return 0;
    }

    public int GetHits(AcVersion version, DanLevel level)
    {
        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            return int.Parse(rows[0]["hits"].ToString());
        }

        return 0;
    }

    public void UpdateScore(AcVersion version, DanLevel level, ClearState state, int gauge, int score, int max_combo, int perfect, int good1, int good2, int good3,
        int bad, int rolls1, int rolls2, int rolls3, int hits)
    {
        ClearState old_clear = GetClear(version, level);
        if (state > old_clear) old_clear = state;
        int old_gauge = Mathf.Max(gauge, GetGauge(version, level));
        int old_socre = Mathf.Max(score, GetScore(version, level));
        int old_max = Mathf.Max(max_combo, GetMaxCombo(version, level));
        int old_p = Mathf.Max(perfect, GetPerfect(version, level));
        int old_g1 = Mathf.Max(good1, GetGoods(version, level, 0));
        int old_g2 = Mathf.Max(good2, GetGoods(version, level, 1));
        int old_g3 = Mathf.Max(good3, GetGoods(version, level, 2));
        int old_b = Mathf.Min(bad, GetBad(version, level));
        int old_rolls1 = Mathf.Max(rolls1, GetRolls(version, level, 0));
        int old_rolls2 = Mathf.Max(rolls2, GetRolls(version, level, 1));
        int old_rolls3 = Mathf.Max(rolls3, GetRolls(version, level, 2));
        int old_hits = Mathf.Max(hits, GetHits(version, level));

        DataRow[] rows = Select(string.Format("version = {0} and course = {1}", (int)version, (int)level));
        if (rows.Length > 0)
        {
            foreach (DataRow row in rows)
                Rows.Remove(row);
        }


        DataRow new_row = NewRow();
        new_row["version"] = (int)version;
        new_row["course"] = (int)level;
        new_row["clear"] = (int)old_clear;
        new_row["gauge"] = old_gauge;
        new_row["score"] = old_socre;
        new_row["max_combo"] = old_max;
        new_row["perfect"] = old_p;
        new_row["good1"] = old_g1;
        new_row["good2"] = old_g2;
        new_row["good3"] = old_g3;
        new_row["bad"] = old_b;
        new_row["drumrolls1"] = old_rolls1;
        new_row["drumrolls2"] = old_rolls2;
        new_row["drumrolls3"] = old_rolls3;
        new_row["hits"] = old_hits;

        Rows.Add(new_row);
    }
}
