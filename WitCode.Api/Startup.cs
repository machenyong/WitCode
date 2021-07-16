using WitCode.Common;
using WitCode.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using NLog.Extensions.Logging;
using Autofac;
using System.Reflection;
using WitCode.Service;
using AspNetCoreRateLimit;
using WitCode.Core.Common.Helpers;
using WitCode.Common.Configs;

namespace WitCode.Api
{
    public class Startup
    {
        private readonly ConfigHelper _configHelper;
        private readonly IHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _configHelper = new ConfigHelper();
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           
            #region ��������
            //��������
            services.AddOptions();
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);//���ü����԰汾
            services.AddMemoryCache();
            //����IpRateLimiting����
            //services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            var ratelimitconfig = _configHelper.Load("ratelimitconfig", _env.EnvironmentName, true);    //��ȡ�Զ���IP����json����
            services.Configure<IpRateLimitOptions>(ratelimitconfig.GetSection("IpRateLimiting"));
            //ע��������͹���洢
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //��ӿ�ܷ���
            services.AddMvc();
            // clientId / clientIp������ʹ������
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //���ã���������Կ��������
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion

            #region AutoMapper �Զ�ӳ��
            var serviceAssembly = Assembly.Load("WitCode.Service");
            services.AddAutoMapper(serviceAssembly);
            //services.AddAutoMapper(typeof(AutoMapperConfig));
            #endregion AutoMapper �Զ�ӳ��

            #region ��־
            //��ӵ����� �����Ĵ�ȡ��
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //��ӵ����� NLog������
            services.AddSingleton<INLogHelper, NLogHelper>();
            //���ó�ȫ�ֵķ���
            //ģ����ͼ��������ӹ�����(CustomExceptionFilter)
            //AddControllers����ֻ��ӿ�����������
            services.AddMvc(config => config.Filters.Add(typeof(CustomExceptionFilter)));
            #endregion 

            #region     ��̨ ԭ�����
            //< !--��Startup���ConfigureServices()�����н������ã�DefaultContractResolver() ԭ����������ص� json ���̨����һ��-- >
            services.AddControllers().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);
            #endregion  //

            #region ���ݿ�DB����
            var dbConfig = _configHelper.Get<DbConfig>("dbconfig", _env.EnvironmentName, true); //��ȡ���ݿ�����
           // services.AddSingleton(dbConfig);     
            #endregion

            #region ����
            //����
            services.AddCors(c =>
            {
                c.AddPolicy("AllRequests", policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                //����swagger��֤����
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //���ȫ�ְ�ȫ����
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"}
            },new string[] { }
        }
    });
                
            });

            #region ע��jwt
            JWTTokenOptions JWTTokenOptions = new JWTTokenOptions();

            //��ȡappsettings������
            services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTToken"));
            //�������Ķ���ʵ���󶨵�ָ�������ý�
            Configuration.Bind("JWTToken", JWTTokenOptions);

            //ע�ᵽIoc����
            services.AddSingleton(JWTTokenOptions);

            //�����֤����
            services.AddAuthentication(option =>
            {
                //Ĭ�������֤ģʽ
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //Ĭ�Ϸ���
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(option =>
            {
                //����Ԫ���ݵ�ַ��Ȩ���Ƿ���ҪHTTP
                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                //������֤����
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //��ȡ������Ҫʹ�õ�Microsoft.IdentityModel.Tokens.SecurityKey����ǩ����֤��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
                    GetBytes(JWTTokenOptions.Secret)),
                    //��ȡ������һ��System.String������ʾ��ʹ�õ���Ч�����߼����ҵķ����ߡ� 
                    ValidIssuer = JWTTokenOptions.Issuer,
                    //��ȡ������һ���ַ��������ַ�����ʾ�����ڼ�����Ч���ڷ������ƵĹ��ڡ�
                    ValidAudience = JWTTokenOptions.Audience,
                    //�Ƿ���֤������
                    ValidateIssuer = false,
                    //�Ƿ���֤������
                    ValidateAudience = false,
                    ////����ķ�����ʱ��ƫ����
                    ClockSkew = TimeSpan.Zero,
                    ////�Ƿ���֤Token��Ч�ڣ�ʹ�õ�ǰʱ����Token��Claims�е�NotBefore��Expires�Ա�
                    ValidateLifetime = true
                };
                //���jwt���ڣ��ڷ��ص�header�м���Token-Expired�ֶ�Ϊtrue��ǰ���ڻ�ȡ����headerʱ�ж�
                option.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                #region Nlog
                //���NLog
                loggerFactory.AddNLog();
                //��������
                NLog.LogManager.LoadConfiguration("NLog.config");
                //�����Զ�����м��
                app.UseLog();
                #endregion

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WitCode.Api v1"));
            }
            app.UseIpRateLimiting();    //��������
            app.UseRouting();
            app.UseStaticFiles();   //���þ�̬�ļ�
            app.UseAuthentication();  //�����֤
            app.UseAuthorization();
            app.UseCors("AllRequests");//����Cors���������м��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        //��������
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<UserService>().As<IUserService>();     //���캯��ע��

            //builder.RegisterType<UserService>().As<IUserService>().PropertiesAutowired(); //����ע��


            #region ����ע��
            //builder.RegisterAssemblyTypes(typeof(Program).Assembly)
           
            var assemblyRepository = Assembly.Load("WitCode.Repository");
            builder.RegisterAssemblyTypes(assemblyRepository)
                .Where(t => t.Name.EndsWith("Service"))     //ע����Service��β
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            #endregion




        }
    }
}
