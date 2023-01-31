using Newtonsoft.Json.Linq;
using System;
using System.Text;
using UnityEngine;

public static class TimeUtil
{
    private static int _server_time_when_init;
    private static int _client_time_when_init;
    public static readonly DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public static int now => _server_time_when_init - _client_time_when_init + _now();
    

    public static DateTime nowDateTime()
    {
        return timestampToDateTime(now);
    }

    private static int _now()
    {
        return (int)(DateTime.Now - TimeZoneInfo.Local.BaseUtcOffset - new DateTime(1970, 1, 1)).TotalSeconds;
    }
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize(double serverTime = 0)
    {
        _client_time_when_init = _now();
        _server_time_when_init = Convert.ToInt32(Math.Floor(serverTime));
    }

    public static void SetServerTime(int server_time)
    {
        _server_time_when_init = server_time;
        _client_time_when_init = _now();
    }



    public static string formatTime(int time, string format = "u")
    {
        var date = timestampToDateTime(time);

        return date.ToString(format);
    }

    public static DateTime timestampToDateTime(int timestamp)
    {
        DateTime date_time_start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long l_time = long.Parse(timestamp.ToString() + "0000000");
        TimeSpan to_now = new TimeSpan(l_time);
        return date_time_start.Add(to_now);
    }


    public static string GetStrYMMDDHHMMSS(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("yyyy-MM-dd HH:mm:ss");
    }
    public static string GetStrYMMDDHHMMSSNoSpace(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("yyyy_MM_ddTHH_mm_ss");
    }
    public static string GetStrY(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("yyyy");
    }
    public static string GetStrMMDD(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("MM-dd");
    }
    public static string GetStrYMMDD(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("yyyy-MM-dd");
    }
    public static string GetStrYMMDDStyle1(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("yyyy.MM.dd");
    }
    public static string GetStrHHMM(int timeStamp)
    {
        return timestampToDateTime(timeStamp).ToString("HH:mm");
    }

    public static string FormatLeftTime(int timeStamp)
    {
        if (timeStamp <= 0)
        {
            return "00:00";
        }
        StringBuilder sb = new StringBuilder();
        long hour = timeStamp / 3600;
        timeStamp %= 3600;
        long min = timeStamp / 60;
        long sec = timeStamp % 60;

        if (hour > 0)
        {
            sb.Append(hour < 10 ? "0" : "");
            sb.Append(hour);
            sb.Append(":");
        }
        sb.Append(min < 10 ? "0" : "");
        sb.Append(min);
        if (hour <= 0)
        {
            sb.Append(":");
            sb.Append(sec < 10 ? "0" : "");
            sb.Append(sec);
        }
        return sb.ToString();
    }

    public static string GetTimeStrHHMMSS(int timeStamp, bool cutHH = false, bool cutMM = false, bool cutSS = false)
    {
        StringBuilder sb = new StringBuilder();
        if (timeStamp <= 0)
        {
            sb.Append("00");
            sb.Append(":");
            sb.Append("00");
            sb.Append(":");
            sb.Append("00");
            return sb.ToString();
        }
        else
        {
            long hour = timeStamp / 3600;
            timeStamp = timeStamp % 3600;
            long min = timeStamp / 60;
            long sec = timeStamp % 60;

            if (cutHH == false)
            {
                sb.Append(hour < 10 ? "0" : "");
                sb.Append(hour);
                sb.Append(":");
            }

            if (cutMM == false)
            {
                sb.Append(min < 10 ? "0" : "");
                sb.Append(min);
            }

            if(cutSS == false)
            {
                sb.Append(":");
                sb.Append(sec < 10 ? "0" : "");
                sb.Append(sec);
            }

            return sb.ToString();
        }
    }
    public static string GetTimeStrHHMMSS(int timeStamp, string strHourUnit, string strMinUnit, string strSecUnit, bool cutHH = false, bool cutMM = false)
    {
        StringBuilder sb = new StringBuilder();
        if (timeStamp <= 0)
        {
            sb.Append("00");
            sb.Append(strHourUnit);
            sb.Append("00");
            sb.Append(strMinUnit);
            sb.Append("00");
            sb.Append(strSecUnit);
            return sb.ToString();
        }
        else
        {
            long hour = timeStamp / 3600;
            timeStamp = timeStamp % 3600;
            long min = timeStamp / 60;
            long sec = timeStamp % 60;

            if (cutHH == false)
            {
                sb.Append(hour < 10 ? "0" : "");
                sb.Append(hour);
                sb.Append(strHourUnit);
            }

            if (cutMM == false)
            {
                sb.Append(min < 10 ? "0" : "");
                sb.Append(min);
                sb.Append(strMinUnit);
            }

            sb.Append(sec < 10 ? "0" : "");
            sb.Append(sec);
            sb.Append(strSecUnit);

            return sb.ToString();
        }
    }

