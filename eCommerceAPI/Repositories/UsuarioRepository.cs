using eCommerce.API.Models;
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
                    usuario.Id = dataReader.GetInt32("Id");
                    usuario.Nome = dataReader.GetString("nome");
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
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "select * from usuarios u left join contatos c " +
                                      "on c.usuarioId = u.id left join EnderecosEntrega e " +
                                      "on u.id = e.usuarioId left join UsuariosDepartamentos ud " +
                                      "on ud.UsuarioId = u.id " +
                                      "left join Departamentos d " +
                                      "on d.id = ud.DepartamentoId " +
                                      "where u.id = @id " +
                                      "select * from Departamentos";

                command.Parameters.AddWithValue("@id", id);
                command.Connection = (SqlConnection)_connection;

                _connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();

                Dictionary<int, Usuario> usuarios = new Dictionary<int, Usuario>();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    if (!(usuarios.ContainsKey(dataReader.GetInt32(0))))
                    {

                        usuario.Id = dataReader.GetInt32(0);
                        usuario.Nome = dataReader.GetString("nome");
                        usuario.Email = dataReader.GetString("email");
                        usuario.Sexo = dataReader.GetString("Sexo");
                        usuario.RG = dataReader.GetString("RG");
                        usuario.CPF = dataReader.GetString("CPF");
                        usuario.NomeMae = dataReader.GetString("NomeMae");
                        usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                        usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                        Contato contato = new Contato();
                        contato.Id = dataReader.GetInt32(9);
                        contato.UsuarioId = usuario.Id;
                        contato.Telefone = dataReader.GetString("telefone");
                        contato.Celular = dataReader.GetString("celular");

                        usuario.Contato = contato;

                        usuarios.Add(usuario.Id, usuario);
                    }
                    else
                    {
                        usuario = usuarios[dataReader.GetInt32(0)];
                    }




                    EnderecoEntrega enderecoEntrega = new EnderecoEntrega();

                    enderecoEntrega.Id = dataReader.GetInt32(13);
                    enderecoEntrega.UsuarioId = usuario.Id;
                    enderecoEntrega.NomeEndereco = dataReader.GetString("NomeEndereco");
                    enderecoEntrega.CEP = dataReader.GetString("cep");
                    enderecoEntrega.Estado = dataReader.GetString("estado");
                    enderecoEntrega.Cidade = dataReader.GetString("cidade");
                    enderecoEntrega.Bairro = dataReader.GetString("Bairro");
                    enderecoEntrega.Endereco = dataReader.GetString("endereco");
                    enderecoEntrega.Numero = dataReader.GetString("numero");
                    enderecoEntrega.Complemento = dataReader.GetString("complemento");

                    usuario.EnderecosEntrega = (usuario.EnderecosEntrega == null) ? new List<EnderecoEntrega>() : usuario.EnderecosEntrega;

                    if (usuario.EnderecosEntrega.FirstOrDefault(a => a.Id == enderecoEntrega.Id) == null)
                    {
                        usuario.EnderecosEntrega.Add(enderecoEntrega);
                    }

                    Departamento departamento = new Departamento();
                    departamento.Id = dataReader.GetInt32(26);
                    departamento.Nome = dataReader.GetString(27);

                    usuario.Departamentos = (usuario.Departamentos == null) ? new List<Departamento>() : usuario.Departamentos;

                    if (usuario.Departamentos.FirstOrDefault(a => a.Id == departamento.Id) == null)
                    {
                        usuario.Departamentos.Add(departamento);
                    }
                }
                return usuarios[usuarios.Keys.First()];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
            return null;
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            SqlTransaction transaction = (SqlTransaction)_connection.BeginTransaction();

            try
            {
                SqlCommand command = new SqlCommand();
                command.Transaction = transaction;
                command.Connection = (SqlConnection)_connection;

                command.CommandText = "insert into usuarios (nome,email,sexo,rg,cpf,nomeMae,situacaoCadastro,dataCadastro)" +
                    " values(@nome,@email,@sexo,@rg,@cpf,@nomeMae,@situacaoCadastro,@dataCadastro);select CAST(scope_identity() AS int)";




                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@rg", usuario.RG);
                command.Parameters.AddWithValue("@cpf", usuario.CPF);
                command.Parameters.AddWithValue("@nomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@situacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);


                usuario.Id = (int)command.ExecuteScalar();

                command.CommandText = "INSERT INTO contatos (usuarioId, telefone, celular) values(@usuarioId, @telefone, @celular);select CAST(scope_identity() AS int)";
                command.Parameters.AddWithValue("@usuarioId", usuario.Id);
                command.Parameters.AddWithValue("@telefone", usuario.Contato.Telefone);
                command.Parameters.AddWithValue("@celular", usuario.Contato.Celular);

                usuario.Contato.UsuarioId = usuario.Id;
                usuario.Contato.Id = (int)command.ExecuteScalar();

                foreach (var endereco in usuario.EnderecosEntrega)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "insert into enderecosEntrega (usuarioId, nomeEndereco,cep,estado,cidade,bairro,endereco,numero,complemento)" +
                        "values(@usuarioId, @nomeEndereco,@cep,@estado,@cidade,@bairro,@endereco,@numero,@complemento);select CAST(scope_identity() AS int)";
                    command.Parameters.AddWithValue("@usuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@nomeEndereco", endereco.NomeEndereco);
                    command.Parameters.AddWithValue("@cep", endereco.CEP);
                    command.Parameters.AddWithValue("@estado", endereco.Estado);
                    command.Parameters.AddWithValue("@cidade", endereco.Cidade);
                    command.Parameters.AddWithValue("@bairro", endereco.Bairro);
                    command.Parameters.AddWithValue("@endereco", endereco.Endereco);
                    command.Parameters.AddWithValue("@numero", endereco.Numero);
                    command.Parameters.AddWithValue("@complemento", endereco.Complemento);

                    endereco.Id = (int)command.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }

                foreach (var departamento in usuario.Departamentos)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "insert into usuariosDepartamentos (usuarioId, departamentoId) values (@usuarioId, @departamentoId)";
                    command.Parameters.AddWithValue("usuarioId", usuario.Id);
                    command.Parameters.AddWithValue("departamentoId", departamento.Id);

                    command.ExecuteScalar();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch(Exception e)
                {
                   
                }
            }
            finally
            {

                _connection.Close();
            }
        }

        public void Update(Usuario usuario)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "update usuarios set nome = @nome,email = @email,sexo = @sexo,rg = @rg,cpf = @cpf,nomeMae = @nomeMae,situacaoCadastro = @situacaoCadastro,dataCadastro = @dataCadastro where id = @id";

                command.Connection = (SqlConnection)_connection;

                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@rg", usuario.RG);
                command.Parameters.AddWithValue("@cpf", usuario.CPF);
                command.Parameters.AddWithValue("@nomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@situacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);

                command.Parameters.AddWithValue("@id", usuario.Id);

                _connection.Open();
                command.ExecuteNonQuery();

            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "delete usuarios where id = @id";
                command.Connection = (SqlConnection)_connection;

                command.Parameters.AddWithValue("@id",id);



                _connection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
