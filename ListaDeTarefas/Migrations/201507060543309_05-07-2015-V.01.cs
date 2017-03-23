namespace ListaDeTarefas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _05072015V01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lista",
                c => new
                    {
                        ListaId = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Ativa = c.Boolean(nullable: false),
                        UsuarioId = c.Int(nullable: false),
                        Prazo = c.DateTime(),
                    })
                .PrimaryKey(t => t.ListaId)
                .ForeignKey("dbo.Usuario", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.Tarefa",
                c => new
                    {
                        TarefaId = c.Int(nullable: false, identity: true),
                        Nome = c.String(),
                        Concluida = c.Boolean(nullable: false),
                        Ativa = c.Boolean(nullable: false),
                        ListaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TarefaId)
                .ForeignKey("dbo.Lista", t => t.ListaId, cascadeDelete: true)
                .Index(t => t.ListaId);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        UsuarioId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Senha = c.String(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lista", "UsuarioId", "dbo.Usuario");
            DropForeignKey("dbo.Tarefa", "ListaId", "dbo.Lista");
            DropIndex("dbo.Tarefa", new[] { "ListaId" });
            DropIndex("dbo.Lista", new[] { "UsuarioId" });
            DropTable("dbo.Usuario");
            DropTable("dbo.Tarefa");
            DropTable("dbo.Lista");
        }
    }
}
