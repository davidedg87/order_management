using PhotoSiTest.API.Middlewares;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder builder)
        {
            // Middleware per reindirizzare le richieste HTTP a HTTPS
            builder.UseHttpsRedirection();
            // Middleware di routing deve essere chiamato prima di altri middleware che dipendono da routing
            builder.UseRouting();
            builder.UseMiddleware<ExceptionHandlingMiddleware>();        
            builder.UseAuthorization(); // Aggiunta del middleware di autorizzazione
            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return builder;
        }
    }

  
}
