using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;



namespace Networking.Client
{
    public enum AuthState
    {
        NotAuthenticated,   // Aún no se ha iniciado sesión
        Authenticating,     // Intentando autenticar
        Authenticated,      // Login exitoso
        Error,              // Error general
        Timeout             // Se agotaron reintentos
    }
}



namespace Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState State { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxRetries = 3)
        {
            Debug.Log("Estado INICIAL: " + State);

            //  Si ya está autenticado -> salir
            if (State == AuthState.Authenticated)
                return State;

            //  Si ya se está autenticando -> esperar
            if (State == AuthState.Authenticating)
                return await WaitForAuthentication();

            State = AuthState.Authenticating;

            await SignInAnonymouslyAsync(maxRetries);

            return State;
        }
        

        private static async Task SignInAnonymouslyAsync(int maxRetries)
        {

            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    Debug.Log($"Intento de login #{attempt + 1}"+" Estado Actual: "+ State);

                    if (!AuthenticationService.Instance.IsSignedIn)
                    {
                        await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    }

                    if (AuthenticationService.Instance.IsSignedIn &&
                        AuthenticationService.Instance.IsAuthorized)
                    {
                        State = AuthState.Authenticated;
                        Debug.Log("Autenticación exitosa."+" Estado Actual: "+ State);
                        return;
                    }
                }
                catch (AuthenticationException e)
                {
                    Debug.LogError($"AuthenticationException: {e}"+ "Estado Actual: " + State);
                    State = AuthState.Error;
                }
                catch (RequestFailedException e)
                {
                    Debug.LogError($"RequestFailedException: {e}"+ "  Estado Actual: " + State);
                    State = AuthState.Error;
                }

                attempt++;
                await Task.Delay(1000);
            }

            //  Si llegamos aquí -> timeout
            State = AuthState.Timeout;
            Debug.LogWarning("Autenticación agotó los reintentos."+ "  Estado Actual: " + State);
        }

        //  MÉTODO PARA ESPERAR SI YA SE ESTÁ AUTENTICANDO
        private static async Task<AuthState> WaitForAuthentication()
        {
            while (State == AuthState.Authenticating)
            {
                await Task.Delay(200);
            }

            return State;
        }
    }
}
