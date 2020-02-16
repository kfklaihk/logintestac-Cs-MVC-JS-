using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using Test_redips_js.Models;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.Core.Objects;

using MoreLinq.Extensions;
using System.Activities.Statements;

namespace Test_redips_js.Controllers
{
    public class HomeController : Controller
    {
        private Test_redips_jsContext db = new Test_redips_jsContext();

        public List<string> stages = new List<string>();

          


        /*
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            stages_init(stages);
        }
        */

        public ActionResult Index()
        {
           
       
        IQueryable<document_type_Model> d;
        IQueryable<wkflow_doc> h;
            string prev_wkflow = "";
            int prev_doc=-1;


            try
            {
                d = (from x in db.document_type_Models
                     orderby x.table_id
                     select x);
            }
            catch (Exception e)
            {
                d = null;
            }

            try
            {
                h = (from x1 in db.history_master_Models
                     join y in db.document_type_Models on x1.awhm_document_type equals y.table_id 
                     orderby x1.awhm_document_type, x1.awhm_workflow_name
                     select  new wkflow_doc {id=x1.table_id, wkflow = x1.awhm_workflow_name, doc = x1.awhm_document_type, doc_s=y.awdt_name });
            }
            catch (Exception e)
            {
                h = null;
            }

            string doclist = "<select name=ndoc>";

                foreach (var doc in d)
                {
                    doclist = doclist + "<option value = " + doc.table_id.ToString() + ">" + doc.awdt_name + "</option>";

                }

                doclist = doclist + "</select>";
                ViewBag.data_d = doclist;

            string tmpllist = "<select name=tmpl>";

            foreach (var tmpl in h)
             {
                if (!(tmpl.wkflow.Equals(prev_wkflow) && tmpl.doc==prev_doc))
                {

                    tmpllist = tmpllist + "<option value = " + tmpl.id + ">" + tmpl.doc_s + "," + tmpl.wkflow + "</option>";
                }
                prev_wkflow = tmpl.wkflow;
                prev_doc = tmpl.doc;

            }

            tmpllist = tmpllist + "</select>";
            ViewBag.data_t = tmpllist;


            return View();
        }

