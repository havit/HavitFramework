# Havit.Business běží na .NET Frameworku, měříme proto net48 i net10.0 (net48 vyžaduje Windows).
dotnet run -c Release -f net10.0 -- --filter * --job short --runtimes net48 net10.0
