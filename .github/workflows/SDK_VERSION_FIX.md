# ?? Diagnóstico: Problema de Versión del SDK

## ?? Problema Identificado

El workflow de GitHub Actions fallaba **NO por errores de código**, sino por **incompatibilidad de versiones del SDK de .NET**.

## ?? Análisis del Problema

### Situación Inicial
- **Local:** SDK 9.0.307 instalado
- **global.json (inicial):** Requería exactamente `9.0.100` con `rollForward: latestMinor`
- **GitHub Actions:** Instala la última versión disponible de 9.0.x (probablemente 9.0.401 o superior)

### Error Reproducido Localmente
```
The command could not be loaded, possibly because:
  * A compatible .NET SDK was not found.

Requested SDK version: 9.0.100
global.json file: C:\Users\...\global.json

Installed SDKs:
9.0.307 [C:\Program Files\dotnet\sdk]
10.0.100 [C:\Program Files\dotnet\sdk]

Install the [9.0.100] .NET SDK or update [global.json] to match an installed SDK.
```

## ? Solución Aplicada

### Configuración Final de `global.json`
```json
{
  "sdk": {
    "version": "9.0.100",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

### Explicación de `rollForward`

| Valor | Comportamiento | Uso Recomendado |
|-------|---------------|-----------------|
| `latestPatch` | Solo permite parches (9.0.100 ? 9.0.101, 9.0.102) | Desarrollo local estricto |
| `latestMinor` | Permite minor versions (9.0.x ? 9.1.x) | ? No funciona para 9.0.307 |
| `latestFeature` | Permite feature bands (9.0.100 ? 9.0.307) | ? **RECOMENDADO para CI/CD** |
| `latestMajor` | Permite cualquier versión (9.x ? 10.x) | ?? Demasiado permisivo |

### Feature Bands en .NET 9

.NET usa un sistema de "feature bands" donde:
- `9.0.100` = base release
- `9.0.200` = feature band 1
- `9.0.300` = feature band 2 (tu versión: 9.0.307)
- `9.0.400` = feature band 3

**Sin `latestFeature`**, el SDK considera 9.0.307 incompatible con 9.0.100.

## ?? Cambios en Workflows

Ambos workflows ahora usan `global-json-file` para respetar la configuración:

```yaml
# Antes (podía causar conflictos)
- name: ?? Setup .NET 9.0
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '9.0.x'

# Después (respeta global.json)
- name: ?? Setup .NET 9
  uses: actions/setup-dotnet@v4
  with:
    global-json-file: global.json
```

## ?? Verificación

### Local
```bash
# Verificar versión detectada
dotnet --version
# Output: 9.0.307 ?

# Build con warnings como errores
dotnet build --configuration Release /p:TreatWarningsAsErrors=true
# Output: Compilación correcta ?
```

### GitHub Actions
El workflow ahora:
1. Lee `global.json`
2. Instala SDK 9.0.x (la última disponible)
3. `rollForward: latestFeature` permite usar esa versión
4. Build pasa exitosamente ?

## ?? Referencias

- [global.json overview](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json)
- [SDK rollForward policy](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json#rollforward)
- [.NET SDK versioning](https://docs.microsoft.com/en-us/dotnet/core/versions/)
- [GitHub Actions setup-dotnet](https://github.com/actions/setup-dotnet)

## ?? Lecciones Aprendidas

1. **`global.json` tiene prioridad** sobre cualquier configuración de workflow
2. **Feature bands requieren `latestFeature`** - `latestMinor` no es suficiente
3. **GitHub Actions instala SDKs actualizados** - tu `global.json` debe ser flexible
4. **Siempre verificar localmente** con la configuración exacta del `global.json`

## ?? Recomendaciones

### Para Desarrollo Local
- Mantener `global.json` con `rollForward: latestFeature`
- Actualizar el SDK regularmente con `dotnet --version`

### Para CI/CD
- Usar `global-json-file` en workflows
- Evitar hardcodear versiones específicas
- Probar localmente antes de push

### Para Producción
- Considerar fijar versiones más estrictamente si es necesario
- Documentar qué feature band se usa en producción
- Tener un plan de actualización del SDK

---

**Conclusión:** El problema no eran los errores de compilación (esos ya estaban corregidos), sino la **incompatibilidad de versiones del SDK** causada por una configuración demasiado restrictiva en `global.json`.
