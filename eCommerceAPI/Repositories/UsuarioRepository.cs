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
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "select * from usuarios";
                command.Connection = (SqlConnection)_connection;

                _connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    usuario.Id = dataReader.GetInt32("id");
                    usuario.Nome = dataReader.GetString("name");
                    usuario.Email = dataReader.GetString("email");
                    usuario.Sexo = dataReader.GetString("Sexo");
                    usuario.RG = dataReader.GetString("RG");
                    usuario.CPF = dataReader.GetString("CPF");
                    usuario.NomeMae = dataReader.GetString("NomeMae");
                    usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                    usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                    usuarios.Add(usuario);
                }
            }
            finally
            {
                _connection.Close();
            }
            return usuarios;
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
