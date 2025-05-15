using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testy nejsou izolované.
// Testy běží celkem (sekvenčně) do dvou sekund.
[assembly: DoNotParallelize]
