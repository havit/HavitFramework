# Change log

## 2.8.4 (27.3.2024)
* `DbContext.Set<TEntity>()` - mikrooptimalizace pro opakované volání
* `IDbSet<TEntity>().AsDbSet()`  - vrátí podkladový `DbSet<TEntity>` pro možnost použít extension metod, např. extension metody `RelationalQueryableExtensions` pro volání stored procedur
* `IDbSet<TEntity>().FindAsync(...)` - metoda odebrána, nebyla nikde použita

## 2.8.2 (14.2.2024)
* Aktualizace na EF Core 8.0.2

## 2.8.1 (10.1.2024)
* ModelValidator nyní nevyžaduje `MaxLengthAttribute` na comptuted sloupcích

## 2.8.0 (29.11.2023)
* Aktualizace na EF Core 8
