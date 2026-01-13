## Metadata (maximální délky stringů)

Na základě modelu jsou pro všechny stringové vlastnosti generována metadata s definicí jejich maximálních délek dle attributu `[MaxLength]` (viz Entity Framework Core - 02 - Model). Pro vlastnosti označované jako "maximální možná délka" se použije hodnota Int32.MaxValue, byť to není správně (nejde uložit tolik znaků, ale tolik byte). Jiná metadata negenerujeme.

Metadata jsou generována přímo do modelu a jsou určena pro definici maximálních délek např. ve view modelu. Změnou délky textu v modelu, se po přegenerování kódu změní vygenerované konstanty, které změní maximální velikosti viewmodelu...

#### Příklad

```csharp
public static class LanguageMetadata
{
    public const int CultureMaxLength = 10;
    public const int NameMaxLength = 200;
    public const int SymbolMaxLength = 50;
    public const int UiCultureMaxLength = 10;
}
```