using System.Text;
using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Auth.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace budget_ac_backend.App.Auth;

public static class AuthBuilder {
    public const string ThisIssuer = "budget_ac_service";

    public static void AddAuth(this WebApplicationBuilder builder) {
        KeystoreService keystoreService = new KeystoreService(builder.Configuration);

        builder.Services.TryAddSingleton<IPasswordHashService>(new PasswordHashService());
        builder.Services.TryAddSingleton<ITokenGeneratorService>(new TokenGeneratorService(keystoreService));
        builder.Services.TryAddSingleton<IValidator<CreateUserRequestData>>(new CreateUserRequestDataValidator());
        builder.Services.TryAddSingleton<IValidator<LoginUserRequestData>>(new LoginRequestDataValidator());
        builder.Services.TryAddSingleton<IValidator<RefreshTokenRequestData>>(new RefreshTokenRequestValidator());

        builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keystoreService.GetSecretKey())),
                    ValidateIssuer = true,
                    ValidIssuer = ThisIssuer,
                    ValidateAudience = false,
                    ValidateLifetime = true
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