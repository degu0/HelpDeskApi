# 📞 API de Gerenciamento de Chamadas

<div align="center">

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/postgresql-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)

API REST robusta e escalável para gerenciamento de chamadas de suporte/atendimento empresarial, construída com as melhores práticas de desenvolvimento e segurança.

[Características](#-características) •
[Tecnologias](#-tecnologias-utilizadas) •
[Instalação](#-instalação) •
[Uso](#-como-usar) •
[API Docs](#-documentação-da-api) •
[Testes](#-testes)

</div>

---

## 📋 Sumário

- Sobre o Projeto
- Características
- Tecnologias Utilizadas
- Pré-requisitos
- Instalação
- Como Usar
- Documentação da API

---

## 🎯 Sobre o Projeto

Esta API foi desenvolvida para facilitar o gerenciamento de chamadas de suporte e atendimento em ambientes corporativos. Com ela, é possível criar, visualizar, atualizar e deletar chamadas, além de contar com um sistema robusto de autenticação e autorização.

### Problema que Resolve

- Centralização do gerenciamento de chamadas de suporte
- Controle de acesso seguro via JWT
- Rastreabilidade completa de todas as operações
- Escalabilidade através de containerização

### Casos de Uso

- Help Desk corporativo
- Suporte técnico ao cliente
- Gerenciamento de tickets de atendimento
- Sistema de chamados internos

---

## ✨ Características

- ✅ **Autenticação JWT** - Sistema seguro de login e autorização
- ✅ **CRUD Completo** - Operações completas para gerenciamento de chamadas
- ✅ **Validação de Dados** - Validação robusta de entrada de dados
- ✅ **Migrations** - Controle de versão do banco de dados
- ✅ **Containerização** - Deploy facilitado com Docker
- ✅ **Testes Automatizados** - Cobertura de testes com xUnit
- ✅ **API RESTful** - Seguindo os padrões REST
- ✅ **Documentação Swagger** - Interface interativa para testar endpoints
- ✅ **Tratamento de Erros** - Respostas de erro padronizadas e informativas
- ✅ **Logging** - Sistema de logs para auditoria e debug

---

## 🚀 Tecnologias Utilizadas

### Backend
- **[C#](https://docs.microsoft.com/en-us/dotnet/csharp/)** - Linguagem de programação principal
- **[ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)** - Framework web para construção da API
- **[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)** - ORM para acesso ao banco de dados

### Banco de Dados
- **[PostgreSQL](https://www.postgresql.org/)** - Banco de dados relacional robusto e open-source

### Segurança
- **[JWT (JSON Web Tokens)](https://jwt.io/)** - Autenticação stateless e segura
- **[BCrypt](https://github.com/BcryptNet/bcrypt.net)** - Hash de senhas

### DevOps
- **[Docker](https://www.docker.com/)** - Containerização da aplicação
- **[Docker Compose](https://docs.docker.com/compose/)** - Orquestração de containers

### Testes
- **[xUnit](https://xunit.net/)** - Framework de testes unitários
- **[Moq](https://github.com/moq/moq4)** - Library para mocking em testes

### Ferramentas Adicionais
- **[Swagger/OpenAPI](https://swagger.io/)** - Documentação interativa da API
- **[Serilog](https://serilog.net/)** - Logging estruturado

---

## 📦 Pré-requisitos

Certifique-se de ter as seguintes ferramentas instaladas em sua máquina:

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download) ou superior
- [Docker](https://www.docker.com/get-started) (versão 20.10+)
- [Docker Compose](https://docs.docker.com/compose/install/) (versão 1.29+)
- [Git](https://git-scm.com/)
- [PostgreSQL 13+](https://www.postgresql.org/download/) (opcional, se não usar Docker)

---

## 🔧 Instalação

### 1️⃣ Clone o Repositório

```bash
git clone https://github.com/seu-usuario/api-chamadas.git
```

### 2️⃣ Opção A: Rodar com Docker (Recomendado)

```bash
# Construir e iniciar os containers
docker-compose up -d --build

# Verificar se os containers estão rodando
docker-compose ps

# Ver logs
docker-compose logs -f
```

A API estará disponível em: `http://localhost:5000`

### 3️⃣ Opção B: Rodar Localmente (Sem Docker)

#### Configure o PostgreSQL

Certifique-se de que o PostgreSQL está rodando e crie o banco de dados:

```sql
CREATE DATABASE chamadas_db;
```

#### Configure a Connection String

Edite o arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=chamadas_db;Username=postgres;Password=sua_senha"
  }
}
```

#### Execute as Migrations

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update

# Ou criar uma nova migration se necessário
dotnet ef migrations add InitialCreate
```

#### Inicie a Aplicação

```bash
dotnet run
```

A API estará disponível em: `http://localhost:5035`

---

## 💻 Como Usar

### Acessar a Documentação Swagger

Após iniciar a aplicação, acesse a documentação interativa:

```
http://localhost:5035/swagger
```

Devido a problemas no uso da biblioteca Swashbuckle com o .NET 10, é recomendável utilizar ferramentas dedicadas para testes de API, como Postman ou Insomnia.

---

# 📋 Documentação da API

## 🏢 Department (Departamentos)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Department` | Lista todos os departamentos |
| `POST` | `/api/Department` | Cria um novo departamento |
| `GET` | `/api/Department/{id}` | Busca um departamento específico por ID |

---

## 🎫 Ticket (Chamados)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Ticket` | Lista todos os tickets |
| `POST` | `/api/Ticket` | Cria um novo ticket |
| `GET` | `/api/Ticket/{id}` | Busca um ticket específico por ID |
| `GET` | `/api/Ticket/department` | Lista tickets por departamento |
| `PATCH` | `/api/Ticket/assign/{ticketId}` | Atribui um ticket a um agente |
| `GET` | `/api/Ticket/user/created` | Lista tickets criados pelo usuário |
| `GET` | `/api/Ticket/user/assign` | Lista tickets atribuídos ao usuário |
| `GET` | `/api/Ticket/status/{status}` | Lista tickets por status |
| `PATCH` | `/api/Ticket/{ticketId}/status` | Atualiza o status de um ticket |
| `PATCH` | `/api/Ticket/{ticketId}/closed` | Fecha um ticket |
| `PATCH` | `/api/Ticket/{ticketId}/reopen` | Reabre um ticket fechado |
| `PATCH` | `/api/Ticket/{ticketId}/archive` | Arquiva um ticket |
| `PATCH` | `/api/Ticket/{ticketId}/transfer/{newAgentId}` | Transfere ticket para outro agente |
| `GET` | `/api/Ticket/user` | Lista tickets do usuário |

---

## 👤 User (Usuários)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/User` | Lista todos os usuários |
| `POST` | `/api/User` | Cria um novo usuário |
| `GET` | `/api/User/{id}` | Busca um usuário específico por ID |
| `GET` | `/api/User/me` | Retorna dados do usuário autenticado |
| `POST` | `/api/User/login` | Realiza login do usuário |

---

### Diretrizes de Contribuição

- Siga os padrões de código C# e .NET
- Escreva testes para novas funcionalidades
- Atualize a documentação quando necessário
- Mantenha commits atômicos e com mensagens descritivas

---

## 🙏 Agradecimentos

<div align="center">

**⭐ Se este projeto foi útil para você, considere dar uma estrela!**

Made with ❤️ by Deyvid Gustavo

</div>
