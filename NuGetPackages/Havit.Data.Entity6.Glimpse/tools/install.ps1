param($installPath, $toolsPath, $package, $project)

# Vyhodit GlimpseSecurityPolicy.cs, co přidává Glimpse do Web projektu
$secPolFile=$null;
try {
    $secPolFile = $project.ProjectItems.Item("GlimpseSecurityPolicy.cs");    
} 
catch {
    Write-Host $_.GetType().FullName
    Write-Host "GlimpseSecurityPolicy does not exists - skipping";
}
if ($secPolFile -ne $null) {
    $secPolFile.Delete();
}