# Havit.Data.EntityFrameworkCore.CodeGenerator — Release Notes

## v2.10.4-pre01 (8/2026)

> ⚠️ **Vyžaduje Havit.Data.EntityFrameworkCore.CodeGenerator.Tool ≥ 2.10.4.**

- Balíček již nenastavuje `CopyLocalLockFileAssemblies=True` v projektu, kde je nainstalován (odebrán packovaný `.props` soubor).
  - **Dopad:** `bin` složka Entity projektu se výrazně zmenší — nebudou se do ní při buildu kopírovat assemblies všech NuGet balíčků (EF Core, Microsoft.EntityFrameworkCore.Design, Roslyn, …). Zrychlí se tím build Entity projektu.
  - **Upozornění:** Pokud jste mimo code generator spoléhali na self-contained `bin` Entity projektu (vlastní skripty, LINQPad apod.), nastavte si `<CopyLocalLockFileAssemblies>True</CopyLocalLockFileAssemblies>` explicitně v csproj Entity projektu.
- Balíček je nyní označen jako `DevelopmentDependency` — nová instalace vygeneruje referenci s `PrivateAssets="all"`:

  ```xml
  <PackageReference Include="Havit.Data.EntityFrameworkCore.CodeGenerator" Version="...">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  ```

  - **Dopad:** CodeGenerator ani jeho transitivní závislosti (zejména `Microsoft.EntityFrameworkCore.Design` a Roslyn — typicky ~80 % objemu publish výstupu na malém projektu) již netečou do aplikací, které Entity projekt referencují.
  - **Stávající projekty:** samotný update verze balíčku v csproj `PrivateAssets` nedoplní — u existujících referencí je potřeba atributy přidat ručně (nebo balíček odinstalovat a nainstalovat znovu). Bez `PrivateAssets` zůstává chování jako dřív (balíček teče do publish výstupu).