        private void stages_init(List<string> alist)
        {
            alist.Add("Draft");
            alist.Add("Request for approval");
            alist.Add("Finance Endorsed waiting for Legal");
            alist.Add("Legal Endorsed waiting for Finance");
            alist.Add("CE Approval");
            alist.Add("PTC Approval");
            alist.Add("Final");

            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ActionResult Save_History(string wkflow, string ndoc)
        {
            var docid = Convert.ToInt32(ndoc);

            var list_s1 = (from x1 in db.StageModels
                       join x2 in db.BtnModels on x1.rowno equals x2.rowid
                       join x3 in db.Btn_Detail_Models on x2.tdid equals x3.tdid
                       join x4 in db.document_type_Models on x1.awst_document_type equals x4.table_id
                           where x1.awst_workflow_name.Equals(wkflow) && x1.awst_document_type == docid &&
                            x2.Stage.awst_workflow_name.Equals(wkflow) && x2.Stage.awst_document_type == docid &&
                             x3.Stage.awst_workflow_name.Equals(wkflow) && x3.Stage.awst_document_type == docid
                                                                                                                                                                                      
                           select new 
                       {
                           awhm_document_type = x4.table_id,
                           awhm_workflow_name = x1.awst_workflow_name,
                           stage = x1,
                           awbn_button_cid = x2.tdid,
                           awhm_start_record_id = 0,
                           awhm_record_id = 0,
                           awhm_from_stage = 0,
                           awhm_to_stage = 0,
                           awhm_remark = "N/A",
                           awhm_button_number = x3.awbd_button_number,
                           awhm_group = x3.awbd_group_list,
                           awhm_user = "",
                           awhm_created_date = DateTime.Now

                       });

            foreach (var s2 in list_s1)
            {

              //  var s = (from x in db.StageModels where x.awst_master_id == s2.awst_master_id select x).ToList().FirstOrDefault();

                var m = new history_master_Model { awhm_document_type = s2.awhm_document_type, awhm_workflow_name = s2.awhm_workflow_name, Stage = s2.stage, tdid = s2.awbn_button_cid,  awhm_start_record_id = s2.awhm_start_record_id, awhm_record_id = s2.awhm_record_id, awhm_from_stage = s2.awhm_from_stage, awhm_to_stage = s2.awhm_to_stage, awhm_remark = s2.awhm_remark, awhm_button_number = s2.awhm_button_number, awhm_group = s2.awhm_group, awhm_user = s2.awhm_user, awhm_created_date = s2.awhm_created_date };
                db.history_master_Models.Add(m);

            }
            db.SaveChanges();

            return RedirectToAction("Test");
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public ActionResult SaveTable_stage_button(string wkflow, string ndoc,string s, string p)
        {
            var docid = Convert.ToInt32(ndoc);
            int i = 2;
            StageModel s1;
            List<StageModel> list_s1 = new List<StageModel> { };
            List<StageModel> list_s2 = new List<StageModel> { };

            //db.Database.ExecuteSqlCommand("TRUNCATE TABLE  StageModels");

            
            


            s = s.Replace("\\", "");
            s = s.Replace("[", "");
            s = s.Replace("]", "");
            s = s.Replace("\"", "");
            stages = s.Split(',').ToList();
            List<string> idlist = new List<string>();
            List <int> del_id = new List<int>();
            foreach (var stage in stages)
            {
                if (idlist.Contains(stage) == false)
                {
                    idlist.Add(stage);

                    try
                    {
                        s1 = (from x in db.StageModels
                              where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type == docid
                              && x.stg.Equals(stage)

                              select x).ToList().FirstOrDefault();

                        s1.rowno = i;
                        s1.awst_stage_no = i - 1;
                        s1.awst_document_type = docid;
                        s1.awst_workflow_name = wkflow;
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        s1 = new StageModel { stg = stage, rowno = i, awst_stage_no = i - 1 , awst_document_type = docid , awst_workflow_name =wkflow};
                        db.StageModels.Add(s1);
                        db.SaveChanges();
                    }
                    list_s1.Add(s1);
                    
                    i++;
                }
            }
            
            try
            {
                list_s2 = (from x in db.StageModels
                           where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type == docid
                           select x).ToList().Except(list_s1).ToList();
                
                 del_id = (from x in list_s2 select x.awst_master_id).ToList();
                foreach (var ss2 in list_s2)
                {
                                   
                    
                    db.StageModels.Remove(ss2);
                    
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                
                list_s2 = null;
            }


            int j = 2;
            int p_=0;
            List<StageModel> list_s1_;
           

            try
            {
                list_s1_ = (from x in db.StageModels
                            where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type == docid

                            select x).ToList();
               
             
            }
            catch (Exception e)
            {
                list_s1_ = null;

            }


            List<string> list_p_;
            List<Btn_Detail_Model> list_pd = new List<Btn_Detail_Model> { };
            List<Btn_Detail_Model> list_pd_1 = new List<Btn_Detail_Model> { };
            List<Btn_Detail_Model> list_pd_2 = new List<Btn_Detail_Model> { };
            int row_id;

            

            //      db.Database.ExecuteSqlCommand("TRUNCATE TABLE AW_button_master");

            try
            { 
                list_p_ = (from x in db.BtnModels
                           where x.Stage.awst_workflow_name.Equals(wkflow) && x.Stage.awst_document_type == docid
                           select x.tdid ).ToList();

                foreach (var lp_ in list_p_)
                {
                    int c = Convert.ToInt32(lp_.Replace("cid", ""));
                    if (c > p_) { p_ = c; }
                }
            }
            catch (Exception e)
            {
                p_ = 0;

            }
            p_ = p_ + 1;

            var p_1s = (from x in db.BtnModels
                       where x.Stage.awst_workflow_name.Equals(wkflow) && x.Stage.awst_document_type == docid
                       select x);

            foreach (var p_1_ in p_1s)
            {


                db.BtnModels.Remove(p_1_);

            }
            db.SaveChanges();



            p = p.Replace("\\", "");
            p = p.Replace("[", "");
            p = p.Replace("\"", "");
            //  p = p.Replace("]", "");

            Regex regex = new Regex(@"],");
            string[] p1 = regex.Split(p);
            List<string> idlist_ = new List<string>();

            foreach (var p_1 in p1)
            {

                var p_11 = p_1.Split(',');
                if (Convert.ToInt32(p_11[2]) != 0 && Convert.ToInt32(p_11[1]) != 0 && p_11[4].Replace("]", "") != "" && idlist_.Contains(p_11[0]) == false)
                {

                    idlist_.Add(p_11[0]);
                    var tmp = p_11[3].Split(new string[] { "nextstg" }, StringSplitOptions.None);
                    var tmp1=tmp[1].Split(new string[] { "cid" }, StringSplitOptions.None);
                    int nextstg = Convert.ToInt32(tmp1[0].Trim());
                    int ocid = Convert.ToInt32(tmp1[1].Trim());
                    if (ocid<0) {
                        ocid = p_;
                    }

                    foreach (var did in del_id)
                    {
                        if (nextstg ==did) {
                            nextstg = -1;
                            break;
                        }


                    }
                    row_id = Convert.ToInt32(p_11[1]);
                    s1 = (from x in list_s1_ where x.rowno == row_id select x).ToList().FirstOrDefault();
                    var m = new BtnModel { tdid = "cid" + ocid.ToString() /* tdid = p_11[0]*/, btn = p_11[4].Replace("]", "").Trim(), rowid = row_id, colid = Convert.ToInt32(p_11[2]), nextstg = nextstg, Stage=s1 };
                    db.BtnModels.Add(m);
                    p_ = p_ + 1;
                }

            }
            db.SaveChanges();

            var btn_p= (from x in db.BtnModels
                    where x.Stage.awst_workflow_name.Equals(wkflow) && x.Stage.awst_document_type == docid
                    select x);

            list_pd_1 = (from x in db.Btn_Detail_Models
                       join y in btn_p
                       on x.tdid equals y.tdid
                     where y.Stage.awst_workflow_name.Equals(wkflow) && y.Stage.awst_document_type == docid
                       select x).ToList();

            list_pd = (from x in db.Btn_Detail_Models
                           where x.Stage.awst_workflow_name.Equals(wkflow) && x.Stage.awst_document_type == docid
                         select x).ToList();

            list_pd_2 = list_pd.Except(list_pd_1).ToList();

            foreach (var pd_2 in list_pd_2)
            {


                db.Btn_Detail_Models.Remove(pd_2);

            }
            db.SaveChanges();
            TempData["msg"] = "Completed Saving Table Snapshot";
            return RedirectToAction("Test", new { wkflow = wkflow, ndoc = ndoc });

        }

        public ActionResult save_stage_detail(string s,string sd, string fc, string ndoc,string[] ngroup, string r,string nstatus)
        {
            StageModel s1;

            var r1 = Convert.ToInt32(r)+1;
            var docid = Convert.ToInt32(ndoc);
            var grp_list = "";
            if (ngroup != null)
            {
                foreach (var gp in ngroup)
                {
                    grp_list = grp_list + gp + ",";
                }
                grp_list = grp_list.Remove(grp_list.Length - 1);
            }
            try
            {
                s1 = (from x in db.StageModels
                      where x.rowno == r1 && x.awst_workflow_name.Equals(s) && x.awst_document_type == docid

                      select x).ToList().FirstOrDefault();

                s1.awst_workflow_name = s;
                s1.awst_stage_no = r1 - 1;
                s1.awst_document_type = docid;
                s1.awst_status = nstatus;
                s1.stg = sd;
                s1.awst_function_code = fc;
                s1.awst_group_list = grp_list;
                s1.rowno = r1;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                s1 = new StageModel { stg = sd, rowno = r1, awst_stage_no = r1 - 1, awst_document_type = docid, awst_status = nstatus, awst_workflow_name = s, awst_function_code = fc, awst_group_list = grp_list };
                db.StageModels.Add(s1);
                db.SaveChanges();
            }
            return RedirectToAction("Test", new { wkflow = s, ndoc = ndoc });
        }

        public ActionResult SaveTable_stage(string s)
        {
            int i = 2;
            StageModel s1;
            List<StageModel> list_s1= new List<StageModel>{ };
            List<StageModel> list_s2 = new List<StageModel> { };

            //db.Database.ExecuteSqlCommand("TRUNCATE TABLE  StageModels");





            s = s.Replace("\\", "");
            s = s.Replace("[", "");
            s = s.Replace("]", "");
            s = s.Replace("\"", "");
            stages = s.Split(',').ToList();
            List<string> idlist = new List<string>();

            foreach (var stage in stages)
            {
                if (idlist.Contains(stage) == false)
                {
                    idlist.Add(stage);

                    try
                    {
                        s1 = (from x in db.StageModels where x.stg.Equals(stage)
                              
                              select x).ToList().FirstOrDefault();

                        s1.rowno = i;
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        s1 = new StageModel { stg = stage, rowno = i };
                        db.StageModels.Add(s1);
                         db.SaveChanges();
                    }
                    list_s1.Add(s1);

                    i++;
                }
            }
            try
            {
                list_s2 = (from x in db.StageModels
                           select x).ToList().Except(list_s1).ToList();

                foreach (var ss2 in list_s2)
                {
                    db.StageModels.Remove(ss2);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {

                list_s2 = null;
            }

                return RedirectToAction("Test");
        }

        //--------------------------
        
        public string SaveTable_btn(string wkflow, string ndoc, string r,string co, string classtext, string id)
        { StageModel s1;
            string ret_str;
            var docid = Convert.ToInt32(ndoc);
            List<string> list_p_;
            int p_ = 0;
            int c = 0;
            int rn = Convert.ToInt32(r);
            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {
                    s1 = (from x in db.StageModels
                          where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type == docid && x.rowno == rn
                          select x).FirstOrDefault();



            }
                catch (Exception e)
                {
                    s1 = null;

                }



                try
                {
                    list_p_ = (from x in db.BtnModels
                               where x.Stage.awst_workflow_name.Equals(wkflow) && x.Stage.awst_document_type == docid
                               select x.tdid).ToList();

                    foreach (var lp_ in list_p_)
                    {
                        c = Convert.ToInt32(lp_.Replace("cid", ""));
                        if (c > p_) { p_ = c; }
                    }
                }
                catch (Exception e)
                {
                    p_ = 0;

                }
                p_ = p_ + 1;

                var tmp = classtext.Split(new string[] { "nextstg" }, StringSplitOptions.None);
                var tmp1 = tmp[1].Split(new string[] { "cid" }, StringSplitOptions.None);
                int nextstg = Convert.ToInt32(tmp1[0].Trim());
                int ocid = Convert.ToInt32(tmp1[1].Trim());
                if (ocid < 0)
                {
                    ocid = p_;
                }

                var m = new BtnModel { tdid = "cid" + ocid.ToString(),Stage=s1, btn = "Please enter text", rowid = rn, colid = Convert.ToInt32(co), nextstg = nextstg };
                db.BtnModels.Add(m);
                db.SaveChanges();
                transaction.Commit();
                ret_str = "cid" + ocid.ToString()+"_"+id;
            }
            
             return ret_str;
          
        }
        //----------------------------------






        [MethodImpl(MethodImplOptions.Synchronized)]
        public ActionResult save_button_detail(string s, string ndoc,string tdid, string p,  string nstage, string sc, string[] ngroup, string fc, string aca, string ct, string cbool, string rt
        , string rbool, string nt, string ntc, string aet, string he, string apbool, string abr, string abc)
        {
            BtnModel p1;
            StageModel s1;
            Btn_Detail_Model p1d;
            int docid = Convert.ToInt32(ndoc);
            int r1 = Convert.ToInt32(abr)+1;
            int c1 = Convert.ToInt32(abc);
            int pnum1 = 0;// Convert.ToInt32(String.IsNullOrWhiteSpace(pnum) ? "0" : pnum);
            int nt1 = Convert.ToInt32(String.IsNullOrWhiteSpace(nt) ? "0" : nt);
            int ntc1 = Convert.ToInt32(String.IsNullOrWhiteSpace(ntc) ? "0" : ntc);
            int aet1 = Convert.ToInt32(String.IsNullOrWhiteSpace(aet) ? "0" : aet);

            int nextstg = Convert.ToInt32(nstage.Replace("\"", ""));
            
            Boolean b1, b2, b3,b4;

            if (aet == "1")
            {
                b4 = true;
            }
            else
            {
                b4 = false;
            }

            if (cbool == "1")
            {
                b1 = true;
            }
            else
            {
                b1 = false;
            }

            if (rbool == "1")
            {
                b2 = true;
            }
            else
            {
                b2 = false;
            }

            if (apbool == "1")
            {
                b3 = true;
            }
            else
            {
               b3 = false;
            }

            s1 = (from x in db.StageModels 
                  where x.rowno == r1 && x.awst_workflow_name.Equals(s) && x.awst_document_type == docid

                  select x).ToList().FirstOrDefault();

            
            var grp_list = "";
            if (ngroup != null)
            {
                foreach (var gp in ngroup)
                {
                    grp_list = grp_list + gp + ",";
                }
                grp_list = grp_list.Remove(grp_list.Length - 1);
            }
            try
            {
                p1 = (from x in db.BtnModels
                      where x.tdid.Equals(tdid) && x.Stage.awst_workflow_name.Equals(s) && x.Stage.awst_document_type == docid

                      select x).ToList().FirstOrDefault();
                p1.Stage = s1;
                p1.nextstg = nextstg;
                p1.btn = p;
                db.SaveChanges();
                pnum1 = p1.colid - 2;
            }
            catch (Exception e)
            {
                p1 = null;


            }


            try
            {

                p1d = (from x in db.Btn_Detail_Models
                       where x.tdid.Equals(tdid) && x.Stage.awst_workflow_name.Equals(s) && x.Stage.awst_document_type == docid

                       select x).ToList().FirstOrDefault();

               

                p1d.Stage = s1;
                p1d.awbd_group_list = grp_list;
                p1d.awbd_button_number = pnum1;
                p1d.awbd_selection_criteria = sc;
                p1d.awbd_after_click_function_code = fc;
                p1d.awbd_after_click_action = aca;
                p1d.awbd_confirmation_text = ct;
                p1d.awbd_remark_text = rt;
                p1d.awbd_hide_entry = he;
                p1d.awbd_notification_type = nt1;
                p1d.awbd_notification_tasklist_config = ntc1;
                p1d.awbd_allow_edit_transaction = b4;
                
                    p1d.awbd_confirmation = b1;
                p1d.awbd_remark = b2;
                p1d.awbd_allow_pending = b3;


                db.SaveChanges();
            }
            catch (Exception e)
            {

                var m = new Btn_Detail_Model { tdid = tdid, Stage = s1, awbd_group_list = grp_list, awbd_button_number = pnum1, awbd_selection_criteria = sc, awbd_after_click_function_code = fc, awbd_after_click_action = aca, awbd_confirmation_text = ct, awbd_remark_text = rt, awbd_hide_entry = he, awbd_notification_type = nt1, awbd_notification_tasklist_config = ntc1, awbd_allow_edit_transaction = b4, awbd_confirmation = b1, awbd_remark = b2, awbd_allow_pending = b3 };
                db.Btn_Detail_Models.Add(m);
                db.SaveChanges();
            }



            return RedirectToAction("Test", new { wkflow = s, ndoc = ndoc });
        }


        //----------------------

        public ActionResult SaveTable_select(string tdid, string nextstg1, string rowno1 , string colno1, string btntext)
        {
            BtnModel p;
            int nextstg = Convert.ToInt32(nextstg1.Replace("\"",""));
            int rowno = Convert.ToInt32(rowno1)+1;
            int colno = Convert.ToInt32(colno1);
            int nextstgid=nextstg;
            

            try
            {
               p = (from x in db.BtnModels where x.tdid.Equals(tdid) //|| x.btn.Equals(btntext)
                                      
                                        select x).ToList().FirstOrDefault();
                                         p.nextstg=nextstgid;
                p.rowid = rowno;
                p.colid = colno;
            db.SaveChanges();
            }
            catch (Exception e)
            {
                p = null;

            }


             return RedirectToAction("Test");
           
        }

        //------------------------------

        public ActionResult new_wkflow_1(string wkflow,string ndoc)
        {
            IQueryable<StageModel> s;
            

            int ndoc1 = Convert.ToInt32(ndoc);

            try
            {
                s = (from x in db.StageModels
                     where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type==ndoc1
                     orderby x.rowno
                     select x);
                if (s.Count()>0)
                {
                    TempData["msg"] = "Workflow name already exists for this document type";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "Error creating Workflow";
                return RedirectToAction("Index");

            }
/*
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE AW_button_detail");
            db.Database.ExecuteSqlCommand("delete from TABLE AW_button_master");
            db.Database.ExecuteSqlCommand("delete from TABLE AW_master");

    */
            return RedirectToAction("Test",new { wkflow = wkflow,ndoc=ndoc});

        }

        //--------------------------------

        public ActionResult new_wkflow_2(string wkflow, string ndoc,string tmpl)
        {
            IQueryable<StageModel> s;
            StageModel s1;
            IQueryable<BtnModel> p1;
            IQueryable<Btn_Detail_Model> p2;
            IQueryable<history_master_Model> h;
            
            int table_id = Convert.ToInt32(tmpl);
            string t_wkflow;
            int t_doc;


            int ndoc1 = Convert.ToInt32(ndoc);

            try
            {
                s = (from x in db.StageModels
                     where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type == ndoc1
                     orderby x.rowno
                     select x);
                if (s.Count() > 0)
                {
                    TempData["msg"] = "Workflow name already exists for this document type";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "Error creating Workflow";
                return RedirectToAction("Index");

            }

            try
            {
                t_doc= (from x in db.history_master_Models
                     where x.table_id == table_id
                       select x.awhm_document_type).FirstOrDefault();


                t_wkflow = (from x in db.history_master_Models
                            where x.table_id == table_id
                            select x.awhm_workflow_name).FirstOrDefault();

               h = (from x in db.history_master_Models
                     where x.awhm_workflow_name.Equals(t_wkflow) && x.awhm_document_type==t_doc
                     
                     select x);

            }
            catch (Exception e)
            {
                h = null;

            }

            List<stage_old_new> l_s = new List<stage_old_new>{ };

          var  l_stageid = (from y in h
                 join x in db.StageModels
                 on new { f1 = y.awhm_workflow_name, f2 = y.awhm_document_type } equals new { f1 = x.awst_workflow_name, f2 = x.awst_document_type }
                 select x.awst_master_id).ToList().Distinct();

            s = (from x in db.StageModels
                 join y in l_stageid
                 on x.awst_master_id equals y
                 select x);

            foreach (var x1 in s)
            {
                l_s.Add( new stage_old_new { oid = x1.awst_master_id, rowno = x1.rowno });

            }

            var  p1a = (from x in db.BtnModels
                  join y1 in s
                  on x.Stage.awst_master_id equals y1.awst_master_id
                  select x);

            p1 = p1a.Include(x=>x.Stage);
            
            var p2a = (from x in db.Btn_Detail_Models
                  join y in p1
                  on new { f1 = x.Stage.awst_master_id, f2 = x.tdid } equals new { f1 = y.Stage.awst_master_id, f2 = y.tdid }
                  select x);
            p2 = p2a.Include(x => x.Stage);

           foreach (var s2 in s)
            {
                
                s2.awst_document_type = ndoc1;
                s2.awst_workflow_name = wkflow;
                db.StageModels.Add(s2);
            }
            db.SaveChanges();
            IEnumerable<StageModel> ss;
            IEnumerable<BtnModel> pp;
            pp = p1.ToList();
            ss = (from x in db.StageModels
                 where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type==ndoc1
                 select x).ToList();

            foreach (var x1 in ss)
            {
                var r = (from r_ in l_s
                         where r_.rowno == x1.rowno
                         select r_).ToList().FirstOrDefault();
                r.nid = x1.awst_master_id;
                
            }

            foreach (var p1_ in p1)
            {
                foreach (var rs in l_s)
                {
                    if (p1_.nextstg == rs.oid)
                    {
                        p1_.nextstg = rs.nid;
                        break;
                    }
                        }


                foreach (var s_ in ss)
                {
                    if ( p1_.Stage.rowno == s_.rowno)
                    {
                        p1_.Stage = s_;
                        

                        break;
                    }
                }
           //     var m = new BtnModel { tdid = p1_.tdid, btn = p1_.btn, rowid = p1_.rowid, colid = p1_.colid, nextstg = p1_.nextstg,  };
                db.BtnModels.Add(p1_);
            }

            int a_rowno=0;

            foreach (var p2_ in p2)
            {
               
                foreach (var p1_ in pp)
                {    
                    foreach (var ss_ in ss)
                    {
                        if (p1_.Stage.awst_master_id == ss_.awst_master_id)
                        {
                            a_rowno = ss_.rowno;
                        }

                    }

                    if (p1_.tdid == p2_.tdid && a_rowno == p2_.Stage.rowno)
                    {
                        p2_.Stage = p1_.Stage;
                       
                        break;
                    }
                }
              
                db.Btn_Detail_Models.Add(p2_);
            }

            db.SaveChanges();

            return RedirectToAction("Test", new { wkflow = wkflow, ndoc = ndoc });

        }
        //------------------------

        public ActionResult load_wkflow(string tmpl)
        {
            IQueryable<history_master_Model> h;
            int table_id = Convert.ToInt32(tmpl);
            string t_wkflow;
            int t_doc;

            try
            {
                t_doc = (from x in db.history_master_Models
                         where x.table_id == table_id
                         select x.awhm_document_type).FirstOrDefault();


                t_wkflow = (from x in db.history_master_Models
                            where x.table_id == table_id
                            select x.awhm_workflow_name).FirstOrDefault();

                

            }
            catch (Exception e)
            {
                t_doc = -1;
                t_wkflow = "";

            }
            return RedirectToAction("Test", new { wkflow = t_wkflow, ndoc = t_doc });
        }



        //---------------------------

        public ActionResult Test(string wkflow, string ndoc)
        {  
            IQueryable<StageModel> s;
            IQueryable<BtnModel> p;
                IQueryable<document_type_Model> d;
                   IQueryable<Group_Master_Model> g;
            string stagelist = "<select name=nstage>";
            string doc_name="";
            int i = 1;
            int ndoc1 = Convert.ToInt32(String.IsNullOrWhiteSpace(ndoc) ? "0" :ndoc);

            try
                   {
                       s = (from x in db.StageModels where x.awst_workflow_name.Equals(wkflow) && x.awst_document_type == ndoc1
                            orderby x.rowno
                                                   select x);
                   }
                   catch (Exception e)
                   {
                       s = null;

                   }
                  try
                   {
                      p = (from x in db.BtnModels where x.Stage.awst_workflow_name.Equals(wkflow) && x.Stage.awst_document_type == ndoc1
                           orderby x.rowid
                                               select x);
                   }
                   catch (Exception e)
                   {
                       p = null;

                   }

            try
            {
                d = (from x in db.document_type_Models
                     orderby x.table_id
                     select x);
            }
            catch (Exception e)
            {
                d = null;

            }

            try
            {
                g = (from x in db.Group_Master_Models
                     orderby x.table_id
                     select x);
            }
            catch (Exception e)
            {
                g = null;

            }

            
           
           
                  foreach (var stg in s)
                  {
                   stagelist = stagelist + "<option value = " + stg.awst_master_id.ToString() + ">" + stg.stg + "</option>";
               
                  } 
                
           
                             stagelist = stagelist + "</select>";
                                   ViewBag.data = stagelist;




           

                                           string doclist = "<select name=ndoc>";

                                           foreach (var doc in d)
                                           {
                                               doclist = doclist + "<option value = " + doc.table_id.ToString() + ">" + doc.awdt_name + "</option>";
                                            if (doc.table_id==Convert.ToInt32(ndoc)) { doc_name = doc.awdt_name; }
                                           }

                                           doclist = doclist + "</select>";
                                           ViewBag.data_d = doclist;






                                           string grplist = "<select name=ngroup multiple=multiple>";

                                           foreach (var grp in g)
                                           {
                                               grplist = grplist + "<option value = " + grp.table_id.ToString() + ">" + grp.gm_en_name + "</option>";

                                           }

                                           grplist = grplist + "</select>";
                                           ViewBag.data_g = grplist;




                           ViewBag.s = s.ToList();
                               ViewBag.p = p.ToList();
            ViewBag.wkflow = wkflow;
            ViewBag.ndoc = ndoc;
            ViewBag.doc_name = doc_name;
            return View();
        }

        public ActionResult Tables(string wkflow, string ndoc, string s = "dbo.AW_master")
        {

            DataTable dt = new DataTable();

            
            string query = "select * from "+s;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Test_redips_jsContext"].ConnectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            ViewBag.wkflow = wkflow;
            ViewBag.ndoc = ndoc;
            return View(dt);
        }


            public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}