    public static string FormatTimeToLocalDDHHMMSS(int timeStamp)
    {
        var day = timeStamp / GameConst.DAY_SECOND;
        var dayTime = timeStamp % GameConst.DAY_SECOND;
        var hour = dayTime / GameConst.HOUR_SECOND;
        var hourTime = dayTime % GameConst.HOUR_SECOND;
        if (day >= 1)
        {
            return "";
            //return $"{day}{Lang.Get("DAY")}{hour}{Lang.Get("HOUR")}";
        }
        var minute = hourTime / GameConst.MINUTE_SECOND;
        if (hour >= 1)
        {
            return "";
            //return $"{hour}{Lang.Get("HOUR")}{minute}{Lang.Get("MINUTE")}";
        }
        var second = hourTime % GameConst.MINUTE_SECOND;
        return "";
        //return $"{minute}{Lang.Get("MINUTE")}{second}{Lang.Get("SECOND")}";
    }

    public static string FormatTimeToLocalDDHHMMSS_BRIEF(int timeStamp)
    {
        var day = timeStamp / GameConst.DAY_SECOND;
        var dayTime = timeStamp % GameConst.DAY_SECOND;
        var hour = dayTime / GameConst.HOUR_SECOND;
        var hourTime = dayTime % GameConst.HOUR_SECOND;
        if (day >= 1)
        {
            return "";
            //return $"{day}{Lang.Get("DAY")}{hour}{Lang.Get("HOUR_BRIEF")}";
        }
        var minute = hourTime / GameConst.MINUTE_SECOND;
        if (hour >= 1)
        {
            return "";
            //return $"{hour}{Lang.Get("HOUR_BRIEF")}{minute}{Lang.Get("MINUTE_BRIEF")}";
        }
        var second = hourTime % GameConst.MINUTE_SECOND;
        return "";
        //return $"{minute}{Lang.Get("MINUTE_BRIEF")}{second}{Lang.Get("SECOND")}";
    }

    public static string FormatLastLoginTime(int passTime)
    {
        int day = 0;
        if (passTime > GameConst.DAY_SECOND * 7)
        {
            day = 7;
        }
        else if (passTime > GameConst.DAY_SECOND)
        {
            day = passTime / GameConst.DAY_SECOND;
        }
        if (day > 0)
        {
            return "";
            //return $"{day}{Lang.Get("DAY")}{Lang.Get("BEFORE")}";
        }
        var hourTime = passTime % GameConst.DAY_SECOND;
        if (passTime > GameConst.HOUR_SECOND)
        {
            int hour = hourTime / GameConst.HOUR_SECOND;
            return "";
            //return $"{hour}{Lang.Get("HOUR")}{Lang.Get("BEFORE")}";
        }
        var minuteTime = hourTime % GameConst.HOUR_SECOND;
        var minute = minuteTime / GameConst.MINUTE_SECOND;
        if (minute <= 0)
        {
            minute = 1;
        }
        return "";
        //return $"{minute}{Lang.Get("MINUTE")}{Lang.Get("BEFORE")}";
    }
    /// <summary>
    /// 两个时间是否为同一天
    /// </summary>
    /// <param name="time1"></param>
    /// <param name="time2"></param>
    /// <returns></returns>
    public static bool isSameDay(DateTime time1, DateTime time2)
    {
        if (time1.Date == time2.Date)
        {
            return true;
        }
        return false;
    }

    public static bool isSameDay(int timestamp1, int timestamp2)
    {
        if (getDateTime(timestamp1).Date == getDateTime(timestamp2).Date)
        {
            return true;
        }
        return false;
    }

    public static DateTime getDateTime(int timestamp)
    {
        return timestampToDateTime(timestamp);
    }

    /// <summary>
    /// 获取今日零点时的unix时间戳
    /// </summary>
    /// <returns></returns>
    public static int getTimeOfToday()
    {
        var dtNow = nowDateTime();
        return dateTimeParseUnixTimestamp(dtNow.Date);
    }

    /// <summary>
    /// 获取明日零点时的unix时间戳
    /// </summary>
    /// <returns></returns>
    public static int getTimeOfTomorrow()
    {
        var dtNowTomorrow = nowDateTime() + new TimeSpan(1, 0, 0, 0);
        return dateTimeParseUnixTimestamp(dtNowTomorrow.Date);
    }

    public static int dateTimeParseUnixTimestamp(DateTime time)
    {
        return (int)(time - TimeZoneInfo.Local.BaseUtcOffset - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    /// <summary>
    /// 获取当前的年月日
    /// </summary>
    /// <returns>(year, month, day)</returns>
    public static void getYearMonthDay(out int year, out int month, out int day)
    {
        var timeNow = nowDateTime();
        year = timeNow.Year;
        month = timeNow.Month;
        day = timeNow.Day;
    }


    public static float frameDeltaTime => Time.deltaTime;

    public static void SyncServerTime(Action callback, Action<JObject> fail_callback = null)
    {
        // GameService.JsonPost(ProtoDef.USER_ACTION_GET_SERVER_TIME, null, (JObject data) => {
        //     if(data.TryGetValue("server_time", out var time))
        //     {
        //         Initialize((double)time);
        //         callback?.Invoke();
        //     }
        // }, fail_callback, false);
    }
}
