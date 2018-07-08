using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscWebSearchResponseModel
    {
        public Response response { get; set; }
        public string type { get; set; }
        public bool success { get; set; }
    }

    public class Response
    {
        public int limit { get; set; }
        public int page { get; set; }
        public DataFiles data_files { get; set; }
        public int pages { get; set; }
        public DataUsers data_users { get; set; }
        public Dictionary<int, directory> directories_translated { get; set; }
        public Dictionary<int, user_login> logins_translated { get; set; }
    }

    public class DataFiles
    {
        public List<Datum> data { get; set; }
        public int hits { get; set; }
        public int time { get; set; }
    }

    public class DataUsers
    {
        public List<Datum2> data { get; set; }
        public int hits { get; set; }
        public int time { get; set; }
    }

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

    public class Datum2
    {
        public string userID { get; set; }
    }

    public class user_login
    {
        public string userID { get; set; }
        public string userLogin { get; set; }
        public string avatar { get; set; }
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

    public class directory
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
}
