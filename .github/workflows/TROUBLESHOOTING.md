# ?? Troubleshooting GitHub Actions Workflows

## ? Error: "Resource not accessible by integration"

### Problema
```
Error: HttpError: Resource not accessible by integration
```

### Causa
El workflow no tiene permisos suficientes para crear check runs, comentarios o estados en pull requests.

### ? Solución Aplicada
Se han añadido los permisos necesarios en ambos workflows:

```yaml
permissions:
  contents: read          # Leer el código del repositorio
  checks: write          # Crear check runs
  pull-requests: write   # Comentar en PRs
  statuses: write        # Actualizar estados
```

### Verificación
1. Revisa que los workflows tengan la sección `permissions:` después del `on:`
2. Confirma que el GITHUB_TOKEN tiene los permisos necesarios
3. En Settings ? Actions ? General ? Workflow permissions, debe estar en:
   - ? "Read and write permissions" (recomendado)
   - O "Read repository contents and packages permissions" + permisos específicos

---

## ?? Error: Test Reporter Fails

### Problema Original
```
Run dorny/test-reporter@v1
Error: HttpError: Resource not accessible by integration
```

### ? Solución
Reemplazado `dorny/test-reporter@v1` con `EnricoMi/publish-unit-test-result-action@v2`:

**Antes:**
```yaml
- name: ?? Test Summary
  uses: test-summary/action@v2
  if: always()
  with:
    paths: "TestResults/**/*.trx"
```

**Después:**
```yaml
- name: ?? Publish Test Results
  uses: EnricoMi/publish-unit-test-result-action@v2
  if: always()
  with:
    files: '**/test-results.trx'
    check_name: 'Unit Test Results'
    comment_mode: off
```

**Ventajas:**
- ? Mejor soporte para pull requests
- ? No requiere permisos adicionales especiales
- ? Genera check runs automáticamente
- ? Compatible con .NET trx files

---

## ?? Problemas Comunes y Soluciones

### 1. **Workflow no se ejecuta en PRs**

**Síntoma:** El workflow no aparece en el PR

**Solución:**
```yaml
on:
  pull_request:
    branches: [ main, develop ]  # Asegúrate de incluir las ramas correctas
```

### 2. **Tests no se encuentran**

**Síntoma:** 
```
No test files were found.
```

**Verificación:**
```bash
# Verifica que el path al proyecto de tests sea correcto
dotnet test CoolLibraryTests/CoolLibraryUnitaryTests.csproj
```

**Solución:**
- Confirma el nombre del proyecto de tests
- Verifica que el path sea relativo a la raíz del repositorio
- Usa `find` para buscar archivos trx:
```yaml
- name: Debug - Find test files
  run: find . -name "*.trx" -print
```

### 3. **Coverage no se genera**

**Síntoma:** No se encuentra `coverage.cobertura.xml`

**Solución:**
```yaml
- name: ?? Run All Unit Tests with Coverage
  run: |
    dotnet test \
      --collect:"XPlat Code Coverage" \
      --results-directory ./TestResults
```

**Verificación:**
```yaml
- name: Debug - List coverage files
  run: find ./TestResults -name "coverage.cobertura.xml" -print
```

### 4. **Codecov Token Missing**

**Síntoma:**
```
Error: Codecov token not found
```

**Solución:**
1. Ve a https://codecov.io
2. Crea cuenta y añade tu repositorio
3. Obtén el token
4. En GitHub: Settings ? Secrets and variables ? Actions
5. New repository secret: `CODECOV_TOKEN` = tu token

**Alternativa (público):**
```yaml
- name: Upload Coverage
  uses: codecov/codecov-action@v4
  with:
    token: ${{ secrets.CODECOV_TOKEN }}  # Opcional para repos públicos
    fail_ci_if_error: false              # No fallar si Codecov falla
```

### 5. **Build Fails: warnaserror**

**Síntoma:**
```
Build FAILED.
```

**Causa:** `--warnaserror` trata warnings como errores

**Solución temporal:**
```yaml
# Cambiar
run: dotnet build --configuration Release --no-restore --warnaserror

# A
run: dotnet build --configuration Release --no-restore
```

**Solución permanente:** Corregir los warnings en el código

---

## ?? Debugging Workflows

### Ver logs detallados

```yaml
- name: ?? Run Tests (verbose)
  run: |
    dotnet test \
      --verbosity diagnostic \
      --logger "console;verbosity=detailed"
```

### Listar archivos generados

```yaml
- name: Debug - List all files
  if: always()
  run: |
    echo "=== TestResults directory ==="
    ls -laR ./TestResults || true
    echo "=== Find TRX files ==="
    find . -name "*.trx" -exec ls -lh {} \;
    echo "=== Find coverage files ==="
    find . -name "coverage.*.xml" -exec ls -lh {} \;
```

### Subir artefactos para inspección

```yaml
- name: Upload test results
  if: always()
  uses: actions/upload-artifact@v4
  with:
    name: test-results
    path: TestResults/
    retention-days: 5
```

---

## ? Checklist de Verificación

Antes de hacer push, verifica:

- [ ] Los workflows tienen la sección `permissions:`
- [ ] Los paths a los proyectos son correctos
- [ ] Las ramas en `on:` son correctas
- [ ] Los tests pasan localmente: `dotnet test`
- [ ] El build pasa localmente: `dotnet build --configuration Release`
- [ ] Los archivos YAML tienen sintaxis correcta (usa un linter)

---

## ?? Obtener Ayuda

1. **Revisa los logs completos:**
   - GitHub ? Actions ? Click en el workflow fallido ? Click en el step que falló

2. **Verifica la sintaxis YAML:**
   - Usa: https://www.yamllint.com/

3. **Ejecuta localmente:**
   ```bash
   # Simula el workflow localmente
   dotnet restore
   dotnet build --configuration Release
   dotnet test --configuration Release --logger "trx"
   ```

4. **Consulta la documentación:**
   - GitHub Actions: https://docs.github.com/en/actions
   - EnricoMi test reporter: https://github.com/EnricoMi/publish-unit-test-result-action

---

## ?? Cambios Aplicados en Este Fix

### Archivos Modificados:
1. ? `.github/workflows/ci.yml`
2. ? `.github/workflows/unit-tests.yml`

### Cambios Específicos:
1. ? Añadida sección `permissions:` con permisos necesarios
2. ? Reemplazado `dorny/test-reporter@v1` con `EnricoMi/publish-unit-test-result-action@v2`
3. ? Reemplazado `test-summary/action@v2` con `EnricoMi/publish-unit-test-result-action@v2`
4. ? Añadido `comment_mode: off` para evitar spam en PRs
5. ? Mantenidos los debug steps para troubleshooting futuro

---

**?? Próximo Paso:** Haz commit y push de estos cambios para que los workflows funcionen correctamente en tus PRs.
