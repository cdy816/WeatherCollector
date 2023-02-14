using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector
{
    public class WeatherData
    {
        /// <summary>
        /// 天气数据
        /// </summary>
        public WeatherInfo weatherinfo { get; set; }
    }

    public class WeatherInfo
    {
        public string cityname { get; set; }
        public string city { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string temp { get; set; }
       /// <summary>
       /// 风向
       /// </summary>
        public string WD { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        public string WS { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public string SD { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        public string qy { get; set; }

        /// <summary>
        /// 能见度
        /// </summary>
        public string njd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string aqi_pm25 { get; set; }

    }
}
