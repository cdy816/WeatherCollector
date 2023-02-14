using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector
{
    public class WeatherHelper
    {
        public static WeatherHelper Helper = new WeatherHelper();

        private Dictionary<string, string> mUrlMaps = new Dictionary<string, string>();

        public const string RealUrl = "http://d1.weather.com.cn/sk_2d/{0}.html?=";

        public const string DayUrl = "http://weather.com.cn/data/cityinfo/{0}";

        private DirectAccessDriver.ClientApi.DriverProxy mProxy;

        /// <summary>
        /// 
        /// </summary>
        public WeatherHelper()
        {
            Init();
        }

        public void Init()
        {
            var vinfo = this.GetType().Assembly.GetManifestResourceStream("WeatherCollector.WeatherList.txt");
           if(vinfo != null)
            {
               var datas = new System.IO.StreamReader(vinfo).ReadToEnd().Split("\r\n",StringSplitOptions.RemoveEmptyEntries);
                if(datas.Length > 0)
                {
                    foreach(var data in datas)
                    {
                        string[] sval = data.Split("=");
                        if(sval.Length>1)
                        {
                            mUrlMaps.TryAdd(sval[1], string.Format(RealUrl, sval[0]));
                        }
                    }
                }
            }

            mProxy = new DirectAccessDriver.ClientApi.DriverProxy();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            mProxy.Open("127.0.0.1",9800);
            mProxy.Login("Admin", "Admin");

            while (true)
            {
             
                foreach (var vv in mUrlMaps)
                {
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        string sval = GetFromUrl(vv.Value);
                        var vdata = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(sval);
                        if (vdata != null)
                        {
                            var vals = new List<Cdy.Tag.RealTagValue3>();
                            vals.Add(new Cdy.Tag.RealTagValue3() { TagName = vdata.cityname + ".温度", Value = double.Parse(vdata.temp), ValueType = (byte)Cdy.Tag.TagType.Double, Quality = 0 });
                            vals.Add(new Cdy.Tag.RealTagValue3() { TagName = vdata.cityname + ".湿度", Value = double.Parse(vdata.SD.Replace("%", "")) / 100.0, ValueType = (byte)Cdy.Tag.TagType.Double, Quality = 0 });
                            vals.Add(new Cdy.Tag.RealTagValue3() { TagName = vdata.cityname + ".气压", Value = double.Parse(vdata.qy.Replace("hPa", "")), ValueType = (byte)Cdy.Tag.TagType.Double, Quality = 0 });
                            vals.Add(new Cdy.Tag.RealTagValue3() { TagName = vdata.cityname + ".风向", Value = vdata.WD, ValueType = (byte)Cdy.Tag.TagType.String, Quality = 0 });
                            vals.Add(new Cdy.Tag.RealTagValue3() { TagName = vdata.cityname + ".风速", Value = vdata.WS, ValueType = (byte)Cdy.Tag.TagType.String, Quality = 0 });
                            
                            if(!mProxy.SetTagRealAndHisValue(vals))
                            {
                                Console.WriteLine($" 更新 {vv.Key} 到数据库失败");
                            }
                        }
                        sw.Stop();
                        Console.WriteLine($" 更新 {vv.Key} 数据，耗时: {sw.ElapsedMilliseconds}");
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(1000 * 60 * 10);
            }
        }

        private string GetFromUrl(string sval)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Referer", "http://www.weather.com.cn/");
            var res = client.GetStringAsync(sval+DateTime.Now.Ticks).Result;
            if(res.StartsWith("var"))
            {
                return res.Replace("var dataSK=", "");
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
