Notes-Back-CS 📝

🚀 Sistema de gerenciamento de tarefas (To-Do List) - Backend - CSharp

📌 Sobre o projeto
Notes-Back-CS é um sistema de gerenciamento de tarefas desenvolvido em .NET Core 9 e utilizando SQL Server como banco de dados.
Este repositório contém toda a parte de backend, incluindo autenticação de usuários, manipulação de tarefas e integração com banco de dados.
O frontend está implementado em Angular, disponível em um repositório separado.

🏗️ Tecnologias utilizadas
✔ .NET Core 9 - Framework principal para desenvolvimento backend.
✔ SQL Server - Banco de dados para armazenamento das informações.
✔ Entity Framework (EF Core) + Migrations - Gerenciamento da camada de persistência.
✔ JWT (Json Web Token) - Autenticação e segurança.
✔ Swagger - Documentação da API.
✔ Moq & XUnit - Testes unitários.

🔧 Pré-requisitos
Antes de rodar o projeto, certifique-se de ter instalado:
- .NET Core 9
- SQL Server
- Visual Studio (para rodar os testes e compilar o projeto)

🚀 Como rodar o projeto
1️⃣ Clone este repositório:
git clone https://github.com/aldrinhronchi/Notes-Back-CS.git


2️⃣ Acesse o diretório do projeto:
cd Notes-Back-CS


3️⃣ Configure as variáveis de ambiente e appsettings.json com suas credenciais do banco de dados.
4️⃣ Execute as migrations para configurar o banco de dados:
dotnet ef database update


5️⃣ Rode a aplicação:
dotnet run


🧪 Testes
Todos os testes unitários estão na solução (.sln) dentro da aplicação Notes-Back-CS.Tests.
Para rodar os testes no Visual Studio, basta executar:
dotnet test


Isso garantirá que todas as validações do backend estejam funcionando corretamente antes da implementação final.
📌 Estrutura do projeto
📂 Connections → Contém configurações e conexão com o banco de dados.
📂 Controllers → Define os endpoints da API (Tarefas, Usuários).
📂 Models → Modelos de entidades, incluindo Usuario, Tarefa e ViewModels.
📂 Services → Implementação da lógica de negócios, separada por domínio.
📂 Tests → Testes unitários com Moq e XUnit.
📂 Extensions → Helpers e middlewares úteis para a aplicação.

📡 Endpoints da API
🔑 Autenticação
POST /api/Usuario/Autenticar → Autentica um usuário e retorna um JWT.
✅ Gerenciamento de Tarefas
GET /api/Tarefa/ListarTarefas → Retorna uma lista de tarefas do usuário.
POST /api/Tarefa/SalvarTarefa → Cria ou atualiza uma tarefa.
DELETE /api/Tarefa/ExcluirTarefa/{id} → Exclui uma tarefa específica.
👤 Gerenciamento de Usuários
GET /api/Usuario/ListarUsuarios → Lista usuários com filtros opcionais.
POST /api/Usuario/SalvarUsuario → Cria ou atualiza um usuário.
DELETE /api/Usuario/ExcluirUsuario/{id} → Remove um usuário do sistema.

📜 Licença
Este projeto está licenciado sob a AGPL-3.0 License.
🔗 Frontend
O frontend deste sistema está desenvolvido em Angular e disponível em outro repositório.
