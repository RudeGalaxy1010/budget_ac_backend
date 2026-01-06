using System.Text;
using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Auth.Validation;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Repository.SqlLite;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace budget_ac_backend.App.Auth;

public static class AuthBuilder {
    public const string IssuerName = "budget_ac_service";

    public static void AddAuth(this WebApplicationBuilder builder) {
        builder.Services.AddSingleton<IKeystoreService>(serviceProvider => {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return new KeystoreService(configuration);
        });
        builder.Services.TryAddSingleton<IPasswordHashService, PasswordHashService>();
        builder.Services.TryAddSingleton<ITokenGeneratorService, TokenGeneratorService>();
        builder.Services.TryAddSingleton<IValidator<CreateUserRequestData>, CreateUserRequestDataValidator>();
        builder.Services.TryAddSingleton<IValidator<LoginUserRequestData>, LoginRequestDataValidator>();
        builder.Services.TryAddSingleton<IValidator<RefreshTokenRequestData>, RefreshTokenRequestValidator>();

        builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        IKeystoreService keystore = context.HttpContext.RequestServices.GetRequiredService<IKeystoreService>();
                        string key = keystore.GetSecretKey();
                        context.Options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = IssuerName,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        if (builder.Environment.IsDevelopment()) {
            builder.Services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Use token in format: Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

    }
}
