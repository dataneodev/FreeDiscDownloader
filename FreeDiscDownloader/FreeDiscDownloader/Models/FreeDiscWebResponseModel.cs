using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class Datum
    {
        public string id { get; set; }
        public string type { get; set; }
        public string size_format { get; set; }
        public string type_fk { get; set; }
        public string name { get; set; }
        public string name_url { get; set; }
        public string status { get; set; }
        public string parent_id { get; set; }
        public string trash { get; set; }
        public bool adult { get; set; }
        public string date_add_format { get; set; }
        public bool new_file { get; set; }
        public int saved_from { get; set; }
        public int saved_from_file { get; set; }
        public int user_id { get; set; }
        public string icon { get; set; }
        public string extension { get; set; }
    }

    public class DataFiles
    {
        public IList<Datum> data { get; set; }
        public int hits { get; set; }
        public int time { get; set; }
    }

    public class Datum2
    {
        public string userID { get; set; }
    }

    public class DataUsers
    {
        public IList<Datum> data { get; set; }
        public int hits { get; set; }
        public int time { get; set; }
    }

    public class w1015129
    {
        public int id { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public string name_url { get; set; }
    public string status { get; set; }
    public int parent_id { get; set; }
    public string dir_count { get; set; }
    public string file_count { get; set; }
    public string trash { get; set; }
    public bool new_file { get; set; }
}


    public class DirectoriesTranslated
{
//    public 1015129 1015129 { get; set; }

    }

    public class w54682
    {
        public string userID { get; set; }
public string userLogin { get; set; }
public object avatar { get; set; }
public string userRootDirID { get; set; }
public string last_visit_date { get; set; }
public string register_date { get; set; }
public string filesCount { get; set; }
public string viewsCount { get; set; }
public string filesSize { get; set; }
public bool online { get; set; }
public string display { get; set; }
public string url { get; set; }
public string files_count_format { get; set; }
public string files_size_format { get; set; }
    }

   
    public class ss0
    {
        public int userID { get; set; }
public string display { get; set; }
    }

    public class LoginsTranslated
{
//    public 54682 54682 { get; set; }

    }

    public class Response
{
    public int limit { get; set; }
    public int page { get; set; }
    public DataFiles data_files { get; set; }
    public int pages { get; set; }
    public DataUsers data_users { get; set; }
    public DirectoriesTranslated directories_translated { get; set; }
    public LoginsTranslated logins_translated { get; set; }
}

public class Example
{
    public Response response { get; set; }
    public string type { get; set; }
    public bool success { get; set; }
}

}
