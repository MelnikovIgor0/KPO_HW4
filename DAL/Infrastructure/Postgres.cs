﻿using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.NameTranslation;
using DAL.Entities;

namespace DAL.Infrastructure;

public static class Postgres
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

    /// <summary>
    /// Map DAL models to composite types (enables UNNEST)
    /// </summary>
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        mapper.MapComposite<Dish>("dish", Translator);
        mapper.MapComposite<Dish>("order", Translator);
        mapper.MapComposite<Dish>("order_dish", Translator);
        mapper.MapComposite<Session>("session", Translator);
        mapper.MapComposite<User>("user", Translator);
    }

    /// <summary>
    /// Add migration infrastructure
    /// </summary>
    public static void AddMigrations(IServiceCollection services)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(Constants.ConnectionString)
                .ScanIn(typeof(Postgres).Assembly).For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
}