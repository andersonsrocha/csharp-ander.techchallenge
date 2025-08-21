FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar o arquivo da solução
COPY TechChallenge.sln ./

# Copy project files
COPY src/TechChallenge.Api/TechChallenge.Api.csproj src/TechChallenge.Api/
COPY src/TechChallenge.Application/TechChallenge.Application.csproj src/TechChallenge.Application/
COPY src/TechChallenge.Data/TechChallenge.Data.csproj src/TechChallenge.Data/
COPY src/TechChallenge.Domain/TechChallenge.Domain.csproj src/TechChallenge.Domain/
COPY src/TechChallenge.Security/TechChallenge.Security.csproj src/TechChallenge.Security/
COPY tests/TechChallenge.Application.Test/TechChallenge.Application.Test.csproj tests/TechChallenge.Application.Test/

# Realizar o restore
RUN dotnet restore

# Copiar arquivos
COPY src/ src/
COPY tests/ tests/

# Construir o projeto
RUN dotnet build -c Release --no-restore

# Publicar o projeto
RUN dotnet publish src/TechChallenge.Api/TechChallenge.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Criar non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copiar os arquivos publicados
COPY --from=build /app/publish .

# Trocar ownership para non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Expor a porta
EXPOSE 80

ENTRYPOINT ["dotnet", "TechChallenge.Api.dll"]
