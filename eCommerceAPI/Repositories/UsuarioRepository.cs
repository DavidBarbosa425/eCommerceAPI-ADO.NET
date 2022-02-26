using eCommerceAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace eCommerceAPI.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private IDbConnection _connection;

        public UsuarioRepository()
        {
            _connection = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=eCommerce;Data Source=DESKTOP-G95Q0DC\\SQLEXPRESS");
        }

        private static List<Usuario> _db = new List<Usuario>()
        {
            new Usuario(){Id=1,Nome="James Hetfild", Email="metallica@hotmail.com"},
            new Usuario(){Id=2,Nome="Joe Satriani", Email="joe@hotmail.com"},
            new Usuario(){Id=3,Nome="Michael Jackson", Email="mj@gmail.com"},
        };

        public void Delete(int id)
        {
            _db.Remove(_db.FirstOrDefault(x => x.Id == id));
        }

        public List<Usuario> Get()
        {
            return _db;
        }

        public Usuario Get(int id)
        {
            return _db.FirstOrDefault(x => x.Id == id);
        }

        public void Insert(Usuario usuario)
        {
            var ultimoUsuario = _db.LastOrDefault();

            if (ultimoUsuario == null)
            {
                usuario.Id = 1;
            }
            else
            {
                usuario.Id = ultimoUsuario.Id;
                usuario.Id++;
            }
            _db.Add(usuario);
        }

        public void Update(Usuario usuario)
        {
            _db.Remove(_db.FirstOrDefault(x => x.Id == usuario.Id));
            _db.Add(usuario);
        }
    }
}
