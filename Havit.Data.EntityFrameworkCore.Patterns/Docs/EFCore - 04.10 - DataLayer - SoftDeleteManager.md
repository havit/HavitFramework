## SoftDeleteManager

Implementeace `ISoftDeleteManager` rozhodují o tom, zda daná entita podporuje soft delete a pokud ano, poskytuje metody pro nastavení příznaku smazání (a odebrání příznaku smazání).

Výchozí implementace `SoftDeleteManager` říká, že soft-delete jsou ty entity, které mají vlastnost `Deleted` typu `Nullable<DateTime>`.
