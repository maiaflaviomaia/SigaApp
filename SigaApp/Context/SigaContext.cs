using Microsoft.EntityFrameworkCore;
using SigaApp.Models.Entidades;

namespace SigaApp.Context
{
    public class SigaContext : DbContext
    {
        public SigaContext(DbContextOptions<SigaContext> options) : base(options)
        {

        }

        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<CentroDeCusto> CentroDeCustos { get; set; }
        public DbSet<Cliente> Clietes { get; set; }
        public DbSet<ContaContabil> ContasContabeis { get; set; }
        public DbSet<ContaPagar> ContasPagar { get; set; }
        public DbSet<ContaReceber> ContasRebecer { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Estudio> Estudios { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Lancamento> Lancamentos { get; set; }
        public DbSet<MensagemSite> MensagensSite { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }
        public DbSet<OrcamentoCustos> OrcamentoCustos { get; set; }
        public DbSet<OrcamentoFornecedor> OrcamentoFornecedores { get; set; }
        public DbSet<OrcamentoServico> OrcamentoServicos { get; set; }
        public DbSet<ServicoPrestado> ServicosPrestados { get; set; }
        public DbSet<SessaoGravacao> SessoesGravacoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Cliente>().ToTable("CLIENTES");
            builder.Entity<Fornecedor>().ToTable("FORNECEDORES");
            builder.Entity<ServicoPrestado>().ToTable("SERVICO_PRESTADO");
            builder.Entity<Cargo>().ToTable("CARGOS");
            builder.Entity<Funcionario>().ToTable("FUNCIONARIOS");
            builder.Entity<Usuario>().ToTable("USUARIOS");
            builder.Entity<Estudio>().ToTable("ESTUDIOS");
            builder.Entity<SessaoGravacao>().ToTable("SESSAO_GRAVACAO");
            builder.Entity<Orcamento>().ToTable("ORCAMENTOS");
            builder.Entity<OrcamentoServico>().ToTable("ORCAMENTO_SERVICO");
            builder.Entity<OrcamentoFornecedor>().ToTable("ORCAMENTO_FORNECEDOR");
            builder.Entity<OrcamentoCustos>().ToTable("ORCAMENTO_CUSTO");
            builder.Entity<ContaContabil>().ToTable("CONTA_CONTABIL");
            builder.Entity<Categoria>(x => {
                x.ToTable("CATEGORIAS");
                x.HasKey(c => c.CategoriaID);
                x.HasMany(c => c.SubCategoria).WithOne().HasForeignKey(c => c.CategoriaPai).HasPrincipalKey(c => c.CategoriaID);
            });
            builder.Entity<ContaPagar>().ToTable("CONTAS_PAGAR");
            builder.Entity<Empresa>().ToTable("EMPRESAS");
            builder.Entity<ContaReceber>().ToTable("CONTAS_RECEBER");
            builder.Entity<CentroDeCusto>().ToTable("CENTRO_DE_CUSTO");
            builder.Entity<Lancamento>().ToTable("LANCAMENTOS");
            builder.Entity<Agenda>().ToTable("AGENDA");
            builder.Entity<MensagemSite>().ToTable("MENSAGENS_SITE");
        }
    }
}
