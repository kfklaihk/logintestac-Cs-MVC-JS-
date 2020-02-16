using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Test_redips_js.Models
{
    [Table("AW_master")]
    public class StageModel
    {
        [Key, Column("awst_master_id")]
        public int awst_master_id { get; set; }
        [Column("awst_stage_description"), StringLength(255)]
        public string stg { get; set; }
        
        public int awst_stage_no { get; set; }
        public  int awst_document_type{get; set;}
        [StringLength(255)]
        public string awst_workflow_name { get; set; }
        [StringLength(255)]
        public string awst_group_list { get; set; }
        [StringLength(255)]
        public string awst_function_code { get; set; }
        [StringLength(255)]
        public string awst_status { get; set; }
        [Column("awst_rowno")]
        public int rowno { get; set; }
        //   public ICollection<BtnModel> BtnModels { get; set; }
    }

    [Table("AW_button_master")]
    public class BtnModel 
    {
        [Key, Column("awbm_button_id")]
        public int Id { get; set; }
        [Column("awbm_button_cid"), StringLength(10)]
        public string tdid { get; set; }
        [Column("awbm_button_text")]
        public string btn { get; set; }
        [Column("awbm_button_colno")]
        public int colid { get; set; }
        [Column("awbm_button_rowno")]
        public int rowid { get; set; }
        [Column("awbm_next_stage")]
        public int nextstg { get; set; }
       
        public int awst_master_id { get; set; }
        public StageModel Stage { get; set; }




    }

    [Table("AW_button_detail")]
    public class Btn_Detail_Model
    {
        [Key, Column("awbd_button_id")]
        public int Id { get; set; }
        public string awbd_selection_criteria { get; set; }
        public string awbd_group_list { get; set; }
        [StringLength(255)]
        public string awbd_after_click_function_code { get; set; }
        [StringLength(255)]
        public string awbd_after_click_action { get; set; }
        public bool awbd_confirmation { get; set; }
        public string awbd_confirmation_text { get; set; }
        public bool awbd_remark { get; set; }
        public string awbd_remark_text { get; set; }
        public int awbd_notification_type { get; set; }
        public int awbd_notification_tasklist_config { get; set; }
        public bool awbd_allow_edit_transaction { get; set; }
        public string awbd_hide_entry { get; set; }
        public bool awbd_allow_pending { get; set; }
        public int awbd_button_number { get; set; }
        
        [Column("awbm_button_cid"), StringLength(10)]
        public string tdid { get; set; }
      
        public int awst_master_id { get; set; }
        public StageModel Stage { get; set; }
        

    }



        [Table("AW_history_master")]
    public class history_master_Model
    {
        [Key, Column("awhm_table_id")]
        public int table_id { get; set; }
      
        public int awhm_document_type { get; set; }
        [StringLength(255)]
        public string awhm_workflow_name { get; set; }

        public int awhm_start_record_id { get; set; }
        public int awhm_record_id { get; set; }
        
        public int awhm_from_stage { get; set; }
        public int awhm_to_stage { get; set; }
        public string awhm_remark { get; set; }
        public int awhm_button_number { get; set; }
        [StringLength(255)]
        public string awhm_group { get; set; }
        [StringLength(255)]
        public string awhm_user { get; set; }
        public DateTime awhm_created_date { get; set; }
        

        
        [Column("awbm_button_cid")]
        public string tdid { get; set; }
        
        public int awst_master_id { get; set; }
        public StageModel Stage { get; set; }





    }


    [Table("AW_history_detail")]
    public class history_detail_Model
    {
        [Key, Column("awhd_table_id")]
        public int table_id { get; set; }

        [StringLength(255)]
        public string awhd_original_status { get; set; }
        [StringLength(255)]
        public string awhd_revise_status { get; set; }
        [StringLength(255)]
        public string awhd_remark { get; set; }
        public DateTime awhd_created_date { get; set; }

        [Key, Column("awhm_table_id")]
        public history_master_Model history_Master_Model { get; set; }

    }

    [Table("AW_document_type")]
    public class document_type_Model
    {
        [Key, Column("awhd_table_id")]
        public int table_id { get; set; }
        [StringLength(255)]
        public string awdt_name { get; set; }
        public bool awdt_transaction { get; set; }
    }

    [Table("Group_Master")]
    public class Group_Master_Model
    {
        [Key, Column("TableId")]
        public int table_id { get; set; }
        public int cm_id { get; set; }
        public int gm_seq_no { get; set; }
        [StringLength(256)]
        public string gm_en_name { get; set; }
        [StringLength(256)]
        public string gm_tc_name { get; set; }
        [StringLength(256)]
        public string gm_sc_name { get; set; }
        [StringLength(128)]
        public string gm_lockedby { get; set; }
        [StringLength(128)]
        public string gm_updatedby { get; set; }
        [StringLength(128)]
        public string gm_createdby { get; set; }

        public DateTime gm_effective_date { get; set; }
        public DateTime gm_expiry_date { get; set; }
        public DateTime gm_updatedate { get; set; }
        public DateTime gm_createdate { get; set; }
        public int Series { get; set; }
    }

}