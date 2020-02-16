using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace Test_redips_js.Models
{
    public class Test_redips_jsContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public Test_redips_jsContext() : base("name=Test_redips_jsContext")
        {
            Database.SetInitializer<Test_redips_jsContext>(new CreateDatabaseIfNotExists<Test_redips_jsContext>());
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Test_redips_js.Models.StageModel> StageModels { get; set; }
        public DbSet<Test_redips_js.Models.BtnModel> BtnModels { get; set; }
        public DbSet<Test_redips_js.Models.Btn_Detail_Model> Btn_Detail_Models { get; set; }
        public DbSet<Test_redips_js.Models.history_master_Model> history_master_Models { get; set; }
        public DbSet<Test_redips_js.Models.history_detail_Model> history_detail_Models { get; set; }
        public DbSet<Test_redips_js.Models.document_type_Model> document_type_Models { get; set; }
        public DbSet<Test_redips_js.Models.Group_Master_Model> Group_Master_Models { get; set; }
    }
}
