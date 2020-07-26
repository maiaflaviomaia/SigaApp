using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Repository;

namespace SigaApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                    options.LoginPath = "/Usuario/Login";
                    options.LogoutPath = "/Usuario/Logout";
                    options.AccessDeniedPath = "/Usuario/Login";
                });

            services.AddDbContext<SigaContext>(options => options.UseMySql(Configuration.GetConnectionString("SigaConnection")));

            services.Configure<Email>(Configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmail, EmailRepository>();

            services.AddScoped<ICliente, ClienteRepository>();
            services.AddScoped<IFornecedor, FornecedorRepository>();
            services.AddScoped<ICargo, CargoRepository>();
            services.AddScoped<IServicoPrestado, ServicoPrestadoRepository>();
            services.AddScoped<IFuncionario, FuncionarioRepository>();
            services.AddScoped<IUsuario, UsuarioRepository>();
            services.AddScoped<IEstudio, EstudioRepository>();
            services.AddScoped<ISessaoGravacao, SessaoGravacaoRepository>();
            services.AddScoped<IOrcamento, OrcamentoRepository>();
            services.AddScoped<IOrcamentoServico, OrcamentoServicoRepository>();
            services.AddScoped<IOrcamentoFornecedor, OrcamentoFornecedorRepository>();
            services.AddScoped<IOrcamentoCusto, OrcamentoCustoRepository>();
            services.AddScoped<IContaContabil, ContaContabilRepository>();
            services.AddScoped<IContaPagar, ContaPagarRepository>();
            services.AddScoped<ICategoria, CategoriaRepository>();
            services.AddScoped<IEmpresa, EmpresaRepository>();
            services.AddScoped<IContaReceber, ContaReceberRepository>();
            services.AddScoped<ICentroDeCusto, CentroDeCustoRepository>();
            services.AddScoped<ILancamento, LancamentoRepository>();
            services.AddScoped<IAgenda, AgendaRepository>();
            services.AddScoped<IMensagemSite, MensagemSiteRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Site}/{action=PaginaInicial}/{id?}");
            });

            RotativaConfiguration.Setup(env);
        }
    }
}
