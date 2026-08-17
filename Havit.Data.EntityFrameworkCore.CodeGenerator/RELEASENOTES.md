# Havit.Data.EntityFrameworkCore.CodeGenerator — Release Notes

## v2.10.4 (8/2026)

> ⚠️ **Vyžaduje Havit.Data.EntityFrameworkCore.CodeGenerator.Tool ≥ 2.10.4.**

- Balíček již nenastavuje `CopyLocalLockFileAssemblies=True` v projektu, kde je nainstalován (odebrán packovaný `.props` soubor).

- Balíček je nyní označen jako `DevelopmentDependency` — nová instalace vygeneruje referenci s `PrivateAssets="all"`.

  - **Dopad:** CodeGenerator ani jeho transitivní závislosti (zejména `Microsoft.EntityFrameworkCore.Design` a Roslyn — typicky ~80 % objemu publish výstupu na malém projektu) již netečou do aplikací, které Entity projekt referencují.
  - ⚠️ **Stávající projekty:** samotný update verze balíčku v csproj `PrivateAssets` nedoplní — u existujících referencí je potřeba atributy přidat ručně (nebo balíček odinstalovat a nainstalovat znovu). Bez `PrivateAssets` zůstává chování jako dřív (balíček se dostává do publish výstupu).
