# Order Management System

Um sistema simples de gestão de pedidos, desenvolvido com **.NET 7+ (C#)**, **Entity Framework**, **PostgreSQL**, **React** com **TailwindCSS** e **Azure Service Bus**.
Permite criar, listar, editar e excluir pedidos, com processamento assíncrono via mensageria.

---

## Funcionalidades

- **Backend (API RESTful em C#):**
  - Criar, listar, editar e excluir pedidos.
  - Persistência em banco PostgreSQL.
  - Envio de eventos para o Azure Service Bus ao criar pedidos.
  - Worker que processa pedidos assincronamente e atualiza o status.

- **Frontend (React + TailwindCSS):**
  - Tabela responsiva de pedidos.
  - Formulário para criar pedidos.
  - Modal para editar pedidos.
  - Botões de ação com ícones.
  - Atualização automática da lista.

---

## Tecnologias Utilizadas

- **Backend:** .NET 7+, Entity Framework Core, PostgreSQL, Azure Service Bus
- **Frontend:** React, TypeScript, TailwindCSS, Axios
- **Infra:** Docker (opcional)
- **Mensageria:** Azure Service Bus

---

## Pré-requisitos

- [.NET 7 SDK ou superior](https://dotnet.microsoft.com/download)
- [Node.js 18+ e npm](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/download/)
- Conta no [Azure Service Bus](https://portal.azure.com/)
- (Opcional) [Docker](https://www.docker.com/)

---

## Configuração do Banco de Dados

1. Instale e inicie o PostgreSQL.
2. Crie um banco chamado `OrderManagement`.
3. Anote o usuário e senha do PostgreSQL.

---

## Configuração do Azure Service Bus

1. No portal do Azure, crie um **Namespace** do Service Bus.
2. Crie uma **fila** chamada `orders`.
3. Em "Políticas de acesso compartilhado", copie a **Connection String** de `RootManageSharedAccessKey`.

---

## Configuração do Backend

1. Acesse a pasta do backend:
   ```sh
   cd OrderManagementSystem.API
   ```
2. Edite o arquivo `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=OrderManagement;Username=postgres;Password=SUA_SENHA",
       "ServiceBus": "Endpoint=sb://SEU_NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SUA_KEY"
     },
     "ServiceBus": {
       "QueueName": "orders"
     },
     ...
   }
   ```
   - Substitua `SUA_SENHA` e `SUA_KEY` pelos seus dados reais.

3. **Rode as migrations** para criar as tabelas:
   ```sh
   dotnet tool install --global dotnet-ef # (se ainda não tiver)
   dotnet ef database update --project OrderManagementSystem.API/OrderManagementSystem.API.csproj
   ```

4. **Inicie a API:**
   ```sh
   dotnet run --project OrderManagementSystem.API/OrderManagementSystem.API.csproj
   ```
   - A API estará disponível em `http://localhost:5228` (ou porta configurada).

5. **Inicie o Worker:**
   ```sh
   dotnet run --project OrderManagementSystem.Worker/OrderManagementSystem.Worker.csproj
   ```

---

## Configuração do Frontend

1. Acesse a pasta do frontend:
   ```sh
   cd order-management-frontend
   ```

2. Instale as dependências:
   ```sh
   npm install
   ```

3. **Ajuste o endereço da API** (se necessário) em `src/services/api.ts`:
   ```ts
   const api = axios.create({
     baseURL: 'http://localhost:5228/api'
   });
   ```

4. Inicie o frontend:
   ```sh
   npm start
   ```
   - O sistema abrirá em `http://localhost:3000`.

---

## Como Usar

- **Criar Pedido:** Preencha o formulário e clique em "Criar Pedido".
- **Editar Pedido:** Clique no ícone de lápis ao lado do pedido, edite e salve.
- **Excluir Pedido:** Clique no ícone de lixeira ao lado do pedido.
- **Status:** O status do pedido é atualizado automaticamente pelo worker após o processamento.

---

## Docker (Opcional)

Se quiser rodar tudo via Docker, crie um arquivo `docker-compose.yml` com os serviços (PostgreSQL, API, Worker, Frontend) e rode:
```sh
docker-compose up --build
```

---

## Dicas e Solução de Problemas

- **Erro de conexão com o banco:** Verifique usuário, senha e se o PostgreSQL está rodando.
- **Erro de Service Bus:** Confirme a connection string e se a fila existe.
- **Página em branco no frontend:** Rode `npm install` novamente e reinicie o servidor.
- **Migrations:** Sempre rode as migrations após alterar modelos.

---

## Estrutura do Projeto

```
CaseTMB/
├── OrderManagementSystem.API/         # Backend .NET
├── OrderManagementSystem.Worker/      # Worker para processamento assíncrono
├── order-management-frontend/         # Frontend React + TailwindCSS
```

---

## Contato

Dúvidas ou sugestões? Abra uma issue ou entre em contato! 