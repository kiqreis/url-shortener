🔗 URL Shortener
Um encurtador de URLs moderno e escalável construído com .NET 9, ASP.NET Core, ReactJS, SQL Server, Redis e autenticação baseada em JWT. O projeto segue boas práticas de desenvolvimento, como separação de responsabilidades, injeção de dependência, arquitetura limpa e testes automatizados. Ele é projetado para ser performático, seguro e fácil de manter, com suporte a futuras expansões como CQRS (via Brighter) e mensageria com Azure Service Bus.

🧱 Tecnologias e Ferramentas
Backend (.NET)

.NET 9: Framework para desenvolvimento robusto e performático.
ASP.NET Core Web API: API RESTful com suporte a endpoints versionados.
Entity Framework Core: ORM com suporte a migrations para gerenciamento do banco de dados.
SQL Server: Banco relacional rodando em container Docker.
ASP.NET Identity com JWT Bearer Authentication: Autenticação e autorização seguras com tokens JWT.
Redis: Cache distribuído para URLs encurtadas e consultas frequentes.
Swagger (Swashbuckle): Documentação interativa da API.
Shouldly: Usado para validação de entrada (ex.: URLs, parâmetros) e facilitação de asserções em testes.
xUnit: Framework de testes unitários e de integração.
Docker + Docker Compose: Containerização para desenvolvimento e deploy.

Frontend

ReactJS: Biblioteca para construção de interfaces reativas e componentizadas.
TailwindCSS: Framework CSS para estilização rápida e responsiva.

DevOps / Infraestrutura

Docker: Containerização do backend, SQL Server e Redis.
Docker Compose: Orquestração de containers para desenvolvimento local.
Redis: Cache distribuído para alta performance.
SQL Server: Banco de dados relacional para persistência.

Em desenvolvimento

CQRS com Brighter: Padrão para segregação de comandos e consultas, usando a biblioteca Brighter.
Mensageria com Azure Service Bus: Processamento assíncrono de eventos (ex.: cliques em URLs).
Testes expandidos com xUnit e Shouldly: Cobertura de casos de borda e integração.


🚀 Features Implementadas
Backend

Encurtamento de URLs:
Geração de chaves únicas (ex.: meudominio.com/abc123) para URLs longas.
Validação de URLs com System.Uri para garantir formato válido.
Persistência no SQL Server com índices otimizados na coluna de chaves.


Autenticação e Autorização:
ASP.NET Identity com JWT para autenticação de usuários.
Endpoints protegidos com políticas baseadas em claims (ex.: role, userId).
Configuração de expiração de tokens (padrão: 1 hora).


Cache com Redis:
Cache de URLs encurtadas para reduzir consultas ao banco.
Configuração de TTL (Time-To-Live) de 24 horas para entradas de cache.


Documentação da API:
Swagger com endpoints documentados e exemplos de requisições/respostas.
Suporte a versionamento (/my-api/v1/).


Testes:
Testes unitários com xUnit e Shouldly para lógica de negócio (ex.: geração de chaves, validação de URLs).
Testes de integração com WebApplicationFactory para simular chamadas à API.
Shouldly utilizado para asserções legíveis e validação de entradas (ex.: URLs malformadas).



Frontend

Interface de usuário:
Formulário para encurtar URLs com validação em tempo real.
Exibição de URLs encurtadas com opção de cópia para a área de transferência.
Estilização responsiva com TailwindCSS.


Integração com API:
Requisições via fetch com tratamento de erros e feedback visual.
Suporte a autenticação via JWT (login/logout).



Infraestrutura

Containerização:
Docker Compose para orquestrar SQL Server, Redis e a aplicação .NET.
Imagens otimizadas para build e deploy.


Performance:
Índices no SQL Server para consultas rápidas.
Cache Redis para reduzir latência em acessos frequentes.




🔧 Features em Desenvolvimento

CQRS com Brighter:
Implementação do padrão Command Query Responsibility Segregation usando a biblioteca Brighter.
Comandos para criação/edição de URLs (ex.: CreateShortUrlCommand) e consultas para recuperação (ex.: GetShortUrlQuery).
Benefício: Separação clara entre operações de escrita e leitura, permitindo escalabilidade independente.
Status: Planejado, com definição inicial de comandos e handlers.


Mensageria com Azure Service Bus:
Fila para processamento assíncrono de eventos, como registro de cliques em URLs.
Configuração de Dead Letter Queue (DLQ) para mensagens com falhas.
Tópicos para eventos como expiração de URLs ou notificações.
Status: Planejado, com integração inicial em testes.


Testes expandidos:
Aumento da cobertura de testes com xUnit e Shouldly para cenários de borda (ex.: URLs inválidas, falhas de autenticação).
Testes de carga com ferramentas como k6 para simular tráfego intenso.
Testes de frontend com Jest e React Testing Library para componentes React.
Status: Testes unitários básicos implementados; testes de carga e frontend pendentes.


Estatísticas de uso:
Endpoint para exibir métricas de cliques (ex.: por dia, região, dispositivo).
Integração com Chart.js no frontend para visualização de gráficos.
Status: Planejado, com modelagem inicial de dados.


URLs personalizadas:
Suporte a aliases personalizados para URLs encurtadas (ex.: meudominio.com/meu-alias).
Validação de unicidade e prevenção de conflitos.
Status: Planejado.


Expiração de URLs:
Configuração de TTL customizável para URLs (ex.: válidas por 7 dias).
Limpeza automática de URLs expiradas via job assíncrono.
Status: Planejado.




🚀 Executando o Projeto
Pré-requisitos

Docker e Docker Compose instalados.
.NET 9 SDK para desenvolvimento local.
Node.js (versão LTS recomendada).

Subindo com Docker Compose

Clone o repositório: git clone (https://github.com/kiqreis/url-shortener)
cd url-shortener


Execute o Docker Compose:docker-compose up --build


Acesse a aplicação:
API: http://localhost:5000/swagger
Frontend: http://localhost:3000



Executando localmente (sem Docker)

Configure o SQL Server e Redis localmente ou use containers:docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
docker run -p 6379:6379 -d redis


Configure as variáveis de ambiente no arquivo appsettings.json (ex.: connection strings, chaves JWT).
Execute o backend:cd backend
dotnet run


Execute o frontend:cd frontend
npm install
npm start



Executando testes

Testes unitários e de integração:cd backend
dotnet test


Testes de frontend (futuro):cd frontend
npm test




🛠️ Configuração Avançada

Redis: Configure TTL no appsettings.json (ex.: "Redis:CacheTTL": "24h").
SQL Server: Adicione índices manuais para colunas de alta consulta (ex.: ShortUrlKey).
Swagger: Acesse a documentação em /swagger após iniciar a API.


📚 Próximos Passos

CI/CD: Configurar pipeline com GitHub Actions para build, testes e deploy.
Monitoramento: Integrar Prometheus/Grafana para métricas de performance.
Segurança: Adicionar rate limiting com AspNetCoreRateLimit e sanitização de URLs com DOMPurify no frontend.
Escalabilidade: Migrar para Kubernetes em produção.


🤝 Contribuições
Contribuições são bem-vindas! Siga estas etapas:

Faça um fork do repositório.
Crie uma branch para sua feature (git checkout -b feature/nova-feature).
Commit suas mudanças (git commit -m "Adiciona nova feature").
Envie para o repositório remoto (git push origin feature/nova-feature).
Abra um Pull Request.


📜 Licença
Este projeto está licenciado sob a MIT License.
