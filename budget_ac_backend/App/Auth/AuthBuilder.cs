using System.Text;
using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Auth.Validation;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
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

        List<User> userProfiles = new List<User>();

        // Test user
        // Login: john.doe@gmail.com
        // Password: string
        if (builder.Environment.IsDevelopment()) {
            userProfiles.Add(new User {
                Id = -1,
                Name = "John Doe",
                Email = "john.doe@gmail.com",
                PasswordHash = [30, 45, 115, 9, 174, 46, 251, 240, 12, 83, 14, 124, 209, 15, 118, 218],
                Salt = [221, 126, 68, 171, 79, 190, 50, 187, 252, 113, 105, 127, 203, 117, 8, 229],
                RegisteredAt = DateTime.UtcNow,
                RefreshToken = "##########",
                RefreshExpiresAt = DateTime.UtcNow
            });
        }

        builder.Services.TryAddSingleton<IUserRepository>(new InMemoryUserRepository(userProfiles));

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