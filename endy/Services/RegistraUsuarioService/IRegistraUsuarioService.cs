namespace endy.Services.RegistraUsuarioService
{
    public interface IRegistraUsuarioService
    {
        bool registrarUsuario(string userName, string pass);
        bool loginUsuarioService(string userName, string pass);
    }
}
