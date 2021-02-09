using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Earthquakes_Ursula
{
    public class Earthquakes
    {
        //const string ConnectionString = @"Server=SARAHPRICE\SPRICE;Database=DB_Ursula;Trusted_Connection=Yes;"; //@"Data Source=SARAHPRICE\\SPRICE;Initial Catalog=DB_Ursula;Integrated Security=True";//localhost
        const string ConnectionString = @"Server=DESKTOP-NJ20LJ7;Database=DB_Ursula;Trusted_Connection=Yes;"; //@"Data Source=SARAHPRICE\\SPRICE;Initial Catalog=DB_Ursula;Integrated Security=True";//localhost
       
        public Dictionary<string, EarthquakeData> m_dicEarthquakes = new Dictionary<string, EarthquakeData>();
        public List<EarthquakeData> m_lstEarthQdata = new List<EarthquakeData>();
        public GeoEngine_Workspace.GeoEngine m_Geoengine = null;
        public class EarthquakeData
        {
            public string EarthquakeID = "";
            public string EventDate = "";
            public float Latitude = 0.0f;
            public float Longitude = 0.0f;
            public float Depth = 0;
            public float Magnitude = 0;
            public float MagnitudeType = 0;
            public string Place = "";
            public string Region = "";
            public float nextdistance = 0;
            public double BearingNext = -1;
        }
        public Earthquakes()
        {
            m_Geoengine = new GeoEngine_Workspace.GeoEngine();
        }

        private void ReadSingleRow(IDataRecord record)
        {
            // Console.WriteLine(String.Format("{0}, {1},{2}, {3},{4}, {5},{6}, {7}", record[0], record[1], record[2], record[3], record[4], record[5], record[6], record[7]));
            EarthquakeData data = new EarthquakeData();
            data.EarthquakeID = record[0].ToString();

        
            data.Latitude = float.Parse(record[2].ToString());
            data.Longitude = float.Parse(record[3].ToString());
            try
            {
                if (data.EarthquakeID.Length == 4)
                {
                    m_dicEarthquakes.Add(data.EarthquakeID, data);
                }

            }
            catch
            {

            }
            
        }
        private void ReadSingleRowWithYear(IDataRecord record,string year)
        {
            // Console.WriteLine(String.Format("{0}, {1},{2}, {3},{4}, {5},{6}, {7}", record[0], record[1], record[2], record[3], record[4], record[5], record[6], record[7]));
            EarthquakeData data = new EarthquakeData();
            data.EarthquakeID = record[0].ToString();
            DateTime years = Convert.ToDateTime(record[1]);
            string FinalYear = String.Format("{0:yyyy}", years);  
            data.Latitude = float.Parse(record[2].ToString());
            data.Longitude = float.Parse(record[3].ToString());
            try
            {
                if (FinalYear==year)
                {
                    m_dicEarthquakes.Add(data.EarthquakeID, data);
                    m_lstEarthQdata.Add(m_dicEarthquakes[data.EarthquakeID]);
                    UpdateRoute();
                }

            }
            catch
            {

            }

        }

       
        public void GetAllEarthquakes()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                //1. Create a command object identifying the stored procedure 
               // SqlCommand cmd = new SqlCommand("sp_GetAllEarthquakes_V1.0", conn);
                SqlCommand cmd = new SqlCommand("sp_GetAllEarthquakes", conn);
                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        ReadSingleRow((IDataRecord)rdr);
                    }
                }

                conn.Close();
            }
        }
        public void GetAllEarthquakesWithFilter(string year)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                //1. Create a command object identifying the stored procedure 
                // SqlCommand cmd = new SqlCommand("sp_GetAllEarthquakes_V1.0", conn);
                SqlCommand cmd = new SqlCommand("sp_GetAllEarthquakes", conn);
                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        ReadSingleRowWithYear((IDataRecord)rdr,year);
                 
                    }
                }

                conn.Close();


            }
        }

        public string DeleteEarthquake()
        {
            try
            {
                string code = m_lstEarthQdata.Last().EarthquakeID;
                m_lstEarthQdata.RemoveAt(m_lstEarthQdata.Count - 1);
                UpdateRoute();
                return code; 
            }
            catch
            {

            }
            return "";
        }

        public bool AddEarthquake(string EarthquakeID)
        {
            try
            {
                if (m_dicEarthquakes[EarthquakeID] != null)
                    {
                        m_lstEarthQdata.Add(m_dicEarthquakes[EarthquakeID]);
                        UpdateRoute();
                        return true;
                    }
            }
            catch
            {
            }
            return false;
        }

        void UpdateRoute()
        {
            if (m_lstEarthQdata.Count < 2)
                return;
            for (int i = 0; i < m_lstEarthQdata.Count - 1; i++)
            {
                m_lstEarthQdata[i].BearingNext = m_Geoengine.BearingTo(m_lstEarthQdata[i].Latitude, m_lstEarthQdata[i].Longitude, m_lstEarthQdata[i + 1].Latitude, m_lstEarthQdata[i + 1].Longitude);// GetBearing(m_lstRoutedata[i], m_lstRoutedata[i + 1]);
                m_lstEarthQdata[i].nextdistance = (float)m_Geoengine.Distance(m_lstEarthQdata[i].Latitude, m_lstEarthQdata[i].Longitude, m_lstEarthQdata[i + 1].Latitude, m_lstEarthQdata[i + 1].Longitude, 'm');
            }
        }

        public double GetDistance(EarthquakeData datafrom, EarthquakeData datato)
        {
            if (datafrom.Latitude == datato.Latitude && datafrom.Longitude == datato.Longitude)
                return 0;
            double theta = datafrom.Longitude - datato.Longitude;
            double dist = Math.Sin(Deg2rad(datafrom.Latitude)) * Math.Sin(Deg2rad(datato.Latitude)) + Math.Cos(Deg2rad(datafrom.Latitude)) * Math.Cos(Deg2rad(datato.Latitude)) * Math.Cos(Deg2rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            return Math.Round(Math.Abs(dist), 3);
        }

        public double GetBearing(EarthquakeData datafrom, EarthquakeData datato)
        {
            var dLon = datato.Longitude - datafrom.Longitude;
            var y = Math.Sin(dLon) * Math.Cos(datato.Latitude);
            var x = Math.Cos(datafrom.Latitude) * Math.Sin(datato.Latitude) - Math.Sin(datafrom.Latitude) * Math.Cos(datato.Latitude) * Math.Cos(dLon);
            double angle = Rad2deg(Math.Atan2(y, x));
            if (angle < 0)
                angle += 180;
            return Math.Round(angle, 3);
        }

        private double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }

    class Program
    {
        static Earthquakes m_Earthquakes = new Earthquakes();
        
        static void UpdateConsole()
        {
            Console.Clear();
            Console.WriteLine("Ursula Earthquake Computer Console Edition Vers 2.2");
            Console.WriteLine("'A' to Add Earthquake to Route            'R' to Remove last Earthquake from Selection");
            Console.WriteLine("'P' to Print Distance to File                'Q' to Quit");
            Console.WriteLine("'Y' to Enter Year and Print Distance to File ");
            Console.WriteLine("'T' to Add earthquake                      K to delete Earthquake");
            Console.WriteLine("'L' to Update Earthquake");
            Console.WriteLine();
            Console.WriteLine("Earthquake ID    Latitude    Longitude   Bearing to Selected Earthquake    Distance to Selected Earthquake");

            double totaldistance = 0;
            int ncount = m_Earthquakes.m_lstEarthQdata.Count;
            if (ncount == 0)
            {
                Console.WriteLine("--              --          --          --                         --");
            }
            else
            {
                for (int i = 0; i < ncount - 1; i++)
                {
                    totaldistance += m_Earthquakes.m_lstEarthQdata[i].nextdistance;
                    Console.WriteLine(m_Earthquakes.m_lstEarthQdata[i].EarthquakeID + "             " + m_Earthquakes.m_lstEarthQdata[i].Latitude.ToString() + "       " + m_Earthquakes.m_lstEarthQdata[i].Longitude.ToString() + "       " + m_Earthquakes.m_lstEarthQdata[i].BearingNext.ToString() + "                     " + m_Earthquakes.m_lstEarthQdata[i].nextdistance.ToString());
                }
                Console.WriteLine(m_Earthquakes.m_lstEarthQdata[ncount - 1].EarthquakeID + "             " + m_Earthquakes.m_lstEarthQdata[ncount - 1].Latitude.ToString() + "       " + m_Earthquakes.m_lstEarthQdata[ncount - 1].Longitude.ToString() + "       --                          --");
            }
            Console.WriteLine();
            Console.WriteLine("Number of Earthquakes: " + m_Earthquakes.m_lstEarthQdata.Count);
            Console.WriteLine("Length of distance: " + totaldistance);
            Console.WriteLine();

            Console.WriteLine("Make Selection");

        }
        static void Main(string[] args)
        {

            m_Earthquakes = new Earthquakes();
            //double angle = m_Airports.GetBearing(-6.082, 145.392, 49.951, -125.271);
            m_Earthquakes.GetAllEarthquakes();
            UpdateConsole();
            string line = "";
         string   txtBoxID;
         string   txtEvent= "";
           string txtLat= "";
         string   txtLong= "";
          string  txtDepth= "";
          string  txtMagnitude= "";
         string   txtType= "";
         string   txtPlace= "";
           string  txtRegion= "";
            while ((line = Console.ReadLine()) != null)
            {
                if (line.Contains("Q"))
                    break;
                if (line.Contains("P"))
                {
                    File.WriteAllText("DistanceFile.txt", "\"lat\";\"lon\";\"weight\"\n");
                    for (int i = 0; i < m_Earthquakes.m_lstEarthQdata.Count; i++)
                    {
                        File.AppendAllText("DistanceFile.txt", m_Earthquakes.m_lstEarthQdata[i].Latitude.ToString() + ";" + m_Earthquakes.m_lstEarthQdata[i].Longitude.ToString() + ";" + m_Earthquakes.m_lstEarthQdata[i].nextdistance.ToString() + "\n");
                    }
                    Console.WriteLine("Save route in DistanceFile.txt");
                }
                if (line.Contains("Y"))
                {
                    Console.WriteLine("Please enter Event date ");
                    line = Console.ReadLine();
                    try
                    {
                        m_Earthquakes.GetAllEarthquakesWithFilter(line);
           
                        File.WriteAllText("DistanceFile.txt", "\"lat\";\"lon\";\"weight\"\n");
                        for (int i = 0; i < m_Earthquakes.m_lstEarthQdata.Count; i++)
                        {
                            File.AppendAllText("DistanceFile.txt", m_Earthquakes.m_lstEarthQdata[i].Latitude.ToString() + ";" + m_Earthquakes.m_lstEarthQdata[i].Longitude.ToString() + ";" + m_Earthquakes.m_lstEarthQdata[i].nextdistance.ToString() + "\n");
                        }
                        Console.WriteLine("Save route in DistanceFile.txt");

                    }
                    catch
                    {

                    }

                  
                }
                    //add earthquake
                else if (line.Contains("T"))
                {
                    Console.WriteLine("Please enter event date. format yyyy-MM-dd HH:mmss");
                    txtEvent = Console.ReadLine();
                    Console.WriteLine("Please enter Latitude");
                    txtLat = Console.ReadLine();
                    Console.WriteLine("Please enter Longhitude");
                    txtLong = Console.ReadLine();
                    Console.WriteLine("Please enter Depth");
                    txtDepth = Console.ReadLine();
                    Console.WriteLine("Please enter Magnitude");
                    txtMagnitude = Console.ReadLine();
                    Console.WriteLine("Please enter Magnitude Type");
                    txtType = Console.ReadLine();
                    Console.WriteLine("Please enter Place");
                    txtPlace = Console.ReadLine();
                    Console.WriteLine("Please enter Region");
                    txtRegion = Console.ReadLine();
                    GeoEngine_Workspace.Earthquake earthquakedata=new GeoEngine_Workspace.Earthquake();
                    earthquakedata.EventDate=Convert.ToDateTime(txtEvent);
                    earthquakedata.Latitude = Convert.ToDecimal(txtLat);
                    earthquakedata.Longitude = Convert.ToDecimal(txtLong);
                    earthquakedata.Depth = Convert.ToDecimal(txtDepth);
                    earthquakedata.Magnitude = Convert.ToDecimal(txtMagnitude);
                    earthquakedata.MagnitudeType = txtType;
                    earthquakedata.Place = txtPlace;
                    earthquakedata.Region = txtRegion;
                    List<GeoEngine_Workspace.Earthquake> lstEarthquake = new List<GeoEngine_Workspace.Earthquake>();
                    lstEarthquake.Add(earthquakedata);
                    GeoEngine_Workspace.DataAccessClass da = new GeoEngine_Workspace.DataAccessClass();
                    da.TransactEarthquake(lstEarthquake,"INSERT");
                    Console.WriteLine("Successfully Added");
                  
                }
                //update earthquake
                else if (line.Contains("L"))
                {
                    Console.WriteLine("Please enter Earthquake ID");
                    txtBoxID = Console.ReadLine();
                    Console.WriteLine("Please enter event date. format yyyy-MM-dd HH:mmss");
                    txtEvent = Console.ReadLine();
                    Console.WriteLine("Please enter Latitude");
                    txtLat = Console.ReadLine();
                    Console.WriteLine("Please enter Longhitude");
                    txtLong = Console.ReadLine();
                    Console.WriteLine("Please enter Depth");
                    txtDepth = Console.ReadLine();
                    Console.WriteLine("Please enter Magnitude");
                    txtMagnitude = Console.ReadLine();
                    Console.WriteLine("Please enter Magnitude Type");
                    txtType = Console.ReadLine();
                    Console.WriteLine("Please enter Place");
                    txtPlace = Console.ReadLine();
                    Console.WriteLine("Please enter Region");
                    txtRegion = Console.ReadLine();
                    GeoEngine_Workspace.Earthquake earthquakedata = new GeoEngine_Workspace.Earthquake();
                    earthquakedata.EarthquakeID = Convert.ToInt32(txtBoxID);
                    earthquakedata.EventDate = Convert.ToDateTime(txtEvent);
                    earthquakedata.Latitude = Convert.ToDecimal(txtLat);
                    earthquakedata.Longitude = Convert.ToDecimal(txtLong);
                    earthquakedata.Depth = Convert.ToDecimal(txtDepth);
                    earthquakedata.Magnitude = Convert.ToDecimal(txtMagnitude);
                    earthquakedata.MagnitudeType = txtType;
                    earthquakedata.Place = txtPlace;
                    earthquakedata.Region = txtRegion;
                    List<GeoEngine_Workspace.Earthquake> lstEarthquake = new List<GeoEngine_Workspace.Earthquake>();
                    lstEarthquake.Add(earthquakedata);
                    GeoEngine_Workspace.DataAccessClass da = new GeoEngine_Workspace.DataAccessClass();
                    da.TransactEarthquake(lstEarthquake, "UPDATE");
                    Console.WriteLine("Successfully UPDATE");

                }
                //update earthquake
                else if (line.Contains("K"))
                {
                    Console.WriteLine("Please enter Earthquake ID");
                    txtBoxID = Console.ReadLine();
                   
                    GeoEngine_Workspace.Earthquake earthquakedata = new GeoEngine_Workspace.Earthquake();
                    earthquakedata.EarthquakeID = Convert.ToInt32(txtBoxID);
                    earthquakedata.EventDate = Convert.ToDateTime(txtEvent);
                    earthquakedata.Latitude = Convert.ToDecimal(txtLat);
                    earthquakedata.Longitude = Convert.ToDecimal(txtLong);
                    earthquakedata.Depth = Convert.ToDecimal(txtDepth);
                    earthquakedata.Magnitude = Convert.ToDecimal(txtMagnitude);
                    earthquakedata.MagnitudeType = txtType;
                    earthquakedata.Place = txtPlace;
                    earthquakedata.Region = txtRegion;
                    List<GeoEngine_Workspace.Earthquake> lstEarthquake = new List<GeoEngine_Workspace.Earthquake>();
                    lstEarthquake.Add(earthquakedata);
                    GeoEngine_Workspace.DataAccessClass da = new GeoEngine_Workspace.DataAccessClass();
                    da.TransactEarthquake(lstEarthquake, "DELETE");
                    Console.WriteLine("Successfully DELETED");

                }
                else if (line.Contains("A"))
                {
                    Console.WriteLine("Please enter 3 letter Earthquake ID");
                    line = Console.ReadLine();
                    try
                    {
                        //int nid = int.Parse(line.Replace("A", "").Trim());
                        bool result = m_Earthquakes.AddEarthquake(line);
                        if (result)
                            Console.WriteLine("Add Earthquake:" + line);
                        else
                            Console.WriteLine("Add Fail:" + line);

                    }
                    catch
                    {

                    }
                }
                else if (line.Contains("R"))
                {
                    try
                    {
                        string code = m_Earthquakes.DeleteEarthquake();
                        if (code != "")
                        {
                            Console.WriteLine("Remove Earthquake:" + code);
                        }

                    }
                    catch
                    {

                    }
                }
                else
                    UpdateConsole();
            }
        }


    }
}

            




        