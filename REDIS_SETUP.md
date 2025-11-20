# Redis Cache - Instrucciones de Configuración

## ?? Configuración Implementada

Se ha implementado Redis como sistema de caché distribuida con las siguientes características:

- **Expiración**: 20 segundos por defecto
- **Endpoints con caché**:
  - `GET /api/v1/Authors/with-books` ? Cache key: `authors:all`
  - `GET /api/v1/Books/ListBooks` ? Cache key: `books:all`
  - `GET /api/v1/Books/{id}` ? Cache key: `books:{id}`
  - `GET /api/v1/Customers` ? Cache key: `customers:all`

## ?? Configuración Local

### Opción 1: Redis con Docker (Recomendado)

```bash
docker run --name redis-coolibrary -p 6379:6379 -d redis:latest
```

### Opción 2: Redis en Windows

1. Descargar Redis desde: https://github.com/microsoftarchive/redis/releases
2. Instalar y ejecutar
3. Por defecto corre en `localhost:6379`

### Verificar Configuración

En `appsettings.Development.json`:
```json
"ConnectionStrings": {
  "Redis": "localhost:6379"
}
```

## ?? Configuración en Azure

### Crear Azure Cache for Redis

1. **Portal de Azure**:
   - Crear recurso ? Azure Cache for Redis
   - Nombre: `coollibrary-redis`
   - Pricing tier: Basic C0 (250 MB) para desarrollo, Standard/Premium para producción

2. **Obtener Connection String**:
   - Azure Portal ? Tu Redis Cache ? Access keys
   - Copiar "Primary connection string"

3. **Configurar en Azure App Service**:

   **Opción A: Variables de Entorno**
   ```
   ConnectionStrings__Redis = "tu-nombre.redis.cache.windows.net:6380,password=tu-password,ssl=True,abortConnect=False"
   ```

   **Opción B: Azure Key Vault** (Recomendado)
   - Guardar el connection string en Key Vault con el nombre `Redis-ConnectionString`
   - La aplicación ya está configurada para leer desde Key Vault

### Connection String de Azure Redis

Formato típico:
```
nombre.redis.cache.windows.net:6380,password=clave-primaria,ssl=True,abortConnect=False
```

## ?? Monitoreo

### Logs de Cache Hit/Miss

La aplicación registra en los logs cuando hace:
- **Cache HIT**: Datos obtenidos de Redis (más rápido)
- **Cache MISS**: Datos obtenidos de la base de datos (primera vez o después de 20 segundos)

Buscar en logs:
```
Cache miss for authors:all, fetching from database
Cache miss for books:all, fetching from database
```

### Verificar Redis (local)

```bash
# Conectar a Redis CLI
docker exec -it redis-coolibrary redis-cli

# Ver todas las keys
KEYS *

# Ver una key específica
GET CoolLibrary_authors:all

# Ver TTL (tiempo restante)
TTL CoolLibrary_authors:all
```

## ?? Beneficios

1. **Reducción de carga en SQL Server**: Las peticiones repetidas en 20 segundos se sirven desde Redis
2. **Mejora de rendimiento**: Redis es in-memory, mucho más rápido que SQL
3. **Escalabilidad**: Compatible con Azure Cache for Redis para producción
4. **Sin cambios en código**: Transparente para los clientes de la API

## ?? Invalidación de Caché

Actualmente el caché expira automáticamente a los 20 segundos. Para invalidación manual:

1. **Agregar método en servicios de creación/actualización/eliminación**:
```csharp
// Después de crear/actualizar/eliminar
await _cacheService.RemoveAsync("authors:all");
```

2. **Implementación futura**: Sistema de eventos para invalidar automáticamente

## ?? Ajustar Tiempo de Expiración

Modificar en `RedisCacheService.cs`:
```csharp
private readonly TimeSpan _defaultExpiration = TimeSpan.FromSeconds(20);
```

O pasar tiempo personalizado:
```csharp
await _cacheService.SetAsync(key, data, TimeSpan.FromMinutes(5));
```

## ?? Pruebas

1. Iniciar Redis (local o Azure)
2. Ejecutar la API
3. Llamar a `GET /api/v1/Authors/with-books` varias veces seguidas
4. Observar logs: Primera llamada = MISS, siguientes = HIT
5. Esperar 20 segundos
6. Llamar de nuevo: MISS (caché expirado)
