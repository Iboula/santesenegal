using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SanteSenegal.Api.Endpoints;

public static class SousServiceEndpoints
{
    public static void MapSousServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sous-services");

        // TODO: Implémenter les endpoints SousService quand les Query/Command seront créées
        // GetAll, GetById, GetByServiceId, GetBySpecialite, GetAvailable, Search, GetByPriceRange
        // Create, Update, UpdatePrice

        group.MapGet("/", () => Results.Ok(new List<object>()));
    }
}
