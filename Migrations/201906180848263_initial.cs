namespace Test_redips_js.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AW_button_detail",
                c => new
                    {
                        awbd_button_id = c.Int(nullable: false, identity: true),
                        awbd_selection_criteria = c.String(),
                        awbd_group_list = c.String(),
                        awbd_after_click_function_code = c.String(maxLength: 255),
                        awbd_after_click_action = c.String(maxLength: 255),
                        awbd_confirmation = c.Boolean(nullable: false),
                        awbd_confirmation_text = c.String(),
                        awbd_remark = c.Boolean(nullable: false),
                        awbd_remark_text = c.String(),
                        awbd_notification_type = c.Int(nullable: false),
                        awbd_notification_tasklist_config = c.Int(nullable: false),
                        awbd_allow_edit_transaction = c.Boolean(nullable: false),
                        awbd_hide_entry = c.String(),
                        awbd_allow_pending = c.Boolean(nullable: false),
                        awbd_button_number = c.Int(nullable: false),
                        awbm_button_cid = c.String(maxLength: 10),
                    awst_master_id = c.Int(),
                    })
                .PrimaryKey(t => t.awbd_button_id)
                .ForeignKey("dbo.AW_master", t => t.awst_master_id)
                .Index(t => t.awst_master_id);
            
            CreateTable(
                "dbo.AW_master",
                c => new
                    {
                        awst_master_id = c.Int(nullable: false, identity: true),
                        awst_stage_description = c.String(maxLength: 255),
                        awst_stage_no = c.Int(nullable: false),
                        awst_document_type = c.Int(nullable: false),
                        awst_workflow_name = c.String(maxLength: 255),
                        awst_group_list = c.String(maxLength: 255),
                        awst_function_code = c.String(maxLength: 255),
                        awst_status = c.String(maxLength: 255),
                        awst_rowno = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.awst_master_id);
            
            CreateTable(
                "dbo.AW_button_master",
                c => new
                    {
                        awbm_button_id = c.Int(nullable: false, identity: true),
                        awbm_button_cid = c.String(maxLength: 10),
                        awbm_button_text = c.String(),
                        awbm_button_colno = c.Int(nullable: false),
                        awbm_button_rowno = c.Int(nullable: false),
                        awbm_next_stage = c.Int(nullable: false),
                    awst_master_id = c.Int(),
                })
                .PrimaryKey(t => t.awbm_button_id)
                .ForeignKey("dbo.AW_master", t => t.awst_master_id)
                .Index(t => t.awst_master_id);

            CreateTable(
                "dbo.AW_document_type",
                c => new
                    {
                        awhd_table_id = c.Int(nullable: false, identity: true),
                        awdt_name = c.String(maxLength: 255),
                        awdt_transaction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.awhd_table_id);
            
            CreateTable(
                "dbo.Group_Master",
                c => new
                    {
                        TableId = c.Int(nullable: false, identity: true),
                        cm_id = c.Int(nullable: false),
                        gm_seq_no = c.Int(nullable: false),
                        gm_en_name = c.String(maxLength: 256),
                        gm_tc_name = c.String(maxLength: 256),
                        gm_sc_name = c.String(maxLength: 256),
                        gm_lockedby = c.String(maxLength: 128),
                        gm_updatedby = c.String(maxLength: 128),
                        gm_createdby = c.String(maxLength: 128),
                        gm_effective_date = c.DateTime(nullable: false),
                        gm_expiry_date = c.DateTime(nullable: false),
                        gm_updatedate = c.DateTime(nullable: false),
                        gm_createdate = c.DateTime(nullable: false),
                        Series = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TableId);
            
            CreateTable(
                "dbo.AW_history_detail",
                c => new
                    {
                        awhd_table_id = c.Int(nullable: false, identity: true),
                        awhd_original_status = c.String(maxLength: 255),
                        awhd_revise_status = c.String(maxLength: 255),
                        awhd_remark = c.String(maxLength: 255),
                        awhd_created_date = c.DateTime(nullable: false),
                        history_Master_Model_table_id = c.Int(),
                    })
                .PrimaryKey(t => t.awhd_table_id)
                .ForeignKey("dbo.AW_history_master", t => t.history_Master_Model_table_id)
                .Index(t => t.history_Master_Model_table_id);
            
            CreateTable(
                "dbo.AW_history_master",
                c => new
                    {
                        awhm_table_id = c.Int(nullable: false, identity: true),
                        awhm_document_type = c.Int(nullable: false),
                        awhm_workflow_name = c.String(maxLength: 255),
                        awhm_start_record_id = c.Int(nullable: false),
                        awhm_record_id = c.Int(nullable: false),
                        awhm_from_stage = c.Int(nullable: false),
                        awhm_to_stage = c.Int(nullable: false),
                        awhm_remark = c.String(),
                        awhm_button_number = c.Int(nullable: false),
                        awhm_group = c.String(maxLength: 255),
                        awhm_user = c.String(maxLength: 255),
                        awhm_created_date = c.DateTime(nullable: false),
                        awbm_button_cid = c.String(),
                    awst_master_id = c.Int(),
                    })
                .PrimaryKey(t => t.awhm_table_id)
                .ForeignKey("dbo.AW_master", t => t.awst_master_id)
                .Index(t => t.awst_master_id);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AW_history_detail", "history_Master_Model_table_id", "dbo.AW_history_master");
            DropForeignKey("dbo.AW_history_master", "awst_master_id", "dbo.AW_master");
            DropForeignKey("dbo.AW_button_master", "awbm_button_rowno", "dbo.AW_master");
            DropForeignKey("dbo.AW_button_detail", "awst_master_id", "dbo.AW_master");
            DropIndex("dbo.AW_history_master", new[] { "awst_master_id" });
            DropIndex("dbo.AW_history_detail", new[] { "history_Master_Model_table_id" });
            DropIndex("dbo.AW_button_master", new[] { "awst_master_id" });
            DropIndex("dbo.AW_button_detail", new[] { "awst_master_id" });
            DropTable("dbo.AW_history_master");
            DropTable("dbo.AW_history_detail");
            DropTable("dbo.Group_Master");
            DropTable("dbo.AW_document_type");
            DropTable("dbo.AW_button_master");
            DropTable("dbo.AW_master");
            DropTable("dbo.AW_button_detail");
        }
    }
}
