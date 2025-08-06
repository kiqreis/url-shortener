# 🔗 URL Shortener

Um encurtador de URLs moderno e escalável construído com .NET 9, ASP.NET Core, ReactJS, SQL Server, Redis e autenticação baseada em JWT. O projeto segue boas práticas como separação de responsabilidades, injeção de dependência, arquitetura limpa e testes automatizados. Foi projetado para ser performático, seguro e fácil de manter, com suporte planejado para CQRS (via Brighter) e mensageria com Azure Service Bus.

---

## 🧱 Tecnologias e Ferramentas

### 🔧 Backend (.NET)

- **.NET 9** – Framework moderno, robusto e performático.
- **ASP.NET Core Web API** – API RESTful com suporte a versionamento.
- **Entity Framework Core** – ORM com suporte a migrations.
- **SQL Server** – Banco de dados relacional em container Docker.
- **ASP.NET Identity + JWT** – Autenticação e autorização seguras.
- **Redis** – Cache distribuído para alta performance.
- **Swagger (Swashbuckle)** – Documentação interativa da API.
- **Shouldly + xUnit** – Testes legíveis e poderosos.
- **Docker + Docker Compose** – Containerização e orquestração.

### 💻 Frontend

- **ReactJS** – Interface reativa e componentizada.
- **TailwindCSS** – Estilização rápida e responsiva.
- **Fetch** – Comunicação com a API backend.
- **Suporte a JWT** – Login/logout e acesso autenticado.

### ⚙️ Infraestrutura

- **Docker Compose** – Orquestração local de containers (.NET, SQL Server, Redis).
- **Redis e SQL Server** – Serviços em containers otimizados.

### 🚧 Em Desenvolvimento

- **CQRS com Brighter** – Segregação de comandos e consultas.
- **Mensageria com Azure Service Bus** – Processamento assíncrono.
- **Testes expandidos** – Unitários, integração, carga e frontend.

---

## ✅ Funcionalidades Implementadas

### Backend

- 🔗 **Encurtamento de URLs**
  - Geração de chave única (`meudominio.com/abc123`)
  - Validação com `System.Uri`
  - Persistência com índice otimizado no SQL Server

- 🔐 **Autenticação e Autorização**
  - ASP.NET Identity com JWT
  - Endpoints protegidos por claims (`role`, `userId`)
  - Expiração configurável (padrão: 1 hora)

- ⚡ **Cache com Redis**
  - URLs encurtadas cacheadas por 24h
  - Redução de carga no banco

- 📘 **Documentação**
  - Swagger com exemplos e versionamento (`/my-api/v1`)

- 🧪 **Testes**
  - Unitários com xUnit + Shouldly
  - Integração com `WebApplicationFactory`

### Frontend

- 🖥️ **Interface**
  - Formulário com validação em tempo real
  - Exibição e cópia de URLs encurtadas
  - Responsividade com TailwindCSS

- 🔗 **Integração**
  - Consumo da API com tratamento de erros
  - Login/logout via JWT

### Infraestrutura

- 📦 **Containerização**
  - Backend, SQL Server e Redis via Docker Compose

- 🚀 **Performance**
  - Índices no SQL Server
  - Redis com TTL para acessos frequentes

---

## 🔧 Funcionalidades em Desenvolvimento

### 🧭 CQRS com Brighter
- Separação entre comandos e consultas
- Comandos: `CreateShortUrlCommand`, etc.
- Consultas: `GetShortUrlQuery`, etc.
- **Status:** Planejado

### 📤 Mensageria com Azure Service Bus
- Registro de cliques e eventos
- DLQ e tópicos para expiração/notificações
- **Status:** Planejado

### 🧪 Testes Expandidos
- Casos de borda, falhas de autenticação, etc.
- Testes de carga com `k6`
- Testes de frontend com `Jest` + `React Testing Library`
- **Status:** Parciais

### 📊 Estatísticas de Uso
- Métricas de cliques por dia, região, etc.
- Gráficos com Chart.js
- **Status:** Planejado

### 🧾 URLs Personalizadas
- Alias customizados (`meudominio.com/meu-alias`)
- Validação e prevenção de conflitos
- **Status:** Planejado

### ⌛ Expiração de URLs
- TTL customizável (ex.: 7 dias)
- Limpeza via job assíncrono
- **Status:** Planejado

---

## 🚀 Executando o Projeto

### Pré-requisitos

- Docker + Docker Compose
- .NET 9 SDK
- Node.js (LTS recomendado)

### Com Docker Compose

```bash
git clone https://github.com/kiqreis/url-shortener
cd url-shortener
docker-compose up --build
