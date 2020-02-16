using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test_redips_js.Models
{
    public class wkflow_doc
    {
       public int id { get; set; }
        public string wkflow { get; set; }
       public int doc { get; set; }
        public string doc_s { get; set; }
    }

    public class stage_old_new
    {
        public int oid { get; set; }
        
        public int nid { get; set; }
        public int rowno { get; set; }
    }


}