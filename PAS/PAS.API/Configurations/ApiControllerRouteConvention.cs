using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace PAS.API.Configurations;

public sealed class ApiControllerRouteConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix;

    public ApiControllerRouteConvention(string routePrefix)
    {
        _routePrefix = new AttributeRouteModel(new RouteAttribute(routePrefix));
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            {
                continue;
            }

            foreach (var selector in controller.Selectors)
            {
                selector.AttributeRouteModel = _routePrefix;
            }
        }
    }
}
