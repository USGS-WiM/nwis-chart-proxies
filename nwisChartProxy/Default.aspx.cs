// Service for getting presentation quality graph url for a given site number, chart parameter and date range

using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace nwisChartProxy
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string site_no = Request.QueryString["site_no"];
            string chart_param = Request.QueryString["chart_param"];
            string begin_date = Request.QueryString["begin_date"];
            string end_date = Request.QueryString["end_date"];
            string previous_x_days = Request.QueryString["days_prev_to_current"];
            string result = "";
            
            if (previous_x_days != null && previous_x_days != "")
            {
                //Switch to int.TryParse at some point...
                int daydelta = int.Parse(previous_x_days);

                TimeSpan daysPrevious = new TimeSpan(daydelta, 0, 0, 0);

                DateTime now = DateTime.Now;

                DateTime begin_datetime = now.Subtract(daysPrevious);

                string year = begin_datetime.Year.ToString();
                string month = begin_datetime.Month.ToString();
                if (month.Length == 1)
                {
                    month = '0' + month;
                }
                string day = begin_datetime.Day.ToString();
                if (day.Length == 1)
                {
                    day = '0' + day;
                }

                begin_date = year + month + day;

            }
            
            string url = "http://waterdata.usgs.gov/nwis/uv/?format=img_default&site_no=" + site_no + "&set_arithscale_y=on&begin_date=" + begin_date + "&end_date=" + end_date;
            

            HttpWebRequest wsRequest = (HttpWebRequest)WebRequest.Create(url);

            wsRequest.Method = "GET";
            
            HttpWebResponse wsResponse = (HttpWebResponse)wsRequest.GetResponse();
            
            string responseStatus = wsResponse.StatusCode.ToString();

            System.IO.StreamReader str = new System.IO.StreamReader(wsResponse.GetResponseStream());

            if (responseStatus == "OK")
            {
                string contents = str.ReadToEnd();
                string response = "";

                string[] urls = contents.Split(new string[]{"http"}, StringSplitOptions.None);

                for (int i = 0; i < urls.Length; i++)
                {
                    int param = urls[i].IndexOf(chart_param);
                    if (param != -1)
                    {
                        string[] imageUrl = urls[i].Split(new string[]{"gif"}, StringSplitOptions.None);
                        response = "http" + imageUrl[0] + "gif";
                    }
                }
		Response.Write(response);
                //Response.Write(Server.HtmlEncode(contents));

            } else {
                Response.Write("Error with request");
            }
            
            str.Close();

        }

    }
}
