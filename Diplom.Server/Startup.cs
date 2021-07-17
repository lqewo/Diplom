using System;
using System.IdentityModel.Tokens.Jwt;
using Diplom.Common.Entities;
using Diplom.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Diplom.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // получаем строку подключения из файла конфигурации
            var connection = Configuration.GetConnectionString("DefaultConnection");

            // добавляем контекст ApplicationContext в качестве сервиса в приложение
            services.AddDbContext<ApplicationDbContext>(options =>
                                                                options.UseSqlServer(connection));

            services.AddIdentity<SiteUser, IdentityRole>(options =>
                    {
                        options.User.RequireUniqueEmail = true; // у каждого юзера уникальная почта
                        options.Password.RequireDigit = false; // в пароле НЕ требуются обязательно наличие цифр
                        options.Password.RequiredLength = 6; // длина пароля
                        options.Password.RequireUppercase = false; // в пароле НЕ требуется обязательно заглавные буквы
                        options.Password.RequireLowercase = false; // в пароле НЕ требуется обязательно в нижнем регистре буквы
                        options.Password.RequireNonAlphanumeric = false; // в пароле НЕ требуется обязательно символы
                        options.Password.RequiredUniqueChars = 0; // количество обязательных символов в пароле
                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>(); // где будут юзеры

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // строка, представляющая издателя
                            ValidIssuer = AuthService.Issuer,

                            // установка потребителя токена
                            ValidAudience = AuthService.Audience,

                            // установка ключа безопасности
                            IssuerSigningKey = AuthService.GetSymmetricSecurityKey(),
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors(builder => builder.AllowAnyOrigin()
                                          .AllowAnyMethod());
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}