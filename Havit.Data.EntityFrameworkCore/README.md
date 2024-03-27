# Change log

## 2.8.5
* `DbContext.Set<TEntity>()` - mikrooptimalizace pro opakované volání
* `IDbSet<TEntity>().FindAsync(...)` - metoda odebrána, nebyla nikde použita

## 2.8.2 (14.2.2024)
* Aktualizace na EF Core 8.0.2

## 2.8.1 (10.1.2024)
* ModelValidator nyní nevyžaduje `MaxLengthAttribute` na comptuted sloupcích

## 2.8.0 (29.11.2023)
* Aktualizace na EF Core 8
